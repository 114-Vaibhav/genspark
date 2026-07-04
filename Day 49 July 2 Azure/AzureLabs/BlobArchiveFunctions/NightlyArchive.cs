using Microsoft.Azure.Functions.Worker;
// using Microsoft.Azure.Functions.Worker.Extensions.Timer;
// using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BlobArchiveFunctions;

public class NightlyArchive
{
    private readonly BlobArchiveService _archiveService;
    private readonly ILogger<NightlyArchive> _logger;

    public NightlyArchive(
        BlobArchiveService archiveService,
        ILogger<NightlyArchive> logger)
    {
        _archiveService = archiveService;
        _logger = logger;
    }

    [Function("NightlyArchive")]
    public async Task Run(
        [TimerTrigger("0 0 0 * * *")] TimerInfo timer)
    {
        _logger.LogInformation(
            "Nightly archive started at {Time}",
            DateTime.UtcNow);

        await _archiveService.ArchiveOldBlobsAsync();
    }
}

// /Users/vaibhavgupta/AzureLabs/BlobArchiveFunctions/NightlyArchive.cs