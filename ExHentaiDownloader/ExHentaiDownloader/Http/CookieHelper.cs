using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExHentaiDownloader.Http
{
    public static class CookieHelper
    {
        public static string GetCookie()
        {
            return File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "cookie.cookie") + ParseHelper.unconfig;
        }

        public static void SaveCookie(string cookie)
        {
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "cookie.cookie", cookie);
        }

    }
}
