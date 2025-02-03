namespace FFhub_backend.Models
{
    public class VideoAndTags
    {
        public int VideoId { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string ThumbNail { get; set; }
        public List<string> Tags { get; set; }
        public int TotalVideos { get; set; }
        public int Views { get; set; }
    }
}
