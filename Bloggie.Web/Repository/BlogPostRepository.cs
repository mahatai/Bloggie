using Azure;
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repository
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BloggieDbContext _bloggieDbContext;

        public BlogPostRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbContext = bloggieDbContext;
        }
        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await _bloggieDbContext.BlogPosts.AddAsync(blogPost);
            await _bloggieDbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingblogpost= await _bloggieDbContext.BlogPosts.FindAsync(id);
            if (existingblogpost != null)
            {
                _bloggieDbContext.BlogPosts.Remove(existingblogpost);
                await _bloggieDbContext.SaveChangesAsync();
                return existingblogpost;
            }
            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await _bloggieDbContext.BlogPosts.Include(x=>x.Tags).ToListAsync();
        }

        public async Task<BlogPost?> GetAsync(Guid id)
        {
            return await _bloggieDbContext.BlogPosts.Include(x=>x.Tags).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> GetUrlHandleAsync(string urlHandle)
        {
            return await _bloggieDbContext.BlogPosts.Include(x=>x.Tags)
                .FirstOrDefaultAsync(x=>x.UrlHandle == urlHandle);
        }

        public  async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var existingBlogPost=_bloggieDbContext.BlogPosts.Include(x=>x.Tags).
                                                  FirstOrDefault(x=>x.Id == blogPost.Id);

            if(existingBlogPost != null)
            {
                existingBlogPost.Id = blogPost.Id;
                existingBlogPost.Heading = blogPost.Heading;
                existingBlogPost.PageTitle = blogPost.PageTitle;
                existingBlogPost.Content = blogPost.Content;
                existingBlogPost.ShortDescription = blogPost.ShortDescription;  
                existingBlogPost.Author= blogPost.Author;
                existingBlogPost.FeaturedImageUrl = blogPost.FeaturedImageUrl;
                existingBlogPost.UrlHandle = blogPost.UrlHandle;
                existingBlogPost.Visible = blogPost.Visible;
                existingBlogPost.PublishedDate= blogPost.PublishedDate;
                existingBlogPost.Tags = blogPost.Tags;

                await _bloggieDbContext.SaveChangesAsync();
                return existingBlogPost;
            }

            return null;
        }
    }
}

