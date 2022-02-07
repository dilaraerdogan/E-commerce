using BookShopApp.data.Concrete.EfCore;
using BookShopApp.entity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Models
{
    public class PageInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string CurrentCategory { get; set; }

        public int TotalPages()
        {
            return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
        }
    }

    public class ProductCategoryListViewModel
    {
       
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }

    }
    public class ProductSubCategoryListViewModel
    {
        public PageInfo PageInfo { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public MainCategory MainCategories { get; set; }
    }
}