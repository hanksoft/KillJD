using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareDealer.Mode
{
    public class ProductInfo
    {
        /// <summary>
        /// 资源编号
        /// </summary>
        public string RID { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 供应商编号
        /// </summary>
        public string VenderId { get; set; }
        /// <summary>
        /// 店铺编号
        /// </summary>
        public string ShopId { get; set; }
        /// <summary>
        /// 商品编号（取值与产品编号相同）
        /// </summary>
        public string SkuId { get; set; }
        /// <summary>
        /// 配送区域
        /// </summary>
        public string CatArea { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 商品价格
        /// </summary>
        public double ProductPrice { get; set; }
        /// <summary>
        /// 手机专享价
        /// </summary>
        public double ProductMobilePrice { get; set; }
        /// <summary>
        /// 商品底价 记录历史最低价格
        /// </summary>
        public double ProductBasePrice { get; set; }
        /// <summary>
        /// 商品询价日期
        /// </summary>
        public DateTime ProductPriceDate { get; set; }
        /// <summary>
        /// 商品价格来源
        /// </summary>
        public string ProductPriceType { get; set; }
        /// <summary>
        /// 价格趋势
        /// </summary>
        /// <remarks>涨价、持平、走低</remarks>
        public string ProductPriceTrend { get; set; }
        /// <summary>
        /// 商品类别
        /// </summary>
        public string ProductType { get; set; }
        /// <summary>
        /// 商品类别名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 是否有货
        /// </summary>
        /// <remarks>-1 下柜 0 无货 1 有货 2 配货 3 预订 </remarks>
        public int ProductIsSaled { get; set; }
        /// <summary>
        /// 商品库存情况 不入库，用于显示
        /// </summary>
        public string ProductStock { get; set; }
        /// <summary>
        /// 商品标签
        /// </summary>
        public string ProductTag { get; set; }
        /// <summary>
        /// 商品品牌
        /// </summary>
        public string ProductBrand { get; set; }
        /// <summary>
        /// 商品配送方式
        /// </summary>
        public string ProductDispatchMode { get; set; }
        /// <summary>
        /// 商品评价总次数
        /// </summary>
        public int ProductEvaluateCount { get; set; }
        /// <summary>
        /// 好评率
        /// </summary>
        public double ProductGoodRate { get; set; }
        /// <summary>
        /// 差评信息
        /// </summary>
        public string ProductPoorInfo { get; set; }
        /// <summary>
        /// 商品图片路径
        /// </summary>
        public string ProductImagePath { get; set; }
        /// <summary>
        /// 商品图片网站路径
        /// </summary>
        public string ProductImageWebPath { get; set; }
        /// <summary>
        /// 商品图片  不入库，用于显示
        /// </summary>
        public Bitmap ProductImage { get; set; }
        /// <summary>
        /// 商品原始地址
        /// </summary>
        public string ProductURL { get; set; }
        /// <summary>
        /// 商品详细信息
        /// </summary>
        public string ProductDetail { get; set; }
        /// <summary>
        /// 商品归属
        /// </summary>
        public string ProductAttach { get; set; }
        /// <summary>
        /// 商品原生数据（页面原始Html数据）
        /// </summary>
        public string NativeData { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 可用性
        /// </summary>
        public Boolean BEnable { get; set; }
    }
}
