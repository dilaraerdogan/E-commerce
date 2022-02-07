using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShopApp.entity
{
    public class Category
    {

        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public int MainCategoryId { get; set; }
        public MainCategory MainCategory { get; set; }
        
        public List<ProductCategory> ProductCategories { get; set; }

    }



}
