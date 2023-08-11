using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repository
{
    public class BlogPostLikeRepository : IBlogPostLikeRepository
    {
        private readonly BloggieDbContext _bloggieDbContext;

        public BlogPostLikeRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbContext = bloggieDbContext;
        }

        public async Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike)
        {
           await _bloggieDbContext.BlogPostsLike.AddAsync(blogPostLike);
           await _bloggieDbContext.SaveChangesAsync(); 
           return blogPostLike; 
        }

        public async Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostID)
        {
           return await _bloggieDbContext.BlogPostsLike.Where(x=>x.BlogPostId == blogPostID).ToListAsync();
        }

        public async Task<int> GetTotalLikes(Guid blogPostID)
        {
            return await _bloggieDbContext.BlogPostsLike.CountAsync(x=>x.BlogPostId==blogPostID);
        }
    }
}
