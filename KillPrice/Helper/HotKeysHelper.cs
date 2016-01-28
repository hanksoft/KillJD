using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoHelper
{
    public class HotKeysHelper
    {
        //如果函数执行成功，返回值不为0。
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
        //引入系统API user32.dll是非托管代码，不能用命名空间的方式直接引用，所以需要用“DllImport”进行引入后才能使用
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool RegisterHotKey(
            IntPtr hWnd,    //要定义热键的窗口的句柄
            int id,         //定义热键ID（不能与其它ID重复）
            int modifiers,  //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
            Keys vk         //定义热键的内容
            );

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool UnregisterHotKey(
            IntPtr hWnd,   //要取消热键的窗口的句柄
            int id         //要取消热键的ID
            );


        int keyid = 10;     //区分不同的快捷键
        Dictionary<int, HotKeyCallBackHanlder> keymap = new Dictionary<int, HotKeyCallBackHanlder>();   //每一个key对于一个处理函数
        public delegate void HotKeyCallBackHanlder();

        //定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）
        //组合控制键
        [Flags()]
        public enum HotkeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }

        //注册快捷键
        public void Regist(IntPtr hWnd, int modifiers, Keys vk, HotKeyCallBackHanlder callBack)
        {
            int id = keyid++;
            if (!RegisterHotKey(hWnd, id, modifiers, vk))
                throw new Exception("注册失败！");
            keymap[id] = callBack;
        }

        // 注销快捷键
        public void UnRegist(IntPtr hWnd, HotKeyCallBackHanlder callBack)
        {
            foreach (KeyValuePair<int, HotKeyCallBackHanlder> var in keymap)
            {
                if (var.Value == callBack)
                {
                    UnregisterHotKey(hWnd, var.Key);
                    return;
                }
            }
        }

        // 快捷键消息处理
        public void ProcessHotKey(Message m)
        {
            //如果m.Msg的值为0x0312那么表示用户按下了热键
            if (m.Msg == 0x312)
            {
                int id = m.WParam.ToInt32();
                HotKeyCallBackHanlder callback;
                if (keymap.TryGetValue(id, out callback))
                    callback();
            }

            //const int WM_HOTKEY = 0x0312;
            ////按快捷键 
            //switch (m.Msg)
            //{
            //    case WM_HOTKEY:
            //        switch (m.WParam.ToInt32())
            //        {
            //            case 100:    //按下的是Shift+S
            //                //此处填写快捷键响应代码         
            //                break;
            //            case 101:    //按下的是Ctrl+B
            //                //此处填写快捷键响应代码
            //                break;
            //            case 102:    //按下的是Alt+D
            //                //此处填写快捷键响应代码
            //                break;
            //        }
            //        break;
            //}
        }
    }
}
