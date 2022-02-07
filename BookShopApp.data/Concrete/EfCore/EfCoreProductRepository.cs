using BookShopApp.entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopApp.data.Concrete.EfCore
{
    public class EfCoreProductRepository 
    {
        public List<Product> GetAll()
        {
            using(var context = new ShopContext())
            {
                return context.Products.OrderBy(i=>i.ProductId).ToList();
            }
        }
        public List<Product> GetAllMainCategoryProducts(string mainCategory)
        {
            using (var context = new ShopContext())
            {
                  var products = context
                           .Products
                           .Where(i => i.Onay)
                           .OrderBy(i=>i.ProductId)
                           .AsQueryable();

            if (!string.IsNullOrEmpty(mainCategory))
            {
                products = products
                            .Include(i => i.ProductCategories)
                            .ThenInclude(i => i.Category)
                            .ThenInclude(i=>i.MainCategory)
                            .OrderBy(i => i.ProductId)
                            .Where(i => i.ProductCategories.Any(a => a.Category.MainCategory.Name.ToLower() == mainCategory));
            }
            return products.ToList();
            }  
        }
        public Product GetByIdWithCategories(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Products
                              .Where(i => i.ProductId == id)
                              .Include(i => i.ProductCategories)
                              .ThenInclude(i => i.Category)
                              .FirstOrDefault(); 
            }
        }
        public void Create(Product product,int[] categoryIds) 
        {
            using (var context = new ShopContext())
            {
                context.Products.Add(product);
                context.SaveChanges();
                product.ProductCategories = categoryIds.Select(catid => new ProductCategory()
                {
                    ProductId = product.ProductId,
                    CategoryId = catid
                }).ToList();
                context.SaveChanges();
            }
        }

        public void EditQuantity(int productId,int quantity)
        {
            using(var context  = new ShopContext())
            {
                var product = context.Products.Find(productId);

                product.StokAdedi -= quantity;
                
                context.SaveChanges();
            }

        }

        public void Delete(Product product)
        {
            using (var context = new ShopContext())
            {
                context.Products.Remove(product);
                context.SaveChanges();
            }
        }
        public Product GetById(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Products.Find(id);
            }
        }
        public int GetCountByCategory(string category)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products.Where(i=>i.Onay).AsQueryable();

                if (!string.IsNullOrEmpty(category))
                {
                    products = products
                                .Include(i => i.ProductCategories)
                                .ThenInclude(i => i.Category)
                                .Where(i => i.ProductCategories.Any(a => a.Category.Url.ToLower() == category.ToLower()));             
                } 
                return products.Count();
            }
        }
        public List<Product> GetHomePageProducts()
        {
            using (var context = new ShopContext())
            {
                return context.Products
                    .Where(i=>i.Onay && i.IsHome==true)
                    .Take(24)
                    .ToList();
            }
        }
        public List<Product> GetPopularProducts()
        {
            using (var context = new ShopContext())
            {
                return context.Products.ToList();
            }
        }

        public Product GetProductDetails(string url)
        {
            using (var context = new ShopContext())
            {
                return context.Products
                    .Where(i => i.Url == url)
                    .Include(i => i.ProductCategories)
                    .ThenInclude(i => i.Category)
                    .FirstOrDefault();
            }
        }
        public List<Product> GetProductsByCategory(string name,int page,int pageSize)
        {
            using (var context = new ShopContext())
            {
               var products = context
                    .Products
                    .Where(i=>i.Onay)
                    .OrderBy(i=>i.ProductId)
                    .AsQueryable();

                if(!string.IsNullOrEmpty(name))
                {
                    products = products
                                .Include(i => i.ProductCategories)
                                .ThenInclude(i => i.Category)
                                .OrderBy(i => i.ProductId)                            
                                .Where(i => i.ProductCategories.Any(a => a.Category.Url.ToLower() == name.ToLower()));
                }
                return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
        }
        public List<Product> GetSearchResult(string searchString)
        {
            using (var context = new ShopContext())
            {
                var products = context
                    .Products
                    .Where(i => i.Onay && (i.KitapAdi.ToLower().Contains(searchString.ToLower()) || i.Cevirmen.ToLower().Contains(searchString.ToLower()) || i.YayınEvi.ToLower().Contains(searchString.ToLower())) || i.Yazar.ToLower().Contains(searchString.ToLower()))
                    .AsQueryable();
                return products.ToList();
            }
        }
        public void Update(Product entity, int[] categoryIds)
        {
            using (var context = new ShopContext())
            {
                var product = context.Products
                    .Include(i => i.ProductCategories)
                    .FirstOrDefault(i => i.ProductId == entity.ProductId);
                if(product!=null)
                {
                    product.KitapAdi = entity.KitapAdi;
                    product.Ucret = entity.Ucret;
                    product.Icerik = entity.Icerik;
                    product.Url = entity.Url;
                    product.Yazar = entity.Yazar;
                    product.YayınEvi = entity.YayınEvi;
                    product.Cevirmen = entity.Cevirmen;
                    product.ResimUrl = entity.ResimUrl;
                    product.StokAdedi = entity.StokAdedi;

                    product.ProductCategories = categoryIds.Select(catid => new ProductCategory()
                    {
                        ProductId=entity.ProductId,
                        CategoryId= catid
                    }).ToList();
                    context.SaveChanges();
                }
            }
        }
    }
}