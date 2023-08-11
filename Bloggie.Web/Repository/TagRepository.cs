using Azure;
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly BloggieDbContext _bloggieDbContext;

        public TagRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbContext = bloggieDbContext;
        }
        public async Task<Tag> AddAsync(Tag tag)
        {
            await _bloggieDbContext.Tags.AddAsync(tag);
            await _bloggieDbContext.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var tag = await _bloggieDbContext.Tags.FindAsync(id);
            if (tag != null)
            {
                _bloggieDbContext.Tags.Remove(tag);
                await _bloggieDbContext.SaveChangesAsync();
                return tag;
            }
            return null;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
           return await _bloggieDbContext.Tags.ToListAsync();
        }

        public async Task<Tag?> GetAsync(Guid id)
        {
            return await _bloggieDbContext.Tags.FirstAsync(x => x.Id == id);
        }

        public  async Task<Tag?> UpdateAsync(Tag tag)
        {
            var existingtag= await _bloggieDbContext.Tags.FindAsync(tag.Id);
            if (existingtag != null)
            {
                existingtag.Name = tag.Name;
                existingtag.DisplayName = tag.DisplayName;
                _bloggieDbContext.SaveChanges();
                return existingtag;
            }
            return null;
        }
    }
}
