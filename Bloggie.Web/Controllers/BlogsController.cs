using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IBlogPostLikeRepository _blogPostLikeRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBlogPostCommentRepository _blogPostCommentRepository;

        public BlogsController(IBlogPostRepository blogPostRepository,
                                 IBlogPostLikeRepository blogPostLikeRepository,
                                    SignInManager<IdentityUser> signInManager,
                                       UserManager<IdentityUser> userManager ,
                                           IBlogPostCommentRepository blogPostCommentRepository)  
                                                              
        {
            _blogPostRepository = blogPostRepository;
            _blogPostLikeRepository = blogPostLikeRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _blogPostCommentRepository = blogPostCommentRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var liked = false;
            var blogpost= await _blogPostRepository.GetUrlHandleAsync(urlHandle);
            var blogDetailsViewModel = new BlogDetailsViewModel();
          
            if(blogpost!= null) 
            {
                var totalLikes =await _blogPostLikeRepository.GetTotalLikes(blogpost.Id);
                if (_signInManager.IsSignedIn(User))
                {
                    var likesforBlog = await _blogPostLikeRepository.GetLikesForBlog(blogpost.Id);
                    var userId = _userManager.GetUserId(User);
                    if(userId!= null)
                    {
                        var likeFromUser=likesforBlog.FirstOrDefault(x=>x.UserId==Guid.Parse(userId));
                        if(likeFromUser!=null)
                        {
                            liked=true;
                        }
                    }
                }
              var  blogComments= await _blogPostCommentRepository.GetCommentsBlogIdAsync(blogpost.Id);
              var blogCommentsForView = new List<BlogComment>();
                foreach(var comment in blogComments)
                {
                    blogCommentsForView.Add(new BlogComment
                    {
                         Desription=comment.Description,
                         DateAdded=comment.DateAdded,
                         Username=(await _userManager.FindByIdAsync(comment.UserId.ToString())).UserName
                    });
                }

                blogDetailsViewModel = new BlogDetailsViewModel
                {
                    Id = blogpost.Id,
                    Content = blogpost.Content,
                    PageTitle=blogpost.PageTitle,
                    Author=blogpost.Author,
                    FeaturedImageUrl=blogpost.FeaturedImageUrl,
                    Heading=blogpost.Heading,
                    PublishedDate=blogpost.PublishedDate,
                    ShortDescription=blogpost.ShortDescription,
                    UrlHandle=blogpost.UrlHandle,
                    Visible=blogpost.Visible,
                    Tags=blogpost.Tags,
                    TotalLikes=totalLikes,
                    Liked=liked,
                    Comments= blogCommentsForView
                };
            }
            return View(blogDetailsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BlogDetailsViewModel blogDetailsViewModel)
        {
          if(_signInManager.IsSignedIn(User))
            {
                var blogPostCommentModel = new BlogPostComment
                {
                    BlogPostId = blogDetailsViewModel.Id,
                    Description = blogDetailsViewModel.CommentDescription,
                    UserId=Guid.Parse(_userManager.GetUserId(User)),
                    DateAdded=DateTime.Now
                };
              await _blogPostCommentRepository.AddAsync(blogPostCommentModel);
              return RedirectToAction("Index", "Blogs", new {urlHandle=blogDetailsViewModel.UrlHandle});
            }
            return View();
        }
    }
}
