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

namespace MainSite.Pages.Main
{
    public class Upload
    {
        [Display(Name = "Add a picture")]
        [DataType(DataType.Upload)]
        public IFormFile Submittedfile { get; set; }
    }
    public class AddfileModel : PageModel
    {
        private readonly UserManager<IdentityUser> _UserManager;
        public AddfileModel(UserManager<IdentityUser> userManager)
        {
            _UserManager = userManager;
        }
        public void OnGet(string path)
        {
            
        }
        public string _path;
        [BindProperty]
        public Upload FileUpload { get; set; }
        public async Task<IActionResult> OnPost(string desc)
        {
            var request = new FileElementAddRequest();
            request.FileInfo = new FileElementSend() { Name= FileUpload.Submittedfile.FileName , Path= _path, Owner= User.Identity.Name,Description= desc ,IsDirectory=false,Shared=false};
            using (var ms = new MemoryStream())
            {
                FileUpload.Submittedfile.CopyTo(ms);
                request.Content = ms.ToArray();
                // act on the Base64 data
            }
            try
            {
                if(!Api.CheckPath("\\"+ User.Identity.Name).Result)
                {
                    if(!Api.AddDir(User.Identity.Name,"", User.Identity.Name).Result)
                    {
                        throw new Exception("Failed to create root dir");
                    }
                }    
                var response = Api.AddFile(request).Result;
                if(response.Count==0)
                {
                    throw new Exception("Api not on");
                }
            }
            catch(Exception ex)
            {
                return RedirectToPage("Error.cshtml");
            }
            return RedirectToPage("Index.cshtml");
        }
    }
}
