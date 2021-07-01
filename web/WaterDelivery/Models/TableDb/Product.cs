using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class Product
    {

        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public string img { get; set; }
        public DateTime date { get; set; }
        public bool is_active { get; set; }
        public int fk_categoryID { get; set; }
        public int all_qty { get; set; }
        public string specification { get; set; }
        public string ByUser { get; set; } // خاص بالتاجر

        public virtual ICollection<Favorite> Favorites { get; set; }

        public Product()
        {
            Favorites = new HashSet<Favorite>();
        }
    }
}