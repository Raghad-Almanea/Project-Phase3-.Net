using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class Setting
    {
        [Key]
        public int id { get; set; }
        public string Condtions { get; set; }
        public string Condtions_en { get; set; }
        public string phone { get; set; }
        public string phone2 { get; set; }
        public string Address { get; set; }
        public string facebook { get; set; }
        public string whatapp { get; set; }
        public string instgram { get; set; }
        public string twitter { get; set; }
        public string snapchat { get; set; }
        public string aboutUs { get; set; }
        public string aboutUs_en { get; set; }
        public double delivery { get; set; }
        public double delivery_default_distance_in_km { get; set; } = 10;
        public double delivery_for_aditinal_km { get; set; } = 1;
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string FooterDescription { get; set; }
        public string FooterDescription_en { get; set; }
        public string GooglePlayUrl { get; set; }
        public string AppleStoreUrl { get; set; }

        public double vat { get; set; }
        public int PointsPerOrder { get; set; } // عدد النقاط لكل طلب
        public int PointsPerRiyal { get; set; } // عدد النقاط مقابل كل ريال 
        public string Title1 { get; set; }
        public string Title1_en { get; set; }
        public string Description1 { get; set; }
        public string Description1_en { get; set; }

        public string Title2 { get; set; }
        public string Title2_en { get; set; }
        public string Description2 { get; set; }
        public string Description2_en { get; set; }

        public string Title3 { get; set; }
        public string Title3_en { get; set; }
        public string Description3 { get; set; }
        public string Description3_en { get; set; }


    }
}