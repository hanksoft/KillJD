using Hank.BrowserParse;
using Hank.ComLib;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using WareDealer.Helper;
using WareDealer.Mode;

namespace WareDealer
{
    /// <summary>
    /// 京东监听器
    /// </summary>
    /// <remarks>用于自动获取数据</remarks>
    public class JDListener
    {
        #region Class Interface
        /// <summary>
        /// 初始化线程
        /// </summary>
        /// <remarks>用于进度条准备工作</remarks>
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
        /// 线程完成
        /// </summary>
        public Action<bool> EndProcess;

        private static JDListener _listen;

        private JDListener() { }

        public static JDListener GetInstance()
        {
            return _listen ?? (_listen= new JDListener());
        }

        /// <summary>
        /// 更新商品信息
        /// </summary>
        public void UpdateWareThread()
        {
            Thread updateAllWare = new Thread(delegate() { UpdateAllWare(); }) { Name = "updateThread", IsBackground = true };
            updateAllWare.Start();
        }
        /// <summary>
        /// 更新商品信息
        /// </summary>
        /// <param name="wares"></param>
        public void UpdateWareThread(List<ProductInfo> wares)
        {
            Thread updateSomeWare = new Thread(delegate() { UpdateThread(wares); }) { Name = "updateSomeWare", IsBackground = true };
            updateSomeWare.Start();
        }

        /// <summary>
        /// 登录获取京东商品关注数据
        /// </summary>
        public void GetFocusThread()
        {
            Thread focusThread = new Thread(delegate() { GetFocusWare(); }) { Name = "focusThread", IsBackground = true };
            focusThread.Start();
        }
        /// <summary>
        /// 获取京东商品分类
        /// </summary>
        public void GetJDWareTypesThread()
        {
            Thread typesThread = new Thread(delegate() { GetJDTypesThread(); }) { Name = "typesThread", IsBackground = true };
            typesThread.Start();
            
        }

        #endregion Class Interface

        #region Release Private Method
        private void InitAutoProcess(int iLength)
        {
            if (InitProcess != null)
            {
                InitProcess(iLength);
            }
        }

        private void ShowAutoStep(int iStep)
        {
            if (ShowStep != null)
            {
                ShowStep(iStep);
            }
        }

        private void EndAutoProcess(bool bRtn)
        {
            if (EndProcess != null)
            {
                EndProcess(bRtn);
            }
        }

        private void ShowAutoMessage(string msg)
        {
            if (ShowMessage != null)
            {
                ShowMessage(msg);
            }
        }
        #endregion Release Private Method

        /// <summary>
        /// 更新数据库中所有商品信息
        /// </summary>
        private void UpdateAllWare()
        {
            OtCom.XLogInfo("开始自动获取所有商品价格及库存情况！");
            //获取数据库中所有商品
            List<ProductInfo> wares = DBHelper.GetInstance().WareGetAll();
            UpdateThread(wares);
        }

        /// <summary>
        /// 更新指定商品集合信息
        /// </summary>
        /// <param name="wares"></param>
        private void UpdateThread(List<ProductInfo> wares)
        {
            if (wares != null && wares.Count > 0)
            {
                UpdateBatchThread(wares);
                //InitAutoProcess(wares.Count);
                //int i = 0;
                //foreach (var item in wares)
                //{
                //    i++;
                //    WareDealer.WareService.GetInstance().UpdateWareinfo(item);
                //    ShowAutoStep(i);
                //}
                //EndAutoProcess(true);
            }
        }
        /// <summary>
        /// 批量更新价格线程
        /// </summary>
        /// <remarks>默认一个批次50个商品</remarks>
        private void UpdateBatchThread(List<ProductInfo> wares)
        {
            if (wares != null && wares.Count > 0)
            {
                InitAutoProcess(wares.Count);
                int i = 0;
                int j = 0;
                List<string> wareList = new List<string>();
                foreach (var item in wares)
                {
                    i++;
                    j++;
                    wareList.Add(item.ProductID);
                    if ((wares.Count < 80 && i == wares.Count) || j == 80)
                    {
                        WareDealer.WareService.GetInstance().GetBatchWarePrice(wareList);
                        wareList.Clear();
                        j = 0;
                    }
                    
                    ShowAutoStep(i);
                }
                EndAutoProcess(true);
            }
        }

        /// <summary>
        /// 自动获取关注数据
        /// </summary>
        private void GetFocusWare()
        {
            try
            {
                //确定采集模式为批量采集
                SysParams.GatherModel = GatherType.Batch;
                //1、登录京东
                JDKiller.GetInstance().InitProcess = InitProcess;
                JDKiller.GetInstance().EndProcess = EndProcess;
                JDKiller.GetInstance().ShowStep = ShowStep;
                JDKiller.GetInstance().ShowMessage = ShowMessage;
                ShowAutoMessage("开始登录");
                JDKiller.GetInstance().InitLogin(SysParams.JDUserName, SysParams.JDUserPass);
                if (JDKiller.GetInstance().Login4JD(""))
                {
                    ShowAutoMessage("获取关注商品列表");
                    //2、获取关注商品列表
                    List<WebSiteModel> weburls = JDKiller.GetInstance().GetWatchList();
                    if (weburls != null && weburls.Count > 0)
                    {
                        ShowAutoMessage("导入关注数据");
                        //3、导入关注数据
                        WareImport.GetInstance().ShowStep = null;
                        WareImport.GetInstance().EndProcess = null;
                        WareImport.GetInstance().ShowMessage = null;
                        WareImport.GetInstance().ImportWareList(weburls);
                        ShowAutoMessage("导入完成");
                    }
                }
                else
                {
                    ShowAutoMessage("登录失败！");
                }
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
            }
            
        }

        private void GetJDTypesThread()
        {
            ShowAutoMessage("开始获取京东商品分类...");
            GetWareTypes();
        }

        /// <summary>
        /// 获取京东商品分类信息
        /// </summary>
        public void GetWareTypes()
        {
            try
            {
                //确定采集模式为批量采集
                SysParams.GatherModel = GatherType.Batch;
                ShowAutoMessage("清空京东商品分类数据...");
                //清空京东商品分类数据
                DBHelper.GetInstance().WareJDTypeClear();
                //<a href="http://channel.jd.com/737-752.html" target="_blank">厨房小电<i>&gt;</i></a>
                //<a href="http://list.jd.com/list.html?cat=9987,653,655" target="_blank">手机</a>
                //<a href="http://list.jd.com/list.html?cat=737,794,798" target="_blank">平板电视</a>
                string url = "http://dc.3.cn/category/get";
                string rtnHtml = HttpHelper.GetResponseGBK(url, "get", string.Empty);
                if (!string.IsNullOrEmpty(rtnHtml))
                {
                    JDWareTypes jdTypes = JsonConvert.DeserializeObject<JDWareTypes>(rtnHtml);
                    
                    List<JDWareType> _types = new List<JDWareType>();

                    if (jdTypes.data != null && jdTypes.data.Count > 0)
                    {
                        ShowAutoMessage("解析京东商品分类数据...");
                        foreach (var item in jdTypes.data)
                        {
                            if (item.s != null && item.s.Count > 0)
                            //if (item.s != null && item.s.Count > 0 && item.id == "a")
                            {
                                List<JDWareType> rtnList = ParseSubType(item.s, 0);
                                if (rtnList != null && rtnList.Count > 0)
                                {
                                    _types.AddRange(rtnList);
                                }
                            }
                        }
                    }
                    if (_types.Count > 0)
                    {
                        ShowAutoMessage("京东商品分类数据入库...");
                        DBHelper.GetInstance().WareJDTypeAdd(_types);
                        ShowAutoMessage("京东商品分类数据入库完成");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAutoMessage(ex.Message);
                OtCom.XLogErr(ex.Message);
            }

        }
        /// <summary>
        /// 解析商品子分类
        /// </summary>
        /// <param name="sTypes"></param>
        /// <param name="iLevel"></param>
        /// <returns></returns>
        private List<JDWareType> ParseSubType(List<subType> sTypes, int iLevel)
        {
            try
            {
                ShowAutoMessage("递归循环获取分类...");
                List<JDWareType> rtnTypes = new List<JDWareType>();
                //记录递归循环次数
                int iCount = iLevel;
                if (sTypes != null && sTypes.Count > 0)
                {
                    foreach (var item in sTypes)
                    {
                        JDWareType wType = new JDWareType();
                        //"n" : "jiadian.jd.com|家用电器||0",
                        string[] typeNames = item.n.Split('|');
                        if (typeNames.Length > 0)
                        {
                            wType.TypeName = typeNames[1];
                            wType.TypeUrl = typeNames[0];
                            wType.TypeID = -1;
                            wType.TopID = -1;
                            wType.TopTID = 0;
                            wType.BEnable = false;
                            if (!typeNames[0].Contains("jd"))
                            {
                                //"n" : "737-794|大家电||0",
                                //"n" : "737-794-798|平板电视||0",
                                //"n" : "list.jd.com/list.html?cat=737,13297,1300&ev=%402047_584926&go=0&JL=3_%E4%BA%A7%E5%93%81%E7%B1%BB%E5%9E%8B_%E6%B2%B9%E7%83%9F%E6%9C%BA|油烟机||0",
                                string[] ids = typeNames[0].Split('-');
                                if (ids != null && ids.Length > 1)
                                {
                                    //分类编号最后一级存在两个或三个情况
                                    if (ids.Length == iLevel + 1)
                                    {
                                        wType.TypeID = int.Parse(ids[iLevel]);
                                        wType.TopID = int.Parse(ids[iLevel - 1]);

                                    }
                                    else
                                    {
                                        wType.TypeID = int.Parse(ids[iLevel - 1]);
                                        wType.TopID = int.Parse(ids[iLevel - 2]);
                                    }
                                    wType.TopTID = int.Parse(ids[0]);
                                }
                            }
                            else
                            {
                                string urlData = Regex.Match(typeNames[0], @"\d{1,5},\d{1,5},\d{1,5}").Value;
                                if (!string.IsNullOrEmpty(urlData))
                                {
                                    string[] ids = urlData.Split(',');
                                    if (ids != null && ids.Length > 0)
                                    {
                                        if (ids.Length == iLevel + 1)
                                        {
                                            wType.TypeID = int.Parse(ids[iLevel]);
                                            wType.TopID = int.Parse(ids[iLevel - 1]);
                                        }
                                        else
                                        {
                                            wType.TypeID = int.Parse(ids[iLevel - 1]);
                                            wType.TopID = int.Parse(ids[iLevel - 2]);
                                        }
                                        wType.TopTID = int.Parse(ids[0]);
                                    }
                                }
                            }
                            wType.TypeLevel = iLevel;

                        }
                        if (item.s != null && item.s.Count > 0)
                        {
                            iCount++;
                            List<JDWareType> subRtn = ParseSubType(item.s, iCount);
                            if (subRtn != null && subRtn.Count > 0)
                            {
                                //在未获取到分类编号情况下，使用子类上级编号
                                if (wType.TypeID == -1)
                                {
                                    JDWareType subType = subRtn.Find(t => t.TypeLevel == iCount && t.TopID != -1);
                                    wType.TypeID = subType != null ? subType.TopID : -1;
                                    wType.TopID = iLevel == 0 ? 0 : subRtn[0].TopTID;
                                }
                                rtnTypes.AddRange(subRtn);
                            }
                            iCount--;
                        }
                        wType.BEnable = iLevel == 0 && wType.TypeID != -1 ? true : wType.TopID != iLevel && wType.TypeID != -1;
                        rtnTypes.Add(wType);
                    }
                }
                return rtnTypes;
            }
            catch (Exception ex)
            {
                ShowAutoMessage(ex.Message);
                OtCom.XLogErr(ex.Message);
                return null;
            }

        }
    }
}
