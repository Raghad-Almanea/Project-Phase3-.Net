using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WaterDelivery.ViewModels;

namespace WaterDelivery.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "من فضلك ادخل البريد الالكتروني")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "من فضلك ادخل بريد الكتروني صحيح")]
        public string Email { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل كلمة المرور")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "من فضلك ادخل البريد الالكتروني")]
        [EmailAddress(ErrorMessage = "يجب ادخال بريد الكتروني صحيح")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل اسم المستخدم")]

        public string FullName { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل صورة المستخدم")]

        [Display(Name = "Img")]
        public string Img { get; set; }
        [Required(ErrorMessage = "من فضلك ادخل كلمة المرور")]
        [StringLength(100, ErrorMessage = "يجب ان تزيد كلمة المرور عن 6 ارقام", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "يرجى التأكد من تطابق كلمتا المرور")]
        public string ConfirmPassword { get; set; }


        public int FkCity { get; set; }


    }

    public class EditViewModel
    {
        [Required(ErrorMessage = "من فضلك ادخل البريد الالكترونى")]
        [EmailAddress(ErrorMessage = "يجب ادخال بريد الكترونى صحيح")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string Id { get; set; }
        [Display(Name = "Img")]
        public string Img { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل  العنوان")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل  رقم الهاتف")]
        [Display(Name = "phone")]
        public string phone { get; set; }

        public string FullName { get; set; }


        //[Required(ErrorMessage = "من فضلك ادخل اسم المستخدم")]
        //public string FullName { get; set; }

        //[Display(Name = "Image")]
        //public string Img { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "يرجى ادخال البريد الالكتروني")]
        [EmailAddress(ErrorMessage = "يجب ادخال بريد الكتروني صحيح")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "يجب ادخال كلمة المرور")]
        [StringLength(100, ErrorMessage = "يجب ان لا تقل كلمة المرور عن 6 ارقام", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "كلمة المرور لا تطابق تأكيد كلمة المرور")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "من فضلك ادخل البريد الالكتروني")]
        [EmailAddress(ErrorMessage = "من فضلك ادخل بريد الكتروني صحيح")]
        [Display(Name = "البريد الالكتروني")]
        public string Email { get; set; }
    }
}
