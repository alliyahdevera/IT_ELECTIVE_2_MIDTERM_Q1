using Microsoft.AspNetCore.Mvc;
using MusicPlaylist.Models;
using MusicPlaylist.Services;

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
                return RedirectToAction("Login", "Account");
            }

            UserProfile? user =
                _appData.Profiles.FirstOrDefault(profile =>
                    profile.ProfileId == profileId.Value);

            if (user == null)
            {
                HttpContext.Session.Clear();

                return RedirectToAction("Login", "Account");
            }

            int playlistCount =
                _appData.PlaylistItems.Count(item =>
                    item.ProfileId == profileId.Value);

            ViewBag.PlaylistCount = playlistCount;

            return View(user);
        }
    }
}