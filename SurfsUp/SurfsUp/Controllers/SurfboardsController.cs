using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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
        private readonly ApplicationDbContext _context;

        public SurfboardsController(ApplicationDbContext context)
        {
            _context = context;
        }

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
            //return _context.Surfboard != null ?
            //    View(await _context.Surfboard.ToListAsync()) :
            //    Problem("Entity set 'SurfsUpContext.Surfboard'  is null.");
        }

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

        [Authorize(Policy = "RequiredAdminRole")]

        // GET: Surfboards/Create
        public IActionResult Create()
        {
            return View();
        }

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
            ModelState.Remove("RowVersion");

            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.PostAsJsonAsync("https://localhost:7260/api/Rentals/", surfboard);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }
                //_context.Add(surfboard);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            return View(surfboard);
        }

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

        /*
         * Not updated to API
         */
        // POST: Surfboards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequiredAdminRole")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,BoardType,Length,Width,Thickness,Volume,Price,EquipmentTypes,Image,RowVersion")] Surfboard surfboard)
        {
            if (id != surfboard.ID)
            {
                return NotFound();
            }

            var SurfboardToBeUpdated = await _context.Surfboard.FirstOrDefaultAsync(x => x.ID == id);

            //Hvis objektet ikke kan findes, er det slettet. Den følgende 'if' kodeblok håndterer dette scenarie
            if (SurfboardToBeUpdated == null)
            {
                Surfboard deletedSurfboard = new Surfboard();
                //Forsøger at udfylde deletedBoardPost objektet, med det objekt controlleren holder på fra viewet.
                //Det vil sige, det boardpost objektet vi modtog fra viewet i denne metode.
                await TryUpdateModelAsync(deletedSurfboard);
                //AddModelError, tilføjer fejlbeskeden brugeren skal se.
                ModelState.AddModelError("", "Board ændringer kan ikke gemmes. En anden bruger har slettet boarded");
                //Sender objektet tilbage til viewet
                return View(deletedSurfboard);
            }

            //Sætter original værdien for RowVersion,
            //dvs. den oprindelige vi fik fra da vi kaldte Edit GET metoden.
            _context.Entry(SurfboardToBeUpdated).Property("RowVersion").OriginalValue = surfboard.RowVersion;

            if (await TryUpdateModelAsync<Surfboard>(SurfboardToBeUpdated, "",
                b => b.Name,
                b => b.BoardType,
                b => b.Length,
                b => b.Width,
                b => b.Thickness,
                b => b.Volume,
                b => b.Price,
                b => b.EquipmentTypes,
                b => b.Image,
                b => b.IsRented
                ))
            {
                try
                {
                    //SaveChangesAsync skaber en ConcurrenceException såfremt UPDATE SQL command returnerer 0 ændrede rækker.
                    //Det gør den hvis RowVersion kollonen på objektet i databasen
                    //er anderledes fra det boardpost objekt vi arbejder med her.
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    //Finder den entity der var involveret i exception
                    var exceptionEntry = ex.Entries.Single();
                    //Trækker det enkelte objekt ud og hardcaster til et BoardPost objekt
                    var clientValues = (Surfboard)exceptionEntry.Entity;
                    //Forespørger databasen for at finde frem til de nye værdier der ligger i databasen
                    var databaseEntry = exceptionEntry.GetDatabaseValues();

                    //Hvis boardet er slettet i mellemtiden:
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("", "Board ændringer kan ikke gemmes. En anden bruger har slettet boarded");
                    }
                    else
                    {
                        //Caster objektet til et Boardpost objekt
                        var databaseValue = (Surfboard)databaseEntry.ToObject();

                        #region Fejlmeddelelser for hver textbox i view
                        if (clientValues.Name != databaseValue.Name)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.Name}");
                        }
                        if (clientValues.Length != databaseValue.Length)
                        {
                            ModelState.AddModelError("Length", $"Nuværende værdi: {databaseValue.Length}");
                        }
                        if (clientValues.Width != databaseValue.Width)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.Width}");
                        }
                        if (clientValues.Volume != databaseValue.Volume)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.Volume}");
                        }
                        if (clientValues.Thickness != databaseValue.Thickness)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.Thickness}");
                        }
                        if (clientValues.BoardType != databaseValue.BoardType)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.BoardType}");
                        }
                        if (clientValues.Image != databaseValue.Image)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.Image}");
                        }
                        if (clientValues.Price != databaseValue.Price)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.Price}");
                        }
                        if (clientValues.EquipmentTypes != databaseValue.EquipmentTypes)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.EquipmentTypes}");
                        }
                        #endregion

                        ModelState.AddModelError("", "Kunne ikke gemme ændringerne." +
                            " En anden bruger har i mellemtiden lavet ændringer i dette board." +
                            " Ædringerne er vist i textboksene. Click på Save igen, for at gemme dine ændringer");
                        //Sætter RowVersion propertien for objektet til at være den nyere fra databasen
                        SurfboardToBeUpdated.RowVersion = (byte[])databaseValue.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }

            return View(SurfboardToBeUpdated);
        }

        /*
         * Not updated to API
         */
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

            return View(surfboard);
        }

        /*
         * Not updated to API
         */
        // POST: Surfboards/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "RequiredAdminRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Surfboard == null)
            {
                return Problem("Entity set 'SurfsUpContext.Surfboard'  is null.");
            }
            var surfboard = await _context.Surfboard.FindAsync(id);
            if (surfboard != null)
            {
                _context.Surfboard.Remove(surfboard);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurfboardExists(int id)
        {
            return (_context.Surfboard?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
