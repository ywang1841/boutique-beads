using System.Text.RegularExpressions;

namespace BoutiqueBeads.Services;

// Turns a normal YouTube link (watch?v=, youtu.be/, or already-embed) into the
// //www.youtube.com/embed/{id} form needed for an <iframe>, plus a thumbnail URL.
// Swap this out (or extend it) if you use Vimeo or another host instead.
public static class YouTubeHelper
{
    private static readonly Regex VideoIdPattern = new(
        @"(?:youtu\.be/|youtube\.com/(?:watch\?v=|embed/|shorts/))([A-Za-z0-9_-]{11})",
        RegexOptions.Compiled);

    public static string? ExtractVideoId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var match = VideoIdPattern.Match(url);
        return match.Success ? match.Groups[1].Value : null;
    }

    /// <summary>Returns a URL safe to put in an &lt;iframe src&gt;, or null if the link isn't recognized.</summary>
    public static string? GetEmbedUrl(string? url)
    {
        var id = ExtractVideoId(url);
        return id is null ? null : $"https://www.youtube.com/embed/{id}";
    }

    /// <summary>Returns YouTube's auto-generated thumbnail for the video, or null if not recognized.</summary>
    public static string? GetThumbnailUrl(string? url)
    {
        var id = ExtractVideoId(url);
        return id is null ? null : $"https://img.youtube.com/vi/{id}/hqdefault.jpg";
    }
}
