using MusicPlaylist.Models;

namespace MusicPlaylistMvc.Services
{
    public class AppDataService
    {
        public List<UserProfile> Profiles { get; set; } = new();

        public List<PlaylistItem> PlaylistItems { get; set; } = new();

        public List<MusicVideo> MusicVideos { get; set; } = new();

        public AppDataService()
        {
            Profiles.AddRange(new[]
            {
                new UserProfile
                {
                    ProfileId = 1,
                    FullName = "Alliyah De Vera",
                    Username = "alliyah",
                    Password = "password123"
                },

                new UserProfile
                {
                    ProfileId = 2,
                    FullName = "Sophia Solis",
                    Username = "soph",
                    Password = "password123"
                },

                new UserProfile
                {
                    ProfileId = 3,
                    FullName = "Kevin Roque",
                    Username = "kev",
                    Password = "password123"
                }
            });
        }
    }
}