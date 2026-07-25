using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MusicPlaylist.Models;
using MusicPlaylistMvc.Services;

namespace MusicPlaylist.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly AppDataService _appData;

        public PlaylistController(AppDataService appData)
        {
            _appData = appData;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int? profileId =
                HttpContext.Session.GetInt32("ProfileId");

            if (profileId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<PlaylistItem> playlist =
                _appData.PlaylistItems
                    .Where(item =>
                        item.ProfileId == profileId.Value)
                    .OrderByDescending(item => item.DateAdded)
                    .ToList();

            return View(playlist);
        }

        [HttpGet]
        public IActionResult Add()
        {
            int? profileId =
                HttpContext.Session.GetInt32("ProfileId");

            if (profileId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new PlaylistItem());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(PlaylistItem item)
        {
            int? profileId =
                HttpContext.Session.GetInt32("ProfileId");

            if (profileId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(item);
            }

            string? videoId =
                ExtractVideoId(item.YouTubeUrl);

            if (string.IsNullOrWhiteSpace(videoId))
            {
                ModelState.AddModelError(
                    nameof(item.YouTubeUrl),
                    "Please enter a valid YouTube video URL.");

                return View(item);
            }

            bool alreadyExists =
                _appData.PlaylistItems.Any(existingItem =>
                    existingItem.ProfileId == profileId.Value &&
                    existingItem.VideoId == videoId);

            if (alreadyExists)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This song is already in your playlist.");

                return View(item);
            }

            string normalizedYouTubeUrl =
                $"https://www.youtube.com/watch?v={videoId}";

            string embedUrl =
                $"https://www.youtube.com/embed/{videoId}";

            string thumbnailUrl =
                $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";

            item.ProfileId = profileId.Value;
            item.VideoId = videoId;
            item.Title = item.Title.Trim();
            item.YouTubeUrl = normalizedYouTubeUrl;
            item.EmbedUrl = embedUrl;
            item.ThumbnailUrl = thumbnailUrl;
            item.DateAdded = DateTime.Now;

            _appData.PlaylistItems.Add(item);

            MusicVideo? globalSong =
                _appData.MusicVideos.FirstOrDefault(song =>
                    song.VideoId == videoId);

            if (globalSong == null)
            {
                _appData.MusicVideos.Add(new MusicVideo
                {
                    VideoId = videoId,
                    Title = item.Title,
                    YouTubeUrl = normalizedYouTubeUrl,
                    EmbedUrl = embedUrl,
                    ThumbnailUrl = thumbnailUrl,
                    PlayCount = 0
                });
            }

            TempData["SuccessMessage"] =
                "Song added to your playlist.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(string videoId)
        {
            int? profileId =
                HttpContext.Session.GetInt32("ProfileId");

            if (profileId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(videoId))
            {
                TempData["ErrorMessage"] =
                    "Unable to remove the selected song.";

                return RedirectToAction(nameof(Index));
            }

            PlaylistItem? playlistItem =
                _appData.PlaylistItems.FirstOrDefault(item =>
                    item.ProfileId == profileId.Value &&
                    item.VideoId == videoId);

            if (playlistItem == null)
            {
                TempData["ErrorMessage"] =
                    "The song was not found in your playlist.";

                return RedirectToAction(nameof(Index));
            }

            _appData.PlaylistItems.Remove(playlistItem);

            TempData["SuccessMessage"] =
                "Song removed from your playlist.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            int? profileId =
                HttpContext.Session.GetInt32("ProfileId");

            if (profileId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _appData.PlaylistItems.RemoveAll(item =>
                item.ProfileId == profileId.Value);

            TempData["SuccessMessage"] =
                "Your playlist has been cleared.";

            return RedirectToAction(nameof(Index));
        }

        private static string? ExtractVideoId(string youtubeUrl)
        {
            if (string.IsNullOrWhiteSpace(youtubeUrl))
            {
                return null;
            }

            youtubeUrl = youtubeUrl.Trim();

            if (!Uri.TryCreate(
                    youtubeUrl,
                    UriKind.Absolute,
                    out Uri? uri))
            {
                return null;
            }

            string host =
                uri.Host.ToLowerInvariant();

            // Example:
            // https://youtu.be/VIDEO_ID
            if (host == "youtu.be" ||
                host == "www.youtu.be")
            {
                return CleanVideoId(
                    uri.AbsolutePath.Trim('/'));
            }

            bool validYouTubeHost =
                host == "youtube.com" ||
                host == "www.youtube.com" ||
                host == "m.youtube.com" ||
                host == "music.youtube.com";

            if (!validYouTubeHost)
            {
                return null;
            }

            // Example:
            // https://www.youtube.com/watch?v=VIDEO_ID
            if (uri.AbsolutePath.Equals(
                    "/watch",
                    StringComparison.OrdinalIgnoreCase))
            {
                Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                    queryValues =
                        QueryHelpers.ParseQuery(uri.Query);

                if (queryValues.TryGetValue(
                        "v",
                        out var videoId))
                {
                    return CleanVideoId(
                        videoId.ToString());
                }
            }

            // Examples:
            // /embed/VIDEO_ID
            // /shorts/VIDEO_ID
            // /live/VIDEO_ID
            string[] pathParts =
                uri.AbsolutePath.Split(
                    '/',
                    StringSplitOptions.RemoveEmptyEntries);

            if (pathParts.Length >= 2)
            {
                string firstPart =
                    pathParts[0].ToLowerInvariant();

                if (firstPart == "embed" ||
                    firstPart == "shorts" ||
                    firstPart == "live")
                {
                    return CleanVideoId(pathParts[1]);
                }
            }

            return null;
        }

        private static string? CleanVideoId(string videoId)
        {
            if (string.IsNullOrWhiteSpace(videoId))
            {
                return null;
            }

            videoId = videoId.Trim();

            int extraParameterPosition =
                videoId.IndexOfAny(
                    new[] { '?', '&', '#' });

            if (extraParameterPosition >= 0)
            {
                videoId =
                    videoId[..extraParameterPosition];
            }

            // Standard YouTube video IDs contain 11 characters.
            if (videoId.Length != 11)
            {
                return null;
            }

            bool validCharacters =
                videoId.All(character =>
                    char.IsLetterOrDigit(character) ||
                    character == '-' ||
                    character == '_');

            return validCharacters
                ? videoId
                : null;
        }
    }
}