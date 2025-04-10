﻿using FFhub_backend.Models.Requests;
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
        [Route("videosCount")]
        public async Task<IActionResult> GetVideosCount([FromQuery] string? tags)
        {
            List<int> tagsList = null;
            if (!string.IsNullOrEmpty(tags))
            {
                tagsList = tags.Split(',').Select(id => int.Parse(id)).ToList();
            }

            var result = await _dataService.GetVideosCount(tagsList);
            return Ok(result);
        }

        [HttpGet]
        [Route("videos")]
        public async Task<IActionResult> GetVideos([FromQuery] string? tags, int page)
        {
            List<int> tagsList = null;
            if (!string.IsNullOrEmpty(tags))
            {
                tagsList = tags.Split(',').Select(id => int.Parse(id)).ToList();
            }
            
            var result = await _dataService.GetVideos(page, tagsList);
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
            if(req.AdminPass == Environment.GetEnvironmentVariable("admin_pass"))
            {
                var result = await _dataService.ReviewVideoSuggestion(req.VideoId, req.Pass, req.Thumbnail);
                return Ok(result);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("addTags")]
        public async Task<IActionResult> AddTags([FromBody] AddTags req)
        {
            if (req.AdminPass == Environment.GetEnvironmentVariable("admin_pass"))
            {
                var result = await _dataService.AddTagsToVideo(req.VideoId, req.Tags);
                return Ok(result);
            }
            else
            {
                return Unauthorized();
            }
        }
               

        [HttpDelete]
        [Route("deletetag")]
        public async Task<IActionResult> DelteTagFromVideo([FromQuery] int videoId, string tag, string adminPass)
        {
            if (adminPass == Environment.GetEnvironmentVariable("admin_pass"))
            {
                var result = await _dataService.DeleteTagFromVideo(videoId, tag);
                return Ok(result);
            }
            return Unauthorized();
               
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delte([FromQuery] int id, string adminPass)
        {
            if (adminPass == Environment.GetEnvironmentVariable("admin_pass"))
            {
                var result = await _dataService.DeleteVideo(id);
                return Ok(result);
            }
            return Unauthorized(); 
        }
    }
}
