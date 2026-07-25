namespace MusicPlaylist.Models
{
    public class HomeViewModel
    {
        public string ProfileName { get; set; } = string.Empty;

        public List<MusicVideo> TopSongs { get; set; } = new();

        public int TotalProfiles { get; set; }

        public int TotalSongs { get; set; }

        public int TotalStreams { get; set; }
    }
}
