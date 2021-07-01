using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models
{
    public class userModel
    {
        public int user_id { get; set; }
        public string code { get; set; }
        public string lang { get; set; } = "ar";
        public string phone { get; set; }
        public string password { get; set; }
        public string device_id { get; set; } = "";
        public string current_pass { get; set; }
        public string address { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string old_password { get; set; }
        public string new_password { get; set; }
        public bool notify { get; set; }
        //public int fk_branch { get; set; }
    }
}