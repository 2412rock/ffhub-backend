using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FFhub_backend.Database.Models
{
    public class DBVideo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VideoId { get; set; }
        public string Title { get; set; }
        public string Link  { get; set; }
        public bool IsSuggestion { get; set; }

        public string ThumbNail { get; set; }

        public ICollection<DBVideoTag> VideoTags { get; set; } = new List<DBVideoTag>();
    }
}
