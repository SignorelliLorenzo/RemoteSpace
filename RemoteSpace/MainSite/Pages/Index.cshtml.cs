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
        public IndexModel(ILogger<IndexModel> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            Filelist = new List<FileElement>();
        }
        [BindProperty]
        public List<FileElement> Filelist { get; set; }
        public void OnGet()
        {
            
            path = "\\"+User.Identity.Name;
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
        }

    }
}
