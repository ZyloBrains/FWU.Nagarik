using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Models;

namespace FWU.Nagarik.Api.Services;

public interface ICertificateService
{
    Task<Models.Certificate?> GetCertificateAsync(string regdNo, string programName, string certificateType);
    Task<List<Models.Certificate>> GetCertificatesAsync(string? searchRegdNo, string? certificateType, int page, int pageSize);
    Task<int> GetCertificateCountAsync(string? searchRegdNo, string? certificateType);
    Task<Models.Certificate> UploadCertificateAsync(string regdNo, string programName, string certificateType, Stream fileStream, string fileName, long fileSize, string uploadedBy);
    Task<List<Models.Certificate>> BulkUploadAsync(string certificateType, List<(Stream Stream, string FileName, long FileSize)> files, string uploadedBy);
    Task<bool> DeleteCertificateAsync(int id);
}

public class CertificateService : ICertificateService
{
    private readonly AppDbContext _dbContext;
    private readonly IAzureBlobStorageService _blobStorageService;

    public CertificateService(AppDbContext dbContext, IAzureBlobStorageService blobStorageService)
    {
        _dbContext = dbContext;
        _blobStorageService = blobStorageService;
    }

    public async Task<Models.Certificate?> GetCertificateAsync(string regdNo, string programName, string certificateType)
    {
        return await _dbContext.Certificates
            .FirstOrDefaultAsync(c =>
                c.RegdNo == regdNo &&
                c.ProgramName == programName &&
                c.CertificateType == certificateType);
    }

    public async Task<List<Models.Certificate>> GetCertificatesAsync(string? searchRegdNo, string? certificateType, int page, int pageSize)
    {
        var query = _dbContext.Certificates.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchRegdNo))
        {
            var search = searchRegdNo.ToLower();
            query = query.Where(c => c.RegdNo.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(certificateType))
        {
            query = query.Where(c => c.CertificateType == certificateType);
        }

        return await query
            .OrderByDescending(c => c.UploadedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCertificateCountAsync(string? searchRegdNo, string? certificateType)
    {
        var query = _dbContext.Certificates.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchRegdNo))
        {
            var search = searchRegdNo.ToLower();
            query = query.Where(c => c.RegdNo.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(certificateType))
        {
            query = query.Where(c => c.CertificateType == certificateType);
        }

        return await query.CountAsync();
    }

    public async Task<Models.Certificate> UploadCertificateAsync(string regdNo, string programName, string certificateType, Stream fileStream, string fileName, long fileSize, string uploadedBy)
    {
        var blobName = $"{regdNo}_{programName}_{certificateType}.pdf";
        var blobUrl = await _blobStorageService.UploadAsync(fileStream, blobName, "application/pdf");

        var certificate = new Models.Certificate
        {
            RegdNo = regdNo,
            ProgramName = programName,
            CertificateType = certificateType,
            BlobName = blobName,
            BlobUrl = blobUrl,
            OriginalFileName = fileName,
            FileSizeBytes = fileSize,
            UploadedBy = uploadedBy,
            UploadedAt = DateTime.UtcNow
        };

        _dbContext.Certificates.Add(certificate);
        await _dbContext.SaveChangesAsync();

        return certificate;
    }

    public async Task<List<Models.Certificate>> BulkUploadAsync(string certificateType, List<(Stream Stream, string FileName, long FileSize)> files, string uploadedBy)
    {
        var uploaded = new List<Models.Certificate>();

        foreach (var (stream, fileName, fileSize) in files)
        {
            var regdNo = ParseRegdNoFromFileName(fileName);
            var programName = ParseProgramNameFromFileName(fileName);

            if (string.IsNullOrWhiteSpace(regdNo) || string.IsNullOrWhiteSpace(programName))
                continue;

            var blobName = $"{regdNo}_{programName}_{certificateType}.pdf";
            var blobUrl = await _blobStorageService.UploadAsync(stream, blobName, "application/pdf");

            var certificate = new Models.Certificate
            {
                RegdNo = regdNo,
                ProgramName = programName,
                CertificateType = certificateType,
                BlobName = blobName,
                BlobUrl = blobUrl,
                OriginalFileName = fileName,
                FileSizeBytes = fileSize,
                UploadedBy = uploadedBy,
                UploadedAt = DateTime.UtcNow
            };

            _dbContext.Certificates.Add(certificate);
            uploaded.Add(certificate);
        }

        if (uploaded.Count > 0)
            await _dbContext.SaveChangesAsync();

        return uploaded;
    }

    public async Task<bool> DeleteCertificateAsync(int id)
    {
        var certificate = await _dbContext.Certificates.FindAsync(id);
        if (certificate == null)
            return false;

        await _blobStorageService.DeleteAsync(certificate.BlobName);

        _dbContext.Certificates.Remove(certificate);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    private static string ParseRegdNoFromFileName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        var parts = name.Split('_');
        return parts.Length >= 1 ? parts[0].Trim() : string.Empty;
    }

    private static string ParseProgramNameFromFileName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        var parts = name.Split('_');
        return parts.Length >= 2 ? parts[1].Trim() : string.Empty;
    }
}
