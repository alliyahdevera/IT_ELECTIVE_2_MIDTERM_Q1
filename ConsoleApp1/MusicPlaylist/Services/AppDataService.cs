using MusicPlaylistMvc.Models;

namespace MusicPlaylistMvc.Services
{
    public class AppDataService
    {
        // List of existing user accounts.
        public List<UserProfile> Profiles { get; set; } = new();

        // Stores the personal playlist of every profile.
        public List<PlaylistItem> PlaylistItems { get; set; } = new();

        // Stores songs and their overall play counts.
        public List<MusicVideo> MusicVideos { get; set; } = new();

        public AppDataService()
        {
            // Sample accounts used by the Login page.
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
                    FullName = "Althea Vallejos",
                    Username = "althea",
                    Password = "password123"
                },

                new UserProfile
                {
                    ProfileId = 3,
                    FullName = "Clarisse Villamor",
                    Username = "clarisse",
                    Password = "password123"
                }
            });
        }
    }
}