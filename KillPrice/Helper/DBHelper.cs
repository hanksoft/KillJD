using Hank.ComLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WareDealer.Mode;

namespace KillPrice.Helper
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
        /// 插入商品信息
        /// </summary>
        /// <param name="_myProduct"></param>
        public void WareInsert(ProductInfo _myProduct)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                db.ExecInsert(_myProduct, "ProductInfo", new string[] { "RID","ProductID", "ProductName", "ProductPrice","ProductIsSaled",
                    "ProductEvaluateCount", "ProductGoodRate", "ProductImagePath", "ProductURL", "ProductDetail","ProductBrand",
                    "ProductAttach","NativeData","CreateTime","CreateUser","BEnable" });
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
                db.ExecUpdate(_myProduct, "ProductInfo", new string[] {"ProductID", "ProductName", "ProductPrice","ProductIsSaled",
                    "ProductTag", "ProductBrand", "ProductAttach", "ProductType","ProductImagePath"}, string.Format(" ProductID = '{0}'", _myProduct.ProductID), null);
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
                string sql = "select a.*,(case a.ProductIsSaled when 1 then '有货' when 2 then '配货' else '无货' end) as ProductStock, b.Name as TypeName from ProductInfo a left join Dic_ProductType b on a.ProductType = b.TID order by a.CreateTime";
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
                string sql = string.Format("select a.*,(case a.ProductIsSaled when 1 then '有货' when 2 then '配货' else '无货' end) as ProductStock, "
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
        /// <param name="rid">商品编号</param>
        /// <returns></returns>
        public ProductInfo WareGetOne(string rid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("select * from ProductInfo where rid='{0}'", rid);
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
        public void WareDelOne(string rid)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                string sql = string.Format("delete from ProductInfo where rid ='{0}'", rid);
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
        /// <param name="pid">商品编号</param>
        /// <param name="price">商品价格</param>
        /// <param name="dt">入库时间</param>
        public void WarePriceInsert(string pid, double price, DateTime dt)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                //sqlite用的全球时间UTC,要用datetime()函数转换若干 localtime UTC
                //用data_time.ToString("s");这种方法转换成 iso 8601标准字符串格式
                string sql = string.Format("insert into ProductPriceHistory (PID,Price,PriceDate) select '{0}',{1},'{2}'", pid, price, dt.ToString("s"));
                db.Exec(sql);
                db.Commit();

            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
            }
        }

        public void WarePriceInsert(ProductPriceHistory price)
        {
            OtDB db = GetDb();
            try
            {
                db.Begin();
                //sqlite用的全球时间UTC,要用datetime()函数转换若干 localtime UTC
                //用data_time.ToString("s");这种方法转换成 iso 8601标准字符串格式
                string sql = string.Format("insert into ProductPriceHistory (PID,Price,PriceDate,PriceType) select '{0}',{1},'{2}','{3}'", price.PID, price.Price, price.PriceDate.ToString("s"), price.PriceType);
                db.Exec(sql);
                db.Commit();

            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                //db.Rollback();
            }
        }

        /// <summary>
        /// 插入商品价格
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <param name="price">商品价格</param>
        public void WarePriceInsert(string pid, double price)
        {
            WarePriceInsert(pid, price, DateTime.Now);
            //OtDB db = GetDb();
            //try
            //{
            //    db.Begin();
            //    string sql = string.Format("insert into ProductPriceHistory (PID,Price,PriceDate) select '{0}',{1},datetime('now')", pid, price);
            //    db.Exec(sql);
            //    db.Commit();

            //}
            //catch (Exception ex)
            //{
            //    OtCom.XLogErr(ex.Message);
            //    db.Rollback();
            //}
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
        /// 获取商品历史价格
        /// </summary>
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
                string sql = string.Format("select rowid as RID,*,datetime(PriceDate) as PriceTime from ProductPriceHistory where pid = '{0}' order by PriceDate", pid);
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
        #endregion Price Manage

    }
}
