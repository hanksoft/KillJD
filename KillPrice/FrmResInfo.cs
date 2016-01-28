using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using Hank.ComLib;
using Hank.UiCtlLib;
using WareDealer;
using WareDealer.Mode;
using WareDealer.Helper;

namespace KillPrice
{
    public partial class FrmResInfo : Form
    {
        ProductInfo _myProduct = null;
        /// <summary>
        /// 是否编辑状态
        /// </summary>
        private bool _isEdit = false;
        public FrmResInfo()
        {
            InitializeComponent();
        }

        public FrmResInfo(ProductInfo pInfo)
        {
            InitializeComponent();
            _isEdit = true;
            _myProduct = pInfo;
            btnGet.Text = "最新信息";
            btnSave.Text = "更新商品";
        }

        #region GridView
        OtDevGridView _pricesView = new OtDevGridView();

        private void InitGrid()
        {
            try
            {
                List<DevColHeads> Heads = new List<DevColHeads>();

                DevColHeads col = new DevColHeads();
                col.HeadName = "编号";
                col.IsShow = false;
                col.FieldName = "RID";
                col.DataType = "System.Int64";
                col.ColWidth = 50;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "询价日期";
                col.FieldName = "PriceTime";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.ColWidth = 100;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "价格";
                col.FieldName = "Price";
                col.DataType = "System.Double";
                col.ColWidth = 30;
                col.AllowSize = true;
                col.IsIDtag = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "类型";
                col.FieldName = "PriceType";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.ColWidth = 80;
                Heads.Add(col);

                _pricesView.ShowMainTool = false;
                _pricesView.ShowPageToolBar = false;

                _pricesView.InitGrid(Heads);
                //_productView.ContextMenuStrip = menus;
                //_productView.AddCondition("[LvlId]=0", GetColor(OtCom.GetConstCfg("Log_NormalColor", "-1")));
                //_productView.AddCondition("[LvlId]=1", GetColor(OtCom.GetConstCfg("Log_TipColor", "-1")));
                //_productView.AddCondition("[LvlId]=2", GetColor(OtCom.GetConstCfg("Log_ErrorColor", "-1")));
                //_productView.AddCondition("[LvlId]=3", GetColor(OtCom.GetConstCfg("Log_WarningColor", "-1")));
                //_productView.AddCondition("[LvlId]=4", GetColor(OtCom.GetConstCfg("Log_DebugColor", "-1")));

                _pricesView.Dock = DockStyle.Fill;
                _pricesView.ContextMenuStrip = contextMenuStrip1;
                groupControl2.Controls.Add(_pricesView);

            }
            catch (Exception ex)
            {
                //OtCom.XLogErr("LogView-> InitGrid()方法:{0}", ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        private void RefreshGrid()
        {
            if (_myProduct != null && !string.IsNullOrEmpty(_myProduct.ProductID))
            {
                List<ProductPriceHistory> rtnList = DBHelper.GetInstance().WarePriceHistoryGet(_myProduct.ProductID);
                if (rtnList != null && rtnList.Count > 0)
                {
                    _pricesView.RefreshGrid(rtnList);
                }
            }
        }

        #endregion GridView

        private void btnGet_Click(object sender, EventArgs e)
        {
            try
            {
                string tID = string.IsNullOrEmpty(txtID.Text) ? "1010527324" : txtID.Text.Trim();
                _myProduct = WareDealer.WareService.GetInstance().GetWareInfoByID(tID);

                SetUIControls();

            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
            }

        }

        /// <summary>
        /// 清空界面元素值
        /// </summary>
        private void ClearUIControls()
        {
            txtName.Text = "";
            txtPrice.Text = "";
            txtEvaluate.Text = "";
            txtGoodRate.Text = "";
            imgBox1.Image = null;
            txtDispatch.Text = "";
            txtPicPath.Text = "";
            txtBrand.Text = "";
            txtProductTag.Text = "";

            _pricesView.RefreshGrid();
        }

        /// <summary>
        /// 设置界面元素值
        /// </summary>
        private void SetUIControls()
        {
            ClearUIControls();
            if (_myProduct != null)
            {
                txtName.Text = !string.IsNullOrEmpty(_myProduct.ProductName) ? _myProduct.ProductName : txtName.Text;
                txtPrice.Text = _myProduct.ProductPrice.ToString();
                txtEvaluate.Text = _myProduct.ProductEvaluateCount.ToString();
                txtGoodRate.Text = _myProduct.ProductGoodRate.ToString();
                //memoResponse.Text = _myProduct.NativeData;
                imgBox1.Image = _myProduct.ProductImage;
                txtDispatch.Text = _myProduct.ProductDispatchMode;
                txtPicPath.Text = _myProduct.ProductImagePath;
                txtBrand.Text = _myProduct.ProductBrand;
                txtProductTag.Text = _myProduct.ProductTag;
                cmbPriceTrend.Text = _myProduct.ProductPriceTrend;
                txtPriceType.Text = _myProduct.ProductPriceType;

                if (!string.IsNullOrEmpty(_myProduct.ProductAttach))
                {
                    cmbBlong.SelectedIndex = _myProduct.ProductAttach == "自营" ? 0 : _myProduct.ProductAttach == "旗舰店" ? 1 : 2;
                }

                if (!string.IsNullOrEmpty(_myProduct.ProductType))
                {
                    cmbType.EditValue = _myProduct.ProductType;
                    //cmbType.ItemIndex = 0;
                }
                cmbStock.Text = _myProduct.ProductStock;

            }
            else
            {
                txtName.Text = "未找到编号为" + txtID.Text.Trim() + "的商品！";
            }

        }

        /// <summary>
        /// 获取界面元素值
        /// </summary>
        private void GetUiControlsText()
        {
            if (_myProduct != null)
            {
                _myProduct.ProductName = txtName.Text;
                _myProduct.ProductPrice = double.Parse(txtPrice.Text);
                _myProduct.ProductBasePrice = double.Parse(txtPrice.Text);
                _myProduct.ProductBrand = txtBrand.Text;
                _myProduct.ProductType = cmbType.EditValue.ToString();
                _myProduct.ProductTag = txtProductTag.Text;
                _myProduct.ProductAttach = cmbBlong.EditValue.ToString();
                //-1 下柜 0 无货 1 有货 2 配货 3 预订
                string sKC = cmbStock.Text;
                switch (sKC)
                {
                    case "下柜":
                        _myProduct.ProductIsSaled = -1;
                        break;
                    case "无货":
                        _myProduct.ProductIsSaled = 0;
                        break;
                    case "配货":
                        _myProduct.ProductIsSaled = 2;
                        break;
                    case "预订":
                        _myProduct.ProductIsSaled = 3;
                        break;
                    default:
                        _myProduct.ProductIsSaled = 1;
                        break;
                }
                _myProduct.ProductPriceTrend = cmbPriceTrend.EditValue.ToString();
                _myProduct.ProductPriceType = cmbBlong.EditValue.ToString();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //获取界面元素值
                GetUiControlsText();
                if (_isEdit)
                {
                    DBHelper.GetInstance().WareUpdate(_myProduct);
                }
                else
                {
                    if (!DBHelper.GetInstance().WareIsExists(_myProduct.ProductID))
                    {
                        DBHelper.GetInstance().WareInsert(_myProduct);
                        DBHelper.GetInstance().WarePriceInsert(_myProduct.ProductID, _myProduct.ProductPrice);
                    }
                    else
                    {
                        MessageBox.Show("当前商品已经存在！无法继续保存。", "系统警告");
                        return;
                    }
                }
                MessageBox.Show("商品保存成功！", "系统提示");
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
                MessageBox.Show("商品保存失败！", "系统提示");
            }

        }

        private string _imgPath = "";
        private void FrmResInfo_Load(object sender, EventArgs e)
        {
            _imgPath = Path.Combine(Environment.CurrentDirectory, "Images");
            if (!Directory.Exists(_imgPath))
            {
                Directory.CreateDirectory(_imgPath);
            }
            InitGrid();
            InitComboBox();

            if (_myProduct != null)
            {
                InitUiControlsData();
            }
            dateEdit1.DateTime = DateTime.Now;
        }

        private void InitComboBox()
        {
            List<Dic_ProductType> types = DBHelper.GetInstance().WareTypeGet();
            if (types != null && types.Count > 0)
            {
                cmbType.Properties.DisplayMember = "Name";
                cmbType.Properties.ValueMember = "TID";
                cmbType.Properties.DataSource = types;
                cmbType.Properties.HideSelection = true;

                cmbType.ItemIndex = 0;
            }

        }

        private void InitUiControlsData()
        {
            txtID.Text = _myProduct.ProductID;
            txtID.Properties.ReadOnly = true;
            SetUIControls();
            
            btnGet.Text = "最新信息";
            btnSave.Text = "更新商品";
            //btnGet.Enabled = false;

            imgBox1.Image = LoadImageByStream(_myProduct.ProductImagePath);
            RefreshGrid();
        }

        /// <summary>
        /// 通过内存流加载图片
        /// </summary>
        /// <param name="imgPath">图片全路径</param>
        /// <returns></returns>
        private Image LoadImageByStream(string imgPath)
        {
            Image rtnImg = null;

            try
            {
                //img11.360buyimg.com/n1/g10/M00/14/0E/rBEQWFFn04IIAAAAAAOO4IarXP8AAD_wAAMNjcAA474464.jpg
                if (Regex.Match(imgPath, "^//img").Success)
                {
                    return WareDealer.WareService.GetInstance().Get_img(imgPath);
                }
                if (!string.IsNullOrEmpty(imgPath) && File.Exists(imgPath))
                {
                    using (FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        byte[] filebytes = new byte[fs.Length];
                        fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));

                        rtnImg = Image.FromStream(new MemoryStream(filebytes));
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }


            return rtnImg;
        }

        private void btnOpenUrl_Click(object sender, EventArgs e)
        {
            if (_myProduct != null)
            {
                System.Diagnostics.Process.Start(_myProduct.ProductURL);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPriceGet_Click(object sender, EventArgs e)
        {
            try
            {
                double myPrice = double.Parse(txtPrice.Text);
                ProductPriceHistory priceObj = new ProductPriceHistory()
                {
                    PID = _myProduct.ProductID,
                    PriceDate = dateEdit1.DateTime,
                    Price = myPrice,
                    PriceType = txtPriceType.Text
                };
                DBHelper.GetInstance().WarePriceInsert(priceObj);
                RefreshGrid();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
            }


        }

        private void txtPrice_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (MessageBox.Show("是否获取最新价格？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                double myPrice = WareDealer.WareService.GetInstance().GetWarePriceByID(_myProduct.ProductID);
                if (myPrice > 0)
                {
                    txtPrice.Text = myPrice.ToString();
                    DBHelper.GetInstance().WarePriceInsert(_myProduct.ProductID, myPrice);
                    RefreshGrid();
                }
            }
        }

        private void menuDelPrice_Click(object sender, EventArgs e)
        {
            ProductPriceHistory del = _pricesView.GetCurData<ProductPriceHistory>();
            if (del != null)
            {
                if (MessageBox.Show("是否删除当前价格？\r\n数据删除后将不能恢复，请慎重！", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    {
                        DBHelper.GetInstance().WarePriceDel(del.RID);
                        RefreshGrid();
                    }
                }
            }
        }

        private void btnGetImg_Click(object sender, EventArgs e)
        {
            //重新加载图片
            //1、首先加载本地图片；
            //2、若本地图片不存在，则加载ProductImageWebPath中存储的网络图片
            //3、若网络图片值为空，则到Html中去图片地址，并加载图片
            WareDealer.WareService.GetInstance().ReloadImg(_myProduct);
            imgBox1.Image = _myProduct.ProductImage;
            txtPicPath.Text = _myProduct.ProductImagePath;
        }


    }
}
