using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WareDealer.Mode
{
    public class ProductType
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string TID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 可用性
        /// </summary>
        public Boolean BEnable { get; set; }
    }
}
