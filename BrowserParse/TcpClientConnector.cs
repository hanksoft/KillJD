using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Hank.BrowserParse
{
    public class TcpClientConnector
    {
        /// <summary> 
        /// 在指定时间内尝试连接指定主机上的指定端口。 （默认端口：80,默认链接超时：5000毫秒）
        /// </summary> 
        /// <param name= "hostname ">要连接到的远程主机的 DNS 名。</param> 
        /// <param name= "port ">要连接到的远程主机的端口号。 </param> 
        /// <param name= "millisecondsTimeout ">要等待的毫秒数，或 -1 表示无限期等待。</param> 
        /// <returns>已连接的一个 TcpClient 实例。</returns> 
        public static TcpClient Connect(string hostname, int? port, int? millisecondsTimeout)
        {
            ConnectorState cs = new ConnectorState();
            cs.Hostname = hostname;
            cs.Port = port ?? 80;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectThreaded), cs);
            if (cs.Completed.WaitOne(millisecondsTimeout ?? 5000, false))
            {
                if (cs.TcpClient != null) return cs.TcpClient;
                return null;
                //throw cs.Exception;
            }
            else
            {
                cs.Abort();
                return null;
                //throw new SocketException(11001); // cannot connect 
            }
        }

        private static void ConnectThreaded(object state)
        {
            ConnectorState cs = (ConnectorState)state;
            cs.Thread = Thread.CurrentThread;
            try
            {
                TcpClient tc = new TcpClient(cs.Hostname, cs.Port);
                if (cs.Aborted)
                {
                    try { tc.GetStream().Close(); }
                    catch { }
                    try { tc.Close(); }
                    catch { }
                }
                else
                {
                    cs.TcpClient = tc;
                    cs.Completed.Set();
                }
            }
            catch (Exception e)
            {
                cs.Exception = e;
                cs.Completed.Set();
            }
        }

        private class ConnectorState
        {
            public string Hostname;
            public int Port;
            public volatile Thread Thread;
            public readonly ManualResetEvent Completed = new ManualResetEvent(false);
            public volatile TcpClient TcpClient;
            public volatile Exception Exception;
            public volatile bool Aborted;
            public void Abort()
            {
                if (Aborted != true)
                {
                    Aborted = true;
                    try { Thread.Abort(); }
                    catch { }
                }
            }
        }
    }
}
