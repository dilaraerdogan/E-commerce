using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Models
{
    public class RegisterModel
    {
     
        [Display(Name = "Firstname", Prompt = "Lütfen isminizi giriniz.")]
        [Required(ErrorMessage = "İsim giriniz zorunlu alan!")]
        public string FirstName { get; set; }

        [Display(Name = "Lastname", Prompt = "Lütfen soyisminizi giriniz.")]
        [Required(ErrorMessage = "Soyisim giriniz zorunlu alan!")]
        public string LastName { get; set; }

        [Display(Name = "Username", Prompt = "Lütfen üyelik için kullanıcı adı giriniz.")]
        [Required(ErrorMessage = "Kullanıcı adı giriniz zorunlu alan!")]
        public string UserName { get; set; }

        [Display(Name = "Password", Prompt = "Parola en az 6 karakterden oluşup büyük-küçük harf içermelidir")]
        [Required(ErrorMessage = "Belirtilen özelliklere göre şifre giriniz zorunlu alan!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "RePassword", Prompt = "Lütfen tekrardan şifrenizi giriniz.")]
        [Required(ErrorMessage = "Tekrar şifre giriniz zorunlu alan!")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string RePassword { get; set; }


        [Display(Name = "Email", Prompt = "Lütfen Email adresinizi giriniz.")]
        [Required(ErrorMessage = "Email adresi giriniz zorunlu alan!")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
