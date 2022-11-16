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

        private readonly string APILinkRentalsV1 = @"https://localhost:7260/api/v1/Rentals";
        private readonly string APILinkRentalsV2 = @"https://localhost:7260/api/v2/Rentals";

        private readonly string APILinkSurfboardsV1 = @"https://localhost:7260/api/v1/Surfboards/";
        private readonly string APILinkSurfboardsV2 = @"https://localhost:7260/api/v2/Surfboards/";

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;

        }

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

                using HttpResponseMessage response = await client.GetAsync(APILinkSurfboardsV1);
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

                using HttpResponseMessage response = await client.GetAsync(APILinkSurfboardsV2);
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

        public IActionResult Privacy()
        {
            return View();
        }

        #region Works With API
        public async Task<IActionResult> Details(int? id)
        {
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(APILinkSurfboardsV1 + id);
            response.EnsureSuccessStatusCode();

            var jsonRespone = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var surfboard = JsonSerializer.Deserialize<Surfboard>(jsonRespone, options);

            return View(surfboard);
        }
        #endregion

        #region Works With API
        public async void CheckAndDelete()
        {
            DateTime nowDate = DateTime.Now;
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(APILinkRentalsV1);
            response.EnsureSuccessStatusCode();

            var jsonRespone = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var rental = JsonSerializer.Deserialize<List<Rental>>(jsonRespone, options);

            var rentCheck = rental.ToList();

            client = new HttpClient();
            using HttpResponseMessage surfResponse = await client.GetAsync(APILinkSurfboardsV1);
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
                            using HttpResponseMessage tempResponse = await client.PutAsJsonAsync(APILinkRentalsV1 + rent.ID, rent);
                            using HttpResponseMessage temp2Response = await client.PutAsJsonAsync(APILinkRentalsV1 + surfboard.ID, surfboard);

                            if (!tempResponse.IsSuccessStatusCode && !temp2Response.IsSuccessStatusCode)
                            {
                                //return NotFound();
                            }
                        }
                    }
                }

            }
        }
        #endregion
    }
}