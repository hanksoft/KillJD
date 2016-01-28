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

namespace WareDealer
{
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

        public ProductInfo GetWareInfo(string url)
        {
            //校验URL是否符合规范
            if (Regex.Match(url, "http://item.jd.com/(\\d{1,14}).html",RegexOptions.IgnoreCase).Success)
            {
                try
                {
                    string tID = Regex.Match(url, "\\d{1,14}").Value;
                    string html = HttpHelper.GetResponse(url, "get", string.Empty);
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
                    _myProduct = new ProductInfo();
                    _myProduct.RID = Guid.NewGuid().ToString();
                    _myProduct.ProductID = tID;
                    _myProduct.ProductURL = url;
                    _myProduct.NativeData = html;
                    //\<title[^\>]*\>\s*(?<Title>.*?)\s*\</title\>
                    string regEx = @"【.{11}】-京东";
                    string extTitle = Regex.Match(sRegexReturn, regEx).Value;
                    //_myProduct.ProductName = GetTitleContent(sRegexReturn, "title").Replace(extTitle, "");
                    _myProduct.ProductName = GetTitleContent(sRegexReturn).Replace(extTitle, "");

                    //获取商品归属类型 自营、旗舰店、第三方
                    //根据商品编号取值不同来判断商品归属
                    //自营 venderId:0 shopId:'0'
                    //旗舰店 venderId:1000001624 shopId:'1000001624'
                    //第三方 venderId:153108 shopId:'148186'
                    //cat: [9855,9857,9908]
                    //skuid: 202459,
                    string skuId = Regex.Match(html, "skuid:(\\d{1,14})").Value.Replace("skuid:", "");
                    string venderId = Regex.Match(html, "venderId:(\\d{1,14})").Value.Replace("venderId:", "");
                    string shopId = Regex.Match(html, "shopId:'(\\d{1,14})'").Value.Replace("shopId:'", "").TrimEnd('\'');
                    string cat = Regex.Match(html, @"cat: \[(\d{1,5}),(\d{1,5}),(\d{1,5})\]").Value.Replace("cat: [", "").TrimEnd(']');
                    if (!string.IsNullOrEmpty(venderId))
                    {
                        _myProduct.ProductAttach = venderId != shopId ? "第三方" : venderId == "0" ? "自营" : "旗舰店";
                        _myProduct.SkuId = skuId;
                        _myProduct.ShopId = shopId;
                        _myProduct.VenderId = venderId;
                        _myProduct.CatArea = cat;
                    }
                    //商品配送区域
                    GetWareArea();

                    //获取图片
                    sRegexReturn = GetImg(html);
                    if (!string.IsNullOrEmpty(sRegexReturn))
                    {
                        _myProduct.ProductImagePath = sRegexReturn;
                        _myProduct.ProductImageWebPath = sRegexReturn;
                        _myProduct.ProductImage = Get_img(sRegexReturn);
                    }
                    //string html2 = ClearHtml(html);
                    //获取评价信息
                    GetWareEvaluate(tID);
                    _myProduct.ProductPriceTrend = "持平";
                    _myProduct.ProductPriceType = "京东";
                    //获取商品库存情况
                    string stock = Regex.Match(html, @"<h3><strong>该商品已下柜，非常抱歉！</strong></h3>").Value;
                    if (string.IsNullOrEmpty(stock))
                    {
                        GetWareService(tID, venderId, cat); 
                    }
                    else
                    {
                        _myProduct.ProductBrand = "";
                        _myProduct.ProductIsSaled = -1;
                        _myProduct.ProductStock = "下柜";
                    }
                    
                    //商品品牌
                    //_myProduct.ProductBrand = GetWareService(tID, venderId, cat);
                    ////配送方式
                    //_myProduct.ProductDispatchMode = string.Format("由{0}负责发货及售后服务", _myProduct.ProductBrand);

                    //商品价格
                    _myProduct.ProductPrice = GetWarePriceByID(tID);
                    //手机专享价
                    //_myProduct.ProductMobilePrice = GetWareMobilePriceByID(tID);
                    
                    _myProduct.CreateTime = DateTime.Now;
                    _myProduct.CreateUser = "killjd";
                    _myProduct.BEnable = true;

                    return _myProduct;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// 重载图片
        /// </summary>
        /// <param name="webPath"></param>
        /// <param name="pid"></param>
        public void ReloadImg(ProductInfo myInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(myInfo.ProductImageWebPath))
                {
                    string url = string.Format("http://item.jd.com/{0}.html", myInfo.ProductID);

                    string tID = Regex.Match(url, "\\d{1,14}").Value;
                    string html = HttpHelper.GetResponse(url, "get", string.Empty);
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
                    
                    //获取图片
                    sRegexReturn = GetImg(html);
                    if (!string.IsNullOrEmpty(sRegexReturn))
                    {
                        _myProduct = myInfo;
                        myInfo.ProductImagePath = sRegexReturn;
                        myInfo.ProductImageWebPath = sRegexReturn;
                    }
                }
                myInfo.ProductImage = Get_img(myInfo.ProductImageWebPath);
                
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
                string html_evaluate = HttpHelper.GetResponse(url_evaluate, "get", string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 获取评价信息
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
                string html_evaluate = HttpHelper.GetResponse(url_evaluate, "get", string.Empty);

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
        /// 获取服务信息
        /// </summary>
        /// <param name="tID"></param>
        /// <param name="venderId"></param>
        /// <param name="cat"></param>
        /// <returns></returns>
        private int GetWareService(string tID, string venderId, string cat)
        {
            try
            {
                //配送服务
                //http://c0.3.cn/stock?skuId=1700908129&venderId=32533&cat=6144,12041,12047&area=1_72_2799_0&buyNum=1&extraParam={%22originid%22:%221%22}&ch=1&callback=getStockCallback
                string url_service = "http://c0.3.cn/stock?skuId=" + tID + "&venderId=" + venderId.Trim() + "&cat=" + cat + "&area=22_1930_49322_0&buyNum=1&extraParam={%22originid%22:%221%22}&ch=1&callback=getStockCallback";

                string html_service = HttpHelper.GetResponse(url_service, "get", string.Empty);
                string str_stock = html_service.Replace("getStockCallback(", "").TrimEnd(')');
                StockInfo jdStock = JsonConvert.DeserializeObject<StockInfo>(str_stock);

                if (_myProduct == null)
                {
                    _myProduct = new ProductInfo();
                }
                _myProduct.ProductBrand = string.IsNullOrEmpty(jdStock.Stock.self_D.deliver) ? jdStock.Stock.D.deliver : jdStock.Stock.self_D.deliver;
                //-1 下柜 0 无货 1 有货 2 配货 3 预订
                _myProduct.ProductIsSaled = jdStock.Stock.StockState == 33 ? 1 : (jdStock.Stock.StockState == 40 ? 2 : (jdStock.Stock.StockState == 36 ? 3 : 0)); //33 有货(1), 40 可配货(2), 36 预订(3), 无货(0)
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
                //-1代表已下柜
                _myProduct.ProductIsSaled = -1;
                return -1;
            }
            
        }

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
                //double myPrice = 0;
                ////商品价格 http://p.3.cn/prices/get?skuid=J_1108248&type=1&area=22_1930_49322&callback=cnp
                ////cnp([{"id":"J_1108248","p":"49.00","m":"80.00"}]);
                //string url_price = string.Format("http://p.3.cn/prices/get?skuid=J_{0}&type=1&area=1_72_4137&callback=cnp", pid);
                //string html_price = HttpHelper.GetResponse(url_price, "get", string.Empty);
                //string str_price = html_price.Replace("cnp([", "").Replace("]);", "");

                //JdWarePrice price = JsonConvert.DeserializeObject<JdWarePrice>(str_price);
                //myPrice = double.Parse(price.p);
                ////规则：如果价格小于0，该商品下柜
                //if (myPrice < 0)
                //{
                //    myPrice = 0;
                //    if (_myProduct != null)
                //    {
                //        _myProduct.ProductIsSaled = -1;
                //        _myProduct.ProductStock = "下柜";
                //    }
                //}
                //return myPrice;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
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
                if (_myProduct == null)
                {
                    _myProduct = new ProductInfo();
                }
                double myPrice = 0;
                //商品价格 http://pm.3.cn/prices/pcpmgets?callback=jQuery3820568&skuids=202459&origin=2&source=1&area=1_2800_4134_0&_=1451370905656
                //jQuery5068505([{"id":"202459","pcp":"69.00","p":"59.00","m":"121.00"}]);

                string url_price = string.Format("http://pm.3.cn/prices/pcpmgets?callback=jQuery3820568&skuids={0}&origin=2&source=1&area=1_2800_4134_0&_=1451370905656", pid);
                string html_price = HttpHelper.GetResponse(url_price, "get", string.Empty);
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
                if (!string.IsNullOrEmpty(price.pcp))
                {
                    _myProduct.ProductPriceType = "手机";
                }
                _myProduct.ProductPrice = myPrice;

                return myPrice;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
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

        /// <summary>
        /// 更新商品信息
        /// </summary>
        /// <param name="ware">商品对象</param>
        public void UpdateWareinfo(ProductInfo ware)
        {
            if (ware != null)
            {
                _myProduct = ware;
                //ware = GetWareInfoByID(ware.ProductID);
                //如果厂商编号为空则不处理
                if (string.IsNullOrEmpty(_myProduct.VenderId))
                {
                    return;
                }

                //获取库存情况
                int sale = GetWareService(_myProduct.ProductID, _myProduct.VenderId, _myProduct.CatArea);
                //获取商品价格
                double rtnPrice = GetWarePriceByID(_myProduct.ProductID);
                if (rtnPrice > 0)
                {
                    //DBHelper.GetInstance().WareShopUpdate(_myProduct.ProductID, _myProduct.VenderId, _myProduct.CatArea);
                    DBHelper.GetInstance().WarePriceInsert(_myProduct.ProductID, rtnPrice, DateTime.Now, _myProduct.ProductPriceType);
                    DBHelper.GetInstance().WareRepositoryUpdate(_myProduct.ProductID, rtnPrice, _myProduct.ProductIsSaled);
                    DBHelper.GetInstance().UpdateHistoryPriceBasebyID(_myProduct.ProductID);
                    //DBHelper.GetInstance().WarePriceUpdate(pid, rtnPrice);
                }
            }
        }

        public string GetImg(string str)
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
            string imgUrl = Regex.Match(str, "<img data-img=\"1\" width=\"350\" height=\"350\" src=\"\\S+\"", RegexOptions.IgnoreCase).Value;
            // 搜索匹配的字符串 
            //MatchCollection matches = regex.Matches(str);
            //int i = 0;
            //string[] sUrlList = new string[matches.Count];
            //// 取得匹配项列表 
            //foreach (Match match in matches)
            //{
            //    sUrlList[i++] = match.Groups["imgUrl"].Value;
            //}

            return imgUrl.Replace("<img data-img=\"1\" width=\"350\" height=\"350\" src=\"","").TrimEnd('\"');
        }

        private string _imgPath = "";
        public Bitmap Get_img(string imgSrc)
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
                img.Save(Path.Combine(Environment.CurrentDirectory,_myProduct.ProductImagePath));//随机名

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

        public string GetTitleContent(string str)
        {
            string result = str.Replace("<title>", "").Replace("</title>", "");
            return result;
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

        public static int SaveImageFromWeb(string imgUrl, string path, string fileName)
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

        public string ClearHtml(string text)//过滤html,js,css代码
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return string.Empty;
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
    }
}
