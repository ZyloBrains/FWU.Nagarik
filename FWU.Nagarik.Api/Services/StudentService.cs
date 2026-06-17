using FWU.Nagarik.Api.Mappers;
using FWU.Nagarik.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FWU.Nagarik.Api.Services;

public interface IStudentService
{
    Task<StudentResponse?> VerifyStudentAsync(string registrationNumber, string dobAD);
    Task<TranscriptResponse?> GetTranscriptAsync(string registrationNumber, string dobAD);
}

public class StudentService(Data.AppDbContext dbContext) : IStudentService
{
    private readonly Data.AppDbContext _dbContext = dbContext;

    public async Task<StudentResponse?> VerifyStudentAsync(string registrationNumber, string dobAD)
    {
        var studentAdmissions = await _dbContext.Students
            .Where(s => s.RegdNo == registrationNumber && s.DobAD == dobAD)
            .ToListAsync();

        if (studentAdmissions == null || studentAdmissions.Count == 0)
        {
            await LogVerification(registrationNumber, null, "NotFound", null);
            return null;
        }

        List<StudentData> studentDataList = [];

        foreach (var student in studentAdmissions)
        {
            await LogVerification(registrationNumber, student, "Success", null);
            studentDataList.Add(new StudentData
            {
                RegdNo = student.RegdNo,
                FirstName = student.FirstName,
                MiddleName = student.MiddleName ?? string.Empty,
                LastName = student.LastName,
                DobAD = student.DobAD,
                ProgramName = student.ProgramName,
                IntakeYear = student.IntakeYear,
                StudentStatus = student.StudentStatus,
                Level = student.Level,
                School = student.School,
                CgpaScore = student.CgpaScore,
                GraduateYear = student.GraduateYear
            });
        }

        return new StudentResponse
        {
            Data = registrationNumber,
            Message = "Success",
            OtherData = studentDataList
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

        var transcript = await _dbContext.Transcripts
            .Include(t => t.Institution)
            .Include(t => t.Semesters)
                .ThenInclude(s => s.Subjects)
            .FirstOrDefaultAsync(t => t.RegdNo == registrationNumber);

        if (transcript == null)
            return null;

        var transcriptViewModel = TranscriptMapper.ToViewModel(transcript, student);

        return new TranscriptResponse
        {
            Data = registrationNumber,
            Message = "Success",
            Transcript = transcriptViewModel
        };
    }
}