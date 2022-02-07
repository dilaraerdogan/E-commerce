using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopApp.entity
{
    public class Product
    {
        public string Yazar { get; set; }
        public int? SayfaSayisi { get; set; }
        public string Dil { get; set; }
        public double? BaskiYili { get; set; }
        public string? Cevirmen { get; set; }
        public string YayınEvi { get; set; }
        public int ProductId { get; set; }
        public string KitapAdi { get; set; }
        public double? Ucret { get; set; }
        public string Url { get; set; }
        public string Icerik{ get; set; }
        public string ResimUrl { get; set; }
        public bool Onay { get; set; }
        public bool IsHome { get; set; }
        public int CategoryId { get; set; }

        public int StokAdedi { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }
    }
}
