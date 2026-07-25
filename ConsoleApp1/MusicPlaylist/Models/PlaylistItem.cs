using System.ComponentModel.DataAnnotations;

namespace MusicPlaylistMvc.Models
{
    public class PlaylistItem
    {
        public int ProfileId { get; set; }

        public string VideoId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the song title.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a YouTube URL.")]
        [Display(Name = "YouTube URL")]
        public string YouTubeUrl { get; set; } = string.Empty;

        public string EmbedUrl { get; set; } = string.Empty;

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}