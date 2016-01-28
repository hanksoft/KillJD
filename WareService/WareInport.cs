using WareDealer.Helper;
using WareDealer.Mode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareDealer
{
    /// <summary>
    /// 商品导入
    /// </summary>
    public class WareInport
    {
        //导入文本文件中数据
        //格式要求如 http://item.jd.com/1711116.html
        private WareInport() {}

        private static WareInport _inport;
        public static WareInport GetInstance()
        {
            return _inport ?? (_inport = new WareInport());
        }

        /// <summary>
        /// 导入商品列表
        /// </summary>
        /// <param name="filePath">文本文件路径</param>
        public void InportWareList(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                Read(filePath);
            }
        }

        private ProductInfo _myProduct = null;

        /// <summary>
        /// 读取导入列表文件
        /// </summary>
        /// <param name="path"></param>
        private void Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            InportThreads.WareLength = sr.ReadToEnd().Split('\n').Length;
            if (sr.EndOfStream)
            {
				//重置文件指针至文件头
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
            }
            String line;
            List<ProductInfo> wareList = new List<ProductInfo>();
            while ((line = sr.ReadLine()) != null)
            {
                _myProduct = WareService.GetInstance().GetWareInfo(line);
                if (_myProduct != null)
                {
                    wareList.Add(_myProduct);
                }
                InportThreads.WareStep++;
            }

            foreach (var item in wareList)
            {
                DBHelper.GetInstance().WareInsert(item);
                InportThreads.WareStep++;
            }

            InportThreads.WareEnd = true;
        }
    }

    public static class InportThreads
    {
        public static int WareLength { get; set; }

        public static int WareStep { get; set; }

        public static bool WareEnd { get; set; }
    }
}
