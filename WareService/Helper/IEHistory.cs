//-------接口方式-------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;

namespace WareDealer.Helper
{
    public class IEHistory
    {
        private void button1_Click(object sender, EventArgs e)
        {
            IUrlHistoryStg2 vUrlHistoryStg2 = (IUrlHistoryStg2)new UrlHistory();
            IEnumSTATURL vEnumSTATURL = vUrlHistoryStg2.EnumUrls();
            STATURL vSTATURL;
            uint vFectched;
            while (vEnumSTATURL.Next(1, out vSTATURL, out vFectched) == 0)
            {
                //richTextBox1.AppendText(string.Format("{0}:{1}\r\n", vSTATURL.pwcsTitle, vSTATURL.pwcsUrl));
            }
            //vUrlHistoryStg2.ClearHistory();//清除历史
        }
        struct STATURL
        {
            public static uint SIZEOF_STATURL =
                (uint)Marshal.SizeOf(typeof(STATURL));

            public uint cbSize;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pwcsUrl;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pwcsTitle;
            public FILETIME ftLastVisited,
                ftLastUpdated,
                ftExpires;
            public uint dwFlags;
        }

        [ComImport, Guid("3C374A42-BAE4-11CF-BF7D-00AA006946EE"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IEnumSTATURL
        {
            [PreserveSig]
            uint Next(uint celt, out STATURL rgelt, out uint pceltFetched);
            void Skip(uint celt);
            void Reset();
            void Clone(out IEnumSTATURL ppenum);
            void SetFilter(
                [MarshalAs(UnmanagedType.LPWStr)] string poszFilter,
                uint dwFlags);
        }

        [ComImport, Guid("AFA0DC11-C313-11d0-831A-00C04FD5AE38"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IUrlHistoryStg2
        {
            #region IUrlHistoryStg methods
            void AddUrl(
                [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
                [MarshalAs(UnmanagedType.LPWStr)] string pocsTitle,
                uint dwFlags);

            void DeleteUrl(
                [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
                uint dwFlags);

            void QueryUrl(
                [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
                uint dwFlags,
                ref STATURL lpSTATURL);

            void BindToObject(
                [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
                ref Guid riid,
                [MarshalAs(UnmanagedType.IUnknown)] out object ppvOut);

            IEnumSTATURL EnumUrls();
            #endregion

            void AddUrlAndNotify(
                [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
                [MarshalAs(UnmanagedType.LPWStr)] string pocsTitle,
                uint dwFlags,
                [MarshalAs(UnmanagedType.Bool)] bool fWriteHistory,
                [MarshalAs(UnmanagedType.IUnknown)] object /*IOleCommandTarget*/
                poctNotify,
                [MarshalAs(UnmanagedType.IUnknown)] object punkISFolder);

            void ClearHistory();
        }

        [ComImport, Guid("3C374A40-BAE4-11CF-BF7D-00AA006946EE")]
        class UrlHistory /* : IUrlHistoryStg[2] */ { }
    }
}
