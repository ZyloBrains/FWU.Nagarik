using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace FWU.Nagarik.Api.Services;

public interface IAzureBlobStorageService
{
    Task<string> UploadAsync(Stream stream, string blobName, string contentType);
    Task<Stream?> DownloadAsync(string blobName);
    Task<bool> DeleteAsync(string blobName);
    Task<bool> ExistsAsync(string blobName);
}

public class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ILogger<AzureBlobStorageService> _logger;

    public AzureBlobStorageService(IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
    {
        _logger = logger;
        _containerName = configuration["AzureStorage:ContainerName"] ?? "certificates";
        var useManagedIdentity = configuration.GetValue<bool>("AzureStorage:UseManagedIdentity");

        if (useManagedIdentity)
        {
            var accountName = configuration["AzureStorage:AccountName"];
            var blobUri = $"https://{accountName}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(blobUri), new DefaultAzureCredential());
        }
        else
        {
            var connectionString = configuration["AzureStorage:ConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("AzureStorage:ConnectionString is not configured.");
            _blobServiceClient = new BlobServiceClient(connectionString);
        }
    }

    public async Task<string> UploadAsync(Stream stream, string blobName, string contentType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });

        _logger.LogInformation("Uploaded blob: {BlobName} to container: {Container}", blobName, _containerName);
        return blobClient.Uri.ToString();
    }

    public async Task<Stream?> DownloadAsync(string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
            return null;

        var response = await blobClient.DownloadStreamingAsync();
        return response.Value.Content;
    }

    public async Task<bool> DeleteAsync(string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.DeleteIfExistsAsync();
        _logger.LogInformation("Deleted blob: {BlobName}, Result: {Result}", blobName, response.Value);
        return response.Value;
    }

    public async Task<bool> ExistsAsync(string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        return await blobClient.ExistsAsync();
    }
}
