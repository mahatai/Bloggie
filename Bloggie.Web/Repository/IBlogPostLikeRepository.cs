using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repository
{
    public interface IBlogPostLikeRepository
    {
        Task<int> GetTotalLikes(Guid blogPostID);
        Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostID);
        Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike);

    }
}
