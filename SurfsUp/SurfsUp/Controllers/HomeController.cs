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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private HttpClient _client;
        private readonly string APILinkRentalsV1 = @"https://localhost:7260/api/v1/Rentals/";
        private readonly string APILinkRentalsV2 = @"https://localhost:7260/api/v2/Rentals/";

        private readonly string APILinkSurfboardsV1 = @"https://localhost:7260/api/v1/Surfboards/";
        private readonly string APILinkSurfboardsV2 = @"https://localhost:7260/api/v2/Surfboards/";
        
        public HomeController(ILogger<HomeController> logger, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _client = new HttpClient();
        }

        public async Task<List<Surfboard>> ReturnSurfboardList(string APILink)
        {
            using HttpResponseMessage response = await _client.GetAsync(APILink);
            response.EnsureSuccessStatusCode();
            var jsonRespone = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Deserialize<List<Surfboard>>(jsonRespone, options);
        }

        public async Task<Surfboard> ReturnSurfboard(int? id, string APILink)
        {
            using HttpResponseMessage response = await _client.GetAsync(APILink + id);

            response.EnsureSuccessStatusCode();

            var jsonRespone = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Deserialize<Surfboard>(jsonRespone, options);
        }

        #region Works With API
        public async Task<IActionResult> Index(string SortOrder, string currentFilter, string searchString, int? pageNumber)
        {
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

                var Surfboard = await ReturnSurfboardList(APILinkSurfboardsV1);

                var sort = from s in Surfboard
                           where s.IsRented == false
                           select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    sort = sort.Where(s => s.Name.Contains(searchString));
                }

                switch (SortOrder)
                {
                    case "name_desc":
                        sort = sort.OrderBy(s => s.Name);
                        break;
                    case "price_desc":
                        sort = sort.OrderByDescending(s => s.Price);
                        break;
                    case "BoardType_desc":
                        sort = sort.OrderBy(s => s.BoardType);
                        break;
                    default:
                        sort = sort.OrderBy(s => s.Price);
                        break;
                }
                int pageSize = 3;
                return View(await PaginatedList<Surfboard>.CreateAsync(sort, pageNumber ?? 1, pageSize));

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

                var Surfboard = await ReturnSurfboardList(APILinkSurfboardsV2);

                var sort = from s in Surfboard
                           where s.IsRented == false && s.BoardType == Models.Surfboard.BoardTypes.shortboard
                           select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    sort = sort.Where(s => s.Name.Contains(searchString));
                }

                switch (SortOrder)
                {
                    case "name_desc":
                        sort = sort.OrderBy(s => s.Name);
                        break;
                    case "price_desc":
                        sort = sort.OrderByDescending(s => s.Price);
                        break;
                    case "BoardType_desc":
                        sort = sort.OrderBy(s => s.BoardType);
                        break;
                    default:
                        sort = sort.OrderBy(s => s.Price);
                        break;
                }
                int pageSize = 3;
                return View(await PaginatedList<Surfboard>.CreateAsync(sort, pageNumber ?? 1, pageSize));
            }
        }
        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult NotLoggedIn()
        {
            return View();
        }

        #region Works With API
        public async Task<IActionResult> Details(int? id)
        {
            bool userIsAuthenticated = HttpContext.User.Identity.IsAuthenticated;

            if (!userIsAuthenticated)
            {
                try
                {
                    return View(await ReturnSurfboard(id, APILinkSurfboardsV2));
                }
                catch (Exception e)
                {
                    return RedirectToAction("NotLoggedIn");
                }
            }
            return View(await ReturnSurfboard(id, APILinkSurfboardsV1));
        }
        #endregion

        #region Works With API
        public async void CheckAndDelete()
        {
            DateTime nowDate = DateTime.Now;
            using HttpResponseMessage response = await _client.GetAsync(APILinkRentalsV1);
            response.EnsureSuccessStatusCode();

            var jsonRespone = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var rental = JsonSerializer.Deserialize<List<Rental>>(jsonRespone, options);

            var rentCheck = rental.ToList();

            using HttpResponseMessage surfResponse = await _client.GetAsync(APILinkSurfboardsV1);
            surfResponse.EnsureSuccessStatusCode();
            var surfJsonRespone = await response.Content.ReadAsStringAsync();
            options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var Surfboard = JsonSerializer.Deserialize<List<Surfboard>>(surfJsonRespone, options);

            var allSurfboards = Surfboard.Where(s => s.IsRented == true).ToList();

            foreach (Rental rent in rentCheck)
            {
                if (rent.EndDate <= nowDate)
                {
                    foreach (Surfboard surfboard in allSurfboards)
                    {
                        if (rent.SurfboardID == surfboard.ID)
                        {
                            surfboard.IsRented = false;
                            using HttpResponseMessage tempResponse = await _client.PutAsJsonAsync(APILinkRentalsV1 + rent.ID, rent);
                            using HttpResponseMessage temp2Response = await _client.PutAsJsonAsync(APILinkRentalsV1 + surfboard.ID, surfboard);

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