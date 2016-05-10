using WareDealer.Helper;
using WareDealer.Mode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hank.BrowserParse;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WareDealer
{
    /// <summary>
    /// 商品导入
    /// </summary>
    public class WareImport
    {
        #region field
        private static WareImport _inport;
        /// <summary>
        /// 网站地址列表
        /// </summary>
        private List<WebSiteModel> _sites;
        /// <summary>
        /// 商品对象
        /// </summary>
        private ProductInfo _myProduct = null;
        #endregion field

        //导入文本文件中数据
        //格式要求如 http://item.jd.com/1711116.html
        private WareImport() {}
        
        public static WareImport GetInstance()
        {
            return _inport ?? (_inport = new WareImport());
        }

        public void ImportWareList(List<WebSiteModel> webSites)
        {
            _sites = webSites ?? new List<WebSiteModel>();
            if (_sites != null && _sites.Count > 0)
            {
                ImportWares(_sites);
            }
        }
        /// <summary>
        /// 导入商品列表
        /// </summary>
        /// <param name="filePath">文本文件路径</param>
        public void ImportWareList(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                List<WebSiteModel> webSites = Read(filePath);
                ImportWares(webSites);
            }
        }

        private int _step = 0;
        /// <summary>
        /// 操作进度条
        /// </summary>
        public Action<int> ShowStep;
        private void ShowImportStep(int iStep)
        {
            if (ShowStep != null)
            {
                ShowStep(iStep);
            }
        }
        /// <summary>
        /// 控制消息框
        /// </summary>
        public Action<string> ShowMessage;
        private void ShowImportMessage(string msg)
        {
            if (ShowMessage != null)
            {
                ShowMessage(msg);
            }
        }
        /// <summary>
        /// 控制进度条
        /// </summary>
        public Action<bool> EndProcess;
        private void EndImportProcess(bool bRtn)
        {
            if (EndProcess != null)
            {
                EndProcess(bRtn);
            }
        }

        /// <summary>
        /// 导入商品
        /// </summary>
        /// <param name="webSites">商品地址数据</param>
        private void ImportWares(List<WebSiteModel> webSites)
        {
            try
            {
                //确定采集模式为批量采集
                SysParams.GatherModel = GatherType.Batch;
                _step = 0;
                ShowImportMessage("导入商品线程开始");
                List<ProductInfo> wareList = new List<ProductInfo>();
                ShowImportMessage("等待导入商品总数：" + webSites.Count.ToString());
                foreach (var item in webSites)
                {
                    string pid = Regex.Match(item.url, "\\d{1,14}").Value;
                    //如果商品编号已存在，则不导入
                    if (!string.IsNullOrEmpty(pid) && !DBHelper.GetInstance().WareIsExists(pid))
                    {
                        ImportThreads.WareID = pid;
                        ShowImportMessage("获取商品数据" + item.url);
                        _myProduct = WareService.GetInstance().GetWareInfo(item.url);
                        if (_myProduct != null)
                        {
                            wareList.Add(_myProduct);
                        }
                    }
                    else
                    {
                        ShowImportMessage("商品" + item.url + "已存在");
                    }
                    ShowImportStep(_step);
                    _step++;
                }
                ShowImportMessage("已获取商品总数：" + wareList.Count.ToString());

                foreach (var item in wareList)
                {
                    ShowImportMessage("导入商品数据" + item.ProductName);
                    DBHelper.GetInstance().WareInsert(item);
                    DBHelper.GetInstance().WarePriceInsert(item.ProductID, item.ProductPrice);
                    ShowImportStep(_step);
                    _step++;
                }
                while (_step <= webSites.Count * 2)
                {
                    _step++;
                    ShowImportStep(_step);
                }
                ShowImportMessage("导入商品线程结束");
                EndImportProcess(true);
            }
            catch (Exception ex)
            {
                EndImportProcess(false);
                ShowImportMessage("导入商品线程异常" + ex.Message);
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 读取导入列表文件
        /// </summary>
        /// <param name="path"></param>
        private List<WebSiteModel> Read(string path)
        {
            try
            {
                List<WebSiteModel> webList = new List<WebSiteModel>();
                StreamReader sr = new StreamReader(path, Encoding.Default);
                ImportThreads.WareLength = sr.ReadToEnd().Split('\n').Length;
                if (sr.EndOfStream)
                {
                    //重置文件指针至文件头
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                }
                String line;

                while ((line = sr.ReadLine()) != null)
                {
                    webList.Add(new WebSiteModel() { url = line });
                }

                return webList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
            
        }
    }

    /// <summary>
    /// 导入线程模型
    /// </summary>
    public static class ImportThreads
    {
        /// <summary>
        /// 导入商品数量
        /// </summary>
        public static int WareLength { get; set; }
        /// <summary>
        /// 当前导入个数
        /// </summary>
        public static int WareStep { get; set; }
        /// <summary>
        /// 导入商品完成状态
        /// </summary>
        public static bool WareEnd { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public static string WareID { get; set; }
        /// <summary>
        /// 操作类型 1、获取商品数据 2、导入商品数据
        /// </summary>
        public static string LastMsg { get; set; }
    }
}
