using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterDelivery.ViewModels
{
    public class CreateProductViewModel
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string specification { get; set; }
        [RegularExpression(@"\d+(?:\.\d+)?", ErrorMessage = "Please enter the price numbers only")]
        public string price { get; set; }
        public string img { get; set; }
        public DateTime date { get; set; }
        public bool is_active { get; set; }
        public int fk_categoryID { get; set; }
        public int all_qty { get; set; }
    }
}