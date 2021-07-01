using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using WaterDelivery.Models;
using WaterDelivery.ViewModels;
using System.Drawing.Printing;
using WaterDelivery.Controllers.Api;
using System.IO;
using WaterDelivery.Models.TableDb;
using System.Net.Http.Headers;
using System.Web.Razor.Parser;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml;

namespace WaterDelivery.Controllers
{

    public class SiteHomeController : BaseController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            int UserId = GetUserId();
            if (UserId == 0)
            {
                UserId = -1;
            }
            HomeViewModel model = new HomeViewModel();

            List<SliderViewModel> SliderViewModel = db.Slider.Where(x => x.IsActive == true && x.IsDeleted == false)
                .Select(x => new SliderViewModel
                {
                    SliderId = x.Id,
                    Img = x.FileName,
                    type = x.type
                }).ToList();

            model.Sliders.AddRange(SliderViewModel);

            List<CategoryViewModel> Categorys = db.SubCategory.Where(x => x.is_active == true).Include("Product").Select(x => new CategoryViewModel
            {
                Id = x.Id,
                Name = x.name,
                Products = db.Product.Where(p => p.fk_categoryID == x.Id).Select(product => new ProductHomeViewModel()
                {
                    Price = product.price,
                    ProductId = product.Id,
                    ProductImg = product.img,
                    ProductName = product.name,
                    IsFavourite = db.Favorites.Any(f => f.FkProductId == product.Id && f.FkUserId == UserId)
                }).ToList()


            }).ToList();

            model.Categorys.AddRange(Categorys);

            //return Json(new { Categorys }, JsonRequestBehavior.AllowGet);
            return View(model);
        }



        [HttpPost]
        public async Task<ActionResult> GetProductInfo(int productId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    var product = await db.Product.Where(p => p.Id == productId).Select(p => new { 
                        id = p.Id,
                        name = p.name,
                        price = p.price,
                        img = p.img,
                        description = p.description,
                        specification = p.specification
                    }).FirstOrDefaultAsync();

                    return Json(new
                    {
                        key = 1,
                        data = product,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    key = 0,
                    msg = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }



        //public ActionResult AllProducts(int id)
        public ActionResult AllProducts()
        {

            int UserId = GetUserId();
            if (UserId == 0)
            {
                UserId = -1;
            }
            //var category = db.Category.FirstOrDefault(x => x.Id == id);


            //var products = db.Product.Where(p => p.fk_categoryID == id && p.is_active == true).Select(p => new ProductHomeViewModel
            var products = db.Product.Where(p => p.is_active == true).Select(p => new ProductHomeViewModel
            {
                ProductId = p.Id,
                ProductName = p.name.Substring(0, 48),
                QtyCart = db.Cart.Where(c => c.fk_productID == p.Id && c.fk_userID == UserId).Select(c => c.qty).FirstOrDefault(),
                ProductImg = p.img,
                Price = p.price,
                IsFavourite = db.Favorites.Any(f => f.FkProductId == p.Id && f.FkUserId == UserId)
            }).ToList();
            //if (category != null)
            //{
            //    ViewBag.CategoryName = category.name;
            //}
            return View(products);
        }

        public ActionResult MyFavorites()
        {

            int UserId = GetUserId();
            if (UserId == 0)
            {
                UserId = -1;
            }

            var favorites = (from favorite in db.Favorites
                             join product in db.Product on favorite.FkProductId equals product.Id
                             where favorite.FkUserId == UserId
                             select new ProductHomeViewModel()
                             {
                                 ProductId = product.Id,
                                 ProductName = product.name.Substring(0, 48),
                                 ProductImg = product.img,
                                 Price = product.price,
                                 IsFavourite = db.Favorites.Any(f => f.FkProductId == product.Id && f.FkUserId == UserId)
                             }).ToList();




            return View(favorites);
        }

        [HttpPost]
        public async Task<ActionResult> AddOrRemoveFavorite(int productId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    int currentUserId = GetUserId();
                    var currentUser = db.Client.Find(currentUserId);
                    Favorite favorite = await db.Favorites.FirstOrDefaultAsync(c => c.FkProductId == productId && c.FkUserId == currentUserId);
                    if (favorite == null)
                    {
                        db.Favorites.Add(new Favorite() { FkProductId = productId, FkUserId = currentUserId });
                        await db.SaveChangesAsync();
                        return Json(new
                        {
                            key = 1,
                            msg = creatMessage(currentUser.lang, "تم اضافة المنتج الى المفضلة بنجاح ", "The product has been successfully added to your favorites ")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db.Favorites.Remove(favorite);
                        await db.SaveChangesAsync();
                        return Json(new
                        {
                            key = 1,
                            msg = creatMessage(currentUser.lang, "تم ازالة المنتج من المفضلة بنجاح ", "The product has been removed from your favorites ")
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    key = 0,
                    msg = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ConvertPointsToWalletCash(int points)
        {
            try
            {

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var currentUserId = GetUserId();
                    var client = db.Client.FirstOrDefault(c => c.id == currentUserId);
                    var setting = db.Setting.FirstOrDefault();
                    if (points < setting.PointsPerRiyal)
                    {
                        return Json(new { key = 0, msg = creatMessage(client.lang, "اقل عدد نقاط للتحويل هو  " + setting.PointsPerRiyal, "The minimum points to transfer are " + setting.PointsPerRiyal) });
                    }
                    else
                    {
                        var riyalWalletPoints = points / setting.PointsPerRiyal;
                        var remaining = points % setting.PointsPerRiyal;
                        client.wallet += riyalWalletPoints;
                        client.Points = remaining;
                        db.SaveChanges();
                        return Json(new { key = 1, points = client.Points, wallet = Math.Round(client.wallet, 2), msg = creatMessage(client.lang, "تم تحويل النقاط بنجاح ", "The points transferred successfully") });
                    }

                }



            }
            catch (Exception ex)
            {
                return Json(new
                {
                    key = 0,
                    msg = ex.Message
                });
            }
        }

        [HttpGet]
        public ActionResult GetNotify()
        {
            int UserId = GetUserId();

            List<NotifyViewModel> notify = new List<NotifyViewModel>();
            if (UserId != 0)
            {
                notify = db.Notify.Where(x => x.fk_user == UserId).Select(x => new NotifyViewModel
                {
                    text = x.text,
                    order_id = x.order_id,
                    order_type = x.order_type,
                    Id = x.Id
                }).OrderByDescending(x => x.Id).ToList();

                var notify_not_shown = db.Notify.Where(x => x.fk_user == UserId && x.fk_user_show == false).ToList();
                if (notify_not_shown.Count != 0)
                {
                    notify_not_shown.ForEach(a => a.fk_user_show = true);
                    db.SaveChanges();
                }
            }

            //return Json(new
            //{
            //    key = 1,
            //    notify = notify
            //}, JsonRequestBehavior.AllowGet);
            return View(notify);

        }



        public ActionResult GetCountNotify(string lang)
        {
            int UserId = GetUserId();
            if (UserId != 0)
            {
                var CountNotify = db.Notify.Where(x => x.fk_user == UserId && x.fk_user_show == false).Count();
                return Json(new { key = 1, count = CountNotify }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { key = 1, count = 0 }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult MyOrders()
        {
            int UserId = GetUserId();
            List<OrderSiteViewModel> Orders = new List<OrderSiteViewModel>();
            if (UserId != 0)
            {
                Orders = db.Order.Include("Client").Where(x => x.fk_userID == UserId).Select(x => new OrderSiteViewModel
                {
                    Id = x.Id,
                    NumberOrder = x.Id,
                    Status = x.type,
                    Date = x.date_time
                }).OrderByDescending(x => x.Id).ToList();
            }
            return View(Orders);
        }

        public ActionResult DetailsOrder(int id)
        {
            List<OrderInfoViewModel> Details = db.OrderInfo.Where(x => x.fk_orderID == id).Select(x => new OrderInfoViewModel
            {
                Name = x.name,
                Img = x.img,
                Qty = x.qty,
                Price = x.price,
            }).ToList();

            var Ordertype = db.Order.Where(x => x.Id == id).Select(x => x.type).FirstOrDefault();

            ViewBag.Ordertype = Ordertype;
            ViewBag.OrderNumber = id;

            return View(Details);
        }

        #region EditOrder

        public ActionResult EditOrder(int id)
        {
            List<OrderInfoViewModel> Details = db.OrderInfo.Where(x => x.fk_orderID == id).Select(x => new OrderInfoViewModel
            {
                ProductId = x.fk_product,
                Name = x.name,
                Img = x.img,
                Qty = x.qty,
                Price = x.price
            }).ToList();

            ViewBag.OrderNumber = id;
            return View(Details);
        }

        public ActionResult GetCostOrder(int orderId)
        {
            int UserId = GetUserId();

            if (UserId != 0)
            {

                var Order = db.Order.Where(x => x.fk_userID == UserId && x.Id == orderId)
                    .Select(x => new
                    {
                        Totat = x.total,
                        Delivery = x.delivary,
                        Net_totat = x.net_total
                    }).FirstOrDefault();
                return Json(new { key = 1, Order = Order }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { key = 0, msg = "حدث خطا ما" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditProductInOrder(int productId, int type, int orderId)
        {
            int UserId = GetUserId();

            if (UserId != 0)
            {
                var order = db.Order.FirstOrDefault(x => x.Id == orderId && x.fk_userID == UserId);

                if (order == null)
                {
                    return Json(new { key = 0, msg = "هذا الطلب لم يعد موجود" }, JsonRequestBehavior.AllowGet);
                }

                if (order.type == 3)
                {
                    return Json(new { key = 0, msg = "هذا الطلب لم يعد موجود" }, JsonRequestBehavior.AllowGet);
                }

                var check_product = db.Product.FirstOrDefault(p => p.Id == productId);

                if (check_product.all_qty == 0)
                {
                    return Json(new { key = 0, msg = $" عفوا  { check_product.name}   غير متوفر حاليا .. " }, JsonRequestBehavior.AllowGet);
                }
                //InCrease Qty in Order
                var orderinfo = db.OrderInfo.Where(x => x.fk_orderID == orderId && x.fk_product == productId).FirstOrDefault();
                if (orderinfo != null)
                {
                    if (type == 1)
                    {
                        if (check_product.all_qty < orderinfo.qty + 1)
                        {
                            return Json(new { key = 0, msg = $" عفوا  { check_product.name}   غير متوفر بهذه الكمية حاليا .. " }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {

                            if (check_product.all_qty > 1)
                            {
                                check_product.all_qty = check_product.all_qty - 1;

                                orderinfo.qty = orderinfo.qty + 1;
                                db.SaveChanges();
                                double total = db.OrderInfo.Where(x => x.fk_orderID == order.Id).Select(x => x.qty * x.price).DefaultIfEmpty(0).Sum();

                                order.total = total;
                                order.delivary = order.delivary;
                                order.net_total = total + order.delivary;
                                db.SaveChanges();

                                return Json(new { key = 1, msg = "تم تغيير كمية المنتج بنجاح.." }, JsonRequestBehavior.AllowGet);

                            }
                            else
                            {
                                return Json(new { key = 0, msg = "المنتج " + check_product.name + " لم يعد موجود" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    //DeCrease Qty in Order
                    else
                    {
                        if (orderinfo.qty > 1)
                        {
                            orderinfo.qty = orderinfo.qty - 1;
                            check_product.all_qty = check_product.all_qty + 1;

                            db.SaveChanges();
                            double total = db.OrderInfo.Where(x => x.fk_orderID == order.Id).Select(x => x.qty * x.price).DefaultIfEmpty(0).Sum();

                            order.total = total;
                            order.delivary = order.delivary;
                            order.net_total = total + order.delivary;
                            db.SaveChanges();

                            return Json(new { key = 1, msg = "تم تغيير كمية المنتج بنجاح.." }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            db.OrderInfo.Remove(orderinfo);
                            db.SaveChanges();

                            double total = db.OrderInfo.Where(x => x.fk_orderID == order.Id).Select(x => x.qty * x.price).DefaultIfEmpty(0).Sum();

                            order.total = total;
                            order.delivary = order.delivary;
                            order.net_total = total + order.delivary;
                            db.SaveChanges();

                            if (order.total != 0)
                            {
                                return Json(new { key = 2, msg = "تم ازالة المنتج من الطلب.." }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                db.Order.Remove(order);
                                db.SaveChanges();
                                return Json(new { key = 3, msg = "تم حذف الطلب بالكامل ...." }, JsonRequestBehavior.AllowGet);
                            }

                        }
                    }
                }
            }



            return Json(new { key = 1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteProductFromOrder(int productId, int orderId)
        {
            int UserId = GetUserId();

            if (UserId != 0)
            {
                var order = db.Order.FirstOrDefault(x => x.Id == orderId && x.fk_userID == UserId);
                if (order == null)
                {
                    return Json(new { key = 0, msg = "هذا الطلب لم يعد موجود" }, JsonRequestBehavior.AllowGet);
                }

                if (order.type == 3)
                {
                    return Json(new { key = 0, msg = "هذا الطلب لم يعد موجود" }, JsonRequestBehavior.AllowGet);
                }

                var orderinfo = db.OrderInfo.Where(x => x.fk_orderID == orderId && x.fk_product == productId).FirstOrDefault();
                if (orderinfo != null)
                {
                    db.OrderInfo.Remove(orderinfo);
                    db.SaveChanges();

                    double total = db.OrderInfo.Where(x => x.fk_orderID == order.Id).Select(x => x.qty * x.price).DefaultIfEmpty(0).Sum();

                    order.total = total;
                    order.delivary = order.delivary;
                    order.net_total = total + order.delivary;
                    db.SaveChanges();

                    var CountOrderInfo = db.OrderInfo.Where(x => x.fk_orderID == orderId).Count();
                    if (CountOrderInfo == 0)
                    {
                        db.Order.Remove(order);
                        db.SaveChanges();
                        return Json(new { key = 2, msg = "تم حذف الطلب بنجاح.." }, JsonRequestBehavior.AllowGet);

                    }
                    return Json(new { key = 1, Count = CountOrderInfo, msg = "تم حذف المنتح بنجاح.." }, JsonRequestBehavior.AllowGet);

                }
            }


            return Json(new { key = 1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteOrder(int orderId)
        {
            int UserId = GetUserId();

            if (UserId != 0)
            {
                var order = db.Order.Where(x => x.Id == orderId && x.fk_userID == UserId).FirstOrDefault();
                if (order == null)
                {
                    return Json(new { key = 0, msg = "هذا الطلب لم يعد موجود" }, JsonRequestBehavior.AllowGet);
                }

                if (order.type == 3)
                {
                    return Json(new { key = 0, msg = "هذا الطلب لم يعد موجود" }, JsonRequestBehavior.AllowGet);
                }

                var orderinfo = db.OrderInfo.Where(x => x.fk_orderID == orderId).ToList();
                if (orderinfo.Count() > 0)
                {
                    db.OrderInfo.RemoveRange(orderinfo);
                    db.SaveChanges();
                    db.Order.Remove(order);
                    db.SaveChanges();


                    return Json(new { key = 1, msg = "تم حذف الطلب بنجاح.." }, JsonRequestBehavior.AllowGet);

                }
            }
            return Json(new { key = 0 }, JsonRequestBehavior.AllowGet);

        }



        #endregion




        public ActionResult ContactUs()
        {
            return View();
        }



        [HttpPost]
        public ActionResult ContactUs(string Email, string Name, string message)
        {
            //Check Is Email
            if (!IsValidEmail(Email))
            {
                return Json(new { key = 0, msg = "برجاء ادخال بريد الكترونى صحيح" }, JsonRequestBehavior.AllowGet);
            }
            Complaints contact = new Complaints
            {
                email = Email,
                name = Name,
                text = message
            };
            db.Complaints.Add(contact);
            db.SaveChanges();
            return Json(new { key = 1, msg = "تم الارسال ينجاح.." }, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public ActionResult DeleteProduct(int productId)
        {
            int UserId = GetUserId();
            if (UserId != 0)
            {
                var Products = db.Cart.Where(x => x.fk_productID == productId && x.fk_userID == UserId).FirstOrDefault();
                if (Products != null)
                {
                    db.Cart.Remove(Products);
                    db.SaveChanges();
                    var CountCart = db.Cart.Where(x => x.fk_userID == UserId).Count();

                    return Json(new { key = 1, CountCart = CountCart, msg = "تم حذف المنتج من السلة" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { key = 0, msg = "حدث خطا ما " }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { key = 0, msg = "حدث خطا ما " }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Cart()
        {
            int UserId = GetUserId();
            List<CartViewModel> Products = new List<CartViewModel>();
            if (UserId != 0)
            {
                Products = db.Cart.Include(x => x.fk_product).Where(x => x.fk_userID == UserId).Select(x => new CartViewModel
                {
                    ProductId = x.fk_product.Id,
                    Name = x.fk_product.name,
                    Img = x.fk_product.img,
                    Price = x.fk_product.price,
                    Qty = x.qty
                }).ToList();
            }
            return View(Products);
        }

        [HttpPost]
        public ActionResult AddCart(int productId, int type)
        {
            int UserId = GetUserId();
            if (UserId != 0)
            {
                if (type == 1)
                {
                    var check_product = db.Product.FirstOrDefault(p => p.Id == productId);
                    if (check_product.all_qty == 0)
                    {
                        return Json(new { key = 0, msg = $" عفوا  { check_product.name}   غير متوفر حاليا .. " }, JsonRequestBehavior.AllowGet);
                    }
                    var cartfound = db.Cart.FirstOrDefault(x => x.fk_productID == productId && x.fk_userID == UserId);
                    if (cartfound != null)
                    {
                        cartfound.qty = cartfound.qty + 1;
                        db.SaveChanges();
                    }
                    else
                    {
                        Cart cart = new Cart()
                        {
                            fk_productID = productId,
                            qty = 1,
                            fk_userID = UserId
                        };
                        db.Cart.Add(cart);
                        db.SaveChanges();
                    }
                    return Json(new { key = 1, msg = $" تم اضافة { check_product.name}   الى السلة بنجاح .. " }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var cartfound = db.Cart.FirstOrDefault(x => x.fk_productID == productId && x.fk_userID == UserId);
                    if (cartfound != null)
                    {
                        if (cartfound.qty > 1)
                        {
                            cartfound.qty = cartfound.qty - 1;
                            db.SaveChanges();
                            return Json(new { key = 0, msg = "تم تغيير كمية المنتج بالسلة بنجاح.." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            db.Cart.Remove(cartfound);
                            db.SaveChanges();
                            return Json(new { key = 0, msg = "تم ازالة المنتج من السلة بنجاح.." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { key = 0, msg = "تم ازالة المنتج من السلة بنجاح.." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return Json(new { key = 0, msg = "يرجى تسجيل الدخول اولا .." }, JsonRequestBehavior.AllowGet);
            }
            //return Json(new { key = 0, msg = "يرجى تسجيل الدخول اولا .." }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetProductsCart()
        {
            int UserId = GetUserId();
            if (UserId != 0)
            {

            }
            return View();
        }

        [HttpPost]
        public ActionResult GetCartCount(string lang = "ar")
        {
            int UserId = GetUserId();
            if (UserId != 0)
            {
                var cart = db.Cart.Include(x => x.fk_product).Where(x => x.fk_userID == UserId);

                int count = cart.Select(x => x.qty).DefaultIfEmpty(0).Sum();
                double price = cart.Select(x => x.fk_product.price * x.qty).DefaultIfEmpty(0).Sum();

                return Json(new { key = 1, count = count, price = price }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { key = 0, count = 0, price = 0 }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ConfirmOrder()
        {
            int UserId = GetUserId();
            CostOrderViewModel model = new CostOrderViewModel();
            if (UserId != 0)
            {
                var setting = db.Setting.FirstOrDefault();
                double Delivery = db.Setting.Select(x => x.delivery).FirstOrDefault();

                double total = db.Cart.Include(x => x.fk_product).Where(x => x.fk_userID == UserId).Select(x => x.qty * x.fk_product.price).DefaultIfEmpty(0).Sum();

                double net_total = Delivery + total + (total * setting.vat / 100);

                model.Delivery = Delivery;
                model.total = total;
                model.net_total = net_total;
                model.vat = setting.vat;

                List<AddressViewModel> PerviousAddress = db.AddressUser.Where(x => x.fk_userID == UserId)
                      .Select(x => new AddressViewModel
                      {
                          Id = x.Id,
                          address = x.address
                      }).ToList();

                ViewBag.PerviousAddress = PerviousAddress;
            }
            return View(model);
        }

        public ActionResult AddOrder(string paymentType, string deliveryTime, string Lat = "", string Lng = "", string Address = "", int previousLocationId = 0)
        {
            int UserId = GetUserId();
            Setting setting = db.Setting.FirstOrDefault();
            if (UserId != 0)
            {
                if (Lat == "" && Lng == "" && Address == "" && previousLocationId == 0)
                {
                    return Json(new { key = 0, msg = "برجاء تحديد الموقع .." }, JsonRequestBehavior.AllowGet);
                }
                double Delivery = db.Setting.Select(x => x.delivery).FirstOrDefault();
                double total = db.Cart.Include(x => x.fk_product).Where(x => x.fk_userID == UserId).Select(x => x.qty * x.fk_product.price).DefaultIfEmpty(0).Sum();
                double net_total = Delivery + total;

                if (previousLocationId != 0)
                {
                    var previousLocation = db.AddressUser.Where(x => x.Id == previousLocationId).FirstOrDefault();
                    Lat = previousLocation.lat;
                    Lng = previousLocation.lng;
                    Address = previousLocation.address;
                }

                Order order = new Order()
                {
                    fk_userID = UserId,
                    fk_providerID = null,
                    address = Address,
                    lat = Lat,
                    lng = Lng,
                    date = TimeNow().ToString("dd/MM/yyyy"),
                    delivary = Delivery,
                    net_total = net_total,
                    total = total,
                    type = 1,
                    payment_type = Convert.ToInt32(paymentType),
                    delivary_time = deliveryTime,
                };

                if (order.payment_type == (int)Payment_type.Wallet)
                {
                    var client = db.Client.Find(UserId);
                    

                    if (client.wallet < total)
                    {
                        return Json(new { key = 0, msg = creatMessage(client.lang, "رصيد المحفظة غير كافي لاتمام عملية الدفع", "Your wallet balance not enough to complete the payment process") }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        client.wallet -= total;
                        order.is_paied = 2;
                        client.Points += setting.PointsPerOrder;
                        db.SaveChanges();
                    }
                }

                var cart = db.Cart.Where(x => x.fk_userID == UserId).ToList();
                if (cart.Count == 0)
                {
                    return Json(new { key = 0, msg = "برجاء اضافة منتجات الى العربة" }, JsonRequestBehavior.AllowGet);
                }

                // check order's products qty
                foreach (var item in cart)
                {
                    var check_product = db.Product.FirstOrDefault(p => p.Id == item.fk_productID);

                    if (check_product.all_qty == 0)
                    {
                        return Json(new { key = 0, msg = "المنتج " + check_product.name + " لم يعد موجود" }, JsonRequestBehavior.AllowGet);
                    }
                    if (item.qty > check_product.all_qty)
                    {
                        return Json(new { key = 0, msg = "الحد الاقصى لمنتج " + check_product.name + " هو " + check_product.all_qty }, JsonRequestBehavior.AllowGet);
                    }
                }

                db.Order.Add(order);
                db.SaveChanges();

                foreach (var item in cart)
                {
                    OrderInfo oinfo = new OrderInfo()
                    {
                        fk_orderID = order.Id,
                        fk_product = item.fk_productID,
                        qty = item.qty,
                        img = item.fk_product.img,
                        name = item.fk_product.name,
                        price = item.fk_product.price
                    };
                    db.OrderInfo.Add(oinfo);
                }
                db.SaveChanges();

                if (cart.Count != 0)
                {
                    db.Cart.RemoveRange(cart);
                    db.SaveChanges();
                }

                var ChkLocation = db.AddressUser.Where(x => x.fk_userID == UserId && (x.lat == Lat || x.address == Address)).FirstOrDefault();
                if (ChkLocation == null)
                {
                    AddressUser newaddress = new AddressUser
                    {
                        lat = Lat,
                        lng = Lng,
                        address = Address,
                        fk_userID = UserId,
                        title = "العنوان الحالى",
                        is_used = true,
                    };
                    db.AddressUser.Add(newaddress);
                    db.SaveChanges();
                }

                // هنعمل اشعار لكل المندوبين
                var Providers = db.Provider.Where(x => x.active == true).ToList();
                foreach (var item in Providers)
                {
                    Notify notify = new Notify();
                    notify.date = DateTime.Now;
                    notify.fk_provider = item.id;
                    notify.order_id = order.Id;
                    notify.order_type = order.type;
                    notify.text = "تم اضافة الطلب رقم " + order.Id + " بنجاح";

                    db.Notify.Add(notify);
                    SendPushNotification(item.id, order.Id, order.type, "هناك طلب جديد فى قائمة الطلبات برجاء الاطلاع", 2);
                }
                db.SaveChanges();

                return Json(new
                {
                    key = 1,
                    order_id = order.Id,
                    msg = "تم اضافة الطلب بنجاح"
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { key = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Conditions()
        {
            var Condition = db.Setting.Select(x => x.Condtions).FirstOrDefault();
            ViewBag.Condition = Condition;
            return View();
        }

        public ActionResult AboutUs()
        {
            var aboutUs = db.Setting.Select(x => x.aboutUs).FirstOrDefault();
            ViewBag.aboutUs = aboutUs;
            return View();
        }


        public ActionResult MyProfile()
        {
            int UserId = GetUserId();
            if (UserId != 0)
            {
                var Profile = db.Client.Where(x => x.id == UserId).FirstOrDefault();
                var setting = db.Setting.FirstOrDefault();
                if (Profile != null)
                {
                    ProfileViewModel model = new ProfileViewModel();
                    model.Id = Profile.id;
                    model.Name = Profile.user_name;
                    model.Phone = Profile.phone;
                    model.Img = Profile.img;
                    model.City_Id = Profile.fk_cityID;
                    model.Wallet = Math.Round(Profile.wallet, 2);
                    model.Points = Profile.Points;

                    ViewBag.Cities = GetCitiess();
                    ViewBag.Setting = setting;
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "SiteHome");
                }
            }
            else
            {
                return RedirectToAction("Index", "SiteHome");
            }
        }

        [HttpPost]
        public ActionResult EditProfile(string Name, string Phone, int Id, int CityId, string Img)
        {
            var UserDB = db.Client.Where(x => x.id == Id).FirstOrDefault();
            if (UserDB != null)
            {
                var phone = (from st in db.Client where st.phone == Phone && st.id != UserDB.id select st).FirstOrDefault();
                var phone_provider = (from st in db.Provider where st.phone == Phone select st).FirstOrDefault();

                if (phone != null || phone_provider != null)
                {
                    return Json(new { key = 0, msg = "عذرا هذا الجوال موجود بالفعل" }, JsonRequestBehavior.AllowGet);
                }
                var ChkName = (from st in db.Client where st.user_name == Name && st.id != UserDB.id select st).FirstOrDefault();

                if (ChkName != null)
                {
                    return Json(new { key = 0, msg = "عذرا هذا الاسم موجود بالفعل" }, JsonRequestBehavior.AllowGet);
                }


                UserDB.user_name = Name;
                UserDB.phone = Phone;
                UserDB.fk_cityID = CityId;
                var file = Request.Files["Img"];
                if (file != null)
                {
                    Random rndm = new Random();
                    int count = rndm.Next(1, 1300);
                    string extension = DateTime.Now.Ticks.ToString() + count + ".jpg";


                    string path = Path.Combine(Server.MapPath("~/Content/Img/User/"), extension);
                    file.SaveAs(path);


                    UserDB.img = BaisUrlHoste + "Content/Img/User/" + extension;
                }
                db.SaveChanges();
                return Json(new { key = 1, msg = "تم التعديل بنجاح.." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { key = 0, msg = "تم التحقق من البيانات .." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditPassword(int Id, string OldPassword, string NewPassword)
        {
            var UserDB = db.Client.Where(x => x.id == Id).FirstOrDefault();
            if (UserDB != null)
            {
                if (UserDB.password != OldPassword)
                {
                    return Json(new { key = 0, msg = "يرجى التحقق من كلمة المرور القديمة .." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    UserDB.password = NewPassword;
                    db.SaveChanges();
                    return Json(new { key = 1, msg = "تم تغيير كلمة المرور .." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { key = 0, msg = "تم التحقق من البيانات .." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Search()
        {
            var Products = db.Product.Where(x => x.is_active == true).Select(x => new { x.Id, x.name }).ToList();
            return Json(new { Products = Products }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSearch(int id)
        {
            int UserId = GetUserId();
            if (UserId == 0)
            {
                UserId = -1;
            }

            var Product = db.Product.Where(x => x.Id == id).Select(x => new ProductViewModel
            {
                Id = x.Id,
                img = x.img,
                name = x.name.Substring(0, 48),
                price = x.price,
                all_qty = db.Cart.Where(c => c.fk_productID == x.Id && c.fk_userID == UserId).Select(c => c.qty).FirstOrDefault(),
                // category_name = x.fk_category.name
            }).FirstOrDefault();

            return View(Product);
        }

        public ActionResult Logout()
        {
            int UserId = GetUserId();
            if (UserId != 0)
            {
                if (Request.Cookies["userInfo"] != null)
                {
                    Response.Cookies["userInfo"].Expires = DateTime.Now.AddDays(-1);
                }
                return RedirectToAction("Index", "SiteHome");
            }
            else
            {
                return RedirectToAction("Index", "SiteHome");
            }
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