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

        public int Count { get; set; }

        public int CountUsed { get; set; }
        public DateTime expirdate { get; set; }
        public string CoponCode { get; set; }

        public double Discount { get; set; }
        public double limt_discount { get; set; }
        public bool IsActive { get; set; }

        public class CreateCoponViewModel
        {
            public int ID { get; set; }
            [Required(ErrorMessage = "Please, this field is required.")]
            [RegularExpression("(^[0-9]+)", ErrorMessage = "Please enter only positive numbers")]
            public string Count { get; set; }
            [Required(ErrorMessage = "Please this field is required")]
            [DataType(DataType.DateTime)]
            [PresentOrFutureDate]
            public string expirdate { get; set; }
            [Required(ErrorMessage = "Please this field is required")]
            public string CoponCode { get; set; }
            [Required(ErrorMessage = "Please this field is required")]
            [RegularExpression("([0-9]+)", ErrorMessage = "Please enter only positive numbers")]
            public string Discount { get; set; }
            [Required(ErrorMessage = "Please this field is required")]
            [RegularExpression("([0-9]+)", ErrorMessage = "Please enter only positive numbers")]
            public string limt_discount { get; set; }
            public bool IsActive { get; set; }
        }


    }
}
