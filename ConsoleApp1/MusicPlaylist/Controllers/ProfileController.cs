using Microsoft.AspNetCore.Mvc;
using MusicPlaylist.Models;
using MusicPlaylistMvc.Services;

namespace MusicPlaylist.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDataService _appData;

        public ProfileController(AppDataService appData)
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
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            UserProfile? user =
                _appData.Profiles.FirstOrDefault(profile =>
                    profile.ProfileId == profileId.Value);

            if (user == null)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // Get the songs belonging to the current profile.
            var userPlaylist =
                _appData.PlaylistItems
                    .Where(item =>
                        item.ProfileId == profileId.Value)
                    .ToList();

            int playlistCount =
                userPlaylist.Count;

            // Get all video IDs in the current user's playlist.
            var userVideoIds =
                userPlaylist
                    .Select(item => item.VideoId)
                    .ToHashSet();

            // Add the play counts of the songs in the user's playlist.
            int totalPlayCount =
                _appData.MusicVideos
                    .Where(video =>
                        userVideoIds.Contains(video.VideoId))
                    .Sum(video => video.PlayCount);

            ViewBag.PlaylistCount = playlistCount;
            ViewBag.PlayCount = totalPlayCount;

            return View(user);
        }
    }
}