using Microsoft.AspNetCore.Mvc;
using MusicPlaylist.Models;
using MusicPlaylistMvc.Services;

namespace MusicPlaylistMvc.Controllers
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

            UserProfile? profile =
                _appData.Profiles.FirstOrDefault(item =>
                    item.ProfileId == profileId.Value);

            if (profile == null)
            {
                HttpContext.Session.Clear();

                return RedirectToAction("Login", "Account");
            }

            int playlistCount =
                _appData.PlaylistItems.Count(item =>
                    item.ProfileId == profileId.Value);

            ViewBag.PlaylistCount = playlistCount;

            return View(profile);
        }
    }
}