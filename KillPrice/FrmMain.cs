using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using Hank.BrowserParse;
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
using WareDealer;
using WareDealer.Helper;
using WareDealer.Mode;

namespace KillPrice
{
    [DisplayName("杀京东主界面")]
    public partial class FrmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        #region Field Define
        OtDevGridView _productView = new OtDevGridView();
        Hank.UiCtlLib.BaseTree _typeManagerTree = new BaseTree();
        #endregion Field Define

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            InitSysparams();
            InitDataGrid();
            _productView.Dock = DockStyle.Fill;
            plGrid.Controls.Add(_productView);

            txtUpdater.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barProcess.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            InitTypeTree();
            plTools.Visible = false;

            InitTimers();
        }

        private void InitTimers()
        {
            timer4Net.Interval = 60000;
            timer4Net.Enabled = true;

            timer4Focus.Interval = int.Parse(SysParams.AutoGetFocusTime) * 1000;
            timer4Focus.Enabled = SysParams.AllowAutoGetFocusWare;

            timer4UpdateWare.Interval = int.Parse(SysParams.AutoUpdateTime) * 1000;
            timer4UpdateWare.Enabled = SysParams.AllowAutoUpdateWareInfo;
        }

        #region TypeTree Code
        private void InitTypeTree()
        {
            try
            {
                _typeManagerTree.BeginUpdate();
                if (_typeManagerTree.NodesCount > 0)
                {
                    //_typeManagerTree.ClearNodes();
                    _typeManagerTree.Nodes.Clear();

                }
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

                TreeListNode subNode1 = _typeManagerTree.AddNode(allNode, "走低", "down", null, null);
                subNode1.ImageIndex = 1;
                subNode1.SelectImageIndex = 0;
                TreeListNode subNode2 = _typeManagerTree.AddNode(allNode, "涨价", "up", null, null);
                subNode2.ImageIndex = 1;
                subNode2.SelectImageIndex = 0;
                TreeListNode subNode3 = _typeManagerTree.AddNode(allNode, "持平", "balance", null, null);
                subNode3.ImageIndex = 1;
                subNode3.SelectImageIndex = 0;
                TreeListNode subNode4 = _typeManagerTree.AddNode(allNode, "关注", "focus", null, null);
                subNode4.ImageIndex = 1;
                subNode4.SelectImageIndex = 0;

                TreeListNode unNode = _typeManagerTree.AddNode(jdNode, "未分类", "UnType", null, null);
                unNode.ImageIndex = 1;
                unNode.SelectImageIndex = 0;
                List<ProductType> rtnList = DBHelper.GetInstance().WareTypeGet();
                foreach (var item in rtnList)
                {
                    TreeListNode jdNode1 = _typeManagerTree.AddNode(jdNode, item.Name, item.TID, null, null);
                    jdNode1.ImageIndex = 1;
                    jdNode1.SelectImageIndex = 0;
                    jdNode1.Tag = item.TID;
                }

                TreeListNode trashNode = _typeManagerTree.AddNode(null, "回收站", "trash", null, null);
                trashNode.ImageIndex = 1;
                trashNode.SelectImageIndex = 0;
                //jdNode = _typeManagerTree.AddNode(null, "淘宝", "TAOBAO", null, null);
                //jdNode.ImageIndex = 1;
                //jdNode.SelectImageIndex = 0;

                _typeManagerTree.AllowDrop = true;
                _typeManagerTree.ContextMenuStrip = treeMenu;
                _typeManagerTree.FocusedNodeChanged += _typeManagerTree_FocusedNodeChanged;
                _typeManagerTree.DragOver += _typeManagerTree_DragOver;
                _typeManagerTree.Dock = DockStyle.Fill;
                splitContainerControl1.Panel1.Controls.Add(_typeManagerTree);
                _typeManagerTree.ExpandAll();
                _typeManagerTree.FocusedNode = allNode;
                _typeManagerTree.EndUpdate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        [DisplayName("移动分类")]
        void _typeManagerTree_DragOver(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == DragDropEffects.Move)
            {
                if (e.Data.GetDataPresent(typeof(System.Collections.Generic.List<Object>)))
                {
                    List<Object> obj = e.Data.GetData(typeof(System.Collections.Generic.List<Object>)) as List<Object>;
                    if (obj != null)
                    {
                        try
                        {
                            ProductInfo curWare = obj[0] as ProductInfo;
                            //string[] dList = e.Data.GetFormats();

                            if (curWare != null)
                            {
                                //获取当前操作节点
                                var hi = _typeManagerTree.CalcHitInfo(_typeManagerTree.PointToClient(new Point(e.X, e.Y)));
                                var targetNode = hi.Node;

                                if (targetNode.Tag != null)
                                {
                                    DBHelper.GetInstance().WareTypeUpdate(curWare.ProductID, targetNode.Tag.ToString());
                                    DBHelper.GetInstance().WareReloadOne(curWare.ProductID);
                                    OtCom.XLogInfo(targetNode.Tag.ToString());
                                }
                                else
                                {
                                    object nowKey = _typeManagerTree.GetNodeKey(targetNode);
                                    if (nowKey.ToString() == "ALL" || nowKey.ToString() == "JINDONG" || nowKey.ToString() == "UnType"
                                    || nowKey.ToString() == "down" || nowKey.ToString() == "up" || nowKey.ToString() == "balance"
                                    || nowKey.ToString() == "focus" )
                                    {
                                        MessageBox.Show("系统基础分类不能增加商品！", "系统提示");
                                        return;
                                    }
                                    else if (nowKey.ToString() == "trash")
                                    {
                                        DBHelper.GetInstance().WareDelOne(curWare.ProductID);
                                        OtCom.XLogInfo("trash");
                                    }

                                }
                                RefreshGrid(btnSearch.Tag.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            OtCom.XLogErr(ex.Message);
                        }
                        
                    }
                }
            }
        }

        private void menuAddType_Click(object sender, EventArgs e)
        {
            AddWareType(false, null);
            InitTypeTree();
        }

        private void AddWareType(bool isEdit, ProductType wType)
        {
            UiTypes types = isEdit ? new UiTypes(wType) : new UiTypes();
            ShowPopForm(types, "分类管理");
        }

        private void menuDelType_Click(object sender, EventArgs e)
        {
            //object nowNod = _typeManagerTree.GetValue(e.Node);
            TreeListNode fNode = _typeManagerTree.FocusedNode;
            if (fNode != null)
            {
                object nowKey = _typeManagerTree.GetNodeKey(fNode);
                if (nowKey.ToString() == "ALL" || nowKey.ToString() == "JINDONG" || nowKey.ToString() == "UnType"
                    || nowKey.ToString() == "down" || nowKey.ToString() == "up" || nowKey.ToString() == "balance"
                    || nowKey.ToString() == "focus" || nowKey.ToString() == "trash")
                {
                    MessageBox.Show("系统基础分类不能删除！", "系统提示");
                }
                else
                {
                    if (MessageBox.Show("删除商品分类将不能恢复，确定继续删除？","系统警告",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
                    {
                        DBHelper.GetInstance().WareTypeDelete(nowKey.ToString());
                        MessageBox.Show("当前分类删除成功！", "系统提示");
                        InitTypeTree();
                    }
                }
            }

        }

        private void menuChangeType_Click(object sender, EventArgs e)
        {
            TreeListNode fNode = _typeManagerTree.FocusedNode;
            if (fNode != null)
            {
                object nowKey = _typeManagerTree.GetNodeKey(fNode);
                if (nowKey.ToString() == "ALL" || nowKey.ToString() == "JINDONG" || nowKey.ToString() == "UnType"
                    || nowKey.ToString() == "down" || nowKey.ToString() == "up" || nowKey.ToString() == "balance"
                    || nowKey.ToString() == "focus" || nowKey.ToString() == "trash")
                {
                    MessageBox.Show("系统基础分类不能修改！", "系统提示");
                }
                else
                {
                    object nowValue = _typeManagerTree.GetNodeCaption(fNode);
                    if (nowValue != null)
                    {
                        ProductType cType = new ProductType() { TID = nowKey.ToString(), Name = nowValue.ToString(), BEnable = true };
                        AddWareType(true, cType);
                        InitTypeTree();
                    }
                }
            }
        }

        private void menuRefreshType_Click(object sender, EventArgs e)
        {
            InitTypeTree();
        }

        private void _typeManagerTree_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (e.Node != null)
            {
                //object nowNod = _typeManagerTree.GetValue(e.Node);
                object nowKey = _typeManagerTree.GetNodeKey(e.Node);
                if (nowKey != null)
                {
                    if (nowKey.ToString() == "ALL" || nowKey.ToString() == "JINDONG")
                    {
                        RefreshGrid();
                    }
                    else
                    {
                        RefreshGrid(nowKey.ToString());
                    }
                    btnSearch.Tag = nowKey.ToString(); 
                    txtWareType.Caption = string.Format("商品分类：{0}", _typeManagerTree.GetNodeCaption(e.Node).ToString());
                }
            }
        }

        #endregion TypeTree Code

        #region InitGridView
        /// <summary>
        /// 初始化Grid
        /// </summary>
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
                col.ColWidth = 36;
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
                col.HeadName = "商品归属";
                col.FieldName = "ProductAttach";
                col.DataType = "System.String";
                col.ColWidth = 25;
                //col.IsShow = false;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "历史底价";
                col.FieldName = "ProductBasePrice";
                col.DataType = "System.Double";
                col.ColWidth = 25;
                //col.IsShow = false;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "来源";
                col.FieldName = "ProductPriceType";
                col.DataType = "System.String";
                col.ColWidth = 10;
                col.IsShow = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "网站价格";
                col.FieldName = "ProductPrice";
                col.DataType = "System.Double";
                col.ColWidth = 25;
                //col.IsShow = false;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "手机价格";
                col.FieldName = "ProductMobilePrice";
                col.DataType = "System.Double";
                col.ColWidth = 25;
                //col.IsShow = false;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "QQ价格";
                col.FieldName = "ProductQQPrice";
                col.DataType = "System.Double";
                col.ColWidth = 25;
                //col.IsShow = false;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "微信价格";
                col.FieldName = "ProductWXPrice";
                col.DataType = "System.Double";
                col.ColWidth = 25;
                //col.IsShow = false;
                col.AllowSize = true;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "价格趋势";
                col.FieldName = "ProductPriceTrend";
                col.DataType = "System.String";
                col.ColWidth = 25;
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
                col.HeadName = "优惠信息";
                col.FieldName = "ProductPromoMsg";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.ColWidth = 120;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "购物券";
                col.FieldName = "ProductCoupon";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.ColWidth = 60;
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
                col.HeadName = "差评率";
                col.FieldName = "ProductPoorRate";
                col.AllowSize = true;
                col.DataType = "System.Double";
                col.IsShow = false;
                col.ColWidth = 20;
                Heads.Add(col);

                col = new DevColHeads();
                col.HeadName = "商品分类";
                col.FieldName = "TypeName";
                col.DataType = "System.String";
                col.AllowSize = true;
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
                col.FieldName = "Catalog";
                col.DataType = "System.String";
                col.AllowSize = true;
                col.IsShow = false;
                Heads.Add(col);

                _productView.ShowMainTool = false;
                _productView.ShowRowHeader = true;
                _productView.ShowColumHeader = true;
                _productView.ShowPageToolBar = true;
                _productView.ShowWaitMsg = true;
                _productView.IsCheck = true;

                _productView.InitGrid(Heads);
                _productView.ContextMenuStrip = contextMenus;
                //整行颜色改变
                //_productView.AddCondition("[ProductIsSaled]=0", Color.IndianRed);
                //_productView.AddCondition("[ProductIsSaled]=-1", Color.LightGray);
                //_productView.AddCondition("[ProductStock]='无货'", GetColor(OtCom.GetConstCfg("Log_NormalColor", "-1")));
                //_productView.AddCondition("[LvlId]=1", GetColor(OtCom.GetConstCfg("Log_TipColor", "-1")));
                //_productView.AddCondition("[LvlId]=2", GetColor(OtCom.GetConstCfg("Log_ErrorColor", "-1")));
                //_productView.AddCondition("[LvlId]=3", GetColor(OtCom.GetConstCfg("Log_WarningColor", "-1")));
                //_productView.AddCondition("[LvlId]=4", GetColor(OtCom.GetConstCfg("Log_DebugColor", "-1")));

                _productView.GridClick += _productView_GridClick;
                _productView.GridDBClick += new OtDevGridView.GridDBClickHandler(_productView_GridDBClick);
                (_productView.GetGrid() as GridView).RowCellStyle += new RowCellStyleEventHandler(_productView_RowCellStyle);
                (_productView.GetGrid() as GridView).FocusedRowChanged += FrmMain_FocusedRowChanged;
                _productView.CanDragRow = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        void FrmMain_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            //_minPrice = string.Empty;
            //ProductInfo _tmpWare = _productView.GetCurData<ProductInfo>();
            //_minPrice = _tmpWare.ProductPriceType;
        }

        string _minPrice = "";
        void _productView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "ProductPriceType" && e.CellValue != null && e.CellValue is string)
            {
                _minPrice = e.CellValue.ToString();
            }
            if (
                (e.Column.FieldName == "ProductPrice" || e.Column.FieldName == "ProductMobilePrice" || e.Column.FieldName == "ProductQQPrice" || e.Column.FieldName == "ProductWXPrice")
                && e.CellValue != null && e.CellValue is double)
            {
                double price = (double)e.CellValue;
                Color _tmpColor = new Color();
                if (!string.IsNullOrEmpty(_minPrice))
                {
                    switch (_minPrice)
                    {
                        case"京东":
                            _tmpColor = e.Column.FieldName == "ProductPrice" ? Color.FromArgb(250, 100, 100) : Color.FromArgb(240, 240, 100);
                            break;
                        case "手机":
                            _tmpColor = e.Column.FieldName == "ProductMobilePrice" ? Color.FromArgb(250, 100, 100) : Color.FromArgb(240, 240, 100);
                            break;
                        case "QQ":
                            _tmpColor = e.Column.FieldName == "ProductQQPrice" ? Color.FromArgb(250, 100, 100) : Color.FromArgb(240, 240, 100);
                            break;
                        case "微信":
                            _tmpColor = e.Column.FieldName == "ProductWXPrice" ? Color.FromArgb(250, 100, 100) : Color.FromArgb(240, 240, 100);
                            break;
                    }
                    e.Appearance.BackColor = _tmpColor;
                }
                else
                {
                    e.Appearance.BackColor = price == 0 ? Color.FromArgb(220, 220, 220) : Color.FromArgb(100, 100, 200);
                }
                
                #region comment
                //switch (price)
                //{
                //    case 0:
                //        {
                //            e.Appearance.BackColor = Color.FromArgb(220, 220, 220);
                //        }
                //        break;
                //    case "启动":
                //        {
                //            e.Appearance.BackColor = Color.FromArgb(100, 100, 220);
                //        } break;
                //    case "处理中":
                //        {
                //            e.Appearance.BackColor = Color.FromArgb(100, 100, 250);
                //        }
                //        break;
                //    case "执行中":
                //        {
                //            e.Appearance.BackColor = Color.FromArgb(100, 100, 250);
                //        }
                //        break;
                //    case "完成":
                //        {
                //            e.Appearance.BackColor = Color.FromArgb(100, 220, 100);
                //        }
                //        break;
                //    case "已完成":
                //        {
                //            e.Appearance.BackColor = Color.FromArgb(100, 250, 100);
                //        }
                //        break;
                //    case "异常":
                //        {
                //            e.Appearance.BackColor = Color.FromArgb(250, 100, 100);
                //        }
                //        break;
                //    case "暂停":
                //        {
                //            e.Appearance.BackColor = Color.FromArgb(240, 240, 100);
                //        }
                //        break;
                //    default:
                //        {
                //            e.Appearance.BackColor = Color.FromArgb(250, 250, 250);
                //        }
                //        break;
                //}
                #endregion comment
                e.Appearance.ForeColor = Color.Black;
                e.Appearance.BorderColor = Color.DarkGray;
            }
            if (e.Column.FieldName == "ProductStock" && e.CellValue != null && e.CellValue is string)
            {
                string stock = e.CellValue as string;
                switch (stock)
                {
                    case "下柜":
                        e.Appearance.BackColor = Color.FromArgb(220, 220, 220);
                        break;
                    case "有货":
                        e.Appearance.BackColor = Color.FromArgb(100, 220, 100);
                        break;
                    case "无货":
                        e.Appearance.BackColor = Color.FromArgb(250, 100, 100);
                        break;
                    default:
                        e.Appearance.BackColor = Color.FromArgb(240, 240, 100);
                        break;
                }
                e.Appearance.ForeColor = Color.Black;
                e.Appearance.BorderColor = Color.DarkGray;
            }
        }

        void _productView_GridClick(DataRow dataRow, object objValue)
        {
            ProductInfo curData = _productView.GetCurData<ProductInfo>();
            if (curData != null)
            {
                _dragObj = curData;
            }
        }

        object _dragObj = null;

        void _productView_GridDBClick(System.Data.DataRow dataRow, object objValue, EventArgs e)
        {
            ProductInfo curData = _productView.GetCurData<ProductInfo>();
            if (curData != null)
            {
                FrmResInfo resInfo = new FrmResInfo(curData);
                resInfo.ShowDialog();
            }
        }
        /// <summary>
        /// 加载Grid数据
        /// </summary>
        private void RefreshGrid()
        {
            List<ProductInfo> rtnList = DBHelper.GetInstance().WareGetAll();
            if (rtnList != null && rtnList.Count > 0)
            {
                PubClass.GetInstance().CurWares = rtnList;
                txtWareCount.Caption = string.Format("商品数量：{0}", rtnList.Count);
                _productView.RefreshGrid(rtnList);
            }
            else
            {
                PubClass.GetInstance().CurWares = null;
                txtWareCount.Caption = string.Format("商品数量：{0}", 0);
            }
        }
        /// <summary>
        /// 加载Grid数据
        /// </summary>
        private void RefreshGrid(string typeKey)
        {
            if (!string.IsNullOrEmpty(typeKey))
            {
                List<ProductInfo> rtnList = DBHelper.GetInstance().WareGetAll(typeKey);
                PubClass.GetInstance().CurWares = rtnList;
                _productView.RefreshGrid(rtnList);

                txtWareCount.Caption = string.Format("商品数量：{0}", rtnList != null ? rtnList.Count : 0);
            }
        }

        /// <summary>
        /// 加载Grid数据
        /// </summary>
        /// <param name="typeKey">商品类型</param>
        /// <param name="wName">商品名称</param>
        /// <param name="wStock">商品库存</param>
        /// <param name="wPriceTrend">商品</param>
        /// <param name="wAttach"></param>
        private void RefreshGrid(string typeKey, string wName, string wStock, string wPriceTrend, string wAttach)
        {
            if (!string.IsNullOrEmpty(typeKey))
            {
                List<ProductInfo> rtnList = DBHelper.GetInstance().WareGetAll(typeKey, wName, wStock, wPriceTrend, wAttach);
                PubClass.GetInstance().CurWares = rtnList;
                _productView.RefreshGrid(rtnList);
            }
        }

        #endregion InitGridView

        private void InitSysparams()
        {
            PubClass.GetInstance().GetSysParams();
            RefreshStatubar();
        }

        private void RefreshStatubar()
        {
            txtUserInfo.Caption = string.Format("当前用户：{0}", string.IsNullOrEmpty(SysParams.UserName) ? "Hank" : SysParams.UserName);
            txtDispatchArea.Caption = string.Format("配送区域：{0}", string.IsNullOrEmpty(SysParams.Level1Area) ? "成都高新区（三环外）" : SysParams.Level1Area + SysParams.Level2Area + SysParams.Level3Area);
        }

        private void btnGetProduct_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmResInfo resInfo = new FrmResInfo();
            resInfo.ShowDialog();
        }

        private void menuGet_Click(object sender, EventArgs e)
        {
            btnGetProduct_ItemClick(null, null);
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
                if (btnSearch.Tag != null && btnSearch.Tag.ToString().ToLower() == "trash")
                {
                    RemoveWare(delObj);
                    RefreshGrid("trash");
                }
                else
                {
                    DBHelper.GetInstance().WareDelOne(delObj.RID);
                    RefreshGrid();
                }
                
            }
        }

        private void menuUrl_Click(object sender, EventArgs e)
        {
            ProductInfo _myProduct = _productView.GetCurData<ProductInfo>();
            if (_myProduct != null)
            {
                //在默认浏览器中打开商品网页
                System.Diagnostics.Process.Start(_myProduct.ProductURL);
            }
        }

        private void btnInport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImportWareURL();
        }

        /// <summary>
        /// 导入商品URL
        /// </summary>
        private void ImportWareURL()
        {
            if (!_isProcess)
            {
                _isProcess = true;
                UiImportWare import = new UiImportWare();
                ShowPopForm(import, "导入商品");
                _isProcess = false;
            }
            else
            {
                MessageBox.Show("后台线程正在执行，请等待线程执行完成后再进行导入操作！");
            }
        }

        private void RemoveWare(ProductInfo item)
        {
            if (MessageBox.Show("回收站商品删除后无法恢复！是否继续删除？", "系统警告", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                DBHelper.GetInstance().WareRemoveOne(item.ProductID);
                try
                {
                    //删除商品图片
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
        }

        /// <summary>
        /// 物理删除
        /// </summary>
        private void RemoveWares()
        {
            List<ProductInfo> dels = _productView.GetAll<ProductInfo>();
            if (dels != null && dels.Count > 0)
            {
                if (MessageBox.Show("商品清空后无法恢复！是否继续删除？", "系统警告", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (var item in dels)
                    {
                        DBHelper.GetInstance().WareRemoveOne(item.ProductID);
                        try
                        {
                            //删除商品图片
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
                    RefreshGrid("trash");
                }
            }
            else
            {
                MessageBox.Show("请选择需要删除的商品！", "系统提示");
            }
        }
        /// <summary>
        /// 删除指定商品 逻辑删除
        /// </summary>
        private void DelWares()
        {
            List<ProductInfo> dels = _productView.GetCheckList<ProductInfo>();
            if (dels != null && dels.Count > 0)
            {
                if (MessageBox.Show("商品删除后可在回收站中恢复！是否继续删除？", "系统警告", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (var item in dels)
                    {
                        DBHelper.GetInstance().WareDelOne(item.ProductID);
                    }
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show("请选择需要删除的商品！", "系统提示");
            }
        }

        #region UpdateWareThread
        Stopwatch _stopwathc  = new Stopwatch();
        /// <summary>
        /// 线程启动状态
        /// </summary>
        private bool _isProcess = false;
        /// <summary>
        /// 更新商品价格、库存数据
        /// </summary>
        private void UpdateWareThread(bool isAll)
        {
            if (!_isProcess)
            {
                JDListener.GetInstance().InitProcess = InitUpdateProcess;
                JDListener.GetInstance().ShowStep = ShowUpdateProcessing;
                JDListener.GetInstance().EndProcess = EndUpdateProcess;
                JDListener.GetInstance().ShowMessage = null;

                if (!isAll)
                {
                    txtUpdater.Caption = "手动更新进度";
                    List<ProductInfo> updates = _productView.GetAll<ProductInfo>();
                    if (updates != null && updates.Count > 0)
                    {
                        JDListener.GetInstance().UpdateWareThread(updates);
                    }
                }
                else
                {
                    txtUpdater.Caption = "自动更新进度";
                    JDListener.GetInstance().UpdateWareThread();
                }
                
            }
        }

        private void InitUpdateProcess(int leng)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(InitUpdateProcess), leng);
            }
            else
            {
                _stopwathc.Reset();
                _stopwathc.Start();
                if (leng > 0)
                {
                    _isProcess = true;
                    barProcess.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    txtUpdater.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    barProcess.EditValue = "0";
                    repositoryItemProgressBar1.Maximum = leng;
                }
            }
        }

        private void ShowUpdateProcessing(int iStep)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(ShowUpdateProcessing), iStep);
            }
            else
            {
                barProcess.Caption = iStep.ToString();
                barProcess.EditValue = iStep;
            }
        }

        private void EndUpdateProcess(bool bEnd)
        {
            _isProcess = !bEnd;
            if (bEnd)
            {
                _stopwathc.Stop();
                txtUpdater.Caption = string.Format("本次操作于{0}结束，共计更新商品{1}个，耗时: {2}分钟（{3} ms）", DateTime.Now, repositoryItemProgressBar1.Maximum, 
                    _stopwathc.ElapsedMilliseconds / 1000 /60, _stopwathc.ElapsedMilliseconds);
                //RefreshGrid();
                InitProcessBar();
            }
        }

        private void InitProcessBar()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    barProcess.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    //txtUpdater.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                });
            }
            else
            {
                barProcess.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                //txtUpdater.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
            
        }

        private void barProcess_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //barProcess.
        }

        #endregion UpdateWareThread

        private void menuInport_Click(object sender, EventArgs e)
        {
            ImportWareURL();
            RefreshGrid();
        }

        private void btnDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DelWares();
        }

        private void btnUpdate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UpdateWareThread(false);
        }

        private void menuUpdate_Click(object sender, EventArgs e)
        {
            ProductInfo update = _productView.GetCurData<ProductInfo>();
            if (update != null)
            {
                WareDealer.WareService.GetInstance().UpdateWareinfo(update);
            }
            
        }

        private void btnMain_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RefreshGrid();
        }

        private void btnAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmAbout uiAbout = new FrmAbout();
            uiAbout.ShowDialog();
        }

        private void btnExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Environment.Exit(0);
            Application.Exit();
        }

        private void btnLines_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ProductInfo myWareInfo = _productView.GetCurData<ProductInfo>();
            if (myWareInfo != null)
            {
                UiLines import = new UiLines(myWareInfo.ProductID, myWareInfo.ProductName);
                ShowPopForm(import, string.Format("商品[{0}-{1}]价格趋势图", myWareInfo.ProductID, myWareInfo.ProductName));
            }
            
        }

        /// <summary>
        /// 显示弹出窗体
        /// </summary>
        /// <param name="ucTmp"></param>
        /// <param name="title"></param>
        private void ShowPopForm(UserControl ucTmp, string title)
        {
            if (ucTmp != null && !ucTmp.IsDisposed)
            {
                ucTmp.Dock = DockStyle.Fill;

                Form frm = new XtraForm();
                frm.Text = title;
                frm.ShowInTaskbar = false;
                frm.MinimizeBox = false;
                frm.MaximizeBox = false;
                frm.Height = ucTmp.Size.Height + 30;
                frm.Width = ucTmp.Size.Width + 20;
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Icon = Properties.Resources.frm;
                frm.Controls.Add(ucTmp);
                frm.ShowDialog();
            }
        }

        private void btnConfig_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UiSysParams uiParams = new UiSysParams();
            ShowPopForm(uiParams, "杀京东参数配置");
            RefreshStatubar();
        }

        private void btnHelp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string path = Path.Combine(Environment.CurrentDirectory, "Config", "HandBook.doc");
                if (File.Exists(path))
                {
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    MessageBox.Show("帮助文件不存在，请与Hanksoft联系！！！QQ群415014949", "系统提示");
                }
            }
            catch (Exception ex)
            {
                OtCom.XLogErr(ex.Message);
            }
        }

        private void btnSearch_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _productView.ShowFilter = btnSearch.Checked;
            //plTools.Visible = btnSearch.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearStatus();
        }

        /// <summary>
        /// 清空查询条件
        /// </summary>
        private void ClearStatus()
        {
            txtName.Text = "";
            chkPriceList.Text = "";
            chkStockList.Text = "";
            chkMatchList.Text = "";
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            string wareName = txtName.Text;
            string price = chkPriceList.SelectedText;
            string price1 = chkPriceList.Text;
            if (!string.IsNullOrEmpty(wareName))
            {
                string wType = btnSearch.Tag.ToString();
                RefreshGrid(wType, wareName, "", "", "");
            }
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)    //最小化到系统托盘
            {
                notifyIcon1.Visible = true;    //显示托盘图标
                this.Hide();    //隐藏窗口
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Show();
            WindowState = FormWindowState.Maximized;
            this.Focus();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Environment.Exit(0);
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                notifyIcon1.Visible = true;
                this.Hide();
            }
            else
            {
                notifyIcon1.Visible = false;
            }
        }

        private void menuGetWebUrl_Click(object sender, EventArgs e)
        {
            menuGetWebUrl.Checked = !menuGetWebUrl.Checked;
            SysParams.AllowAutoGetWebUrl = menuGetWebUrl.Checked;
        }

        private void menuChkAutoUpdate_Click(object sender, EventArgs e)
        {
            menuChkAutoUpdate.Checked = !menuChkAutoUpdate.Checked;
            SysParams.AllowAutoUpdateWareInfo = menuChkAutoUpdate.Checked;
        }

        #region GetNetStatus
        private void timer4Net_Tick(object sender, EventArgs e)
        {
            if (SysParams.CheckNetState)
            {
                Thread netThread = new Thread(GetNetStatus) { Name = "GetNetStatus", IsBackground = true };
                netThread.Start();
            }
        }

        /// <summary>
        /// 获取网络连接状态
        /// </summary>
        private void GetNetStatus()
        {
            string[] urls = new string[] { "www.baidu.com" };
            int iRtn = 0;
            bool IsConnect = NetStatusHelper.NetPing(urls, out iRtn);

            if (IsConnect)
            {
                //BarStaticItem是静态文本？具备线程安全
                txtNetState.Caption = "网络畅通";
                txtNetState.ItemAppearance.Normal.ForeColor = Color.Green;
            }
            else
            {
                txtNetState.Caption = "网络中断";
                txtNetState.ItemAppearance.Normal.ForeColor = Color.Red;
            }
        }
        #endregion GetNetStatus

        private void menuMove_Click(object sender, EventArgs e)
        {
            List<ProductInfo> warelst = _productView.GetCheckList<ProductInfo>();
            if (warelst != null && warelst.Count > 0)
            {
                UiSelectType ucType = new UiSelectType();
                ShowPopForm(ucType, "请选择迁移后分类");
                if (ucType.WareType != null)
                {
                    foreach (var item in warelst)
                    {
                        DBHelper.GetInstance().WareTypeUpdate(item.ProductID, ucType.WareType);
                    }
                    MessageBox.Show("迁移商品完成！", "系统提示");
                }
                if (btnSearch.Tag != null)
                {
                    string ty = btnSearch.Tag.ToString();
                    RefreshGrid(ty);
                }
                else
                {
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show("请选择/勾选需要迁移的商品！", "系统提示");
            }
        }

        private void timer4UpdateWare_Tick(object sender, EventArgs e)
        {
            //自动更新商品信息
            if (SysParams.AllowAutoUpdateWareInfo)
            {
                UpdateWareThread(true);
            }
        }

        private void menuClearTrash_Click(object sender, EventArgs e)
        {
            RemoveWares();
        }

        private void timer4Focus_Tick(object sender, EventArgs e)
        {
            //自动获取关注数据
            if (SysParams.AllowAutoGetFocusWare)
            {
                GetFoucsWare();
            }
        }

        /// <summary>
        /// 获取指定用户关注商品
        /// </summary>
        private void GetFoucsWare()
        {
            if (!_isProcess)
            {
                _isProcess = true;
                txtUpdater.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                JDListener.GetInstance().InitProcess = null;
                JDListener.GetInstance().ShowStep = null;
                JDListener.GetInstance().EndProcess = null;
                JDListener.GetInstance().ShowMessage = ShowMessageInStatus;

                txtUpdater.Caption = "获取关注商品";
                JDListener.GetInstance().GetFocusThread();
                _isProcess = false;
                //txtUpdater.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
        }

        private void ShowMessageInStatus(string msg)
        {
            txtUpdater.Caption = msg;
        }

        private void btnGetFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetFoucsWare();
        }

        private void btnLog_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
        }

        private void GetJDWareTypesThread()
        {
            if (!_isProcess)
            {
                _isProcess = true;
                txtUpdater.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                JDListener.GetInstance().InitProcess = null;
                JDListener.GetInstance().ShowStep = null;
                JDListener.GetInstance().EndProcess = null;
                JDListener.GetInstance().ShowMessage = ShowMessageInStatus;

                txtUpdater.Caption = "获取京东商品分类数据";
                JDListener.GetInstance().GetJDWareTypesThread();
                _isProcess = false;
                //txtUpdater.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
        }

        private void btnGetJDType_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GetJDWareTypesThread();
        }

        private void btnFindJDWare_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UiFindJDWare findFrm = new UiFindJDWare();
            ShowPopForm(findFrm, "导入查询商品");
        }
       
    }
}