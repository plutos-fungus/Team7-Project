﻿using System;
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
        // Returns the admin page if your email has the admin role.
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
            //return _context.Surfboard != null ?
            //    View(await _context.Surfboard.ToListAsync()) :
            //    Problem("Entity set 'SurfsUpContext.Surfboard'  is null.");
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

        #region Creat works with API
        [Authorize(Policy = "RequiredAdminRole")]
        // GET: Surfboards/Create
        [Authorize(Policy = "RequiredAdminRole")]
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
        // Returns the actual create surboard site if your email has admin role.
        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,BoardType,Length,Width,Thickness,Volume,Price,EquipmentTypes,Image")] Surfboard surfboard)
        {
            //ModelState.Remove("");
            
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
        // Returns the edit surboard site if your email has admin role.
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
            //if (id == null || _context.Surfboard == null)
            //{
            //    return NotFound();
            //}

            //var surfboard = await _context.Surfboard.AsNoTracking().FirstOrDefaultAsync(s => s.ID == id);
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
        // Returns the actual edit surboard site if your email has admin role.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequiredAdminRole")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,BoardType,Length,Width,Thickness,Volume,Price,EquipmentTypes,Image,")] Surfboard surfboard)
        {
            if (id != surfboard.ID)
            {
                return NotFound();
            }

            return await CheckIfInUse(id, surfboard);
        }

        // GET: Surfboards/Delete/5
        [Authorize(Policy = "RequiredAdminRole")]
        public async Task<IActionResult> Delete(int? id)
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
            //return (surfboard.Any(e => e.ID = id))

            //return (_context.Surfboard?.Any(e => e.ID == id)).GetValueOrDefault();
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
