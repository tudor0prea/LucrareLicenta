using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SafeWheel3.Data;
using SafeWheel3.Data.Migrations;
using SafeWheel3.Models;
using Plata = SafeWheel3.Models.Plata;

namespace SafeWheel3.Controllers
{
    public class PlataController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public PlataController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }


        public ActionResult New([FromForm] Plata p)
        {
            p.UserID = _userManager.GetUserId(User);
            

            //if (ModelState.IsValid)
            //{
                var currentUser = _userManager.GetUserId(User);
                var platitor = db.ApplicationUsers.Where(a => a.Id == currentUser).First();
                if (platitor.Tokens >= 10)
            { platitor.Tokens -= 10;
                //incasatorul este un user care are comentariul pus in plata
                //ApplicationUser incasator = db.ApplicationUsers.Where(a => a.Id == p.Comment.UserId).First();

                //ViewBag.CommentId.User.Tokens += 5;
                Comment comentariu = db.Comments.Where(a => a.Id == p.CommentID).First();

                ApplicationUser incasator = db.ApplicationUsers.Where(a => a.Id==comentariu.UserId).First();    

                incasator.Tokens += 5;
                
                }

            else
            { 
                   TempData["message"] = "Nu aveti suficienti tokens.";
                return RedirectToAction("Index", "Anunt");
                 }
                
                


                db.Plati.Add(p);
                db.SaveChanges();
                TempData["message"] = "Plata a fost efectuata";

            return RedirectToAction("Index", "Anunt");


        }
        
[HttpPost]
public JsonResult cumparaCredite(int nr)
{
    var currentUser = _userManager.GetUserId(User);
    var platitor = db.ApplicationUsers.Where(a => a.Id == currentUser).First();

    platitor.Tokens += nr;

    // Salvează modificările în baza de date
    db.SaveChanges();

    return Json(new { success = true });
}

        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Editor") || User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}
