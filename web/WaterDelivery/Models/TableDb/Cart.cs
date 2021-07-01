using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class Cart
    {
        [Key]
        public int ID { get; set; }
        public int fk_userID { get; set; }
        public int fk_productID { get; set; }
        public int qty { get; set; }

        public virtual Client fk_user { get; set; }
        public virtual Product fk_product { get; set; }


    }
}