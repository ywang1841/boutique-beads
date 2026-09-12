using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BoutiqueBeads.Services;

// Handles all image/video uploads for both the public site and the Admin portal.
// Reads connection info from configuration (see appsettings.json / user-secrets / App Service config):
//
//   "AzureStorage": {
//       "ConnectionString": "...",   // from Storage Account > Access keys
//       "ContainerName": "media"
//   }
public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    // Allow-list to stop anything unexpected being uploaded through the admin form.
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private static readonly HashSet<string> AllowedVideoExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".mov", ".webm"
    };

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureStorage:ConnectionString"]
            ?? throw new InvalidOperationException("AzureStorage:ConnectionString is not configured.");
        var containerName = configuration["AzureStorage:ContainerName"] ?? "media";

        var serviceClient = new BlobServiceClient(connectionString);
        _containerClient = serviceClient.GetBlobContainerClient(containerName);

        // Container is created once at startup if missing, with public read access
        // on blobs only (not container listing) - product photos/videos are meant
        // to be publicly viewable on the storefront.
        _containerClient.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<BlobUploadResult> UploadAsync(
        Stream fileStream,
        string originalFileName,
        string contentType,
        string folder,
        CancellationToken ct = default)
    {
        var extension = Path.GetExtension(originalFileName);
        ValidateExtension(extension, folder);

        // Unique blob name so admin uploads never collide or overwrite each other.
        var blobName = $"{folder}/{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}{extension}";
        var blobClient = _containerClient.GetBlobClient(blobName);

        var headers = new BlobHttpHeaders { ContentType = contentType };
        await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = headers }, ct);

        return new BlobUploadResult(blobName, blobClient.Uri.ToString());
    }

    public async Task DeleteAsync(string blobName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(blobName)) return;
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
    }

    private static void ValidateExtension(string extension, string folder)
    {
        var isVideo = folder.Contains("video", StringComparison.OrdinalIgnoreCase)
                      || folder.Contains("tutorial", StringComparison.OrdinalIgnoreCase);

        var allowed = isVideo ? AllowedVideoExtensions : AllowedImageExtensions;

        if (!allowed.Contains(extension))
        {
            throw new InvalidOperationException(
                $"File type '{extension}' is not allowed in folder '{folder}'. " +
                $"Allowed types: {string.Join(", ", allowed)}");
        }
    }
}
