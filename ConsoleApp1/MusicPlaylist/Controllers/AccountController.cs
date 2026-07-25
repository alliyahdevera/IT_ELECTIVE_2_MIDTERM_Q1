using Microsoft.AspNetCore.Mvc;
using MusicPlaylist.Models;
using MusicPlaylist.Services;

namespace MusicPlaylist.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDataService _appData;

        public AccountController(AppDataService appData)
        {
            _appData = appData;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("ProfileId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            UserProfile? profile =
                _appData.Profiles.FirstOrDefault(item =>
                    item.Username.Equals(
                        model.Username.Trim(),
                        StringComparison.OrdinalIgnoreCase) &&
                    item.Password == model.Password);

            if (profile == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid username or password.");

                return View(model);
            }

            HttpContext.Session.SetInt32(
                "ProfileId",
                profile.ProfileId);

            HttpContext.Session.SetString(
                "ProfileName",
                profile.FullName);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }
    }
}