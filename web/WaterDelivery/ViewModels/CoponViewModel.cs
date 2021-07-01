using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WaterDelivery.Helper;

namespace WaterDelivery.ViewModels
{
    public class CoponsViewModel

    {
        public int ID { get; set; }

        public int Count { get; set; }// عدد المستفيدين 

        public int CountUsed { get; set; }// عدد استخدام الكوبون 
        public DateTime expirdate { get; set; }// تاريخ انتهاء الخصم
        public string CoponCode { get; set; } // 

        public double Discount { get; set; }// نسبه ياعنى 20% 

        public double limt_discount { get; set; } //حد اقصى للخصم مثلا 50 ريال
        public bool IsActive { get; set; }// يعامل معامله الحذف 
    }

    public class CreateCoponViewModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "من فضلك هذا الحقل مطلوب")]
        [RegularExpression("(^[0-9]+)", ErrorMessage = "برجاء ادخال ارقام موجبة فقط")]
        public string Count { get; set; }// عدد المستفيدين 
        //[Required(ErrorMessage = "من فضلك هذا الحقل مطلوب")]
        //[RegularExpression("([0-9]+)", ErrorMessage = "برجاء ادخال ارقام موجبة فقط")]
        //public string CountUsed { get; set; }// عدد استخدام الكوبون 
        [Required(ErrorMessage = "من فضلك هذا الحقل مطلوب")]
        [DataType(DataType.DateTime)]
        [PresentOrFutureDate]
        public string expirdate { get; set; }// تاريخ انتهاء الخصم
        [Required(ErrorMessage = "من فضلك هذا الحقل مطلوب")]
        public string CoponCode { get; set; } // 
        [Required(ErrorMessage = "من فضلك هذا الحقل مطلوب")]
        [RegularExpression("([0-9]+)", ErrorMessage = "برجاء ادخال ارقام موجبة فقط")]
        public string Discount { get; set; }// نسبه ياعنى 20% 
        [Required(ErrorMessage = "من فضلك هذا الحقل مطلوب")]
        [RegularExpression("([0-9]+)", ErrorMessage = "برجاء ادخال ارقام موجبة فقط")]
        public string limt_discount { get; set; } //حد اقصى للخصم مثلا 50 ريال
        public bool IsActive { get; set; }// يعامل معامله الحذف 
    }


}
