using Hank.ComLib;
using WareDealer.Mode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareDealer.Helper
{
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
                db.ExecInsert(_myProduct, "ProductInfo", new string[] { "RID", "ProductID", "VenderId", "ShopId", "ProductName", "ProductPrice",
                    "ProductIsSaled", "ProductType", "ProductTag","ProductEvaluateCount", "ProductGoodRate", "CatArea", "ProductImageWebPath",
                     "ProductImagePath", "ProductURL", "ProductDetail", "ProductBrand", "ProductBasePrice", "ProductAttach", "CreateTime", "CreateUser", "BEnable" });
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
                db.ExecUpdate(_myProduct, "ProductInfo", new string[] { "ProductID", "VenderId", "ShopId", "ProductName", "ProductPrice",
                    "ProductIsSaled", "ProductType", "ProductTag","ProductEvaluateCount", "ProductGoodRate", "CatArea", "ProductImageWebPath",
                     "ProductImagePath", "ProductURL", "ProductDetail", "ProductBrand", "ProductAttach"}, 
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
                string sql = "select a.*,(case a.ProductIsSaled when -1 then '下柜' when 1 then '有货' when 2 then '配货' when 3 then '预订' else '无货' end) as ProductStock, b.Name as TypeName from ProductInfo a left join Dic_ProductType b on a.ProductType = b.TID order by a.CreateTime";
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

        public List<ProductInfo> WareGetAll(string typeid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                //分页查询
                //select * from users order by id limit 10 offset 0;//offset代表从第几条记录“之后“开始查询，limit表明查询多少条结果
                //select * from account limit10,9
                string sql = string.Format("select a.*,(case a.ProductIsSaled when -1 then '下柜' when 1 then '有货' when 2 then '配货' else '无货' end) as ProductStock, "
                    + "b.Name as TypeName from ProductInfo a left join Dic_ProductType b on a.ProductType = b.TID where a.ProductType = '{0}' order by a.CreateTime",typeid);
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
        /// 删除单个商品
        /// </summary>
        /// <param name="rid"></param>
        public void WareDelOne(string pid)
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

        public List<Dic_ProductType> WareTypeGet()
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                //分页查询
                //select * from users order by id limit 10 offset 0;//offset代表从第几条记录“之后“开始查询，limit表明查询多少条结果
                //select * from account limit10,9
                string sql = "select TID,Name from Dic_ProductType order by CreateTime";
                List<Dic_ProductType> rtnList = db.Query<Dic_ProductType>(sql, null);
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
                    string sql = string.Format("insert into ProductPriceHistory (PID,Price,PriceDate,PriceType) select '{0}',{1},'{2}','{3}'", pid, price, dt.ToString("s"), origin);
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

        

        /// <summary>
        /// 更新商品价格
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="price"></param>
        public void WarePriceUpdate(string pid,double price)
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
        /// 更新商品价格及库存
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <param name="price">价格</param>
        /// <param name="stock">库存</param>
        public void WareRepositoryUpdate(string pid, double price, int stock)
        {
            string trend = WarePriceTrend(pid, price);
            OtDB db = GetDb();
            try
            {
                db.Begin();
                //价格对比 --> 涨价、持平、走低、未知
                string sql = string.Format("update ProductInfo set ProductPrice = {0},ProductPriceDate = DateTime('now'),ProductIsSaled={1},ProductPriceTrend='{2}' where ProductID = '{3}';", price, stock, trend, pid);
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
        /// <param name="cat">配送区域</param>
        public void WareShopUpdate(string pid, string vid, string cat)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("update ProductInfo set VenderId = '{0}',CatArea = '{1}' where ProductID = '{2}';", vid, cat, pid);
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

        public void UpdateHistoryPriceBaseAll()
        {

        }

        public void GetMinPrices()
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("select pid,min(price) minprice from ProductPriceHistory group by pid");
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

    }
}
