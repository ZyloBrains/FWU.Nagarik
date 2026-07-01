using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Models;

namespace FWU.Nagarik.Api.Services;

public interface ICsvTranscriptImportService
{
    Task<ImportResult> ImportAsync(Stream csvStream, string uploadedBy);
}

public class ImportResult
{
    public int StudentsCreated { get; set; }
    public int StudentsUpdated { get; set; }
    public int TranscriptsCreated { get; set; }
    public int TranscriptsUpdated { get; set; }
    public int SemestersCreated { get; set; }
    public int SubjectsCreated { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class CsvTranscriptImportService : ICsvTranscriptImportService
{
    private readonly AppDbContext _dbContext;

    private static readonly Dictionary<string, double> GradeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["A"] = 4.0,
        ["A-"] = 3.7,
        ["B+"] = 3.3,
        ["B"] = 3.0,
        ["B-"] = 2.7,
        ["C+"] = 2.3,
        ["C"] = 2.0,
        ["C-"] = 1.7,
        ["D+"] = 1.3,
        ["D"] = 1.0,
        ["F"] = 0.0,
        ["NG"] = 0.0,
        ["I"] = 0.0,
    };

    private static readonly Dictionary<int, string> SemesterNames = new()
    {
        [1] = "First Semester",
        [2] = "Second Semester",
        [3] = "Third Semester",
        [4] = "Fourth Semester",
        [5] = "Fifth Semester",
        [6] = "Sixth Semester",
        [7] = "Seventh Semester",
        [8] = "Eighth Semester",
        [9] = "Ninth Semester",
        [10] = "Tenth Semester",
        [11] = "Eleventh Semester",
        [12] = "Twelfth Semester",
    };

    private static readonly Dictionary<string, int> RomanToYear = new(StringComparer.OrdinalIgnoreCase)
    {
        ["I"] = 0,
        ["II"] = 1,
        ["III"] = 2,
        ["IV"] = 3,
        ["V"] = 4,
        ["VI"] = 5,
    };

    public CsvTranscriptImportService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ImportResult> ImportAsync(Stream csvStream, string uploadedBy)
    {
        var result = new ImportResult();

        using var reader = new StreamReader(csvStream, Encoding.UTF8);
        var lines = ReadCsvLines(reader);

        if (lines.Count < 2)
        {
            result.Errors.Add("CSV file is empty or has no data rows.");
            return result;
        }

        var headers = lines[0];
        var headerMap = BuildHeaderMap(headers);

        var rows = lines.Skip(1)
            .Where(r => r.Length > 0 && !string.IsNullOrWhiteSpace(r[0]))
            .ToList();

        var studentGroups = rows
            .GroupBy(r => GetField(r, headerMap, "RegistrationNo"))
            .ToList();

        foreach (var studentGroup in studentGroups)
        {
            try
            {
                var regdNo = studentGroup.Key;
                var firstRow = studentGroup.First();

                var student = await ProcessStudentAsync(regdNo, firstRow, headerMap, uploadedBy, result);
                if (student == null) continue;

                await ProcessTranscriptAsync(student, studentGroup.ToList(), headerMap, result);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Error processing student '{studentGroup.Key}': {ex.Message}");
            }
        }

        await _dbContext.SaveChangesAsync();
        return result;
    }

    private async Task<Student?> ProcessStudentAsync(string regdNo, string[] row, Dictionary<string, int> headerMap, string uploadedBy, ImportResult result)
    {
        var fullName = GetField(row, headerMap, "FullName");
        var (firstName, middleName, lastName) = ParseFullName(fullName);

        var existing = await _dbContext.Students.FirstOrDefaultAsync(s => s.RegdNo == regdNo);

        if (existing == null)
        {
            var student = new Student
            {
                RegdNo = regdNo,
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                ProgramName = GetField(row, headerMap, "ProgramName"),
                Faculty = GetField(row, headerMap, "FacultyName"),
                CampusName = GetField(row, headerMap, "CollegeName"),
                CampusLocation = GetField(row, headerMap, "CollegeName"),
                StudentStatus = "Active",
                IntakeYear = ExtractIntakeYear(regdNo),
                Level = "Bachelor",
                CourseDuration = "4 Academic Years (8 Semesters)",
                DobAD = string.Empty,
            };

            _dbContext.Students.Add(student);
            result.StudentsCreated++;
            return student;
        }

        existing.FirstName = firstName;
        existing.MiddleName = middleName;
        existing.LastName = lastName;
        existing.ProgramName = GetField(row, headerMap, "ProgramName");
        existing.Faculty = GetField(row, headerMap, "FacultyName");
        existing.CampusName = GetField(row, headerMap, "CollegeName");

        var cgpaStr = GetField(row, headerMap, "CGPA");
        if (double.TryParse(cgpaStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var cgpa))
            existing.CgpaScore = cgpa;

        result.StudentsUpdated++;
        return existing;
    }

    private async Task ProcessTranscriptAsync(Student student, List<string[]> rows, Dictionary<string, int> headerMap, ImportResult result)
    {
        var issueNoStr = GetField(rows.First(), headerMap, "IssueNo");
        if (!int.TryParse(issueNoStr, out var issueSerialNo))
        {
            result.Errors.Add($"Invalid IssueNo '{issueNoStr}' for student '{student.RegdNo}'.");
            return;
        }

        var existingTranscript = await _dbContext.Transcripts
            .FirstOrDefaultAsync(t => t.RegdNo == student.RegdNo && t.IssueSerialNo == issueSerialNo);

        Transcript transcript;
        if (existingTranscript == null)
        {
            transcript = new Transcript
            {
                RegdNo = student.RegdNo,
                IssueSerialNo = issueSerialNo,
                IssueDate = DateTime.UtcNow,
                IsPrinted = false,
                InstitutionId = await GetInstitutionIdAsync(),
            };
            _dbContext.Transcripts.Add(transcript);
            result.TranscriptsCreated++;
        }
        else
        {
            transcript = existingTranscript;
            result.TranscriptsUpdated++;
        }

        await _dbContext.SaveChangesAsync();

        var semesterGroups = rows
            .GroupBy(r =>
            {
                var yearStr = GetField(r, headerMap, "year");
                var partStr = GetField(r, headerMap, "part");
                var yearIdx = RomanToYear.GetValueOrDefault(yearStr.Trim(), 0);
                var partIdx = RomanToYear.GetValueOrDefault(partStr.Trim(), 0);
                return (Year: yearIdx, Part: partIdx);
            })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Part)
            .ToList();

        foreach (var semesterGroup in semesterGroups)
        {
            await ProcessSemesterAsync(transcript, semesterGroup.ToList(), headerMap,
                semesterGroup.Key.Year, semesterGroup.Key.Part, result);
        }
    }

    private async Task ProcessSemesterAsync(Transcript transcript, List<string[]> rows, Dictionary<string, int> headerMap,
        int yearIdx, int partIdx, ImportResult result)
    {
        var semesterNumber = (yearIdx * 2) + partIdx + 1;
        var semesterName = SemesterNames.GetValueOrDefault(semesterNumber, $"Semester {semesterNumber}");
        var academicYear = GetField(rows.First(), headerMap, "AcademicYearName");
        var examRollNo = GetField(rows.First(), headerMap, "ExamRollNo");

        var existingSemester = await _dbContext.Semesters
            .FirstOrDefaultAsync(s => s.TranscriptId == transcript.Id && s.SemesterNumber == semesterNumber);

        Semester semester;
        if (existingSemester == null)
        {
            semester = new Semester
            {
                TranscriptId = transcript.Id,
                Name = semesterName,
                SemesterNumber = semesterNumber,
                AcademicYear = academicYear,
                ExamRollNo = examRollNo,
                SortOrder = semesterNumber,
            };
            _dbContext.Semesters.Add(semester);
            result.SemestersCreated++;
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            semester = existingSemester;
            semester.ExamRollNo = examRollNo;
            semester.AcademicYear = academicYear;
        }

        var sortOrder = 0;
        foreach (var row in rows)
        {
            sortOrder++;
            var subjectCode = GetField(row, headerMap, "SubjectCode").Trim();
            var subjectName = GetField(row, headerMap, "SubjectName").Trim();
            var creditHourStr = GetField(row, headerMap, "CreditHour");
            var gradeLetter = GetField(row, headerMap, "GradeLetter").Trim();

            if (!double.TryParse(creditHourStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var creditHours))
            {
                result.Errors.Add($"Invalid CreditHour '{creditHourStr}' for subject '{subjectCode}' in semester {semesterNumber}.");
                continue;
            }

            var gradeValue = GradeMap.GetValueOrDefault(gradeLetter, 0.0);
            var gradePoint = Math.Round(creditHours * gradeValue, 2);

            var existingSubject = await _dbContext.Subjects
                .FirstOrDefaultAsync(s => s.SemesterId == semester.Id && s.SubjectCode == subjectCode);

            if (existingSubject == null)
            {
                var subject = new Subject
                {
                    SemesterId = semester.Id,
                    SubjectCode = subjectCode,
                    SubjectName = subjectName,
                    CreditHours = creditHours,
                    Grade = gradeLetter,
                    GradeValue = gradeValue,
                    GradePoint = gradePoint,
                    SortOrder = sortOrder,
                };
                _dbContext.Subjects.Add(subject);
                result.SubjectsCreated++;
            }
            else
            {
                existingSubject.SubjectName = subjectName;
                existingSubject.CreditHours = creditHours;
                existingSubject.Grade = gradeLetter;
                existingSubject.GradeValue = gradeValue;
                existingSubject.GradePoint = gradePoint;
                existingSubject.SortOrder = sortOrder;
            }
        }
    }

    private async Task<int?> GetInstitutionIdAsync()
    {
        var institution = await _dbContext.Institutions.FirstOrDefaultAsync(i => i.IsActive);
        return institution?.Id;
    }

    private static (string FirstName, string? MiddleName, string LastName) ParseFullName(string fullName)
    {
        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return (string.Empty, null, string.Empty);
        if (parts.Length == 1) return (parts[0], null, string.Empty);
        if (parts.Length == 2) return (parts[0], null, parts[1]);

        return (parts[0], string.Join(" ", parts[1..^1]), parts[^1]);
    }

    private static string ExtractIntakeYear(string regdNo)
    {
        var parts = regdNo.Split('-');
        return parts.Length >= 2 ? parts[1] : string.Empty;
    }

    private static string GetField(string[] row, Dictionary<string, int> headerMap, string fieldName)
    {
        if (!headerMap.TryGetValue(fieldName, out var index) || index >= row.Length)
            return string.Empty;
        return row[index]?.Trim() ?? string.Empty;
    }

    private static Dictionary<string, int> BuildHeaderMap(string[] headers)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < headers.Length; i++)
        {
            var key = headers[i].Trim().Replace(" ", "").Replace("_", "");
            map[key] = i;
        }
        return map;
    }

    private static List<string[]> ReadCsvLines(StreamReader reader)
    {
        var lines = new List<string[]>();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            lines.Add(ParseCsvLine(line));
        }
        return lines;
    }

    private static string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    current.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == ',')
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
        }

        fields.Add(current.ToString());
        return fields.ToArray();
    }
}
