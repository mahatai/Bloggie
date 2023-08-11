using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminUserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminUserController(IUserRepository userRepository,UserManager<IdentityUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task< IActionResult> List()
        {
            var users=await _userRepository.GetAll();
            var userViewModel = new UserViewModel();
            userViewModel.Users = new List<User>();

            foreach (var user in users)
            {
                userViewModel.Users.Add(new Models.ViewModels.User
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName,
                    EmailAddress = user.Email
                });
            }
            return View(userViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> List(UserViewModel request)
        {
            var identityUser = new IdentityUser
            {
                 UserName=request.Username,
                 Email=request.Email
            };
            var identityResult= await _userManager.CreateAsync(identityUser,request.Password);
            if (identityResult is not null)
            {
                if(identityResult.Succeeded)
                {
                    var roles = new List<string>{ "User" };
                    if(request.AdminRoleCheckBox)
                    {
                        roles.Add("Admin");
                    }
                    identityResult = await _userManager.AddToRolesAsync(identityUser, roles);
                    if(identityResult is not null && identityResult.Succeeded)
                    {
                        return RedirectToAction("List", "AdminUser");
                    }
                }
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
               var identityResult= await _userManager.DeleteAsync(user);
                if (identityResult is not null && identityResult.Succeeded)
                {
                    return RedirectToAction("List", "AdminUser");
                }
            }
            return View();
        }
    }
}
