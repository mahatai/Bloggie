using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repository
{
    public class BlogPostCommentRepository : IBlogPostCommentRepository
    {
        private readonly BloggieDbContext _bloggieDbContext;

        public BlogPostCommentRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbContext = bloggieDbContext;
        }
        public async Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment)
        {
          await _bloggieDbContext.BlogPostComments.AddAsync(blogPostComment);
          await _bloggieDbContext.SaveChangesAsync();
          return blogPostComment;
        }

        public async Task<IEnumerable<BlogPostComment>> GetCommentsBlogIdAsync(Guid blogPostId)
        {
            return await _bloggieDbContext.BlogPostComments.Where(x => x.BlogPostId == blogPostId).ToListAsync();
        }
                                                         
    }
}
