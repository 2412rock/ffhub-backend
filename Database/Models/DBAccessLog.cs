using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FFhub_backend.Database.Models
{
    public class DBAccessLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string IpAddress { get; set; }

        public DateTime AccessTime { get; set; }

    }
}
