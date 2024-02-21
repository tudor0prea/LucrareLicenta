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
    
        public class MarcaController : Controller
        {
            private readonly ApplicationDbContext db;

            public MarcaController(ApplicationDbContext context)
            {
                db = context;
            }
            public ActionResult Index()
            {
                var marci = from marca in db.Marci
                                 orderby marca.Nume
                                 select marca;
                ViewBag.Marci = marci;
                return View();
            }

            public ActionResult Show(int id)
            {
                Marca category = db.Marci.Find(id);
                ViewBag.Marci = category;
                return View();
            }

            public ActionResult Create()
            {
                return View();
            }

            
            [HttpPost]
            public ActionResult Create(Marca mar)
            {
                try
                {
                    db.Marci.Add(mar);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    return View();
                }
            }

            public ActionResult Edit(int id)
            {
                Marca mar = db.Marci.Find(id);
                ViewBag.Marci = mar;
                return View();
            }

            [HttpPost]
            public ActionResult Edit(int id, Marca requestMarca)
            {
                try
                {
                    Marca mar = db.Marci.Find(id);

                    {
                        mar.Nume = requestMarca.Nume;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.Marci = requestMarca;
                    return View();
                }
            }

            [HttpPost]
            public ActionResult Delete(int id)
            {
                Marca mar = db.Marci.Find(id);
                db.Marci.Remove(mar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
 }

