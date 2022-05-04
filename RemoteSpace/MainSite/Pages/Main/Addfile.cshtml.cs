using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainSite.Connect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MainSite.Connect.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace MainSite.Pages.Main
{
    public class AddfileModel : PageModel
    {
        private readonly UserManager<IdentityUser> _UserManager;
        public AddfileModel(UserManager<IdentityUser> userManager)
        {
            _UserManager = userManager;
        }
        public void OnGet(string path,UserManager<IdentityUser> userManager)
        {
            
        }
        public string _path;
        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task<IActionResult> OnPost(string desc)
        {

            var request = new FileElementAddRequest();
            request.FileInfo = new FileElementSend() { Name= Upload.FileName , Path= _path, Owner= User.Identity.Name,Description= desc ,IsDirectory=false,Shared=false};
            using (var ms = new MemoryStream())
            {
                Upload.CopyTo(ms);
                request.Content = ms.ToArray();
                // act on the Base64 data
            }
            try
            {
                var response = Api.AddFile(request);
            }
            catch
            {
                return RedirectToPage("Error");
            }
            return RedirectToPage("Index");
        }
    }
}
