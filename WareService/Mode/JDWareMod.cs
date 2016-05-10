using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareDealer.Mode
{
    #region Price
    /// <summary>
    /// 商品价格
    /// </summary>
    public class JdWarePrice
    {
        //cnp([{"id":"J_1010527324","p":"129.00","m":"350.00"}]);
        //jQuery5068505([{"id":"202459","pcp":"69.00","p":"59.00","m":"121.00"}]);

        /// <summary>
        /// 商品编号
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 网站销售价格/手机专享价
        /// </summary>
        public string p { get; set; }
        /// <summary>
        /// 原始价格
        /// </summary>
        public string m { get; set; }
        /// <summary>
        /// 价格来源
        /// </summary>
        public string src { get; set; }
    }

    /// <summary>
    /// 商品价格（手机）
    /// </summary>
    public class JdWareMobilePrice : JdWarePrice
    {
        /// <summary>
        /// 网站销售价格
        /// </summary>
        public string pcp { get; set; }
    }
    #endregion Price

    #region StockInfo
    public class StockInfo
    {
        public Stock Stock { get; set; }
    }
    public class Stock
    {
        public string serviceInfo { get; set; }
        public string Ext { get; set; }
        public int StockState { get; set; }
        public long realSkuId { get; set; }
        public int code { get; set; }
        public int skuState { get; set; }
        public int PopType { get; set; }
        public int channel { get; set; }
        public string StockStateName { get; set; }
        public string vd { get; set; }
        public int rfg { get; set; }
        public string ArrivalDate { get; set; }
        //public string Dti { get; set; }
        public bool IsPurchase { get; set; }
        public int rn { get; set; }

        //private Dcs _dcs = new Dcs();
        //public Dcs Dc { get { return _dcs;} set { _dcs = value; } }

        private prs _pr = new prs();
        public prs pr { get { return _pr;} set { _pr = value; } }

        private List<irs> _ir = new List<irs>();
        public List<irs> ir { get { return _ir;} set { _ir = value;} }

        private List<irs> _eir = new List<irs>();
        public List<irs> eir { get { return _eir;} set { _eir = value; } }

        private selfs _selfs = new selfs();
        public selfs self_D { get { return _selfs;} set { _selfs = value; } }

        private selfs _selfs1 = new selfs();
        public selfs D { get { return _selfs1; } set { _selfs1 = value; } }
    }

    public class prs
    {
        public int resultCode {get;set;}
        public string promiseResult { get; set; }
    }

    public class irs
    {
        public string iconSrc { get; set; }
        public string iconTip { get; set; }
        public string helpLink { get; set; }
        public string iconCode { get; set; }
        public int resultCode { get; set; }
        public string showName { get; set; }
        public string picUrl { get; set; }
        
    }

    public class selfs
    {
        public int vid { get; set; }
        public string df { get; set; }
        public string cg { get; set; }
        public string deliver { get; set; }
        public int id { get; set; }

        public int type {get;set;}
        public string vender { get; set; }

        public string linkphone { get; set; }
        public string url { get; set; }
        public string po { get; set; }
    }

    public class Dcs
    {
        public string dcash { get; set; }
        public string dtype { get; set; }
        public string freihtType { get; set; }
        public string ordermin { get; set; }
    }
    #endregion StockInfo

    #region CommentCount

    public class CommentInfo
    {
        public CommentCounts CommentCount { get; set; }
    }

    public class CommentCounts
    {
        public int SkuId { get; set; }
        public int ProductId { get; set; }
        public int Score1Count { get; set; }
        public int Score2Count { get; set; }
        public int Score3Count { get; set; }
        public int Score4Count { get; set; }
        public int Score5Count { get; set; }
        public int ShowCount { get; set; }
        public int CommentCount { get; set; }
        public int AverageScore { get; set; }
        public int GoodCount { get; set; }
        public float GoodRate { get; set; }
        public int GoodRateShow { get; set; }
        public int GoodRateStyle { get; set; }
        public int GeneralCount { get; set; }
        public float GeneralRate { get; set; }
        public int GeneralRateShow { get; set; }
        public int GeneralRateStyle { get; set; }
        public int PoorCount { get; set; }
        public float PoorRate { get; set; }
        public int PoorRateShow { get; set; }
        public int PoorRateStyle { get; set; }
    }

    #endregion CommentCount
}
