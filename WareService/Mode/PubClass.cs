using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareDealer.Mode
{
    public class PubClass
    {

    }

    /// <summary>
    /// 商品导入方式
    /// </summary>
    public enum WareImportType
    {
        /// <summary>
        /// 文本模式
        /// </summary>
        Text,
        /// <summary>
        /// IE浏览器
        /// </summary>
        InternetExplore,
        /// <summary>
        /// 火狐浏览器
        /// </summary>
        FireFox,
        /// <summary>
        /// 京东关注
        /// </summary>
        JDWatch,
        /// <summary>
        /// 京东分类数据集
        /// </summary>
        JDType,
        /// <summary>
        /// 京东查询结果集
        /// </summary>
        JDSearch
    }
    /// <summary>
    /// 价格类型定义
    /// </summary>
    public enum WarePriceType
    {
        /// <summary>
        /// 网页价格
        /// </summary>
        WebPrice,
        /// <summary>
        /// 手机价格
        /// </summary>
        AppPrice,
        /// <summary>
        /// QQ价格
        /// </summary>
        QQPrice,
        /// <summary>
        /// 微信价格
        /// </summary>
        WxPrice
    }
}
