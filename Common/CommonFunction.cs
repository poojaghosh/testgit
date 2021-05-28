using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlockChainPayment.Common
{
    public static class CommonFunction
    {
        public static HttpCookie SetCookie(string cookieName, string cookieValue)
        {
            HttpCookie cookies = new HttpCookie(cookieName);
            cookies.Value = cookieValue;
            cookies.Expires = DateTime.Now.AddHours(1);
            HttpContext.Current.Response.Cookies.Add(cookies);
           return cookies;
        }

        public static string GetCookie(string cookieName)
        {
            string value = string.Empty;

            if (HttpContext.Current.Request.Cookies[cookieName] != null)
                value = HttpContext.Current.Request.Cookies[cookieName].Value;
            return value;
        }

        public static void DeleteCookie(string cookieName)
        {
            if (HttpContext.Current.Request.Cookies[cookieName] != null)
                HttpContext.Current.Response.Cookies[cookieName].Expires=DateTime.Now.AddDays(-1);
        }
    }
}