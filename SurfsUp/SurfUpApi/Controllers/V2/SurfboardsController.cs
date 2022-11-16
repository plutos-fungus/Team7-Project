using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurfUpApi.Data;
using SurfUpApi.Models;

namespace SurfUpApi.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class SurfboardsController : ControllerBase
    {
        private readonly SurfUpApiContext _context;

        public SurfboardsController(SurfUpApiContext context)
        {
            _context = context;
        }

        // GET: api/Surfboards
        [MapToApiVersion("2.0")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Surfboard>>> GetSurfboard()
        {
            return await _context.Surfboard.Where(x => x.BoardType == Surfboard.BoardTypes.shortboard).ToListAsync();
        }

        // GET: api/Surfboards/5
        [MapToApiVersion("2.0")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Surfboard>> GetSurfboard(int id)
        {
            var surfboard = await _context.Surfboard.FindAsync(id);

            if (surfboard == null)
            {
                return NotFound();
            }

            return surfboard;
        }

        // PUT: api/Surfboards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [MapToApiVersion("2.0")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSurfboard(int id, Surfboard surfboard)
        {
            if (id != surfboard.ID)
            {
                return BadRequest();
            }

            _context.Entry(surfboard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurfboardExists(id))
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

        // POST: api/Surfboards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [MapToApiVersion("2.0")]
        [HttpPost]
        public async Task<ActionResult<Surfboard>> PostSurfboard(Surfboard surfboard)
        {
            _context.Surfboard.Add(surfboard);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSurfboard", new { id = surfboard.ID }, surfboard);
        }

        // DELETE: api/Surfboards/5
        [MapToApiVersion("2.0")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurfboard(int id)
        {
            var surfboard = await _context.Surfboard.FindAsync(id);
            if (surfboard == null)
            {
                return NotFound();
            }

            _context.Surfboard.Remove(surfboard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SurfboardExists(int id)
        {
            return _context.Surfboard.Any(e => e.ID == id);
        }
    }
}
