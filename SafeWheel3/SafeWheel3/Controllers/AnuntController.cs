using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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



        //ADAUGARE LA FAVORITE

        [HttpPost]
        public IActionResult AddBookmark([FromForm] AnuntBookmark anuntBookmark)
        {
            // Daca modelul este valid
            if (ModelState.IsValid)
            {
                // Verificam daca avem deja articolul in colectie
                if (db.AnuntBookmarks
                    .Where(ab => ab.AnuntId == anuntBookmark.AnuntId)
                    .Where(ab => ab.BookmarkId == anuntBookmark.BookmarkId)
                    .Count() > 0)
                {
                    TempData["message"] = "Acest anunt este deja adaugat in colectie";
                    TempData["messageType"] = "alert-danger";
                }
                else
                {
                    // Adaugam asocierea intre articol si bookmark 
                    db.AnuntBookmarks.Add(anuntBookmark);
                    // Salvam modificarile
                    db.SaveChanges();

                    // Adaugam un mesaj de success
                    TempData["message"] = "Anuntul a fost adaugat in colectia selectata";
                    TempData["messageType"] = "alert-success";
                }

            }
            else
            {
                TempData["message"] = "Nu s-a putut adauga anuntul in colectie";
                TempData["messageType"] = "alert-danger";
            }

            // Ne intoarcem la pagina articolului
            return Redirect("/Anunt/Details/" + anuntBookmark.AnuntId);
        }

        // SFARSIT ADAUGARE LA FAVORITE



        // GET: Anunt
        public async Task<IActionResult> Index()
        {
            var anunturile = db.Anunturi.Include("Dealer").Include("User").OrderBy(a => a.DataFabricatiei);

            var search = "";

            ViewBag.UserCurent = _userManager.GetUserId(User);


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

            //Filtrare pentru Km
            Int32 maxKm = 1000000;
            Int32 minKm = 0;
            List<int> anunturiIdsMaxKm = new List<int>();
            List<int> anunturiIdsMinKm = new List<int>();

            // ---------- minim Km
            if (Convert.ToString(HttpContext.Request.Query["minKm"]) == "")
                minKm = 0;
            else if (Convert.ToString(HttpContext.Request.Query["minKm"]) != null)
                minKm = Convert.ToInt32(HttpContext.Request.Query["minKm"]);

            ViewBag.minKm = minKm;

            anunturiIdsMinKm = db.Anunturi.Where(
               an => an.Km >= minKm
               ).Select(a => a.Id).ToList();

            // ---------- maxim Km
            if (Convert.ToString(HttpContext.Request.Query["maxKm"]) == "")
                maxKm = 1000000;
            else if (Convert.ToString(HttpContext.Request.Query["maxKm"]) != null)
                maxKm = Convert.ToInt32(HttpContext.Request.Query["maxKm"]);

            ViewBag.maxKm = maxKm;

            anunturiIdsMaxKm = db.Anunturi.Where(
                an => an.Km <= maxKm
                ).Select(a => a.Id).ToList();

            // ---- rezultat combinat
            filteredAnunturi = filteredAnunturi
                .Where(ann => anunturiIdsMinKm.Contains(ann.Id) && anunturiIdsMaxKm.Contains(ann.Id))
                .Include("Dealer")
                .OrderBy(a => a.DataFabricatiei);




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

        /*[HttpPost]
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
                var existingAnunt = db.Anunturi.Where(a => a.Id == id).First();
                anunt.DataFabricatiei = existingAnunt.DataFabricatiei;
                anunt.Id= existingAnunt.Id; 

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
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Anunt anunt, IFormFile AnuntImage)
        {
            if (id != anunt.Id)
            {
                return NotFound();
            }

            var existingAnunt = await db.Anunturi.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (existingAnunt == null)
            {
                return NotFound();
            }

            // Păstrăm valoarea originală a DataFabricatiei
            anunt.DataFabricatiei = existingAnunt.DataFabricatiei;
/*
            if (ModelState.IsValid)
            {*/
                if (AnuntImage != null && AnuntImage.Length > 0)
                {
                    // Generăm calea de stocare a fișierului
                    var storagePath = Path.Combine(
                        _env.WebRootPath, // Luăm calea folderului wwwroot
                        "images", // Adăugăm calea folderului images
                        AnuntImage.FileName // Numele fișierului
                    );

                    // Uploadăm fișierul la calea de storage
                    using (var fileStream = new FileStream(storagePath, FileMode.Create))
                    {
                        await AnuntImage.CopyToAsync(fileStream);
                    }

                    // Generăm calea de afișare a fișierului care va fi stocată în baza de date
                    var databaseFileName = "/images/" + AnuntImage.FileName;

                    // Salvăm storagePath-ul în baza de date
                    anunt.Image = databaseFileName;
                }
                else
                {
                    // Păstrăm imaginea existentă dacă nu a fost încărcată una nouă
                    anunt.Image = existingAnunt.Image;
                }

                // Attach the entity and update only the modified properties
                db.Anunturi.Attach(anunt);
            db.Entry(anunt).Property(x => x.DataFabricatiei).IsModified = true;
                db.Entry(anunt).Property(x => x.Marca).IsModified = true;
                db.Entry(anunt).Property(x => x.Pret).IsModified = true;
                db.Entry(anunt).Property(x => x.DealerId).IsModified = true;
                db.Entry(anunt).Property(x => x.Description).IsModified = true;
                db.Entry(anunt).Property(x => x.NrTel).IsModified = true;
                db.Entry(anunt).Property(x => x.Km).IsModified = true;
                if (AnuntImage != null && AnuntImage.Length > 0)
                {
                    db.Entry(anunt).Property(x => x.Image).IsModified = true;
                }

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await db.Anunturi.AnyAsync(e => e.Id == anunt.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            //}

            // Dacă ModelState nu este valid, revenim la view-ul de Editare cu datele introduse
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
            Anunt anunt = db.Anunturi.Include("Comments").Where(anunt=>anunt.Id == id).First();
            if (anunt.UserID == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Anunturi.Remove(anunt);
                db.SaveChanges();
                TempData["message"] = "Anuntul a fost sters";
                ViewBag.message = "Anuntul a fost sters";
                return RedirectToAction("Index");
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un anunt care nu va apartine";
                ViewBag.message = "Nu aveti dreptul sa stergeti un anunt care nu va apartine";
                return RedirectToAction("Index");
            }
            //return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anunt = await db.Anunturi.Include(a => a.Comments)
                .Include(a => a.Dealer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (anunt == null)
            {
                return NotFound();
            }

            // Adaugam bookmark-urile utilizatorului pentru dropdown
            ViewBag.UserBookmarks = db.Bookmarks
                                      .Where(b => b.UserId == _userManager.GetUserId(User))
                                      .ToList();

            //Pentru a vedea ce comentarii are:

            /*            string userCurent = _userManager.GetUserId(User);

                        var UserPlati = db.Plati.Include("Comments").Where(p => p.UserID == userCurent);
                        var UserNonPlati= db.Plati.Include("Comments").Where(p => p.UserID != userCurent);

                        foreach(var item in UserPlati)
                        {
                            ViewBag.ComentariiGata.Add(item.Comment);
                        }

                        foreach (var item in UserNonPlati)
                        {
                            ViewBag.ComentariiNonGata.Add(item.Comment);
                        }*/

            string userCurent = _userManager.GetUserId(User);
            
            List<Comment> ComentariiGata = db.Comments
                                            .Where(c =>  ( db.Plati.Any(p => p.CommentID == c.Id && p.UserID == userCurent) ) &&
                                                           c.AnuntId == id )
                                            .ToList();

            List<Comment> ComentariiNonGata = db.Comments
                                            .Where(c => db.Plati.All(p => p.CommentID != c.Id || p.UserID != userCurent)
                                            && c.AnuntId == id)
                                            .ToList();


            ViewBag.ComentariiGata = ComentariiGata;
            ViewBag.ComentariiNonGata = ComentariiNonGata;
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

            if (User.IsInRole("Admin") )
            {
                ViewBag.AfisareButoane = true;
            }


        }



    }
}
