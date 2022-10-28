using MainSite.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using MainSite.Connect;
using System.Collections.Generic;

namespace MainSite.Pages.Main
{
    [Authorize]
    public class AdminCPModel : PageModel
    {
        public long FreeSpace;
        public Dictionary<string, long> UserSpace; 
        private readonly UserManager<BaseUser> _UserManager;
        public AdminCPModel(UserManager<BaseUser> userManager)
        {
            _UserManager = userManager;
        }
        public async Task<IActionResult> OnGet()
        {
            var users = _UserManager.Users;
            if (!users.Where(x => x.Email == User.Identity.Name).First().Admin)
            {
                RedirectToPage("../Error", new { Error = "Unautorized" });
            }
            UserSpace = Api.GetDirSizes(users.Select(x => x.UserName).ToList()).Result;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (!_UserManager.Users.Where(x => x.Email == User.Identity.Name).First().Admin)
            {
                RedirectToPage("../Error", new { Error = "Unautorized" });
            }
            return Page();
        }
    }
}
