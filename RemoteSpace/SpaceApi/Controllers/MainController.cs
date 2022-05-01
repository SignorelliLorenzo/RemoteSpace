using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceApi;
using SpaceApi.Data;
using SpaceApi.Models;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Api_Pcto.Models;
using SpaceApi.Models.Communication.Request;

namespace SpaceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class MainController : ControllerBase
    {
        private readonly AppFileDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        public MainController(AppFileDbContext context, UserManager<UserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

       
     

       
        [HttpGet("{path}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ResponseFiles> GetFileElement(string path)
        {
            
            var basedir = Environment.GetEnvironmentVariable("MainPath")+"\\"+ _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName; 
            if (!path.StartsWith('\\'))
            {
                path = "\\"+path ;
            }
            var completepath = basedir + path.Trim();
            
            if(!System.IO.File.Exists(completepath))
            {
                
                return new ResponseFiles() { Errors = new List<string>() { "NotFound" }, Status = false, Content = null };
            }
            List<string> elefiles;
            if(System.IO.Directory.Exists(completepath))
            {

                return new ResponseFiles() { Errors = null, Status = true, Content = _context.EleFiles.Where(x => x.Path == path).ToList() };
            }
            return new ResponseFiles() { Errors = new List<string>{"IsAFile" }, Status = true, Content = _context.EleFiles.Where(x => x.Path == path.Substring(0,path.IndexOf(Path.GetDirectoryName(completepath)))).ToList() }; 
           
  
        }

        

        // POST: api/Main
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ResponseFiles> AddFileElement(FileElementAddRequest fileElement)
        {
            var CompleteFile = new FileElement(fileElement.FileInfo, fileElement.Content.Count());
            _context.EleFiles.Add(CompleteFile);
            try
            {

                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                return new ResponseFiles() { Errors = new List<string>() { "FileInfoNotValid:" + ex.Message }, Status = false, Content = null };
            }
            var basedir = Environment.GetEnvironmentVariable("MainPath") + "\\" + _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;
            if (!System.IO.Directory.Exists(basedir))
            {
                Directory.CreateDirectory(basedir);
            }
            var completepath = basedir + "\\" + fileElement.FileInfo.Path + "\\" + fileElement.FileInfo.Name;
            try
            {
                Path.GetFullPath(completepath);
            }
            catch
            {
                return new ResponseFiles() { Errors = new List<string>() { "PathNotValid" }, Status = false, Content = null };
            }
            if (System.IO.File.Exists(completepath))
            {
                return new ResponseFiles() { Errors = new List<string>() { "FileAlreadyExists" }, Status = false, Content = null };
            }
                       
            if (fileElement.FileInfo.IsDirectory)
            {
                try
                {
                    Directory.CreateDirectory(completepath);
                }
                catch (IOException ex)
                {
                    return new ResponseFiles() { Errors = new List<string>() { "NomeGiàEsistente:"+ ex.Message}, Status = false, Content = null };
                }
                catch (Exception ex)
                {
                    return new ResponseFiles() { Errors = new List<string> { ex.Message }, Status = false, Content = null };
                }
                
            }
            else
            {
                
                    var newfile = System.IO.File.Create(completepath);
                
                
                newfile.Write(fileElement.Content);
            }

            return new ResponseFiles() { Errors = null, Status = true, Content = new List<FileElement> { CompleteFile } };

        }

        // DELETE: api/Main/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileElement(int id)
        {
            var fileElement = await _context.EleFiles.FindAsync(id);
            if (fileElement == null)
            {
                return NotFound();
            }

            _context.EleFiles.Remove(fileElement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FileElementExists(int id)
        {
            return _context.EleFiles.Any(e => e.Id == id);
        }
    }
}
