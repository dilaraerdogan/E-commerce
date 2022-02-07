using BookShopApp.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopApp.data.Concrete.EfCore
{
    public class EfCoreFavoriteRepository
    {
        public void AddFavorite(Favorite favorite)
        {
            using (var context = new ShopContext())
            {
                var entity = context.Favorites.Where(f => f.UserId == favorite.UserId && f.ProductId == favorite.ProductId).FirstOrDefault();
                if (entity == null)
                {
                    context.Favorites.Add(favorite);
                    context.SaveChanges();
                }
            }
        }

        public void DeleteFavorite(string userId , int productId)
        {
            using (var context = new ShopContext())
            {
                var favorite = context.Favorites.Where(f => f.UserId == userId && f.ProductId == productId).FirstOrDefault();
                context.Favorites.Remove(favorite);
                context.SaveChanges();
            }
        }

        public List<Favorite> GetAllFavorite(string userId)
        {
            using (var context = new ShopContext())
            {
                var favorites = context.Favorites.Where(f => f.UserId == userId).ToList();
                return favorites;
            }
        }
    }
}
