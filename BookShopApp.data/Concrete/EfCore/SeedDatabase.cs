using BookShopApp.entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BookShopApp.data.Concrete.EfCore
{
    public static class SeedDatabase
    {
        public static void Seed()
        {
            var context = new ShopContext();

            if (context.Database.GetPendingMigrations().Count() == 0)
            {
                if (context.Categories.Count() == 0)
                {
                    context.Categories.AddRange(Categories);
                }

                if (context.Products.Count() == 0)
                {
                    context.Products.AddRange(Products);
                }
            }
            context.SaveChanges();
        }

        private static Category[] Categories ={

        };

        private static Product[] Products ={


        };

    }
}
