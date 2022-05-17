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
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using MainSite.Data;
using MainSite.Connect.Cryptography;

namespace MainSite.Pages.Main
{
    public class Upload
    {
        [Required]
        [Display(Name = "Add a file")]
        [DataType(DataType.Upload)]
        public IFormFile Submittedfile { get; set; }
    }
    [DisableRequestSizeLimit]
    [Authorize]
    public class AddfileModel : PageModel
    {
        private readonly UserManager<UserIdentityCompleted> _UserManager;
        public AddfileModel(UserManager<UserIdentityCompleted> userManager)
        {
            _UserManager = userManager;
        }
        public void OnGet(string path)
        {
            _path = path;
        }
        public string _path;
        [BindProperty]
        public Upload FileUpload { get; set; }
  
        public async Task<IActionResult> OnPost(string path,string desc)
        {
           if(!ModelState.IsValid)
            {
                _path = path;
                return Page();
            }
            var request = new FileElementAddRequest();
            request.FileInfo = new FileElementSend() { Name= FileUpload.Submittedfile.FileName , Path= path, Owner= User.Identity.Name,Description= desc ,IsDirectory=false,Shared=false};
            using (var ms = new MemoryStream())
            {
                var user = _UserManager.GetUserAsync(User).Result;
                FileUpload.Submittedfile.CopyTo(ms);
               
                request.Content = Convert.ToBase64String(FileData.Encrypt(ms.ToArray(), user.Key, user.IV));
                // act on the Base64 data
            }
            try
            {
                if(!Api.CheckPath("\\"+ User.Identity.Name).Result)
                {
                    if(!Api.AddDir(User.Identity.Name,"", User.Identity.Name).Result)
                    {
                        return RedirectToPage("../Error", new { Error = "Failed to create root dir" });
                    }
                }    
                var response = Api.AddFile(request).Result;
                if(response.Count==0)
                {
                    RedirectToPage("../Error", new { Error = "Service unavailable" });
                }
            }
            catch(Exception ex)
            {
                RedirectToPage("../Error", new { Error = ex.Message });
            }
            return RedirectToPage("../Index", new { spath = path });
        }
    }
}
