using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SafeWheel3.Data;
using SafeWheel3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeWheel3.Controllers
{
    public class AnuntController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext db;
        private IWebHostEnvironment _env; 

        public AnuntController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment env
        )
        {
            //_context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            db = context;
            _env = env;
        }

   

        public void UploadImage(Anunt anunt, IFormFile AnuntImage)
        {


            // Verificam daca exista imaginea in request (daca a fost incarcata o imagine)
            if (AnuntImage.Length > 0)
            {
                // Generam calea de stocare a fisierului
                var storagePath = Path.Combine(
                    _env.WebRootPath, // Luam calea folderului wwwroot
                    "images", // Adaugam calea folderului images
                    AnuntImage.FileName  //numele fisierului
                    );


                // General calea de afisare a fisierului care va fi stocata in baza de date
                var databaseFileName = "/images/" + AnuntImage.FileName;

             


                // Salvam storagePath-ul in baza de date
                anunt.Image = databaseFileName;
                db.Anunturi.Add(anunt);
                db.SaveChanges();
            }   
               // return View("Index");
        }

        public IEnumerable<SelectListItem> GetAllDealers()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();
            // extragem toate categoriile din baza de date
            var marci = from mar in db.Marci
                             select mar;
            // iteram prin categorii
            foreach (var marca in marci)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = marca.Id.ToString(),
                    Text = marca.Nume.ToString()
                });
            }
           
            return selectList;
        }

        // GET: Anunt
        public async Task<IActionResult> Index()
        {
            var anunturile = db.Anunturi.Include("Dealer").Include("User").OrderBy(a => a.DataFabricatiei);

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"])!=null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                List<int> anunturiIds=db.Anunturi.Where(
                    an => an.Marca.Contains(search) || an.Description.Contains(search) 
                    ).Select(a => a.Id).ToList();

                anunturile = db.Anunturi.Where(ann => anunturiIds.Contains(ann.Id))
                    //.Include("Id").Include("Marca").Include("DataFabricatiei").Include("Pret")
                    .Include("Dealer")
                    .OrderBy(a=>a.DataFabricatiei);
            }

            // Verificați dacă există un mesaj de succes în TempData
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            // var anunturi = await db.Anunturi.Include(a => a.Dealer).ToListAsync();
            // return View(anunturi);
           /* if (search != "")
            
                {
                    ViewBag.PaginationBaseUrl = "/Anunt/Index/?search=" + search + "&page";
                } 
                else
                {
                    ViewBag.PaginationBaseUrl = "/Anunt/Index/?page";
                }
            */
            return View(anunturile);
        }





        // GET: Anunt/Create
        public IActionResult Create()
        {
            // Populate Dealers dropdown list
            Anunt anunt=new Anunt();
            anunt.Dealers = GetAllDealers();
            ViewBag.Dealers = new SelectList(db.Marci, "Id", "Nume");
            return View();
        }
        /*
        // POST: Anunt/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Anunt anunt)
        {
            var query = db.Anunturi.Include("Dealer");

            if (ModelState.IsValid)
            {
                _context.Add(anunt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, return to the Create view with the entered data
            //MODIF ASTA CU CHAT
            //ViewBag.Dealers = new SelectList(_context.Marci, "Id", "Nume", anunt.DealerId);

            ViewBag.Dealers = GetAllDealers();
            return View(anunt);
        }
        */

        //NOU

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Anunt anunt, IFormFile AnuntImage)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (AnuntImage.Length > 0)
                {
                    // Generam calea de stocare a fisierului
                    var storagePath = Path.Combine(
                        _env.WebRootPath, // Luam calea folderului wwwroot
                        "images", // Adaugam calea folderului images
                        AnuntImage.FileName  //numele fisierului
                        );

                    // Uploadam fisierul la calea de storage
                    using (var fileStream = new FileStream(storagePath, FileMode.Create))
                    {
                        await AnuntImage.CopyToAsync(fileStream);
                    }

                    // General calea de afisare a fisierului care va fi stocata in baza de date
                    var databaseFileName = "/images/" + AnuntImage.FileName;




                    // Salvam storagePath-ul in baza de date
                    anunt.Image = databaseFileName;
                    //db.Anunturi.Add(anunt);
                    //db.SaveChanges();
                }
            
                

                //UploadImage(anunt, AnuntImage);
                db.Anunturi.Add(anunt);
                await db.SaveChangesAsync();

                // Adăugați un mesaj în TempData
                TempData["SuccessMessage"] = "Anuntul a fost adăugat cu succes!";

                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, return to the Create view with the entered data
            ViewBag.Dealers = GetAllDealers();
            return View(anunt);
        }


        // GET: Anunt/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anunt = await db.Anunturi.FindAsync(id);

            if (anunt == null)
            {
                return NotFound();
            }

            // Populate Dealers dropdown list
            ViewBag.Dealers = new SelectList(db.Marci, "Id", "Nume", anunt.DealerId);
            return View(anunt);
        }

        // POST: Anunt/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Anunt anunt, IFormFile AnuntImage)
        {
            ModelState.Clear();
            if (id != anunt.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
               if (AnuntImage != null && AnuntImage.Length > 0)
               {
                    // Generam calea de stocare a fisierului
                    var storagePath = Path.Combine(
                        _env.WebRootPath, // Luam calea folderului wwwroot
                       "images", // Adaugam calea folderului images
                        AnuntImage.FileName  //numele fisierului
                        );

                    // Uploadam fisierul la calea de storage
                    using (var fileStream = new FileStream(storagePath, FileMode.Create))
                    {
                        await AnuntImage.CopyToAsync(fileStream);
                    }

                    // General calea de afisare a fisierului care va fi stocata in baza de date
                    var databaseFileName = "/images/" + AnuntImage.FileName;




                    // Salvam storagePath-ul in baza de date
                    anunt.Image = databaseFileName;
                    //db.Anunturi.Add(anunt);
                    //db.SaveChanges();
                }
                
           

                db.Update(anunt);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, return to the Edit view with the entered data
            ViewBag.Dealers = new SelectList(db.Marci, "Id", "Nume", anunt.DealerId);
            return View(anunt);
        }

        // GET: Anunt/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anunt = await db.Anunturi.Include(a => a.Dealer).FirstOrDefaultAsync(a => a.Id == id);

            if (anunt == null)
            {
                return NotFound();
            }

            return View(anunt);
        }

        // POST: Anunt/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anunt = await db.Anunturi.FindAsync(id);
            db.Anunturi.Remove(anunt);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anunt = await db.Anunturi
                .Include(a => a.Dealer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (anunt == null)
            {
                return NotFound();
            }

            return View(anunt);
        }

    }
}
