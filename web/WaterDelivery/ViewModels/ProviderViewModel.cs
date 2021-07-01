using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.ViewModels
{
    public class ProviderViewModel
    {
        public int id { get; set; }
        public string city_name { get; set; }
        public string user_name { get; set; }
        //public string branch_name { get; set; }
        //public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public bool is_active { get; set; }
    }
}