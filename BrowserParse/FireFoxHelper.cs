using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDde.Client;

namespace Hank.BrowserParse
{
    /// <summary>
    /// 火狐浏览器解析
    /// </summary>
    public class FireFoxHelper
    {
        /// <summary>
        /// 开始监控火狐浏览器
        /// </summary>
        public List<WebSiteModel> MnitorFireFox()
        {
            try
            {
                string sUrl = string.Empty;
                string sTitle = string.Empty;
                DdeClient dde = new DdeClient("Firefox", "WWW_GetWindowInfo");
                dde.Connect();
                // 取得 URL 資訊
                string sUrlInfo = dde.Request("URL", int.MaxValue);
                // DDE Client 進行連結 
                dde.Disconnect();

                List<WebSiteModel> urls = new List<WebSiteModel>();

                // 取得的 sUrlInfo 內容為 "網址","標題",""
                // 取出網址部分
                if (sUrlInfo.Length > 0)
                {
                    //sUrlInfo.Split(',').ToList<>();
                    sUrl = sUrlInfo.Split(',')[0].Trim(new char[] { '"' });
                    sTitle = sUrlInfo.Split(',')[1].ToString();
                    urls.Add(new WebSiteModel() { url = sUrl, title = sTitle });
                }
                return urls;
            }
            catch
            {
                return null;
            }
        }
    }
}
