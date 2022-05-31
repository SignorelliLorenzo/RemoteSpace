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
using MainSite.Data;
using MainSite.Connect.Cryptography;

namespace MainSite.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        public string path;
        public string OldPath;
        public string Error;
        public IndexModel(ILogger<IndexModel> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            Filelist = new List<FileDisplay>();
        }
        [BindProperty]
        public List<FileDisplay> Filelist { get; set; }
        public async Task<IActionResult> OnGet(string spath,string error)
        {
            Error = error;
            if (string.IsNullOrWhiteSpace(spath))
            {
                spath = "\\" + User.Identity.Name;
            }
            if (spath!= "\\" + User.Identity.Name)
            {
                OldPath = spath.Substring(0,spath.Length-("\\"+spath.Split("\\").Last()).Length);
            }
            path = spath;
            
            if (!path.StartsWith("\\"))
            {
                path = "\\"+path;
            }
            if (!path.StartsWith("\\"+User.Identity.Name))
            {
                return RedirectToPage("/Error",new { Error="Path not valid" });
            }
            
            try
            {
                var resultFilelist = Api.GetDirFiles(path).Result;
                Filelist = resultFilelist.Select(f => new FileDisplay(f)).ToList();
            }
            catch(Exception ex) 
            {
                if(ex.InnerException.Message=="NotFound" && path== "\\" + User.Identity.Name)
                {
                    if (!Api.AddDir(User.Identity.Name, "", User.Identity.Name).Result)
                    {
                        return RedirectToPage("/Error", new { Error = "Failed to create root dir" });
                    }
                }
                else
                {
                    return RedirectToPage("/Error", new { Error = ex.InnerException.Message });
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPost(string DirName,string path, string btn, int? Id,string name,string NewName)
        {
            if (!path.StartsWith("\\"))
            {
                path = "\\" + path;
            }
            if (!path.StartsWith("\\" + User.Identity.Name))
            {
                return RedirectToPage("/Error", new { Error = "Path not valid" });
            }
            if(NewName!=null)
            {
                if (!Api.Validate(Id, User.Identity.Name).Result)
                {
                    return RedirectToPage("/Index", new { spath = path, error = "Not valid" });
                }
                Api.Rename(NewName, Id);
                return RedirectToPage("/Index", new { spath = path });
            }
            if (btn=="Delete")
            {
                if (!Api.Validate(Id, User.Identity.Name).Result)
                {
                    return RedirectToPage("/Index", new { spath = path , error="Not valid"}) ;
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
                if (!Api.Validate(Id, User.Identity.Name).Result)
                {
                    return RedirectToPage("/Index", new { spath = path, error = "Not valid" });
                }
                var user = _userManager.GetUserId(User);
                var data = Api.GetFile((int)Id).Result;
               
                return File(FileData.Decrypt(data, user), "application/octet-stream", name);
            }
            if(string.IsNullOrWhiteSpace(DirName) || DirName.Contains("\\"))
            {
                return RedirectToPage("/Index", new { spath = path });
            }    
            try
            {
                if (!Api.CheckPath("\\" + User.Identity.Name).Result)
                {
                    if (!Api.AddDir(User.Identity.Name, "", User.Identity.Name).Result)
                    {
                        return RedirectToPage("/Error", new { Error = "Failed to create root dir" });
                    }
                }
                if (!Api.AddDir(DirName,path, User.Identity.Name).Result)
                {
                    return RedirectToPage("/Index", new { spath = path, error = "Failed to create dir" });
                }
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error", new { Error = ex.Message });
            }
            return RedirectToPage("/Index", new {spath=path});
        }
        
    }
}
