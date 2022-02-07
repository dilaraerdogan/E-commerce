using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopApp.entity
{
    public class MainCategory
    {
        [Key]
        public int MainCategoryId { get; set; }
        public string Name { get; set; }

        public List<Category> Categories { get; set; } 
    }
}
