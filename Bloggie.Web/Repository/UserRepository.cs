using Bloggie.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDBContext _authDBContext;

        public UserRepository(AuthDBContext authDBContext)
        {
            _authDBContext = authDBContext;
        }
        public async  Task<IEnumerable<IdentityUser>> GetAll()
        {
            var users  = await _authDBContext.Users.ToListAsync();
            var superAdminUser = await _authDBContext.Users.FirstOrDefaultAsync(x => x.Email == "superadmin@bloggie.com");
            if (superAdminUser != null)
            {
                users.Remove(superAdminUser);
            }
            return users;
        }
    }
}
