using MusicPlaylist.Models;

namespace MusicPlaylist.Controllers
{
    [HttpPost]
    public IActionResult Add(PlaylistItem item)
    {
        int? profileId = HttpContext.Session.GetInt32("ProfileId");

        if (profileId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        item.ProfileId = profileId.Value;

        bool alreadyAdded = _appData.PlaylistItems.Any(song =>
            song.ProfileId == profileId.Value &&
            song.VideoId == item.VideoId);

        if (!alreadyAdded)
        {
            _appData.PlaylistItems.Add(item);
        }

        return RedirectToAction("Index");
    }
}
