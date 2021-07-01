using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.ViewModels
{
    public class NotifyViewModel
    {
        public int Id { get; set; }
        public int? order_id { get; set; }
        public int? order_type { get; set; }
        public string text { get; set; }
  
    }
}