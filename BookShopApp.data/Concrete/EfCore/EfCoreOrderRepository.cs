using BookShopApp.entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopApp.data.Concrete.EfCore
{
    public class EfCoreOrderRepository: EfCoreGenericRepository<Order,ShopContext>
    {
        public void CreateOrder(Order entity)
        {
            using (var context = new ShopContext())
            {
                context.Orders.Add(entity);
                context.SaveChanges();
            }
        }

        public List<Order> GetOrders(string userId)
        {
            using (var context = new ShopContext())
            {
                var orders = context.Orders
                                    .Include(i => i.OrderItems)
                                    .ThenInclude(i => i.Product)
                                    .AsQueryable();
                if(!string.IsNullOrEmpty(userId))
                {
                    orders = orders.Where(i=>i.UserId == userId);
                }
                return orders.ToList();
            }
        }
    }
}
