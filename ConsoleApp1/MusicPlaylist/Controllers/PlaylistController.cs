using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MusicPlaylist.Models;
using MusicPlaylist.Services;

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
            if (HttpContext.Session.GetInt32("ProfileId") == null)
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

            string normalizedUrl =
                $"https://www.youtube.com/watch?v={videoId}";

            string embedUrl =
                $"https://www.youtube.com/embed/{videoId}";

            string thumbnailUrl =
                $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";

            item.ProfileId = profileId.Value;
            item.VideoId = videoId;
            item.Title = item.Title.Trim();
            item.YouTubeUrl = normalizedUrl;
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
                    YouTubeUrl = normalizedUrl,
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

            PlaylistItem? playlistItem =
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

            if (!Uri.TryCreate(
                    youtubeUrl.Trim(),
                    UriKind.Absolute,
                    out Uri? uri))
            {
                return null;
            }

            string host = uri.Host.ToLowerInvariant();

            if (host == "youtu.be" ||
                host == "www.youtu.be")
            {
                return CleanVideoId(
                    uri.AbsolutePath.Trim('/'));
            }

            if (host != "youtube.com" &&
                host != "www.youtube.com" &&
                host != "m.youtube.com" &&
                host != "music.youtube.com")
            {
                return null;
            }

            if (uri.AbsolutePath.Equals(
                    "/watch",
                    StringComparison.OrdinalIgnoreCase))
            {
                var queryValues =
                    QueryHelpers.ParseQuery(uri.Query);

                if (queryValues.TryGetValue(
                        "v",
                        out var videoId))
                {
                    return CleanVideoId(
                        videoId.ToString());
                }
            }

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

            int parameterPosition =
                videoId.IndexOfAny(new[] { '?', '&', '#' });

            if (parameterPosition >= 0)
            {
                videoId = videoId[..parameterPosition];
            }

            if (videoId.Length != 11)
            {
                return null;
            }

            bool validCharacters =
                videoId.All(character =>
                    char.IsLetterOrDigit(character) ||
                    character == '-' ||
                    character == '_');

            return validCharacters ? videoId : null;
        }
    }
}