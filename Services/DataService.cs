using FFhub_backend.Database;
using FFhub_backend.Database.Models;
using FFhub_backend.HttpReponse;
using FFhub_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FFhub_backend.Services
{
    public class DataService
    {
        private readonly FFDbContext _dbContext;

        public DataService(FFDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Maybe<VideoAndTags>> GetVideo(int id)
        {
            var maybe = new Maybe<VideoAndTags>();
            try
            {
                var video = await _dbContext.Videos.Include(v => v.VideoTags).ThenInclude(vt => vt.Tag).Where(e => e.VideoId == id && e.IsSuggestion == false).FirstOrDefaultAsync();
                if(video != null)
                {
                    var tags = new List<string>();
                    foreach (var sub in video.VideoTags)
                    {
                        tags.Add(sub.Tag.TagName);

                    }

                    var result = new VideoAndTags()
                    {
                        Title = video.Title,
                        ThumbNail = video.ThumbNail,
                        Link = video.Link,
                        Tags = tags
                    };

                    maybe.SetSuccess(result);
                }
                else
                {
                    maybe.SetException("Could not find video");
                }
            }
            catch (Exception e)
            {
                maybe.SetException("Something went wrong " + e.Message);
            }

            return maybe;
        }

        public async Task<Maybe<List<VideoAndTags>>> GetVideos(int page, List<int> tagsSearch, bool suggestion = false)
        {
            var maybe = new Maybe<List<VideoAndTags>>();
            try
            {
                var pageSize = 15; // Number of videos per page
                var skip = (page - 1) * pageSize;
                var videosAndTags = new List<VideoAndTags>();

                var query = _dbContext.Videos.Include(v => v.VideoTags).ThenInclude(vt => vt.Tag).Where(e => e.IsSuggestion == suggestion).AsQueryable();

                var videos = await query
                            .Skip(skip)
                            .Take(pageSize)
                            .ToListAsync();
                foreach (var video in videos)
                {
                    var tags = new List<string>();
                    var foundMatchinTag = tagsSearch != null ? false: true;
                    foreach(var sub in video.VideoTags)
                    {
                        if (tagsSearch != null && tagsSearch.Contains(sub.Tag.TagId))
                        {
                            foundMatchinTag = true;
                        }
                        tags.Add(sub.Tag.TagName);

                    }
                    if (!foundMatchinTag)
                    {
                        continue;
                    }
                    videosAndTags.Add(new VideoAndTags()
                    {
                        VideoId = video.VideoId,
                        Link = video.Link,
                        Title = video.Title,
                        ThumbNail = video.ThumbNail,
                        Tags = tags
                    });
                }
                maybe.SetSuccess(videosAndTags);
            }
            catch(Exception e)
            {
                maybe.SetException("Something went wrong " + e.Message);
            }
            
            return maybe;
        }

        public async Task<Maybe<List<DBTag>>> GetTags(string startsWith = "")
        {
            var maybe = new Maybe<List<DBTag>>();
            try
            {
                var tags = await _dbContext.Tags.Where(e => e.IsSuggestion == false && e.TagName.ToLower().StartsWith(startsWith)).ToListAsync();
                maybe.SetSuccess(tags);
            }
            catch (Exception e) 
            {
                maybe.SetException("Error occured " + e.Message);
            }

            return maybe;
        }

        public async Task<Maybe<string>> AddVideoSuggestion(string link, string title, List<string> tags)
        {
            var maybe = new Maybe<string>();
            try
            {
                var video = new DBVideo()
                {
                    Link = link,
                    Title = title,
                    IsSuggestion = true,
                    ThumbNail = ""
                };

                await _dbContext.Videos.AddAsync(video);

                await _dbContext.SaveChangesAsync();

                for(int i=0; i < tags.Count; i++)
                {
                    var tag = await _dbContext.Tags.FirstOrDefaultAsync(e => e.TagName.ToLower() == tags[i]);
                    if (tag == null)
                    {
                        tag = new DBTag()
                        {
                            TagName = tags[i],
                            IsSuggestion = true,
                        };
                        await _dbContext.Tags.AddAsync(tag);
                        await _dbContext.SaveChangesAsync();
                    }
                    var videoAndTag = new DBVideoTag()
                    {
                        VideoId = video.VideoId,
                        TagId = tag.TagId,
                    };
                    await _dbContext.VideoTags.AddAsync(videoAndTag);
                }
                await _dbContext.SaveChangesAsync();
                maybe.SetSuccess("ok");
            }
            catch(Exception e)
            {
                maybe.SetException("Error " + e.Message);
            }
            return maybe;
        }

        public async Task<Maybe<string>> ReviewVideoSuggestion(int videoId, bool pass, string thumbnail = "") {
            var maybe = new Maybe<string>();

            try
            {
                var video = await _dbContext.Videos.Include(v => v.VideoTags).ThenInclude(vt => vt.Tag).FirstOrDefaultAsync(e => e.VideoId == videoId);
                if (video != null)
                {
                    if (pass)
                    {
                        video.IsSuggestion = false;
                        video.ThumbNail = thumbnail;
                        var videoTags = await _dbContext.VideoTags.Include(v => v.Tag).Where(e => e.VideoId == video.VideoId).ToListAsync();
                        for (int i = 0; i < videoTags.Count; i++)
                        {
                            var tag = videoTags[i].Tag;
                            tag.IsSuggestion = false;
                            _dbContext.Tags.Update(tag);
                        }
                        _dbContext.Videos.Update(video);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var videoTags = await _dbContext.VideoTags.Include(v => v.Tag).Where(e => e.VideoId == video.VideoId).ToListAsync();
                        for (int i = 0; i < videoTags.Count; i++)
                        {
                            var tag = videoTags[i].Tag;
                            _dbContext.Tags.Remove(tag);
                            _dbContext.VideoTags.Remove(videoTags[i]);
                        }
                        _dbContext.Videos.Remove(video);
                        await _dbContext.SaveChangesAsync();
                    }
                    maybe.SetSuccess("ok");
                }
            }
            catch (Exception e)
            {
                maybe.SetException("Error " + e.Message);
            }
            return maybe;
        }
    }
}
