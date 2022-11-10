using Microsoft.AspNetCore.Mvc;
using SurfsUp.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using SurfsUp;
using System.Runtime.Loader;
using SurfsUp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

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

        #region Some Commend
        /*public async Task<IActionResult> Index()
        {
            // Checks for if the DateTime for the rented board is done (plz don't remove)
            CheckAndDelete();
            return _context.Surfboard != null ?
                        View(await _context.Surfboard.Where(s => s.IsRented == false).ToListAsync()) :
                        Problem("Entity set 'SurfsUpContext.Surfboard'  is null.");
        }*/
        #endregion

        #region Works With API
        public async Task<IActionResult> Index(string SortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            HttpClient client = new HttpClient();

            bool userIsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            if (userIsAuthenticated)
            {
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

                using HttpResponseMessage response = await client.GetAsync("https://localhost:7260/api/Surfboards/");
                response.EnsureSuccessStatusCode();
                var jsonRespone = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var Surfboard = JsonSerializer.Deserialize<List<Surfboard>>(jsonRespone, options);

                var Hej = from s in Surfboard
                          where s.IsRented == false
                          select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    Hej = Hej.Where(s => s.Name.Contains(searchString));
                }

                switch (SortOrder)
                {
                    case "name_desc":
                        Hej = Hej.OrderBy(s => s.Name);
                        break;
                    case "price_desc":
                        Hej = Hej.OrderByDescending(s => s.Price);
                        break;
                    case "BoardType_desc":
                        Hej = Hej.OrderBy(s => s.BoardType);
                        break;
                    default:
                        Hej = Hej.OrderBy(s => s.Price);
                        break;
                }
                int pageSize = 3;
                return View(await PaginatedList<Surfboard>.CreateAsync(Hej, pageNumber ?? 1, pageSize));

            }
            else
            {
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

                using HttpResponseMessage response = await client.GetAsync("https://localhost:7260/api/Surfboards/");
                response.EnsureSuccessStatusCode();
                var jsonRespone = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var Surfboard = JsonSerializer.Deserialize<List<Surfboard>>(jsonRespone, options);

                var Hej = from s in Surfboard
                          where s.IsRented == false && s.BoardType == Models.Surfboard.BoardTypes.shortboard
                          select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    Hej = Hej.Where(s => s.Name.Contains(searchString));
                }

                switch (SortOrder)
                {
                    case "name_desc":
                        Hej = Hej.OrderBy(s => s.Name);
                        break;
                    case "price_desc":
                        Hej = Hej.OrderByDescending(s => s.Price);
                        break;
                    case "BoardType_desc":
                        Hej = Hej.OrderBy(s => s.BoardType);
                        break;
                    default:
                        Hej = Hej.OrderBy(s => s.Price);
                        break;
                }
                int pageSize = 3;
                return View(await PaginatedList<Surfboard>.CreateAsync(Hej, pageNumber ?? 1, pageSize));
            }
        }
        #endregion

        #region Some Comment
        /*public async Task<IActionResult> Index(Surfboard.BoardTypes b)
        {
            return _context.Surfboard != null ?
                        View(await _context.Surfboard.Where(s => s.IsRented == false && s.BoardType == b).ToListAsync()) :
                        Problem("Entity set 'SurfsUpContext.Surfboard'  is null.");
        }*/
        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        #region Works With API
        public async Task<IActionResult> Details(int? id)
        {
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync("https://localhost:7260/api/Surfboards/" + id);
            response.EnsureSuccessStatusCode();

            var jsonRespone = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var surfboard = JsonSerializer.Deserialize<Surfboard>(jsonRespone, options);

            return View(surfboard);
            //if (id == null || _context.Surfboard == null)
            //{
            //    return NotFound();
            //}

            //var surfboard = await _context.Surfboard
            //    .FirstOrDefaultAsync(m => m.ID == id);
            //if (surfboard == null)
            //{
            //    return NotFound();
            //}

            //return View(surfboard);
        }
        #endregion

        #region Some Comment
        /*
         * Works????
         */
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
        #endregion

        #region Works With API
        public async void CheckAndDelete()
        {
            DateTime nowDate = DateTime.Now;
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync("https://localhost:7260/api/Rentals/");
            response.EnsureSuccessStatusCode();

            var jsonRespone = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var rental = JsonSerializer.Deserialize<List<Rental>>(jsonRespone, options);

            var rentCheck = rental.ToList();

            client = new HttpClient();
            using HttpResponseMessage surfResponse = await client.GetAsync("https://localhost:7260/api/Surfboards/");
            surfResponse.EnsureSuccessStatusCode();
            var surfJsonRespone = await response.Content.ReadAsStringAsync();
            options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var Surfboard = JsonSerializer.Deserialize<List<Surfboard>>(surfJsonRespone, options);

            var allSurfboards = Surfboard.Where(s => s.IsRented == true).ToList();

            //DateTime nowDate = DateTime.Now;
            //var rentCheck = _context.Rental.ToList();
            //var allSurfboards = _context.Surfboard.Where(s => s.IsRented == true).ToList();

            foreach (Rental rent in rentCheck)
            {
                if (rent.EndDate <= nowDate)
                {
                    foreach (Surfboard surfboard in allSurfboards)
                    {
                        if (rent.SurfboardID == surfboard.ID)
                        {
                            surfboard.IsRented = false;
                            client = new HttpClient();
                            using HttpResponseMessage tempResponse = await client.PutAsJsonAsync("https://localhost:7260/api/Rentals/" + rent.ID, rent);
                            using HttpResponseMessage temp2Response = await client.PutAsJsonAsync("https://localhost:7260/api/Rentals/" + surfboard.ID, surfboard);

                            if (!tempResponse.IsSuccessStatusCode && !temp2Response.IsSuccessStatusCode)
                            {
                                //return NotFound();
                            }
                            //surfboard.IsRented = false;
                            //_context.Surfboard.Update(surfboard);
                            //_context.Rental.Remove(rent);
                        }
                    }
                    //Update Rental.SurfboardId
                    //_context.SaveChanges();
                }

            }
        }
        #endregion
    }
}