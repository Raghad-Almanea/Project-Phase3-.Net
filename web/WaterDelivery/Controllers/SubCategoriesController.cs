using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WaterDelivery.Controllers.Api;
using WaterDelivery.Models;
using WaterDelivery.Models.TableDb;

namespace WaterDelivery.Controllers
{
    public class SubCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SubCategories
        public ActionResult Index(int? mainCat)
        {
            List<SubCategory> subCategories;
            if (mainCat > 0)
            {
                subCategories = db.SubCategory.Where(sc => sc.fk_cat == mainCat).ToList();
                ViewBag.MainId = mainCat;
                return View(subCategories);
            }

            subCategories = db.SubCategory.ToList();
            ViewBag.MainId = mainCat;
            return View(subCategories);
        }

        public ActionResult GetMainCategories()
        {
            var categories = db.Category.Where(c => c.is_active == true).ToList().Select(c => new
            {
                id = c.Id,
                name = c.name
            });

            return Json(new { key = 1, categories = categories }, JsonRequestBehavior.AllowGet);
        }

        // GET: SubCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubCategory subCategory = db.SubCategory.Find(id);
            if (subCategory == null)
            {
                return HttpNotFound();
            }
            return View(subCategory);
        }

        // GET: SubCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SubCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SubCategory subCategory)
        {
            if (ModelState.IsValid)
            {
                if (subCategory.img != null)
                {
                    var file = Request.Files["img"];
                    if (file != null)
                    {
                        Random random = new Random();
                        int number = random.Next(1, 1300);
                        string fileName = DateTime.Now.Ticks.ToString() + number + ".jpg";

                        string path = Path.Combine(Server.MapPath("~/Content/Img/Category"), fileName);
                        file.SaveAs(path);
                        subCategory.img = BaseController.BaisUrlCategory + fileName;
                    }
                }

                db.SubCategory.Add(subCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(subCategory);
        }

        // GET: SubCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubCategory subCategory = db.SubCategory.Find(id);
            if (subCategory == null)
            {
                return HttpNotFound();
            }
            return View(subCategory);
        }

        // POST: SubCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SubCategory subCategory)
        {
            if (ModelState.IsValid)
            {
                var updatedsubCategory = db.SubCategory.Find(subCategory.Id);
                if (subCategory.img != null)
                {
                    var file = Request.Files["img"];
                    if (file != null)
                    {
                        Random random = new Random();
                        int number = random.Next(1, 1300);
                        string fileName = DateTime.Now.Ticks.ToString() + number + ".jpg";

                        string path = Path.Combine(Server.MapPath("~/Content/Img/Category"), fileName);
                        file.SaveAs(path);
                        updatedsubCategory.img = BaseController.BaisUrlCategory + fileName;
                    }
                }
                else
                {
                    updatedsubCategory.img = updatedsubCategory.img;
                }
                updatedsubCategory.fk_cat = subCategory.fk_cat;
                updatedsubCategory.name = subCategory.name;
                updatedsubCategory.name_en = subCategory.name_en;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subCategory);
        }

        // GET: SubCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubCategory subCategory = db.SubCategory.Find(id);
            if (subCategory == null)
            {
                return HttpNotFound();
            }
            return View(subCategory);
        }

        // POST: SubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SubCategory subCategory = db.SubCategory.Find(id);
            db.SubCategory.Remove(subCategory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ChangeState(int? id)
        {

            var subCategory = db.SubCategory.Find(id);
            if (subCategory.is_active == true)
            {
                subCategory.is_active = false;

            }
            else
            {
                subCategory.is_active = true;
            }
            db.SaveChanges();
            return Json(new { key = 1, data = subCategory.is_active }, JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
