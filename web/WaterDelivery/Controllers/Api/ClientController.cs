using WaterDelivery.Models;
using WaterDelivery.Models.TableDb;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PagedList;
using WaterDelivery.ViewModels;

namespace WaterDelivery.Controllers.Api
{
    public class ClientController : BaseController
    {
        #region User

        [HttpPost]  //OK
        public ActionResult Register(Client User, userModel userModel)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    #region validation
                    //name

                    if (User.user_name == null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(User.lang, "Please enter your Name", "Please enter your Name")
                        }, JsonRequestBehavior.AllowGet);
                    }

                    //phone
                    if (User.phone == null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(User.lang, "Please enter your phone number", "Please enter your phone number")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var phone = (from st in db.Client where st.phone == User.phone select st).FirstOrDefault();
                    var phone_provider = (from st in db.Provider where st.phone == User.phone select st).FirstOrDefault();

                    if (phone != null || phone_provider != null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(User.lang, "Sorry this mobile is already exists", "Sorry this mobile is already exists")
                        }, JsonRequestBehavior.AllowGet);
                    }

                    //Password
                    if (User.password == null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(User.lang, " Please enter your password", "Please enter your password")
                        }, JsonRequestBehavior.AllowGet);
                    }


                    #endregion
                    User.img = BaisUrlHoste + "Content/Img/User/generic-user.png";
                    User.active = true;
                    User.active_code = false;
                    User.date = DateTime.Now;
                    User.code = GetFormNumber();
                    db.Client.Add(User);
                    db.SaveChanges();
                    AddressUser addressUser = new AddressUser()
                    {
                        address = userModel.address,
                        fk_userID = User.id,
                        is_used = true,
                        lat = userModel.lat,
                        lng = userModel.lng,
                        title = "current address"
                    };
                    db.AddressUser.Add(addressUser);
                    db.SaveChanges();
                    Device_Id d = new Device_Id()
                    {
                        device_id = userModel.device_id,
                        fk_user = User.id
                    };
                    db.Device_Id.Add(d);
                    db.SaveChanges();

                    // get city data
                    // var city = db.City.FirstOrDefault(c => c.Id == User.fk_cityID);

                    //Send Massage to phone

                    string s = SendMessageText("verfication code ", User.phone, User.code);

                    return Json(new
                    {
                        key = 1,
                        data = new
                        {
                            User.user_name,
                            //User.email,
                            User.phone,
                            User.id,
                            device_id = userModel.device_id ?? "",
                            img = User.img,
                            User.lang,
                            //User.fk_city,
                            //city_id = city.Id,
                            //city_name = city.name
                        },
                        msg = creatMessage(User.lang, "successfully registered", "successfully registered"),
                        status = false,
                        notification = User.notification,
                        User.code
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



        [HttpPost]  //OK
        public ActionResult ConfirmCodeRegister(userModel userModel)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    if (userModel.code == "")
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(userModel.lang, " Please enter your verification code", "Please enter your verification code")
                        }, JsonRequestBehavior.AllowGet);
                    }

                    var codeuser = db.Client.Find(userModel.user_id);
                    if (codeuser != null)
                    {
                        if (codeuser.code == userModel.code)
                        {
                            codeuser.active_code = true;
                            db.SaveChanges();
                            return Json(new
                            {
                                key = 1,
                                msg = creatMessage(codeuser.lang, "Logged in successfully", "Logged in successfully")
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { key = 0, msg = creatMessage(codeuser.lang, "Please enter the code correctly", "Please enter the code correctly") }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { key = 0, msg = creatMessage(codeuser.lang, "Sorry this phone is not registered", "Sorry this phone is not registered") }, JsonRequestBehavior.AllowGet);

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

        [HttpPost]  //OK
        public ActionResult resend_code(userModel userModel)
        {
            try
            {

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    if (userModel.user_id == 0)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(userModel.lang, "Please verify the data", "Please verify the data")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var codeuser = (from st in db.Client where st.id == userModel.user_id select st).SingleOrDefault();

                    if (codeuser != null)
                    {
                        Random rnd = new Random();
                        int code = 1234; //rnd.Next(1, 1300);
                        string s = SendMessage(code.ToString(), codeuser.phone);
                        codeuser.code = code.ToString();
                        db.SaveChanges();
                        return Json(new
                        {
                            key = 1,
                            code = new { code = code, user_id = codeuser.id, phone = codeuser.phone },
                            msg = creatMessage(codeuser.lang, "code sent", "Code sent"),
                            // status = "active",
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { key = 0, msg = creatMessage(userModel.lang, "Sorry phone number is not registered", "Sorry phone number is not registered") }, JsonRequestBehavior.AllowGet);
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


     //     if (userModel.password != checkuser.password)
            

        [HttpPost] //OK
        public ActionResult sign_in(userModel userModel)
        {


            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var checkuser = (from st in db.Client where st.phone == userModel.phone && st.password == userModel.password select st).FirstOrDefault();
                var checkprovider = (from st in db.Provider where st.phone == userModel.phone && st.password == userModel.password select st).FirstOrDefault();

                #region validation

                if (checkuser != null)
                {
                    if (checkuser.active == false && checkuser.active_code == true)
                    {
                        return Json(new
                        {
                            key = 0,
                            data = new { },
                            status = "blocked",
                            msg = creatMessage(checkuser.lang, "This account is closed by the admin", "This account is closed by the addict")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (checkuser.active_code == false)
                    {
                        return Json(new
                        {
                            key = 1,
                            data = new
                            {
                                checkuser.id,
                                checkuser.code,
                                active_code = checkuser.active_code,
                                typeuser = 1,

                            },
                            status = false,
                            msg = creatMessage(userModel.lang, "This account is not active", "This account is not active")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                    // add new device
                    checkuser.lang = userModel.lang;
                    db.SaveChanges();
                    var check_device_id = (from st in db.Device_Id where st.device_id == userModel.device_id && st.fk_user == checkuser.id select st).Any();

                    // get city data
                    var city = db.City.FirstOrDefault(c => c.Id == checkuser.fk_cityID);

                    if (!check_device_id)
                    {
                        Device_Id d = new Device_Id()
                        {
                            device_id = userModel.device_id,
                            fk_user = checkuser.id
                        };
                        db.Device_Id.Add(d);
                        db.SaveChanges();

                    }
                    return Json(new
                    {
                        key = 1,
                        data = new
                        {
                            checkuser.user_name,
                            //checkuser.email,
                            checkuser.phone,
                            checkuser.id,
                            device_id = userModel.device_id ?? "",
                            img = checkuser.img,
                            checkuser.lang,
                            city.Id,
                            city.name,
                            typeuser = 1,
                            wallet = Math.Round(checkuser.wallet, 2),
                            points = checkuser.Points
                        },
                        notification = checkuser.notification,
                        status = true,
                        msg = creatMessage(checkuser.lang, "Logged in successfully", "Logged in successfully")
                    }, JsonRequestBehavior.AllowGet);



                }

                if (checkprovider != null)
                {
                    if (checkprovider.active == false && checkprovider.active_code == true)
                    {
                        return Json(new
                        {
                            key = 0,
                            data = new { },
                            status = "blocked",
                            msg = creatMessage(checkuser.lang, "This account is closed by the admin", "This account is closed by the addict")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (checkprovider.active_code == false)
                    {
                        return Json(new
                        {
                            key = 0,
                            data = new
                            {
                                checkprovider.id,
                                checkprovider.code,
                                active_code = checkprovider.active_code,
                                typeuser = 2
                            },

                            msg = creatMessage(userModel.lang, "This account is not active", "This account is not active")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                    // add new device
                    checkprovider.lang = userModel.lang;
                    db.SaveChanges();
                    var check_device_id = (from st in db.Device_Id where st.device_id == userModel.device_id && st.fk_user == checkprovider.id select st).Any();

                    // get city data
                    var city = db.City.FirstOrDefault(c => c.Id == checkprovider.fk_city);

                    if (!check_device_id)
                    {
                        Device_Id d = new Device_Id()
                        {
                            device_id = userModel.device_id,
                            fk_user = checkprovider.id
                        };
                        db.Device_Id.Add(d);
                        db.SaveChanges();
                    }
                    return Json(new
                    {
                        key = 1,
                        data = new
                        {
                            checkprovider.user_name,
                            //checkuser.email,
                            checkprovider.phone,
                            checkprovider.id,
                            device_id = userModel.device_id ?? "",
                            img = checkprovider.img,
                            checkprovider.national_id_img,
                            checkprovider.drive_licence_img,
                            checkprovider.lang,
                            city.Id,
                            city.name,
                            typeuser = 2
                        },
                        status = true,
                        notification = checkprovider.notification,
                        msg = creatMessage(checkprovider.lang, "Logged in successfully", "Logged in successfully")
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        key = 0,
                        msg = creatMessage(userModel.lang, "Please verify the phone number and the password", "Please verify the phone number and the password")
                    }, JsonRequestBehavior.AllowGet);
                }
            }

        }

        [HttpPost]//OK
        public ActionResult GetDataOfUser(userModel userModel)
        {

            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    var User = (from st in db.Client where st.id == userModel.user_id select st).SingleOrDefault();

                    return Json(new
                    {
                        key = 1,
                        data = new
                        {
                            User.user_name,
                            //User.email,
                            User.phone,
                            User.id,
                            device_id = "",
                            img = User.img,
                            User.lang,
                            wallet = Math.Round(User.wallet, 2),
                            points = User.Points

                        },
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

        [HttpPost]
        public ActionResult Forget_password(userModel userModel)
        {

            try
            {

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    if (userModel.phone == "")
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(userModel.lang, "من فضلك ادخل رقم الهاتف", "Plaese enter your phone number")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var codeuser = (from st in db.Client where st.phone == userModel.phone select st).SingleOrDefault();
                    if (codeuser == null)
                    {
                        return Json(new { key = 0, msg = creatMessage(userModel.lang, "Sorry phone number is not registered", "Sorry phone number is not registered") }, JsonRequestBehavior.AllowGet);

                    }
                    if (codeuser.active == false)
                    {
                        return Json(new
                        {
                            key = 0,
                            data = new { },
                            status = "blocked",
                            msg = creatMessage(codeuser.lang, "This account is closed by the admin", "This account is closed by the addict")
                        }, JsonRequestBehavior.AllowGet);
                    }


                    else
                    {
                        string code = GetFormNumber();
                        string s = SendMessage(code, userModel.phone);
                        codeuser.code = code.ToString();
                        db.SaveChanges();
                        return Json(new
                        {
                            key = 1,
                            code = new { code = code, id = codeuser.id },
                            msg = creatMessage(codeuser.lang, " Code sent", "Code sent")
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
        public ActionResult reset_password(userModel userModel)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    if (userModel.code == "")
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(userModel.lang, "Please enter your verification code", "Please enter your verification code")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (userModel.current_pass == "")
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(userModel.lang, "Please enter your new password ", "Please enter your new password")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    try
                    {
                        var codeuser = (from st in db.Client where st.id == userModel.user_id select st).SingleOrDefault();
                        if (codeuser != null)
                        {
                            if (codeuser.code != userModel.code)
                            {
                                return Json(new { key = 0, msg = creatMessage(codeuser.lang, " Invalid verification code", "  Invalid verification code") }, JsonRequestBehavior.AllowGet);
                            }
                            codeuser.password = userModel.current_pass;
                            db.SaveChanges();


                            return Json(new { key = 1, msg = creatMessage(codeuser.lang, "Password changed successfully", "Password changed successfully") }, JsonRequestBehavior.AllowGet);
                        }


                        else
                        {
                            return Json(new { key = 0, msg = creatMessage(userModel.lang, "Something went wrong", "Something went wrong") }, JsonRequestBehavior.AllowGet);

                        }
                    }
                    catch (Exception)
                    {
                        return Json(new { key = 0, msg = creatMessage(userModel.lang, "Something went wrong", "Something went wrong") }, JsonRequestBehavior.AllowGet);
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

        [HttpPost] //OK
        public ActionResult ChangePassword(userModel userModel)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {


                    if (userModel.user_id == 0)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(userModel.lang, "Sorry this User was not found  ", "Sorry this User was not found")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (userModel.old_password == null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(userModel.lang, "Please enter your old password ", "Please enter your old password")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (userModel.new_password == null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(userModel.lang, "Please enter your new password  ", "Please enter your new password")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var codeuser = (from st in db.Client where st.id == userModel.user_id select st).SingleOrDefault();
                    if (codeuser != null)
                    {
                        if (codeuser.password != userModel.old_password && userModel.old_password != "")
                        {
                            return Json(new
                            {
                                key = 0,
                                msg = creatMessage(codeuser.lang, "Please enter the old password correctly", "Please enter the old password correctly")
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (userModel.old_password == "")
                            {
                                return Json(new
                                {
                                    key = 0,
                                    msg = creatMessage(codeuser.lang, "Please enter your old password ", "Please enter your old password")
                                }, JsonRequestBehavior.AllowGet);
                            }
                            if (userModel.new_password == "")
                            {
                                return Json(new
                                {
                                    key = 0,
                                    msg = creatMessage(codeuser.lang, " Please enter your new password ", "Please enter your new password")
                                }, JsonRequestBehavior.AllowGet);
                            }
                            codeuser.password = userModel.new_password;
                            db.SaveChanges();
                        }

                        return Json(new { key = 1, msg = creatMessage(codeuser.lang, "Password changed successfully", "Password changed successfully") }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { key = 0, msg = creatMessage(userModel.lang, "Something went wrong", "Something went wrong") }, JsonRequestBehavior.AllowGet);
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

        [HttpPost] //OK
        public ActionResult UpdateUserData(Client User, userModel userModel, int fk_city = 0)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    if (User.id == 0)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(userModel.lang, "Sorry this User was not found ", "Sorry this User was not found")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var Userdata = (from st in db.Client where st.id == User.id select st).SingleOrDefault();
                    if (Userdata != null)
                    {
                        if (User.user_name != null)
                        {
                            Userdata.user_name = User.user_name;
                        }
                        if (fk_city != 0)
                        {
                            Userdata.fk_cityID = fk_city;
                        }

                        if (User.phone != null)
                        {
                            var chekphone = (from st in db.Client where (st.phone == User.phone && st.id != User.id) select st).FirstOrDefault();
                            var chekphone_provider = (from st in db.Provider where (st.phone == User.phone) select st).FirstOrDefault();

                            if (chekphone != null || chekphone_provider != null)
                            {
                                return Json(new
                                {
                                    key = 0,
                                    msg = creatMessage(Userdata.lang, "Sorry this phone number is already registered", "Sorry this phone number is already registered")
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                Userdata.phone = User.phone;
                            }
                        }
                       
                        // add new imgae
                        var file = Request.Files["img"];
                        if (file != null)
                        {
                            Random rnd = new Random();
                            int count = rnd.Next(1, 1300);
                            string extension = DateTime.Now.Ticks.ToString() + count + ".png";

                            string path = Path.Combine(Server.MapPath("~/Content/Img/User/"), extension);
                            file.SaveAs(path);

                            Userdata.img = BaisUrlHoste + "Content/Img/User/" + extension;
                        }

                        db.SaveChanges();

                    }

                    // get city data
                    var city = db.City.FirstOrDefault(c => c.Id == Userdata.fk_cityID);

                    return Json(new
                    {
                        key = 1,
                        data = new
                        {
                            Userdata.user_name,
                            //Userdata.email,
                            Userdata.phone,
                            Userdata.id,
                            device_id = "",
                            img = Userdata.img,
                            Userdata.lang,
                            city.Id,
                            city.name,
                            typeuser = 1

                        },
                        msg = creatMessage(Userdata.lang, "Data modified successfully", "Data modified successfully")
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
        [HttpPost]
        public ActionResult Logout(userModel userModel)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    var info = (from st in db.Device_Id
                                where st.device_id == userModel.device_id && st.fk_user == userModel.user_id
                                select st
                                ).SingleOrDefault();
                    if (info != null)
                    {
                        db.Device_Id.Remove(info);
                        db.SaveChanges();
                        return Json(new
                        {
                            key = 1,
                            msg = creatMessage(userModel.lang, "Logged out successfully", "Logged out successfully"),
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            key = 0
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
        public ActionResult GetNotify(int user_id, string lang = "ar")
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                var notify = db.Notify.Where(x => x.fk_user == user_id && x.type == 1).Select(x => new
                {
                    x.text,
                    x.order_id,
                    x.order_type,
                    x.Id

                }).ToList().OrderByDescending(xx => xx.Id);

                var notify_not_shown = db.Notify.Where(x => x.fk_user == user_id && x.fk_user_show == false).ToList();
                if (notify_not_shown.Count != 0)
                {
                    notify_not_shown.ForEach(a => a.fk_user_show = true);
                    db.SaveChanges();

                }

                return Json(new
                {
                    key = 1,
                    notify = notify
                }, JsonRequestBehavior.AllowGet);


            }



        }

        [HttpPost]
        public ActionResult SwitchNotify(int user_id)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var client = context.Client.FirstOrDefault(c => c.id == user_id);
                if (client.notification == true)
                {
                    client.notification = false;
                    context.SaveChanges();
                    return Json(new { key = 1, notification = client.notification, msg = "Notifications cloded successfully " }, JsonRequestBehavior.AllowGet);
                }

                client.notification = true;
                context.SaveChanges();
                return Json(new { key = 1, notification = client.notification, msg = "Notifications opened successfully " }, JsonRequestBehavior.AllowGet);

            }

        }




        #region logic

        [HttpPost] //oK
        public ActionResult GetCategory(int user_id, string lang = "ar")
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var category = (from st in db.Category
                                where st.is_active == true
                                select new
                                {
                                    st.Id,
                                    name = lang == "ar" ? st.name : st.name_en,
                                    SubCategory = (from sts in db.SubCategory
                                                   where sts.is_active == true && sts.fk_cat == st.Id
                                                   select new
                                                   {
                                                       sts.Id,
                                                       name = lang == "ar" ? sts.name : sts.name_en,
                                                       sts.img,
                                                       product_count = db.Product.Where(p => p.fk_categoryID == sts.Id).Count()
                                                   })
                                }).ToList();

                var sliders = db.Slider.Where(x => x.IsActive == true && x.IsDeleted == false && x.type == 1).ToList();
                int cart_count = db.Cart.Where(x => x.fk_userID == user_id).Select(x => x.qty).DefaultIfEmpty().Sum();

                return Json(new
                {
                    key = 1,
                    category = category,
                    sliders = sliders,
                    cart_count
                }, JsonRequestBehavior.AllowGet);

            }
        }



        [HttpPost] //oK
        public ActionResult GetProduct(int category_id, int user_id, int page_number = 1)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var total_count = 0;
                    var products = (from st in db.Product
                                    where st.is_active == true && st.fk_categoryID == category_id
                                    select new
                                    {
                                        st.Id,
                                        st.name,
                                        st.price,
                                        description = st.description,
                                        img = st.img,
                                        cat_id = st.fk_categoryID,
                                        cat_name = db.SubCategory.Where(x => x.fk_cat == st.fk_categoryID).Select(x => x.name).FirstOrDefault(),
                                        product_qty = st.all_qty,
                                        count = db.Cart.Where(x => x.fk_userID == user_id && x.fk_productID == st.Id).Select(c => c.qty).FirstOrDefault(),
                                        favourite = db.Favorites.Any(f => f.FkUserId == user_id && f.FkProductId == st.Id)
                                    }).ToList();
                    //}).ToList();

                    var last_index = Math.Ceiling((double)products.Count() / 10);

                    var pagedProducts = products.AsQueryable().OrderBy(o => o.Id).ToPagedList(page_number, 10);

                    int cart_count = db.Cart.Where(x => x.fk_userID == user_id).Select(x => x.qty).DefaultIfEmpty().Sum();
                    int notify_count = db.Notify.Where(x => x.fk_user == user_id && x.fk_user_show == false).Count();

                    total_count += products.Select(p => p.count).Sum();

                    return Json(new
                    {
                        key = 1,
                        product = pagedProducts,
                        total_count,
                        cart_count,
                        notify_count,
                        last_index,
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
        [HttpPost] //oK
        public ActionResult GetProductDetail(int user_id, int product_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    var productItem = db.Product.Where(p => p.Id == product_id).Select(product => new
                    {

                        product.Id,
                        product.name,
                        product.price,
                        description = product.description,
                        specification = product.specification,
                        img = product.img,
                        favourite = db.Favorites.Any(f => f.FkUserId == user_id && f.FkProductId == product_id)
                    }
                    ).FirstOrDefault();



                    return Json(new
                    {
                        key = 1,
                        product = productItem,
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


        [HttpPost] //oK
        public ActionResult GetCartDetails(int user_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var cart = db.Cart.Where(x => x.fk_userID == user_id).Select(x => new
                    {
                        id = x.fk_productID,
                        qty = x.qty,
                        img = x.fk_product.img,
                        price = x.fk_product.price,
                        product_name = x.fk_product.name,
                        description = x.fk_product.description,
                        favourite = db.Favorites.Any(f => f.FkUserId == user_id && f.FkProductId == x.fk_productID)
                    }).ToList();


                    return Json(new
                    {
                        key = 1,
                        data = cart
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

        [HttpPost]//oK
        public ActionResult Search(string searchString)
        {

            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {

                    var products = context.Product.Where(p => p.is_active == true && ((p.name == searchString) || p.name.StartsWith(searchString) || p.name.Contains(searchString))).Select(p => new
                    {
                        p.Id,
                        p.name,
                        p.price,
                        img = p.img,
                        cat_id = p.fk_categoryID,
                        product_qty = p.all_qty,
                    }).ToList();


                    return Json(new
                    {
                        key = 1,
                        products = products,

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


        [HttpPost]//oK
        public ActionResult AddOrder(int user_id, double deleviry, double net_total, string time,
            double total, int payment_type, string address = "", string lat = "", string lng = "", string lang = "ar", int copon_id = 0)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Setting setting = db.Setting.FirstOrDefault();

                    var client = db.Client.FirstOrDefault(u => u.id == user_id);
                    Order order = new Order()
                    {
                        fk_userID = user_id,
                        fk_providerID = null,
                        address = address,
                        is_paied = 1,
                        delivary_time = time,
                        lat = lat,
                        lng = lng,
                        date = TimeNow().ToString("dd/MM/yyyy"),
                        delivary = deleviry,
                        net_total = net_total,
                        total = total,
                        type = (int)Order_type.New
                    };

                    if (payment_type == (int)Payment_type.Wallet)
                    {

                        if (client.wallet < total)
                        {
                            return Json(new { key = 0, msg = creatMessage(lang, "Your wallet balance not enough to complete the payment process", "Your wallet balance not enough to complete the payment process") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            client.wallet -= total;
                            order.is_paied = 2;
                            client.Points += setting.PointsPerOrder;
                            db.SaveChanges();
                        }
                    }

                    var cart = db.Cart.Where(x => x.fk_userID == user_id).ToList();
                    if (cart.Count == 0)
                    {
                        return Json(new { key = 0, msg = "Please add products to the cart" },
                            JsonRequestBehavior.AllowGet);
                    }

                    // check order's products qty
                    foreach (var item in cart)
                    {

                        var check_product = db.Product.FirstOrDefault(p => p.Id == item.fk_productID);

                        if (check_product.all_qty == 0)
                        {
                            return Json(new { key = 0, msg = "product " + check_product.name + " no longer exists" }, JsonRequestBehavior.AllowGet);
                        }
                        if (item.qty > check_product.all_qty)
                        {
                            return Json(new { key = 0, msg = "Maximum product" + check_product.name + "is" + check_product.all_qty }, JsonRequestBehavior.AllowGet);
                        }

                    }



                    db.Order.Add(order);

                    db.SaveChanges();

                    if (copon_id != 0)
                    {
                        var CheckCopon = db.Copons.SingleOrDefault(x => x.id == copon_id);
                        if (CheckCopon != null)
                        {
                            CoponUsed coponUsed = new CoponUsed()
                            {

                                fk_user = user_id,
                                fk_copon = CheckCopon.id,
                                fk_order = order.Id,

                            };
                            db.CoponUseds.Add(coponUsed);
                            db.SaveChanges();


                        }
                    }


                    foreach (var item in cart)
                    {

                        OrderInfo oinfo = new OrderInfo()
                        {
                            fk_order = order,
                            fk_product = item.fk_productID,
                            qty = item.qty,
                            img = item.fk_product.img,
                            name = item.fk_product.name,
                            price = item.fk_product.price
                        };

                        var check_product = db.Product.FirstOrDefault(p => p.Id == oinfo.fk_product);

                        if (check_product.all_qty == 0)
                        {
                            return Json(new { key = 0, msg = "product " + check_product.name + " no longer exists" }, JsonRequestBehavior.AllowGet);
                        }
                        if (oinfo.qty > check_product.all_qty)
                        {
                            return Json(new { key = 0, msg = "Maximum product" + check_product.name + " is " + check_product.all_qty }, JsonRequestBehavior.AllowGet);
                        }
                        check_product.all_qty = check_product.all_qty - oinfo.qty;

                        db.OrderInfo.Add(oinfo);
                    }

                    client.Points += setting.PointsPerOrder;
                    db.SaveChanges();


                    if (cart.Count != 0)
                    {
                        db.Cart.RemoveRange(cart);
                        db.SaveChanges();
                    }





                    //// هنعمل اشعار لكل المندوبين
                    var Providers = db.Provider.Where(x => x.active == true).ToList();
                    foreach (var item in Providers)
                    {
                        Notify notify = new Notify();
                        notify.date = DateTime.Now;
                        notify.fk_provider = item.id;
                        notify.fk_user = user_id;
                        notify.order_id = order.Id;
                        notify.order_type = order.type;
                        notify.text = "There is a new request in the list of requests, please see  ";
                        notify.type = 2;

                        db.Notify.Add(notify);
                        db.SaveChanges();
                        SendPushNotification(item.id, order.Id, order.type, "There is a new request in the list of requests, please see", 2);
                    }
                    return Json(new
                    {
                        key = 1,
                        order_id = order.Id,
                        msg = creatMessage("ar", "The request has been successfully added", "added successfully")
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

        [HttpPost]
        public ActionResult UseCopon(int user_id, string copon, double total, string lang = "ar")
        {
            try
            {

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var currentdate = TimeNow();
                    var CheckCopon = db.Copons.SingleOrDefault(x => x.copon_code == copon && x.isActive == true);
                    if (CheckCopon != null)
                    {
                        if (CheckCopon.expirdate.Date < currentdate.Date)
                        {
                            return Json(new { key = 0, msg = creatMessage(lang, "Sorry, the validity of the coupon has expired", "Sorry, the validity of the coupon has expired") });
                        }
                        if (CheckCopon.count <= CheckCopon.count_used)
                        {
                            return Json(new { key = 0, msg = creatMessage(lang, "Sorry, the maximum use of the coupon has been exceeded", "Sorry, the maximum use of the coupon has been exceeded") });
                        }
                        var CoponUsedForUser = db.CoponUseds.FirstOrDefault(x => x.fk_copon == CheckCopon.id && x.fk_user == user_id);
                        if (CoponUsedForUser != null)
                        {
                            return Json(new { key = 0, msg = creatMessage(lang, "The copon has already been used", "The copon has already been used") });
                        }
                        var value = (CheckCopon.discount / 100) * total;
                        if (value > CheckCopon.limt_discount)
                        {
                            value = CheckCopon.limt_discount;
                            CheckCopon.count_used = CheckCopon.count_used + 1;
                            db.SaveChanges();
                            var LastTOTAL = total - value;

                            return Json(new { key = 1, copon_id = CheckCopon.id, lasttotal = Math.Round(LastTOTAL, 2), msg = creatMessage(lang, "The maximum discount value is " + CheckCopon.limt_discount + "ريال", "The maximum discount value is" + CheckCopon.limt_discount + "R.S") });
                        }
                        else
                        {
                            CheckCopon.count_used = CheckCopon.count_used + 1;
                            db.SaveChanges();
                            var LastTOTAL = total - value;

                            return Json(new { key = 1, copon_id = CheckCopon.id, discount = Math.Round(value, 2), lasttotal = Math.Round(LastTOTAL, 2), msg = creatMessage(lang, " Successfully charged", "Successfully charged") });
                        }

                    }
                    else
                    {
                        return Json(new { key = 0, msg = creatMessage(lang, "Please make sure of the coupon ", "Please make sure of the coupon") });
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
        [HttpPost]
        public ActionResult ConvertPointsToWalletCash(int user_id, int points, string lang = "ar")
        {
            try
            {

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var client = db.Client.FirstOrDefault(c => c.id == user_id);
                    var setting = db.Setting.FirstOrDefault();
                    if (points < setting.PointsPerRiyal)
                    {
                        return Json(new { key = 0, msg = creatMessage(lang, "The minimum points to transfer are  " + setting.PointsPerRiyal, "The minimum points to transfer are " + setting.PointsPerRiyal) });
                    }
                    else
                    {
                        var riyalWalletPoints = points / setting.PointsPerRiyal;
                        var remaining = points % setting.PointsPerRiyal;
                        client.wallet += riyalWalletPoints;
                        client.Points = remaining;
                        db.SaveChanges();
                        return Json(new { key = 1, points = client.Points, wallet = Math.Round(client.wallet, 2), msg = creatMessage(lang, "The points transferred successfully ", "The points transferred successfully") });
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

        public async Task<ActionResult> AddOrRemoveFavorite(FavoriteViewModel model)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Favorite favorite = await db.Favorites.FirstOrDefaultAsync(c => c.FkProductId == model.fk_product && c.FkUserId == model.user_id);
                    if (favorite == null)
                    {
                        db.Favorites.Add(new Favorite() { FkProductId = model.fk_product, FkUserId = model.user_id });
                        await db.SaveChangesAsync();
                        return Json(new
                        {
                            key = 1,
                            msg = creatMessage(model.lang, "The product has been successfully added to your favorites ", "The product has been successfully added to your favorites ")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db.Favorites.Remove(favorite);
                        await db.SaveChangesAsync();
                        return Json(new
                        {
                            key = 1,
                            msg = creatMessage(model.lang, " The product has been removed from your favorites ", "The product has been removed from your favorites ")
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

        public async Task<ActionResult> GetFavorite(FavoriteData model)
        {

            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var favoriteProductViewModels = await (from favorite in db.Favorites
                                                           join product in db.Product on favorite.FkProductId equals product.Id
                                                           where favorite.FkUserId == model.user_id
                                                           select new FavoriteProductViewModel()
                                                           {
                                                               Id = product.Id,
                                                               Name = product.name,
                                                               Img = product.img,
                                                               Price = product.price
                                                           }).ToListAsync();

                    return Json(new { key = 1, favorites = favoriteProductViewModels }, JsonRequestBehavior.AllowGet);

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

        [HttpPost]//oK
        public ActionResult GetCartOrderDetails(int user_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Setting setting = db.Setting.FirstOrDefault();
                    var cart_sum = db.Cart.Where(x => x.fk_userID == user_id).ToList().Select(x => x.qty * x.fk_product.price).Sum();

                    double delivery = setting.delivery;

                    var addressList = db.AddressUser.Where(x => x.fk_userID == user_id).Select(x => new
                    {
                        x.address,
                        x.lat,
                        x.lng,
                        x.title
                    }).ToList();


                    return Json(new
                    {
                        key = 1,
                        cart_sum,
                        vat = (setting.vat / 100) * cart_sum,
                        persent = setting.vat.ToString() + "%",
                        delivery,
                        total = cart_sum + delivery + ((setting.vat / 100) * cart_sum),
                        address = addressList
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

        [HttpPost]
        public ActionResult ChangeOrderStutes(int fk_order, int type)
        {
            //type // 2- accepted 3-refused 4-finished  Order_type.Client_recipt
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var order = db.Order.FirstOrDefault(x => x.Id == fk_order);


                    if (order != null)
                    {


                        order.type = type;

                        db.SaveChanges();
                    }



                    return Json(new
                    {
                        key = 1,
                        msg = creatMessage("ar", "The request has been successfully approved ", "The request has been successfully approved ")
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

        [HttpPost]
        public ActionResult GetProductList(int user_id, int page_number = 1)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var total_count = 0;
                    var products = (from st in db.Product
                                    where st.is_active == true
                                    select new
                                    {
                                        st.Id,
                                        st.name,
                                        st.price,
                                        img = st.img,
                                        cat_id = st.fk_categoryID,
                                        cat_name = "",
                                        product_qty = st.all_qty,
                                        count = db.Cart.Where(x => x.fk_userID == user_id).Select(c => c.qty).FirstOrDefault()
                                    }).ToList();

                    var last_index = Math.Ceiling((double)products.Count() / 8);

                    var pagedProducts = products.AsQueryable().OrderBy(o => o.Id).ToPagedList(page_number, 8);

                    int cart_count = db.Cart.Where(x => x.fk_userID == user_id).Select(x => x.qty).DefaultIfEmpty().Sum();
                    int notify_count = db.Notify.Where(x => x.fk_user == user_id && x.fk_user_show == false).Count();

                    total_count += products.Select(p => p.count).Sum();

                    return Json(new
                    {
                        key = 1,
                        product = pagedProducts,
                        total_count,
                        cart_count,
                        notify_count,
                        last_index
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






        [HttpPost]
        public ActionResult UpdateOrder(int user_id, int order_id, string products, double total, string address = "", string lat = "", string lng = "", string lang = "ar")
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var order = db.Order.FirstOrDefault(x => x.Id == order_id && x.fk_userID == user_id);
                    if (order == null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(lang, "that order not found", "that order not found")
                        });
                    }
                    if (order.type == 3)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(lang, "This order was delivered", "This order was delivered")
                        });

                    }

                    var old_order_products = order.OrderInfo;// return old qty

                    foreach (var orderInfo in old_order_products)
                    {
                        var check_product = db.Product.FirstOrDefault(p => p.Id == orderInfo.fk_product);

                        check_product.all_qty = check_product.all_qty + orderInfo.qty;
                        db.SaveChanges();
                    }

                    db.OrderInfo.RemoveRange(old_order_products);
                    db.SaveChanges();
                    dynamic all_products = JsonConvert.DeserializeObject(products);
                    foreach (var item in all_products)
                    {
                        int productid = Convert.ToInt32(item.id);
                        int productqty = Convert.ToInt32(item.qty);
                        var product = db.Product.Find(productid);

                        if (product.all_qty == 0)
                        {
                            return Json(new { key = 0, msg = "product " + product.name + " no longer exists" }, JsonRequestBehavior.AllowGet);
                        }
                        if (productqty > product.all_qty)
                        {
                            return Json(new { key = 0, msg = "the maximum product " + product.name + " is " + product.all_qty }, JsonRequestBehavior.AllowGet);
                        }

                        product.all_qty = product.all_qty - productqty;
                        //db.SaveChanges();

                        OrderInfo oinfo = new OrderInfo()
                        {
                            fk_orderID = order.Id,
                            fk_product = product.Id,
                            qty = productqty,
                            img = product.img,
                            name = product.name,
                            price = product.price
                        };
                        db.OrderInfo.Add(oinfo);
                    }
                    order.total = total;
                    order.net_total = total + order.delivary;
                    order.address = address;
                    order.lat = lat;
                    order.lng = lng;

                    db.SaveChanges();

                    if (order.type == 1)
                    {
                        // هنعمل اشعار لكل المندوبين
                        var Providers = db.Provider.Where(x => x.active == true).ToList();
                        foreach (var item in Providers)
                        {
                            Notify notify = new Notify();
                            notify.date = DateTime.Now;
                            notify.fk_provider = item.id;
                            notify.order_id = order.Id;
                            notify.order_type = order.type;
                            notify.text = "The order number " + order.Id + " has been modified successfully";
                            notify.type = 2;

                            db.Notify.Add(notify);
                            db.SaveChanges();
                            SendPushNotification(item.id, order.Id, order.type, " Request number has been modified " + order.Id, 2);
                        }
                        return Json(new
                        {
                            key = 1,
                            order_id = order.Id,
                            msg = creatMessage("ar", "updated successfully", "updated successfully")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // هنعمل اشعار للمندوب اللي موافق على الطلب ده 
                        var Provider = db.Provider.FirstOrDefault(x => x.active == true && x.id == order.fk_providerID);

                        Notify notify = new Notify();
                        notify.date = DateTime.Now;
                        notify.fk_provider = Provider.id;
                        notify.order_id = order.Id;
                        notify.order_type = order.type;
                        notify.text = " Request number has been modified " + order.Id + " successfuly";
                        notify.type = 2;

                        db.Notify.Add(notify);
                        db.SaveChanges();
                        SendPushNotification(Provider.id, order.Id, order.type, " Request number has been modified " + order.Id, 2);

                        return Json(new
                        {
                            key = 1,
                            order_id = order.Id,
                            msg = creatMessage("ar", "updated successfully", "updated successfully")
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
        public ActionResult DeleteOrder(int user_id, int order_id, string lang = "ar")
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    var order = db.Order.FirstOrDefault(x => x.Id == order_id && x.fk_userID == user_id);
                    if (order == null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(lang, "that order not found", "that order not found")
                        });
                    }
                    if (order.type != 1)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(lang, " that order accepted ", "that order accepted")
                        });

                    }

                    var old_order_products = order.OrderInfo;

                    foreach (var orderInfo in old_order_products)
                    {
                        var check_product = db.Product.FirstOrDefault(p => p.Id == orderInfo.fk_product);
                        check_product.all_qty = check_product.all_qty + orderInfo.qty;
                        db.SaveChanges();
                    }



                    db.Order.Remove(order);
                    db.SaveChanges();

                    try
                    {
                        var notify = db.Notify.Where(x => x.order_id == order_id).ToList();
                        if (notify.Count != 0)
                        {
                            db.Notify.RemoveRange(notify);
                            db.SaveChanges();

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


                    return Json(new
                    {
                        key = 1,
                        order_id = order.Id,
                        msg = creatMessage("ar", " order deleted successfully", "order deleted successfully")
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


        [HttpPost]
        public ActionResult GetAllOrder(int user_id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var Orders = db.Order.Where(x => x.fk_userID == user_id).ToList();

                var NewOrder = Orders.Where(x => x.type == 1).Select(x => new
                {
                    NumberOrder = x.Id,
                    Date = x.date,
                }).ToList();
                var CurrentOrder = Orders.Where(x => x.type == 2).Select(x => new
                {
                    NumberOrder = x.Id,
                    Date = x.date,
                }).ToList();
                var OldOrder = Orders.Where(x => x.type == 3).Select(x => new
                {
                    NumberOrder = x.Id,
                    Date = x.date,
                }).ToList();

                return Json(new { key = 1, NewOrder = NewOrder, CurrentOrder = CurrentOrder, OldOrder = OldOrder }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]//OK
        public ActionResult GetNewOrder(int user_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var orders = db.OrderInfo.Where(o => o.fk_order.fk_userID == user_id && o.fk_order.type < (int)Order_type.Client_recipt).AsEnumerable().DistinctBy(oi => oi.fk_orderID)
                        .Select(order => new
                        {
                            order.fk_orderID,
                            order.name,
                            order.img,
                            order.qty,
                            order.price,
                            AllQty = db.OrderInfo.Where(oi => oi.fk_order.fk_userID == user_id && oi.fk_order.type < (int)Order_type.Client_recipt).Select(oi => oi.qty).AsEnumerable().Sum(),
                            order.fk_order.type,
                            dateOrder = order.fk_order.date_time.Day + "-" + order.fk_order.date_time.Month + "-" + order.fk_order.date_time.Year,
                            timeOrder = order.fk_order.date_time.Hour + ":" + order.fk_order.date_time.Minute,
                            date = order.fk_order.date,
                            total = order.fk_order.total + order.fk_order.delivary
                        }).ToList().OrderByDescending(o => o.fk_orderID);


                    return Json(new
                    {
                        key = 1,
                        orders = orders
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

        [HttpPost]//OK
        public ActionResult GetCurrentActiveOrders(int user_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    //var myOrders = db.OrderInfo.Where(o => o.fk_order.fk_userID == user_id && o.fk_order.type == 2).AsEnumerable().DistinctBy(oi => oi.fk_orderID)
                    var myOrders = db.OrderInfo.Where(o => o.fk_order.fk_userID == user_id && (o.fk_order.type >= (int)Order_type.New && o.fk_order.type != (int)Order_type.Deleget_confirm))
                        .Select(order => new
                        {
                            order.fk_orderID,
                            order.name,
                            order.img,
                            order.qty,
                            order.price,
                            order.fk_order.delivary_time,
                            dateOrder = order.fk_order.date_time.Day + "-" + order.fk_order.date_time.Month + "-" + order.fk_order.date_time.Year,
                            timeOrder = order.fk_order.date_time.Hour + ":" + order.fk_order.date_time.Minute,
                            order.fk_order.type,
                            date = order.fk_order.date,
                            total = order.fk_order.net_total
                        }).ToList().OrderByDescending(o => o.fk_orderID);

                    return Json(new
                    {
                        key = 1,
                        orders = myOrders
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

        [HttpPost]//OK
        public ActionResult GetPreviousOrders(int user_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    var myOrders = db.OrderInfo.Where(o => o.fk_order.fk_userID == user_id && o.fk_order.type == (int)Order_type.Client_recipt).AsEnumerable().DistinctBy(oi => oi.fk_orderID)
                        .Select(order => new
                        {
                            order.fk_orderID,
                            order.name,
                            order.img,
                            order.qty,
                            order.price,
                            AllQty = db.OrderInfo.Where(oi => oi.fk_order.fk_userID == user_id && oi.fk_order.type == (int)Order_type.Client_recipt).Select(oi => oi.qty).AsEnumerable().Sum(),
                            order.fk_order.delivary_time,
                            order.fk_order.date_time,
                            order.fk_order.type,
                            dateOrder = order.fk_order.date_time.Day + "-" + order.fk_order.date_time.Month + "-" + order.fk_order.date_time.Year,
                            timeOrder = order.fk_order.date_time.Hour + ":" + order.fk_order.date_time.Minute,
                            date = order.fk_order.date,
                            total = order.fk_order.net_total
                        }).ToList().OrderByDescending(o => o.fk_orderID);

                    return Json(new
                    {
                        key = 1,
                        orders = myOrders
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

        [HttpPost]
        public ActionResult AddUserAddress(AddressUser addressUser)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    AddressUser newAddressUser = new AddressUser()
                    {
                        address = addressUser.address,
                        title = addressUser.title,
                        fk_userID = addressUser.fk_userID,
                        lat = addressUser.lat,
                        lng = addressUser.lng,
                        is_used = false,
                        is_active = true
                    };

                    db.AddressUser.Add(newAddressUser);
                    db.SaveChanges();

                    return Json(new
                    {
                        key = 1,
                        msg = creatMessage("ar", " added successfully", "added successfully")
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
        [HttpPost]
        public ActionResult UpdateUserAddress(AddressUser addressUser)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var ChekAddress = db.AddressUser.Where(x => x.Id == addressUser.Id).SingleOrDefault();
                    if (ChekAddress != null)
                    {
                        ChekAddress.address = addressUser.address;
                        ChekAddress.lat = addressUser.lat;
                        ChekAddress.lng = addressUser.lng;
                        ChekAddress.title = addressUser.title;
                        db.SaveChanges();
                    }


                    return Json(new
                    {
                        key = 1,
                        msg = creatMessage("ar", "added successfully", "added successfully")
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
        [HttpPost]
        public ActionResult GetAddressList(int user_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var addressList = db.AddressUser.Where(x => x.fk_userID == user_id && x.is_active == true).Select(x => new
                    {
                        x.address,
                        x.Id,
                        x.lat,
                        x.lng,
                        x.is_used,
                        x.title
                    }).ToList();
                    return Json(new { key = 1, addressList = addressList }, JsonRequestBehavior.AllowGet);
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
        public ActionResult DeleteAddress(int address_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var addressList = db.AddressUser.Where(x => x.Id == address_id).SingleOrDefault();
                    if (addressList != null)
                    {
                        addressList.is_active = false;
                        db.SaveChanges();
                        return Json(new { key = 1, msg = "Title removed successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { key = 0, addressList = addressList }, JsonRequestBehavior.AllowGet);
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
        public ActionResult AddProductsToCart(int user_id, string products)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    dynamic all_products = JsonConvert.DeserializeObject(products);
                    foreach (var item in all_products)
                    {
                        int productid = Convert.ToInt32(item.id);
                        int productqty = Convert.ToInt32(item.qty);

                        var check_product = db.Product.FirstOrDefault(p => p.Id == productid);

                        if (check_product.all_qty == 0)
                        {
                            return Json(new { key = 0, msg = "product " + check_product.name + " no longer exists" }, JsonRequestBehavior.AllowGet);
                        }
                        if (productqty > check_product.all_qty)
                        {
                            return Json(new { key = 0, msg = "Maximum product" + check_product.name + "is" + check_product.all_qty }, JsonRequestBehavior.AllowGet);
                        }

                        var cartfound = db.Cart.FirstOrDefault(x => x.fk_productID == productid && x.fk_userID == user_id);
                        if (cartfound != null)
                        {
                            if (productqty == 0)
                            {
                                db.Cart.Remove(cartfound);
                            }
                            else
                            {
                                cartfound.qty += productqty;
                            }
                        }
                        else
                        {
                            if (productqty != 0)
                            {

                                Cart cart = new Cart()
                                {
                                    fk_productID = productid,
                                    qty = productqty,
                                    fk_userID = user_id
                                };
                                db.Cart.Add(cart);
                            }
                        }
                    }
                    db.SaveChanges();

                    return Json(new
                    {
                        key = 1,
                        msg = creatMessage("ar", "added successfully", "added successfully")
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
        [HttpPost]
        public ActionResult AddProductsToCartFromOldOrder(int user_id, int order_id)
        {
            //try
            //{
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Order all_products = db.Order.Find(order_id);
                foreach (var item in all_products.OrderInfo)
                {
                    try
                    {

                        int productid = item.fk_product;
                        int productqty = item.qty;

                        var check_product = db.Product.FirstOrDefault(p => p.Id == productid);

                        if (check_product.all_qty == 0)
                        {
                            return Json(new { key = 0, msg = "product " + check_product.name + " no longer exists" }, JsonRequestBehavior.AllowGet);
                        }
                        if (productqty > check_product.all_qty)
                        {
                            return Json(new { key = 0, msg = "Maximum product" + check_product.name + "is" + check_product.all_qty }, JsonRequestBehavior.AllowGet);
                        }

                        var cartfound = db.Cart.FirstOrDefault(x => x.fk_productID == productid && x.fk_userID == user_id);
                        if (cartfound != null)
                        {
                            if (productqty == 0)
                            {
                                db.Cart.Remove(cartfound);
                            }
                            else
                            {
                                cartfound.qty = productqty;
                            }
                        }
                        else
                        {
                            if (productqty != 0)
                            {

                                Cart cart = new Cart()
                                {
                                    fk_productID = productid,
                                    qty = productqty,
                                    fk_userID = user_id
                                };
                                db.Cart.Add(cart);
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
                db.SaveChanges();

                return Json(new
                {
                    key = 1,
                    msg = creatMessage("ar", "added to cart successfully", "added to cart successfully")
                }, JsonRequestBehavior.AllowGet);
            }
            //}
            //catch (Exception ex)
            //{
            //    return Json(new
            //    {
            //        key = 0,
            //        msg = ex.Message
            //    }, JsonRequestBehavior.AllowGet);
            //}
        }

        [HttpPost]
        public ActionResult RateProvider(int user_id, int rate, int order_id, string comment = "", string lang = "ar")
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                int provider_id = db.Order.Where(x => x.Id == order_id).Select(x => x.fk_providerID).FirstOrDefault() ?? -1;
                var rate_found = db.Rate.Any(x => x.fk_providerID == provider_id && x.fk_userID == user_id);
                if (rate_found)
                {
                    return Json(new
                    {
                        key = 0,
                        msg = creatMessage(lang, "rated before", "rated before")
                    }, JsonRequestBehavior.AllowGet);

                }
                Rate rates = new Rate()
                {
                    fk_providerID = provider_id,
                    fk_userID = user_id,
                    rate = rate,
                    comment = comment,

                    date = DateTime.UtcNow.AddHours(3)
                };
                db.Rate.Add(rates);
                db.SaveChanges();
                BaseController.SendPushNotification((int)provider_id, order_id, (int)Order_type.RateDelegert, "You have been rated by the customer", 1);

                double all_rates = db.Rate.Where(x => x.fk_providerID == provider_id).Select(x => x.rate).Average();
                Provider provider = db.Provider.Find(provider_id);
                provider.rate = Math.Round(all_rates, 0);
                db.SaveChanges();
                var OrderRated = db.Order.Where(x => x.Id == order_id).SingleOrDefault();
                if (OrderRated != null)
                {
                    OrderRated.type = (int)Order_type.RateDelegert;
                    db.SaveChanges();
                }
                return Json(new
                {
                    key = 1,
                    msg = creatMessage(lang, "you rate successfully", "you rate successfully")
                }, JsonRequestBehavior.AllowGet);
            }
        }






        [HttpPost]
        public ActionResult EncraseCartProductQty(int user_id, int product_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var cart = db.Cart.FirstOrDefault(x => x.fk_userID == user_id && x.fk_productID == product_id);
                    cart.qty += 1;

                    var check_product = db.Product.FirstOrDefault(p => p.Id == product_id);

                    if (check_product.all_qty == 0)
                    {
                        return Json(new { key = 0, msg = "Product " + check_product.name + "No longer exists" }, JsonRequestBehavior.AllowGet);
                    }
                    if (cart.qty > check_product.all_qty)
                    {
                        return Json(new { key = 0, msg = "Maximum product" + check_product.name + "is" + check_product.all_qty }, JsonRequestBehavior.AllowGet);
                    }

                    db.SaveChanges();

                    return Json(new
                    {
                        key = 1,
                        msg = creatMessage("ar", "quantity encreased successfully", "quantity encreased successfully")
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


        [HttpPost]
        public ActionResult DecraseCartProductQty(int user_id, int product_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var cart = db.Cart.FirstOrDefault(x => x.fk_userID == user_id && x.fk_productID == product_id);
                    cart.qty -= 1;

                    var check_product = db.Product.FirstOrDefault(p => p.Id == product_id);

                    if (check_product.all_qty == 0)
                    {
                        return Json(new { key = 0, msg = "product " + check_product.name + " no longer exists" }, JsonRequestBehavior.AllowGet);
                    }
                    if (cart.qty > check_product.all_qty)
                    {
                        return Json(new { key = 0, msg = "Maximum product" + check_product.name + "is" + check_product.all_qty }, JsonRequestBehavior.AllowGet);
                    }

                    db.SaveChanges();

                    return Json(new
                    {
                        key = 1,
                        msg = creatMessage("ar", "quantity Decreased successfully", "quantity Decreased successfully")
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


        [HttpPost]
        public ActionResult DeleteCartProduct(int user_id, int product_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var cart = db.Cart.FirstOrDefault(x => x.fk_userID == user_id && x.fk_productID == product_id);
                    if (cart == null)
                    {
                        return Json(new
                        {
                            key = 1,
                            msg = creatMessage("ar", "product not found in cart", "product not found in cart")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    db.Cart.Remove(cart);
                    db.SaveChanges();

                    return Json(new
                    {
                        key = 1,
                        msg = creatMessage("ar", "product deleted successfully", "product deleted successfully")
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
        [HttpPost]
        public ActionResult DeleteCartData(int user_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var cart = db.Cart.Where(x => x.fk_userID == user_id);
                    if (!cart.Any())
                    {
                        return Json(new
                        {
                            key = 1,
                            msg = creatMessage("ar", "The cart is empty", "The cart is empty")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    db.Cart.RemoveRange(cart);
                    db.SaveChanges();

                    return Json(new
                    {
                        key = 1,
                        msg = creatMessage("ar", "cart cleared successfully", "cart cleared successfully")
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

        public ActionResult GetOrderInfo(int user_id, int order_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Setting setting = db.Setting.FirstOrDefault();
                    var sum = (db.OrderInfo.Where(oi => oi.fk_orderID == order_id).Select(oi => oi.qty * oi.price)).Sum();
                    var vats = (setting.vat / 100) * sum;
                    var orderFound = db.Order.Where(x => x.Id == order_id && x.fk_userID == user_id).ToList().Select(x => new
                    {
                        x.Id,
                        provider_name = x.fk_provider == null ? "There is no" : x.fk_provider.user_name,
                        provider_img = x.fk_provider == null ? "There is no" : x.fk_provider.img,
                        provider_phone = x.fk_provider == null ? "There is no" : x.fk_provider.phone,
                        delivary_time = x.delivary_time,
                        x.date,
                        stutes = x.type,
                        x.delivary,
                        total = x.net_total,
                        x.address,
                        x.lat,
                        x.lng,
                        AllQty = x.OrderInfo.Select(oi => oi.qty).Sum(),
                        productList = x.OrderInfo.Select(y => new
                        {
                            product_id = y.fk_product,
                            product_img = y.img,
                            product_name = y.name,
                            product_price = y.price,
                            product_qty = y.qty,
                            product_all_qty = db.Product.FirstOrDefault(p => p.Id == y.fk_product).all_qty,

                        }).ToList(),
                        vat = vats,
                        persent = setting.vat.ToString() + "%"
                    }).SingleOrDefault();



                    return Json(new
                    {
                        key = 1,


                        order = orderFound,
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

        public ActionResult GetCities()
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var cities = db.City.Where(x => x.is_active == true).Select(c => new
                    {
                        c.Id,
                        c.name,
                        c.date,
                        c.is_active,
                    }).ToList();
                    if (cities.Count == 0)
                    {
                        return Json(new { key = 0, cities = cities }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { key = 1, cities = cities }, JsonRequestBehavior.AllowGet);
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


        #endregion


        #region app

        [HttpPost]
        public ActionResult Complaint(Complaints complaints, string lang = "ar")
        {
            try
            {

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    try
                    {
                        if (!ModelState.IsValid)
                        {

                            return Json(new
                            {
                                key = 0,
                                msg = creatMessage(lang, "Please enter blank fields", " Please enter blank fields")
                            }, JsonRequestBehavior.AllowGet);
                        }
                        db.Complaints.Add(complaints);


                        db.SaveChanges();
                        return Json(new
                        {
                            key = 1,
                            msg = creatMessage(lang, "successful ", " successful")
                        }, JsonRequestBehavior.AllowGet);
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
        public ActionResult GetSeting()
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var about = (from st in db.Setting select st).ToList().FirstOrDefault();

                    return Json(new
                    {
                        key = 1,
                        data = new
                        {
                            about.facebook,
                            about.instgram,
                            about.Condtions,
                            about.phone,
                            about.snapchat,
                            about.twitter,
                            about.aboutUs,
                            about.delivery,
                            about.Title1,
                            about.Description1,
                            about.Title2,
                            about.Description2,
                            about.Title3,
                            about.Description3,

                            about.Address,
                        }
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

        #endregion
    }
}