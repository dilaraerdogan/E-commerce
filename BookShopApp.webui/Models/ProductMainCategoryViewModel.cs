using BookShopApp.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace BookShopApp.webui.Models
{
    public class ProductMainCategoryViewModel
    {
        public List<Product> Products { get; set; }
        public List<MainCategory> MainCategories { get; set; }
    }
}