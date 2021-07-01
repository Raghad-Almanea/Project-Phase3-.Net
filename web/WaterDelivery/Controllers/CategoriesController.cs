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
    [Authorize(Roles = "ادمن,تصنيفات")]
    public class CategoriesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories
        public ActionResult Index()
        {
            return View(db.Category.ToList());
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {

                //if (category.img != null)
                //{
                //    var file = Request.Files["img"];
                //    if (file != null)
                //    {
                //        Random random = new Random();
                //        int number = random.Next(1, 1300);
                //        string fileName = DateTime.Now.Ticks.ToString() + number + ".jpg";

                //        string path = Path.Combine(Server.MapPath("~/Content/Img/Category"), fileName);
                //        file.SaveAs(path);
                //        category.img = BaisUrlCategory + fileName;
                //    }
                //}

                category.is_active = true;
                db.Category.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Category.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var updatedCategory = db.Category.Find(category.Id);
                //if (category.img != null)
                //{
                //    var file = Request.Files["img"];
                //    if (file != null)
                //    {
                //        Random random = new Random();
                //        int number = random.Next(1, 1300);
                //        string fileName = DateTime.Now.Ticks.ToString() + number + ".jpg";

                //        string path = Path.Combine(Server.MapPath("~/Content/Img/Category"), fileName);
                //        file.SaveAs(path);
                //        category.img = BaisUrlCategory + fileName;
                //        updatedCategory.img = category.img;
                //    }
                //}
                //else
                //{
                //    updatedCategory.img = updatedCategory.img;
                //}

                updatedCategory.name = category.name;
                updatedCategory.name_en = category.name_en;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        public ActionResult ChangeState(int? id)
        {

            var category = db.Category.Find(id);
            if (category.is_active == true)
            {
                category.is_active = false;

            }
            else
            {
                category.is_active = true;
            }
            db.SaveChanges();
            return Json(new { key = 1, data = category.is_active }, JsonRequestBehavior.AllowGet);

        }


        //[HttpPost]
        //public ActionResult Delete(int? id)
        //{
        //    var category = db.Category.Find(id);

        //    db.Category.Remove(category);

        //    db.SaveChanges();
        //    return Json(new { key = 1 }, JsonRequestBehavior.AllowGet);

        //}

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
