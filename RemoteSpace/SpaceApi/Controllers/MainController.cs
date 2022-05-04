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

        [HttpGet("dir/{path}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ResponseFiles> GetDir(string path)
        {
            
            var basedir = Environment.GetEnvironmentVariable("MainPath")+"\\"+ _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName; 
            if (!path.StartsWith('\\'))
            {
                path = "\\"+path ;
            }
            var completepath = basedir + path.Trim();
            
            if(System.IO.File.Exists(completepath))
            {
                return new ResponseFiles() { Errors = new List<string> { "IsAFile" }, Status = true, Content = _context.EleFiles.Where(x => x.Path == path.Substring(0, path.IndexOf(Path.GetDirectoryName(completepath)))).ToList() };

            }
            else if (System.IO.Directory.Exists(completepath))
            {

                return new ResponseFiles() { Errors = null, Status = true, Content = _context.EleFiles.Where(x => x.Path == path).ToList() };
            }
            return new ResponseFiles() { Errors = new List<string>() { "NotFound" }, Status = false, Content = null };
            
           
  
        }
        [HttpGet("file/{path}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ResponseFile> GetFile(int id)
        {

            var fileElement = await _context.EleFiles.FindAsync(id);
            if (fileElement == null)
            {
                return new ResponseFile() { Errors = { "NotFound" }, Status = false, Content=null };
            }
            var basedir = Environment.GetEnvironmentVariable("MainPath") + "\\" + _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;
            string CompletePath = basedir + "\\" + fileElement.Path + "\\" + fileElement.Name;
            if(!System.IO.File.Exists(CompletePath))
            {
                _context.EleFiles.Remove(fileElement);
                return new ResponseFile() { Errors = { "FileDoesNotExist" }, Status = false, Content = null };

            }
            return new ResponseFile() { Errors = null, Status = false, Content = System.IO.File.ReadAllBytes(CompletePath) };
        }
        // POST: api/Main
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ResponseFiles> AddFileElement(FileElementAddRequest fileElement)
        {
            var CompleteFile = new FileElement(fileElement.FileInfo, fileElement.Content.Count());
            
           

            var basedir = Environment.GetEnvironmentVariable("MainPath") + "\\" + _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;
            if (!System.IO.Directory.Exists(basedir))
            {
                Directory.CreateDirectory(basedir);
            }
            var completepath = basedir + "\\" + fileElement.FileInfo.Path +  fileElement.FileInfo.Name;
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

                   
                    return new ResponseFiles() { Errors = new List<string>() { "NomeGiàEsistente:"+ ex.Message}, Status = false, Content = null };
                }
                catch (Exception ex)
                {
                   
                    return new ResponseFiles() { Errors = new List<string> { ex.Message }, Status = false, Content = null };
                }
                
            }
             
                var newfile = System.IO.File.Create(completepath);
                newfile.Write(fileElement.Content);
                newfile.Close();
           
            try
            {
                _context.SaveChanges();
                _context.EleFiles.Add(CompleteFile);
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(completepath);
                return new ResponseFiles() { Errors = new List<string>() { "FileInfoNotValid:" + ex.Message }, Status = false, Content = null };
            }
            
            return new ResponseFiles() { Errors = null, Status = true, Content = new List<FileElement> { CompleteFile } };

        }

        // DELETE: api/Main/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ResponseModel> DeleteFileElement(int id)
        {
            var fileElement = await _context.EleFiles.FindAsync(id);
            if (fileElement == null)
            {
                return new ResponseModel() { Errors = { "NotFound" }, Status = false }; 
            }
            var basedir = Environment.GetEnvironmentVariable("MainPath") + "\\" + _userManager.Users.Where(x => x.Email == _userManager.GetUserId(User)).First().UserName;
            string CompletePath = basedir + "\\" + fileElement.Path + "\\" + fileElement.Name;
            _context.EleFiles.Remove(fileElement);
            await _context.SaveChangesAsync();
            System.IO.File.Delete(CompletePath);
            return new ResponseModel() { Errors = { }, Status=true };
        }

    }
}
