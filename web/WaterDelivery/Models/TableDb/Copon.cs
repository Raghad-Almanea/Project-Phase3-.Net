using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WaterDelivery.Models.TableDb
{
    public class Copon
    {
        [Key]
        public int id { get; set; }
        public int count { get; set; }// عدد المستفيدين 
        public int count_used { get; set; }// عدد استخدام الكوبون 
        public DateTime expirdate { get; set; }// تاريخ انتهاء الخصم
        public string copon_code { get; set; } // 
        public double discount { get; set; }// نسبه ياعنى 20% 
        public double limt_discount { get; set; } //حد اقصى للخصم مثلا 50 ريال
        public bool isActive { get; set; }// يعامل معامله الحذف 
        public DateTime date { get; set; } = DateTime.Now;
        public virtual ICollection<CoponUsed> CoponUseds { get; set; } = new HashSet<CoponUsed>();
    }
}
