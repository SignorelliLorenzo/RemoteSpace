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
    [Route("api")]
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
        [HttpGet("PathEx/{path}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<bool> PathExists(string path)
        {
            path = path.Replace("_5", "\\");
            var basedir = Environment.GetEnvironmentVariable("MainPath") + "\\" + _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName ;
            var user = _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;
            if (!path.StartsWith('\\'))
            {
                path = "\\" + path;
            }
            var completepath = basedir + path.Trim();

            if ((System.IO.File.Exists(completepath) || System.IO.Directory.Exists(completepath)) &&  _context.EleFiles.Where(x=>x.Path+x.Name==path && x.User== user).Any())
            {
                return true;

            }

            return false;
        }
        [HttpGet("Dir/{path}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ResponseFiles> GetDir(string path)
        {
            path = path.Replace("_5", "\\");
            var basedir = Environment.GetEnvironmentVariable("MainPath")+"\\"+ _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName; 
            if (!path.StartsWith('\\'))
            {
                path = "\\"+path ;
            }
            var completepath = basedir + path.Trim();
            var user = _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;

            if (System.IO.File.Exists(completepath))
            {
                return new ResponseFiles() { Errors = new List<string> { "IsAFile" }, Status = true, Content = _context.EleFiles.Where(x => x.Path == path.Substring(0, path.IndexOf(Path.GetDirectoryName(completepath)))&& x.User == user).ToList() };

            }
            else if (System.IO.Directory.Exists(completepath))
            {
                
                var x= new ResponseFiles() { Errors = null, Status = true, Content = (_context.EleFiles.Where(x => x.Path == path && x.User == user).ToList())};
                return x;
            }
            return new ResponseFiles() { Errors = new List<string>() { "NotFound" }, Status = false, Content = null };
        }
        [HttpGet("File/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ResponseFile> GetFile(int id)
        {
            var user = _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;
            var fileElement = _context.EleFiles.Where(x => x.Id == id && x.User ==user).FirstOrDefault();
            if (fileElement == null)
            {
                return new ResponseFile() { Errors = { "NotFound" }, Status = false, Content=null };
            }
            var basedir = Environment.GetEnvironmentVariable("MainPath") + "\\" + _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;
            string CompletePath = basedir + fileElement.Path + "\\" + fileElement.Name;
            if(!System.IO.File.Exists(CompletePath))
            {
                _context.EleFiles.Remove(fileElement);
                return new ResponseFile() { Errors = { "FileDoesNotExist" }, Status = false, Content = null };

            }
            return new ResponseFile() { Errors = null, Status = true, Content = System.IO.File.ReadAllBytes(CompletePath) };
        }
        // POST: api/Main
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [DisableRequestSizeLimit]
        public async Task<ResponseFiles> AddFileElement(FileElementAddRequest fileElement)
        {
            var user = _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;
            FileElement CompleteFile;
            if (fileElement.FileInfo.IsDirectory)
            {
                CompleteFile = new FileElement(fileElement.FileInfo, 0,user );
            }
            else
            {
                CompleteFile = new FileElement(fileElement.FileInfo, fileElement.Content.Count(), user);
            }
            
            
           

            var basedir = Environment.GetEnvironmentVariable("MainPath") + "\\" + _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;
            if (!System.IO.Directory.Exists(basedir))
            {
                Directory.CreateDirectory(basedir);
            }
            var completepath = basedir + CompleteFile.Path +"\\"+ CompleteFile.Name;
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
            else if(Directory.Exists(completepath))
            {

                
                return new ResponseFiles() { Errors = new List<string>() { "DirAlreadyExists" }, Status = false, Content = null };
            }
            if (fileElement.FileInfo.IsDirectory)
            {
                try
                {
                    Directory.CreateDirectory(completepath);
                }
                catch (IOException ex)
                {


                    return new ResponseFiles() { Errors = new List<string>() { "NomeGiàEsistente:" + ex.Message }, Status = false, Content = null };
                }
                catch (Exception ex)
                {

                    return new ResponseFiles() { Errors = new List<string> { ex.Message }, Status = false, Content = null };
                }

            }
            else
            {
                var newfile = System.IO.File.Create(completepath);
                newfile.Write(Convert.FromBase64String(fileElement.Content));
                newfile.Close();
            }
            try
            {
                _context.EleFiles.Add(CompleteFile);
                _context.SaveChanges();
               
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(completepath);
                return new ResponseFiles() { Errors = new List<string>() { "FileInfoNotValid:" + ex.Message }, Status = false, Content = null };
            }
            
            return new ResponseFiles() { Errors = null, Status = true, Content = new List<FileElement> { CompleteFile } };

        }
        private bool RemoveDir (string rootpath,int id,bool save)
        {
            bool failed = false;
            var dir=_context.EleFiles.Find(id);
            var partialpath = rootpath + dir.Path+"\\" + dir.Name;
            if(!Directory.Exists(partialpath))
            {
                return false;
            }
            var elements = _context.EleFiles.Where(x => x.Path == dir.Path + "\\" + dir.Name);
            foreach (var element in elements)
            {
                if (element.IsDirectory)
                {
                    var result = RemoveDir(rootpath, element.Id, false);
                    if (!result && save)
                    {
                        failed = true;
                        break;
                    }
                    else if(!result)
                    {
                        return false;
                    }
                }
                else
                {
                    if(System.IO.File.Exists(partialpath + "\\" + element.Name))
                    {
                        _context.EleFiles.Remove(element);

                    }
                    else
                    {
                        return false;
                    }
                    
                }

            }

            _context.EleFiles.Remove(dir);
            if (save && !failed)
            {
                _context.SaveChanges();
                DirectoryInfo todelete=new DirectoryInfo(partialpath);
                todelete.Delete(true);
            }
            else if(save)
            {

                _context.ChangeTracker.Clear();
                return false;
            }
            return true;
        }
        // DELETE: api/Main/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ResponseModel> DeleteFileElement(int id)
        {
            var user = _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;
            var fileElement =  _context.EleFiles.Where(x=>x.Id==id && x.User== user).FirstOrDefault();
            if (fileElement == null)
            {
                return new ResponseModel() { Errors = new List<string>() { "NotFound" }, Status = false }; 
            }
            var basedir = Environment.GetEnvironmentVariable("MainPath") + "\\" + _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;
            if(fileElement.IsDirectory)
            {
               if( RemoveDir(basedir, fileElement.Id, true))
                {
                    return new ResponseModel() { Errors = new List<string>() { "failed to delete dir" }, Status = true };
                }
            }
            else 
            {
                string CompletePath = basedir + "\\" + fileElement.Path + "\\" + fileElement.Name;
                _context.EleFiles.Remove(fileElement);
                await _context.SaveChangesAsync();
                System.IO.File.Delete(CompletePath);
               
            }
            return new ResponseModel() { Errors = new List<string>() { }, Status = true };
        }
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ResponseFiles> GetFileElement(int id)
        {
            var user = _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;
            var fileElements = _context.EleFiles.Where(x => x.Id == id && x.User == user).ToList();
            if (fileElements.Count() == 0)
            {
                return new ResponseFiles() { Errors = new List<string>() { "NotFound" },Content=null, Status = false };
            }
            
            return new ResponseFiles() { Errors = new List<string>() { }, Content = fileElements, Status = true };
        }
        [HttpPut("{id}-{NewName}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ResponseModel> Rename(int id,string NewName)
        {
            return new ResponseModel() { Errors = new List<string>() { }, Status = true };
        }
        public void ChangePathRecursive(FileElement file, string NewPath)
        {
            if(file.IsDirectory)
            {
                var items=_context.EleFiles.Where(x => x.Path == file.Path + "\\" + file.Name).ToList();
                foreach(var item in items)
                {
                    ChangePathRecursive(item, NewPath + "\\" + item.Name);
                }
            }
        }
    }
}
