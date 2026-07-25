using Microsoft.AspNetCore.Mvc;
using MusicPlaylist.Models;

namespace MusicPlaylist.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppData _appData;

        public ProfileController(AppData appData)
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

            var user = _appData.Profiles.FirstOrDefault(u => u.ProfileId == profileId);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.FullName = user.FullName;
            ViewBag.Username = user.Username;
            ViewBag.PlaylistCount = user.PlaylistCount;

            return View();
        }
    }
}