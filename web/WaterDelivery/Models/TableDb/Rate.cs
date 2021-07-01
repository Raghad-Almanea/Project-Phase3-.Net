using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class Rate
    {
        [Key]
        public int ID { get; set; }
        public int fk_userID { get; set; }
        public int fk_providerID { get; set; }
        public int rate { get; set; }
        public string comment { get; set; }
        public DateTime date { get; set; }

        public virtual Client fk_user { get; set; }
        public virtual Provider fk_provider { get; set; }
    }
}