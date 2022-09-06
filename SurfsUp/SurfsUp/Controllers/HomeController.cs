using Microsoft.AspNetCore.Mvc;
using SurfsUp.Models;
using SurfsUp.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace SurfsUp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SurfsUpContext _context;

        public HomeController(ILogger<HomeController> logger, SurfsUpContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Surfboard != null ?
                        View(await _context.Surfboard.Where(s => s.IsRented == false).ToListAsync()) :
                        Problem("Entity set 'SurfsUpContext.Surfboard'  is null.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}