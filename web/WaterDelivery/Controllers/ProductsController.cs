using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WaterDelivery.Controllers.Api;
using WaterDelivery.Models;
using WaterDelivery.Models.TableDb;
using WaterDelivery.ViewModels;

namespace WaterDelivery.Controllers
{
    [Authorize(Roles = "تاجر,ادمن,منتجات")]
    public class ProductsController : BaseController
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: Products
        public ActionResult Index(int? mainCat, int? subCat)
        {
            var currentUserId = User.Identity.GetUserId();
            List<ProductViewModel> products;
            bool isProvider = User.IsInRole("تاجر");
            if (subCat > 0)
            {
                products = (from product in context.Product
                            from Subcategory in context.SubCategory
                            where product.fk_categoryID == Subcategory.Id && Subcategory.Id == subCat
                            from category in context.Category
                            where Subcategory.fk_cat == category.Id && Subcategory.fk_cat == mainCat && ((isProvider && product.ByUser == currentUserId) || !isProvider)
                            select new ProductViewModel
                            {
                                Id = product.Id,
                                branch_name = "",
                                category_name = Subcategory.name,
                                img = product.img,
                                is_active = product.is_active,
                                name = product.name,
                                price = product.price,
                                all_qty = product.all_qty,
                                description = product.description,
                                maincategory_name = category.name
                            }).ToList();
                ViewBag.MainId = mainCat;
                ViewBag.SubId = subCat;
                return View(products);
            }
            else if (mainCat > 0)
            {
                products = (from product in context.Product
                            from Subcategory in context.SubCategory
                            where product.fk_categoryID == Subcategory.Id
                            from category in context.Category
                            where Subcategory.fk_cat == category.Id && Subcategory.fk_cat == mainCat && ((isProvider && product.ByUser == currentUserId) || !isProvider)
                            select new ProductViewModel
                            {
                                Id = product.Id,
                                branch_name = "",
                                category_name = Subcategory.name,
                                img = product.img,
                                is_active = product.is_active,
                                name = product.name,
                                price = product.price,
                                all_qty = product.all_qty,
                                description = product.description,
                                maincategory_name = category.name
                            }).ToList();
                ViewBag.MainId = mainCat;
                ViewBag.SubId = subCat;
                return View(products);
            }
            else
            {
                products = (from product in context.Product
                            from Subcategory in context.SubCategory
                            where product.fk_categoryID == Subcategory.Id
                            from category in context.Category
                            where Subcategory.fk_cat == category.Id && ((isProvider && product.ByUser == currentUserId) || !isProvider)
                            select new ProductViewModel
                            {
                                Id = product.Id,
                                branch_name = "",
                                category_name = Subcategory.name,
                                img = product.img,
                                is_active = product.is_active,
                                name = product.name,
                                price = product.price,
                                all_qty = product.all_qty,
                                description = product.description,
                                maincategory_name = category.name
                            }).ToList();
                ViewBag.MainId = mainCat;
                return View(products);
            }
        }

        // this code dublicate from SubCategoriesController
        public ActionResult GetMainCategories()
        {
            var categories = context.Category.Where(c => c.is_active == true).ToList().Select(c => new
            {
                id = c.Id,
                name = c.name
            });

            return Json(new { key = 1, categories = categories }, JsonRequestBehavior.AllowGet);
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
        public ActionResult Create(CreateProductViewModel createProductViewModel)
        {
            if (ModelState.IsValid)
            {



                if (createProductViewModel.img != null)
                {
                    var file = Request.Files["img"];
                    if (file != null)
                    {
                        Random random = new Random();
                        int number = random.Next(1, 1300);
                        string fileName = DateTime.Now.Ticks.ToString() + number + ".jpg";

                        string path = Path.Combine(Server.MapPath("~/Content/Img/Product"), fileName);
                        file.SaveAs(path);
                        createProductViewModel.img = BaisUrlProduct + fileName;
                    }
                }

                createProductViewModel.is_active = true;
                createProductViewModel.date = DateTime.Now;

                Product product = new Product()
                {
                    price = Convert.ToDouble(createProductViewModel.price),
                    all_qty = createProductViewModel.all_qty,
                    date = createProductViewModel.date,
                    description = createProductViewModel.description,
                    fk_categoryID = createProductViewModel.fk_categoryID,
                    img = createProductViewModel.img,
                    is_active = createProductViewModel.is_active,
                    name = createProductViewModel.name,
                    specification = createProductViewModel.specification,
                    ByUser = User.Identity.GetUserId()
                };





                context.Product.Add(product);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(createProductViewModel);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = context.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            CreateProductViewModel createProductViewModel = new CreateProductViewModel()
            {
                price = product.price.ToString(),
                all_qty = product.all_qty,
                date = product.date,
                description = product.description,
                specification = product.specification,
                fk_categoryID = product.fk_categoryID,
                img = product.img,
                is_active = product.is_active,
                name = product.name,
                Id = product.Id
            };

            return View(createProductViewModel);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateProductViewModel createProductViewModel)
        {
            if (ModelState.IsValid)
            {
                var updatedProduct = context.Product.Find(createProductViewModel.Id);
                if (createProductViewModel.img != null)
                {
                    var file = Request.Files["img"];
                    if (file != null)
                    {
                        Random random = new Random();
                        int number = random.Next(1, 1300);
                        string fileName = DateTime.Now.Ticks.ToString() + number + ".jpg";

                        string path = Path.Combine(Server.MapPath("~/Content/Img/Product"), fileName);
                        file.SaveAs(path);
                        createProductViewModel.img = fileName;
                        updatedProduct.img = BaisUrlProduct + createProductViewModel.img;
                    }
                }
                else
                {
                    updatedProduct.img = updatedProduct.img;
                }
                updatedProduct.description = createProductViewModel.description;
                updatedProduct.specification = createProductViewModel.specification;
                updatedProduct.name = createProductViewModel.name;
                updatedProduct.price = Convert.ToDouble(createProductViewModel.price);
                updatedProduct.all_qty = createProductViewModel.all_qty;
                updatedProduct.fk_categoryID = createProductViewModel.fk_categoryID;

                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(createProductViewModel);
        }

        //[HttpGet]
        //public ActionResult GetBranches()
        //{
        //    var branches = context.Branch.ToList();
        //    return Json(branches, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public ActionResult GetsubCategories()
        {
            var categories = context.SubCategory.Select(c => new
            {
                c.Id,
                c.name,
                c.fk_cat
            }).ToList();
            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetsubCategoriesFilter(int? mainCat)
        {

            if (mainCat > 0)
            {
                var categories = context.SubCategory.Where(x => x.fk_cat == mainCat).Select(c => new
                {
                    c.Id,
                    c.name,
                    c.fk_cat
                }).ToList();
                return Json(new { key = 1, categories = categories }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { key = 0 }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetCategories()
        {
            var categories = context.Category.Select(c => new
            {
                c.Id,
                c.name
            }).ToList();
            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ChangeState(int? id)
        {

            var product = context.Product.Find(id);
            if (product.is_active == true)
            {
                product.is_active = false;

            }
            else
            {
                product.is_active = true;
            }
            context.SaveChanges();
            return Json(new { key = 1, data = product.is_active }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            var product = context.Product.Find(id);

            context.Product.Remove(product);

            context.SaveChanges();
            return Json(new { key = 1 }, JsonRequestBehavior.AllowGet);

        }

    }
}