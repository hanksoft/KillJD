using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace Hank.BrowserParse
{
    /// <summary>
    /// 网络状态监测
    /// </summary>
    public class NetStatusHelper
    {
        /*****************************************************************************************************
         * 调用方式
         * string url = "www.baidu.com;www.sina.com;www.cnblogs.com;www.google.com;www.163.com;www.csdn.com";
         * string[] urls = url.Split(new char[] { ';' });
         * CheckServeStatus(urls);
         *****************************************************************************************************/

        /// <summary>
        /// 检测网络连接状态
        /// </summary>
        /// <param name="urls"></param>
        public static void CheckServeStatus(string[] urls)
        {
            int errCount = 0;//ping时连接失败个数

            if (!LocalConnectionStatus())
            {
                Debug.WriteLine("网络异常~无连接");
            }
            else if (!NetPing(urls, out errCount))
            {
                if ((double)errCount / urls.Length >= 0.3)
                {
                    Debug.WriteLine("网络异常~连接多次无响应");
                }
                else
                {
                    Debug.WriteLine("网络不稳定");
                }
            }
            else
            {
                Debug.WriteLine("网络正常");
            }
        }

        private const int INTERNET_CONNECTION_MODEM = 1;
        private const int INTERNET_CONNECTION_LAN = 2;

        [System.Runtime.InteropServices.DllImport("winInet.dll")]
        private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);

        /// <summary>
        /// 判断本地的连接状态
        /// </summary>
        /// <returns></returns>
        private static bool LocalConnectionStatus()
        {
            System.Int32 dwFlag = new Int32();
            if (!InternetGetConnectedState(ref dwFlag, 0))
            {
                Debug.WriteLine("LocalConnectionStatus--未连网!");
                return false;
            }
            else
            {
                if ((dwFlag & INTERNET_CONNECTION_MODEM) != 0)
                {
                    Debug.WriteLine("LocalConnectionStatus--采用调制解调器上网。");
                    return true;
                }
                else if ((dwFlag & INTERNET_CONNECTION_LAN) != 0)
                {
                    Debug.WriteLine("LocalConnectionStatus--采用网卡上网。");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Ping命令检测网络是否畅通
        /// </summary>
        /// <param name="urls">URL数据</param>
        /// <param name="errorCount">ping时连接失败个数</param>
        /// <returns></returns>
        public static bool NetPing(string[] urls, out int errorCount)
        {
            bool isconn = true;
            Ping ping = new Ping();
            errorCount = 0;
            try
            {
                PingReply pr;
                for (int i = 0; i < urls.Length; i++)
                {
                    pr = ping.Send(urls[i]);
                    if (pr.Status != IPStatus.Success)
                    {
                        isconn = false;
                        errorCount++;
                    }
                    Debug.WriteLine("Ping " + urls[i] + "    " + pr.Status.ToString());
                }
            }
            catch
            {
                isconn = false;
                errorCount = urls.Length;
            }
            //if (errorCount > 0 && errorCount < 3)
            //  isconn = true;
            return isconn;
        }
        /// <summary>
        /// 通过Ping命令获取网络情况
        /// </summary>
        /// <param name="strIp"></param>
        /// <returns></returns>
        private static string CmdPing(string strIp)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";           //设置程序名   
            p.StartInfo.UseShellExecute = false;        //关闭shell的使用   
            p.StartInfo.RedirectStandardInput = true;   //重定向标准输入   
            p.StartInfo.RedirectStandardOutput = true;  //重定向标准输出   
            p.StartInfo.RedirectStandardError = true;   //重定向错误输出   
            p.StartInfo.CreateNoWindow = true;          //不显示窗口   
            string pingrst;
            p.Start();
            p.StandardInput.WriteLine("ping -n 1 " + strIp);    //-n 1 : 向目标IP发送一次请求   
            p.StandardInput.WriteLine("exit");
            string strRst = p.StandardOutput.ReadToEnd();   //命令执行完后返回结果的所有信息   
            if (strRst.IndexOf("(0% loss)") != -1)
            {
                pingrst = "与目标通路";
            }
            else if (strRst.IndexOf("Destination host unreachable.") != -1)
            {
                pingrst = "无法到达目的主机";
            }
            else if (strRst.IndexOf("Request timed out.") != -1)
            {
                pingrst = "超时";
            }
            else if (strRst.IndexOf("not find") != -1)
            {
                pingrst = "无法解析主机";
            }
            else
            {
                pingrst = strRst;
            }
            p.Close();
            return pingrst;
        }        
    }
}
