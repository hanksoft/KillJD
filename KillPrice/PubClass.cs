using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WareDealer.Helper;
using WareDealer.Mode;

namespace KillPrice
{
    public class PubClass
    {
        /// <summary>
        /// 当前程序版本号
        /// </summary>
        public string AppVersion = "1.5";

        private List<TabSysParams> _myParams = new List<TabSysParams>();
        private static PubClass _instance;
        /// <summary>
        /// 当前分类商品集合
        /// </summary>
        public List<ProductInfo> CurWares { get; set; }
        private PubClass() { }

        public static PubClass GetInstance()
        {
            return _instance ?? (_instance = new PubClass());
        }

        public void GetSysParams()
        {
            _myParams = DBHelper.GetInstance().GetSysParams();
            ParseSysParams(_myParams);
        }

        /// <summary>
        /// 获取系统参数
        /// </summary>
        public void ParseSysParams(List<TabSysParams> myParams)
        {
            #region Params parse
            if (myParams != null && myParams.Count > 0)
            {
                foreach (var item in myParams)
                {
                    switch (item.ParamKey)
                    {
                        case "UserName":
                            SysParams.UserName = string.IsNullOrEmpty(item.ParamValue) || item.ParamValue.ToLower() == "null" ? "Test" : item.ParamValue;
                            break;
                        case "UserPass":
                            SysParams.UserPass = item.ParamValue;
                            break;
                        case "JDUserName":
                            SysParams.JDUserName = string.IsNullOrEmpty(item.ParamValue) || item.ParamValue.ToLower() == "null" ? "Test" : item.ParamValue;
                            break;
                        case "JDUserPass":
                            SysParams.JDUserPass = item.ParamValue;
                            break;
                        case "AllowAutoUpdateWareInfo":
                            SysParams.AllowAutoUpdateWareInfo = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "AllowAutoGetWebUrl":
                            SysParams.AllowAutoGetWebUrl = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "CheckNetState":
                            SysParams.CheckNetState = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "ProductVersion":
                            SysParams.ProductVersion = item.ParamValue;
                            break;
                        case "AllowAutoUpdateVersion":
                            SysParams.AllowAutoUpdateVersion = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "AllowLoginVerify":
                            SysParams.AllowLoginVerify = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "DispathArea":
                            SysParams.DispathArea = item.ParamValue;
                            break;
                        case "AllowRunAtSystemStart":
                            SysParams.AllowRunAtSystemStart = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "AllowAutoGetFocusWare":
                            SysParams.AllowAutoGetFocusWare = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "AutoGetFocusTime":
                            SysParams.AutoGetFocusTime = item.ParamValue;
                            break;
                        case "AutoUpdateTime":
                            SysParams.AutoUpdateTime = item.ParamValue;
                            break;
                        case "AllowAutoGC":
                            SysParams.AllowAutoGC = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "AllowGetPrice":
                            SysParams.AllowGetPrice = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "AllowGetMobilePrice":
                            SysParams.AllowGetMobilePrice = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "AllowGetQQPrice":
                            SysParams.AllowGetQQPrice = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "AllowGetWXPrice":
                            SysParams.AllowGetWXPrice = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "AllowGetStock":
                            SysParams.AllowGetStock = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "AllowGetCoupon":
                            SysParams.AllowGetCoupon = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "AllowGetPostMessage":
                            SysParams.AllowGetPostMessage = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "AllowGetPromo":
                            SysParams.AllowGetPromo = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "GatherJDWare":
                            SysParams.GatherJDWare = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "GatherQJWare":
                            SysParams.GatherQJWare = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "Gather3Ware":
                            SysParams.Gather3Ware = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "GetWareBaseInfo":
                            SysParams.GetWareBaseInfo = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "GetWarePrice":
                            SysParams.GetWarePrice = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "GetWareStock":
                            SysParams.GetWareStock = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "GetWareCoupon":
                            SysParams.GetWareCoupon = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "GetWarePostMessage":
                            SysParams.GetWarePostMessage = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                        case "GetWarePicture":
                            SysParams.GetWarePicture = string.IsNullOrEmpty(item.ParamValue) ? false : bool.Parse(item.ParamValue);
                            break;
                    }
                }
            }

            string[] areas = SysParams.DispathArea.Split('_');
            if (areas != null && areas.Length > 0)
            {
                SysParams.Level1Area = DBHelper.GetInstance().GetWareArea(areas[0]);
                SysParams.Level2Area = DBHelper.GetInstance().GetWareArea(areas[1]);
                SysParams.Level3Area = DBHelper.GetInstance().GetWareArea(areas[2]);
            }

            #endregion Params parse
        }
        /// <summary>
        /// 整合系统参数
        /// </summary>
        public void AssemblyParams()
        {
            if (_myParams != null && _myParams.Count > 0)
            {
                foreach (var item in _myParams)
                {
                    switch (item.ParamKey)
                    {
                        case "UserName":
                            item.ParamValue = SysParams.UserName;
                            break;
                        case "UserPass":
                            item.ParamValue = PassWordHelper.GetInstance().DesStr(SysParams.UserPass, "isarahan", "wolfstud");
                            break;
                        case "JDUserName":
                            item.ParamValue = SysParams.JDUserName;
                            break;
                        case "JDUserPass":
                            item.ParamValue = PassWordHelper.GetInstance().DesStr(SysParams.JDUserPass, "isarahan", "wolfstud");
                            break;
                        case "AllowAutoUpdateWareInfo":
                            item.ParamValue = SysParams.AllowAutoUpdateWareInfo.ToString();
                            break;
                        case "AutoUpdateTime":
                            item.ParamValue = SysParams.AutoUpdateTime.ToString();
                            break;
                        case "AllowAutoGetFocusWare":
                            item.ParamValue = SysParams.AllowAutoGetFocusWare.ToString();
                            break;
                        case "AutoGetFocusTime":
                            item.ParamValue = SysParams.AutoGetFocusTime.ToString();
                            break;
                        case "AllowAutoGetWebUrl":
                            item.ParamValue = SysParams.AllowAutoGetWebUrl.ToString();
                            break;
                        case "CheckNetState":
                            item.ParamValue = SysParams.CheckNetState.ToString();
                            break;
                        case "ProductVersion":
                            item.ParamValue = SysParams.ProductVersion;
                            break;
                        case "AllowAutoUpdateVersion":
                            item.ParamValue = SysParams.AllowAutoUpdateVersion.ToString();
                            break;
                        case "AllowLoginVerify":
                            item.ParamValue = SysParams.AllowLoginVerify.ToString();
                            break;
                        case "DispathArea":
                            item.ParamValue = SysParams.DispathArea;
                            break;
                        case "AllowRunAtSystemStart":
                            item.ParamValue = SysParams.AllowRunAtSystemStart.ToString();
                            break;
                        case "AllowAutoGC":
                            item.ParamValue = SysParams.AllowAutoGC.ToString();
                            break;
                        case "AllowGetPrice":
                            item.ParamValue = SysParams.AllowGetPrice.ToString();
                            break;
                        case "AllowGetMobilePrice":
                            item.ParamValue = SysParams.AllowGetMobilePrice.ToString();
                            break;
                        case "AllowGetQQPrice":
                            item.ParamValue = SysParams.AllowGetQQPrice.ToString();
                            break;
                        case "AllowGetWXPrice":
                            item.ParamValue = SysParams.AllowGetWXPrice.ToString();
                            break;
                        case "AllowGetStock":
                            item.ParamValue = SysParams.AllowGetStock.ToString();
                            break;
                        case "AllowGetCoupon":
                            item.ParamValue = SysParams.AllowGetCoupon.ToString();
                            break;
                        case "AllowGetPostMessage":
                            item.ParamValue = SysParams.AllowGetPostMessage.ToString();
                            break;
                        case "AllowGetPromo":
                            item.ParamValue = SysParams.AllowGetPromo.ToString();
                            break;
                        case "GatherJDWare":
                            item.ParamValue = SysParams.GatherJDWare.ToString();
                            break;
                        case "GatherQJWare":
                            item.ParamValue = SysParams.GatherQJWare.ToString();
                            break;
                        case "Gather3Ware":
                            item.ParamValue = SysParams.Gather3Ware.ToString();
                            break;
                        case "GetWareBaseInfo":
                            item.ParamValue = SysParams.GetWareBaseInfo.ToString();
                            break;
                        case "GetWarePrice":
                            item.ParamValue = SysParams.GetWarePrice.ToString();
                            break;
                        case "GetWareStock":
                            item.ParamValue = SysParams.GetWareStock.ToString();
                            break;
                        case "GetWareCoupon":
                            item.ParamValue = SysParams.GetWareCoupon.ToString();
                            break;
                        case "GetWarePostMessage":
                            item.ParamValue = SysParams.GetWarePostMessage.ToString();
                            break;
                        case "GetWarePicture":
                            item.ParamValue = SysParams.GetWarePicture.ToString();
                            break;
                    }
                }
                DBHelper.GetInstance().UpdateSysParams(_myParams);
            }

        }
    }
}
