using Microsoft.AspNetCore.Authorization;
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

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                List<int> anunturiIds = db.Anunturi.Where(
                    an => an.Marca.Contains(search) || an.Description.Contains(search)
                    ).Select(a => a.Id).ToList();

                anunturile = db.Anunturi.Where(ann => anunturiIds.Contains(ann.Id))
                    //.Include("Id").Include("Marca").Include("DataFabricatiei").Include("Pret")
                    .Include("Dealer")
                    .OrderBy(a => a.DataFabricatiei);
            }
            var filteredAnunturi = anunturile;


            //Filtrare pentru pret
            Int32 maxPrice = 1000000;
            Int32 minPrice = 0;
            List<int> anunturiIdsMAX = new List<int>();
            List<int> anunturiIdsMIN = new List<int>();

            // ---------- minim
            if (Convert.ToString(HttpContext.Request.Query["minPrice"]) == "")
                minPrice = 0;
            else
                if (Convert.ToString(HttpContext.Request.Query["minPrice"]) != null)
                minPrice = Convert.ToInt32(HttpContext.Request.Query["minPrice"]);

            ViewBag.minPrice = minPrice; 

            anunturiIdsMIN = db.Anunturi.Where(
               an => an.Pret >= minPrice
               ).Select(a => a.Id).ToList();

            // ----------maxim

            if (Convert.ToString(HttpContext.Request.Query["maxPrice"]) == "")
                maxPrice = 1000000;
            else
                 if (Convert.ToString(HttpContext.Request.Query["maxPrice"]) != null)
                maxPrice = Convert.ToInt32(HttpContext.Request.Query["maxPrice"]);

            ViewBag.maxPrice = maxPrice;

            anunturiIdsMAX = db.Anunturi.Where(
                an => an.Pret <= maxPrice
                ).Select(a => a.Id).ToList();


            // ---- rezultat
            filteredAnunturi = filteredAnunturi.Where(ann => anunturiIdsMIN.Contains(ann.Id) && anunturiIdsMAX.Contains(ann.Id))
                .Include("Dealer").OrderBy(a => a.DataFabricatiei);




            //Filtrare pentru Data
            DateOnly minData = new DateOnly(1950, 1, 1);
            DateOnly maxData = new DateOnly(2025, 1, 1);
            List<int> anunturiIdsMAXD = new List<int>();
            List<int> anunturiIdsMIND = new List<int>();
            // -- minima
            if (Convert.ToString(HttpContext.Request.Query["minData"]) == "")
                minData = new DateOnly(1950, 1, 1);
            else
                if (Convert.ToString(HttpContext.Request.Query["minData"]) != null)
            {
                DateTime dateTime = Convert.ToDateTime(HttpContext.Request.Query["minData"]);
                minData = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
            }

            ViewBag.minData = minData;   
            anunturiIdsMIND = db.Anunturi.Where(
               an => an.DataFabricatiei >= minData
               ).Select(a => a.Id).ToList();

            // --- maxima
            if (Convert.ToString(HttpContext.Request.Query["maxData"]) == "")
                maxData = new DateOnly(2025, 1, 1);
            else
               if (Convert.ToString(HttpContext.Request.Query["maxData"]) != null)
            {
                DateTime dateTime = Convert.ToDateTime(HttpContext.Request.Query["maxData"]);
                maxData = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
            }

            ViewBag.maxData = maxData; 
            anunturiIdsMAXD = db.Anunturi.Where(
             an => an.DataFabricatiei <= maxData
             ).Select(a => a.Id).ToList();

            // ------- rezultat
            filteredAnunturi = filteredAnunturi.Where(ann => anunturiIdsMIND.Contains(ann.Id) && anunturiIdsMAXD.Contains(ann.Id))
              .Include("Dealer").OrderBy(a => a.DataFabricatiei);



            // Filtrare dupa marca

            ViewBag.Dealers = GetAllDealers();

            int chosenDealer = -1;
            ViewBag.ChosenDealer = -1;




            List<int> anunturiIdsDealer = new List<int>();

            if (Convert.ToString(HttpContext.Request.Query["dealer"]) != "-1")
            {
                chosenDealer = Convert.ToInt16(HttpContext.Request.Query["dealer"]);
                ViewBag.ChosenDealer = chosenDealer;
                anunturiIdsDealer = db.Anunturi.Include("Dealer").Where(an => an.DealerId == chosenDealer).Select(a => a.Id).ToList();
                filteredAnunturi = filteredAnunturi.Where(ann => anunturiIdsDealer.Contains(ann.Id)).Include("Dealer")
                 .OrderBy(a => a.DataFabricatiei);

            }
            if (Convert.ToString(HttpContext.Request.Query["dealer"]) == null)
                filteredAnunturi = db.Anunturi.Include("Dealer").OrderBy(a => a.DataFabricatiei);


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
            return View(filteredAnunturi);
        }





        // GET: Anunt/Create
        public IActionResult Create()
        {
            // Populate Dealers dropdown list
            Anunt anunt=new Anunt();
            
            //salvam id ul userului
            anunt.UserID = _userManager.GetUserId(User);
            
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


        /*
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

                //salvam id ul userului
                anunt.UserID = _userManager.GetUserId(User);

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
        */


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Anunt anunt, IFormFile AnuntImagee, List<IFormFile> AnuntImages)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (AnuntImagee.Length > 0)
                {
                    // Generam calea de stocare a fisierului
                    var storagePath = Path.Combine(
                        _env.WebRootPath, // Luam calea folderului wwwroot
                        "images", // Adaugam calea folderului images
                        AnuntImagee.FileName  //numele fisierului
                        );

                    // Uploadam fisierul la calea de storage
                    using (var fileStream = new FileStream(storagePath, FileMode.Create))
                    {
                        await AnuntImagee.CopyToAsync(fileStream);
                    }

                    // General calea de afisare a fisierului care va fi stocata in baza de date
                    var databaseFileName = "/images/" + AnuntImagee.FileName;
                    anunt.Image = databaseFileName;
                }
                   
                
                
                if (AnuntImages != null && AnuntImages.Count > 0)
                {
                    // Creează o listă pentru a stoca numele fișierelor imaginilor
                    List<string> imagePaths = new List<string>();

                    // Iterează prin fiecare imagine și încarc-o
                    foreach (var AnuntImage in AnuntImages)
                    {
                        if (AnuntImage.Length > 0)
                        {
                            // Generează calea de stocare a fișierului
                            var storagePath = Path.Combine(
                                _env.WebRootPath, // Ia calea către folderul wwwroot
                                "images", // Adaugă calea către folderul images
                                AnuntImage.FileName // Numele fișierului
                            );

                            // Încarcă fișierul la calea de stocare
                            using (var fileStream = new FileStream(storagePath, FileMode.Create))
                            {
                                await AnuntImage.CopyToAsync(fileStream);
                            }

                            // Adaugă calea fișierului la lista de căi
                            var databaseFileName = "/images/" + AnuntImage.FileName;
                            imagePaths.Add(databaseFileName);
                        }
                    }

                    // Salvăm căile imaginilor în obiectul Anunt
                    anunt.ImagePaths = imagePaths;
                }

                // Salvăm ID-ul utilizatorului
                anunt.UserID = _userManager.GetUserId(User);

                // Adăugăm anunțul în contextul bazei de date
                db.Anunturi.Add(anunt);
                await db.SaveChangesAsync();

                // Adăugați un mesaj în TempData
                TempData["SuccessMessage"] = "Anunțul a fost adăugat cu succes!";

                return RedirectToAction(nameof(Index));
            }

            // Dacă ModelState nu este valid, revenim la vizualizarea Create cu datele introduse
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

            var anunt = await db.Anunturi.Include(a=> a.Comments)
                .Include(a => a.Dealer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (anunt == null)
            {
                return NotFound();
            }
            SetAccessRights();
            return View(anunt);
        }
        




        // Adaugarea unui comentariu asociat unui articol in baza de date
        [HttpPost]
        [Authorize(Roles = "Admin,Specialist")]
        public IActionResult Details([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);
            comment.UserName = _userManager.GetUserName(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Anunt/Details/" + comment.AnuntId);
            }

            else
            {
                Anunt art = db.Anunturi.Include("Dealer")
                                         .Include("User")
                                         .Include("Comments")
                                         .Where(art => art.Id == comment.AnuntId)
                                         .First();

                //return Redirect("/Anunt/Details/" + comment.AnuntId);

                SetAccessRights();
              
                return View(art);
            }
        }



        private void SetAccessRights()
        {
            ViewBag.EsteAdmin = User.IsInRole("Specialist");

            ViewBag.UserCurent = _userManager.GetUserId(User);

            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Specialist") || User.IsInRole("Admin") )
            {
                ViewBag.AfisareButoane = true;
            }


        }



    }
}
