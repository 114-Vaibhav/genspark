using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BlobArchiveFunctions;

public class BlobArchiveService
{
    private readonly ILogger<BlobArchiveService> _logger;
    private readonly string _connectionString;

    public BlobArchiveService(
        ILogger<BlobArchiveService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _connectionString =
            configuration["BlobStorageConnectionString"]
            ?? throw new InvalidOperationException(
                "BlobStorageConnectionString is not configured.");
    }

    public async Task<int> ArchiveOldBlobsAsync(int olderThanDays = 7)
    {
        var serviceClient = new BlobServiceClient(_connectionString);

        var uploads = serviceClient.GetBlobContainerClient("uploads");
        var archive = serviceClient.GetBlobContainerClient("archive");

        await archive.CreateIfNotExistsAsync();

        int count = 0;

        var cutoff = DateTimeOffset.UtcNow.AddDays(-olderThanDays);

        await foreach (var blob in uploads.GetBlobsAsync())
        {
            if (blob.Properties.LastModified < cutoff)
            {
                var source = uploads.GetBlobClient(blob.Name);
                var destination = archive.GetBlobClient(blob.Name);

                await destination.StartCopyFromUriAsync(source.Uri);
                await source.DeleteAsync();

                _logger.LogInformation("Archived: {Blob}", blob.Name);

                count++;
            }
        }

        _logger.LogInformation("Files moved: {Count}", count);

        return count;
    }
}