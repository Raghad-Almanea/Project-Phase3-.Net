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
    [Authorize(Roles = "ادمن,اعلانات")]
    public class SlidersController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Adverts
        public ActionResult Index()
        {
            return View(db.Slider.ToList());
        }



        // GET: Adverts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Adverts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Slider slider)
        {
            if (ModelState.IsValid)
            {
                if (slider.type==2)
                {
                    int count = db.Slider.Where(c => c.type == 2).ToList().Count();
                    if (count >= 1)
                    {
                        ViewBag.Msg = "It is not possible to add more than one ad space";
                        return View();
                    }
                }
              
                if (slider.FileName != null)
                {
                    var file = Request.Files["FileName"];
                    if (file != null)
                    {
                        Random random = new Random();
                        int number = random.Next(1, 1300);
                        string fileName = DateTime.Now.Ticks.ToString() + number + ".jpg";

                        string path = Path.Combine(Server.MapPath("~/Content/Img/Adverts"), fileName);
                        file.SaveAs(path);
                        slider.FileName = BaisUrlHoste + "Content/Img/Adverts/" + fileName;
                    }
                }

                slider.IsActive = true;

                db.Slider.Add(slider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(slider);
        }

        // GET: Adverts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slider advert = db.Slider.Find(id);
            if (advert == null)
            {
                return HttpNotFound();
            }
            return View(advert);
        }

        // POST: Adverts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Slider slider)
        {
            if (ModelState.IsValid)
            {

                if (slider.type == 2)
                {
                    int count = db.Slider.Where(c => c.type == 2 && c.Id != slider.Id).ToList().Count();
                    if (count >= 1)
                    {
                        ViewBag.Msg = "It is not possible to add more than one ad space";
                        Slider advert = db.Slider.Find(slider.Id);
                        return View(advert);
                    }
                }

                var updatedSlider = db.Slider.Find(slider.Id);
                if (slider.FileName != null)
                {
                    var file = Request.Files["FileName"];
                    if (file != null)
                    {
                        Random random = new Random();
                        int number = random.Next(1, 1300);
                        string fileName = DateTime.Now.Ticks.ToString() + number + ".jpg";

                        string path = Path.Combine(Server.MapPath("~/Content/Img/Adverts"), fileName);
                        file.SaveAs(path);
                        slider.FileName = BaisUrlHoste + "Content/Img/Adverts/" + fileName;
                        updatedSlider.FileName = slider.FileName;
                    }
                }
                else
                {
                    updatedSlider.FileName = updatedSlider.FileName;
                }

                updatedSlider.type = slider.type;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(slider);
        }

        [HttpPost]
        public ActionResult ChangeState(int? id)
        {

            var slider = db.Slider.Find(id);
            if (slider.IsActive == true)
            {
                slider.IsActive = false;

            }
            else
            {
                slider.IsActive = true;
            }
            db.SaveChanges();
            return Json(new { key = 1, data = slider.IsActive }, JsonRequestBehavior.AllowGet);

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
