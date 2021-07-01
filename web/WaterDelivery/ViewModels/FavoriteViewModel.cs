using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterDelivery.ViewModels
{
    public class FavoriteViewModel
    {
        public int user_id { get; set; }
        public int fk_product { get; set; }
        public string lang { get; set; }


    }

    public class ChekFavorite
    {
        public int key { get; set; }
        public string msg { get; set; }
    }
    public class FavoriteProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public double Price { get; set; }
    }

    public class FavoriteData
    {
        public int user_id { get; set; }
        public string lang { get; set; }
    }
}
