using FFhub_backend.Models.Requests;
using FFhub_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FFhub_backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly DataService _dataService;
        public DataController(DataService dataService)
        {
            _dataService = dataService;
        }
        [HttpGet]
        [Route("videos")]
        public async Task<IActionResult> GetVideos([FromQuery] string? tags)
        {
            List<int> tagsList = null;
            if (!string.IsNullOrEmpty(tags))
            {
                tagsList = tags.Split(',').Select(id => int.Parse(id)).ToList();
            }
            
            var result = await _dataService.GetVideos(1, tagsList);
            return Ok(result);
        }

        [HttpGet]
        [Route("videosuggestions")]
        public async Task<IActionResult> GetVideoSuggestions()
        { 
            var result = await _dataService.GetVideos(1, null, true);
            return Ok(result);
        }

        [HttpGet]
        [Route("video")]
        public async Task<IActionResult> GetVideo([FromQuery] int id)
        {
            var result = await _dataService.GetVideo(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("tags")]
        public async Task<IActionResult> GetTags([FromQuery] string query)
        {
            var result = await _dataService.GetTags(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("suggest")]
        public async Task<IActionResult> Suggest([FromBody] VideoSuggestion req)
        {
            var result = await _dataService.AddVideoSuggestion(req.Link, req.Title, req.Tags);
            return Ok(result);
        }

        [HttpPost]
        [Route("reviewSuggestion")]
        public async Task<IActionResult> ReviewSuggestion([FromBody] ReviewSuggestion req)
        {
            var result = await _dataService.ReviewVideoSuggestion(req.VideoId, req.Pass, req.Thumbnail);
            return Ok(result);
        }
    }
}
