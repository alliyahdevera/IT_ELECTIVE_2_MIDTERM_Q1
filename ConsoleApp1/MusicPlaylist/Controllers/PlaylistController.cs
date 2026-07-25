using Microsoft.AspNetCore.Mvc;
using MusicPlaylist.Models;
using MusicPlaylistMvc.Services;

namespace MusicPlaylistMvc.Controllers
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

            var playlist = _appData.PlaylistItems
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

            item.ProfileId = profileId.Value;
            item.VideoId = videoId;
            item.YouTubeUrl =
                $"https://www.youtube.com/watch?v={videoId}";
            item.EmbedUrl =
                $"https://www.youtube.com/embed/{videoId}";
            item.DateAdded = DateTime.Now;

            _appData.PlaylistItems.Add(item);

            // Add one global copy for the Home Top 5.
            bool globalSongExists =
                _appData.MusicVideos.Any(song =>
                    song.VideoId == videoId);

            if (!globalSongExists)
            {
                _appData.MusicVideos.Add(new MusicVideo
                {
                    VideoId = videoId,
                    Title = item.Title,
                    YouTubeUrl = item.YouTubeUrl,
                    EmbedUrl = item.EmbedUrl,
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

            var playlistItem =
                _appData.PlaylistItems.FirstOrDefault(item =>
                    item.ProfileId == profileId.Value &&
                    item.VideoId == videoId);

            if (playlistItem == null)
            {
                TempData["ErrorMessage"] =
                    "Song was not found.";

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
                "Playlist cleared.";

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

            string host = uri.Host.ToLower();

            // Example:
            // https://youtu.be/VIDEO_ID
            if (host == "youtu.be" ||
                host == "www.youtu.be")
            {
                return CleanVideoId(
                    uri.AbsolutePath.Trim('/'));
            }

            // Only allow YouTube URLs.
            if (host != "youtube.com" &&
                host != "www.youtube.com" &&
                host != "m.youtube.com" &&
                host != "music.youtube.com")
            {
                return null;
            }

            // Example:
            // https://youtube.com/watch?v=VIDEO_ID
            if (uri.AbsolutePath.Equals(
                    "/watch",
                    StringComparison.OrdinalIgnoreCase))
            {
                var queryValues =
                    Microsoft.AspNetCore.WebUtilities
                        .QueryHelpers.ParseQuery(uri.Query);

                if (queryValues.TryGetValue(
                        "v",
                        out var videoId))
                {
                    return CleanVideoId(
                        videoId.ToString());
                }
            }

            // Examples:
            // https://youtube.com/embed/VIDEO_ID
            // https://youtube.com/shorts/VIDEO_ID
            // https://youtube.com/live/VIDEO_ID
            string[] pathParts = uri.AbsolutePath
                .Split(
                    '/',
                    StringSplitOptions.RemoveEmptyEntries);

            if (pathParts.Length >= 2)
            {
                string firstPart =
                    pathParts[0].ToLower();

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

            int parameterPosition =
                videoId.IndexOfAny(new[] { '?', '&', '#' });

            if (parameterPosition >= 0)
            {
                videoId =
                    videoId[..parameterPosition];
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