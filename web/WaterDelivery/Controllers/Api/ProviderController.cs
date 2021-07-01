using WaterDelivery.Models;
using WaterDelivery.Models.TableDb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace WaterDelivery.Controllers.Api
{
    public class ProviderController : BaseController
    {
        // GET: Client
        public ActionResult Index()
        {
            return View();
        }
        #region User 

        [HttpPost]
        public ActionResult Register(Provider User, userModel userModel)
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
                            msg = creatMessage(User.lang, "من فضلك ادخل الاسم", "Please enter your Name")
                        }, JsonRequestBehavior.AllowGet);
                    }

                    //phone
                    if (User.phone == null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(User.lang, "من فضلك ادخل رقم الجوال", "Please enter your phone number")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var phone = (from st in db.Provider where st.phone == User.phone select st).SingleOrDefault();
                    if (phone != null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(User.lang, "عذرا هذا الجوال موجود بالفعل", "Sorry this mobile is already present")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    ////email
                    //if (User.email == null)
                    //{
                    //    return Json(new
                    //    {
                    //        key = 0,
                    //        msg = creatMessage(User.lang, "من فضلك ادخل البريد الالكترونى", "Please enter your email")
                    //    }, JsonRequestBehavior.AllowGet);
                    //}
                    //bool checkemail = IsValidEmail(User.email);
                    //if (!checkemail)
                    //{
                    //    return Json(new
                    //    {
                    //        key = 0,
                    //        msg = creatMessage(User.lang, "تاكد من ان البريد اللاكترونى مكتوب بشكل صحيح", "Make sure your e-mail is written correctly")
                    //    }, JsonRequestBehavior.AllowGet);
                    //}
                    //var email = (from st in db.Provider where st.email == User.email select st).SingleOrDefault();
                    //if (email != null)
                    //{
                    //    return Json(new
                    //    {
                    //        key = 0,
                    //        msg = creatMessage(User.lang, "عذرا هذا البريد الالكترونى موجود بالفعل", "Sorry this email is already present")
                    //    }, JsonRequestBehavior.AllowGet);
                    //}

                    //Password
                    if (User.password == null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(User.lang, "من فضلك ادخل  كلمه المرور", "Please enter your password")
                        }, JsonRequestBehavior.AllowGet);
                    }


                    #endregion
                    User.img = BaisUrlHoste + "Content/Img/Provider/generic-user.png";

                    var drive_license = Request.Files["drive_licence_img"];
                    if (drive_license != null)
                    {
                        Random rnd = new Random();
                        int count = rnd.Next(1, 1300);
                        string extension = DateTime.Now.Ticks.ToString() + count + ".jpg";

                        string path = Path.Combine(Server.MapPath("~/Content/Img/Provider/Drive_License/"), extension);
                        drive_license.SaveAs(path);

                        User.drive_licence_img = BaisUrlHoste + "Content/Img/Provider/Drive_License/" + extension;
                    }

                    var nationalId = Request.Files["national_id_img"];
                    if (nationalId != null)
                    {
                        Random rnd = new Random();
                        int count = rnd.Next(1, 1300);
                        string extension = DateTime.Now.Ticks.ToString() + count + ".jpg";

                        string path = Path.Combine(Server.MapPath("~/Content/Img/Provider/NationalID/"), extension);
                        nationalId.SaveAs(path);

                        User.national_id_img = BaisUrlHoste + "Content/Img/Provider/NationalID/" + extension;
                    }


                    User.active = true;
                    User.active_code = false;
                    User.date = DateTime.Now;
                    User.code = GetFormNumber();
                    db.Provider.Add(User);
                    db.SaveChanges();

                    Device_Id d = new Device_Id()
                    {
                        device_id = userModel.device_id,
                        fk_user = User.id
                    };
                    db.Device_Id.Add(d);
                    db.SaveChanges();

                    // get city data
                    var city = db.City.FirstOrDefault(c => c.Id == User.fk_city);

                    //Send Massage to phone

                    string s = SendMessageText("كود التحقق ", User.phone, User.code);


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
                            city_id = city.Id,
                            city_name = city.name
                        },
                        msg = creatMessage(User.lang, "تم التسجيل بنجاح", "successfully registered"),
                        status = false,
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
        [HttpPost]
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
                            msg = creatMessage(userModel.lang, "من فضلك ادخل كود التحقق", "Please enter your verification code")
                        }, JsonRequestBehavior.AllowGet);
                    }

                    var codeuser = db.Provider.Find(userModel.user_id);
                    if (codeuser != null)
                    {
                        if (codeuser.code == userModel.code)
                        {
                            codeuser.active_code = true;
                            db.SaveChanges();
                            return Json(new
                            {
                                key = 1,
                                msg = creatMessage(codeuser.lang, "تم تفعيل الدخول بنجاح", "Logged in successfully")
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { key = 0, msg = creatMessage(codeuser.lang, "برجاء ادخال الكود بشكل صحيح", "Please enter the code correctly") }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { key = 0, msg = creatMessage(codeuser.lang, "عذرا هذا الهاتف غير مسجل لدينا", "Sorry this phone is not registered") }, JsonRequestBehavior.AllowGet);

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
                            msg = creatMessage(userModel.lang, "من فضلك تاكد من البيانات", "Please verify the data")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var codeuser = (from st in db.Provider where st.id == userModel.user_id select st).SingleOrDefault();

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
                            msg = creatMessage(codeuser.lang, "تم ارسال الكود الى رقم هاتفك", "Code sent"),
                            // status = "active",
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { key = 0, msg = creatMessage(userModel.lang, "عذرا رقم الهاتف غير مسجل لدينا", "Sorry phone number is not registered") }, JsonRequestBehavior.AllowGet);
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
        //[HttpPost]
        //public ActionResult sign_in(userModel userModel)
        //{
        //    try
        //    {

        //        using (ApplicationDbContext db = new ApplicationDbContext())
        //        {
        //            var checkuser = (from st in db.Provider where st.phone == userModel.phone select st).SingleOrDefault();
        //            if(checkuser !=null)
        //            {
        //                #region validation
        //                if (checkuser == null)
        //                {
        //                    return Json(new
        //                    {
        //                        key = 0,
        //                        msg = creatMessage(userModel.lang, "من فضلك  تاكد من رقم الهاتف", "Please enter your phone number")
        //                    }, JsonRequestBehavior.AllowGet);
        //                }
        //                if (userModel.phone == "")
        //                {
        //                    return Json(new
        //                    {
        //                        key = 0,
        //                        msg = creatMessage(userModel.lang, "من فضلك ادخل رقم الهاتف", "Please enter your phone number")
        //                    }, JsonRequestBehavior.AllowGet);
        //                }
        //                if (userModel.password == "")
        //                {
        //                    return Json(new
        //                    {
        //                        key = 0,
        //                        msg = creatMessage(userModel.lang, "من فضلك ادخل  كلمة المرور ", "Please enter your  password")
        //                    }, JsonRequestBehavior.AllowGet);
        //                }
        //                if (userModel.password != checkuser.password)
        //                {
        //                    return Json(new
        //                    {
        //                        key = 0,
        //                        msg = creatMessage(userModel.lang, "من فضلك تاكد من كلمة المرور ", "Please sure your  password")
        //                    }, JsonRequestBehavior.AllowGet);
        //                }
        //                if (checkuser.active == false && checkuser.active_code == true)
        //                {
        //                    return Json(new
        //                    {
        //                        key = 0,
        //                        data = new { },
        //                        status = "blocked",
        //                        msg = creatMessage(checkuser.lang, "هذا الحساب مغلق من قبل الادمن", "This account is closed by the addict")
        //                    }, JsonRequestBehavior.AllowGet);
        //                }
        //                if (checkuser.active_code == false)
        //                {
        //                    return Json(new
        //                    {
        //                        key = 0,
        //                        data = new
        //                        {
        //                            checkuser.id,
        //                            checkuser.code,
        //                            active_code = checkuser.active_code,
        //                        },

        //                        msg = creatMessage(userModel.lang, "هذا الحساب لم يفعل بعد", "This account is not active")
        //                    }, JsonRequestBehavior.AllowGet);
        //                }
        //                #endregion

        //                // add new device
        //                checkuser.lang = userModel.lang;
        //                db.SaveChanges();
        //                var check_device_id = (from st in db.Device_Id where st.device_id == userModel.device_id && st.fk_user == checkuser.id select st).Any();

        //                // get city data
        //                var city = db.City.FirstOrDefault(c => c.Id == checkuser.fk_city);

        //                if (!check_device_id)
        //                {
        //                    Device_Id d = new Device_Id()
        //                    {
        //                        device_id = userModel.device_id,
        //                        fk_user = checkuser.id
        //                    };
        //                    db.Device_Id.Add(d);
        //                    db.SaveChanges();
        //                }
        //                return Json(new
        //                {
        //                    key = 1,
        //                    data = new
        //                    {
        //                        checkuser.user_name,
        //                        //checkuser.email,
        //                        checkuser.phone,
        //                        checkuser.id,
        //                        device_id = userModel.device_id ?? "",
        //                        img = checkuser.img,
        //                        checkuser.national_id_img,
        //                        checkuser.drive_licence_img,
        //                        checkuser.lang,
        //                        city.Id,
        //                        city.name

        //                    },
        //                    status = true,
        //                    notification = checkuser.notification,
        //                    msg = creatMessage(checkuser.lang, "تم تسجيل الدخول بنجاح", "Logged in successfully")
        //                }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {

        //            }





        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            key = 0,
        //            msg = ex.Message
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public ActionResult GetDataOfUser(userModel userModel)
        {

            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    var User = (from st in db.Provider where st.id == userModel.user_id select st).SingleOrDefault();

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
                            User.national_id_img,
                            User.drive_licence_img,
                            User.lang

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
                    var codeuser = (from st in db.Provider where st.phone == userModel.phone select st).SingleOrDefault();
                    if (codeuser == null)
                    {
                        return Json(new { key = 0, msg = creatMessage(userModel.lang, "عذرا رقم الهاتف غير مسجل لدينا", "Sorry phone number is not registered") }, JsonRequestBehavior.AllowGet);

                    }
                    if (codeuser.active == false)
                    {
                        return Json(new
                        {
                            key = 0,
                            data = new { },
                            status = "blocked",
                            msg = creatMessage(codeuser.lang, "هذا الحساب مغلق من قبل الادمن", "This account is closed by the addict")
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
                            msg = creatMessage(codeuser.lang, "تم ارسال الكود الى رقم هاتفك", "Code sent")
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
                            msg = creatMessage(userModel.lang, "من فضلك ادخل كود التحقق", "Please enter your verification code")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (userModel.current_pass == "")
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(userModel.lang, "من فضلك ادخل كلمة المرور الجديدة ", "Please enter your new password")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    try
                    {
                        var codeuser = (from st in db.Provider where st.id == userModel.user_id select st).SingleOrDefault();
                        if (codeuser != null)
                        {
                            if (codeuser.code != userModel.code)
                            {
                                return Json(new { key = 0, msg = creatMessage(codeuser.lang, " كود التحقق غير صحيح", "  Invalid verification code") }, JsonRequestBehavior.AllowGet);
                            }

                            codeuser.password = userModel.current_pass;
                            db.SaveChanges();


                            return Json(new { key = 1, msg = creatMessage(codeuser.lang, "تم تغيير كلمة المرور بنجاح", "Password changed successfully") }, JsonRequestBehavior.AllowGet);
                        }



                        else
                        {
                            return Json(new { key = 0, msg = creatMessage(userModel.lang, "حدث خطا ما", "Something went wrong") }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception)
                    {
                        return Json(new { key = 0, msg = creatMessage(userModel.lang, "حدث خطا ما", "Something went wrong") }, JsonRequestBehavior.AllowGet);
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
                            msg = creatMessage(userModel.lang, "عذرا لم يتم العثور على هذا المقدم ", "Sorry this User was not found")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (userModel.old_password == null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(userModel.lang, "من فضلك ادخل كلمة المرور القديمة ", "Please enter your old password")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (userModel.new_password == null)
                    {
                        return Json(new
                        {
                            key = 0,
                            msg = creatMessage(userModel.lang, "من فضلك ادخل كلمة المرور الجديدة  ", "Please enter your new password")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var codeuser = (from st in db.Provider where st.id == userModel.user_id select st).SingleOrDefault();
                    if (codeuser != null)
                    {
                        if (codeuser.password != userModel.old_password && userModel.old_password != "")
                        {
                            return Json(new
                            {
                                key = 0,
                                msg = creatMessage(codeuser.lang, "من فضلك ادخل كلمة المرور القديمة بشكل صحيح", "Please enter the old password correctly")
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (userModel.old_password == "")
                            {
                                return Json(new
                                {
                                    key = 0,
                                    msg = creatMessage(codeuser.lang, "من فضلك ادخل كلمة المرور القديمة ", "Please enter your old password")
                                }, JsonRequestBehavior.AllowGet);
                            }
                            if (userModel.new_password == "")
                            {
                                return Json(new
                                {
                                    key = 0,
                                    msg = creatMessage(codeuser.lang, "من فضلك ادخل كلمة المرور الجديدة ", "Please enter your new password")
                                }, JsonRequestBehavior.AllowGet);
                            }
                            codeuser.password = userModel.new_password;
                            db.SaveChanges();
                        }

                        return Json(new { key = 1, msg = creatMessage(codeuser.lang, "تم تغيير كلمة المرور بنجاح", "Password changed successfully") }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { key = 0, msg = creatMessage(userModel.lang, "حدث خطا ما", "Something went wrong") }, JsonRequestBehavior.AllowGet);
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
        public ActionResult UpdateUserData(Provider User, userModel userModel)
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
                            msg = creatMessage(userModel.lang, "عذرا لم يتم العثور على هذا المستخدم ", "Sorry this User was not found")
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var Userdata = (from st in db.Provider where st.id == User.id select st).SingleOrDefault();
                    if (Userdata != null)
                    {
                        if (User.user_name != null)
                        {
                            Userdata.user_name = User.user_name;
                        }

                        if (User.fk_city != 0)
                        {
                            Userdata.fk_city = User.fk_city;
                        }

                        if (User.phone != null)
                        {
                            var chekphone = (from st in db.Provider where (st.phone == User.phone && st.id != User.id) select st).SingleOrDefault();
                            var chek = (from st in db.Client where (st.phone == User.phone) select st).SingleOrDefault();

                            if (chekphone != null || chek != null)
                            {
                                return Json(new
                                {
                                    key = 0,
                                    msg = creatMessage(Userdata.lang, "عذرا رقم الهاتف مسجل من قبل", "Sorry this phone number is already registered")
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                Userdata.phone = User.phone;
                            }
                        }
                        //if (User.email != null)
                        //{
                        //    var chekemail = (from st in db.Provider where (st.email == User.email && st.id != User.id) select st).SingleOrDefault();
                        //    if (chekemail != null)
                        //    {
                        //        return Json(new
                        //        {
                        //            key = 0,
                        //            msg = creatMessage(Userdata.lang, "عذرا هذا البريد الالكترونى مسجل من قبل", "Sorry this email is already registered")
                        //        }, JsonRequestBehavior.AllowGet);
                        //    }
                        //    else
                        //    {
                        //        Userdata.email = User.email;
                        //    }
                        //}

                        // add new imgae
                        var file = Request.Files["Img"];
                        if (file != null)
                        {
                            Random rnd = new Random();
                            int count = rnd.Next(1, 1300);
                            string extension = DateTime.Now.Ticks.ToString() + count + ".jpg";

                            string path = Path.Combine(Server.MapPath("~/Content/Img/Provider/"), extension);
                            file.SaveAs(path);

                            Userdata.img = BaisUrlHoste + "Content/Img/Provider/" + extension;
                        }

                        var drive_license = Request.Files["drive_licence_img"];
                        if (drive_license != null)
                        {
                            Random rnd = new Random();
                            int count = rnd.Next(1, 1300);
                            string extension = DateTime.Now.Ticks.ToString() + count + ".jpg";

                            string path = Path.Combine(Server.MapPath("~/Content/Img/Provider/Drive_License/"), extension);
                            drive_license.SaveAs(path);

                            User.drive_licence_img = BaisUrlHoste + "Content/Img/Provider/Drive_License/" + extension;
                            Userdata.drive_licence_img = User.drive_licence_img;
                        }
                        else
                        {
                            Userdata.drive_licence_img = Userdata.drive_licence_img;
                        }

                        var nationalId = Request.Files["national_id_img"];
                        if (nationalId != null)
                        {
                            Random rnd = new Random();
                            int count = rnd.Next(1, 1300);
                            string extension = DateTime.Now.Ticks.ToString() + count + ".jpg";

                            string path = Path.Combine(Server.MapPath("~/Content/Img/Provider/NationalID/"), extension);
                            nationalId.SaveAs(path);

                            User.national_id_img = BaisUrlHoste + "Content/Img/Provider/NationalID/" + extension;
                            Userdata.national_id_img = User.national_id_img;
                        }
                        else
                        {
                            Userdata.national_id_img = Userdata.national_id_img;
                        }




                        db.SaveChanges();



                    }

                    // get city data
                    var city = db.City.FirstOrDefault(c => c.Id == Userdata.fk_city);

                    return Json(new
                    {
                        key = 1,
                        data = new
                        {
                            Userdata.user_name,
                            //Userdata.email,
                            Userdata.phone,
                            Userdata.id,
                            userModel.device_id,
                            img = Userdata.img,
                            Userdata.national_id_img,
                            Userdata.drive_licence_img,
                            Userdata.lang,
                            city.Id,
                            city.name,
                            typeuser = 1

                        },
                        msg = creatMessage(Userdata.lang, "تم تعديل البيانات بنجاح", "Data modified successfully")
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
                            msg = creatMessage(userModel.lang, "تم تسجيل الخروج بنجاح", "Logged out successfully"),
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

                var notify = db.Notify.Where(x => x.fk_provider == user_id && x.type == 2).Select(x => new
                {
                    x.text,
                    x.order_id,
                    x.order_type,
                    x.Id

                }).ToList().OrderByDescending(xx => xx.Id);

                var notify_not_shown = db.Notify.Where(x => x.fk_provider == user_id && x.fk_provider_show == false).ToList();
                if (notify_not_shown.Count != 0)
                {
                    notify_not_shown.ForEach(a => a.fk_provider_show = true);
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
                var provider = context.Provider.FirstOrDefault(c => c.id == user_id);
                if (provider.notification == true)
                {
                    provider.notification = false;
                    context.SaveChanges();
                    return Json(new { key = 1, notification = provider.notification, msg = "تم قفل الاشعارات بنجاح " }, JsonRequestBehavior.AllowGet);
                }

                provider.notification = true;
                context.SaveChanges();
                return Json(new { key = 1, notification = provider.notification, msg = "تم تشغيل الاشعارات بنجاح " }, JsonRequestBehavior.AllowGet);

            }

        }



        #endregion 
        #region logic
        [HttpPost]
        public ActionResult GetNewOrder()
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var orders = (from st in db.Order
                                  join client in db.Client on st.fk_userID equals client.id
                                  where st.type == (int)Order_type.New
                                  select new
                                  {
                                      st.Id,
                                      st.net_total,
                                      st.date,
                                      st.address,
                                      st.delivary,
                                      st.delivary_time,
                                      st.fk_userID,
                                      client.img,
                                      client.user_name,
                                      client.phone,
                                      dateOrder = st.date_time.Day + "-" + st.date_time.Month + "-" + st.date_time.Year,
                                      timeOrder = st.date_time.Hour + ":" + st.date_time.Minute,
                                      st.type,
                                      st.lat,
                                      st.lng,
                                      st.total

                                  }).ToList().OrderByDescending(x => x.Id);

                    //var clients = db.Client.Where(x => x.active == true).ToList();
                    //foreach (var item in clients)
                    //{
                    //    Notify notify = new Notify();
                    //    notify.date = DateTime.Now;
                    //    notify.fk_user = item.id;
                    //    notify.text = "تم الموافقة على الطلب";

                    //    db.Notify.Add(notify);
                    //    db.SaveChanges();
                    //    //SendPushNotification(item.id, notify.text, 1);
                    //}

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
        [HttpPost]
        public ActionResult GetAcceptedOrder(int fk_providerId)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var product = (from st in db.Order
                                   join client in db.Client on st.fk_userID equals client.id
                                   where st.fk_providerID == fk_providerId
                                         && st.type == (int)Order_type.accept_deleget
                                   select new
                                   {
                                       st.Id,
                                       st.net_total,
                                       st.date,
                                       st.address,
                                       st.delivary,
                                       st.delivary_time,
                                       st.fk_userID,
                                       st.lat,
                                       st.lng,
                                       st.total,
                                       st.type,
                                       client.img,
                                       client.user_name,
                                       client.phone,

                                   }).ToList().OrderByDescending(x => x.Id);

                    // هنعمل اشعار للعميل
                    //var client = db.Client.Where(x => x.active == true && x.id == fk_user).ToList();
                    //foreach (var item in client)
                    //{
                    //    Notify notify = new Notify();
                    //    notify.date = DateTime.Now;
                    //    notify.fk_user = item.id;
                    //    notify.text = "تم الموافقة على الطلب";

                    //    db.Notify.Add(notify);
                    //    db.SaveChanges();
                    //    SendPushNotification(item.id, notify.text, 1);
                    //}


                    return Json(new
                    {
                        key = 1,
                        product = product
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
        public ActionResult ChangeOrderStutes(int fk_order, int type, int fk_providerId)
        {
            //type // 2- accepted 3-refused 4-finished
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Order order = db.Order.FirstOrDefault(x => x.Id == fk_order);

                    if (order.fk_providerID != fk_providerId && order.type > (int)Order_type.New)
                    {
                        return Json(new { key = 0, msg = "تم الموافقة على الطلب من قبل مندوب آخر" }, JsonRequestBehavior.AllowGet);
                    }
                    if (order.type == (int)Order_type.New)
                    {
                        order.type = type;
                        order.fk_providerID = fk_providerId;

                        db.SaveChanges();
                    }

                    int fk_user = order.fk_userID;

                    if (order.type == (int)Order_type.accept_deleget)
                    {
                        Notify notify = new Notify();
                        notify.date = DateTime.Now;
                        notify.fk_user = fk_user;
                        notify.order_id = fk_order;
                        notify.order_type = order.type;
                        notify.text = "تم استلام الطلب رقم " + fk_order;
                        notify.type = 1;
                        db.Notify.Add(notify);
                        db.SaveChanges();
                        SendPushNotification(fk_user, order.Id, order.type, notify.text, 1);

                        return Json(new
                        {
                            key = 1,
                            order_id = order.Id,
                            msg = creatMessage("ar", "تم الموافقة على الطلب بنجاح", "The order accepted successfully")
                        }, JsonRequestBehavior.AllowGet);
                    }

                    if (order.type == (int)Order_type.dashborad_Ready)
                    {
                        Notify notify = new Notify();
                        order.type = type;
                        notify.date = DateTime.Now;
                        notify.fk_user = fk_user;
                        notify.order_id = fk_order;
                        notify.order_type = order.type;
                        notify.text = "تم تسليم الطلب رقم " + fk_order + " بنجاح";
                        notify.type = 1;
                        db.Notify.Add(notify);
                        db.SaveChanges();
                        SendPushNotification(fk_user, order.Id, order.type, notify.text, 1);

                        return Json(new
                        {
                            key = 1,
                            order_id = order.Id,
                            msg = creatMessage("ar", "تم تسليم الطلب بنجاح", "The order delivered successfully")
                        }, JsonRequestBehavior.AllowGet);
                    }

                    if (order.type == (int)Order_type.Deleget_Diliverd)
                    {
                        order.type = type;
                        Notify notify = new Notify();
                        order.type = type;
                        notify.date = DateTime.Now;
                        notify.fk_user = fk_user;
                        notify.order_id = fk_order;
                        notify.order_type = order.type;
                        notify.text = "تم انهاء الطلب رقم " + fk_order + " بنجاح";
                        notify.type = 1;
                        db.Notify.Add(notify);
                        db.SaveChanges();
                        SendPushNotification(fk_user, order.Id, order.type, notify.text, 1);

                        return Json(new
                        {
                            key = 1,
                            msg = creatMessage("ar", "تم انهاء الطلب بنجاح", "The order was finished successfully")
                        }, JsonRequestBehavior.AllowGet);
                    }


                    return Json(new
                    {
                        key = 1,
                        msg = creatMessage("ar", " تم استلام الطلب جاري العمل ... ", "The order is in progress")
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
        public ActionResult GetOrderInfo(int order_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Setting setting = db.Setting.FirstOrDefault();
                    var sum = (db.OrderInfo.Where(oi => oi.fk_orderID == order_id).Select(oi => oi.qty * oi.price)).Sum();
                    var vats = (setting.vat / 100) * sum;
                    var orderFound = db.Order.Where(x => x.Id == order_id).ToList().Select(x => new
                    {
                        x.Id,
                        provider_name = x.fk_provider == null ? "لا يوجد" : x.fk_provider.user_name,
                        provider_img = x.fk_provider == null ? "لا يوجد" : x.fk_provider.img,
                        provider_phone = x.fk_provider == null ? "لا يوجد" : x.fk_provider.phone,
                        delivary_time = x.delivary_time,
                        x.date,
                        stutes = x.type,
                        x.delivary,
                        total = x.net_total,
                        x.address,
                        x.lat,
                        x.lng,
                        x.fk_userID,
                        clientName = db.Client.Where(xx => xx.id == x.fk_userID).Select(xx => xx.user_name).FirstOrDefault(),
                        clientphone = db.Client.Where(xx => xx.id == x.fk_userID).Select(xx => xx.phone).FirstOrDefault(),
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
                    },JsonRequestBehavior.AllowGet);
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
        //public ActionResult GetOrderInfo(int user_id, int order_id)
        //{
        //    try
        //    {
        //        using (ApplicationDbContext db = new ApplicationDbContext())
        //        {
        //            var orderInfo = ((from orderinf in db.OrderInfo
        //                              join order in db.Order on orderinf.fk_orderID equals order.Id
        //                              where orderinf.fk_orderID == order_id && order.fk_providerID == user_id
        //                              select new { orderinf, order }).ToList().Select(x => new
        //                              {
        //                                  order_id = x.order.Id,
        //                                  user_id = x.order.fk_user.id,
        //                                  x.order.fk_user.user_name,
        //                                  x.order.fk_user.img,
        //                                  x.order.fk_user.phone,
        //                                  delivary_time = x.order.delivary_time,
        //                                  x.order.date,
        //                                  x.order.type,
        //                                  x.order.delivary,
        //                                  total = x.order.total + x.order.delivary,
        //                                  x.order.address,
        //                                  x.order.lat,
        //                                  x.order.lng,
        //                                  productList = (from o in db.OrderInfo
        //                                                 join product in db.Product on o.fk_product equals product.Id
        //                                                 where o.fk_orderID == x.order.Id
        //                                                 select new
        //                                                 {
        //                                                     product_id = product.Id,
        //                                                     product_img = product.img,
        //                                                     product_name = product.name,
        //                                                     product_price = product.price,
        //                                                     product_qty = o.qty

        //                                                 }
        //                                    ).ToList()
        //                              })).ToList();


        //            return Json(new { key = 1, order = orderInfo }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            key = 0,
        //            msg = ex.Message
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public ActionResult GetCurrentActiveOrders(int user_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    //var myOrders = db.OrderInfo.Where(o => o.fk_order.fk_providerID == user_id && (o.fk_order.type >= (int)Order_type.accept_deleget && o.fk_order.type != (int)Order_type.Deleget_Diliverd)).AsEnumerable().DistinctBy(oi => oi.fk_orderID)
                    //    .Select(order => new
                    //    {
                    //        user_img = order.fk_order.fk_user.img,
                    //        order.fk_order.fk_user.user_name,
                    //        order.fk_order.fk_user.phone,
                    //        order.fk_orderID,

                    //    }).ToList().OrderByDescending(o => o.fk_orderID);
                    var myOrders = (from st in db.Order
                                    join client in db.Client on st.fk_userID equals client.id
                                    where
                                    (st.fk_providerID == user_id && (st.type < (int)Order_type.Client_recipt))
                                    select new
                                    {
                                        st.Id,
                                        st.net_total,
                                        st.date,
                                        st.address,
                                        st.delivary,
                                        st.fk_userID,
                                        st.lat,
                                        st.lng,
                                        st.total,
                                        st.type,
                                        client.img,
                                        client.user_name,
                                        client.phone,
                                        dateOrder = st.date_time.Day + "-" + st.date_time.Month + "-" + st.date_time.Year,
                                        timeOrder = st.date_time.Hour + ":" + st.date_time.Minute

                                    }).ToList().OrderByDescending(x => x.Id);
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
        public ActionResult GetPreviousOrders(int user_id)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    //var myOrders = db.OrderInfo.
                    //    Where(o => o.fk_order.fk_providerID == user_id && o.fk_order.type == (int)Order_type.Deleget_confirm).AsEnumerable().DistinctBy(oi => oi.fk_orderID)
                    //    .Select(order => new
                    //    {
                    //        user_img = order.fk_order.fk_user.img,
                    //        user_name = order.fk_order.fk_user.user_name,
                    //        phone = order.fk_order.fk_user.phone,
                    //        fk_orderID = order.fk_orderID,
                    //        date = order.fk_order.date,
                    //        total = order.fk_order.total + order.fk_order.delivary


                    //    }).ToList().OrderByDescending(o => o.fk_orderID);

                    var myOrders = (from st in db.Order
                                    join client in db.Client on st.fk_userID equals client.id
                                    where
                                    (st.fk_providerID == user_id && (st.type >= (int)Order_type.Client_recipt))
                                    select new
                                    {
                                        st.Id,
                                        st.net_total,
                                        st.date,
                                        st.address,
                                        st.delivary,
                                        st.fk_userID,
                                        st.lat,
                                        st.lng,
                                        st.total,
                                        st.type,
                                        client.img,
                                        client.user_name,
                                        client.phone,
                                        dateOrder = st.date_time.Day + "-" + st.date_time.Month + "-" + st.date_time.Year,
                                        timeOrder = st.date_time.Hour + ":" + st.date_time.Minute

                                    }).ToList().OrderByDescending(x => x.Id);

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

        public ActionResult GetCities()
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var cities = db.City.Where(c => c.is_active == true).Select(c => new
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
                                msg = creatMessage(lang, "من فضلك ادخل الحقول الفارغه", " Please enter blank fields")
                            }, JsonRequestBehavior.AllowGet);
                        }
                        db.Complaints.Add(complaints);


                        db.SaveChanges();
                        return Json(new
                        {
                            key = 1,
                            msg = creatMessage(lang, "تم الارسال بنجاح", " successful")
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
                            about.delivery
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