using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopApp.entity
{
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }
        public string UserId { get;  set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}
