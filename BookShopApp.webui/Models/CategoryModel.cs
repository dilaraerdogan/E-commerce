using BookShopApp.entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Models
{
    public class CategoryModel
    {

        public int CategoryId { get; set; }

       [Required(ErrorMessage ="Kategori adı zorunludur")]
       [StringLength(20,MinimumLength=3,ErrorMessage ="Kategori için 3-20 aralığında karakter giriniz.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Url adı zorunludur")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Kategori url için 3-20 aralığında karakter giriniz.")]
        public string Url { get; set; }

        public List<Product> Products { get; set; }
        public List<MainCategory> MainCategories { get; set; }
        public int MainCategoryId { get; set; }



    }
}
