using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class Complaints
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string text { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
        public bool is_active { get; set; }

    }
}