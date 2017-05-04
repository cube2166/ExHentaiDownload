using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExHentaiDownloader.Http
{
    public static class HttpHandler
    {
        public static async Task TryLog(string username, string password)
        {
            try
            {
                var cookie = await LogInHelper.GetLoginCookieAsync(username, password);
                CookieHelper.SaveCookie(cookie);
            }
            catch (LoginException)
            {
                throw new LoginException("Login Error,Please check your account");
            }
            catch (LogAccessException)
            {
                throw new LoginException("Login Error,Maybe your account have no access to exhentai");
            }
        }

        public async static Task<string> GetStringWithCookie(string uriString, string cookie)
        {
            string returnStr = "";
            HttpWebRequest webRequest = HttpWebRequest.Create(uriString) as HttpWebRequest;
            webRequest.Headers["Cookie"] = cookie;
            using (HttpWebResponse webResponse = await webRequest.GetResponseAsync() as HttpWebResponse)
            {
                using (var getContent = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                {
                    returnStr = await getContent.ReadToEndAsync();
                }
            }
            return returnStr;
        }
    }
}
