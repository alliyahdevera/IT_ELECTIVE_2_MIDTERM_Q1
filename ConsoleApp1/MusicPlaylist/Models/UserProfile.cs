namespace MusicPlaylist.Models
{
    public class UserProfile
    {
        public int ProfileId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}