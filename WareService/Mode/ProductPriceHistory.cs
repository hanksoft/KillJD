using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareDealer.Mode
{
    /// <summary>
    /// 产品价格历史信息
    /// </summary>
    public class ProductPriceHistory
    {
        public Int64 RID { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 产品价格
        /// </summary>
        public Double Price { get; set; }
        /// <summary>
        /// 价格类型
        /// </summary>
        /// <remarks>京东、手机、超市（欧尚、沃尔玛、家乐福、红旗）、其它</remarks>
        public string PriceType { get; set; }
        /// <summary>
        /// 计价日期
        /// </summary>
        public DateTime PriceDate { get; set; }
        /// <summary>
        /// 价格入库时间
        /// </summary>
        public string PriceTime { get; set; }
    }
}
