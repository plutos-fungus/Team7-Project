using Microsoft.AspNetCore.Mvc;
using SurfsUp.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using SurfsUp;
using System.Runtime.Loader;
using SurfsUp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace SurfsUp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index(string SortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            // Checks for if the DateTime for the rented board is done (plz don't remove)
            CheckAndDelete();


            ViewData["CurrentFilter"] = searchString;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(SortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = String.IsNullOrEmpty(SortOrder) ? "price_desc" : "";
            ViewData["BoardTypeSortParm"] = String.IsNullOrEmpty(SortOrder) ? "BoardType_desc" : "";
            ViewData["CurrentSort"] = SortOrder;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            // Finds all the surfboards that is not rented in the database
            var Surfboard = from s in _context.Surfboard
                            where s.IsRented == false
                            select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                Surfboard = Surfboard.Where(s => s.Name.Contains(searchString));
            }

            switch (SortOrder)
            {
                case "name_desc":
                    Surfboard = Surfboard.OrderBy(s => s.Name);
                    break;
                case "price_desc":
                    Surfboard = Surfboard.OrderByDescending(s => s.Price);
                    break;
                case "BoardType_desc":
                    Surfboard = Surfboard.OrderBy(s => s.BoardType);
                    break;
                default:
                    Surfboard = Surfboard.OrderBy(s => s.Price);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Surfboard>.CreateAsync(Surfboard.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public IActionResult CanNotRent()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Surfboard == null)
            {
                return NotFound();
            }

            var surfboard = await _context.Surfboard
                .FirstOrDefaultAsync(m => m.ID == id);
            if (surfboard == null)
            {
                return NotFound();
            }

            return View(surfboard);
        }

        public void CheckAndDelete()
        {
            DateTime nowDate = DateTime.Now;
            var rentCheck = _context.Rental.ToList();
            var allSurfboards = _context.Surfboard.Where(s => s.IsRented == true).ToList();

            foreach (Rental rental in rentCheck)
            {
                if (rental.EndDate <= nowDate)
                {
                    foreach (Surfboard surfboard in allSurfboards)
                    {
                        if (rental.SurfboardID == surfboard.ID)
                        {
                            surfboard.IsRented = false;
                            _context.Surfboard.Update(surfboard);
                            _context.Rental.Remove(rental);
                        }
                    }
                    // Update Rental.SurfboardId
                    _context.SaveChanges();
                }

            }
        }

    }
}