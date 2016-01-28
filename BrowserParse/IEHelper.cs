using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHDocVw;

namespace Hank.BrowserParse
{
    /// <summary>
    /// IE浏览器解析器
    /// </summary>
    public class IEHelper
    {
        /// <summary>
        /// 获取IE全部网址
        /// </summary>
        /// <returns></returns>
        public List<UrlList> MonitorIE()
        {
            try
            {
                ShellWindowsClass shellWindows = new ShellWindowsClass();
                List<UrlList> ieUrls = new List<UrlList>();
                foreach (InternetExplorer ie in shellWindows)
                {
                    ieUrls.Add(new UrlList() { url = ie.LocationURL, title = ie.LocationName });
                }
                return ieUrls;
            }
            catch
            {
                return null;
            }
        }
    }

    public class UrlList
    {
        public string url { set; get; }

        public string title { get; set; }
    }

}
