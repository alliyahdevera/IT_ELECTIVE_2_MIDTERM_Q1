using Microsoft.AspNetCore.Mvc;
using MusicPlaylist.Models;
using MusicPlaylistMvc.Services;

namespace MusicPlaylistMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDataService _appData;

        public HomeController(AppDataService appData)
        {
            _appData = appData;
        }

        public IActionResult Index()
        {
            int? profileId = HttpContext.Session.GetInt32("ProfileId");

            if (profileId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentProfile = _appData.Profiles
                .FirstOrDefault(profile =>
                    profile.ProfileId == profileId.Value);

            if (currentProfile == null)
            {
                HttpContext.Session.Clear();

                return RedirectToAction("Login", "Account");
            }

            var topSongs = _appData.MusicVideos
                .Where(song => song.PlayCount > 0)
                .OrderByDescending(song => song.PlayCount)
                .ThenBy(song => song.Title)
                .Take(5)
                .ToList();

            var viewModel = new HomeViewModel
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
        public IActionResult RecordPlay(string videoId)
        {
            int? profileId = HttpContext.Session.GetInt32("ProfileId");

            if (profileId == null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(videoId))
            {
                return BadRequest();
            }

            var song = _appData.MusicVideos
                .FirstOrDefault(item =>
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
    }

}
