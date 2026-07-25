using Microsoft.AspNetCore.Mvc;
using MusicPlaylist.Models;

namespace MusicPlaylist.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var profile = _appData.Profiles.FirstOrDefault(item =>
                item.Username == model.Username &&
                item.Password == model.Password);

            if (profile == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid username or password."
                );

                return View(model);
            }

            HttpContext.Session.SetInt32(
                "ProfileId",
                profile.ProfileId
            );

            HttpContext.Session.SetString(
                "ProfileName",
                profile.FullName
            );

            return RedirectToAction(
                "Index",
                "Home"
            );
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login",
                "Account"
            );
        }
    }
}