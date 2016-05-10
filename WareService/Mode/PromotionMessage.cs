using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WareDealer.Mode
{
    /// <summary>
    /// 促销信息模型
    /// </summary>
    public class PromotionMessage
    {
        /// <summary>
        /// 优惠券？
        /// </summary>
        public object quan { get; set; }
        /// <summary>
        /// 优惠券详情
        /// </summary>
        public List<Coupon> skuCoupon { get; set; }
        public int adsStatus { get; set; }
        /// <summary>
        /// 促销广告
        /// </summary>
        public List<ADMessage> ads { get; set; }

        public int quanStatus { get; set; }
        public int promStatus { get; set; }
        /// <summary>
        /// 促销信息
        /// </summary>
        public PromoTag prom { get; set; }
    }

    /// <summary>
    /// 优惠券信息
    /// </summary>
    public class Coupon
    {
        public int couponType { get; set; }
        public int couponKind { get; set; }
        public Int32 batchId { get; set; }
        public Int64 beginTime { get; set; }
        public int userClass { get; set; }
        public string url { get; set; }
        public int addDays { get; set; }
        public int area { get; set; }
        public Int64 endTime { get; set; }
        public int quota { get; set; }
        public string toUrl { get; set; }
        public int roleId { get; set; }
        public int discount { get; set; }
        public string key { get; set; }
        public int limitType { get; set; }
        public string name { get; set; }
        public string timeDesc { get; set; }
    }
    /// <summary>
    /// 促销广告信息
    /// </summary>
    public class ADMessage
    {
        public string id { get; set; }
        public string ad { get; set; }
    }
    /// <summary>
    /// 促销信息
    /// </summary>
    public class PromoTag
    {
        public string hit { get; set; }
        public int ending { get; set; }
        public object tags { get; set; }
        public string jl { get; set; }
        public string vl { get; set; }

        public List<PickTag> pickOneTag { get; set; }
    }

    public class PickTag
    {
        public string st { get; set; }
        public string code { get; set; }
        public string content { get; set; }
        public string d { get; set; }
        public Int32 tr { get; set; }
        public string name { get; set; }
        public string pid { get; set; }
    }
}
