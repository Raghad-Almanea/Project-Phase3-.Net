using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string branch_name { get; set; }
        public string category_name { get; set; }
        public string maincategory_name { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public string img { get; set; }
        public bool is_active { get; set; }
        public int all_qty { get; set; }
    }
}