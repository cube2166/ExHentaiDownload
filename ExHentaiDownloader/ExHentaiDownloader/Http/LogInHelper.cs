using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExHentaiDownloader.Http
{
    public static class LogInHelper
    {
        public static void LogCookieCheck(string cookie)
        {
            string MemberIdRegex = @"ipb_member_id=([^;]*)";
            string PassHashRegex = @"ipb_pass_hash=([^;]*)";
            string IgneousRegex = @"igneous=([^;]*)";
            var MemberIdStr = Regex.Match(cookie, MemberIdRegex);
            var PassHashStr = Regex.Match(cookie, PassHashRegex);
            var IgneousStr = Regex.Match(cookie, IgneousRegex);
            if (!MemberIdStr.Success || !PassHashStr.Success || !IgneousStr.Success)
            {
                throw new CookieException("Cookie Error");
            }
        }

        public static async Task<string> GetLoginCookieAsync(string userName, string passWord)
        {
            string postStr = "UserName=" + userName + "&PassWord=" + passWord + "&x=0&y=0";
            byte[] data = Encoding.UTF8.GetBytes(postStr);
            HttpWebRequest loginRequest = HttpWebRequest.CreateHttp("http://forums.e-hentai.org/index.php?act=Login&CODE=01&CookieDate=1 ");
            loginRequest.Method = "POST";
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            using (Stream stream = await loginRequest.GetRequestStreamAsync())
            {
                stream.Write(data, 0, data.Length);
            }
            string logCookie = "";
            using (HttpWebResponse logResponse = (HttpWebResponse)(await loginRequest.GetResponseAsync()))
            {
                logCookie = logResponse.Headers["Set-Cookie"];
            }
            string MemberIdRegex = @"ipb_member_id=([^;]*)";
            string PassHashRegex = @"ipb_pass_hash=([^;]*)";
            var MemberIdStr = Regex.Match(logCookie, MemberIdRegex);
            var PassHashStr = Regex.Match(logCookie, PassHashRegex);
            if (!MemberIdStr.Success || !PassHashStr.Success)
            {
                throw new LoginException("Login Error");
            }
            HttpWebRequest webRequest = HttpWebRequest.CreateHttp("http://exhentai.org/");
            webRequest.Headers["Cookie"] = MemberIdStr.Value + ";" + PassHashStr.Value;
            string imgCookie = "";
            using (HttpWebResponse webResponse = await webRequest.GetResponseAsync() as HttpWebResponse)
            {
                if (webResponse.ContentType == "image/gif")
                {
                    throw new LogAccessException("No Access");
                }
                imgCookie = webResponse.Headers["Set-Cookie"];
            }
            string igneousRegex = @"igneous=([^;]*)";
            var igneousStr = Regex.Match(imgCookie, igneousRegex);
            return MemberIdStr.Value + ";" + PassHashStr.Value + ";" + igneousStr.Value;
        }
    }
}
