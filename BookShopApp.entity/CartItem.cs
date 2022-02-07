using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopApp.entity
{
    //bir kart içerisinde birden fazla ürün eklenebilir
    //ve eklenen her bir üründe cartıtem ıcerısınde ki
    //tek bir kayıta karşılık gelir.
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        //hangi ürünün alındıgını
        public Product Product { get; set; }
        public Cart Cart { get; set; }
        public int CartId { get; set; }
        //hangi kulllanıcı oldugunu gosterıcek
        public int Quantity { get; set; }
        //kac adet alındıgını gosterıcek

    }
}
