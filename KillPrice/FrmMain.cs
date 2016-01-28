using DevExpress.XtraTreeList.Nodes;
using Hank.ComLib;
using Hank.UiCtlLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WareDealer.Helper;
using WareDealer.Mode;

namespace KillPrice
{
    public partial class FrmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnGetProduct_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmResInfo resInfo = new FrmResInfo();
            resInfo.ShowDialog();
        }
        OtDevGridView _productView = new OtDevGridView();
        Hank.UiCtlLib.BaseTree _typeManagerTree = new BaseTree();
        private void InitTypeTree()
        {
            ImageList imglist = new ImageList();
            imglist.Images.Add(Properties.Resources.Open);
            imglist.Images.Add(Properties.Resources.closed);
            imglist.Images.Add(Properties.Resources.group);
            imglist.Images.Add(Properties.Resources.set16);
            _typeManagerTree.ImageList = imglist;
            _typeManagerTree.Appearance.FocusedCell.BackColor = Color.DarkOrange;

            TreeListNode jdNode = _typeManagerTree.AddNode(null, "京东", "JINDONG", null, null);
            jdNode.ImageIndex = 1;
            jdNode.SelectImageIndex = 0;

            TreeListNode allNode = _typeManagerTree.AddNode(jdNode, "全部", "ALL", null, null);
            allNode.ImageIndex = 1;
            allNode.SelectImageIndex = 0;

            List<Dic_ProductType> rtnList = DBHelper.GetInstance().WareTypeGet();
            foreach (var item in rtnList)
            {
                TreeListNode jdNode1 = _typeManagerTree.AddNode(jdNode, item.Name, item.TID, null, null);
                jdNode1.ImageIndex = 1;
                jdNode1.SelectImageIndex = 0;
            }

            //jdNode = _typeManagerTree.AddNode(null, "淘宝", "TAOBAO", null, null);
            //jdNode.ImageIndex = 1;
            //jdNode.SelectImageIndex = 0;

            _typeManagerTree.ContextMenuStrip = treeMenu;
            _typeManagerTree.FocusedNodeChanged +=_typeManagerTree_FocusedNodeChanged;
            _typeManagerTree.Dock = DockStyle.Fill;
            splitContainerControl1.Panel1.Controls.Add(_typeManagerTree);
            _typeManagerTree.ExpandAll();
            _typeManagerTree.FocusedNode = allNode;
        }

        private void _typeManagerTree_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (e.Node != null)
            {
                //object nowNod = _typeManagerTree.GetValue(e.Node);
                object nowKey = _typeManagerTree.GetNodeKey(e.Node);
                if (nowKey.ToString() == "ALL")
                {
                    RefreshGrid();
                }
                else
                {
                    RefreshGrid(nowKey.ToString());
                }
            }
        }
        private void InitDataGrid()
        {
            
            try
            {
                List<DevColHeads> Heads = new List<DevColHeads>();

                DevColHeads col = new DevColHeads();
                col.HeadName = "编号";
                col.IsShow = false;
                col.FieldName = "RID";
                col.DataType = "System.String";
                col.ColWidth = 50;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "商品编号";
                col.FieldName = "ProductID";
                col.DataType = "System.String";
                col.ColWidth = 30;
                col.AllowSize = true;
                col.IsShow = false;
                col.IsIDtag = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "商品名称";
                col.FieldName = "ProductName";
                col.DataType = "System.String";
                col.ColWidth = 200;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "商品分类";
                col.FieldName = "TypeName";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.ColWidth = 40;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "商品标签";
                col.FieldName = "ProductTag";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.ColWidth = 40;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "历史底价";
                col.FieldName = "ProductBasePrice";
                col.DataType = "System.Double";
                col.ColWidth = 30;
                //col.IsShow = false;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "当前价格";
                col.FieldName = "ProductPrice";
                col.DataType = "System.Double";
                col.ColWidth = 30;
                //col.IsShow = false;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "价格趋势";
                col.FieldName = "ProductPriceTrend";
                col.DataType = "System.String";
                col.ColWidth = 30;
                //col.IsShow = false;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "商品库存";
                col.FieldName = "ProductIsSaled";
                col.DataType = "System.Int32";
                col.ColWidth = 30;
                col.IsShow = false;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "商品库存";
                col.FieldName = "ProductStock";
                col.DataType = "System.String";
                col.ColWidth = 30;
                //col.IsShow = false;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "询价日期";
                col.FieldName = "ProductPriceDate";
                col.DataType = "System.DateTime";
                col.ColWidth = 30;
                //col.IsShow = false;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "商品归属";
                col.FieldName = "ProductAttach";
                col.DataType = "System.String";
                col.ColWidth = 30;
                //col.IsShow = false;
                col.AllowSize = true;
                col.ColWidth = 50;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "评价数量";
                col.FieldName = "ProductEvaluateCount";
                col.DataType = "System.Int32";
                col.AllowSize = true;
                col.ColWidth = 20;
                //col.IsShow = false;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "好评率";
                col.FieldName = "ProductGoodRate";
                col.AllowSize = true;
                col.DataType = "System.Double";
                //col.IsShow = false;
                col.ColWidth = 20;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "商店编号";
                col.FieldName = "VenderId";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.IsShow = false;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "配送区域";
                col.FieldName = "CatArea";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.IsShow = false;
                Heads.Add(col);

                _productView.ShowMainTool = false;
                _productView.ShowPageToolBar = false;
                _productView.IsCheck = true;

                _productView.InitGrid(Heads);
                _productView.ContextMenuStrip = contextMenus;
                _productView.AddCondition("[ProductIsSaled]=0", Color.IndianRed);
                _productView.AddCondition("[ProductIsSaled]=-1", Color.LightGray);
                //_productView.AddCondition("[ProductStock]='无货'", GetColor(OtCom.GetConstCfg("Log_NormalColor", "-1")));
                //_productView.AddCondition("[LvlId]=1", GetColor(OtCom.GetConstCfg("Log_TipColor", "-1")));
                //_productView.AddCondition("[LvlId]=2", GetColor(OtCom.GetConstCfg("Log_ErrorColor", "-1")));
                //_productView.AddCondition("[LvlId]=3", GetColor(OtCom.GetConstCfg("Log_WarningColor", "-1")));
                //_productView.AddCondition("[LvlId]=4", GetColor(OtCom.GetConstCfg("Log_DebugColor", "-1")));

                _productView.GridDBClick += new OtDevGridView.GridDBClickHandler(_productView_GridDBClick);

                //RefreshGrid();
            }
            catch (Exception ex)
            {
                //OtCom.XLogErr("LogView-> InitGrid()方法:{0}", ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        void _productView_GridDBClick(System.Data.DataRow dataRow, object objValue, EventArgs e)
        {
            ProductInfo curData = _productView.GetCurData<ProductInfo>();
            if (curData != null)
            {
                FrmResInfo resInfo = new FrmResInfo(curData);
                resInfo.ShowDialog();
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            InitDataGrid();
            _productView.Dock = DockStyle.Fill;
            splitContainerControl1.Panel2.Controls.Add(_productView);

            barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barProcess.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            InitTypeTree();
            
        }

        private void menuGet_Click(object sender, EventArgs e)
        {
            btnGetProduct_ItemClick(null, null);
        }
        
        private void RefreshGrid()
        {
            List<ProductInfo> rtnList = DBHelper.GetInstance().WareGetAll();
            if (rtnList != null && rtnList.Count > 0)
            {
                _productView.RefreshGrid(rtnList);
            }
        }

        private void RefreshGrid(string typeKey)
        {
            List<ProductInfo> rtnList = DBHelper.GetInstance().WareGetAll(typeKey);
            _productView.RefreshGrid(rtnList);
            
        }

        private void menuRefresh_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void menuDel_Click(object sender, EventArgs e)
        {
            ProductInfo delObj = _productView.GetCurData<ProductInfo>();
            if (delObj != null)
            {
                DBHelper.GetInstance().WareDelOne(delObj.RID);
                RefreshGrid();
            }
            
        }

        private void menuUrl_Click(object sender, EventArgs e)
        {
            ProductInfo _myProduct = _productView.GetCurData<ProductInfo>();
            if (_myProduct != null)
            {
                System.Diagnostics.Process.Start(_myProduct.ProductURL);
            }
        }

        private void btnInport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InportWareURL();
        }

        /// <summary>
        /// 导入商品URL
        /// </summary>
        private void InportWareURL()
        {
            UiImportWare import = new UiImportWare();
            Form frm = new Form();
            frm.MinimizeBox = false;
            frm.MaximizeBox = false;
            frm.Height = import.Size.Height;
            frm.Width = import.Size.Width;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.Controls.Add(import);
            import.Dock = DockStyle.Fill;
            frm.ShowDialog();
            //FrmInportWare inport = new FrmInportWare();
            //inport.ShowDialog();
        }

        /// <summary>
        /// 删除指定商品
        /// </summary>
        private void DelWares()
        {

            List<ProductInfo> dels = _productView.GetCheckList<ProductInfo>();
            if (dels != null && dels.Count > 0)
            {
                if (MessageBox.Show("商品删除后无法恢复！是否继续删除？", "系统警告", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (var item in dels)
                    {
                        DBHelper.GetInstance().WareDelOne(item.ProductID);
                        try
                        {
                            string imgPath = Path.Combine(Environment.CurrentDirectory, item.ProductImagePath);
                            if (File.Exists(imgPath))
                            {
                                File.Delete(imgPath);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                        
                    }
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show("请选择需要删除的商品！", "系统提示");
            }

        }

        private void UpdateWareThread()
        {
            barProcess.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            Thread updateThread = new Thread(delegate() { UpdateWare(); }) { Name = "updateThread", IsBackground = true };
            updateThread.Start();
        }

        /// <summary>
        /// 更新商品信息
        /// </summary>
        private void UpdateWare()
        {
            
            List<ProductInfo> updates = _productView.GetAll<ProductInfo>();
            //List<ProductInfo> updates = _productView.GetCheckList<ProductInfo>();
            if (updates != null && updates.Count > 0)
            {
                int i = 0;
                InitProcess(updates.Count);
                //barProcess.EditValue = "0";
                //repositoryItemProgressBar1.Maximum = updates.Count;
                foreach (var item in updates)
                {
                    i++;
                    WareDealer.WareService.GetInstance().UpdateWareinfo(item);
                    //WareDealer.WareService.GetInstance().UpdateWarePriceByID(item.ProductID);
                    ShowProcessing(barProcess, i.ToString());
                }
            }
            RefreshGrid();
            InitProcessBar();
        }

        private void InitProcess(int leng)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { barProcess.EditValue = "0"; repositoryItemProgressBar1.Maximum = leng; });
            }
            else
            {
                barProcess.EditValue = "0"; 
                repositoryItemProgressBar1.Maximum = leng;
            }
        }

        private void InitProcessBar()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    barProcess.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                });
            }
            else
            {
                barProcess.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
            
        }

        private void ShowProcessing(DevExpress.XtraBars.BarEditItem item, string msgString)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { item.EditValue = msgString; item.Caption = msgString; });
            }
            else
            {
                item.Caption = msgString;
                item.EditValue = msgString;
            }
        }

        private void menuInport_Click(object sender, EventArgs e)
        {
            InportWareURL();
            RefreshGrid();
        }

        private void btnDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DelWares();
        }

        private void btnUpdate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UpdateWareThread();
        }

        private void menuUpdate_Click(object sender, EventArgs e)
        {
            
            UpdateWareThread();
        }

        private void barProcess_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //barProcess.
        }

        private void btnMain_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RefreshGrid();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UiTypes types = new UiTypes();
            Form form = new Form();
            form.Controls.Add(types);
            form.Show();
        }

        private void btnAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmAbout uiAbout = new FrmAbout();
            uiAbout.ShowDialog();
        }

        private void btnExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
    }
}