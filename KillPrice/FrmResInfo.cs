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
using System.Diagnostics;
using System.Threading;

namespace KillPrice
{
    [DisplayName("商品信息")]
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

        private void FrmResInfo_Load(object sender, EventArgs e)
        {
            progressPanel1.Visible = false;
            _imgPath = Path.Combine(Environment.CurrentDirectory, "Images");
            if (!Directory.Exists(_imgPath))
            {
                Directory.CreateDirectory(_imgPath);
            }
            InitGrid();
            InitComboBox();
            InitMsgGrid();

            if (_myProduct != null && _isEdit)
            {
                InitUiControlsData();
            }
            datePrice.DateTime = DateTime.Now;
        }
        /// <summary>
        /// 初始化类型下拉列表
        /// </summary>
        private void InitComboBox()
        {
            List<ProductType> types = DBHelper.GetInstance().WareTypeGet();
            if (types != null && types.Count > 0)
            {
                cmbType.Properties.DisplayMember = "Name";
                cmbType.Properties.ValueMember = "TID";
                cmbType.Properties.DataSource = types;
                cmbType.Properties.HideSelection = true;

                cmbType.ItemIndex = 0;
            }

        }

        /// <summary>
        /// 初始化界面控件值
        /// </summary>
        private void InitUiControlsData()
        {
            txtID.Text = _myProduct.ProductID;
            txtID.Properties.ReadOnly = true;
            SetUIControls();

            btnGet.Text = "最新信息";
            btnSave.Text = "更新商品";
            //btnGet.Enabled = false;

            imgBox1.Image = LoadImageByStream(_myProduct.ProductImagePath);
            //RefreshPriceGrid();
            //RefreshMsgGridThread();
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
                panelHisPrice.Controls.Add(_pricesView);

            }
            catch (Exception ex)
            {
                //OtCom.XLogErr("LogView-> InitGrid()方法:{0}", ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 填充价格数据列表
        /// </summary>
        private void RefreshPriceGrid()
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
                //pageInfo.Controls.Add(progressPanel1);
                //progressPanel1.BringToFront();
                //progressPanel1.Show();
                string tID = string.IsNullOrEmpty(txtID.Text) ? "1010527324" : txtID.Text.Trim();
                SysParams.GatherModel = GatherType.Single;
                ProductInfo tmpWare = WareDealer.WareService.GetInstance().GetWareInfoByID(tID); 
                if (_isEdit)
                {
                    //如果是浏览/编辑状态，历史低价、价格趋势不变
                    tmpWare.ProductBasePrice = _myProduct.ProductBasePrice;
                    tmpWare.ProductPriceTrend = _myProduct.ProductPriceTrend;
                }
                _myProduct = tmpWare;
                

                SetUIControls();
                
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
            }
            finally
            {
                //progressPanel1.Hide();
            }

        }

        /// <summary>
        /// 清空界面元素值
        /// </summary>
        private void ClearUIControls()
        {
            txtName.Text = "";
            cmbBlong.SelectedIndex = 0;
            cmbStock.SelectedIndex = 0;
            //cmbType.selecti
            txtProductTag.Text = "";
            txtBrand.Text = "";
            txtPicPath.Text = "";
            imgBox1.Image = null;

            txtPrice.Text = "";
            txtBasePrice.Text = "";
            txtPriceType.Text = "";
            cmbPriceTrend.SelectedIndex = 0;

            txtDispatch.Text = "";
            txtPromoMsg.Text = "";
            txtProductCoupon.Text = "";

            txtEvaluate.Text = "";
            txtGoodRate.Text = "";
            txtGeneralRate.Text = "";
            txtPoorRate.Text = "";
            txtHotCommentTag.Text = "";

            _msgsView.ClearData();
            _pricesView.ClearData();
            //_pricesView.RefreshGrid();
        }

        /// <summary>
        /// 设置界面元素值
        /// </summary>
        private void SetUIControls()
        {
            ClearUIControls();
            if (_myProduct != null)
            {
                #region BaseInfo
                txtName.Text = !string.IsNullOrEmpty(_myProduct.ProductName) ? _myProduct.ProductName : txtName.Text;
                txtBrand.Text = _myProduct.ProductBrand;
                txtProductTag.Text = _myProduct.ProductTag;
                cmbStock.Text = _myProduct.ProductStock;
                if (!string.IsNullOrEmpty(_myProduct.ProductAttach))
                {
                    cmbBlong.SelectedIndex = _myProduct.ProductAttach == "自营" ? 0 : _myProduct.ProductAttach == "旗舰店" ? 1 : 2;
                }
                if (!string.IsNullOrEmpty(_myProduct.ProductType))
                {
                    cmbType.EditValue = _myProduct.ProductType;
                }
                imgBox1.Image = _myProduct.ProductImage;
                txtPicPath.Text = _myProduct.ProductImagePath;
                chkFocus.Checked = _myProduct.Focus;
                #endregion BaseInfo

                switch (_myProduct.ProductPriceType)
                {
                    case "京东":
                        txtPrice.Text = _myProduct.ProductPrice.ToString();
                        break;
                    case "手机":
                        txtPrice.Text = _myProduct.ProductMobilePrice.ToString();
                        break;
                    case "QQ":
                        txtPrice.Text = _myProduct.ProductQQPrice.ToString();
                        break;
                    case "微信":
                        txtPrice.Text = _myProduct.ProductWXPrice.ToString();
                        break;
                }
                
                txtPriceType.Text = _myProduct.ProductPriceType;
                txtBasePrice.Text = _myProduct.ProductBasePrice.ToString();
                cmbPriceTrend.Text = _myProduct.ProductPriceTrend;

                txtDispatch.Text = _myProduct.ProductDispatchMode;
                txtPromoMsg.Text = _myProduct.ProductPromoMsg;
                txtProductCoupon.Text = _myProduct.ProductCoupon;

                txtEvaluate.Text = _myProduct.ProductEvaluateCount.ToString();
                txtGoodRate.Text = _myProduct.ProductGoodRate.ToString();
                txtGeneralRate.Text = _myProduct.ProductGeneralRate.ToString();
                txtPoorRate.Text = _myProduct.ProductPoorRate.ToString();
                txtHotCommentTag.Text = _myProduct.ProductHotCommentTag;

                if (_isEdit)
                {
                    //RefreshPriceGrid();
                    //RefreshMsgGrid();
                }
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
                _myProduct.Focus = chkFocus.Checked;
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
                        DBHelper.GetInstance().WareUpdate(_myProduct);
                        //MessageBox.Show("当前商品已经存在！无法继续保存。", "系统警告");
                        //return;
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
                    return WareDealer.WareService.GetInstance().GetRemoteImage(imgPath);
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
                Debug.WriteLine(ex.Message);
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

        private void btnPriceAdd_Click(object sender, EventArgs e)
        {
            try
            {
                double myPrice = double.Parse(txtPrice.Text);
                ProductPriceHistory priceObj = new ProductPriceHistory()
                {
                    PID = _myProduct.ProductID,
                    PriceDate = datePrice.DateTime,
                    Price = myPrice,
                    PriceType = txtPriceType.Text
                };
                DBHelper.GetInstance().WarePriceInsert(priceObj);
                RefreshPriceGrid();
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
            }
        }

        /// <summary>
        /// 获取当前商品价格
        /// </summary>
        private void GetPrice()
        {
            try
            {
                if (MessageBox.Show("是否获取最新价格？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) 
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    progressPanel1.Visible = true;
                    progressPanel1.Show();
                    double myPrice = WareDealer.WareService.GetInstance().GetWarePriceByID(_myProduct.ProductID);
                    if (myPrice > 0)
                    {
                        txtPrice.Text = myPrice.ToString();
                        DBHelper.GetInstance().WarePriceInsert(_myProduct.ProductID, myPrice);
                        RefreshPriceGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                progressPanel1.Visible = false;
            }
            
        }

        private void txtPrice_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            GetPrice();
        }

        private void menuDelPrice_Click(object sender, EventArgs e)
        {
            ProductPriceHistory del = _pricesView.GetCurData<ProductPriceHistory>();
            if (del != null)
            {
                if (MessageBox.Show("是否删除当前价格？\r\n数据删除后将不能恢复，请慎重！", "系统提示", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    {
                        DBHelper.GetInstance().WarePriceDel(del.RID);
                        RefreshPriceGrid();
                    }
                }
            }
        }

        private void btnGetImg_Click(object sender, EventArgs e)
        {
            //重新加载图片
            //1、首先加载本地图片；
            //2、若本地图片不存在，则加载ProductImageWebPath中存储的网络图片
            //3、若网络图片值为空，则到Html中去获取图片地址，并加载图片
            try
            {
                WareDealer.WareService.GetInstance().ReloadImg(_myProduct, true);

                imgBox1.Image = _myProduct.ProductImage;
                txtPicPath.Text = _myProduct.ProductImagePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPriceGet_Click(object sender, EventArgs e)
        {
            GetPrice();
        }

        /// <summary>
        /// 根据查询结果显示商品
        /// </summary>
        /// <param name="index"></param>
        private void ShowMyProduct(int index)
        {
            ProductInfo tmpProduct = PubClass.GetInstance().CurWares[index];
            _myProduct = DBHelper.GetInstance().WareGetOne(tmpProduct.ProductID);
            if (_myProduct != null)
            {
                InitUiControlsData();
            }
            else
            {
                MessageBox.Show("提取商品信息失败，请与杀京东开发商WolfStudio工作室联系！", "系统提示");
            }
        }

        /// <summary>
        /// 下一个商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_myProduct != null && PubClass.GetInstance().CurWares != null && PubClass.GetInstance().CurWares.Count > 0)
            {
                int index = PubClass.GetInstance().CurWares.FindIndex(t => t.ProductID == _myProduct.ProductID);
                if (index >= 0)
                {
                    index++;
                    if (index >= PubClass.GetInstance().CurWares.Count)
                    {
                        index = 0;
                    }
                    ShowMyProduct(index);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(txtID.Text))
                {
                    _myProduct = DBHelper.GetInstance().WareGetOne(txtID.Text);
                }

            }
        }
        /// <summary>
        /// 上一个商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTop_Click(object sender, EventArgs e)
        {
            if (_myProduct != null && PubClass.GetInstance().CurWares != null && PubClass.GetInstance().CurWares.Count > 0)
            {
                int index = PubClass.GetInstance().CurWares.FindIndex(t => t.ProductID == _myProduct.ProductID);
                if (index >= 0)
                {
                    index --;
                    if (index < 0)
                    {
                        index = PubClass.GetInstance().CurWares.Count - 1;
                    }
                    ShowMyProduct(index);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(txtID.Text))
                {
                    _myProduct = DBHelper.GetInstance().WareGetOne(txtID.Text);
                }
                
            }
        }

        private void btnAddCart_Click(object sender, EventArgs e)
        {
            MessageBox.Show("功能开发中,敬请期待！", "系统提示");
        }

        private void btnFlowPic_Click(object sender, EventArgs e)
        {
            MessageBox.Show("功能开发中,敬请期待！", "系统提示");
        }

        private void btnGetCopon_Click(object sender, EventArgs e)
        {
            MessageBox.Show("功能开发中,敬请期待！", "系统提示");
        }

        #region MsgsView
        /// <summary>
        /// 获取商品评价数据流程1
        /// </summary>
        private void GetBadMsgThread()
        {
            InitMsgProcess();
            ShowMsgProcess("获取商品评价数据");
            WareService.GetInstance().GetEvaluateMsg(_myProduct.ProductID, true);
            ShowMsgProcess("评价数据入库");
            RefreshMsgGrid();
        }
        /// <summary>
        /// 初始化评价进度
        /// </summary>
        private void InitMsgProcess()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(InitMsgProcess), null);
            }
            else
            {
                pagePubMouth.Controls.Add(progressPanel1);
                progressPanel1.BringToFront();
                progressPanel1.Description = "努力加载中...";
                progressPanel1.Show();
                groupControl3.Text = "差评信息（0）";
            }
        }
        /// <summary>
        /// 在进度条上显示消息
        /// </summary>
        /// <param name="msg"></param>
        private void ShowMsgProcess(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(ShowMsgProcess), msg);
            }
            else
            {
                progressPanel1.Description = msg + "...";
            }
        }
        /// <summary>
        /// 进度完成后处理流程
        /// </summary>
        /// <param name="iCount"></param>
        private void EndMsgProcess(int iCount)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(EndMsgProcess), iCount);
            }
            else
            {
                progressPanel1.Hide();
                groupControl3.Text = string.Format("差评信息（{0}）", iCount);
            }
        }

        private void btnEvaluation_Click(object sender, EventArgs e)
        {
            _isGetProcess = true;
            Thread badMsgThread = new Thread(delegate() { GetBadMsgThread(); }) { Name = "badMsgThread", IsBackground = true };
            badMsgThread.Start();
        }

        OtDevGridView _msgsView = new OtDevGridView();
        bool _isGetProcess = false;
        /// <summary>
        /// 初始化评价列表
        /// </summary>
        private void InitMsgGrid()
        {
            try
            {
                List<DevColHeads> Heads = new List<DevColHeads>();

                DevColHeads col = new DevColHeads();
                col.HeadName = "编号";
                col.IsShow = false;
                col.FieldName = "PID";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.IsIDtag = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "评价日期";
                col.FieldName = "MsgDate";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.ColWidth = 66;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "评价内容";
                col.FieldName = "MsgContent";
                col.DataType = "System.String";
                col.ColWidth = 240;
                col.AllowSize = true;
                col.EditStyle = enum_RespEditType.MemoEdit;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "评价人";
                col.FieldName = "MsgUser";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.ColWidth = 54;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "评价人等级";
                col.FieldName = "MsgUserLevel";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.ColWidth = 54;
                Heads.Add(col);

                _msgsView.ShowMainTool = false;
                _msgsView.ShowPageToolBar = false;
                //_msgsView.RowAutoHeight = true;
                //_msgsView.ModelView = enum_ModelView.Card;
                //_msgsView.ColumnWidthChanged += 

                _msgsView.InitGrid(Heads);
                //_productView.ContextMenuStrip = menus;
                //_productView.AddCondition("[LvlId]=0", GetColor(OtCom.GetConstCfg("Log_NormalColor", "-1")));
                //_productView.AddCondition("[LvlId]=1", GetColor(OtCom.GetConstCfg("Log_TipColor", "-1")));
                //_productView.AddCondition("[LvlId]=2", GetColor(OtCom.GetConstCfg("Log_ErrorColor", "-1")));
                //_productView.AddCondition("[LvlId]=3", GetColor(OtCom.GetConstCfg("Log_WarningColor", "-1")));
                //_productView.AddCondition("[LvlId]=4", GetColor(OtCom.GetConstCfg("Log_DebugColor", "-1")));

                _msgsView.Dock = DockStyle.Fill;
                //_msgsView.ContextMenuStrip = contextMenuStrip1;
                groupControl3.Controls.Add(_msgsView);

            }
            catch (Exception ex)
            {
                //OtCom.XLogErr("LogView-> InitGrid()方法:{0}", ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 填充评价信息线程
        /// </summary>
        private void RefreshMsgGridThread()
        {
            _msgsView.ClearData();
            _isGetProcess = false;
            Thread msgGridShowThread = new Thread(delegate() { RefreshMsgGrid(); }) { Name = "msgGridShowThread", IsBackground = true };
            msgGridShowThread.Start();
        }
        /// <summary>
        /// 填充评价信息列表
        /// </summary>
        private void RefreshMsgGrid()
        {
            if (_myProduct != null && !string.IsNullOrEmpty(_myProduct.ProductID))
            {
                if (!_isGetProcess)
                {
                    InitMsgProcess();
                }
                List<ProductMessage> rtnList = DBHelper.GetInstance().WareMessageGet(_myProduct.ProductID);
                int iCount = rtnList != null ? rtnList.Count : 0;
                RefreshMsgGrid(rtnList);
                EndMsgProcess(iCount);
            }

        }
        /// <summary>
        /// 跨线程时处理评价列表
        /// </summary>
        /// <param name="msgs"></param>
        private void RefreshMsgGrid(List<ProductMessage> msgs)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<List<ProductMessage>>(RefreshMsgGrid), msgs);
            }
            else
            {
                if (msgs != null && msgs.Count > 0)
                {
                    _msgsView.RefreshGrid(msgs);
                }
                else
                {
                    _msgsView.ClearData();
                }
                
            }
        }

        #endregion MsgsView


        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (_isEdit)
            {
                if (e.Page == pagePrice)
                {
                    RefreshPriceGrid();
                }
                if (e.Page == pagePubMouth)
                {
                    RefreshMsgGridThread();
                    //RefreshMsgGrid();
                }
                
            }
            
        }

        
    }
}
