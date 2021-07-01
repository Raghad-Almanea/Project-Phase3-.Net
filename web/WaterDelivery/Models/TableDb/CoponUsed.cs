using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WaterDelivery.Models.TableDb
{
    public class CoponUsed
    {
        [Key]
        public int id { get; set; }

        public int fk_copon { get; set; }

        public int fk_order { get; set; }

        public int fk_user { get; set; }

    }
}
