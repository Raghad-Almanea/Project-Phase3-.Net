using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string date { get; set; }
        public DateTime date_time { get; set; }

        public string user_name { get; set; }
        public string user_phone { get; set; }
        public string address { get; set; }
        public int order_status { get; set; }

        public double total { get; set; }
        public bool is_active { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string mandoob_name { get; set; }
    }
}