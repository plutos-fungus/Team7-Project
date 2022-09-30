using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfsUp.Areas.Identity.Data;
using SurfsUp.Models;

namespace SurfsUp.Controllers
{
    public class RentalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private int globalId;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public RentalsController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Rentals
        
        public async Task<IActionResult> Index()
        {
            // Checks if the email is an admin or not
            // The admin page can only be accessed by the admin email.
            if (!this.User.IsInRole("Admin"))
            {
                var usr = await _userManager.GetUserAsync(HttpContext.User);
                var Rentals = from r in _context.Rental
                              where r.Email == usr.Email
                              select r;
                return View(await Rentals.ToListAsync());
            }

            return _context.Rental != null ?
                        View(await _context.Rental.ToListAsync()) :
                        Problem("Entity set 'SurfsUpContext.Rental'  is null.");
        }

        // GET: Rentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Rental == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental
                .FirstOrDefaultAsync(m => m.ID == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // GET: Rentals/Create
        public IActionResult Create(int id, byte[] rowVersion)
        {
            Rental r = new();
            r.SurfboardID = id;
            globalId = id;
            return View(r);
        }

        // POST: Rentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("ID,RentalDate,StartDate,EndDate,Email,SurfboardID")] Rental rental)
        {
            bool TempBool = true;
            try
            {
                var rented = _context.Surfboard.Where(s => s.ID == rental.SurfboardID).ToList();
                foreach (Surfboard surfboard in rented)
                {
                    surfboard.IsRented = true;
                    _context.Update(surfboard);
                }
                _context.Add(rental);
                await _context.SaveChangesAsync();
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                TempBool = false;
                return Redirect($"/Home/CanNotRent/{id}");
            }
        }

        // GET: Rentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rental == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }
            return View(rental);
        }

        // POST: Rentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RentalDate,StartDate,EndDate,Email,SurfboardID")] Rental rental)
        {
            if (id != rental.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rental);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(rental);
        }

        // GET: Rentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || _context.Rental == null)
            {
                return NotFound();
            }

            var rental = await _context.Rental
                .FirstOrDefaultAsync(m => m.ID == id);
            if (rental == null)
            {
                return NotFound();
            }
            var rented = _context.Surfboard.Where(s => s.ID == rental.SurfboardID).ToList();
            // from Surfboard in _context.Surfboard
            // where Surfboard.ID == globalId
            // select Surfboard;
            foreach (Surfboard surfboard in rented)
            {
                surfboard.IsRented = false;
                _context.Update(surfboard);
            }
            await _context.SaveChangesAsync();
            _context.Add(rental);
            if (id == null || _context.Rental == null)
            {
                return NotFound();
            }
            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Rental == null)
            {
                return Problem("Entity set 'SurfsUpContext.Rental'  is null.");
            }
            var rental = await _context.Rental.FindAsync(id);
            if (rental != null)
            {
                _context.Rental.Remove(rental);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return (_context.Rental?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
