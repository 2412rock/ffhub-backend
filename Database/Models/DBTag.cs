using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FFhub_backend.Database.Models
{
    public class DBTag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TagId { get; set; }
        
        public string TagName { get; set; }

        public bool IsSuggestion { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<DBVideoTag> VideoTags { get; set; } = new List<DBVideoTag>();
    }
}
