using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class Notify
    {
        public int Id { get; set; }
        public int fk_user { get; set; }
        public int fk_provider { get; set; }
        public string text { get; set; }
        public DateTime date { get; set; }
        public int? order_id { get; set; }
        public int? order_type { get; set; }
        public bool fk_user_show { get; set; }
        public bool fk_provider_show { get; set; }

        public int type { get; set; }

    }
}