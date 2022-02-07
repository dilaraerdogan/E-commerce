using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Models
{
    public class FavoriteModel
    {
        public int FavoriteId { get; set; }
        public string UserId { get; set; }

        public int ProductId { get; set; }
    }
}
