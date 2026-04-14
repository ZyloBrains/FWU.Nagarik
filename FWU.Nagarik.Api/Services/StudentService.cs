using FWU.Nagarik.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FWU.Nagarik.Api.Services;

public interface IStudentService
{
    Task<StudentResponse?> VerifyStudentAsync(string registrationNumber, string dobAD);
    Task<TranscriptResponse?> GetTranscriptAsync(string registrationNumber, string dobAD);
}

public class StudentService(Data.AppDbContext dbContext, ILogger<StudentService> logger) : IStudentService
{
    private readonly Data.AppDbContext _dbContext = dbContext;
    private readonly ILogger<StudentService> _logger = logger;

    public async Task<StudentResponse?> VerifyStudentAsync(string registrationNumber, string dobAD)
    {
        var student = await _dbContext.Students
            .FirstOrDefaultAsync(s => s.RegdNo == registrationNumber && s.DobAD == dobAD);

        await LogVerification(registrationNumber, student, student != null ? "Success" : "NotFound", null);

        if (student == null)
            return null;

        var studentData = new StudentData
        {
            RegdNo = student.RegdNo,
            FirstName = student.FirstName,
            MiddleName = student.MiddleName,
            LastName = student.LastName,
            DobAD = student.DobAD,
            ProgramName = student.ProgramName,
            IntakeYear = student.IntakeYear,
            StudentStatus = student.StudentStatus,
            Level = student.Level,
            School = student.School,
            CgpaScore = student.CgpaScore,
            GraduateYear = student.GraduateYear
        };

        return new StudentResponse
        {
            Data = registrationNumber,
            Message = "Success",
            OtherData = [studentData]
        };
    }

    private async Task LogVerification(string regdNo, Student? student, string status, string? errorMessage)
    {
        var log = new VerificationLog
        {
            RegdNo = regdNo,
            FirstName = student?.FirstName ?? string.Empty,
            MiddleName = student?.MiddleName ?? string.Empty,
            LastName = student?.LastName ?? string.Empty,
            DobAD = student?.DobAD ?? string.Empty,
            ProgramName = student?.ProgramName ?? string.Empty,
            IntakeYear = student?.IntakeYear ?? string.Empty,
            StudentStatus = student?.StudentStatus ?? string.Empty,
            Level = student?.Level,
            School = student?.School,
            CgpaScore = student?.CgpaScore,
            GraduateYear = student?.GraduateYear,
            VerifiedAt = DateTime.UtcNow,
            VerificationStatus = status,
            ErrorMessage = errorMessage
        };

        _dbContext.VerificationLogs.Add(log);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TranscriptResponse?> GetTranscriptAsync(string registrationNumber, string dobAD)
    {
        var student = await _dbContext.Students
            .FirstOrDefaultAsync(s => s.RegdNo == registrationNumber && s.DobAD == dobAD);

        if (student == null)
            return null;

        var marks = await _dbContext.StudentMarks
            .Where(m => m.RegdNo == registrationNumber)
            .OrderBy(m => m.AcademicYear)
            .ThenBy(m => m.Semester)
            .ToListAsync();

        var subjectMarks = marks.Select(m => new SubjectMark
        {
            SubjectName = m.SubjectName,
            SubjectCode = m.SubjectCode,
            Marks = m.Marks,
            Grade = m.Grade,
            Semester = m.Semester,
            AcademicYear = m.AcademicYear
        }).ToList();

        var transcriptData = new TranscriptData
        {
            RegdNo = student.RegdNo,
            FirstName = student.FirstName,
            MiddleName = student.MiddleName,
            LastName = student.LastName,
            ProgramName = student.ProgramName,
            IntakeYear = student.IntakeYear,
            Marks = subjectMarks
        };

        return new TranscriptResponse
        {
            Data = registrationNumber,
            Message = "Success",
            OtherData = [transcriptData]
        };
    }
}