using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WareDealer.Mode
{
    /// <summary>
    /// 产品评价信息
    /// </summary>
    public class ProductMessage
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 评价类型 1 差评 2 中评 3 好评
        /// </summary>
        public int MsgType { get; set; }
        /// <summary>
        /// 评价信息
        /// </summary>
        public string MsgContent { get; set; }
        /// <summary>
        /// 评价人
        /// </summary>
        public string MsgUser { get; set; }
        /// <summary>
        /// 评价人等级
        /// </summary>
        public string MsgUserLevel { get; set; }
        /// <summary>
        /// 评价人地区
        /// </summary>
        public string MsgProvince { get; set; }
        /// <summary>
        /// 评价时间
        /// </summary>
        public string MsgDate { get; set; }
    }
}
