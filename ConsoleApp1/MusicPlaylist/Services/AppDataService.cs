using MusicPlaylist.Models;

namespace MusicPlaylist.Services
{
    public class AppDataService
    {
        public List<UserProfile> Profiles { get; } = new();

        public List<PlaylistItem> PlaylistItems { get; } = new();

        public List<MusicVideo> MusicVideos { get; } = new();

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
                    Username = "kcer",
                    Password = "password123"
                }
            });
        }
    }
}