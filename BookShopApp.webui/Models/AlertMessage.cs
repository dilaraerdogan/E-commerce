using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Models
{
    public class AlertMessage
    {
        public string Title { get; set; }//başlık kısmı
        public string Message { get; set; }//body kısmı
        public string AlertType { get; set; }//css kısmı
    }
}
