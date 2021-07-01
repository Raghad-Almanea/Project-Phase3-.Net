using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using WaterDelivery.Controllers.Api;
using WaterDelivery.Models;
using WaterDelivery.ViewModels;

namespace WaterDelivery.Controllers
{
    [Authorize(Roles = "ادمن")]
    public class TClientsController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        // GET: Clients
        public ActionResult Index()
        {

            var Tclients = context.Users.Where(u =>
                    u.Roles.Join(context.Roles, usrRole => usrRole.RoleId,
                    role => role.Id, (usrRole, role) => role).Any(r => r.Name.Equals("تاجر"))).Select(c => new ClientViewModel
                    {
                        userId = c.Id,
                        user_name = c.FullName,
                        img = c.Img,
                        city_name = context.City.Where(cc => cc.Id == c.FkCity).Select(cc => cc.name).FirstOrDefault()  /*city.name*/
                    }).ToList();


            

            return View(Tclients);
        }



    }
}