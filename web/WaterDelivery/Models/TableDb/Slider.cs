using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class Slider
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public int type { get; set; } // 1- MainSlider 2-SubImgAdvert
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}