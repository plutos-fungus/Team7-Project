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
using Newtonsoft.Json;
using SurfsUp.Areas.Identity.Data;
using SurfsUp.Models;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http;

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
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync("https://localhost:7260/api/Rentals/");
            response.EnsureSuccessStatusCode();
            
            var jsonRespone = await response.Content.ReadAsStringAsync();
            
            var rental =
                System.Text.Json.JsonSerializer.Deserialize<List<Rental>>(jsonRespone);

            if (!this.User.IsInRole("Admin") && rental != null)
            {
                var usr = await _userManager.GetUserAsync(HttpContext.User);

                List<Rental> Rentals = new List<Rental>();
                foreach (Rental valueA in rental)
                {
                    if (valueA.Email == usr.Email)
                    {
                        Rentals.Add(valueA);
                    }
                }
                //var Rentals = from r in _context.Rental
                //              where r.Email == usr.Email
                //              select r;
                return View(Rentals);
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
        public IActionResult Create(int id)
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
        public async Task<IActionResult> Create([Bind("ID,RentalDate,StartDate,EndDate,Email,SurfboardID")] Rental rental)
        {
            

            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.PostAsJsonAsync("https://localhost:7260/api/Rentals/", rental);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }
                var rented = _context.Surfboard.Where(s => s.ID == rental.SurfboardID).ToList();
                // from Surfboard in _context.Surfboard
                // where Surfboard.ID == globalId
                // select Surfboard;
                foreach (Surfboard surfboard in rented)
                {
                    surfboard.IsRented = true;
                    _context.Update(surfboard);
                }
                _context.Add(rental);
                await _context.SaveChangesAsync();
                return Redirect("/Home/Index");
            }
            return Redirect("/Home/Index");
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
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.PutAsJsonAsync("https://localhost:7260/api/Rentals/"+id, rental);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

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
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Rental == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.DeleteAsync("https://localhost:7260/api/Rentals/" + id);
            if (!response.IsSuccessStatusCode)
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
