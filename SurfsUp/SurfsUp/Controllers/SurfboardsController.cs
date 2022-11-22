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
        private HttpClient client;
        private readonly string APILinkSurfboard = @"https://localhost:7260/api/v1/Surfboards/";

        public SurfboardsController()
        {
            client = new HttpClient();
        }

        public async Task<object> ReturnSurfboardOrSurfboardList(int? id)
        {
            string link = APILinkSurfboard;

            if (id != null)
            {
                link += id;
            }

            using HttpResponseMessage response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var jsonRespone = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            if (id == null)
            {
                return JsonSerializer.Deserialize<List<Surfboard>>(jsonRespone, options);
            }

            return JsonSerializer.Deserialize<Surfboard>(jsonRespone, options);
        }

        public async Task<Surfboard> ReturnSurfboardObject(int? id)
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
        // GET: Surfboards
        [Authorize(Policy = "RequiredAdminRole")]
        public async Task<IActionResult> Index()
        {
            return View(await ReturnSurfboardOrSurfboardList(null));
        }
        #endregion

        #region Details works With API
        // GET: Surfboards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return View(await ReturnSurfboardOrSurfboardList(id));
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
            using HttpResponseMessage response = await client.PostAsJsonAsync(APILinkSurfboard, surfboard);
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

            var surfboard = await ReturnSurfboardOrSurfboardList(id);

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

            using HttpResponseMessage response = await client.PutAsJsonAsync(APILinkSurfboard + id, surfboard);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("NotWorking");
            }

            return RedirectToAction("Index");
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

            var surfboard = await ReturnSurfboardObject(id);

            using HttpResponseMessage SurfboardPutResponse = await client.PutAsJsonAsync(APILinkSurfboard + surfboard.ID, surfboard);

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
            using HttpResponseMessage response = await client.DeleteAsync(APILinkSurfboard + id);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}