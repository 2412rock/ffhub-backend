namespace FFhub_backend.Models.Requests
{
    public class VideoSuggestion
    {
        public string Title { get; set; }
        public string Link { get; set; }

        public List<string> Tags { get; set; }
    }
}
