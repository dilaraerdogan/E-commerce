using BookShopApp.entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Models
{
    public class ProductModel
    {
        public string Yazar { get; set; }
        public string? Cevirmen { get; set; }

        [Required(ErrorMessage = "Yayınevi zorunlu bir alan.")]
        public string YayınEvi { get; set; }
        public int ProductId { get; set; }

        [Display(Name="KitapAdi", Prompt = "Kitap adını giriniz")]
        [Required(ErrorMessage ="Ürün adı zorunlu alan.")]
        [StringLength(60,MinimumLength=3, ErrorMessage ="Kitap ismi 3-60 karakter aralığı olmalıdır")]
        public string KitapAdi { get; set; }

        [Display(Name = "Url", Prompt = "Ürünün url'ini giriniz")]
        [Required(ErrorMessage = "Kitap url zorunlu bir alan.")]
        public string Url { get; set; }


        [Display(Name = "Ucret", Prompt = "Ürün ücretini giriniz")]
        [Required(ErrorMessage = "Kitap ücreti zorunlu bir alan.")]
        [Range(1,10000, ErrorMessage ="1-1000 arasında değer girebilirsiniz")]
        public double? Ucret { get; set; }



        [Display(Name = "Icerik", Prompt = "Ürün içeriğini giriniz")]
        public string Icerik { get; set; }



        [Display(Name = "SayfaSayisi", Prompt = "Ürünün sayfa sayısını giriniz")]
        public int? SayfaSayisi { get; set; }


        [Display(Name = "BaskiYili", Prompt = "Ürün baskı yılını giriniz")]
        [Required(ErrorMessage = "Baskı yılı zorunlu bir alan.")]
        [Range(1800, 2021, ErrorMessage = "1800-2021 arasında değer girebilirsiniz")]
        public double? BaskiYili { get; set; }


        [Display(Name = "Dil", Prompt = "Ürünün yazıldığı dili giriniz")]
        public string Dil { get; set; }


        [Required(ErrorMessage = "Kitap resmi zorunlu bir alan.")]
        public string ResimUrl { get; set; }


        public bool IsHome { get; set; }

        public bool IsApproved { get; set; }
        public bool Onay { get; set; }
        public bool AnaSayfa { get; set; }

        public int CategoryId { get; set; }

        public int StokAdedi { get; set; }

        public List<Category> SelectedCategories { get; set; }
      
    }
}
