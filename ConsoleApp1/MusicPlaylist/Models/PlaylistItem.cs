namespace MusicPlaylist.Models
{
    public class PlaylistItem
    {
        public int ProfileId { get; set; }

        public string VideoId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string ChannelTitle { get; set; } = string.Empty;

        public string ThumbnailUrl { get; set; } = string.Empty;
    }
}
