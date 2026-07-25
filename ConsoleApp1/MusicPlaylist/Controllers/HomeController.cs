using Microsoft.AspNetCore.Mvc;
using MusicPlaylist.Models;
using MusicPlaylist.Services;

namespace MusicPlaylist.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDataService _appData;

        public HomeController(AppDataService appData)
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

            UserProfile? currentProfile =
                _appData.Profiles.FirstOrDefault(profile =>
                    profile.ProfileId == profileId.Value);

            if (currentProfile == null)
            {
                HttpContext.Session.Clear();

                return RedirectToAction("Login", "Account");
            }

            List<MusicVideo> topSongs =
                _appData.MusicVideos
                    .Where(song => song.PlayCount > 0)
                    .OrderByDescending(song => song.PlayCount)
                    .ThenBy(song => song.Title)
                    .Take(5)
                    .ToList();

            HomeViewModel viewModel = new()
            {
                ProfileName = currentProfile.FullName,
                TopSongs = topSongs,
                TotalProfiles = _appData.Profiles.Count,
                TotalSongs = _appData.MusicVideos.Count,
                TotalStreams = _appData.MusicVideos.Sum(
                    song => song.PlayCount)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RecordPlay(string videoId)
        {
            int? profileId =
                HttpContext.Session.GetInt32("ProfileId");

            if (profileId == null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(videoId))
            {
                return BadRequest();
            }

            MusicVideo? song =
                _appData.MusicVideos.FirstOrDefault(item =>
                    item.VideoId == videoId);

            if (song == null)
            {
                return NotFound();
            }

            song.PlayCount++;

            return Ok(new
            {
                message = "Play recorded.",
                playCount = song.PlayCount
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier
            });
        }
    }
}