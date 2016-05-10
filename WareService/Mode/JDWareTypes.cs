using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WareDealer.Mode
{
    public class JDWareTypes
    {
        public List<JDType> data { get; set; }
    }

    public class JDType
    {
        public string id { get; set; }
        public object t { get; set; }
        public object b { get; set; }
        public List<subType> s { get; set; }
        public object c { get; set; }
        public object p { get; set; }
    }

    public class subType
    {
        public string n { get; set; }
        public List<subType> s { get; set; }
    }
    /// <summary>
    /// 商品分类数据库模型
    /// </summary>
    public class JDWareType
    {
        /// <summary>
        /// 分类编号
        /// </summary>
        public int TypeID { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 上级编号 第一级编号为0
        /// </summary>
        public int TopID { get; set; }
        /// <summary>
        /// 顶级编号
        /// </summary>
        public int TopTID { get; set; }
        /// <summary>
        /// 分类链接地址
        /// </summary>
        public string TypeUrl { get; set; }
        /// <summary>
        /// 分类级别
        /// </summary>
        public int TypeLevel { get; set; }
        /// <summary>
        /// 可用性
        /// </summary>
        public bool BEnable { get; set; }
    }
}
