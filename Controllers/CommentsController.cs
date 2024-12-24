using FFhub_backend.Models.Requests;
using FFhub_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FFhub_backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly CommentService _commentService;
        public CommentsController(CommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        [Route("comments")]
        public async Task<IActionResult> GetComments([FromQuery] int videoId)
        {
            var result = await _commentService.GetComments(videoId);
            return Ok(result);
        }

        [HttpPost]
        [Route("postcomment")]
        public async Task<IActionResult> PostComment([FromBody] Comment req)
        {
            var result = await _commentService.PostComment(req.VideoId, req.CommentText);
            return Ok(result);
        }

        [HttpDelete]
        [Route("deletecomment")]
        public async Task<IActionResult> DeleteComment([FromQuery] int videoId, int commentId)
        {
            var result = await _commentService.DeleteComment(videoId, commentId);
            return Ok(result);
        }
    }
}
