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

namespace SpaceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public async Task<ResponseFiles> GetFileElement(string path)
        {
            
            var basedir = Environment.GetEnvironmentVariable("MainPath")+"\\"+ _userManager.GetUserName(User).Replace('.','-'); 
            if (!path.StartsWith('\\'))
            {
                basedir = basedir + "\\";
            }
            path = basedir + path.Trim();
            
            if(!System.IO.File.Exists(path))
            {
                if (!Regex.Match(path, @"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$").Success)
                {
                    return new ResponseFiles() { Errors = new List<string>() { "Path format not valid ES: folder\\file.ext" }, Status = false, Content = null };
                }
                return new ResponseFiles() { Errors = new List<string>() { "NotFound" }, Status = false, Content = null };
            }
            if(System.IO.Directory.Exists(path))
            {
                System.IO.Directory.GetFiles();
            }
            System.IO.File file = new System.IO.File("");
            return new ResponseFiles() { Errors = new List<string>(), Status = true, Content = null }; ;
        }

        // PUT: api/Main/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFileElement(int id, FileElement fileElement)
        {
            if (id != fileElement.Id)
            {
                return BadRequest();
            }

            _context.Entry(fileElement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileElementExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Main
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FileElement>> PostFileElement(FileElement fileElement)
        {
            _context.EleFiles.Add(fileElement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFileElement", new { id = fileElement.Id }, fileElement);
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
