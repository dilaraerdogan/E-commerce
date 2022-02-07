using BookShopApp.data.Concrete.EfCore;
using BookShopApp.webui.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShopApp.webui.Controllers
{
    public class HomeController : Controller
    {
        private EfCoreCategoryRepository _categoryService;
        private EfCoreProductRepository _productService;
        private EfCoreMainCategoryRepository _mainCategoryService;
        public HomeController()
        {
            this._categoryService = new EfCoreCategoryRepository();
            this._productService = new EfCoreProductRepository();
            this._mainCategoryService = new EfCoreMainCategoryRepository();
        }
        public IActionResult Index()
        {
            var productMainCategoryViewModel = new ProductMainCategoryViewModel()
            {
                Products = _productService.GetHomePageProducts(),
                MainCategories = _mainCategoryService.GetAllMainCategory()
            };
            return View(productMainCategoryViewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View("MyView");
        }
    }
}
