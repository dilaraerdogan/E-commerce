using BookShopApp.entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopApp.data.Concrete.EfCore
{
    public class EfCoreMainCategoryRepository
    {
        public MainCategory GetById(int id)
        {
            using (var context = new ShopContext())
            {
                return context.MainCategories.Find(id);
            }
        }
        public void Update(MainCategory mainCategory)
        {
            using (var context = new ShopContext())
            {
                context.Entry(mainCategory).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public void Create(MainCategory mainCategory)
        {
            using (var context = new ShopContext())
            {
                context.MainCategories.Add(mainCategory);
                context.SaveChanges();
            }
        }
        public void Delete(MainCategory category)
        {
            using (var context = new ShopContext())
            {
                context.MainCategories.Remove(category);
                context.SaveChanges();
            }
        }
     
        public MainCategory GetByIdWithCategories(int maincategoryId)
        {
            using (var context = new ShopContext())
            {
                return context.MainCategories
                    .Where(i => i.MainCategoryId == maincategoryId)
                    .Include(i => i.Categories)
                    .FirstOrDefault();
            }
        }

        public MainCategory GetByIdWithProducts(int maincategoryId)
        {
            using (var context = new ShopContext())
            {
                return context.MainCategories
                    .Where(i => i.MainCategoryId == maincategoryId)
                    .Include(i => i.Categories)
                    .ThenInclude(i => i.ProductCategories)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefault();
            }
        }
        public MainCategory GetCategoryDetails(string Name)
        {
            using (var context = new ShopContext())
            {
                return context.MainCategories
                    .Where(i => i.Name.ToLower() == Name.ToLower())
                    .FirstOrDefault();
            }

        }
        public List<MainCategory> GetAllMainCategory()
        {
            using (var context = new ShopContext())
            {

                return context.MainCategories
                    .OrderBy(i=>i.MainCategoryId)
                    .ToList();
            }
        }
        public List<MainCategory> GetAll()
        {
            using (var context = new ShopContext())
            {
                return context.MainCategories.OrderBy(i=>i.MainCategoryId).ToList();
            }
        }

    }
}
