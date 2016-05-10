using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KillPrice
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

            //汉化DX控件 部分Win7计算机可能会出现异常，原因暂时未明
            new DevCHS();
            Application.Run(new FrmMain());
        }

        /// <summary>
        /// 当前应用域内未捕获异常的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string errMsg = string.Format("应用程序出现无法处理的异常，即将退出！\r\n\r\n{0}", ex.Message);
            MessageBox.Show(errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            Environment.Exit(0);
        }

        /// <summary>
        /// 未捕获线程异常的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DisplayName("未处理异常捕获")]
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {

            string errMsg = string.Format("应用程序出现无法处理的线程异常！是否退出？{0}{1}",
                Environment.NewLine, e.Exception.Message);
                errMsg += string.Format("{0}{1}", Environment.NewLine, e.Exception);

            switch (MessageBox.Show(errMsg, "错误", MessageBoxButtons.YesNo, MessageBoxIcon.Stop))
            {
                case DialogResult.Yes:
                    //2010-09-16修改，强行结束当前进程，当模块的构造方法中出现异常时，Environment.Exit不能完全退出，还会有一个线程在运行
                    //Environment.Exit(0);
                    Process.GetCurrentProcess().Kill();
                    break;
                case DialogResult.No:
                default:
                    break;
            }
        }
    }
}
