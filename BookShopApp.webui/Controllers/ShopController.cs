using BookShopApp.data.Concrete.EfCore;
using BookShopApp.entity;
using BookShopApp.webui.Identity;
using BookShopApp.webui.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Controllers
{
    public class ShopController:Controller
    {
        private EfCoreCategoryRepository _categoryService;
        private EfCoreProductRepository _productService;
        private EfCoreMainCategoryRepository _mainCategoryService;
        private UserManager<User> _userManager;
        private EfCoreFavoriteRepository _favoriteService;
        public ShopController(UserManager<User> userManager)
        {
            this._categoryService = new EfCoreCategoryRepository();
            this._productService = new EfCoreProductRepository();
            this._mainCategoryService = new EfCoreMainCategoryRepository();
            this._favoriteService = new EfCoreFavoriteRepository();
            this._userManager = userManager;
        }

        public IActionResult SubList(string maincategory,string category,int page=1)
        {
            var mainCategory = _mainCategoryService.GetCategoryDetails(maincategory);
            const int pageSize = 6;

            if (Convert.ToString(HttpContext.Request.Query["page"]) != null)
            {
                page = Convert.ToInt32(HttpContext.Request.Query["page"]);
            }

            var productSubCategoryViewModel = new ProductSubCategoryListViewModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategory(category),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurrentCategory = category
                },
                Products = _productService.GetProductsByCategory(category, page, pageSize),

                Categories = _categoryService.GetAllCategory(mainCategory.MainCategoryId),

                MainCategories = mainCategory
            };
            ViewBag.MainCategory = maincategory;
            return View(productSubCategoryViewModel);
        }

        public IActionResult List(string maincategory, int page = 1)
        {
            int mainCategoryId = Convert.ToInt32(HttpContext.Request.Query["id"]);

            const int pageSize = 2;

            if (Convert.ToString(HttpContext.Request.Query["page"]) != null)
            {
                page = Convert.ToInt32(HttpContext.Request.Query["page"]);
            }

            var productCategoryViewModel = new ProductCategoryListViewModel()
            {
                /*PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategory(maincategory),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurrentCategory = maincategory
                },*/
                Products = _productService.GetAllMainCategoryProducts(maincategory),

                Categories = _categoryService.GetAllCategory(mainCategoryId)
            };

            ViewBag.MainCategory = maincategory;
            return View(productCategoryViewModel);
        }


        public IActionResult Details(string url)
        {
            if(url == null)
            {
                return NotFound();
            }
            Product product = _productService.GetProductDetails(url);
            
            if(product==null)
            {
                return NotFound();
            }
            return View(new ProductDetailModel{
                Product = product,
                Categories = product.ProductCategories.Select(i=>i.Category).ToList()
            });
        }

        public IActionResult Search(string q)
        {
            var productMainCategoryViewModel = new ProductMainCategoryViewModel()
            {
                Products = _productService.GetSearchResult(q),
                MainCategories = _mainCategoryService.GetAll()
            };
            return View(productMainCategoryViewModel);
        }

        public IActionResult AddToFavorite(int id)
        {
            var userId = _userManager.GetUserId(User);

            var favorite = new Favorite()
            {
                UserId = userId,
                ProductId = id,
            };

            _favoriteService.AddFavorite(favorite);
            return Redirect("/shop/favorites");
        }

        public IActionResult DeleteFavorite(int id)
        {
            var userId = _userManager.GetUserId(User);


            _favoriteService.DeleteFavorite(userId, id);
            return Redirect("/shop/favorites");
        }

        public IActionResult GetFavorite()
        {
            var userId = _userManager.GetUserId(User);

            var favorites = _favoriteService.GetAllFavorite(userId);


            List<Product> productList = new List<Product>();


            foreach (var favorite in favorites)
            {
                var product = _productService.GetById(favorite.ProductId);
                productList.Add(product);
            }

            ProductListViewModel productListView = new ProductListViewModel()
            {
                Products = productList,
            };
            return View("Favorite",productListView);
        }
    }
}
