using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WaterDelivery.Helper
{
    public class PresentOrFutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // your validation logic
            if (Convert.ToDateTime(value) > DateTime.Today)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("لا يمكن تحديد تاريخ سابق");
            }
        }
    }
}
