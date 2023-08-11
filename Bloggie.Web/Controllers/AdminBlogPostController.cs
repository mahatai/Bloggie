using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdminBlogPostController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public AdminBlogPostController(ITagRepository tagRepository,IBlogPostRepository blogPostRepository)
        {
            _tagRepository = tagRepository;
            _blogPostRepository = blogPostRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var tags = await _tagRepository.GetAllAsync();
            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            var blogPostDomainModel = new BlogPost
            {
                 Heading = addBlogPostRequest.Heading,
                 PageTitle = addBlogPostRequest.PageTitle,
                 Content = addBlogPostRequest.Content,
                 ShortDescription = addBlogPostRequest.ShortDescription,
                 FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                 UrlHandle = addBlogPostRequest.UrlHandle,
                 PublishedDate = addBlogPostRequest.PublishedDate,
                 Author=addBlogPostRequest.Author,
                 Visible=addBlogPostRequest.Visible,

            };
            var selectedTag=new List<Tag>();
            foreach(var selectedTagId in addBlogPostRequest.SelectedTags)
            {
                var selectedAsGuid=Guid.Parse(selectedTagId);
                var existingTag=await _tagRepository.GetAsync(selectedAsGuid);
                if (existingTag != null)
                {
                    selectedTag.Add(existingTag);
                }
                blogPostDomainModel.Tags=selectedTag;
            }
            await _blogPostRepository.AddAsync(blogPostDomainModel);
            return RedirectToAction("List");
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var blogPosts= await _blogPostRepository.GetAllAsync();
            return View(blogPosts);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var existingBlogPost= await _blogPostRepository.GetAsync(id);
            var tagDomainmodel=await _tagRepository.GetAllAsync();
            if(existingBlogPost != null)
            {
                var model = new EditBlogPostRequest
                {
                    Id = existingBlogPost.Id,
                    Heading = existingBlogPost.Heading,
                    PageTitle = existingBlogPost.PageTitle,
                    Content = existingBlogPost.Content,
                    Author = existingBlogPost.Author,
                    FeaturedImageUrl = existingBlogPost.FeaturedImageUrl,
                    UrlHandle = existingBlogPost.UrlHandle,
                    ShortDescription = existingBlogPost.ShortDescription,
                    PublishedDate = existingBlogPost.PublishedDate,
                    Visible = existingBlogPost.Visible,
                    Tags = tagDomainmodel.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }),
                    SelectedTags = existingBlogPost.Tags.Select(x => x.Id.ToString()).ToArray()

                };
                return View(model);
            }
            return View(null);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            var blogPostDomainModel = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                PageTitle = editBlogPostRequest.PageTitle,
                Content = editBlogPostRequest.Content,
                Author = editBlogPostRequest.Author,
                FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
                UrlHandle = editBlogPostRequest.UrlHandle,
                ShortDescription = editBlogPostRequest.ShortDescription,
                PublishedDate = editBlogPostRequest.PublishedDate,
                Visible = editBlogPostRequest.Visible,

            };
            var selectedTags = new List<Tag>();
            foreach(var selectedtags in editBlogPostRequest.SelectedTags)
            {
                if(Guid.TryParse(selectedtags,out var tag))
                {
                    var foundtag= await _tagRepository.GetAsync(tag);
                    if(foundtag != null)
                    {
                        selectedTags.Add(foundtag);
                    }
                }
            }
            blogPostDomainModel.Tags = selectedTags;

          var updatedBlogpost=  await _blogPostRepository.UpdateAsync(blogPostDomainModel);
            if (updatedBlogpost != null)
            {
                return RedirectToAction("List");
            }
            return RedirectToAction("Edit");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            var existingBlogPost = await _blogPostRepository.DeleteAsync(editBlogPostRequest.Id);
            if (existingBlogPost != null)
            {
                return RedirectToAction("List");
            }
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
        }
    }
}
