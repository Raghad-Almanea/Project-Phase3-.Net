using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WaterDelivery.Models.TableDb
{
    public class Order
    {
        public Order()
        {
            OrderInfo = new HashSet<OrderInfo>();
        }
        [Key]
        public int Id { get; set; }
        public int fk_userID { get; set; }
        public int? fk_providerID { get; set; }
        public int is_paied { get; set; } = 1;
        public int type { get; set; } = 1;//1-new 2- accepted 3-refused 4-finished
        public int payment_type { get; set; }
        public double total { get; set; }
        public double delivary { get; set; }
        public string delivary_time { get; set; }
        public double net_total { get; set; }
        public string address { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string date { get; set; } = DateTime.Now.ToString("MM/dd/yyyy");
        public DateTime date_time { get; set; } = DateTime.Now;
        public bool is_active { get; set; }
        public virtual Client fk_user { get; set; }
        public virtual Provider fk_provider { get; set; }
        public virtual ICollection<OrderInfo> OrderInfo { get; set; }
    }
    enum Order_type
    {
        New = 1,
        accept_deleget = 2,
        dashborad_Ready = 3,
        Deleget_Diliverd = 4,
        Client_recipt = 5,
        Deleget_confirm = 6,
        RateDelegert = 7
    }

    enum Payment_type
    {
        Cash = 1,
        Wallet = 2
    }
}