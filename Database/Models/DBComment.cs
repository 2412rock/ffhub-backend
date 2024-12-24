using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FFhub_backend.Database.Models
{
    public class DBComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }
        public int VideoId { get; set; }
        public string CommentText { get; set; }
    }
}
