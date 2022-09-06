using Microsoft.AspNetCore.Mvc;
using SurfsUp.Models;
using SurfsUp.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Loader;

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
            CheckAndDelete();
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
                    //Update Rental.SurfboardId
                    _context.SaveChanges();
                }

            }
        }
    }
}