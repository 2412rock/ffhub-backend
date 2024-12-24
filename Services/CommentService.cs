using FFhub_backend.Database;
using FFhub_backend.Database.Models;
using FFhub_backend.HttpReponse;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace FFhub_backend.Services
{
    public class CommentService
    {
        private readonly FFDbContext _dbContext;

        public CommentService(FFDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Maybe<List<DBComment>>> GetComments(int videoId)
        {
            var maybe = new Maybe<List<DBComment>>();
            try
            {
                var comments = await _dbContext.Comments.Where(e => e.VideoId == videoId).ToListAsync();
                maybe.SetSuccess(comments);
                return maybe;
                
            }
            catch(Exception e)
            {
                maybe.SetException(e.Message);
            }

            return maybe;
        }

        public async Task<Maybe<string>> PostComment(int videoId, string commentText)
        {
            var maybe = new Maybe<string>();
            try
            {
                await _dbContext.Comments.AddAsync(new DBComment() { CommentText = commentText, VideoId = videoId });
                await _dbContext.SaveChangesAsync();
                maybe.SetSuccess("ok");
                return maybe;

            }
            catch (Exception e)
            {
                maybe.SetException(e.Message);
            }

            return maybe;
        }

        public async Task<Maybe<string>> DeleteComment(int videoId, int commendId)
        {
            var maybe = new Maybe<string>();
            try
            {
                var comment = await _dbContext.Comments.FirstOrDefaultAsync(e => e.VideoId == videoId && e.CommentId == commendId);
                if(comment != null)
                {
                    _dbContext.Comments.Remove(comment);
                    await _dbContext.SaveChangesAsync();
                    maybe.SetSuccess("ok");
                }
                else
                {
                    maybe.SetException("Could not find comment");
                }
            }
            catch (Exception e)
            {
                maybe.SetException(e.Message);
            }

            return maybe;
        }
    }
}
