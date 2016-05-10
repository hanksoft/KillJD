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
        public List<WebSiteModel> MonitorIE()
        {
            try
            {
                ShellWindowsClass shellWindows = new ShellWindowsClass();
                List<WebSiteModel> ieUrls = new List<WebSiteModel>();
                foreach (InternetExplorer ie in shellWindows)
                {
                    ieUrls.Add(new WebSiteModel() { url = ie.LocationURL, title = ie.LocationName });
                }
                return ieUrls;
            }
            catch
            {
                return null;
            }
        }
    }
}
