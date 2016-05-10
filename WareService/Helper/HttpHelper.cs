using System;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Hank.ComLib;

namespace WareDealer.Helper
{
    public class HttpHelper
    {
        public static CookieContainer CookieContainers = new CookieContainer();

        public static string FireFoxAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2.23) Gecko/20110920 Firefox/3.6.23";
        public static string IE7 = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; InfoPath.2; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET4.0C; .NET4.0E)";

        public static string GetResponseGBK(string url, string method, string data)
        {
            return GetResponse(url, method, data, Encoding.GetEncoding("gb2312"));
        }

        public static string GetResponseUTF(string url, string method, string data)
        {
            return GetResponse(url, method, data, Encoding.GetEncoding("UTF-8"));
        }

        /// <summary>
        /// 获取网页数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method">"POST" or "GET"</param>
        /// <param name="data">when the method is "POST", the data will send to web server, if the method is "GET", the data should be string.empty</param>
        /// <returns></returns>
        public static string GetResponse(string url, string method, string data, Encoding encode)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.KeepAlive = true;
                req.Method = method.ToUpper();
                req.AllowAutoRedirect = true;
                req.CookieContainer = CookieContainers;
                req.ContentType = "application/x-www-form-urlencoded";

                req.UserAgent = IE7;
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.Timeout = 50000;

                if (method.ToUpper() == "POST" && data != null)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] postBytes = encoding.GetBytes(data); ;
                    req.ContentLength = postBytes.Length;
                    Stream st = req.GetRequestStream();
                    st.Write(postBytes, 0, postBytes.Length);
                    st.Close();
                }

                System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
                {
                    return true;
                };

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Stream resst = res.GetResponseStream();
                StreamReader sr = new StreamReader(resst, encode);
                string str = sr.ReadToEnd();

                return str;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                return string.Empty;
            }
        }

        public static Stream GetResponseImage(string url)
        {
            Stream resst = null;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.KeepAlive = true;
                req.Method = "GET";
                req.AllowAutoRedirect = true;
                req.CookieContainer = CookieContainers;
                req.ContentType = "application/x-www-form-urlencoded";

                req.UserAgent = IE7;
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.Timeout = 50000;

                System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
                {
                    return true;
                };

                Encoding myEncoding = Encoding.GetEncoding("UTF-8");

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                resst = res.GetResponseStream();

                return resst;
            }
            catch
            {
                return null;
            }
        }

        public static string GetRegexString(string pattern, string source)
        {
            Regex r = new Regex(pattern);
            MatchCollection mc = r.Matches(source);
            return mc[0].Groups[1].Value;
        }

        public static string[] GetRegexStrings(string pattern, string source)
        {
            Regex r = new Regex(pattern);
            MatchCollection mcs = r.Matches(source);

            string[] ret = new string[mcs.Count];

            for (int i = 0; i < mcs.Count; i++)
                ret[i] = mcs[i].Groups[1].Value;

            return ret;
        }

        //public static string[] GetURLs()
        //{
        //    try
        //    {
        //        string s = "";
        //        SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
        //        foreach (SHDocVw.InternetExplorer ie in shellWindows)
        //        {
        //            s = ie.LocationURL;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return null;
        //    }
        //}

        //public static string[] GetUrlsExt()
        //{
        //    ShellWindows shellwindows = new ShellWindows();

        //    foreach (InternetExplorer ie in shellwindows)
        //    {
        //        string filename = Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
        //        if (filename.Equals("iexplore"))
        //        {
        //            if (txt_ReceiveUrl.Text != ie.LocationURL.ToString())
        //            {
        //                txt_ReceiveUrl.Text = ie.LocationURL.ToString();
        //                txt_pageTitle.Text = ie.FullName.ToString();

        //            }
        //        }
        //    }
        //}
    }
}
