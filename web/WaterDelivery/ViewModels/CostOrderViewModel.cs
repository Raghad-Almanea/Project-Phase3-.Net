using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.ViewModels
{
    public class CostOrderViewModel
    {  
        public double Delivery { get; set; }
        public double total { get; set; }
        public double net_total { get; set; }
        public double vat { get; set; }
    }
}