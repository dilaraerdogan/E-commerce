using BookShopApp.entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Models
{
    public class MainCategoryModel
    {
        public int MainCategoryId { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Kategori için 1-20 aralığında karakter giriniz.")]
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public List<Category> Categories { get; set; }
    }
}
