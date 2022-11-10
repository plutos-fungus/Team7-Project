using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
//using AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfsUp.Areas.Identity.Data;
using SurfsUp.Models;

namespace SurfsUp.Controllers
{
    public class SurfboardsController : Controller
    {

        public SurfboardsController()
        {
        }

        #region Index works With API
        // GET: Surfboards
        [Authorize(Policy = "RequiredAdminRole")]
        public async Task<IActionResult> Index()
        {
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync("https://localhost:7260/api/Surfboards/");
            response.EnsureSuccessStatusCode();
            var jsonRespone = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var Surfboard = JsonSerializer.Deserialize<List<Surfboard>>(jsonRespone, options);

            return View(Surfboard);
        }
        #endregion

        #region Details works With API
        // GET: Surfboards/Details/5
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
        }
        #endregion

        #region Creat works with API
        [Authorize(Policy = "RequiredAdminRole")]
        // GET: Surfboards/Create
        public IActionResult Create()
        {
            Surfboard s = new();
            return View();
        }
        #endregion

        #region Create works with API
        /*
        * Doesn't work!!
        */
        // POST: Surfboards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,BoardType,Length,Width,Thickness,Volume,Price,EquipmentTypes,Image")] Surfboard surfboard)
        {
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.PostAsJsonAsync("https://localhost:7260/api/Surfboards/", surfboard);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            return Redirect("/Surfboards");
        }
        #endregion

        #region Edit works with API
        // GET: Surfboards/Edit/5
        [Authorize(Policy = "RequiredAdminRole")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync("https://localhost:7260/api/Surfboards/" + id);
            response.EnsureSuccessStatusCode();

            var jsonRespone = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var surfboard = JsonSerializer.Deserialize<Surfboard>(jsonRespone, options);
            if (surfboard == null)
            {
                return NotFound();
            }
            return View(surfboard);
        }
        #endregion

        #region Edit works with API
        /*
         * Not updated to API
         */
        // POST: Surfboards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequiredAdminRole")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,BoardType,Length,Width,Thickness,Volume,Price,EquipmentTypes,Image,")] Surfboard surfboard)
        {
            if (id != surfboard.ID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.PutAsJsonAsync("https://localhost:7260/api/Surfboards/" + id, surfboard);
                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToAction("NotWorking");
                }
                return View(surfboard);
            }
            return RedirectToAction("NotWorking");
        }
        #endregion

        public IActionResult NotWorking()
        {
            return View();
        }

        #region Delete works with API
        /*
         * Not updated to API
         */
        // GET: Surfboards/Delete/5
        [Authorize(Policy = "RequiredAdminRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync("https://localhost:7260/api/Surfboards/" + id);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var surfboard = JsonSerializer.Deserialize<Surfboard>(jsonResponse, options);

            using HttpResponseMessage SurfboardPutResponse = await client.PutAsJsonAsync("https://localhost:7260/api/Surfboards/" + surfboard.ID, surfboard);

            if (!SurfboardPutResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }


            return View(surfboard);
        }
        #endregion

        #region Delete Confirmed works with API
        /*
         * Not updated to API
         */
        // POST: Surfboards/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "RequiredAdminRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.DeleteAsync("https://localhost:7260/api/Surfboards/" + id);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Not Used?
        private bool SurfboardExists(int id)
        {
            var surfboard = GetSurfboardAsync();

            if (surfboard.Id == id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task<List<Surfboard>> GetSurfboardAsync()
        {
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync("https://localhost:7260/api/Rentals/");
            response.EnsureSuccessStatusCode();
            var jsonRespone = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var surfboard = JsonSerializer.Deserialize<List<Surfboard>>(jsonRespone, options);
            return surfboard;
        }
        #endregion
    }
}