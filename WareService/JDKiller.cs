using Hank.BrowserParse;
using Hank.ComLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WareDealer.Helper;
using WareDealer.Mode;

namespace WareDealer
{
    /// <summary>
    /// 京东商城助手
    /// </summary>
    /// <remarks>欢迎加入QQ群415014949一起讨论</remarks>
    public class JDKiller
    {
        #region Field
        private JDLoginer _jdLoginer;
        //private List<WebSiteModel> _websites = new List<WebSiteModel>();
        private bool _isAuthcode = false;
        /// <summary>
        /// 是否需要验证码
        /// </summary>
        public bool IsAuthcode
        {
            get { return _isAuthcode; }
        }

        private Image _imageAuthCode;
        /// <summary>
        /// 验证码
        /// </summary>
        public Image ImageAuthCode
        {
            get { return _imageAuthCode; }
        }
        #endregion Field

        #region Class Interface
        /// <summary>
        /// 初始化进度条
        /// </summary>
        public Action<int> InitProcess;
        /// <summary>
        /// 操作进度条
        /// </summary>
        public Action<int> ShowStep;
        /// <summary>
        /// 显示消息
        /// </summary>
        public Action<string> ShowMessage;
        /// <summary>
        /// 获取关注完成
        /// </summary>
        public Action<bool> EndProcess;

        private static JDKiller _killer;
        public static JDKiller GetInstance()
        {
            return _killer ?? (_killer = new JDKiller());
        }
        /// <summary>
        /// 初始化登录接口
        /// </summary>
        /// <param name="uName"></param>
        /// <param name="uPass"></param>
        public void InitLogin(string uName, string uPass)
        {
            try
            {
                if (_jdLoginer == null)
                {
                    _jdLoginer = new JDLoginer()
                    {
                        r = new Random().NextDouble().ToString(),
                        //"6dc04d7e7f9b42aca76c1e2cce37059e15601070"
                        //"c7b303dc911d40729338dd10e4fd2238757832600"
                        eid = "6dc04d7e7f9b42aca76c1e2cce37059e15601070",
                        //WBIVASC7H6QRBITLOKI63TPLQXFMGD7T4A7P3PZLD6VAYCKL4GVQYJ5OHJCYRXIKSWQUYTCDQ4VUA
                        //"211f6cb42bee7b250c6a80b5c75b85ca"
                        //"b8b0d4ab226290696f575c771a2c72bc"
                        fp = "211f6cb42bee7b250c6a80b5c75b85ca", 
                        loginname = uName,
                        loginpwd = uPass,
                        cookies = "",
                        chkRememberMe = ""
                    };
                }
                _loginParams.Clear();
                GetLoginUI();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
            }
        }
        /// <summary>
        /// 重取验证码图片
        /// </summary>
        public void ReGetAuthCode()
        {
            GetCodeImg();
        }
        /// <summary>
        /// 登录京东
        /// </summary>
        /// <param name="myAuthCode"></param>
        /// <returns></returns>
        public bool Login4JD(string myAuthCode)
        {
            try
            {
                return Login(myAuthCode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 获取关注数据
        /// </summary>
        /// <returns></returns>
        public List<WebSiteModel> GetWatchList()
        {
            try
            {
                List<WebSiteModel> rtnSites = GetWactchs();
                Logout();
                return rtnSites;
            }
            catch (Exception ex)
            {
                ShowGetMessage(ex.Message);
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        #endregion Class Interface

        /// <summary>
        /// 获取京东页面数据
        /// </summary>
        /// <param name="website"></param>
        /// <returns></returns>
        private string GetJDWebHtml(string website)
        {
            try
            {
                if (string.IsNullOrEmpty(website))
                {
                    return null;
                }
                HttpItem item = new HttpItem();
                SFHttpHelper helper = new SFHttpHelper();
                HttpResult result = new HttpResult();
                item.URL = website;
                item.Encoding = Encoding.UTF8;
                item.Header.Add("Accept-Encoding", "gzip, deflate");
                item.ContentType = "text/html";
                item.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                item.Cookie = _jdLoginer.cookies;
                result = helper.GetHtml(item);
                if (result.Cookie != null)
                {
                    _jdLoginer.cookies = result.Cookie;
                }
                
                return result.Html;
            }
            catch (Exception ex)
            {
                ImportThreads.LastMsg = "失败：未获取到网页数据";
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 是否要验证码
        /// </summary>
        private bool CheckAuthcode()
        {
            //判断是否需要验证 返回Json({"verifycode":false})
            //https://passport.jd.com/uc/showAuthCode?r=0.7007493122946471&version=2015
            //https://authcode.jd.com/verify/image?a=1&acid=1c55bd67-241f-4b29-a56b-5c5bc6c717f7&uid=1c55bd67-241f-4b29-a56b-5c5bc6c717f7&yys=1454509280755
            HttpItem item = new HttpItem();
            SFHttpHelper helper = new SFHttpHelper();
            HttpResult result = new HttpResult();
            string r = new Random().NextDouble().ToString();
            item.URL = string.Format("https://passport.jd.com/uc/showAuthCode?r={0}&version=2015", r);
            item.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            item.Postdata = string.Format("loginName={0}",_jdLoginer.loginname);
            item.PostDataType = PostDataType.String;
            item.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";
            item.Cookie = _jdLoginer.cookies;
            result = helper.GetHtml(item);
            if (result.Html.ToLower().Contains("false"))
            {
                _isAuthcode = false;
                return false;
            }
            else
            {
                _isAuthcode = true;
                return true;
            }
        }
        /// <summary>
        /// 退出京东
        /// </summary>
        private void Logout()
        {
            //退出登录   https://passport.jd.com/uc/login?ltype=logout
            HttpItem item = new HttpItem();
            SFHttpHelper helper = new SFHttpHelper();
            HttpResult result = new HttpResult();
            item.URL = "https://passport.jd.com/uc/login?ltype=logout";
            item.Cookie = _jdLoginer.cookies;
            result = helper.GetHtml(item);
            if (result != null)
            {

            }
            _jdLoginer = null;
            ShowGetMessage("退出京东登录");
        }

        Dictionary<string, string> _loginParams = new Dictionary<string, string>();

        private void ParseLoginParams()
        {
            if (_loginParams.Count > 0)
            {
                if (_loginParams.ContainsKey("uuid"))
                {
                    _jdLoginer.uuid = _loginParams["uuid"].ToString();
                }
                if (_loginParams.ContainsKey("_t"))
                {
                    _jdLoginer.uuid = _loginParams["_t"].ToString();
                }
                if (_loginParams.ContainsKey("uuid"))
                {
                    _jdLoginer.uuid = _loginParams["uuid"].ToString();
                }
            }
        }
        /// <summary>
        /// 获取登录界面数据
        /// </summary>
        private void GetLoginUI()
        {
            string loginHtml = GetJDWebHtml("https://passport.jd.com/new/login.aspx");

            //<input[^(>[\\s\\S]*?<)]*?hidden[^(>[\\s\\S]*?<)]*?/>
            MatchCollection matchs = Regex.Matches(loginHtml, @"<input.+?(?=name=""(?<name>\S+)"").+?(?=value=""(?<value>\S+)"").+?>");
            if (matchs.Count > 0)
            {
                foreach (Match mitem in matchs)
                {
                    _loginParams.Add(mitem.Groups["name"].ToString(), mitem.Groups["value"].ToString());

                    string name = mitem.Groups["name"].ToString();
                    string value = mitem.Groups["value"].ToString();
                    if (name == "uuid")
                    {
                        _jdLoginer.uuid = value;
                    }
                    else if (name == "_t")
                    {
                        _jdLoginer._t = value;
                    }
                    else
                    {
                        _jdLoginer.tname = name;
                        _jdLoginer.tvalue = value;
                    }
                }
                //判断是否需要验证码 TO-DO
                if (CheckAuthcode())
                {
                    _isAuthcode = true;
                    //获取验证码图片
                    GetCodeImg();
                }
            }
            else
            {
                ImportThreads.LastMsg = "获取网页参数错误";
            }
        }
        /**********************************************************************************************************
            //京东模拟登录步骤
            //第一步：获取登录页面基础输入值；
            //第二步：判断是否需要验证码，如需要则取验证码图片到本地并由用户输入；
            //第三步：拼装所有提交信息；
            //第四步：提交数据到指定服务；
            //第五步：获取返回信息，通过正则表达式来取需要的数据。
            //https://passport.jd.com/new/login.aspx
            //post https://passport.jd.com/uc/loginService?uuid=b9155d01-fbe3-4a61-b15f-0fd99ea101a8&&r=0.8045588354580104&version=2015 
            //如uuid、r、version
            //uuid ; r 随机数种子 会过期，过期后提示“authcode为空”;version 京东登录脚本的版本号
            //验证码图片获取地址
            //https://authcode.jd.com/verify/image?a=1&amp;acid=dd73def5-a635-4692-af7d-464491d99579&amp;uid=dd73def5-a635-4692-af7d-464491d99579
            //<div id="o-authcode" class="item item-vcode item-fore4  hide ">
            //https://passport.jd.com/uc/showAuthCode?r=0.7007493122946471&version=2015
        **********************************************************************************************************/

        /// <summary>
        /// 登录京东商城
        /// </summary>
        private bool Login(string authcode)
        {
            HttpItem item = new HttpItem();
            SFHttpHelper helper = new SFHttpHelper();
            HttpResult result = new HttpResult();

            //如果需要验证码就需要jda，jdb，jdc，jdv这些，如果没有出验证码，可以直接post登录成功的
            //string cookies = "__jda=95931165.290243407.1371634814.1371634814.1371634814.1; __jdb=95931165.1.290243407|1.1371634814; __jdc=95931165; __jdv=95931165|direct|-|none|-;" + _jdLoginer.cookies;
            string cookies = "__jdu=1394616361; __jda=122270672.1394616361.1461636833.1461636833.1461636833.1; "
                +"unpl=V2_ZzNtbRBWQxYiDhMAckpaBGJRE1tLB0oSIV8UB3tNWAZjChpeclRCFXIUR1FnGlsUZwIZXUZcQBRFCHZXchBYAGEHG1hyV0YdPHhGVXoYXQRmABRdcmdAFEUAdlR5EVkCZwQQWkJncxJFOJLoxM7du7KOg4nZ9HMXdABEXH0RXANXAiJcch"
                +"wtFDgIRFx%2bHlwCZQQSbUM%3d; mt_subsite=||1111%2C1461636920; __jdb=122270672.5.1394616361|1.1461636833; __jdc=122270672; __jdv=122270672|click.union.timesdata.net"
                +"|t_288547584_149xA1000000271|tuiguang|c012f7de8b704c078a86efcb1e525892; _tp=gvgIRVyymbK6lFmmkN3g4qGeJnHoph%2BdcBXbCOaBySY%3D; unick=%E4%B8%96%E7%BA%AA%E9%AB%98%E6%A1%A5;"
                +" _pst=jd_7bb8087728c44; TrackID=1F5uT6zOHbmBW01TKlUyQ7GbEINQ09CbCTnXNkT2aqA8pwDg2ZR_B_Z5jWfneDJfNRQPoMXHH9sGbM7iJlmjiDX9BnAqcsEDlmDkvSQOQIH0; pinId=jc-OZwkDIvyjW1nEe9zdD7V9-x-f3wj7;"
                +" _jrda=1; _jrdb=1461637460015; 3AB9D23F7A4B3C9B=78d97a3d1e9e4ae2832d21dbe81d6fd7394012304; alc=/Y1Y1CquLQZPWbDaIhh/8w==; _ntvsWYk=rsy+/seWbN6G+IYtHAWOUz3B8xVwiF4oDNcJUecODec=;"
                +" mp=13908052076; _ntEhbrP=KPnWKr0GkGLYESVBeRIc7ucPudXAa+ADs6Z9GI6c5ug=; _ntevPgB=BAeWuzhoS+8o0U6OEKkSQH0hOa/4bp32f3dvImaQbzU=; _ntRblQD=vEujL7I++TMt+jeraYPTW/s56Fm8gBpoC9gN2FYO0XA="
                + _jdLoginer.cookies;
            cookies = cookies.Replace("HttpOnly,", null);
            _jdLoginer.cookies = cookies;

            //https://passport.jd.com/uc/loginService?uuid=c4ee6786-a737-43f2-9110-0aea68500665&ReturnUrl=http%3A%2F%2Fwww.jd.com%2F&r=0.12566684937051964&version=2015
            //https://passport.jd.com/uc/loginService?uuid=c8422a2d-e011-4783-8bca-38625607a086&ltype=logout&r=0.04991701628602441&version=2015
            item.URL = string.Format("https://passport.jd.com/uc/loginService?uuid={0}&ltype=logout&r={1}&version=2015", _jdLoginer.uuid, _jdLoginer.r);
            item.Method = "post";
            item.Allowautoredirect = true;
            item.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            item.Postdata = string.Format("uuid={0}&machineNet=&machineCpu=&machineDisk=&eid={1}&fp={2}&_t={3}&{4}={5}"
                + "&loginname={6}&nloginpwd={7}&loginpwd={7}&chkRememberMe=on&authcode={8}",
                _jdLoginer.uuid, _jdLoginer.eid, _jdLoginer.fp, _jdLoginer._t, _jdLoginer.tname, _jdLoginer.tvalue,
                _jdLoginer.loginname, _jdLoginer.loginpwd, authcode);
            item.Header.Add("x-requested-with", "XMLHttpRequest");
            item.Header.Add("Accept-Encoding", "gzip, deflate");
            //item.Referer = "http://passport.jd.com/new/login.aspx?ReturnUrl=http%3a%2f%2fjd2008.jd.com%2fJdHome%2fOrderList.aspx";
            item.Accept = "*/*";
            item.Encoding = Encoding.UTF8;
            item.Cookie = cookies;
            result = helper.GetHtml(item);
            _jdLoginer.cookies = result.Cookie;

            if (!result.Html.Contains("success"))
            {
                string rtnMsg = result.Html.Remove(0, 1).TrimEnd(')');
                LoginMsg jdMsg = JsonConvert.DeserializeObject<LoginMsg>(rtnMsg);
                //用户名错误({"username":"\u8d26\u6237\u540d\u4e0d\u5b58\u5728\uff0c\u8bf7\u91cd\u65b0\u8f93\u5165"})
                if (result.Html.ToLower().Contains("username"))
                {
                    ShowGetMessage("失败：用户名错误!" + jdMsg.value);
                    ImportThreads.LastMsg = "失败：用户名错误!" + jdMsg.value;
                }
                else if (result.Html.ToLower().Contains("pwd"))
                {
                    ShowGetMessage("失败：密码验证不通过!" + jdMsg.value);
                    //密码错误 ({"pwd":"\u8d26\u6237\u540d\u4e0e\u5bc6\u7801\u4e0d\u5339\u914d\uff0c\u8bf7\u91cd\u65b0\u8f93\u5165"})
                    ImportThreads.LastMsg = "失败：密码验证不通过!" + jdMsg.value;
                }
                else if (result.Html.ToLower().Contains("emptyauthcode"))
                {
                    ShowGetMessage("失败：请输入登录验证码!" + jdMsg.value);
                    //验证码错误 ({"emptyAuthcode":"\u8bf7\u8f93\u5165\u9a8c\u8bc1\u7801"})
                    //({"_t":"_ntcKIiJ","emptyAuthcode":"\u9a8c\u8bc1\u7801\u4e0d\u6b63\u786e\u6216\u9a8c\u8bc1\u7801\u5df2\u8fc7\u671f"})
                    ImportThreads.LastMsg = "失败：请输入登录验证码!" + jdMsg.value;
                }
                else
                {
                    ImportThreads.LastMsg = jdMsg.value;
                }
                ImportThreads.WareEnd = true;
                return false;
            }
            ShowGetMessage("登录成功！");
            ImportThreads.LastMsg = "登录成功！";
            return true;
            
        }

        public class LoginMsg
        {
            public string name {get;set;}
            public string value {get;set;}
        }

        /// <summary>
        /// 获取验证码图片
        /// </summary>
        private void GetCodeImg()
        {
            //<img id="JD_Verification1" class="verify-code" src2="https://authcode.jd.com/verify/image?a=1&amp;acid=6bb6fa1d-a929-404c-a0cc-1d10e13aa639&amp;uid=6bb6fa1d-a929-404c-a0cc-1d10e13aa639"
            //"https://authcode.jd.com/verify/image?a=1&acid=1c55bd67-241f-4b29-a56b-5c5bc6c717f7&uid=1c55bd67-241f-4b29-a56b-5c5bc6c717f7&yys=1454509280755";
            try
            {
                //Match rtnRgx = Regex.Match(loginHtml, @"src2=""(.*?)""", RegexOptions.Singleline);
                {
                    int sjs = new Random().Next();
                    string imgUrl = string.Format("https://authcode.jd.com/verify/image?a=1&acid={0}&uid={0}&yys={1}", _jdLoginer.uuid, sjs);

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imgUrl);
                    request.Timeout = 20000;
                    request.ServicePoint.ConnectionLimit = 100;
                    request.ReadWriteTimeout = 30000;
                    request.Method = "GET";
                    //request.CookieContainer
                    //CookieContainer co = new CookieContainer();
                    //co.SetCookies(new Uri("https://passport.jd.com/uc/loginService"), _jdLoginer.cookies);
                    //request.CookieContainer = co;

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK)
                        return;
                    Stream resStream = response.GetResponseStream();
                    //this.pictureBox1.Image = Image.FromStream(resStream);
                    _imageAuthCode = new Bitmap(resStream);
                }
            }
            catch (Exception ex)
            {
                _imageAuthCode = null;
                Debug.WriteLine(ex.Message);
            }

        }

        private void InitGetProcess(int iLen)
        {
            if (InitProcess != null)
            {
                InitProcess(iLen);
            }
        }

        /// <summary>
        /// 显示获取数据
        /// </summary>
        /// <param name="msg"></param>
        private void ShowGetMessage(string msg)
        {
            if (ShowMessage != null)
            {
                ShowMessage(msg);
            }
        }

        private void ShowGetStep(int iStep)
        {
            if (ShowStep != null)
            {
                ShowStep(iStep);
            }
        }

        private void EndGetProcess(bool bRtn)
        {
            if (EndProcess != null)
            {
                EndProcess(bRtn);
            }
        }

        /// <summary>
        /// 获取所有关注数据
        /// </summary>
        private List<WebSiteModel> GetWactchs()
        {
            try
            {
                ShowGetMessage("获取关注数据开始");
                //获取关注数据
                //关注首页 http://t.jd.com/home/follow
                //http://t.jd.com/home/follow?index=2
                ShowGetMessage("提取http://t.jd.com/home/follow页面数据开始");
                //ImportThreads.LastMsg = "获取http://t.jd.com/home/follow数据";
                string gzhtml = GetJDWebHtml("http://t.jd.com/home/follow");
                if (string.IsNullOrEmpty(gzhtml))
                {
                    return null;
                }
                ShowGetMessage("提取http://t.jd.com/home/follow页面数据完成");
                List<WebSiteModel> gzSites = new List<WebSiteModel>();
                //关注分页处理
                //获取商品数量并计算分页数 当前京东界面是20个商品为一页
                //全部商品<em>(336)</em>
                Match reg = Regex.Match(gzhtml, "全部商品<em>\\(\\d{1,4}\\)</em>");
                if (reg.Success)
                {
                    string warelength = reg.Value.Replace("全部商品<em>(", "").Replace(")</em>", "");
                    int wLength = 0;
                    int.TryParse(warelength, out wLength);
                    iStep = 0;
                    InitGetProcess(wLength);

                    ShowGetMessage("关注商品总长度"+wLength.ToString());
                    if (wLength > 0)
                    {
                        float iNum = (float)wLength / 20;
                        int pagenum = (int)(wLength / 20);
                        if (pagenum < iNum)
                        {
                            pagenum++;
                        }
                        //获取关注首页数据
                        //通过正则方式获取所有关注数据
                        ShowGetMessage(string.Format("获取首页数据，共计需处理{0}页关注商品", pagenum));
                        gzSites = GetWatchWareList(gzhtml);

                        if (pagenum > 1)
                        {
                            for (int i = 2; i <= pagenum; i++)
                            {
                                string sitestr = string.Format("http://t.jd.com/home/follow?index={0}", i);
                                string gzHtml1 = GetJDWebHtml(sitestr);
                                ShowGetMessage("开始获取" + sitestr + "数据");
                                gzSites.AddRange(GetWatchWareList(gzHtml1));
                            }
                        }
                        while (iStep < wLength)
                        {
                            ShowGetStep(iStep++);
                        }
                    }
                }
                ShowGetMessage("提取http://t.jd.com/home/follow页面数据完成");
                EndGetProcess(true);
                return gzSites;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                EndGetProcess(false);
                ShowGetMessage("失败：" + ex.Message);
                return null;
            }

        }

        private List<WebSiteModel> GetWareInfoByWatch(string watchhtml)
        {
            /*  
            * <div class="p-name">
            * <input type="checkbox" name="sku" class="jdcheckbox" value="1232734">
            * <a href="//item.jd.com/1232734.html" target="_blank" title="Jabees BSport 无线运动立体声音乐蓝牙耳机 通用型 入耳式 防水防汗 蓝色" 
            * alt="Jabees BSport 无线运动立体声音乐蓝牙耳机 通用型 入耳式 防水防汗 蓝色">Jabees BSport 无线运动立体声音乐蓝牙耳机 通用型 入耳式 防水防汗 蓝色
            * </a>
            * </div>
            * */
            //MatchCollection matchs = Regex.Matches(watchhtml, @"<div class='p-name'>.<div class='p-price'>");
            Match tt = Regex.Match(watchhtml, @"<div class=""p-name"">(.*?)</div>", RegexOptions.Singleline);
            if (tt.Success)
            {
                //ShowMessage(tt.Value);
            }
            return null;
        }
        private int iStep = 0;
        /// <summary>
        /// 获取关注列表数据
        /// </summary>
        /// <param name="watchhtml"></param>
        private List<WebSiteModel> GetWatchWareList(string watchhtml)
        {
            List<WebSiteModel> rtnSites = new List<WebSiteModel>();
            MatchCollection matchs = Regex.Matches(watchhtml, @"<div class=""p-name"">(.*?)</div>", RegexOptions.Singleline);
            foreach (Match item in matchs)
            {
                if (!item.Value.Contains("item.jd.com/{{sku}}.html"))
                {
                    ImportThreads.LastMsg = item.Value;
                    Match reg = Regex.Match(item.Value, "//item.jd.com/(\\d{1,14}).html", RegexOptions.Singleline);
                    if (reg.Success)
                    {
                        string site = "http:" + reg.Value;
                        rtnSites.Add(new WebSiteModel() { url = site });
                        ShowGetMessage(string.Format("添加{0}到商品列表",site));
                        ShowGetStep(iStep++);
                    }

                }
            }
            return rtnSites;
        }

        #region JDWareTypes
        /// <summary>
        /// 获取京东商品分类数据
        /// </summary>
        public void GetWareTypeData(string category, string myType)
        {
            try
            {
                if (string.IsNullOrEmpty(category))
                {
                    return;
                }
                //确定采集模式为批量采集
                SysParams.GatherModel = GatherType.Batch;

                ShowGetMessage("获取京东商品分类数据线程启动!!!");
                Stopwatch getWatch = new Stopwatch();
                getWatch.Start();
                string wareCat = category.Replace('_', ',');
                //http://list.jd.com/list.html?cat=9987,653,655
                string site = string.Format("http://list.jd.com/list.html?cat={0}&go=0", wareCat);
                ShowGetMessage(string.Format("获取商品分类{0}页面", site));
                string html = HttpHelper.GetResponseUTF(site, "get", string.Empty);
                if (string.IsNullOrEmpty(html))
                {
                    //return null;
                }
                //获取当前分类总商品页数
                //<span class="fp-text"><b>\d{1,2}</b><em>/</em><i>\d{1,2}</i></span>
                //<span class="fp-text"><b>1</b><em>/</em><i>46</i></span>
                string pageNoReg = @"<span class=""fp-text"">\s*<b>\d{1,2}</b><em>/</em><i>\d{1,2}</i>\s*</span>";
                string rtnPageNo = Regex.Match(html, pageNoReg,RegexOptions.Multiline).Value;
                if (string.IsNullOrEmpty(rtnPageNo))
                {
                    rtnPageNo = Regex.Match(html, @"<span class=""fp-text"">\s*(.*)</i>\s*</span>", RegexOptions.Multiline).Value;
                }
                int wLength = 0;

                if (!string.IsNullOrEmpty(rtnPageNo))
                {
                    string rr = Regex.Match(rtnPageNo, @"<i>\d{1,2}").Value.Replace("<i>", "");
                    int pNo = int.Parse(rr);
                    wLength = pNo * 60;
                    InitGetProcess(wLength);
                    iStep = 0;
                    ShowGetMessage(string.Format("京东商品当前分类共计{0}页", pNo));
                    ShowGetMessage("第 1 页商品获取开始!!!");
                    //处理第一页的数据
                    ParseWareTypeData(html, myType);

                    for (int i = 2; i < pNo; i++)
                    {
                        ShowGetMessage(string.Format("第 {0} 页商品获取开始...", i));
                        site = string.Format("http://list.jd.com/list.html?cat={0}&page={1}&go=0&JL=6_0_0", wareCat, i);
                        html = HttpHelper.GetResponseUTF(site, "get", string.Empty);
                        ParseWareTypeData(html, myType);
                        ShowGetMessage(string.Format("第 {0} 页商品获取完成", i));
                    }
                }
                while (iStep < wLength)
                {
                    ShowGetStep(iStep++);
                }

                getWatch.Stop();
                ShowGetMessage(string.Format("获取京东商品分类数据线程完成，共计耗时{0} ms", getWatch.ElapsedMilliseconds));
                EndGetProcess(true);
            }
            catch (Exception ex)
            {
                EndGetProcess(false);
                ShowGetMessage(string.Format("商品处理异常：{0}", ex.Message));
                OtCom.XLogErr(ex.Message);
            }
        }

        /// <summary>
        /// 获取分类下所有商品详细数据
        /// </summary>
        /// <param name="html"></param>
        public void ParseWareTypeData(string html, string myType)
        {
            try
            {
                //string re = "<(link|script)(.*?type)[^>]+?(/)?>(?(3)|\\s*</\\1>)"; 提取页面link和Script
                //string regStr = @"var pay_after = \[(\d{1,16}\,){58}(\d{1,16})\];";
                //var slaveWareList ={(.*)};
                //var pay_after = \[(.*)\];
                string regStr = @"var pay_after =\s*\[(\d{1,16}\,)+(\d{1,16})\];";
                string rtnData = Regex.Match(html, regStr).Value;
                ShowGetMessage("解析页面商品...");
                if (!string.IsNullOrEmpty(rtnData))
                {
                    string tt = rtnData.Replace("var pay_after = [", "").Replace("];", "");
                    string[] wareList = tt.Split(',');
                    if (wareList != null && wareList.Length > 0)
                    {
                        
                        ShowGetMessage(string.Format("当前页面共计{0}个商品", wareList.Length));
                        foreach (var item in wareList)
                        {
                            iStep++;
                            if (!DBHelper.GetInstance().WareIsExists(item))
                            {
                                ShowGetMessage(string.Format("获取商品[{0}]信息", item));
                                ProductInfo tmpWare = WareService.GetInstance().GetWareInfoByID(item);
                                if (tmpWare != null)
                                {
                                    tmpWare.ProductType = myType;
                                    ShowGetMessage(string.Format("商品[{0}]{1}信息入库...", item, tmpWare.ProductName));
                                    DBHelper.GetInstance().WareInsert(tmpWare);
                                    DBHelper.GetInstance().WarePriceInsert(tmpWare.ProductID, tmpWare.ProductPrice);
                                    ShowGetMessage(string.Format("商品[{0}]{1}信息入库完成。", item, tmpWare.ProductName));
                                }
                            }
                            ShowGetStep(iStep);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowGetMessage(string.Format("商品处理异常：{0}", ex.Message));
                OtCom.XLogErr(ex.Message);
            }
        }
        #endregion JDWareTypes 
    }
}
