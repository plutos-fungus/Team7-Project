using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurfUpApi.Data;
using SurfUpApi.Models;

namespace SurfUpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly SurfUpApiContext _context;

        public RentalsController(SurfUpApiContext context)
        {
            _context = context;
        }

        // GET: api/Rentals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rental>>> GetRental()
        {
            return await _context.Rental.ToListAsync();
        }

        // GET: api/Rentals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rental>> GetRental(int id)
        {
            var rental = await _context.Rental.FindAsync(id);

            if (rental == null)
            {
                return NotFound();
            }

            return rental;
        }

        // PUT: api/Rentals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRental(int id, Rental rental)
        {
            if (id != rental.ID)
            {
                return BadRequest();
            }

            _context.Entry(rental).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentalExists(id))
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

        // POST: api/Rentals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Rental>> PostRental(Rental rental)
        {
            _context.Rental.Add(rental);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RentalExists(rental.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw new Exception("DBUpdateException, no id conflict");
                }
            }
            return CreatedAtAction("GetRental", new { id = rental.ID }, rental);
        }

        // DELETE: api/Rentals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRental(int id)
        {
            var rental = await _context.Rental.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            _context.Rental.Remove(rental);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RentalExists(int id)
        {
            return _context.Rental.Any(e => e.ID == id);
        }
    }
}
