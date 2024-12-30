namespace FFhub_backend.Models.Requests
{
    public class ReviewSuggestion
    {
        public int VideoId { get; set; }
        public bool Pass { get; set; }
        public string? Thumbnail { get; set; }
        public string AdminPass { get; set; }
    }
}
