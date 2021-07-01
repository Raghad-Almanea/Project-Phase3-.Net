using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.ViewModels
{
    public class ClientViewModel
    {
        public int id { get; set; }
        public string city_name { get; set; }
        public string user_name { get; set; }
        public string branch_name { get; set; }
        //public string email { get; set; }
        public string phone { get; set; }
        public bool is_active { get; set; }
        public string img { get; set; }
        public double wallet { get; set; }
        public string userId { get; set; }
    }
}