//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using WaterDelivery.Models;
//using WaterDelivery.Models.TableDb;

//namespace WaterDelivery.Controllers
//{
//    [Authorize]
//    public class BranchesController : Controller
//    {
//        private ApplicationDbContext db = new ApplicationDbContext();

//        // GET: Branches
//        public ActionResult Index()
//        {
//            return View(db.Branch.ToList());
//        }

//        // GET: Branches/Create
//        //public ActionResult Create()
//        //{
//        //    return View();
//        //}

//        // POST: Branches/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "Id,name,date,is_active")] Branch branch)
//        {
//            if (ModelState.IsValid)
//            {
//                branch.is_active = true;
//                branch.date = DateTime.Now;
//                db.Branch.Add(branch);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            return View(branch);
//        }

//        // GET: Branches/Edit/5
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Branch branch = db.Branch.Find(id);
//            if (branch == null)
//            {
//                return HttpNotFound();
//            }
//            return View(branch);
//        }

//        // POST: Branches/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "Id,name,date,is_active")] Branch branch)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(branch).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            return View(branch);
//        }

//        [HttpPost]
//        public ActionResult ChangeState(int? id)
//        {

//            var branch = db.Branch.Find(id);
//            if (branch.is_active == true)
//            {
//                branch.is_active = false;

//            }
//            else
//            {
//                branch.is_active = true;
//            }
//            db.SaveChanges();
//            return Json(new { key = 1, data = branch.is_active }, JsonRequestBehavior.AllowGet);

//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
