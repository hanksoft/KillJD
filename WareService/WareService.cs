using WareDealer.Helper;
using WareDealer.Mode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using Hank.ComLib;
using System.ComponentModel;
using Hank.BrowserParse;

namespace WareDealer
{
    [DisplayName("商品服务")]
    public class WareService
    {
        private WareService() { }
        private ProductInfo _myProduct = null;

        private static WareService _wareService;

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static WareService GetInstance()
        {
            return _wareService ?? (_wareService = new WareService());
        }

        /// <summary>
        /// 根据商品编号获取商品信息
        /// </summary>
        /// <param name="pid"></param>
        public ProductInfo GetWareInfoByID(string pid)
        {
            if (string.IsNullOrEmpty(pid))
            {
                return null;
            }
            string url = string.Format("http://item.jd.com/{0}.html", pid);
            return GetWareInfo(url);
        }
        /// <summary>
        /// 获取商品页面具备信息
        /// </summary>
        /// <param name="url">访问网址</param>
        /// <param name="isNew">是否新商品</param>
        /// <returns></returns>
        private ProductInfo GetWareBaseInfo(string url, bool isNew)
        {
            try
            {
                string tID = Regex.Match(url, "\\d{1,14}").Value;
                string html = HttpHelper.GetResponseGBK(url, "get", string.Empty);
                if (string.IsNullOrEmpty(html))
                {
                    //MessageBox.Show("未找到商品！", "系统提示");
                    return null;
                }
                //获取商品名称
                //string ss = Regex.Match(html, @"<title>[^<]*</title>").Value;
                //Regex reg = new Regex("<title>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                string sRegexReturn = Regex.Match(html, @"\<title[^\>]*\>\s*(?<Title>.*?)\s*\</title\>").Value;
                if (sRegexReturn.IndexOf("京东(JD.COM)") >= 0)
                {
                    //MessageBox.Show("未找到编号为" + tID + "的商品！", "系统提示");
                    return null;
                }
                if (isNew)
                {
                    _myProduct = new ProductInfo();
                    _myProduct.RID = Guid.NewGuid().ToString();
                    _myProduct.ProductID = tID;
                    _myProduct.ProductURL = url;
                }
                _myProduct.NativeData = html;
                _myProduct.PageConfig = Regex.Match(html, @"var pageConfig = {\w*};").Value;

                //_myProduct.ProductName = GetTitleContent(sRegexReturn, "title").Replace(extTitle, "");
                _myProduct.ProductName = GetTitleContent(sRegexReturn);

                //获取商品库存情况
                string stock = Regex.Match(_myProduct.NativeData, @"<h3><strong>该商品已下柜，非常抱歉！</strong></h3>").Value;
                if (!string.IsNullOrEmpty(stock))
                {
                    _myProduct.ProductIsSaled = -1;
                    _myProduct.ProductStock = "下柜";
                }
                //TO-DO 暂无报价商品处理

                //获取商品归属类型 自营、旗舰店、第三方
                //根据商品编号取值不同来判断商品归属
                //自营 venderId:0 shopId:'0'
                //旗舰店 venderId:1000001624 shopId:'1000001624'
                //第三方 venderId:153108 shopId:'148186'
                //cat: [9855,9857,9908]
                //skuid: 202459,
                //skuidkey:'F7057726EEDA872B4FF09D080FB941D5',
                //brand: 18374,
                //\s* 任意空白字符(可能是0次) \s+ 匹配重复1次或更多次
                string skuId = Regex.Match(html, @"skuid:\s*(\d{1,14})").Value.Replace("skuid:", "").Trim();
                string skuidKey = Regex.Match(html, @"skuidkey:\s*'\w*'").Value.Replace("skuidkey:", "").Trim().Replace("'","");
                string venderId = Regex.Match(html, "venderId:(\\d{1,14})").Value.Replace("venderId:", "");
                string shopId = Regex.Match(html, "shopId:'(\\d{1,14})'").Value.Replace("shopId:'", "").TrimEnd('\'');
                if (string.IsNullOrEmpty(shopId))
                {
                    shopId = Regex.Match(html, @"shopId:(\d{1,14})").Value.Replace("shopId:", "");
                }
                string cat = Regex.Match(html, @"cat: \[(\d{1,5}),(\d{1,5}),(\d{1,5})\]").Value.Replace("cat: [", "").TrimEnd(']');
                string brandid = Regex.Match(html, @"brand:\s*(\d{1,10})").Value.Replace("brand:", "").Trim();
                if (!string.IsNullOrEmpty(venderId))
                {
                    _myProduct.ProductAttach = venderId != shopId ? "第三方" : venderId == "0" ? "自营" : "旗舰店";
                    _myProduct.SkuID = skuId;
                    _myProduct.SkuidKey = skuidKey;
                    _myProduct.ShopId = shopId;
                    _myProduct.VenderId = venderId;
                    _myProduct.Catalog = cat; 
                    _myProduct.BrandID = brandid;
                }
                return _myProduct;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 获取商品扩展信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ProductInfo GetWareInfo(string url)
        {
            //校验URL是否符合规范
            if (Regex.Match(url, "http://item.jd.com/(\\d{1,14}).html", RegexOptions.IgnoreCase).Success)
            {
                try
                {
                    ProductInfo rtnWare = GetWareBaseInfo(url,true);
                    if (rtnWare == null)
                    {
                        return null;
                    }

                    //根据采集模式返回对应数据
                    return (SysParams.GatherModel == GatherType.Batch ? GetBatchWareInfo() : GetSingleWareInfo());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
            return null;
        }
        /// <summary>
        /// 商品批量模式信息获取
        /// </summary>
        /// <returns></returns>
        private ProductInfo GetBatchWareInfo()
        {
            try
            {
                //采集渠道判断
                switch (_myProduct.ProductAttach)
                {
                    case "第三方":
                        if (!SysParams.Gather3Ware)
                        {
                            return null;
                        }
                        break;
                    case "自营":
                        if (!SysParams.GatherJDWare)
                        {
                            return null;
                        }
                        break;
                    case "旗舰店":
                        if (!SysParams.GatherQJWare)
                        {
                            return null;
                        }
                        break;
                }

                //采集参数判断
                if (SysParams.GetWarePicture)
                {
                    //获取图片
                    string sRegexReturn = GetImgPath(_myProduct.NativeData);
                    if (!string.IsNullOrEmpty(sRegexReturn))
                    {
                        _myProduct.ProductImagePath = sRegexReturn;
                        _myProduct.ProductImageWebPath = sRegexReturn;
                        _myProduct.ProductImage = GetRemoteImage(sRegexReturn);
                    }
                }

                _myProduct.ProductPriceTrend = "持平";
                _myProduct.ProductPriceType = "京东";

                if (_myProduct.ProductStock != "下柜")
                {
                    if (SysParams.GetWareStock)
                    {
                        //商品库存
                        GetWareService(_myProduct.ProductID, _myProduct.VenderId, _myProduct.Catalog, SysParams.DispathArea);
                    }
                    if (SysParams.GetWarePrice)
                    {
                        //商品价格
                        _myProduct.ProductPrice = GetWarePriceByID(_myProduct.ProductID);
                        _myProduct.ProductBasePrice = _myProduct.ProductPrice;
                    }
                    if (SysParams.GetWarePostMessage)
                    {
                        //获取评价统计信息
                        GetWareEvaluate(_myProduct.ProductID);
                        //商品评价详细信息
                        GetEvaluateMsg(_myProduct.ProductID, false);
                    }
                    if (SysParams.GetWareCoupon)
                    {
                        //获取促销信息
                        GetWarePromotion(_myProduct.ProductID, _myProduct.VenderId, _myProduct.ShopId, SysParams.DispathArea, _myProduct.Catalog);
                    }
                }

                _myProduct.CreateTime = DateTime.Now;
                _myProduct.CreateUser = string.IsNullOrEmpty(SysParams.UserName) ? "killjd.cn" : SysParams.UserName;
                _myProduct.BEnable = true;

                return _myProduct;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                return null;
            }
            
        }

        /// <summary>
        /// 商品普通模式信息获取
        /// </summary>
        /// <returns></returns>
        private ProductInfo GetSingleWareInfo()
        {
            try
            {
                //获取图片
                string sRegexReturn = GetImgPath(_myProduct.NativeData);
                if (!string.IsNullOrEmpty(sRegexReturn))
                {
                    _myProduct.ProductImagePath = sRegexReturn;
                    _myProduct.ProductImageWebPath = sRegexReturn;
                    _myProduct.ProductImage = GetRemoteImage(sRegexReturn);
                }

                _myProduct.ProductPriceTrend = "持平";
                _myProduct.ProductPriceType = "京东";

                if (_myProduct.ProductStock != "下柜")
                {
                    //商品库存
                    GetWareService(_myProduct.ProductID, _myProduct.VenderId, _myProduct.Catalog, SysParams.DispathArea);
                    //商品价格
                    _myProduct.ProductPrice = GetWarePriceByID(_myProduct.ProductID);
                    _myProduct.ProductBasePrice = _myProduct.ProductPrice;
                    //获取评价统计信息
                    GetWareEvaluate(_myProduct.ProductID);
                    //商品评价详细信息
                    GetEvaluateMsg(_myProduct.ProductID, false);
                    //获取促销信息
                    GetWarePromotion(_myProduct.ProductID, _myProduct.VenderId, _myProduct.ShopId, SysParams.DispathArea, _myProduct.Catalog);
                }

                _myProduct.CreateTime = DateTime.Now;
                _myProduct.CreateUser = string.IsNullOrEmpty(SysParams.UserName) ? "killjd.cn" : SysParams.UserName;
                _myProduct.BEnable = true;

                return _myProduct;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                return null;
            }
            
        }

        /// <summary>
        /// 重载图片
        /// </summary>
        /// <param name="ProductInfo">产品对象</param>
        /// <param name="isSaveToDB">是否入库</param>
        public void ReloadImg(ProductInfo myInfo, bool isSaveToDB)
        {
            try
            {
                _myProduct = myInfo;
                if (string.IsNullOrEmpty(myInfo.ProductImageWebPath))
                {
                    string url = string.Format("http://item.jd.com/{0}.html", myInfo.ProductID);

                    string tID = Regex.Match(url, "\\d{1,14}").Value;
                    string html = HttpHelper.GetResponseGBK(url, "get", string.Empty);
                    if (string.IsNullOrEmpty(html))
                    {
                        return;
                    }
                    //获取商品名称
                    string sRegexReturn = Regex.Match(html, @"\<title[^\>]*\>\s*(?<Title>.*?)\s*\</title\>").Value;
                    if (sRegexReturn.IndexOf("京东(JD.COM)") >= 0)
                    {
                        //MessageBox.Show("未找到编号为" + tID + "的商品！", "系统提示");
                        return;
                    }

                    //获取图片路径
                    sRegexReturn = GetImgPath(html);
                    if (!string.IsNullOrEmpty(sRegexReturn))
                    {
                        myInfo.ProductImagePath = sRegexReturn;
                        myInfo.ProductImageWebPath = sRegexReturn;
                    }
                }

                Bitmap img = GetRemoteImage(myInfo.ProductImageWebPath);
                if (img != null)
                {
                    myInfo.ProductImage = img;
                    if (isSaveToDB)
                    {
                        DBHelper.GetInstance().WareImageUpdate(myInfo.ProductID, myInfo.ProductImagePath, myInfo.ProductImageWebPath);
                    }
                }
                else
                {
                    myInfo.ProductImage = img;
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// 商品配送区域
        /// </summary>
        private void GetWareArea()
        {
            try
            {
                //http://d.jd.com/hotwords/get?Position=A-electronic-011
                string url_evaluate = "http://d.jd.com/hotwords/get?Position=A-electronic-011";
                string html_evaluate = HttpHelper.GetResponseGBK(url_evaluate, "get", string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 获取评价统计数据
        /// </summary>
        /// <param name="tID"></param>
        private void GetWareEvaluate(string tID)
        {
            try
            {
                //评价
                //http://club.jd.com/clubservice.aspx?method=GetCommentsCount&referenceIds=1108248&callback=jQuery4125625&_=1442670780111
                //jQuery4125625({"CommentsCount":[{"SkuId":1108248,"ProductId":1108248,"Score1Count":4,"Score2Count":4,"Score3Count":18,"Score4Count":88,"Score5Count":1226,"ShowCount":72,"CommentCount":1340,"AverageScore":5,"GoodCount":1314,"GoodRate":0.982,"GoodRateShow":98,"GoodRateStyle":148,"GeneralCount":22,"GeneralRate":0.016,"GeneralRateShow":2,"GeneralRateStyle":2,"PoorCount":4,"PoorRate":0.0020,"PoorRateShow":0,"PoorRateStyle":0}]});
                //http://club.jd.com/clubservice.aspx?method=GetCommentsCount&referenceIds=202459&callback=jQuery4556453&_=1451373413300
                //jQuery4556453({"CommentsCount":[{"SkuId":202459,"ProductId":202459,"Score1Count":7,"Score2Count":1,"Score3Count":14,"Score4Count":110,"Score5Count":566,"ShowCount":21,"CommentCount":698,"AverageScore":5,"GoodCount":676,"GoodRate":0.969,"GoodRateShow":97,"GoodRateStyle":145,"GeneralCount":15,"GeneralRate":0.021,"GeneralRateShow":2,"GeneralRateStyle":3,"PoorCount":7,"PoorRate":0.01,"PoorRateShow":1,"PoorRateStyle":2}]});
                string url_evaluate = string.Format("http://club.jd.com/clubservice.aspx?method=GetCommentsCount&referenceIds={0}&callback=jQuery4125625&_=1442670780111", tID);
                string html_evaluate = HttpHelper.GetResponseGBK(url_evaluate, "get", string.Empty);

                //总评价数量
                string sEvaluate = Regex.Match(html_evaluate, "\"CommentCount\":(\\d{1,5})").Value;
                _myProduct.ProductEvaluateCount = int.Parse(sEvaluate.Replace("\"CommentCount\":", ""));
                //好评率
                string sGoodRateShow = Regex.Match(html_evaluate, "\"GoodRateShow\":(\\d{1,3})").Value;
                _myProduct.ProductGoodRate = Double.Parse(sGoodRateShow.Replace("\"GoodRateShow\":", ""));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// 获取评价详细信息
        /// </summary>
        /// <param name="tID"></param>
        public void GetEvaluateMsg(string tID, bool needAllComment)
        {
            //差评
            //http://club.jd.com/productpage/p-255742-s-1-t-3-p-0.html?callback=fetchJSON_comment98vv17736
            //中评
            //http://club.jd.com/productpage/p-255742-s-2-t-3-p-0.html?callback=fetchJSON_comment98vv17736
            //好评
            //http://club.jd.com/productpage/p-255742-s-3-t-3-p-0.html?callback=fetchJSON_comment98vv17736
            try
            {
                string url = string.Format("http://club.jd.com/productpage/p-{0}-s-1-t-3-p-0.html", tID);
                string rtnMsg = HttpHelper.GetResponseGBK(url, "get", string.Empty);

                List<EvaluateMsg> getMsgs = new List<EvaluateMsg>();
                EvaluateMsg badMsg = JsonConvert.DeserializeObject<EvaluateMsg>(rtnMsg);
                if (badMsg != null && badMsg.comments != null && badMsg.comments.Count > 0)
                {
                    _myProduct.ProductEvaluateCount = badMsg.productCommentSummary.commentCount;
                    _myProduct.ProductGoodRate = badMsg.productCommentSummary.goodCount;
                    _myProduct.ProductPoorRate = badMsg.productCommentSummary.poorCount;
                    _myProduct.ProductGeneralRate = badMsg.productCommentSummary.generalCount;
                    _myProduct.ProductHotCommentTag = "";
                    if (badMsg.hotCommentTagStatistics != null && badMsg.hotCommentTagStatistics.Count > 0)
                    {
                        foreach (var item in badMsg.hotCommentTagStatistics)
                        {
                            _myProduct.ProductHotCommentTag += item.name + " ";
                        }
                    }
                    if (needAllComment)
                    {
                        getMsgs.Add(badMsg);
                        int iCount = badMsg.productCommentSummary.poorCount;
                        float iNum = (float)iCount / 10;
                        int pagenum = (int)(iCount / 10);
                        if (pagenum < iNum)
                        {
                            pagenum++;
                        }

                        if (pagenum > 1)
                        {
                            for (int i = 1; i <= pagenum; i++)
                            {
                                url = string.Format("http://club.jd.com/productpage/p-{0}-s-1-t-3-p-{1}.html", tID, i);
                                rtnMsg = HttpHelper.GetResponseGBK(url, "get", string.Empty);
                                if (!string.IsNullOrEmpty(rtnMsg))
                                {
                                    badMsg = JsonConvert.DeserializeObject<EvaluateMsg>(rtnMsg);
                                    if (badMsg != null && badMsg.comments != null && badMsg.comments.Count > 0)
                                    {
                                        getMsgs.Add(badMsg);
                                    }
                                }
                            }
                        }
                    }
                }
                if (getMsgs.Count > 0)
                {
                    List<ProductMessage> msgs = new List<ProductMessage>();
                    foreach (var eMsg in getMsgs)
                    {
                        foreach (var item in eMsg.comments)
                        {
                            ProductMessage msg = new ProductMessage()
                            {
                                PID = tID,
                                MsgType = 1,
                                MsgContent = item.content,
                                MsgUser = item.nickname,
                                MsgUserLevel = item.userLevelName,
                                MsgProvince = item.userProvince,
                                MsgDate = item.creationTime
                            };
                            msgs.Add(msg);
                        }
                    }
                    DBHelper.GetInstance().WareMessagesAdd(msgs);
                }
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
            }
        }

        /// <summary>
        /// 获取服务信息
        /// </summary>
        /// <param name="tID">商品编号</param>
        /// <param name="venderId">供应商编号</param>
        /// <param name="cat">配送区域</param>
        /// <returns>库存标记</returns>
        private int GetWareService(string tID, string venderId, string cat,string dispatch)
        {
            try
            {
                string stock = Regex.Match(_myProduct.NativeData, @"<h3><strong>该商品已下柜，非常抱歉！</strong></h3>").Value;
                if (!string.IsNullOrEmpty(stock))
                {
                    _myProduct.ProductIsSaled = -1;
                    _myProduct.ProductStock = "下柜";
                    return -1;
                }
                //配送服务 22_1930_4283_0
                if (string.IsNullOrEmpty(cat))
                {
                    cat = "6144,12041,12047";
                }
                //http://c0.3.cn/stock?skuId=1700908129&venderId=32533&cat=6144,12041,12047&area=1_72_2799_0&buyNum=1&extraParam={%22originid%22:%221%22}&ch=1&callback=getStockCallback
                //http://c0.3.cn/stock?skuId=852431&venderId=1000000248&cat=670,686,690&area=1_72_2799_0&buyNum=1&extraParam={"originid":"1"}&ch=1&callback=getStockCallback 
                //string url_service = "http://c0.3.cn/stock?skuId=" + tID + "&venderId=" + venderId.Trim() + "&cat=" + cat + "&area=" + dispatch 
                //    + "&buyNum=1&extraParam={%22originid%22:%221%22}&ch=1&callback=getStockCallback";

                string url_service = "http://c0.3.cn/stock?skuId=" + tID + "&venderId=" + venderId.Trim() + "&cat=" + cat + "&area=" + dispatch
                    + "&buyNum=1&extraParam={%22originid%22:%221%22}&ch=1";

                string html_service = HttpHelper.GetResponseGBK(url_service, "get", string.Empty);
                string str_stock = html_service.Replace("getStockCallback(", "").TrimEnd(')');
                if (string.IsNullOrEmpty(str_stock))
                {
                    return  -1;
                }
                StockInfo jdStock = JsonConvert.DeserializeObject<StockInfo>(str_stock);

                if (_myProduct == null)
                {
                    _myProduct = new ProductInfo();
                }
                _myProduct.ProductDispatchMode = ClearHtml(jdStock.Stock.serviceInfo);
                _myProduct.ProductBrand = string.IsNullOrEmpty(jdStock.Stock.self_D.deliver) ? jdStock.Stock.D.deliver : jdStock.Stock.self_D.deliver;
                //-1 下柜 0 无货 1 有货 2 配货 3 预订
                //33 有货(1), 40 可配货(2), 36 预订(3), 34 无货(0), 下柜（-1）
                _myProduct.ProductIsSaled = jdStock.Stock.StockState == 33 ? 1 : (jdStock.Stock.StockState == 40 ? 2 : (jdStock.Stock.StockState == 36 ? 3 : 0)); 
                _myProduct.ProductStock = jdStock.Stock.StockStateName;
                //品牌
                //string _service = Regex.Match(html_service, "\"deliver\":\"\\w*\"").Value.Replace("\"deliver\":\"", "").TrimEnd('\"');
                ////付款方式
                //string warePay = Regex.Match(html_service, "\"iconSrc\":\"\\w*\"").Value.Replace("\"iconSrc\":\"", "").TrimEnd('\"');
                ////库存情况
                //string wareAvailable = Regex.Match(html_service, "\"StockStateName\":\"\\w*\"").Value.Replace("\"StockStateName\":\"", "").TrimEnd('\"');

                //_myProduct.ProductDispatchMode = warePay;
                //_myProduct.ProductIsSaled = wareAvailable.IndexOf("现货") >= 0;
                //_myProduct.ProductStock = wareAvailable;

                return _myProduct.ProductIsSaled;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //-1代表已下柜
                _myProduct.ProductIsSaled = -1;
                return -1;
            }

        }
        
        /// <summary>
        /// 获取商品促销信息
        /// </summary>
        /// <param name="skuid">产品编号</param>
        /// <param name="venderID">商家编号</param>
        /// <param name="shopID">店铺编号</param>
        /// <param name="area">配送区域</param>
        /// <param name="cat">分类信息</param>
        private void GetWarePromotion(string skuid,string venderID,string shopID,string area,string cat)
        {
            try
            {
                //JengineD/1.7.10.1 403 Forbidden
                //http://cd.jd.com/promotion/v2?skuId=1579607941&area=1_72_2799_0&shopId=138013&venderId=142424&cat=1318%2C12102%2C9765&_=1459345921060
                string url = string.Format("http://cd.jd.com/promotion/v2?skuId={0}&area={1}&shopId={2}&venderId={3}&cat={4}&_={5}", skuid, area, shopID, venderID, cat, new Random().Next());
                string rtnStr = HttpHelper.GetResponseGBK(url, "get", string.Empty);
                if (string.IsNullOrEmpty(rtnStr))
                {
                    return;
                }
                PromotionMessage promo = JsonConvert.DeserializeObject<PromotionMessage>(rtnStr);
                if (promo != null)
                {
                    _myProduct.ProductTag = (promo.ads != null && promo.ads.Count > 0) ? ClearHtml(promo.ads[0].ad) : "";
                    _myProduct.ProductPromoMsg = promo.prom.pickOneTag != null && promo.prom.pickOneTag.Count > 0 ? ClearHtml(promo.prom.pickOneTag[0].content) : "";
                    if (promo.skuCoupon != null && promo.skuCoupon.Count > 0)
                    {
                        _myProduct.ProductCoupon = "";
                        foreach (var item in promo.skuCoupon)
                        {
                            _myProduct.ProductCoupon += string.Format("{0}-{1} ", item.quota, item.discount);
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
            }
        }

        #region WarePrice
        /// <summary>
        /// 获取指定商品价格
        /// </summary>
        /// <param name="pid">商品编号</param>
        /// <returns></returns>
        public double GetWarePriceByID(string pid)
        {
            try
            {
                return GetWareMobilePriceByID(pid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }

        }

        private List<ProductInfo> _wares4Batch = new List<ProductInfo>();
        /// <summary>
        /// 批价格解析
        /// </summary>
        /// <param name="url">访问网址</param>
        /// <param name="src">价格来源</param>
        private void BatchParse(string url, WarePriceType src)
        {
            try
            {
                string price = HttpHelper.GetResponseGBK(url, "get", string.Empty);
                if (string.IsNullOrEmpty(price) || price.IndexOf("error") >= 0)
                {

                    //获取价格失败则退出
                    return;
                }

                //网页价格
                List<JdWarePrice> jdPrices = JsonConvert.DeserializeObject<List<JdWarePrice>>(price);
                if (jdPrices != null && jdPrices.Count > 0)
                {
                    foreach (var item in jdPrices)
                    {
                        string pid = item.id.IndexOf('J') >= 0 ? item.id.Replace("J_","") : item.id;
                        ProductInfo tmpWare = _wares4Batch.Find(t => t.ProductID == pid);
                        if (tmpWare == null)
                        {
                            tmpWare = new ProductInfo();
                            tmpWare.ProductID = pid;
                            _wares4Batch.Add(tmpWare);
                        }

                        switch (src)
                        {
                            case WarePriceType.WebPrice:
                                tmpWare.ProductPrice = double.Parse(item.p);
                                break;
                            case WarePriceType.AppPrice:
                                tmpWare.ProductMobilePrice = double.Parse(item.p);
                                break;
                            case WarePriceType.QQPrice:
                                tmpWare.ProductQQPrice = double.Parse(item.p);
                                break;
                            case WarePriceType.WxPrice:
                                tmpWare.ProductWXPrice = double.Parse(item.p);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
            }
            
        }

        /// <summary>
        /// 批量价格获取
        /// </summary>
        /// <param name="pids">商品编号集合</param>
        public void GetBatchWarePrice(List<string> pids)
        {
            //https://pm.3.cn/prices/mgets?origin=2&skuids=1309453,1713088,1191695
            //[{"id":"1309453","p":"209.00","m":"328.00"},{"id":"1713088","p":"118.00","m":"208.00"},{"id":"1191695","p":"148.00","m":"268.00"}]
            if (pids != null && pids.Count > 0)
            {
                try
                {
                    //string skuids = string.Join(",J_", pids.ToArray());
                    string skuids = string.Join(",", pids.ToArray());
                    //网站价格获取
                    //string url = string.Format("https://p.3.cn/prices/mgets?type=1&skuIds=J_{0}", skuids);
                    string url = string.Format("http://pm.3.cn/prices/pcpmgets?skuids={0}&origin=2&source=1&area=1_2800_4134_0&_={1}", skuids, GetTimeStamp());
                    BatchParse(url, WarePriceType.WebPrice);
                    
                    skuids = string.Join(",", pids.ToArray());
                    //手机平台价格获取
                    url = string.Format("https://pm.3.cn/prices/mgets?origin=2&skuids={0}", skuids);
                    BatchParse(url, WarePriceType.AppPrice);
                    
                    //QQ平台价格获取
                    url = string.Format("https://pe.3.cn/prices/mgets?origin=4&skuids={0}", skuids);
                    BatchParse(url, WarePriceType.QQPrice);
                    
                    //微信平台价格获取
                    url = string.Format("https://pe.3.cn/prices/mgets?origin=5&skuids={0}", skuids);
                    BatchParse(url, WarePriceType.WxPrice);
                    
                    if (_wares4Batch != null && _wares4Batch.Count > 0)
                    {
                        foreach (var item in _wares4Batch)
                        {
                            if (item.ProductPrice == -1)
                            {
                                //价格为-1则下柜
                                DBHelper.GetInstance().WareDelOne(item.ProductID);
                            }
                            else
                            {
                                //获取商品当批次最低价
                                JdWarePrice tmpPrice = GetMinPriceSrc(item.ProductPrice, item.ProductMobilePrice, item.ProductQQPrice, item.ProductWXPrice);
                                //与前一批次对比，获得价格趋势
                                //数据存盘
                                DBHelper.GetInstance().WareRepositoryUpdate(item.ProductID, double.Parse(tmpPrice.p), item.ProductPrice, item.ProductMobilePrice, item.ProductQQPrice,
                                    item.ProductWXPrice, tmpPrice.src);
                                DBHelper.GetInstance().WarePriceInsert(item.ProductID, double.Parse(tmpPrice.p), DateTime.Now, tmpPrice.src);
                                DBHelper.GetInstance().UpdateHistoryPriceBasebyID(item.ProductID);
                            }
                        }
                        
                        //清空当前商品列表数据
                        _wares4Batch.Clear();
                    }
                }
                catch (Exception ex)
                {
                    OtCom.XLogErr(ex.Message);
                }
            }
        }

        /// <summary>
        /// 获取手机专享价
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public double GetWareMobilePriceByID(string pid)
        {
            try
            {
                //PC https://p.3.cn/prices/mgets?type=1&skuIds=
                //APP https://pm.3.cn/prices/mgets?origin=2&skuids=
                //QQ https://pe.3.cn/prices/mgets?origin=4&skuids=
                //weixin https://pe.3.cn/prices/mgets?origin=5&skuids=
                //http://item.m.jd.com/ware/thirdAddress.json?checkParam=KKLLTL&wareId={0}&provinceId={1}&cityId={2}&countryId={3}&action=getTowns
                if (_myProduct == null)
                {
                    _myProduct = new ProductInfo();
                }
                double myPrice = 0;
                //商品价格 http://pm.3.cn/prices/pcpmgets?callback=jQuery3820568&skuids=202459&origin=2&source=1&area=1_2800_4134_0&_=1451370905656
                //jQuery5068505([{"id":"202459","pcp":"69.00","p":"59.00","m":"121.00"}]);

                //通过接口获取所有平台价格
                //http://p.3.cn/prices/mgets?type=1&skuIds=J_10221286883,J_2031542,J_1433346,J_1079888,J_2326463,J_1618201,J_1905327,J_10221540634&callback=jQuery6726231&_=1459406187522
                //string pcPrice = string.Format("https://p.3.cn/prices/mgets?type=1&skuIds=J_{0}", pid);
                //string pcPrice1 = HttpHelper.GetResponseGBK(pcPrice, "get", string.Empty).Replace("[","").Replace("]\n","");
                //JdWarePrice pcPrices = JsonConvert.DeserializeObject<JdWarePrice>(pcPrice1);
                //if (pcPrices != null)
                //{
                //    _myProduct.ProductPrice = double.Parse(pcPrices.p);
                //}

                string appPrice = string.Format("https://pm.3.cn/prices/mgets?origin=2&skuids={0}", pid);
                string appPrice1 = HttpHelper.GetResponseGBK(appPrice, "get", string.Empty).Replace("[", "").Replace("]\n", "");
                JdWarePrice appPrices = JsonConvert.DeserializeObject<JdWarePrice>(appPrice1);
                if (appPrices != null)
                {
                    _myProduct.ProductMobilePrice = double.Parse(appPrices.p);
                }

                string qqPrice = string.Format("https://pe.3.cn/prices/mgets?origin=4&skuids={0}", pid);
                string qqPrice1 = HttpHelper.GetResponseGBK(qqPrice, "get", string.Empty).Replace("[", "").Replace("]\n", "");
                JdWarePrice qqPrices = JsonConvert.DeserializeObject<JdWarePrice>(qqPrice1);
                if (qqPrices != null)
                {
                    _myProduct.ProductQQPrice = double.Parse(qqPrices.p);
                }

                string wxPrice = string.Format("https://pe.3.cn/prices/mgets?origin=5&skuids={0}", pid);
                string wxPrice1 = HttpHelper.GetResponseGBK(wxPrice, "get", string.Empty).Replace("[", "").Replace("]\n", "");
                JdWarePrice wxPrices = JsonConvert.DeserializeObject<JdWarePrice>(wxPrice1);
                if (wxPrices != null)
                {
                    _myProduct.ProductWXPrice = double.Parse(wxPrices.p);
                }
                //判断哪个价格最低，最低值作为返回值
                double minPrice = GetMinPrice(_myProduct.ProductPrice, _myProduct.ProductMobilePrice, _myProduct.ProductQQPrice, _myProduct.ProductWXPrice);
                //正常途径获取价格
                //用于验证价格数据
                string url_price = string.Format("http://pm.3.cn/prices/pcpmgets?callback=jQuery3820568&skuids={0}&origin=2&source=1&area=1_2800_4134_0&_=1451370905656", pid);
                string html_price = HttpHelper.GetResponseGBK(url_price, "get", string.Empty);
                if (string.IsNullOrEmpty(html_price))
                {
                    return 0;
                }
                string str_price = html_price.Replace("jQuery3820568([", "").Replace("]);", "");

                JdWareMobilePrice price = JsonConvert.DeserializeObject<JdWareMobilePrice>(str_price);
                myPrice = double.Parse(price.p);
                //规则：如果价格小于0，该商品下柜
                if (myPrice < 0)
                {
                    myPrice = 0;
                    _myProduct.ProductIsSaled = -1;
                    _myProduct.ProductStock = "下柜";
                }
                //if (!string.IsNullOrEmpty(price.pcp))
                //{
                //    _myProduct.ProductPriceType = "手机";
                //}
                _myProduct.ProductPrice = myPrice;

                return (myPrice == 0 ? 0 : minPrice);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        private JdWarePrice GetMinPriceSrc(double price1, double price2, double price3, double price4)
        {
            JdWarePrice tmpJDPrice = new JdWarePrice();
            tmpJDPrice.p = price1.ToString();
            tmpJDPrice.src = "京东";
            //加入double数组
            double[] listPrice = new double[4];
            listPrice[0] = price1;
            listPrice[1] = price2;
            listPrice[2] = price3;
            listPrice[3] = price4;

            //存储临时的需要冒泡的值
            double tempPrice = listPrice.Max();

            for (int i = 0; i < listPrice.Length; i++)
            {
                //当值为0时，取最大值代替
                if (listPrice[i] == 0)
                {
                    listPrice[i] = tempPrice;
                }
            }

            //tempPrice = listPrice.Min();

            tempPrice = listPrice[0];
            //价格索引，用于判断是哪个平台价格最低
            int tempIndex = 0;
            //从数组的第一个值遍历到倒数第二个值
            for (int i = 1; i < listPrice.Length; i++)
            {
                if (listPrice[i] < tempPrice)
                {
                    tempPrice = listPrice[i];
                    tempIndex = i;
                }
                ////从比i大1的值开始遍历到结束
                ////这里比较的总是比i大的值，因为之前的值已经冒泡完成
                //for (int j = i + 1; j < listPrice.Length; j++)
                //{
                //    //如果前一个值大于后一个值，他们交换位置
                //    if (listPrice[i] > listPrice[j])
                //    {
                //        //交换位置
                //        tempPrice = listPrice[i];
                //        listPrice[i] = listPrice[j];
                //        listPrice[j] = tempPrice;
                //    }
                //}
            }
            switch (tempIndex)
            {
                case 0:
                    tmpJDPrice.src = "京东";
                    break;
                case 1:
                    tmpJDPrice.src = "手机";
                    break;
                case 2:
                    tmpJDPrice.src = "QQ";
                    break;
                case 3:
                    tmpJDPrice.src = "微信";
                    break;
            }
            tmpJDPrice.p = tempPrice.ToString();

            return tmpJDPrice;
        }
        /// <summary>
        /// 冒泡算法求最小非0值
        /// </summary>
        /// <param name="price1">网站价格</param>
        /// <param name="price2">APP价格</param>
        /// <param name="price3">QQ价格</param>
        /// <param name="price4">微信价格</param>
        /// <returns>最小价格</returns>
        private double GetMinPrice(double price1, double price2, double price3, double price4)
        {
            //加入double数组
            double[] listPrice = new double[4];
            listPrice[0] = price1;
            listPrice[1] = price2;
            listPrice[2] = price3;
            listPrice[3] = price4;
            
            //存储临时的需要冒泡的值
            double tempPrice = listPrice.Max();
            
            for (int i = 0; i < listPrice.Length; i++)
            {
                //当值为0时，取最大值代替
                if (listPrice[i] == 0)
                {
                    listPrice[i] = tempPrice;
                }
            }

            //tempPrice = listPrice.Min();

            tempPrice = listPrice[0];
            //价格索引，用于判断是哪个平台价格最低
            int tempIndex = 0;
            //从数组的第一个值遍历到倒数第二个值
            for (int i = 1; i < listPrice.Length; i++)
            {
                if (listPrice[i] < tempPrice)
                {
                    tempPrice = listPrice[i];
                    tempIndex = i;
                }
                ////从比i大1的值开始遍历到结束
                ////这里比较的总是比i大的值，因为之前的值已经冒泡完成
                //for (int j = i + 1; j < listPrice.Length; j++)
                //{
                //    //如果前一个值大于后一个值，他们交换位置
                //    if (listPrice[i] > listPrice[j])
                //    {
                //        //交换位置
                //        tempPrice = listPrice[i];
                //        listPrice[i] = listPrice[j];
                //        listPrice[j] = tempPrice;
                //    }
                //}
            }
            switch (tempIndex)
            {
                case 0:
                    _myProduct.ProductPriceType = "京东";
                    break;
                case 1:
                    _myProduct.ProductPriceType = "手机";
                    break;
                case 2:
                    _myProduct.ProductPriceType = "QQ";
                    break;
                case 3:
                    _myProduct.ProductPriceType = "微信";
                    break;
            }
            return tempPrice;
        }

        /// <summary>
        /// 更新商品信息
        /// </summary>
        /// <param name="pid">商品编号</param>
        public void UpdateWarePriceByID(string pid)
        {
            if (!string.IsNullOrEmpty(pid))
            {
                double rtnPrice = GetWarePriceByID(pid);
                if (rtnPrice > 0)
                {
                    DBHelper.GetInstance().WarePriceInsert(pid, rtnPrice);
                    DBHelper.GetInstance().WarePriceUpdate(pid, rtnPrice);
                }
            }
        }
        #endregion WarePrice

        /// <summary>
        /// 更新商品信息
        /// </summary>
        /// <param name="ware">商品对象</param>
        public void UpdateWareinfo(ProductInfo ware)
        {
            if (ware != null)
            {
                _myProduct = ware;
                //获取页面最新信息
                GetWareBaseInfo(ware.ProductURL, false);
                //如果店铺编号或厂商编号为空则不处理
                if (string.IsNullOrEmpty(ware.VenderId) || string.IsNullOrEmpty(ware.ShopId))
                {
                    //更新商品异常信息（前期可能未获取的数据）
                    DBHelper.GetInstance().WareShopUpdate(_myProduct.ProductID, _myProduct.VenderId, _myProduct.ShopId, _myProduct.Catalog);
                    //return;
                }
                //下柜商品自动进入回收站
                if (_myProduct.ProductStock == "下柜" && SysParams.AllowAutoGC)
                {
                    DBHelper.GetInstance().WareDelOne(_myProduct.ProductID);
                    return;
                }

                //判断是否更新商品库存信息
                if (_myProduct.ProductStock != "下柜" && SysParams.AllowGetStock)
                {
                    //获取库存情况
                    int sale = GetWareService(_myProduct.ProductID, _myProduct.VenderId, _myProduct.Catalog, SysParams.DispathArea);
                    //如果库存为下柜状态，则商品价格为0
                    if (sale == -1)
                    {
                        DBHelper.GetInstance().WareRepositoryUpdate(_myProduct.ProductID, 0, 0, 0, 0, "", -1);
                        return;
                    }
                    else
                    {
                        DBHelper.GetInstance().WareRepositoryUpdate(_myProduct.ProductID, _myProduct.ProductDispatchMode);
                    }
                }

                //判断是否更新商品价格信息
                if (_myProduct.ProductStock != "下柜" && SysParams.AllowGetPrice)
                {
                    //获取商品价格
                    double rtnPrice = GetWarePriceByID(_myProduct.ProductID);
                    if (rtnPrice > 0)
                    {
                        DBHelper.GetInstance().WarePriceInsert(_myProduct.ProductID, rtnPrice, DateTime.Now, _myProduct.ProductPriceType);
                        DBHelper.GetInstance().WareRepositoryUpdate(_myProduct.ProductID, rtnPrice, _myProduct.ProductMobilePrice, _myProduct.ProductQQPrice,
                            _myProduct.ProductWXPrice, _myProduct.ProductPriceType, _myProduct.ProductIsSaled);
                        DBHelper.GetInstance().UpdateHistoryPriceBasebyID(_myProduct.ProductID);
                    }
                }
                
                if (_myProduct.ProductStock != "下柜" && SysParams.AllowGetPostMessage)
                {
                    //获取评价统计信息
                    GetEvaluateMsg(_myProduct.ProductID, false);
                    DBHelper.GetInstance().WareMessageUpdate(_myProduct.ProductID, _myProduct.ProductEvaluateCount, _myProduct.ProductGoodRate, 
                        _myProduct.ProductGeneralRate, _myProduct.ProductPoorRate, _myProduct.ProductHotCommentTag);
                }

                if (_myProduct.ProductStock != "下柜" && (SysParams.AllowGetPromo || SysParams.AllowGetCoupon))
                {
                    //获取促销信息
                    GetWarePromotion(_myProduct.ProductID, _myProduct.VenderId, _myProduct.ShopId, SysParams.DispathArea, _myProduct.Catalog);
                    DBHelper.GetInstance().WarePromotionUpdate(_myProduct.ProductID, _myProduct.ProductTag, _myProduct.ProductPromoMsg, _myProduct.ProductCoupon);
                }
                
            }


        }

        /// <summary>
        /// 获取图片路径
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public string GetImgPath(string strHtml)
        {
            //获取网络图片
            //Image O_Image = Image.FromStream(WebRequest.Create("http://www.baidu.com/img/baidu_logo.gif").GetResponse().GetResponseStream());
            //将获取的图片赋给图片框
            //pictureBox1.Image = O_Image;

            //{
            //    WebRequest request = WebRequest.Create(@"http://keleyi.com/image/a/tvh00n12.jpg");
            //    request.Credentials = CredentialCache.DefaultCredentials;
            //    Stream s = request.GetResponse().GetResponseStream();

            //    byte[] b = new byte[74373];
            //    MemoryStream mes_keleyi_com = new MemoryStream(b);
            //    s.Read(b, 0, 74373);
            //    s.Close();
            //    Image image = Image.FromStream(mes_keleyi_com);
            //    Console.WriteLine("高{0}，宽{1}", image.Height, image.Width);
            //    Console.WriteLine("更多信息请访问keleyi.com，按任意健退出 - 柯乐义");
            //    Console.ReadKey();
            //}

            //http://img10.360buyimg.com/n1/1154/91de27bc-02b5-4e10-865f-92ae9f26616e.jpg 大图
            //http://img10.360buyimg.com/n5/1154/91de27bc-02b5-4e10-865f-92ae9f26616e.jpg 小图
            //<img data-img="1" width="350" height="350" src="//img10.360buyimg.com/n1/1154/91de27bc-02b5-4e10-865f-92ae9f26616e.jpg" alt="伊莱克斯（Electrolux） ETS3000 烤面包机 多士炉 带防尘盖 黑色 "/>
            //<img data-img="1" width="350" height="350" src="//img14.360buyimg.com/n1/jfs/t889/17/903106213/86480/3332b359/55530cbaN5d99e2e2.jpg" alt="3M 口罩 KN90 自吸过滤式 防颗粒物呼吸器 有呼气阀 25/盒 9001V"/>
            //Regex regex = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            //图片大小可能不是350时无法获取到图片路径
            //string imgUrl = Regex.Match(strHtml, "<img data-img=\"1\" width=\"350\" height=\"350\" src=\"\\S+\"", RegexOptions.IgnoreCase).Value;

            string imgTag = Regex.Match(strHtml, "<img data-img=\"1\" width=\"\\d{1,3}\" height=\"\\d{1,3}\" src=\"\\S+\"", RegexOptions.IgnoreCase).Value;
            // 搜索匹配的字符串 
            //MatchCollection matches = regex.Matches(str);
            //int i = 0;
            //string[] sUrlList = new string[matches.Count];
            //// 取得匹配项列表 
            //foreach (Match match in matches)
            //{
            //    sUrlList[i++] = match.Groups["imgUrl"].Value;
            //}
            if (string.IsNullOrEmpty(imgTag))
            {
                imgTag = Regex.Match(strHtml, "<img data-img=\"1\" src=\"\\S+\" ", RegexOptions.IgnoreCase).Value;
            }

            string imgUrl = Regex.Match(imgTag, "src=\"\\S+\"").Value;

            return string.IsNullOrEmpty(imgUrl) ? null : imgUrl.Replace("src=\"", "").TrimEnd('\"');
        }

        private string _imgPath = "";
        /// <summary>
        /// 获取远端图片
        /// </summary>
        /// <param name="imgSrc">远端图片地址</param>
        /// <returns></returns>
        public Bitmap GetRemoteImage(string imgSrc)
        {
            _imgPath = Path.Combine(Environment.CurrentDirectory, "Images");
            if (!Directory.Exists(_imgPath))
            {
                Directory.CreateDirectory(_imgPath);
            }

            Bitmap img = null;
            HttpWebRequest req;
            HttpWebResponse res = null;
            try
            {
                string ext = Path.GetExtension(imgSrc);
                System.Uri httpUrl = new System.Uri("http:" + imgSrc);
                req = (HttpWebRequest)(WebRequest.Create(httpUrl));
                req.Timeout = 180000; //设置超时值10秒
                //req.UserAgent = "XXXXX";
                //req.Accept = "XXXXXX";
                req.Method = "GET";
                res = (HttpWebResponse)(req.GetResponse());
                img = new Bitmap(res.GetResponseStream());//获取图片流  

                _myProduct.ProductImagePath = Path.Combine("Images", DateTime.Now.ToFileTime().ToString() + ext);
                img.Tag = _myProduct.ProductImagePath;
                img.Save(Path.Combine(Environment.CurrentDirectory, _myProduct.ProductImagePath));//随机名

                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imageUrl);
                //request.Method = "GET";
                //request.Proxy = null;
                //response = request.GetResponse();
                //stream = response.GetResponseStream();
                //MemoryStream ms = new MemoryStream();
                //if (!response.ContentType.ToLower().StartsWith("text/"))
                //{
                //    byte[] bytes = new byte[4096];
                //    int count = stream.Read(bytes, 0, 4096);
                //    while (count > 0)
                //    {
                //        ms.Write(bytes, 0, count);
                //        count = stream.Read(bytes, 0, 4096);
                //    }
                //    return ms.GetBuffer();
                //}
                //return null;
            }

            catch (Exception ex)
            {
                string aa = ex.Message;
            }
            finally
            {
                res.Close();
            }
            return img;
        }

        /// <summary>  
        /// 获取字符中指定标签的值  
        /// </summary>  
        /// <param name="str">字符串</param>  
        /// <param name="title">标签</param>  
        /// <returns>值</returns>  
        public string GetTitleContent(string str, string title)
        {
            string tmpStr = string.Format("<{0}[^>]*?>(?<Text>[^<]*)</{1}>", title, title); //获取<title>之间内容  

            Match TitleMatch = Regex.Match(str, tmpStr, RegexOptions.IgnoreCase);

            string result = TitleMatch.Groups["Text"].Value;
            return result;
        }

        /// <summary>
        /// 解析标题字符
        /// </summary>
        /// <param name="strTitle"></param>
        /// <returns></returns>
        public string GetTitleContent(string strTitle)
        {
            try
            {
                //\<title[^\>]*\>\s*(?<Title>.*?)\s*\</title\>
                string regEx = @"【.{11}】-京东";
                string extTitle = Regex.Match(strTitle, regEx).Value;
                if (string.IsNullOrEmpty(extTitle))
                {
                    extTitle = Regex.Match(strTitle, "京东JD.COM").Value;
                }

                string myTitle = strTitle.Replace("<title>", "").Replace("</title>", "");

                if (!string.IsNullOrEmpty(extTitle))
                {
                    myTitle = myTitle.Replace(extTitle, "");
                }
                return myTitle;
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                return strTitle;
            }
            
        }

        /// <summary>  
        /// 获取字符中指定标签的值  
        /// </summary>  
        /// <param name="str">字符串</param>  
        /// <param name="title">标签</param>  
        /// <param name="attrib">属性名</param>  
        /// <returns>属性</returns>  
        public string GetTitleContent(string str, string title, string attrib)
        {

            string tmpStr = string.Format("<{0}[^>]*?{1}=(['\"\"]?)(?<url>[^'\"\"\\s>]+)\\1[^>]*>", title, attrib); //获取<title>之间内容  

            Match TitleMatch = Regex.Match(str, tmpStr, RegexOptions.IgnoreCase);

            string result = TitleMatch.Groups["url"].Value;
            return result;
        }

        public int SaveImageFromWeb(string imgUrl, string path, string fileName)
        {
            if (path.Equals(""))
                throw new Exception("未指定保存文件的路径");

            string imgName = imgUrl.ToString().Substring(imgUrl.ToString().LastIndexOf("/") + 1);
            string defaultType = ".jpg";
            string[] imgTypes = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            string imgType = imgUrl.ToString().Substring(imgUrl.ToString().LastIndexOf("."));
            foreach (string it in imgTypes)
            {
                if (imgType.ToLower().Equals(it))
                    break;
                if (it.Equals(".bmp"))
                    imgType = defaultType;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imgUrl);
            request.UserAgent = "Mozilla/6.0 (MSIE 6.0; Windows NT 5.1; Natas.Robot)";
            request.Timeout = 3000;

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();

            if (response.ContentType.ToLower().StartsWith("image/"))
            {
                byte[] arrayByte = new byte[1024];
                int imgLong = (int)response.ContentLength;
                int l = 0;

                if (fileName == "")
                    fileName = imgName;

                FileStream fso = new FileStream(path + fileName + imgType, FileMode.Create);
                while (l < imgLong)
                {
                    int i = stream.Read(arrayByte, 0, 1024);
                    fso.Write(arrayByte, 0, i);
                    l += i;
                }

                fso.Close();
                stream.Close();
                response.Close();

                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 清空文本中HTML标签
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string ClearHtml(string text)//过滤html,js,css代码
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return "";
            text = Regex.Replace(text, "<head[^>]*>(?:.|[\r\n])*?</head>", "");
            text = Regex.Replace(text, "<script[^>]*>(?:.|[\r\n])*?</script>", "");
            text = Regex.Replace(text, "<style[^>]*>(?:.|[\r\n])*?</style>", "");

            text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", ""); //<br> 
            text = Regex.Replace(text, "\\&[a-zA-Z]{1,10};", "");
            text = Regex.Replace(text, "<[^>]*>", "");

            text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", ""); //&nbsp;
            text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty); //其它任何标记
            text = Regex.Replace(text, "[\\s]{2,}", " "); //两个或多个空格替换为一个

            text = text.Replace("'", "''");
            text = text.Replace("\r\n", "");
            text = text.Replace("  ", "");
            text = text.Replace("\t", "");
            return text.Trim();
        }

        public List<WebSiteModel> GetSearchList(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return null;
            }
            List<WebSiteModel> sites = new List<WebSiteModel>();
            string url = string.Format("http://search.jd.com/Search?keyword={0}&enc=utf-8&suggest=1.his.0&wq=&pvid=q1r6s1ni.7uj14o", keyword);
            string rtn = HttpHelper.GetResponseGBK(url, "get", string.Empty);
            //http://search.jd.com/Search?keyword=%E5%B0%8F%E9%BA%A6%E7%8E%8B%E5%95%A4%E9%85%92&enc=utf-8&suggest=1.his.0&wq=&pvid=q1r6s1ni.7uj14o
            //http://search.jd.com/Search?keyword=%E5%8D%8E%E4%B8%BA&enc=utf-8&wq=%E5%8D%8E%E4%B8%BA&pvid=8r7cs1ni.kiv6upc5#
            //keyword=%E5%8D%8E%E4%B8%BA&enc=utf-8&qrst=1&rt=1&stop=1&vt=2&bs=1&ev=exbrand_%E5%8D%8E%E4%B8%BA%EF%BC%88HUAWEI%EF%BC%89%40&page=11&s=301&click=0
            //http://search.jd.com/Search?keyword=%E5%8D%8E%E4%B8%BA&enc=utf-8&wq=%E5%8D%8E%E4%B8%BA&pvid=8r7cs1ni.kiv6upc5#
            //keyword=%E5%8D%8E%E4%B8%BA&enc=utf-8&qrst=1&rt=1&stop=1&vt=2&bs=1&ev=exbrand_%E5%8D%8E%E4%B8%BA%EF%BC%88HUAWEI%EF%BC%89%40&page=13&s=361&click=0
            /*<div id="J_topPage" class="f-pager">
						<span class="fp-text">
							<b>1</b><em>/</em><i>178</i>
						</span>
						<a class="fp-prev disabled" href="javascript:;">&lt;</a>
						<a class="fp-next" href="javascript:;">&gt;</a>
					</div>
					<div class="f-result-sum">共<span id="J_resCount" class="num">5311</span>件商品</div>
             * <em><font class="skcolor_ljg">哈尔滨</font>（Harbin） <font class="skcolor_ljg">小麦王</font>啤酒330ml*4*6听  整箱装</em>
             * //item.jd.com/2600210.html
             * <a target="_blank" title="哈尔滨（Harbin） 小麦王啤酒330ml*4*6听  整箱装" href="//item.jd.com/1154022.html" onclick="searchlog(1,1154022,1,1,'','flagsClk=648')">
							<em><font class="skcolor_ljg">哈尔滨</font>（Harbin） <font class="skcolor_ljg">小麦王</font>啤酒330ml*4*6听  整箱装</em>
             */
            if (!string.IsNullOrEmpty(rtn))
            {
                string pageNum = Regex.Match(rtn, @"<i>\d{1,3}</i>").Value;
                if (!string.IsNullOrEmpty(pageNum))
                {
                    //获取商品数量及页数
                    string wareNum = Regex.Match(rtn, "<div class=\"f-result-sum\">共<span id=\"J_resCount\" class=\"num\">\\d{1,4}</span>件商品</div>").Value;
                    //商品数量
                    int wNum = 0;
                    if (!string.IsNullOrEmpty(wareNum))
                    {
                        wNum = int.Parse(Regex.Match(wareNum, "\\d{1,4}").Value);
                    }
                    //总页数
                    int pNum = int.Parse(pageNum.Replace("<i>", "").Replace("</i>", ""));

                    MatchCollection wNameList = Regex.Matches(rtn, "<a target=\"_blank\" title=\"(.*)\"\\s*href=\"//item.jd.com/\\d{1,14}.html\"\\s*onclick=\"(.*)\">\\s*<em>");
                    for (int i = 0; i < wNameList.Count; i++)
                    {
                        string tt = wNameList[i].Value;
                        string wUrl = Regex.Match(tt, "//item.jd.com/\\d{1,14}.html").Value;
                        string wName = Regex.Match(tt, "title=\"(.*)\" ").Value.Replace("title=","");
                        if (!sites.Exists(t => t.url == wUrl))
                        {
                            sites.Add(new WebSiteModel()
                            {
                                title = wName,
                                url = wUrl
                            });
                        }
                    }
                }
            }

            return sites;
        }

    }
}
