using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class Client
    {
        public Client()
        {
            Favorites = new HashSet<Favorite>();
            AddressUser = new HashSet<AddressUser>();
            Order = new HashSet<Order>();
            Rate = new HashSet<Rate>();

        }

        [Key]
        public int id { get; set; }

        public int fk_cityID { get; set; } = 2;
        public string user_name { get; set; }
        //public string email { get; set; }
        public string phone { get; set; }

        public string password { get; set; }
        public string code { get; set; } // Generat code Rondom use in  sms
        public bool active_code { get; set; } = true;
        public string lang { get; set; } = "ar";  //اللغه هتكون عند اليوزر وتكون عربى فى الاول - وتتغير بسيرفس
        public string img { get; set; } //mult part
        public string device_type { get; set; } = "android"; // android or ios
        public string user_type { get; set; } = "client"; //لو نفس التطبيق  - provider or client
        public bool active { get; set; } //تفعيل الادمن
        public bool notification { get; set; }
        public DateTime date { get; set; }

        public double wallet { get; set; } // المحفظة
        public int Points { get; set; } // عدد النقاط

        public virtual City fk_city { get; set; }
        public virtual ICollection<AddressUser> AddressUser { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<Rate> Rate { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }

    }
}