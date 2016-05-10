using Hank.ComLib;
using WareDealer.Mode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WareDealer.Helper
{
    [DisplayName("数据库助手")]
    public class DBHelper
    {
        /// <summary>
        /// 建立数据库连接
        /// </summary>
        /// <returns></returns>
        OtDB GetDb()
        {
            return _sqlDB ?? (_sqlDB = new OtDB("sqlite"));
        }
        private OtDB _sqlDB = null;

        private static DBHelper _dbhelper = null;
        private DBHelper() { }

        public static DBHelper GetInstance()
        {
            return _dbhelper ?? (_dbhelper = new DBHelper());
        }

        /// <summary>
        /// 判断商品是否已存在
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public bool WareIsExists(string pid)
        {
            try
            {
                ProductInfo rtnobj = WareGetOne(pid);
                return (rtnobj != null);
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 新增商品信息
        /// </summary>
        /// <param name="_myProduct"></param>
        public void WareInsert(ProductInfo _myProduct)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                db.ExecInsert(_myProduct, "ProductInfo", new string[] { "RID", "ProductID", "SkuID", "SkuidKey", "VenderId", "ShopId", "ProductName", 
                    "BrandID", "ProductBrand", "ProductAttach", "ProductIsSaled",  "ProductTag", "Catalog", "ProductImageWebPath",
                     "ProductImagePath", "ProductURL","ProductCoupon", "ProductPromoMsg", "ProductDispatchMode",
                     "ProductPrice", "ProductBasePrice","ProductMobilePrice","ProductQQPrice","ProductWXPrice", "ProductType", "ProductPriceTrend",
                     "ProductGeneralRate", "ProductPoorRate", "ProductHotCommentTag", "ProductEvaluateCount", "ProductGoodRate", 
                      "CreateTime", "CreateUser", "Focus", "BEnable" });
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 更新商品信息
        /// </summary>
        /// <param name="_myProduct"></param>
        public void WareUpdate(ProductInfo _myProduct)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                db.ExecUpdate(_myProduct, "ProductInfo", new string[] { "ProductName", "BrandID", "ProductBrand", "ProductAttach", "ProductIsSaled", 
                    "ProductTag", "Catalog", "ProductImageWebPath",
                     "ProductImagePath", "ProductURL","ProductCoupon", "ProductPromoMsg", "ProductDispatchMode",
                     "ProductPrice", "ProductType", "ProductMobilePrice", "ProductQQPrice", "ProductWXPrice", //"ProductPriceTrend", "ProductBasePrice",
                     "ProductGeneralRate", "ProductPoorRate", "ProductHotCommentTag", "ProductEvaluateCount", "ProductGoodRate", "Focus" },
                      string.Format(" ProductID = '{0}'", _myProduct.ProductID), null);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 获取所有商品信息
        /// </summary>
        /// <returns></returns>
        public List<ProductInfo> WareGetAll()
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                //分页查询
                //select * from users order by id limit 10 offset 0;//offset代表从第几条记录“之后“开始查询，limit表明查询多少条结果
                //select * from account limit10,9
                string sql = "select a.*,(case a.ProductIsSaled when -1 then '下柜' when 1 then '有货' when 2 then '配货' when 3 then '预订' else '无货' end) as ProductStock, "
                    + "b.Name as TypeName from ProductInfo a left join ProductType b on a.ProductType = b.TID where a.BEnable=1 order by a.CreateTime";
                List<ProductInfo> rtnList = db.Query<ProductInfo>(sql, null);
                db.Commit();

                return rtnList;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
                return null;
            }
        }

        /// <summary>
        /// 获取某个类别商品数据
        /// </summary>
        /// <param name="typeid"></param>
        /// <returns></returns>
        public List<ProductInfo> WareGetAll(string typeid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = "";
                if (typeid == "UnType") //未分类
                {
                    sql = "select a.*,(case a.ProductIsSaled when -1 then '下柜' when 1 then '有货' when 2 then '配货' else '无货' end) as ProductStock, "
                    + "b.Name as TypeName from ProductInfo a left join ProductType b on a.ProductType = b.TID where a.BEnable=1 and (a.ProductType = '' or a.ProductType is null) order by a.CreateTime";
                }
                else if (typeid == "down") //降价、走低
                {
                    sql = "select a.*,(case a.ProductIsSaled when -1 then '下柜' when 1 then '有货' when 2 then '配货' when 3 then '预订' else '无货' end) as ProductStock, "
                    + "b.Name as TypeName from ProductInfo a left join ProductType b on a.ProductType = b.TID where a.ProductPriceTrend='走低' and a.BEnable=1 order by a.CreateTime";
                }
                else if (typeid == "up") //涨价
                {
                    sql = "select a.*,(case a.ProductIsSaled when -1 then '下柜' when 1 then '有货' when 2 then '配货' when 3 then '预订' else '无货' end) as ProductStock, "
                    + "b.Name as TypeName from ProductInfo a left join ProductType b on a.ProductType = b.TID where a.ProductPriceTrend='涨价' and a.BEnable=1 order by a.CreateTime";
                }
                else if (typeid == "balance") //持平
                {
                    sql = "select a.*,(case a.ProductIsSaled when -1 then '下柜' when 1 then '有货' when 2 then '配货' when 3 then '预订' else '无货' end) as ProductStock, "
                    + "b.Name as TypeName from ProductInfo a left join ProductType b on a.ProductType = b.TID where a.ProductPriceTrend='持平' and a.BEnable=1 order by a.CreateTime";
                }
                else if (typeid == "focus") //重点关注
                {
                    sql = "select a.*,(case a.ProductIsSaled when -1 then '下柜' when 1 then '有货' when 2 then '配货' when 3 then '预订' else '无货' end) as ProductStock, "
                    + "b.Name as TypeName from ProductInfo a left join ProductType b on a.ProductType = b.TID where a.Focus=1 and a.BEnable=1 order by a.CreateTime";
                }
                else if (typeid == "trash") //回收站
                {
                    sql = "select a.*,(case a.ProductIsSaled when -1 then '下柜' when 1 then '有货' when 2 then '配货' when 3 then '预订' else '无货' end) as ProductStock, "
                    + "b.Name as TypeName from ProductInfo a left join ProductType b on a.ProductType = b.TID where a.BEnable=0 order by a.CreateTime";
                }
                else
                {
                    //分页查询
                    //select * from users order by id limit 10 offset 0;//offset代表从第几条记录“之后“开始查询，limit表明查询多少条结果
                    //select * from account limit10,9
                    sql = string.Format("select a.*,(case a.ProductIsSaled when -1 then '下柜' when 1 then '有货' when 2 then '配货' else '无货' end) as ProductStock, "
                        + "b.Name as TypeName from ProductInfo a left join ProductType b on a.ProductType = b.TID where a.ProductType = '{0}' and a.BEnable=1 order by a.CreateTime", typeid);
                }
                List<ProductInfo> rtnList = db.Query<ProductInfo>(sql, null);
                db.Commit();

                return rtnList;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
                return null;
            }
        }
        /// <summary>
        /// 获取某个类别商品数据
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="wName"></param>
        /// <param name="wStock"></param>
        /// <param name="wPriceTrend"></param>
        /// <param name="wAttach"></param>
        /// <returns></returns>
        public List<ProductInfo> WareGetAll(string typeid, string wName, string wStock, string wPriceTrend, string wAttach)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string wStr = string.IsNullOrEmpty(wName) ? "" : string.Format("where ProductName like '%{0}%'",wName);
                string sql = string.Format("select a.*,(case a.ProductIsSaled when -1 then '下柜' when 1 then '有货' when 2 then '配货' when 3 then '预订' else '无货' end) as ProductStock, "
                    + "b.Name as TypeName from ProductInfo a left join ProductType b on a.ProductType = b.TID {0} where a.BEnable=1 order by a.CreateTime", wStr);
                List<ProductInfo> rtnList = db.Query<ProductInfo>(sql, null);
                db.Commit();

                return rtnList;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
                return null;
            }
        }

        /// <summary>
        /// 获取某个商品信息
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <returns></returns>
        public ProductInfo WareGetOne(string pid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("select * from ProductInfo where productid='{0}'", pid);
                ProductInfo rtnList = db.QueryOneRow<ProductInfo>(sql, null);
                db.Commit();

                return rtnList;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
                return null;
            }
        }

        /// <summary>
        /// 逻辑删除/启用商品
        /// </summary>
        /// <param name="pid"></param>
        public void WareDelOne(string pid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("update ProductInfo set BEnable = 0 where productid ='{0}'", pid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 逻辑删除/启用商品
        /// </summary>
        /// <param name="pid"></param>
        public void WareReloadOne(string pid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("update ProductInfo set BEnable = 1 where productid ='{0}'", pid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 删除单个商品
        /// </summary>
        /// <param name="rid"></param>
        public void WareRemoveOne(string pid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("delete from ProductInfo where productid ='{0}'", pid);
                db.Exec(sql);
                sql = string.Format("delete from ProductPriceHistory where pid='{0}'", pid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 清空商品库
        /// </summary>
        public void WareClear()
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("delete from ProductInfo");
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        #region WareType
        public List<ProductType> WareTypeGet()
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                //分页查询
                //select * from users order by id limit 10 offset 0;//offset代表从第几条记录“之后“开始查询，limit表明查询多少条结果
                //select * from account limit10,9
                string sql = "select TID,Name from ProductType order by CreateTime";
                List<ProductType> rtnList = db.Query<ProductType>(sql, null);
                db.Commit();

                return rtnList;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
                return null;
            }
        }
        /// <summary>
        /// 增加商品类型
        /// </summary>
        /// <param name="wType"></param>
        public void WareTypeInsert(ProductType wType)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                db.ExecInsert(wType, "ProductType", new string[] { "TID", "Name", "Description", "BEnable", "CreateTime" });
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 更新产品类型
        /// </summary>
        /// <param name="wType"></param>
        public void WareTypeUpdate(ProductType wType)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                db.ExecUpdate(wType, "ProductType", new string[] { "Name", "Description", "BEnable" },
                    string.Format(" TID = '{0}'", wType.TID), null);
                db.Commit();

            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 更新产品属性类型
        /// </summary>
        /// <param name="pid"></param>
        public void WareTypeUpdate(string pid,string typeid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("update ProductInfo set ProductType = '{1}' where ProductID = '{0}'", pid, typeid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 删除商品类型
        /// </summary>
        /// <param name="tid"></param>
        public void WareTypeDelete(string tid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                //将商品中分类数据置空
                string sql = string.Format("update  Productinfo set ProductType='' where ProductType = '{0}'", tid);
                db.Exec(sql);
                //删除商品分类
                sql = string.Format("delete from ProductType where tid = '{0}'", tid);
                db.Exec(sql);
                db.Commit();

            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        

        #endregion WareType

        #region Price Manage
        /// <summary>
        /// 插入商品价格
        /// </summary>
        /// <param name="price">历史价格对象</param>
        public void WarePriceInsert(ProductPriceHistory price)
        {
            if (price != null)
            {
                WarePriceInsert(price.PID, price.Price, price.PriceDate, price.PriceType);
            }
        }

        /// <summary>
        /// 插入商品价格
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <param name="price">商品价格</param>
        public void WarePriceInsert(string pid, double price)
        {
            WarePriceInsert(pid, price, DateTime.Now, "京东");
        }
        
        /// <summary>
        /// 插入商品价格
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <param name="price">商品价格</param>
        /// <param name="dt">入库时间</param>
        /// <param name="origin">数据来源 如京东、手机、超市</param>
        public void WarePriceInsert(string pid, double price, DateTime dt, string origin)
        {
            OtDB db = GetDb();
            try
            {
                //先查找当天有没有相同的数据
                if (!WarePriceHistoryGetOne(pid, dt, price))
                {
                    db.Begin();
                    //string sql = string.Format("select * from ProductPriceHistory");
                    //sqlite用的全球时间UTC,要用datetime()函数转换若干 localtime UTC
                    //用data_time.ToString("s");这种方法转换成 iso 8601标准字符串格式
                    string sql = string.Format("insert into ProductPriceHistory (PID, Price, PriceDate, PriceType) select '{0}',{1},'{2}','{3}'", pid, price, dt.ToString("s"), origin);
                    db.Exec(sql);
                    db.Commit();
                }

            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
            }
        }

        public void WarePriceUpdate(string pid, double price, WarePriceType priceSrc)
        {
            OtDB db = GetDb();
            try
            {
                string sql = "";
                switch (priceSrc)
                {
                    case WarePriceType.WebPrice:
                        sql = string.Format("update ProductInfo set ProductPrice = {0},ProductPriceDate = DateTime('now') where ProductID = '{1}';", price, pid);
                        break;
                    case WarePriceType.AppPrice:
                        sql = string.Format("update ProductInfo set ProductMobilePrice = {0},ProductPriceDate = DateTime('now') where ProductID = '{1}';", price, pid);
                        break;
                    case WarePriceType.QQPrice:
                        sql = string.Format("update ProductInfo set ProductQQPrice = {0},ProductPriceDate = DateTime('now') where ProductID = '{1}';", price, pid);
                        break;
                    case WarePriceType.WxPrice:
                        sql = string.Format("update ProductInfo set ProductWXPrice = {0},ProductPriceDate = DateTime('now') where ProductID = '{1}';", price, pid);
                        break;
                    default:
                        break;
                }
                if (string.IsNullOrEmpty(sql))
                {
                    return;
                }
                db.Begin();
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 更新商品价格
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <param name="price">网页价格</param>
        public void WarePriceUpdate(string pid, double price)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("update ProductInfo set ProductPrice = {0},ProductPriceDate = DateTime('now') where ProductID = '{1}';", price, pid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 更新商品所有价格
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="price1"></param>
        /// <param name="price2"></param>
        /// <param name="price3"></param>
        /// <param name="price4"></param>
        /// <param name="priceSrc"></param>
        public void WareRepositoryUpdate(string pid, double baseprice, double price1, double price2, double price3, double price4, string priceSrc)
        {
            string trend = "未知";
            trend = WarePriceTrend(pid, baseprice);
            OtDB db = GetDb();
            try
            {
                db.Begin();
                //价格对比 --> 涨价、持平、走低、未知
                string sql = string.Format("update ProductInfo set ProductPrice = {0}, ProductMobilePrice = {1}, ProductQQPrice = {2}, ProductWXPrice = {3}," +
                    "ProductPriceDate = DateTime('now'), ProductPriceTrend='{4}',ProductPriceType = '{5}' where ProductID = '{6}';",
                    price1, price2, price3, price4, trend, priceSrc, pid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 更新商品价格及库存
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <param name="price1">网页价格</param>
        /// <param name="price2">手机价格</param>
        /// <param name="price3">QQ价格</param>
        /// <param name="price4">微信价格</param>
        /// <param name="priceSrc">价格来源</param>
        /// <param name="stock">库存</param>
        public void WareRepositoryUpdate(string pid, double price1, double price2, double price3, double price4, string priceSrc, int stock)
        {
            string trend = "未知";
            if (stock >= 0)
            {
                trend = WarePriceTrend(pid, price1);
            } 
            OtDB db = GetDb();
            try
            {
                db.Begin();
                //价格对比 --> 涨价、持平、走低、未知
                string sql = string.Format("update ProductInfo set ProductPrice = {0}, ProductMobilePrice = {1}, ProductQQPrice = {2}, ProductWXPrice = {3},"+
                    "ProductPriceDate = DateTime('now'), ProductIsSaled={4}, ProductPriceTrend='{5}',ProductPriceType = '{6}' where ProductID = '{7}';",
                    price1, price2, price3, price4, stock, trend, priceSrc, pid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        public void WareRepositoryUpdate(string pid, string dispatch)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                //价格对比 --> 涨价、持平、走低、未知
                string sql = string.Format("update ProductInfo set ProductDispatchMode = '{0}' where ProductID = '{1}';", dispatch, pid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }
        /// <summary>
        /// 获取商品价格趋势
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <param name="price">商品对比价格</param>
        /// <returns></returns>
        public string WarePriceTrend(string pid,double price)
        {
            double rtnPrice = WareLastPriceGet(pid);
            if (rtnPrice != 0)
            {
                //价格对比 --> 涨价、持平、走低、未知
                string rtnstr = rtnPrice > price ? "走低" : (rtnPrice < price ? "涨价" : "持平");
                return rtnstr;
            }
            return "未知";
        }

        /// <summary>
        /// 获取某个商品当前日期前的最后价格
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public double WareLastPriceGet(string pid)
        {
            double rtnPrice = 0;
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("select price,max(PriceDate) pricetime from ProductPriceHistory where pid = {0} and Date(PriceDate) < Date(DateTime('now'));", pid);
                //string sql = string.Format("select ProductPrice from ProductInfo where ProductID = '{0}';", pid);
                rtnPrice = db.QueryOneCol<double>(sql);
                db.Commit();
                return rtnPrice;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
                return 0;
            }
        }

        /// <summary>
        /// 更新商品归属
        /// </summary>
        /// <param name="pid">编号</param>
        /// <param name="vid">商店编号</param>
        /// <param name="sid">厂商编号</param>
        /// <param name="cat">配送区域</param>
        public void WareShopUpdate(string pid, string vid, string sid, string cat)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("update ProductInfo set VenderId = '{0}',ShopID = '{1}',Catalog = '{2}' where ProductID = '{3}';", vid, sid, cat, pid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 删除商品价格
        /// </summary>
        /// <param name="id">商品序号</param>
        public void WarePriceDel(Int64 id)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("delete from ProductPriceHistory where rowid = {0}", id);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                 OtCom.XLogErr(ex.Message);
                 db.Rollback();
            }
        }

        /// <summary>
        /// 判断某个商品当天是否有相同价格存在
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <param name="dt">日期</param>
        /// <param name="price">价格</param>
        /// <returns></returns>
        public bool WarePriceHistoryGetOne(string pid,DateTime dt,double price)
        {
            bool rtnJudge = false;
            //开始日期
            DateTime dtBegin = dt.Date;
            //结束日期
            DateTime dtEnd = dt.AddDays(1).Date;
            ProductPriceHistory rtnObj = WarePriceHistoryGetOne(pid, dtBegin, dtEnd, price);
            rtnJudge = rtnObj != null;
            return rtnJudge;
        }
        /// <summary>
        /// 判断某个商品当天是否有相同价格存在
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <param name="dt1">开始日期</param>
        /// <param name="dt2">结束日期</param>
        /// <param name="price">价格</param>
        /// <returns></returns>
        public ProductPriceHistory WarePriceHistoryGetOne(string pid, DateTime dt1, DateTime dt2, double price)
        {
            OtDB db = GetDb();
            try
            {
                //datetime(PriceDate,'localtime')  datetime(PriceDate,'utc')
                db.Begin();
                //分页查询
                //select * from users order by id limit 10 offset 0;//offset代表从第几条记录“之后“开始查询，limit表明查询多少条结果
                //select * from account limit10,9
                string sql = string.Format("select rowid as RID,*,datetime(PriceDate) as PriceTime from ProductPriceHistory " +
                    "where pid = '{0}' and (PriceDate between '{1}' and '{2}') and Price={3}  order by PriceDate", pid, dt1.ToString("s"), dt2.ToString("s"), price);
                List<ProductPriceHistory> rtnList = db.Query<ProductPriceHistory>(sql, null);
                db.Commit();
                if (rtnList.Count > 0)
                {
                    return rtnList[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
                return null;
            }
        }

        /// <summary>
        /// 获取商品历史价格
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <returns></returns>
        public List<ProductPriceHistory> WarePriceHistoryGet(string pid)
        {
            OtDB db = GetDb();
            try
            {
                //datetime(PriceDate,'localtime')  datetime(PriceDate,'utc')
                db.Begin();
                //分页查询
                //select * from users order by id limit 10 offset 0;//offset代表从第几条记录“之后“开始查询，limit表明查询多少条结果
                //select * from account limit10,9
                string sql = string.Format("select rowid as RID,*,datetime(PriceDate) as PriceTime from ProductPriceHistory where pid = '{0}' order by PriceDate desc", pid);
                List<ProductPriceHistory> rtnList = db.Query<ProductPriceHistory>(sql, null);
                db.Commit();

                return rtnList;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
                return null;
            }
        }

        /// <summary>
        /// 获取商品历史价格 只取部分，重复的价格将抛弃部分点
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public List<ProductPriceHistory> WarePriceHistoryGetMore(string pid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("select rowid as RID,*,datetime(PriceDate) as PriceTime from ProductPriceHistory where pid = '{0}' "
                    +"order by PriceDate desc", pid);
                List<ProductPriceHistory> rtnList = db.Query<ProductPriceHistory>(sql, null);
                db.Commit();

                return rtnList;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
                return null;
            }
        } 

        public void GetMinPrices()
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("select pid, min(price) minprice from ProductPriceHistory group by pid");
                //List<ProductPriceHistory> rtnList = db.Query<ProductPriceHistory>(sql, null);
                db.Commit();

            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }
        /// <summary>
        /// 更新商品底价
        /// </summary>
        /// <param name="pid">商品编号</param>
        public void UpdateHistoryPriceBasebyID(string pid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("update ProductInfo set  ProductBasePrice=(select min(price) from ProductPriceHistory t "
                    +"where t.pid =  '{0}') where ProductInfo.ProductID = '{1}'", pid, pid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }
        #endregion Price Manage

        #region SysParams
        public List<TabSysParams> GetSysParams()
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("select * from SysParams");
                List<TabSysParams> myParams = db.Query<TabSysParams>(sql, null);
                db.Commit();
                //ParseSysParams(myParams);
                return myParams;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                return null;
                //db.Rollback();
            }
        }
        /// <summary>
        /// 更新系统参数集合
        /// </summary>
        /// <param name="myParams"></param>
        public void UpdateSysParams(List<TabSysParams> myParams)
        {
            foreach (var item in myParams)
            {
                UpdateSysParam(item.ParamKey, item.ParamValue);
            }
        }

        /// <summary>
        /// 添加系统参数
        /// </summary>
        /// <param name="pKey"></param>
        /// <param name="PValue"></param>
        private void InsertSysParam(string pKey, string PValue)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("select * from SysParams where ParamKey = '{0}';", pKey);
                TabSysParams rtn = db.QueryOneRow<TabSysParams>(sql);
                if (rtn == null)
                {
                    sql = string.Format("insert into SysParams (ParamKey,ParamValue,ParamDescription) values ('{0}','{1}','');", pKey, PValue);
                    db.Exec(sql);
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 更新系统参数值
        /// </summary>
        /// <param name="pKey"></param>
        /// <param name="pValue"></param>
        public void UpdateSysParam(string pKey, string pValue)
        {
            //首先判断参数是否存在
            InsertSysParam(pKey, pValue);

            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("update SysParams set ParamValue = '{0}' where ParamKey = '{1}';", pValue, pKey);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 获取商品配送区域
        /// </summary>
        /// <param name="pid">上级编号</param>
        /// <param name="level">等级</param>
        public List<DispatchArea> GetWareArea(int pid, int level)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("select * from DispatchArea where parentid = {0} and level = {1}", pid, level);
                List<DispatchArea> myAreas = db.Query<DispatchArea>(sql, null);
                db.Commit();
                return myAreas;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
                return null;
            }
        }

        /// <summary>
        /// 根据编号获取商品区域
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        public string GetWareArea(string aid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("select name from DispatchArea where id = {0}", aid);
                string name = db.QueryOneCol<string>(sql);
                db.Commit();
                return name;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
                return null;
            }
        }

        #endregion SysParams

        /// <summary>
        /// 更新商品海报数据
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="imgPath"></param>
        public void WareImageUpdate(string pid, string imgPath, string imgWebPath)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("update ProductInfo set ProductImagePath = '{0}',ProductImageWebPath = '{1}' where ProductID = '{2}';", imgPath, imgWebPath, pid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        #region 商品评价信息
        /// <summary>
        /// 添加商品评价信息
        /// </summary>
        /// <param name="msgs"></param>
        public void WareMessagesAdd(List<ProductMessage> msgs)
        {
            if (msgs != null && msgs.Count > 0)
            {
                WareMessageClear(msgs[0].PID);
                foreach (var item in msgs)
                {
                    WareMessageAdd(item);
                }
            }
        }
        /// <summary>
        /// 清除商品评价信息
        /// </summary>
        /// <param name="pid"></param>
        private void WareMessageClear(string pid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                db.Exec(string.Format("delete from ProductMessage where PID = '{0}'", pid));
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 添加评价信息
        /// </summary>
        /// <param name="msg"></param>
        public void WareMessageAdd(ProductMessage msg)
        {
            if (msg == null)
            {
                return;
            }
            OtDB db = GetDb();
            try
            {
                db.Begin();
                //db.Exec(string.Format("delete from ProductMessage where PID = '{0}'", msg.PID));
                db.ExecInsert(msg, "ProductMessage", new string[] { "PID", "MsgType", "MsgContent", "MsgUser", "MsgUserLevel", "MsgProvince", "MsgDate" });
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 获取评价信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public List<ProductMessage> WareMessageGet(string pid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("select * from ProductMessage where PID = '{0}'", pid);
                List<ProductMessage> msgs = db.Query<ProductMessage>(sql);
                db.Commit();
                return msgs;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
                return null;
            }
        }
        /// <summary>
        /// 更新评价统计信息
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <param name="count">评价总数</param>
        /// <param name="good">好评</param>
        /// <param name="general">中评</param>
        /// <param name="poor">差评</param>
        /// <param name="tag">印象</param>
        public void WareMessageUpdate(string pid, int count, double good, double general, double poor, string tag)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("update ProductInfo set ProductEvaluateCount = {0}, ProductGoodRate = {1}, ProductPoorRate = {2}," +
                "ProductGeneralRate = {3}, ProductHotCommentTag = '{4}' where ProductID = '{5}';", count, good, general, poor, tag, pid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }
        #endregion 商品评价信息

        #region jdTypes

        /// <summary>
        /// 清空京东商品类别数据
        /// </summary>
        public void WareJDTypeClear()
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                db.Exec("delete from JDWareType");
                db.Commit();

            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }
        /// <summary>
        /// 添加京东商品类别
        /// </summary>
        /// <param name="_types"></param>
        public void WareJDTypeAdd(List<JDWareType> _types)
        {
            if (_types == null && _types.Count == 0)
            {
                return; 
            }
            OtDB db = GetDb();
            try
            {
                foreach (var item in _types)
                {
                    db.Begin();
                    db.ExecInsert(item, "JDWareType", new string[] { "TypeID", "TypeName", "TopID", "TypeUrl", "TypeLevel", "BEnable" });
                    db.Commit();
                }
                
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        /// <summary>
        /// 获取京东商品分类数据
        /// </summary>
        /// <param name="wherestr"></param>
        /// <returns></returns>
        public List<JDWareType> GetWareJDTypes(string wherestr)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("select * from JDWareType where BEnable = 1 {0}", wherestr);
                List<JDWareType> rtns = db.Query<JDWareType>(sql);
                db.Commit();
                return rtns;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
                return null;
            }

        }
        #endregion jdTypes

        #region 商品促销信息
        public void WarePromotionUpdate(string pid,string tag, string promo,string copon)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("update ProductInfo set ProductTag = '{0}',ProductPromoMsg = '{1}',ProductCoupon = '{2}' where ProductID = '{3}';", tag, promo, copon, pid);
                db.Exec(sql);
                db.Commit();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                db.Rollback();
            }
        }

        #endregion 商品促销信息
    }
}
