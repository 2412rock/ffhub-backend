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
        private readonly MailService _mailService;

        public DataService(FFDbContext dbContext, MailService mailService)
        {
            _dbContext = dbContext;
            _mailService = mailService;
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
                        Tags = tags,
                        Views = video.Views,
                    };
                    video.Views += 1;
                    // increase views
                    _dbContext.Videos.Update(video);
                    await _dbContext.SaveChangesAsync();
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

        public async Task<Maybe<int>> GetVideosCount(List<int> tagsSearch, bool suggestion = false)
        {
            var maybe = new Maybe<int>();
            var count = 0;
            try
            {
                var query = _dbContext.Videos.Include(v => v.VideoTags).ThenInclude(vt => vt.Tag).Where(e => e.IsSuggestion == suggestion).AsQueryable();

                var videos = await query.ToListAsync();


                foreach (var video in videos)
                {
                    var addVideo = true;

                    if (tagsSearch != null)
                    {
                        tagsSearch.ForEach(searchTagId =>
                        {
                            var match = video.VideoTags.FirstOrDefault(e => e.TagId == searchTagId);
                            if (match == null)
                            {
                                // this tag cannot be found, discard video
                                addVideo = false;
                            }
                        });
                    }

                    if (addVideo)
                    {
                        count++;
                    }
                }
                maybe.SetSuccess(count);
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
                var pageSize = 20; // Number of videos per page
                var skip = (page - 1) * pageSize;
                var videosAndTags = new List<VideoAndTags>();

                var query = _dbContext.Videos.Include(v => v.VideoTags).ThenInclude(vt => vt.Tag).Where(e => e.IsSuggestion == suggestion).AsQueryable();

                var videos = await query.ToListAsync();
                var lastVideoIndex = skip + pageSize;
                var currentVideoIndex = 0;


                foreach (var video in videos)
                {
                    var addVideo = true;

                    if(tagsSearch != null)
                    {
                        tagsSearch.ForEach(searchTagId => {
                            var match = video.VideoTags.FirstOrDefault(e => e.TagId == searchTagId);
                            if (match == null)
                            {
                                // this tag cannot be found, discard video
                                addVideo = false;
                            }
                        });
                    }

                    if (addVideo)
                    {
                        var tags = new List<string>();

                        foreach (var sub in video.VideoTags)
                        {
                            tags.Add(sub.Tag.TagName);
                        }
                        if(currentVideoIndex == lastVideoIndex)
                        {
                            break;
                        }
                        else if(currentVideoIndex >= skip)
                        {
                            videosAndTags.Add(new VideoAndTags()
                            {
                                VideoId = video.VideoId,
                                Link = video.Link,
                                Title = video.Title,
                                ThumbNail = video.ThumbNail,
                                Tags = tags,
                            });
                        }
                       currentVideoIndex++;
                    }
                    
                }

                videosAndTags.ForEach(e => {
                    e.TotalVideos = videosAndTags.Count();
                });
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
                var tags = await _dbContext.Tags.Where(e => e.IsSuggestion == false && e.TagName.ToLower().Contains(startsWith.ToLower())).ToListAsync();
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
                _mailService.SendMail();
                maybe.SetSuccess("ok");
            }
            catch(Exception e)
            {
                maybe.SetException("Error " + e.Message);
            }
            return maybe;
        }

        public async Task<Maybe<string>> DeleteVideo(int videoId)
        {
            var maybe = new Maybe<string>();
            try
            {
                var video = await _dbContext.Videos.Include(v => v.VideoTags).ThenInclude(vt => vt.Tag).FirstOrDefaultAsync(e => e.VideoId == videoId);
                if (video != null)
                {
                    var videoTags = await _dbContext.VideoTags.Include(v => v.Tag).Where(e => e.VideoId == video.VideoId).ToListAsync();
                    var comments = await _dbContext.Comments.Where(e => e.VideoId == videoId).ToListAsync();

                    for (int i = 0; i < videoTags.Count; i++)
                    {
                        var tag = videoTags[i].Tag;
                        _dbContext.VideoTags.Remove(videoTags[i]);
                    }

                    for(int i = 0; i < comments.Count; i++)
                    {
                        _dbContext.Comments.Remove(comments[i]);
                    }

                    _dbContext.Videos.Remove(video);
                    await _dbContext.SaveChangesAsync();
                    maybe.SetSuccess("ok");
                }
                else
                {
                    maybe.SetException("Video not found");
                }
            }
            catch (Exception e)
            {
                maybe.SetException(e.Message);
            }
            return maybe;
        }

        public async Task<Maybe<string>> AddTagsToVideo(int videoId, List<string> tags) {
            var maybe = new Maybe<string>();
            try
            {
                var video = await _dbContext.Videos.Include(v => v.VideoTags).ThenInclude(vt => vt.Tag).FirstOrDefaultAsync(e => e.VideoId == videoId);
                if(video != null)
                {
                    for (int i = 0; i < tags.Count; i++)
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
                else
                {
                    maybe.SetException("No video found");
                }
            }
            catch (Exception e)
            {
                maybe.SetException(e.Message);
            }

            return maybe;
        }

        public async Task<Maybe<string>> DeleteTagFromVideo(int videoId, string tag)
        {
            var maybe = new Maybe<string>();
            try
            {
                var video = await _dbContext.Videos.Include(v => v.VideoTags).ThenInclude(vt => vt.Tag).FirstOrDefaultAsync(e => e.VideoId == videoId);
                if(video != null)
                {
                    var tagObj = await _dbContext.Tags.FirstOrDefaultAsync(e => e.TagName == tag);
                    if(tagObj == null)
                    {
                        maybe.SetException("Tag not found");
                        return maybe;
                    }
                    var videoAndTag = video.VideoTags.FirstOrDefault(e => e.TagId == tagObj.TagId);
                    if(videoAndTag != null)
                    {
                        _dbContext.VideoTags.Remove(videoAndTag);
                        await _dbContext.SaveChangesAsync();
                        maybe.SetSuccess("ok");
                    }
                    else
                    {
                        maybe.SetException("Could not find tag for video");
                    }

                }
                else
                {
                    maybe.SetException("Video not found");
                }
            }
            catch (Exception e)
            {
                maybe.SetException(e.Message);
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
                            if (tag.IsSuggestion)
                            {
                                _dbContext.Tags.Remove(tag);
                            }
                            
                            _dbContext.VideoTags.Remove(videoTags[i]);
                        }
                        _dbContext.Videos.Remove(video);
                        await _dbContext.SaveChangesAsync();
                    }
                    maybe.SetSuccess("ok");
                }
                else
                {
                    maybe.SetException("Video not found");
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
