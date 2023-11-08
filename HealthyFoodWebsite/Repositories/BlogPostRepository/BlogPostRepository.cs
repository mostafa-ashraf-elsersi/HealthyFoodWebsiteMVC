using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.PaginationEngine;
using HealthyFoodWebsite.EmailTemplate;
using HealthyFoodWebsite.Repositories.BlogSubscriberRepository;
using Microsoft.EntityFrameworkCore;

namespace HealthyFoodWebsite.Repositories.BlogRepository
{
    public class ThreadGatheringObject
    {
        public BlogPost BlogPost { get; set; } = null!;
        public List<BlogSubscriber> Subscribers { get; set; } = null!;
        public EmailFactory EmailObject { get; set; } = null!;
    }

    public class BlogPostRepository : AbstractBlogPostRepository
    {
        // Object Fields Zone
        private readonly HealthyFoodDbContext dbContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AbstractBlogSubscriberRepository blogSubscriberRepository;
        private readonly ImageUploader.ImageUploader imageUploader;
        private readonly EmailFactory emailObject;
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);
        private Thread mailThread;


        // Dependency Injection Zone
        public BlogPostRepository(HealthyFoodDbContext dbContext,
                                  IHttpContextAccessor httpContextAccessor,
                                  AbstractBlogSubscriberRepository blogSubscriberRepository,
                                  ImageUploader.ImageUploader imageUploader,
                                  EmailFactory emailObject)
        {
            this.dbContext = dbContext;
            this.httpContextAccessor = httpContextAccessor;
            this.blogSubscriberRepository = blogSubscriberRepository;
            this.imageUploader = imageUploader;
            this.emailObject = emailObject;
            mailThread = null!;
        }


        // Object Methods Zone
        public override async Task<(List<BlogPost>, int)> GetPagesWithCountAsync(int pageIndex)
        {
           try
            {
                await semaphoreSlim.WaitAsync(-1);

                var posts = httpContextAccessor.HttpContext!.User.IsInRole("BusinessOwner") ?
                              await dbContext
                                    .Blog
                                    .AsNoTracking()
                                    .ToListAsync() :
                              await dbContext
                                    .Blog
                                    .Where(post => post.IsDisplayed)
                                    .AsNoTracking()
                                    .ToListAsync();

                semaphoreSlim.Release();

                var pagesList = posts.GetPages(6).ToList();

                var pagesCount = pagesList.Count;

                return (pagesList[pageIndex], pagesCount);
            }
            catch
            {
                return (new List<BlogPost>(),  0);
            }
        }

        public override async Task<List<BlogPost>> GetLastThreePostsAsync()
        {
            await semaphoreSlim.WaitAsync(-1);

            var posts = await Task.Run(() => dbContext
                                                                 .Blog
                                                                 .Where(post => post.IsDisplayed)
                                                                 .AsEnumerable()
                                                                 .TakeLast(3)
                                                                 .ToList());

            semaphoreSlim.Release();

            return posts;
        }

        public override async Task<BlogPost?> GetByIdAsync(int id)
        {
            await semaphoreSlim.WaitAsync(-1);

            var post = httpContextAccessor.HttpContext!.User.IsInRole("BusinessOwner") ?
                              await dbContext
                                    .Blog
                                    .FindAsync(id) :
                              await dbContext
                                    .Blog
                                    .FirstOrDefaultAsync(post => post.Id == id && post.IsDisplayed);

            semaphoreSlim.Release();

            return post;
        }

        public override async Task<bool> InsertAsync(BlogPost entity)
        {
            try
            {

                string? posterUri = await imageUploader.UploadImageToServerAsync(entity.PosterFile!, "\\img\\blogPosts\\");
                entity.PosterUri = posterUri;

                await semaphoreSlim.WaitAsync(-1);

                await dbContext.Blog.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                var gatheringObject = new ThreadGatheringObject
                {
                    BlogPost = entity,
                    Subscribers = await blogSubscriberRepository.GetAllAsync(),
                    EmailObject = emailObject
                };

                mailThread = new(new ParameterizedThreadStart(async (gatheringObject) =>
                {
                    try
                    {
                        var _gatheringObject = (ThreadGatheringObject)gatheringObject!;

                        var _entity = _gatheringObject.BlogPost;
                        var _subscribers = _gatheringObject.Subscribers;
                        var _emailObject = _gatheringObject.EmailObject;

                        foreach (var subscriber in _subscribers)
                        {
                            await _emailObject.NotifyBlogSubscriberAsync(_entity.Title,
                                                        subscriber.UserName,
                                                        subscriber.EmailAddress);
                        }
                    }
                    catch
                    {
                    }

                }));

                mailThread.Start(gatheringObject);

                return true;
            }
            catch
            {
                return false;
            }

        }

        public override async Task<bool> UpdateAsync(BlogPost entity)
        {
            try
            {
                if (entity.PosterFile is not null)
                {
                    if (!imageUploader.DeleteImageFromServerAsync(entity.PosterUri))
                        throw new Exception("An error occurred in the deletion function of the image uploader service.");

                    var posterUri = await imageUploader.UploadImageToServerAsync(entity.PosterFile, "\\img\\blogPosts\\");
                    entity.PosterUri = posterUri;
                }

                await semaphoreSlim.WaitAsync(-1);

                dbContext.ChangeTracker.Clear();

                dbContext.Blog.Update(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> DeactivateAsync(BlogPost entity)
        {
            try
            {
                if (entity.IsDisplayed == true)
                    entity.IsDisplayed = false;

                else if (entity.IsDisplayed == false)
                    entity.IsDisplayed = true;
                
                else
                    throw new Exception();

                await semaphoreSlim.WaitAsync(-1);

                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override async Task<bool> DeleteAsync(BlogPost entity)
        {
            try
            {
                await semaphoreSlim.WaitAsync(-1);

                dbContext.Blog.Remove(entity);
                await dbContext.SaveChangesAsync();

                semaphoreSlim.Release();

                return true;
            }
            catch
            {
                return false;
            }
        }


        // Disposing Objects Zone
        ~BlogPostRepository()
        {
            semaphoreSlim.Dispose();
        }
    }
}