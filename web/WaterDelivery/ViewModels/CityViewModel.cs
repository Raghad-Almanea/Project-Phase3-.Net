using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.ViewModels
{
    public class CityViewModel
    {
        public int Id { get; set; }

        //public string branch_name { get; set; }
        public string city_name { get; set; }
        public bool is_active { get; set; }
    }
}