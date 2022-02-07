using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApp.webui.Extensions
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T: class
        {//this ITempDataDictionary(tipi) tempData
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            //burada da serilize ettiğimiz bilgiyi disserilize ederek geri almamız gerekiyor

            object o;

            tempData.TryGetValue(key, out o);

            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }
}
