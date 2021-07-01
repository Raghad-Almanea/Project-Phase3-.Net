using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class SubCategory
    {
       
        [Key]
        public int Id { get; set; }
        public int fk_cat { get; set; }
        public string name { get; set; }
        public string name_en { get; set; }
        public string img { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
        public bool is_active { get; set; } = true;

    }
}