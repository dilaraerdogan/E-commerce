using BookShopApp.entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BookShopApp.data.Concrete.EfCore
{
    public class EfCoreCategoryRepository 
    {
        public void Create(Category category)
        {
            using (var context = new ShopContext())
            {
                context.Categories.Add(category);
                context.SaveChanges();
            }
        }
        public void Delete(Category category)
        {
            using (var context = new ShopContext())
            {
                context.Categories.Remove(category);
                context.SaveChanges();
            }
        }
        public List<Category> GetAll()
        {
            using (var context = new ShopContext())
            {
                return context.Categories.OrderBy(i=>i.CategoryId).ToList();
            }
        }

        public Category GetById(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Categories.Find(id);
            }
        }
        public void Update(Category category)
        {
            using (var context = new ShopContext())
            {
                context.Entry(category).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public void DeleteFromCategory(int productId, int categoryId)
        {
            using (var context = new ShopContext())
            {
                var cmd = "delete from \"ProductCategory\" where \"ProductId\" = @p0 and \"CategoryId\" = @p1";
                context.Database.ExecuteSqlRaw(cmd,productId,categoryId);
            }
        }
        public List<Category> GetAllCategory(int id){
            using(var context = new ShopContext()){
                return context.Categories
                       .Where(i=>i.MainCategoryId == id )
                       .OrderBy(i=>i.CategoryId)
                       .ToList();
            }
        }
        public List<Category> CategoriyeGore(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Categories
                   .Where(i => i.CategoryId == id)
                   .OrderBy(i => i.CategoryId)
                   .ToList();
            }
        }
        public Category GetByIdWithProducts(int categoryId)
        {
            using (var context = new ShopContext())
            {
                return context.Categories
                    .Where(i => i.CategoryId == categoryId)
                    .Include(i => i.ProductCategories)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefault();
            }
        }
    }
}
