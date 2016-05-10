using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WareDealer.Helper;

namespace WareDealer.Mode
{
    /// <summary>
    /// 系统参数类
    /// </summary>
    public static class SysParams
    {
        #region SysBase Params
        /// <summary>
        /// 用户名
        /// </summary>
        public static string UserName { get; set; }

        private static string _pass;
        /// <summary>
        /// 用户密码
        /// </summary>
        public static string UserPass
        {
            get
            {
                try
                {
                    string _value = !string.IsNullOrEmpty(_pass) ? PassWordHelper.GetInstance().UnDesStr(_pass, "isarahan", "wolfstud") : "123456";
                    return _value;
                }
                catch (Exception)
                {
                    return "";
                }

            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _pass = value;
                }
            }
        }

        private static string _jdUser;
        /// <summary>
        /// 京东用户名
        /// </summary>
        public static string JDUserName
        {
            get { return _jdUser; }
            set 
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _jdUser = value; 
                }
                else
                {
                    _jdUser = "killjd.cn";
                }
                
            }
        }

        private static string _jdPass;
        /// <summary>
        /// 京东用户密码
        /// </summary>
        public static string JDUserPass 
        {
            get 
            {
                try
                {
                    string _value = !string.IsNullOrEmpty(_jdPass) ? PassWordHelper.GetInstance().UnDesStr(_jdPass, "isarahan", "wolfstud") : "123456";
                    return _value;
                }
                catch (Exception)
                {
                    return "";
                }
                
            }
            set 
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _jdPass = value;
                }
            }
        }
        /// <summary>
        /// 在系统启动时运行
        /// </summary>
        public static bool AllowRunAtSystemStart { get; set; }
        
        /// <summary>
        /// 检查本机网络连接情况
        /// </summary>
        public static bool CheckNetState { get; set; }
        /// <summary>
        /// 产品版本号
        /// </summary>
        public static string ProductVersion { get; set; }
        /// <summary>
        /// 自动更新程序
        /// </summary>
        /// <remarks>默认在程序启动时获取新版本，需与网站接口</remarks>
        public static bool AllowAutoUpdateVersion { get; set; }
        /// <summary>
        /// 软件是否要求进行登录验证
        /// </summary>
        public static bool AllowLoginVerify { get; set; }
        /// <summary>
        /// 商品配送区域
        /// </summary>
        public static string DispathArea { get; set; }
        /// <summary>
        /// 一级城市
        /// </summary>
        public static string Level1Area { get; set; }
        /// <summary>
        /// 二级城市
        /// </summary>
        public static string Level2Area { get; set; }
        /// <summary>
        /// 三级城市
        /// </summary>
        public static string Level3Area { get; set; }
        #endregion SysBase Params

        #region SysExt Params
        /// <summary>
        /// 自动获取浏览器页面
        /// </summary>
        public static bool AllowAutoGetWebUrl { get; set; }
        /// <summary>
        /// 自动获取关注商品
        /// </summary>
        public static bool AllowAutoGetFocusWare { get; set; }

        private static string _focusTime = "60";
        /// <summary>
        /// 获取关注商品心跳时长 (单位：秒)
        /// </summary>
        /// <remarks>5分钟 10分钟 60分钟 4小时 每天</remarks>
        public static string AutoGetFocusTime
        {
            get 
            {
                return _focusTime;
            }
            set
            {
                switch (value)
                {
                    case "5分钟":
                        _focusTime = (5 * 60).ToString();
                        break;
                    case "10分钟":
                        _focusTime = (10 * 60).ToString();
                        break;
                    case "30分钟":
                        _focusTime = (30 * 60).ToString();
                        break;
                    case "60分钟":
                        _focusTime = (60 * 60).ToString();
                        break;
                    case "4小时":
                        _focusTime = (4 * 60 * 60).ToString();
                        break;
                    case "每天":
                        _focusTime = (24 * 60 * 60).ToString();
                        break;
                    default:
                        _focusTime = value;
                        break;
                }
            }
        }
        /// <summary>
        /// 下柜商品自动进入回收站
        /// </summary>
        /// <remarks>Garbage Collection</remarks>
        public static bool AllowAutoGC { get; set; }
        #endregion SysExt Params

        #region Gather Params
        /// <summary>
        /// 采集模式
        /// </summary>
        /// <remarks>在批量采集模式下，才对采集参数进行响应</remarks>
        public static GatherType GatherModel { get; set; }
        //public static 
        /// <summary>
        /// 采集京东自营商品
        /// </summary>
        public static bool GatherJDWare { get; set; }
        /// <summary>
        /// 采集旗舰店商品
        /// </summary>
        public static bool GatherQJWare { get; set; }
        /// <summary>
        /// 采集第三方商品
        /// </summary>
        public static bool Gather3Ware { get; set; }
        /// <summary>
        /// 采集商品基本信息
        /// </summary>
        public static bool GetWareBaseInfo { get; set; }
        /// <summary>
        /// 采集商品价格
        /// </summary>
        public static bool GetWarePrice { get; set; }
        /// <summary>
        /// 采集商品库存
        /// </summary>
        public static bool GetWareStock { get; set; }
        /// <summary>
        /// 采集商品促销信息
        /// </summary>
        public static bool GetWareCoupon { get; set; }
        /// <summary>
        /// 采集商品评价
        /// </summary>
        public static bool GetWarePostMessage { get; set; }
        /// <summary>
        /// 采集商品海报
        /// </summary>
        public static bool GetWarePicture { get; set; }

        #endregion Gather Params

        #region Monitor Params
        /// <summary>
        /// 自动更新商品信息（价格、库存情况）
        /// </summary>
        public static bool AllowAutoUpdateWareInfo { get; set; }

        private static string _updateTime = "1800";
        /// <summary>
        /// 自动更新商品时长
        /// </summary>
        /// <remarks>5分钟\10分钟\30分钟\60分钟\3小时\每天</remarks>
        public static string AutoUpdateTime
        {
            get
            {
                return _updateTime;
            }
            set
            {
                switch (value)
                {
                    case "5分钟":
                        _updateTime = (5 * 60).ToString();
                        break;
                    case "10分钟":
                        _updateTime = (10 * 60).ToString();
                        break;
                    case "30分钟":
                        _updateTime = (30 * 60).ToString();
                        break;
                    case "60分钟":
                        _updateTime = (60 * 60).ToString();
                        break;
                    case "3小时":
                        _updateTime = (3 * 60 * 60).ToString();
                        break;
                    case "每天":
                        _updateTime = (24 * 60 * 60).ToString();
                        break;
                    default:
                        _updateTime = value;
                        break;
                }
            }
        }
        /// <summary>
        /// 更新时获取网站价格
        /// </summary>
        public static bool AllowGetPrice { get; set; }
        /// <summary>
        /// 更新时获取手机端价格
        /// </summary>
        public static bool AllowGetMobilePrice { get; set; }
        /// <summary>
        /// 更新时获取QQ端价格
        /// </summary>
        public static bool AllowGetQQPrice { get; set; }
        /// <summary>
        /// 更新时获取微信端价格
        /// </summary>
        public static bool AllowGetWXPrice { get; set; }

        /// <summary>
        /// 更新时获取商品库存
        /// </summary>
        public static bool AllowGetStock { get; set; }
        /// <summary>
        /// 更新时获取优惠券
        /// </summary>
        public static bool AllowGetCoupon { get; set; }
        /// <summary>
        /// 更新时获取商品评价信息
        /// </summary>
        public static bool AllowGetPostMessage { get; set; }
        /// <summary>
        /// 更新时获取促销信息
        /// </summary>
        public static bool AllowGetPromo { get; set; }
        #endregion Monitor Params
    }

    /// <summary>
    /// 采集类型定义
    /// </summary>
    public enum GatherType
    {
        /// <summary>
        /// 批量采集
        /// </summary>
        Batch,
        /// <summary>
        /// 普通采集
        /// </summary>
        Single
    }
}
