using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class Provider
    {
        public Provider()
        {
            Orders = new HashSet<Order>();
            Rates = new HashSet<Rate>();
        }
        [Key]
        public int id { get; set; }

        public int fk_city { get; set; } = 2;
        public string FkUser { get; set; }
        public string user_name { get; set; }
        //public int fk_branch { get; set; }
        //public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string password { get; set; }
        public string code { get; set; } // Generat code Rondom use in  sms
        public bool active_code { get; set; } = true;
        public string lang { get; set; } = "ar";  //اللغه هتكون عند اليوزر وتكون عربى فى الاول - وتتغير بسيرفس
        public string img { get; set; } //mult part
        public string drive_licence_img { get; set; } // muli part
        public string national_id_img { get; set; } // muli part
        public string device_type { get; set; } = "android"; // android or ios
        public string user_type { get; set; } = "client"; //لو نفس التطبيق  - provider or client
        public bool active { get; set; } //تفعيل الادمن
        public double rate { get; set; }
        public bool notification { get; set; }
        public DateTime date { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Rate> Rates { get; set; }

    }
}