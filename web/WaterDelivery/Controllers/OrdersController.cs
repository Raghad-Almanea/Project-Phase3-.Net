using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using WaterDelivery.Controllers.Api;
using WaterDelivery.Models;
using WaterDelivery.Models.TableDb;
using WaterDelivery.ViewModels;

namespace WaterDelivery.Controllers
{
    [Authorize(Roles = "ادمن,طلبات,تاجر")]
    public class OrdersController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        // GET: Orders

        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();
            bool isProvider = User.IsInRole("تاجر");

            var orders = (from client in context.Client
                          join order in context.Order on client.id equals order.fk_userID
                          join orderInfo in context.OrderInfo on order.Id equals orderInfo.fk_orderID
                          join product in context.Product on orderInfo.fk_product equals product.Id
                          where ((isProvider && product.ByUser == currentUserId) || !isProvider)
                          select new OrderViewModel
                          {
                              Id = order.Id,
                              user_name = client.user_name,
                              user_phone = client.phone,
                              address = order.address,
                              //branch_name = "",
                              mandoob_name = context.Provider.FirstOrDefault(p => p.id == order.fk_providerID).user_name ?? "---",
                              date = order.delivary_time,
                              date_time = order.date_time,
                              total = order.net_total,
                              order_status = order.type,
                              is_active = order.is_active,
                              lat = order.lat,
                              lng = order.lng
                          }).OrderByDescending(o => o.Id).DistinctBy(o => o.Id).ToList();

            return View(orders);
        }

        public ActionResult ShowOrderDetails(int id)
        {
            var currentUserId = User.Identity.GetUserId();
            bool isProvider = User.IsInRole("تاجر");

            var orderDetails = (from orderInfo in context.OrderInfo
                                join product in context.Product on orderInfo.fk_product equals product.Id
                                where orderInfo.fk_orderID == id && ((isProvider && product.ByUser == currentUserId) || !isProvider)
                                select new
                                {
                                    product_name = product.name,
                                    product_img = product.img,
                                    product.price,
                                    orderInfo.qty,
                                    category_name = context.SubCategory.FirstOrDefault(c => c.Id == product.fk_categoryID).name,
                                    main_category_name = context.Category.Where(c => c.Id == context.SubCategory.FirstOrDefault(cc => cc.Id == product.fk_categoryID).fk_cat).Select(c => c.name).FirstOrDefault()
                                    ,
                                }).ToList();

            return Json(orderDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ChangeStateOrder(int? id)
        {

            var order = context.Order.Find(id);
            if (order.type == (int)Order_type.accept_deleget)
            {
                order.type = (int)Order_type.dashborad_Ready;
                context.SaveChanges();
                Notify notify = new Notify();
                notify.date = DateTime.Now;
                notify.fk_user = order.fk_userID;
                notify.order_id = id;
                notify.order_type = (int)Order_type.dashborad_Ready;
                notify.text = "تم تجهيز الطلب رقم " + id;
                notify.type = 1;
                context.Notify.Add(notify);
                context.SaveChanges();
                BaseController.SendPushNotification(order.fk_userID, order.Id, (int)Order_type.dashborad_Ready, "تم تجهيز الطلب رقم  " + id, 1);
                Notify notify1 = new Notify();
                notify1.date = DateTime.Now;
                notify1.fk_provider = (int)order.fk_providerID;
                notify1.order_id = id;
                notify1.order_type = (int)Order_type.dashborad_Ready;
                notify1.text = "تم تجهيز الطلب رقم " + id;
                notify.type = 2;
                context.Notify.Add(notify1);
                context.SaveChanges();
                BaseController.SendPushNotification((int)order.fk_providerID, order.Id, (int)Order_type.dashborad_Ready, "تم تجهيز الطلب رقم  " + id, 1);
                return Json(new { key = 1, data = "" }, JsonRequestBehavior.AllowGet);
            }
            if (order.type == (int)Order_type.New)
            {

                return Json(new { key = 1, data = "لا يمكن تجهيز طلب لم يوافق عليه المندوب بعد" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { key = 1, data = "" }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult ChangeState(int? id)
        {

            var order = context.Order.Find(id);
            if (order.is_active == true)
            {
                order.is_active = false;

            }
            else
            {
                order.is_active = true;
            }
            context.SaveChanges();
            return Json(new { key = 1, data = order.is_active }, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult GetBranches()
        //{
        //    var branches = context.Branch.ToList();
        //    return Json(branches, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult GetOrdersByBranch(int id)
        {

            if (id == 0)
            {
                var allOrders = (from client in context.Client
                                 join order in context.Order on client.id equals order.fk_userID
                                 select new OrderViewModel
                                 {
                                     Id = order.Id,
                                     user_name = client.user_name,
                                     user_phone = client.phone,
                                     address = order.address,
                                     //branch_name = "",
                                     total = order.total,
                                     order_status = order.type,
                                     is_active = order.is_active
                                 }).ToList();

                return Json(allOrders, JsonRequestBehavior.AllowGet);
            }

            var orders = (from client in context.Client
                          join order in context.Order on client.id equals order.fk_userID
                          select new OrderViewModel
                          {
                              Id = order.Id,
                              user_name = client.user_name,
                              user_phone = client.phone,
                              address = order.address,
                              //branch_name = "",
                              total = order.total,
                              order_status = order.type,
                              is_active = order.is_active
                          }).ToList();

            return Json(orders, JsonRequestBehavior.AllowGet);
        }
    }
}