namespace FFhub_backend.Models.Requests
{
    public class AddTags
    {
        public int VideoId { get; set; }
        public List<string> Tags { get; set; }
    }
}
