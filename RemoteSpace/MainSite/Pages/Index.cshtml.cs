using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MainSite.Connect;
using MainSite.Connect.Models;

namespace MainSite.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        public string path;
        public string OldPath;
        public IndexModel(ILogger<IndexModel> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            Filelist = new List<FileElement>();
        }
        [BindProperty]
        public List<FileElement> Filelist { get; set; }
        public async Task<IActionResult> OnGet(string spath)
        {
            if(spath!= "\\" + User.Identity.Name)
            {
                OldPath = path;
            }
            path = spath;
            if(string.IsNullOrWhiteSpace(path))
            {
                path = "\\" + User.Identity.Name;
            }
            if (!path.StartsWith("\\"))
            {
                path = "\\"+path;
            }
            if (!path.StartsWith("\\"+User.Identity.Name))
            {
                return RedirectToPage("/Error");
            }
            
            try
            {
                Filelist = Api.GetDirFiles(path).Result;
            }
            catch(Exception ex)
            {
                if(ex.InnerException.Message=="NotFound")
                {
                    if (!Api.AddDir(User.Identity.Name, "", User.Identity.Name).Result)
                    {
                        throw new Exception("Failed to create root dir");
                    }
                }
                else
                {
                    throw ex.InnerException;
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPost(string DirName,string path)
        {
            if (!path.StartsWith("\\"))
            {
                path = "\\" + path;
            }
            if (!path.StartsWith("\\" + User.Identity.Name))
            {
                return RedirectToPage("/Error");
            }
            try
            {
                if (!Api.CheckPath("\\" + User.Identity.Name).Result)
                {
                    if (!Api.AddDir(User.Identity.Name, "", User.Identity.Name).Result)
                    {
                        throw new Exception("Failed to create root dir");
                    }
                }
                if (!Api.AddDir(DirName,path, User.Identity.Name).Result)
                {
                    throw new Exception("Failed");
                }
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
            return RedirectToPage("/Index", new {spath=path});
        }
    }
}
