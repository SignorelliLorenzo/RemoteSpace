using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceApi;
using SpaceApi.Data;

namespace SpaceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly AppFileDbContext _context;

        public MainController(AppFileDbContext context)
        {
            _context = context;
        }

        // GET: api/Main
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileElement>>> GetEleFiles()
        {
            return await _context.EleFiles.ToListAsync();
        }

        //// GET: api/Main/5
        //[HttpGet("{path}")]
        //public async Task<ActionResult<FileElement>> GetFileElement(string path)
        //{
        //    if(!Regex.Match(path, @"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$").Success)
        //    {

        //    }

        //    return fileElement;
        //}

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
