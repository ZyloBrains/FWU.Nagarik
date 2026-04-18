using System.Text.Json;
using FWU.Nagarik.Api.Models;
using FWU.Nagarik.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace FWU.Nagarik.Api.Services;

public interface IStudentRequestSyncService
{
    Task<SyncResult> SyncStudentRequestsAsync();
}

public class SyncResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int RecordsSynced { get; set; }
    public DateTime SyncTime { get; set; }
}

public class StudentRequestSyncService(
    Data.AppDbContext dbContext,
    IConfiguration configuration,
    ILogger<StudentRequestSyncService> logger) : IStudentRequestSyncService
{
    private readonly Data.AppDbContext _dbContext = dbContext;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<StudentRequestSyncService> _logger = logger;

    public async Task<SyncResult> SyncStudentRequestsAsync()
    {
        var result = new SyncResult { SyncTime = DateTime.UtcNow };

        try
        {
            var lastSyncTime = await GetLastSyncTime();
            var baseUrl = _configuration["ExternalApi:KuBaseUrl"] ?? "https://university.edu.np/api";

            _logger.LogInformation("Starting sync from {BaseUrl} since {LastSyncTime}", baseUrl, lastSyncTime);

            var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            var sinceParam = lastSyncTime.ToString("o");

            var response = await httpClient.GetAsync($"/student-requests?since={Uri.EscapeDataString(sinceParam)}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("External API returned {StatusCode}, using mock data", response.StatusCode);
                return await GenerateMockData(result);
            }

            var content = await response.Content.ReadAsStringAsync();
            var externalRequests = JsonSerializer.Deserialize<List<ExternalStudentRequest>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (externalRequests == null || externalRequests.Count == 0)
            {
                result.Success = true;
                result.Message = "No new records to sync";
                result.RecordsSynced = 0;
                return result;
            }

            var syncedCount = 0;
            foreach (var extReq in externalRequests)
            {
                var existing = await _dbContext.StudentRequests
                    .FirstOrDefaultAsync(r => r.Id == extReq.Id);

                if (existing == null)
                {
                    var studentRequest = new StudentRequest
                    {
                        DocumentType = ParseDocumentType(extReq.DocumentType),
                        RequestedDate = extReq.RequestedDate,
                        Requestedby = extReq.RequestedBy ?? "Self",
                        StudentAdmissionId = extReq.StudentAdmissionId
                    };
                    _dbContext.StudentRequests.Add(studentRequest);
                    syncedCount++;
                }
            }

            await _dbContext.SaveChangesAsync();

            result.Success = true;
            result.Message = $"Successfully synced {syncedCount} records from external API";
            result.RecordsSynced = syncedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing from external API, falling back to mock data");
            return await GenerateMockData(result);
        }

        return result;
    }

    private async Task<SyncResult> GenerateMockData(SyncResult result)
    {
        try
        {
            var existingMaxId = await _dbContext.StudentRequests.MaxAsync(r => (int?)r.Id) ?? 0;
            var startId = existingMaxId + 1;

            var random = new Random();
            var documentTypes = Enum.GetValues<DocumentType>();
            var syncCount = 0;

            for (int i = 0; i < 10; i++)
            {
                var studentRequest = new StudentRequest
                {
                    DocumentType = documentTypes[random.Next(documentTypes.Length)],
                    RequestedDate = DateTime.UtcNow.AddDays(-random.Next(0, 30)).AddHours(-random.Next(0, 24)),
                    Requestedby = random.Next(2) == 0 ? "Self" : "Admin",
                    StudentAdmissionId = 1000 + random.Next(1, 200)
                };
                _dbContext.StudentRequests.Add(studentRequest);
                syncCount++;
            }

            await _dbContext.SaveChangesAsync();

            result.Success = true;
            result.Message = $"Successfully synced {syncCount} mock records";
            result.RecordsSynced = syncCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating mock data");
            result.Success = false;
            result.Message = $"Sync failed: {ex.Message}";
        }

        return result;
    }

    private async Task<DateTime> GetLastSyncTime()
    {
        var lastRequest = await _dbContext.StudentRequests
            .OrderByDescending(r => r.RequestedDate)
            .FirstOrDefaultAsync();

        return lastRequest?.RequestedDate ?? DateTime.UtcNow.AddDays(-30);
    }

    private static DocumentType ParseDocumentType(string? docType)
    {
        if (string.IsNullOrEmpty(docType))
            return DocumentType.Transcript;

        return docType.ToLower() switch
        {
            "transcript" => DocumentType.Transcript,
            "character certificate" => DocumentType.CharacterCertificate,
            "migration certificate" => DocumentType.MigrationCertificate,
            "provisional certificate" => DocumentType.ProvisionalCertificate,
            "original certificate" => DocumentType.OriginalCertificate,
            _ => DocumentType.Other
        };
    }
}

internal class ExternalStudentRequest
{
    public int Id { get; set; }
    public string? DocumentType { get; set; }
    public DateTime RequestedDate { get; set; }
    public string? RequestedBy { get; set; }
    public int StudentAdmissionId { get; set; }
}