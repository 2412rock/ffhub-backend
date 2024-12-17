namespace FFhub_backend.Database.Models
{
    public class DBVideoTag
    {
        public int VideoId { get; set; }
        public DBVideo Video { get; set; } = null!; // Navigation property

        public int TagId { get; set; }
        public DBTag Tag { get; set; } = null!; // Navigation property
    }
}
