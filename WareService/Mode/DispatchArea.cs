using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WareDealer.Mode
{
    /// <summary>
    /// 商品配送区域
    /// </summary>
    public class DispatchArea
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 上级目录
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }
    }
}
