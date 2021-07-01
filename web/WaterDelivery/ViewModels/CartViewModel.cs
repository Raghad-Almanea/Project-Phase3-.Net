using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.ViewModels
{
    public class CartViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
    }

}