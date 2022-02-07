using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Models
{
    public class CartModel
    {
        public int CartId { get; set; }
        public List<CartItemModel> CartItems { get; set; }

        public double? TotalPrice()
        {
            return CartItems.Sum(i => i.Ucret * i.Quantity);
        }
        public double? OrderPrice()
        {
            return TotalPrice() + kargo();
        }
        public double kargo()
        {
            return 9.99;
        }
    }
        public class CartItemModel
        {
            public int CartItemId { get; set; }
            public int ProductId { get; set; }
            public string KitapAdi { get; set; }
            public double? Ucret { get; set; }
            public string ResimUrl { get; set; }
            public int Quantity { get; set; }
        }
}
