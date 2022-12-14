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
using JsonSerializer = System.Text.Json.JsonSerializer;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.Extensions.Options;

namespace SurfsUp.Controllers
{
    public class RentalsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private HttpClient client;
        private readonly string APILinkRental = @"https://localhost:7260/api/v1/Rentals/";
        private readonly string APILinkSurfboard = @"https://localhost:7260/api/v1/Surfboards/";


        public RentalsController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            //_context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            client = new HttpClient();
        }

        public async Task<object> ReturnRentalOrRentalList(int? id)
        {
            string link = APILinkRental;

            if (id != null)
            {
                link += id;
            }

            using HttpResponseMessage response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            if (id == null)
            {
                return JsonSerializer.Deserialize<List<Rental>>(jsonResponse, options);
            }
            return JsonSerializer.Deserialize<Rental>(jsonResponse, options);
        }

        public async Task<List<Rental>> UserSpecificRental(List<Rental> rental)
        {
            List<Rental> Rentals = new List<Rental>();
            if (!this.User.IsInRole("Admin") && rental != null)
            {
                var usr = await _userManager.GetUserAsync(HttpContext.User);
                foreach (Rental valueA in rental)
                {
                    if (valueA.Email == usr.Email)
                    {
                        Rentals.Add(valueA);
                    }
                }
            }
            return Rentals;
        }

        public async Task<Surfboard> ReturnSurfboardObject(int id)
        {
            using HttpResponseMessage SurfboardResponse = await client.GetAsync(APILinkSurfboard + id);
            SurfboardResponse.EnsureSuccessStatusCode();
            var jsonResponse = await SurfboardResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var Surfboard = JsonSerializer.Deserialize<Surfboard>(jsonResponse, options);
            return Surfboard;
        }

        #region Index works With API
        // GET: Rentals
        public async Task<IActionResult> Index()
        {
            var rental = await ReturnRentalOrRentalList(null) as List<Rental>;

            if (this.User.IsInRole("Admin"))
            {
                return rental != null ?
                    View(rental.ToList()) :
                    Problem("Entity set 'SurfsUpContext.Rental'  is null.");
            }
            else
            {
                return View(await UserSpecificRental(rental));
            }
        }
        #endregion

        #region Works With API
        // GET: Rentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return View(await ReturnRentalOrRentalList(id));
        }
        #endregion

        #region Works With API Rentals/Create/ID
        // GET: Rentals/Create
        public IActionResult Create(int id)
        {
            //create is clicked on index moving the user to the create page
            // instantiate a new rental with the given SurfboardID
            Rental r = new();
            r.SurfboardID = id;
            //returns the new rental to the view layer
            return View(r);
        }
        #endregion

        #region Works with API Rentals/Create/rental
        // POST: Rentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,EndDate,Email,SurfboardID")] Rental rental)
        {
            int RentalCount = 0;
            using HttpResponseMessage RentalResponse = await client.GetAsync(APILinkRental);
            RentalResponse.EnsureSuccessStatusCode();
            var jsonResponse = await RentalResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var rentals = JsonSerializer.Deserialize<List<Rental>>(jsonResponse, options);
            
            foreach (Rental r in rentals)
            {
                if (r.Email == rental.Email)
                {
                    RentalCount++;
                }
            }
            if (RentalCount < 3)
            {
                using HttpResponseMessage response = await client.PostAsJsonAsync(APILinkRental, rental);
                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToAction("CanNotRent");
                }
                var Surfboard = await ReturnSurfboardObject(rental.SurfboardID);
                Surfboard.IsRented = true;
                using HttpResponseMessage SurfboardPutResponse = await client.PutAsJsonAsync(APILinkSurfboard + Surfboard.ID, Surfboard);
                if (!SurfboardPutResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("CanNotRent");
                }
                return Redirect("/Home/Index");
            }
            return RedirectToAction("CanNotRent");


        }
        #endregion


        #region Works With API
        // GET: Rentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await ReturnRentalOrRentalList(id);
            if (rental == null)
            {
                return NotFound();
            }
            return View(rental);
        }
        #endregion

        #region Works With API
        // POST: Rentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,EndDate,Email,SurfboardID")] Rental rental)
        {
            if (id != rental.ID)
            {
                return RedirectToAction("CanNotDelete");
            }

            using HttpResponseMessage response = await client.PutAsJsonAsync(APILinkRental + id, rental);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Works With API Rentals/Delete
        // GET: Rentals/Delete/5
        //[HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return View(await ReturnRentalOrRentalList(id));
        }
        #endregion Works With API

        #region Works With API Rentals/Delete
        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // this code block requests to delete a rental
            using HttpResponseMessage response = await client.DeleteAsync(APILinkRental + id);
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("CanNotDelete");
            }
            var Surfboard = await ReturnSurfboardObject(id);

            Surfboard.IsRented = false;
            // sends the updated surfboard to the Api, so it can be rented once again
            using HttpResponseMessage SurfboardPutResponse = await client.PutAsJsonAsync(APILinkSurfboard + Surfboard.ID, Surfboard);

            if (!SurfboardPutResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion

        public IActionResult CanNotRent()
        {
            return View();
        }

        public IActionResult CanNotDelete()
        {
            return View();
        }

        public IActionResult CanNotEdit()
        {
            return View();
        }
    }
}