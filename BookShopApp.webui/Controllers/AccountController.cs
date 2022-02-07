using BookShopApp.data.Concrete.EfCore;
using BookShopApp.webui.EmailServices;
using BookShopApp.webui.Extensions;
using BookShopApp.webui.Identity;
using BookShopApp.webui.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Controllers
{
    [AutoValidateAntiforgeryToken]//account ıcındekı butun post metotlarında
    //valida işlemi yerine getirlir
    public class AccountController:Controller
    {
        private EfCoreCartRepository _cartService;

        private UserManager<User> _userManager;//kullanıcı olusturma,login,parola sıfırlama gibi temel metotları barındırıyo

        private SignInManager<User> _signInManager;//cookie işlemlerini yonetır

        private IEmailSender _emailSender;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            this._cartService = new EfCoreCartRepository();
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        [HttpGet]
        public IActionResult Login(string ReturnUrl=null)
        {
            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]//postlara yazılabılır
        //cookılerın gizliligini korumak ıcındır
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //var user = await _userManager.FindByNameAsync(model.UserName);
            var user = await _userManager.FindByEmailAsync(model.Email);
           
            if(user==null)
            {
                ModelState.AddModelError("","Bu kullanıcı adı ile daha önce hesap oluşturulmamış.");
                return View(model);
            }

            if(!await _userManager.IsEmailConfirmedAsync(user))//userın hesabı e mailden onaylanmış mı?
            {
                ModelState.AddModelError("","Lütfen E-mail hesabınıza gelen link ile üyeliğinizi onaylayınız.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user,model.Password,true,false);
            
            if(result.Succeeded)
            {
                return Redirect(model.ReturnUrl??"~/");//??->anlamı nulla esit degilse demektir
                //nulla esit degilse anasayfaya gıt demektır "~/" gosterımı-> Home Index ı ıfade eder
            }

            ModelState.AddModelError("","Girilen kullanıcı E-mail veya parolası yanlış.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user,model.Password);
            if(result.Succeeded)//işlem sonucunda bir kullanıcı olusturulmussa
            {
                await _userManager.AddToRoleAsync(user, "customer");
                //login sayfasına gıtmeden once kullanıcıya bir token olusturulması gerek--generate token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //yukarıda user bilgisi verdik ve verdigimiz bilgiyle bir tane token olusturdu

                var url = Url.Action("ConfirmEmail","Account", new
                {
                    userId = user.Id,
                    token = code
                });
                //email
                //kullanıcıya url i mail olarak gondermelıyız
                await _emailSender.SendEmailAsync(model.Email, "Hesabınızı onaylayınız.", $"Lütfen email hesabınızı onaylamak için linke <a href='https://localhost:5001{url}'>tıklayınız.</a>");

                return RedirectToAction("Login","Account");
                //kullancıya ilgili login sayfasına git
                //ve olusturdugun kullanıcı bılgısıyle bır logın işlemi yap dendi
            }
            ModelState.AddModelError("", "Bilinmeyen bir hata oluştu lütfen tekrar deneyiniz");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();//cookie'yi tarayıcıdan siler

            TempData.Put("message", new AlertMessage()
            {
                Title = "Oturum kapatıldı.",
                Message = "Hesabınız güvenli bir şekilde kapatıldı.",
                AlertType = "success"
            });
            return Redirect("~/");//anasayfaya yonlendırdı
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            //userId bilgisi ve token bilgisi(benzersiz bir sayı) bilgisi gelicek
            //ve bu sayıyı kullanıcıya mail olarak gonderıcez
            //kullanıcı lınke tıkladıgı zaman hesabı onaylanacak
            if(userId==null || token==null)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Geçersiz token.",
                    Message = "Geçersiz token.",
                    AlertType = "danger"
                });
                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);
            
            if(user!=null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    //cart objesini oluştur.
                    _cartService.InitializeCart(user.Id);
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Hesabınız onaylandı.",
                        Message = "Hesabınız onaylandı.",
                        AlertType = "success"
                    });

                    return View();
                }
            }
            
            TempData.Put("message", new AlertMessage()
            {
                Title = "Hesabınız onaylanmadı.",
                Message = "Hesabınız onaylanmadı.",
                AlertType = "warning"
            });
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if(string.IsNullOrEmpty(Email))
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(Email);

            if(user ==null)
            {
                return View();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);//parola resetleme

            var url = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id,
                token = code
            });
            //email
            //kullanıcıya url i mail olarak gondermelıyız
            await _emailSender.SendEmailAsync(Email,"ResetPassword",$"Parolanızı yenilemek için linke <a href='https://localhost:5001{url}'>tıklayınız.</a>");

            TempData.Put("message", new AlertMessage()
            {
                Title = "Şifre değiştirme talebi.",
                Message = "Şifre değiştirme talebiniz mail hesabınıza gönderildi.",
                AlertType = "success"
            });
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if(userId==null || token==null)
            {
                return RedirectToAction("Home","Index");

            }
            var model = new ResetPasswordModel { Token=token };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {

            if(!ModelState.IsValid)//isValidse bir problem yoksa
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user==null)
            {
                return RedirectToAction("Home","Index");
            }

            var result = await _userManager.ResetPasswordAsync(user,model.Token,model.Password);
            // resetleme işlemini var result kısmında yaptık
            TempData.Put("message", new AlertMessage()
            {
                Title = "Şifre değiştirme işlemi.",
                Message = "Şifreniz başarılı bir şekilde değiştirildi.",
                AlertType = "success"
            });

            if (result.Succeeded)
            {
                return RedirectToAction("Login","Account");
            }
            return View(model);
        }

        //public IActionResult AccessDenied()
        //{//erişim reddediliyor
        //    return View();
        //}
    }
}
