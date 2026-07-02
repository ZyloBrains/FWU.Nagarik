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
    public int RowsSkipped { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class CsvTranscriptImportService : ICsvTranscriptImportService
{
    private readonly AppDbContext _dbContext;

    private static readonly string[] RequiredHeaders =
    [
        "RegistrationNo", "FullName", "SubjectCode", "SubjectName",
        "CreditHour", "GradeLetter", "IssueNo", "year", "part"
    ];

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

        var missingHeaders = RequiredHeaders.Where(h => !headerMap.ContainsKey(h)).ToList();
        if (missingHeaders.Count > 0)
        {
            result.Errors.Add($"Missing required CSV columns: {string.Join(", ", missingHeaders)}");
            return result;
        }

        var allRows = lines.Skip(1)
            .Where(r => r.Length > 0 && !string.IsNullOrWhiteSpace(r[0]))
            .Select((r, i) => new { Row = r, LineNumber = i + 2 })
            .ToList();

        var validRows = new List<(string[] Row, int LineNumber)>();
        var invalidRows = new List<(string[] Row, int LineNumber, string Error)>();

        foreach (var item in allRows)
        {
            var validationError = ValidateRow(item.Row, headerMap, item.LineNumber);
            if (validationError != null)
            {
                invalidRows.Add((item.Row, item.LineNumber, validationError));
            }
            else
            {
                validRows.Add((item.Row, item.LineNumber));
            }
        }

        foreach (var invalid in invalidRows)
        {
            result.Errors.Add($"Row {invalid.LineNumber}: {invalid.Error}");
        }
        result.RowsSkipped = invalidRows.Count;

        var studentGroups = validRows
            .GroupBy(r => GetField(r.Row, headerMap, "RegistrationNo"))
            .ToList();

        foreach (var studentGroup in studentGroups)
        {
            try
            {
                var regdNo = studentGroup.Key;
                var firstRow = studentGroup.First().Row;

                var student = await ProcessStudentAsync(regdNo, firstRow, headerMap, uploadedBy, result);
                if (student == null) continue;

                var transcriptGroups = studentGroup
                    .GroupBy(r => new
                    {
                        IssueNo = GetField(r.Row, headerMap, "IssueNo").Trim(),
                        ProgramName = GetField(r.Row, headerMap, "ProgramName").Trim()
                    })
                    .ToList();

                foreach (var transcriptGroup in transcriptGroups)
                {
                    await ProcessTranscriptRowsAsync(student, transcriptGroup.Select(r => r.Row).ToList(), headerMap, result);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Error processing student '{studentGroup.Key}': {ex.Message}");
            }
        }

        return result;
    }

    private static string? ValidateRow(string[] row, Dictionary<string, int> headerMap, int lineNumber)
    {
        var regdNo = GetField(row, headerMap, "RegistrationNo");
        if (string.IsNullOrWhiteSpace(regdNo))
            return "RegistrationNo is required";

        var fullName = GetField(row, headerMap, "FullName");
        if (string.IsNullOrWhiteSpace(fullName))
            return "FullName is required";

        var subjectCode = GetField(row, headerMap, "SubjectCode");
        if (string.IsNullOrWhiteSpace(subjectCode))
            return "SubjectCode is required";

        var subjectName = GetField(row, headerMap, "SubjectName");
        if (string.IsNullOrWhiteSpace(subjectName))
            return "SubjectName is required";

        var issueNoStr = GetField(row, headerMap, "IssueNo");
        if (string.IsNullOrWhiteSpace(issueNoStr) || !int.TryParse(issueNoStr, out _))
            return $"IssueNo '{issueNoStr}' is not a valid number";

        var yearStr = GetField(row, headerMap, "year");
        if (string.IsNullOrWhiteSpace(yearStr) || !RomanToYear.ContainsKey(yearStr.Trim()))
            return $"year '{yearStr}' is not valid (use I, II, III, IV, V, or VI)";

        var partStr = GetField(row, headerMap, "part");
        if (string.IsNullOrWhiteSpace(partStr) || !RomanToYear.ContainsKey(partStr.Trim()))
            return $"part '{partStr}' is not valid (use I or II)";

        var creditHourStr = GetField(row, headerMap, "CreditHour");
        if (string.IsNullOrWhiteSpace(creditHourStr) || !double.TryParse(creditHourStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var creditHours) || creditHours <= 0)
            return $"CreditHour '{creditHourStr}' is not a valid number greater than 0";

        var gradeLetter = GetField(row, headerMap, "GradeLetter");
        if (string.IsNullOrWhiteSpace(gradeLetter) || !GradeMap.ContainsKey(gradeLetter.Trim()))
            return $"GradeLetter '{gradeLetter}' is not a recognized grade";

        var cgpaStr = GetField(row, headerMap, "CGPA");
        if (!string.IsNullOrWhiteSpace(cgpaStr))
        {
            if (!double.TryParse(cgpaStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var cgpa) || cgpa < 0 || cgpa > 4)
                return $"CGPA '{cgpaStr}' is not a valid number between 0 and 4";
        }

        var yearIdx = RomanToYear[yearStr.Trim()];
        var partIdx = RomanToYear[partStr.Trim()];
        var semesterNumber = (yearIdx * 2) + partIdx + 1;
        if (semesterNumber < 1 || semesterNumber > 12)
            return $"Computed semester number {semesterNumber} is out of range (1-12)";

        return null;
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

    private async Task ProcessTranscriptRowsAsync(Student student, List<string[]> rows, Dictionary<string, int> headerMap, ImportResult result)
    {
        var issueNoStr = GetField(rows.First(), headerMap, "IssueNo");
        if (!int.TryParse(issueNoStr, out var issueSerialNo))
        {
            result.Errors.Add($"Invalid IssueNo '{issueNoStr}' for student '{student.RegdNo}'.");
            return;
        }

        foreach (var row in rows)
        {
            var subjectCode = GetField(row, headerMap, "SubjectCode").Trim();

            var existing = await _dbContext.Transcripts
                .FirstOrDefaultAsync(t =>
                    t.RegdNo == student.RegdNo &&
                    t.IssueSerialNo == issueSerialNo &&
                    t.SubjectCode == subjectCode);

            var yearStr = GetField(row, headerMap, "year");
            var partStr = GetField(row, headerMap, "part");
            var yearIdx = RomanToYear.GetValueOrDefault(yearStr.Trim(), 0);
            var partIdx = RomanToYear.GetValueOrDefault(partStr.Trim(), 0);
            var semesterNumber = (yearIdx * 2) + partIdx + 1;
            var semesterName = SemesterNames.GetValueOrDefault(semesterNumber, $"Semester {semesterNumber}");

            var creditHourStr = GetField(row, headerMap, "CreditHour");
            double.TryParse(creditHourStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var creditHours);

            var gradeLetter = GetField(row, headerMap, "GradeLetter").Trim();
            var gradeValue = GradeMap.GetValueOrDefault(gradeLetter, 0.0);
            var gradePoint = Math.Round(creditHours * gradeValue, 2);

            var cgpaStr = GetField(row, headerMap, "CGPA");
            double? cgpa = null;
            if (double.TryParse(cgpaStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedCgpa))
                cgpa = parsedCgpa;

            if (existing == null)
            {
                var transcript = new Transcript
                {
                    RegdNo = student.RegdNo,
                    IssueSerialNo = issueSerialNo,
                    IssueDate = DateTime.UtcNow,
                    IsPrinted = false,
                    StudentName = $"{student.FirstName} {student.MiddleName} {student.LastName}".Trim(),
                    ProgramName = GetField(row, headerMap, "ProgramName"),
                    FacultyName = GetField(row, headerMap, "FacultyName"),
                    CollegeName = GetField(row, headerMap, "CollegeName"),
                    AcademicYearName = GetField(row, headerMap, "AcademicYearName"),
                    SemesterNumber = semesterNumber,
                    SemesterName = semesterName,
                    Year = yearStr.Trim(),
                    Part = partStr.Trim(),
                    ExamRollNo = GetField(row, headerMap, "ExamRollNo"),
                    SubjectCode = subjectCode,
                    SubjectName = GetField(row, headerMap, "SubjectName").Trim(),
                    CreditHours = creditHours,
                    Grade = gradeLetter,
                    GradeValue = gradeValue,
                    GradePoint = gradePoint,
                    CGPA = cgpa,
                    SortOrder = semesterNumber * 100 + (existing?.SortOrder ?? 0),
                };
                _dbContext.Transcripts.Add(transcript);
                result.TranscriptsCreated++;
            }
            else
            {
                existing.StudentName = $"{student.FirstName} {student.MiddleName} {student.LastName}".Trim();
                existing.ProgramName = GetField(row, headerMap, "ProgramName");
                existing.FacultyName = GetField(row, headerMap, "FacultyName");
                existing.CollegeName = GetField(row, headerMap, "CollegeName");
                existing.AcademicYearName = GetField(row, headerMap, "AcademicYearName");
                existing.SemesterNumber = semesterNumber;
                existing.SemesterName = semesterName;
                existing.Year = yearStr.Trim();
                existing.Part = partStr.Trim();
                existing.ExamRollNo = GetField(row, headerMap, "ExamRollNo");
                existing.SubjectName = GetField(row, headerMap, "SubjectName").Trim();
                existing.CreditHours = creditHours;
                existing.Grade = gradeLetter;
                existing.GradeValue = gradeValue;
                existing.GradePoint = gradePoint;
                existing.CGPA = cgpa;
                result.TranscriptsUpdated++;
            }
        }
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
