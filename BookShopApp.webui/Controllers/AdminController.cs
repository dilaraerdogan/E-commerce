using BookShopApp.data.Concrete.EfCore;
using BookShopApp.entity;
using BookShopApp.webui.Identity;
using BookShopApp.webui.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Controllers
{
    //AdminControllerda Admine bir rol verildigi için
    //AdminControllerı sadece Admin olan kullanıcılar gorebılır
    [Authorize(Roles = "Admin")]
    //[Authorize]
    public class AdminController:Controller
    {
        private EfCoreCategoryRepository _categoryService;
        private EfCoreProductRepository _productService;
        private EfCoreMainCategoryRepository _mainCategoryService;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            this._categoryService = new EfCoreCategoryRepository();
            this._productService = new EfCoreProductRepository();
            this._mainCategoryService = new EfCoreMainCategoryRepository();
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user!=null)
            {
                var selectedRoles = await _userManager.GetRolesAsync(user);
                //kullanıcının rol bilgilerini aldık

                var roles = _roleManager.Roles.Select(i=>i.Name);

                ViewBag.Roles = roles;
                return View(new UserDetailsModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = selectedRoles
                });
            }
            return Redirect("~/admin/user/list");
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserDetailsModel model, string[] selectedRoles)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if(user!=null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.EmailConfirmed = model.EmailConfirmed;

                    var result = await _userManager.UpdateAsync(user);

                    if(result.Succeeded)
                    {
                        //userla alakalı işlemler _userManager üzerinden
                        //ancak bütün veritabanındaki rolleri almak istersek _roleManager üzerinden bilgileri alırız

                        var userRoles = await _userManager.GetRolesAsync(user);
                        selectedRoles = selectedRoles?? new string[]{};
                        await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles).ToArray<string>());
                        //birden fazla kaynak ekleniyor
                        await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles).ToArray<string>());
                        //birden fazla kaynak siliniyor

                        return Redirect("/admin/user/list");
                    }
                }
                return Redirect("/admin/user/list");
            }
            return View(model);
        }

        public IActionResult UserList()
        {
            return View(_userManager.Users);
        }
        public async Task<IActionResult> RoleEdit(string id)
        {
            //gelen role idsine göre veritabanında ilgili id bilgisini sorgulucaz
            //sorguladıktan sonra o role ait olan kullanıcılar ve ait olmayan diğer kullanıcılarıda alıcaz
            //amacı diğer kullanıcılarada ekleme sansı vermek

            var role = await _roleManager.FindByIdAsync(id);

            var members = new List<User>();
            var nonmembers = new List<User>();
            var userList = _userManager.Users.ToList();

            foreach (var item in userList)
            {
                //await _userManager.IsInRoleAsync(user,role.Name); aldıgımız kullanıcının o role ait olup olmadıgını soyluyor(kontrol ediliyor)
                //IsInRoleAsync zaten bool tipi dönüyor
                //daha sonra da eger kullanıcı o role aitse members'a değilse nonmembers a attık.
                //var list = await _userManager.IsInRoleAsync(user,role.Name)
                //                ?members:nonmembers;
                //list.Add(user);
                //yani kullanıcı memberssa members Listesine ekleniyor
                //nonmemberssa nonmembers listesine ekleniyor

                var list = await _userManager.IsInRoleAsync(item, role.Name) ? members : nonmembers;
                list.Add(item);
            }
            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            if(ModelState.IsValid)
            {
                foreach(var userId in model.IdsToAdd ?? new string[] {})
                {//ilgili role eklenecek olan userları başta dolaşıyoruz
                    var user = await _userManager.FindByIdAsync(userId);
                    if(user!=null)
                    {
                        var result = await _userManager.AddToRoleAsync(user,model.RoleName);
                        if(!result.Succeeded)
                        {
                            foreach(var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }

                foreach (var userId in model.IdsToDelete ?? new string[] {})
                {//ilgili role eklenecek olan userları başta dolaşıyoruz
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }
            }
            return Redirect("/admin/role/"+model.RoleId);
        }


        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }

        public IActionResult RoleCreate()
        {
         
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if (ModelState.IsValid)
            {//başarılı iste resultte bir rol oluşturduk
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if(result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }
        public IActionResult ProductList()
        {
            return View(new ProductListViewModel()
            {
                Products = _productService.GetAll()

            }) ;
        }

        public IActionResult CategoryList()
        {
            var categoryModel = new CategoryListViewModel()
            {
                Categories = _categoryService.GetAll(),
                MainCategories = _mainCategoryService.GetAll()
            };
            return View(categoryModel);
        }


        [HttpGet]
        public IActionResult ProductCreate()
        {
            ViewBag.Categories = _categoryService.GetAll();

            return View();
        }
        [HttpPost]
        public IActionResult ProductCreate(ProductModel model)
        {
            if(ModelState.IsValid)
            {
                var entity = new Product()
                {
                    KitapAdi = model.KitapAdi,
                    Cevirmen = model.Cevirmen,
                    Yazar = model.Yazar,
                    YayınEvi = model.YayınEvi,
                    Url = model.Url,
                    Ucret = model.Ucret,
                    Icerik = model.Icerik,
                    ResimUrl = model.ResimUrl,
                    CategoryId = model.CategoryId,
                    SayfaSayisi = model.SayfaSayisi,
                    Onay = model.Onay,
                    Dil = model.Dil,
                    BaskiYili = model.BaskiYili,
                    IsHome = model.AnaSayfa,
                    StokAdedi = model.StokAdedi,
                    
                };
                int[] categoryIds = { model.CategoryId };
                _productService.Create(entity, categoryIds);

                var msg = new AlertMessage()
                {
                    Message = $"{entity.KitapAdi} isimli ürün eklendi.",
                    AlertType = "success"
                };

                TempData["message"] = JsonConvert.SerializeObject(msg);
                return RedirectToAction("ProductList");
            }
            ViewBag.Categories = _categoryService.GetAll();
            return View(model);
        }
        
        [HttpGet]
        public IActionResult CategoryAdd()
        {
            ViewBag.MainCategories = _mainCategoryService.GetAll();
            return View();
        }
        [HttpPost]
        public IActionResult CategoryAdd(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                // Console.WriteLine(category.MainCategoryId);
                var entity = new Category()
                {
                    Name = category.Name,
                    Url = category.Url,
                    MainCategoryId = category.MainCategoryId
                };

                if (entity != null)
                {
                    _categoryService.Create(entity);
                    var msg2 = new AlertMessage()
                    {
                        Message = $"{entity.Name} isimli altcategory eklendi.",
                        AlertType = "success"
                    };

                    TempData["message"] = JsonConvert.SerializeObject(msg2);
                    return RedirectToAction("CategoryList");
                }
                return RedirectToAction("CategoryList");
            }
            ViewBag.MainCategories = _mainCategoryService.GetAll();

            return View(category);
        }

        [HttpGet]
        public IActionResult MainCategoryCreate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult MainCategoryCreate(MainCategoryModel mainCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new MainCategory()
                {
                   Name = mainCategory.Name,
                };

                    if (entity != null)
                    {
                          _mainCategoryService.Create(entity);
                          var msg2 = new AlertMessage()
                          {
                                Message = $"{entity.Name} isimli ustcategory eklendi.",
                                AlertType = "success"
                          };

                          TempData["message"] = JsonConvert.SerializeObject(msg2);

                          return RedirectToAction("CategoryList");
                    }
                 return RedirectToAction("CategoryList");
            }
            return View(mainCategory);
        }

        [HttpGet]
        public IActionResult ProductEdit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var entity = _productService.GetByIdWithCategories((int)id);

            if(entity==null)
            {
                return NotFound();
            }
            var model = new ProductModel()
            {
                ProductId = entity.ProductId,
                KitapAdi = entity.KitapAdi,
                Cevirmen = entity.Cevirmen,
                Yazar = entity.Yazar,
                YayınEvi = entity.YayınEvi,
                Url = entity.Url,
                Ucret = Convert.ToDouble(entity.Ucret),
                Icerik = entity.Icerik,
                ResimUrl = entity.ResimUrl,
                SayfaSayisi = entity.SayfaSayisi,
                BaskiYili = entity.BaskiYili,
                AnaSayfa = entity.IsHome,
                Onay = entity.Onay,
                StokAdedi = entity.StokAdedi,
                SelectedCategories = entity.ProductCategories.Select(i => i.Category).ToList()
            };
            ViewBag.Categories = _categoryService.GetAll();
           
            return View(model);
        }
     
        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductModel model,int[] categoryIds,IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var entity = _productService.GetById(model.ProductId);
                

                if (entity == null)
                {
                    return NotFound();
                }
                entity.KitapAdi = model.KitapAdi;
                entity.Ucret = Convert.ToDouble((model.Ucret/100));
                entity.Url = model.Url;
                entity.Cevirmen = model.Cevirmen;
                entity.YayınEvi = model.YayınEvi;
                entity.Yazar = model.Yazar;
                entity.Icerik = model.Icerik;
                entity.SayfaSayisi = model.SayfaSayisi;
                entity.BaskiYili = model.BaskiYili;
                entity.IsHome = model.AnaSayfa;
                entity.Onay = model.Onay;
                entity.StokAdedi = model.StokAdedi;

                if(file!=null)
                {
                    var extention = Path.GetExtension(file.FileName);
                    var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                    entity.ResimUrl = randomName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\img",randomName);
                
                    using(var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                _productService.Update(entity, categoryIds);

                var msg = new AlertMessage()
                {
                    Message = $"{entity.KitapAdi} isimli ürün güncellendi.",
                    AlertType = "success"
                };

                TempData["message"] = JsonConvert.SerializeObject(msg);

                return RedirectToAction("ProductList");
            }
            ViewBag.Categories = _categoryService.GetAll();
            return View(model);
        }
      
        [HttpGet]
        public IActionResult MainCategoryEdit(int? maincategoryid)
        {
            ViewBag.Categories = _categoryService.GetAll();

            if (maincategoryid == null)
            {
                return NotFound();
            }
            var entity = _mainCategoryService.GetByIdWithCategories((int)maincategoryid);

            if (entity == null)
            {
                return NotFound();
            }
                var model1 = new MainCategoryModel()
                {
                    MainCategoryId = entity.MainCategoryId,
                    Name = entity.Name,
                };
                return View(model1);
        }
     
        [HttpPost]
        public IActionResult MainCategoryEdit(MainCategoryModel mainCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = _mainCategoryService.GetById(mainCategory.MainCategoryId);

                if (entity == null)
                {
                    return NotFound();
                }
                entity.MainCategoryId = mainCategory.MainCategoryId;
                entity.Name = mainCategory.Name;

                _mainCategoryService.Update(entity);

                var msg1 = new AlertMessage()
                {
                    Message = $"{entity.Name} isimli üstcategory güncellendi.",
                    AlertType = "danger"
                };

                TempData["message"] = JsonConvert.SerializeObject(msg1);
                return RedirectToAction("CategoryList");
            }
            return View(mainCategory);
        }


        [HttpGet]
        public IActionResult CategoryEdit(int? categoryId)
        {
            ViewBag.Products = _categoryService.GetByIdWithProducts((int)categoryId);
            if(categoryId == null)
            {
                return NotFound();
            }

            var entity = _categoryService.GetByIdWithProducts((int)categoryId);

            if(entity == null)
            {
                return NotFound();
            }
            var model2 = new CategoryModel()
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Url = entity.Url,
                Products = entity.ProductCategories.Select(p => p.Product).ToList()
            };
            return View(model2);
        }
        [HttpPost]
        public IActionResult CategoryEdit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                var entity2 = _categoryService.GetById(category.CategoryId);

                if (entity2 == null)
                {
                    return NotFound();
                }
                entity2.CategoryId = category.CategoryId;
                entity2.Name = category.Name;
                entity2.Url = category.Url;

                _categoryService.Update(entity2);

                var msg2 = new AlertMessage()
                {
                    Message = $"{entity2.Name} isimli altcategory güncellendi.",
                    AlertType = "danger"
                };

                TempData["message"] = JsonConvert.SerializeObject(msg2);
                return RedirectToAction("CategoryList");
            }
            return View(category);
        }

        public IActionResult DeleteProduct(int productId)
        {
            var entity = _productService.GetById(productId);

            if(entity!=null)
            {
                _productService.Delete(entity);
            }

            var msg = new AlertMessage()
            {
                Message = $"{entity.KitapAdi} isimli ürün silindi.",
                AlertType = "danger"
            };

            TempData["message"] = JsonConvert.SerializeObject(msg);
            return RedirectToAction("ProductList");
        }

        public IActionResult DeleteMainCategory(int mainCategoryId)
        {

            var entity = _mainCategoryService.GetById(mainCategoryId);

                if (entity != null)
                {
                    _mainCategoryService.Delete(entity);
                }
                var msg1 = new AlertMessage()
                {
                    Message = $"{entity.Name} isimli üstcategory silindi.",
                    AlertType = "danger"
                };
                TempData["message"] = JsonConvert.SerializeObject(msg1);
                return RedirectToAction("CategoryList");
        }
       
        public IActionResult DeleteCategory(int categoryId)
        {

            var entity2 = _categoryService.GetById(categoryId);

            if (entity2 != null)
            {
                _categoryService.Delete(entity2);
            }
        
            var msg2 = new AlertMessage()
            {
                Message = $"{entity2.Name} isimli altcategory silindi.",
                AlertType = "danger"
            };
            TempData["message"] = JsonConvert.SerializeObject(msg2);
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteFromCategory(int productId, int categoryId)
        {
            _categoryService.DeleteFromCategory(productId,categoryId);
            return Redirect("/admin/categories/" + categoryId);
        }
    }
}