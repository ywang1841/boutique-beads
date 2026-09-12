namespace BoutiqueBeads.Services;

public interface IBlobStorageService
{
    /// <summary>
    /// Uploads a file (image or video) to Blob Storage under the given virtual folder
    /// (e.g. "products", "tutorials", "thumbnails") and returns the public URL and blob name.
    /// </summary>
    Task<BlobUploadResult> UploadAsync(Stream fileStream, string originalFileName, string contentType, string folder, CancellationToken ct = default);

    /// <summary>
    /// Deletes a previously uploaded blob, e.g. when a product image is removed or replaced.
    /// </summary>
    Task DeleteAsync(string blobName, CancellationToken ct = default);
}

public record BlobUploadResult(string BlobName, string Url);
