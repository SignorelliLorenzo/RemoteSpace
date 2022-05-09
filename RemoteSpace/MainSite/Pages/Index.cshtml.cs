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
            Filelist = new List<FileDisplay>();
        }
        [BindProperty]
        public List<FileDisplay> Filelist { get; set; }
        public async Task<IActionResult> OnGet(string spath)
        {
            
            if (string.IsNullOrWhiteSpace(spath))
            {
                spath = "\\" + User.Identity.Name;
            }
            if (spath!= "\\" + User.Identity.Name)
            {
                OldPath = spath.Substring(0,spath.IndexOf("\\"+spath.Split("\\").Last()));
            }
            path = spath;
            
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
                var resultFilelist = Api.GetDirFiles(path).Result;
                Filelist = resultFilelist.Select(f => new FileDisplay(f)).ToList();
            }
            catch(Exception ex) 
            {
                if(ex.InnerException.Message=="NotFound" && path== "\\Admin@admin.com")
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
        public async Task<IActionResult> OnPost(string DirName,string path, string btn, int? Id,string name)
        {
            if (!path.StartsWith("\\"))
            {
                path = "\\" + path;
            }
            if (!path.StartsWith("\\" + User.Identity.Name))
            {
                return RedirectToPage("/Error");
            }
            if(btn=="Delete")
            {
                if (Id == null)
                {
                    return RedirectToPage("/Index", new { spath = path });
                }

                var RESULT = Api.DeleteFile((int)Id).Result;

                if (!RESULT)
                {
                    return NotFound();
                }

                return RedirectToPage("/Index", new { spath = path });
            }
            else if (btn == "Download")
            {
                if (Id == null)
                {
                    return RedirectToPage("/Index", new { spath = path });
                }
                var data = Api.GetFile((int)Id).Result;
                return File(data, "application/octet-stream", name);
            }
            if(string.IsNullOrWhiteSpace(DirName))
            {
                return RedirectToPage("/Index", new { spath = path });
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
