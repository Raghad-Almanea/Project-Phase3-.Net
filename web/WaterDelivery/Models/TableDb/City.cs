using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class City
    {
        public City()
        {
            Client = new HashSet<Client>();
        }
        public int Id { get; set; }
        public string name { get; set; }
        public string date { get; set; } = DateTime.Now.ToString("yyyy MMMM dd");
        public bool is_active { get; set; }

        public virtual ICollection<Client> Client { get; set; }
    }
}