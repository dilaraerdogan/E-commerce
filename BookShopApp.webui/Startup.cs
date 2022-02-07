using BookShopApp.data.Concrete.EfCore;
using BookShopApp.webui.EmailServices;
using BookShopApp.webui.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationContext>(options => options.UseNpgsql("Host=queenie.db.elephantsql.com;Port=5432;Database=oeyepkpj;Username=oeyepkpj;Password=LwmMnUtIToQ3RWhLr_F38xzDtLjfqGqq"));
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
           
            services.Configure<IdentityOptions>(options =>{
                //password
                options.Password.RequireDigit = true;//RequireDigit true dedigimizde mutlaka passwordde sayısal bır deger olmalı kuralı getırılmıs olur
                options.Password.RequireLowercase = true;//parolada mutlaka kucuk harf olmalı kuralı
                options.Password.RequireUppercase = true;//büyük harf
                options.Password.RequiredLength = 6;//parolanın minimum uzunlugu 6 karakter
                options.Password.RequireNonAlphanumeric = false;

                //Lockout
                options.Lockout.MaxFailedAccessAttempts = 5;//kullanıcı en fazla 5 kere giriş hatası yapabılsın
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);//giriş için en fazla 5 dk süre
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;//her kullanıcının birbirinden farklı bir mail hesabı olsun kuralı
                options.SignIn.RequireConfirmedEmail = true;//kullanıcı uye olduktan sonra mutlaka hesabını onaylaması gerekır kuralı
                options.SignIn.RequireConfirmedPhoneNumber = false;//telefon sms onayı



            });

            //cookie:kullanıcının tarayıcısında uygulama tarafından bırakılan bır bılgı
            //yani kullanıcı tekrar girdiginde uygulamayı tanımasını saglayan bılgıler
            services.ConfigureApplicationCookie(options=>{
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;//kullanıcı istek yapmadıgı surece 20 dk ızın verır uygulamada kalmasına
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name = ".BookShop.Security.Cookie",
                    SameSite = SameSiteMode.Strict
                };

            });

            services.AddScoped<IEmailSender, SmtpEmailSender>(i=>
                new SmtpEmailSender(
                    _configuration["EmailSender:Host"],
                    _configuration.GetValue<int>("EmailSender:Port"),
                    _configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    _configuration["EmailSender:UserName"],
                    _configuration["EmailSender:Password"])
                );

            services.AddControllersWithViews();
        } 
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            if (env.IsDevelopment())
            {
                SeedDatabase.Seed();
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "orders",
                pattern: "orders",
                defaults: new { controller = "Cart", action = "GetOrders" }
                );
                
                endpoints.MapControllerRoute(
                name: "checkout",
                pattern: "checkout",
                defaults: new { controller = "Cart", action = "Checkout" }
                );

                endpoints.MapControllerRoute(
                   name: "cart",
                   pattern: "cart",
                   defaults: new { controller = "Cart", action = "Index" }
                );

              
                endpoints.MapControllerRoute(
                   name: "adminusers",
                   pattern: "admin/user/list",
                   defaults: new { controller = "Admin", action = "UserList" }
                );

                endpoints.MapControllerRoute(
                    name: "adminuseredit",
                    pattern: "admin/user/{id?}",
                    defaults: new { controller = "Admin", action = "UserEdit" }
                );

                endpoints.MapControllerRoute(
                    name: "adminroles",
                    pattern: "admin/role/list",
                    defaults: new { controller = "Admin", action = "RoleList" }
                );

                endpoints.MapControllerRoute(
                   name: "adminrolecreate",
                   pattern: "admin/role/create",
                   defaults: new { controller = "Admin", action = "RoleCreate" }
                );

                endpoints.MapControllerRoute(
                name: "adminroleedit",
                pattern: "admin/role/{id?}",
                defaults: new { controller = "Admin", action = "RoleEdit" }
                );

                endpoints.MapControllerRoute(
                   name: "adminproducts",
                   pattern: "admin/products",
                   defaults: new { controller = "Admin", action = "ProductList" }
               );

                endpoints.MapControllerRoute(
                   name: "adminproductcreate",
                   pattern: "admin/products/create",
                   defaults: new { controller = "Admin", action = "ProductCreate" }
                ); 
                
                endpoints.MapControllerRoute(
                   name: "adminproductedit",
                   pattern: "admin/products/{id?}",
                   defaults: new { controller = "Admin", action = "ProductEdit" }
               );

                endpoints.MapControllerRoute(
                   name: "admincategories",
                   pattern: "admin/categories",
                   defaults: new { controller = "Admin", action = "CategoryList" }
                );

                endpoints.MapControllerRoute(
                  name: "admincategorycreate",
                  pattern: "admin/categories/create",
                  defaults: new { controller = "Admin", action = "CategoryAdd" }
              );

                endpoints.MapControllerRoute(
                   name: "admincategorycreate",
                   pattern: "admin/maincategories/create",
                   defaults: new { controller = "Admin", action = "MainCategoryCreate" }
               );

                endpoints.MapControllerRoute(
                  name: "admincategoryedit",
                  pattern: "admin/maincategories/{maincategoryid?}",
                  defaults: new { controller = "Admin", action = "MainCategoryEdit" }
              );

                endpoints.MapControllerRoute(
                   name: "admincategoryedit",
                   pattern: "admin/categories/{categoryid?}",
                   defaults: new { controller = "Admin", action = "CategoryEdit" }
               );
                endpoints.MapControllerRoute(
                name: "shop",
                pattern: "shop/deletefavorite/{id?}",
                defaults: new { controller = "Shop", action = "DeleteFavorite" }
               );
                endpoints.MapControllerRoute(
                name: "shop",
                pattern: "shop/addfavorite/{id?}",
                defaults: new { controller = "Shop", action = "AddToFavorite" }
               );

                endpoints.MapControllerRoute(
                 name: "shop",
                 pattern: "shop/favorites",
                 defaults: new { controller = "Shop", action = "GetFavorite" }
                );

                endpoints.MapControllerRoute(
                  name: "shop",
                  pattern: "shop/{mainCategory?}",
                  defaults: new { controller = "Shop", action = "list" }
               );

                //localhost/search 
                endpoints.MapControllerRoute(
                  name: "shop",
                  pattern: "shop/{mainCategory}/{category}",
                  defaults: new { controller = "Shop", action = "sublist" }
               );

                endpoints.MapControllerRoute(
                   name: "search",
                   pattern: "search",
                   defaults: new { controller = "Shop", action = "search" }
                );

                endpoints.MapControllerRoute(
                   name: "productdetails",
                   pattern: "{url}",
                   defaults: new { controller = "Shop", action = "details" }
                );


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
