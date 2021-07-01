using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class OrderInfo
    {
        public int Id { get; set; }
        public int fk_orderID { get; set; }
        public int fk_product { get; set; }
        public int qty { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public string img { get; set; }


        public virtual Order fk_order { get; set; }
    }
}