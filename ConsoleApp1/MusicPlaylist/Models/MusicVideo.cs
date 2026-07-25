namespace MusicPlaylist.Models
{
    public class MusicVideo
    {
        public string VideoId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string YouTubeUrl { get; set; } = string.Empty;

        public string EmbedUrl { get; set; } = string.Empty;

        public string ThumbnailUrl { get; set; } = string.Empty;

        public int PlayCount { get; set; }
    }
}