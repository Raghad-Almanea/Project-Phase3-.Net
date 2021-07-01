using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.ViewModels
{
    public class OrderSiteViewModel
    {
        public int Id { get; set; }
        public int NumberOrder { get; set; }
        public int Status { get; set; }
        public DateTime Date { get; set; }
    }
}