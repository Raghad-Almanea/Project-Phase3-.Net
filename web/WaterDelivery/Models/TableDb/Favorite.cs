using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WaterDelivery.Models.TableDb;

namespace WaterDelivery.Models.TableDb
{
    public class Favorite
    {
        public int Id { get; set; }
        public int FkUserId { get; set; }
        public int FkProductId { get; set; }
        public virtual Client User { get; set; }
        public virtual Product Product { get; set; }

    }
}
