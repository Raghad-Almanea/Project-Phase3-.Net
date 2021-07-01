using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class Device_Id
    {
        [Key]
        public int id { get; set; }
        public int fk_user { get; set; }
        public bool is_client { get; set; }
        public string device_id { get; set; }
    }
}