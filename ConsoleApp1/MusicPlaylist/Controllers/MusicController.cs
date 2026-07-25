using Microsoft.AspNetCore.Mvc;
using MusicPlaylist.Models;
using MusicPlaylistMvc.Services;

namespace MusicPlaylistMvc.Controllers
{
    public class MusicController : Controller
    {
        private readonly YouTubeService _youTubeService;
        private readonly AppDataService _appData;
        private readonly ILogger<MusicController> _logger;

        public MusicController(
            YouTubeService youTubeService,
            AppDataService appData,
            ILogger<MusicController> logger)
        {
            _youTubeService = youTubeService;
            _appData = appData;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Search));
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            MusicSearchViewModel model,
            CancellationToken cancellationToken)
        {
            int? profileId =
                HttpContext.Session.GetInt32("ProfileId");

            if (profileId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            model.PlaylistVideoIds =
                _appData.PlaylistItems
                    .Where(item =>
                        item.ProfileId == profileId.Value)
                    .Select(item => item.VideoId)
                    .ToList();

            if (string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                return View(model);
            }

            try
            {
                model.SearchResults =
                    await _youTubeService.SearchMusicAsync(
                        model.SearchTerm,
                        cancellationToken);
            }
            catch (InvalidOperationException exception)
            {
                model.ErrorMessage = exception.Message;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(
                    exception,
                    "An error occurred while searching YouTube.");

                model.ErrorMessage =
                    "Unable to search YouTube right now. Check the API key and internet connection.";
            }
            catch (TaskCanceledException)
            {
                model.ErrorMessage =
                    "The YouTube request took too long. Please try again.";
            }

            return View(model);
        }
    }
}