using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Threading;
using WareDealer;
using System.IO;
using System.Text.RegularExpressions;
using WareDealer.Mode;
using DevExpress.XtraEditors.Controls;
using Hank.BrowserParse;
using System.Diagnostics;
using WareDealer.Helper;

namespace KillPrice
{
    public partial class UiImportWare : DevExpress.XtraEditors.XtraUserControl
    {
        #region field
        /// <summary>
        /// 是否正在导入 导入时不允许退出当前窗体
        /// </summary>
        bool _isImport = false;
        /// <summary>
        /// 线程状态控制 根据该值判断线程是否停止
        /// </summary>
        bool _isProcessing = false;
        /// <summary>
        /// 操作类型 0、文本 1、IE 2、FireFox 3、JDWatch
        /// </summary>
        WareImportType _typeIndex = WareImportType.Text;
        #endregion field

        public UiImportWare()
        {
            InitializeComponent();
        }

        private void UiImportWare_Load(object sender, EventArgs e)
        {
            memoImport.Text = "";
            memoImport.Visible = false;
        }

        private void wizardControl1_CancelClick(object sender, CancelEventArgs e)
        {
            if (_isImport)
            {
                MessageBox.Show("正在导入商品，不能退出！", "系统提示");
                e.Cancel = _isImport;
            }
        }

        private void wizardControl1_NextClick(object sender, DevExpress.XtraWizard.WizardCommandButtonClickEventArgs e)
        {
            if (wizardControl1.SelectedPage == SourcePage)
            {
                SetPagesState();
            }
            if (wizardControl1.SelectedPage == ChoseTypePage)
            {
                TypeItemPage.AllowNext = false;
            }
        }

        /// <summary>
        /// 设置各操作页状态
        /// </summary>
        private void SetPagesState()
        {
            switch (groupSource.SelectedIndex)
            {
                case 0:
                    TextPage.Visible = true;
                    IEPage.Visible = false;
                    FirePage.Visible = false;
                    WatchPage.Visible = false;
                    TextPage.AllowNext = false;
                    ChoseTypePage.Visible = false;
                    TypeItemPage.Visible = false;
                    importPage.Visible = true;
                    _typeIndex = WareImportType.Text;
                    break;
                case 1:
                    TextPage.Visible = false;
                    IEPage.Visible = true;
                    FirePage.Visible = false;
                    WatchPage.Visible = false;
                    IEPage.AllowNext = false;
                    ChoseTypePage.Visible = false;
                    TypeItemPage.Visible = false;
                    importPage.Visible = true;
                    _typeIndex = WareImportType.InternetExplore;
                    GetIEWebsite();
                    break;
                case 2:
                    TextPage.Visible = false;
                    IEPage.Visible = false;
                    FirePage.Visible = true;
                    WatchPage.Visible = false;
                    FirePage.AllowNext = false;
                    ChoseTypePage.Visible = false;
                    TypeItemPage.Visible = false;
                    importPage.Visible = true;
                    _typeIndex = WareImportType.FireFox;
                    break;
                case 6:
                    TextPage.Visible = false;
                    IEPage.Visible = false;
                    FirePage.Visible = false;
                    WatchPage.Visible = false;
                    ChoseTypePage.AllowNext = false;
                    ChoseTypePage.Visible = true;
                    TypeItemPage.Visible = true;
                    importPage.Visible = false;
                    _typeIndex = WareImportType.JDType;
                    GetJDTypes();
                    GetMyTypes();
                    break;
                default:
                    TextPage.Visible = false;
                    IEPage.Visible = false;
                    FirePage.Visible = false;
                    WatchPage.Visible = true;
                    WatchPage.AllowNext = false;
                    txtUser.Text = SysParams.JDUserName;
                    txtPass.Text = SysParams.JDUserPass;
                    txtPass.Properties.PasswordChar = '*';
                    ChoseTypePage.Visible = false;
                    TypeItemPage.Visible = false;
                    importPage.Visible = true;
                    _typeIndex = WareImportType.JDWatch;
                    break;
            }
        }

        private void wizardControl1_SelectedPageChanged(object sender, DevExpress.XtraWizard.WizardPageChangedEventArgs e)
        {
            if (wizardControl1.SelectedPage == importPage)
            {
                wartImportProcess();
            }
            if (wizardControl1.SelectedPage == TypeItemPage)
            {
                //开始获取并解析京东商品分类
                GetJDTypesData();
            }
        }

        #region TextPage

        private void btnOpenFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            openFileDialog1.Multiselect = false;
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                btnOpenFile.Text = fileName;
                Read(fileName);
                if (chkWarelist.CheckedItemsCount > 0)
                {
                    TextPage.AllowNext = true;
                }
            }
        }

        /// <summary>
        /// 读取导入列表文件
        /// </summary>
        /// <param name="path"></param>
        private void Read(string path)
        {
            chkWarelist.Items.Clear();
            StreamReader sr = new StreamReader(path, Encoding.Default);
            
            if (sr.EndOfStream)
            {
                //重置文件指针至文件头
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
            }
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                if (Regex.Match(line, "http://item.jd.com/(\\d{1,14}).html", RegexOptions.IgnoreCase).Success)
                {
                    chkWarelist.Items.Add(line, true);
                }
            }
        }

        private void TextPagesProcess()
        {
            if (chkWarelist != null && chkWarelist.CheckedItemsCount > 0)
            {
                ImportThreads.WareLength = chkWarelist.CheckedItemsCount;
                List<WebSiteModel> sites = new List<WebSiteModel>();
                foreach (CheckedListBoxItem item in chkWarelist.CheckedItems)
                {
                    sites.Add(new WebSiteModel() { url = item.Value.ToString() });
                }

                ImportProcess4AllTypes(sites);
            }
        }

        #endregion TextPage

        #region IEPage

        CheckedListBoxControl _ieChkListBox;
        private void GetIEWebsite()
        {
            IEHelper ieWeb = new IEHelper();
            List<WebSiteModel> iesites = ieWeb.MonitorIE();
            if (iesites != null && iesites.Count > 0)
            {
                if (_ieChkListBox == null)
                {
                    _ieChkListBox = new CheckedListBoxControl();
                    grpIE.Controls.Add(_ieChkListBox);
                    _ieChkListBox.Dock = DockStyle.Fill;
                }
                
                foreach (var item in iesites)
                {
                    if (Regex.Match(item.url, "http://item.jd.com/(\\d{1,14}).html", RegexOptions.IgnoreCase).Success)
                    {
                        if (_ieChkListBox.Items.IndexOf(item.url.TrimEnd()) >= 0)
                        {
                            continue;
                        }
                        _ieChkListBox.Items.Add(item.url.TrimEnd(), true);
                    }
                }
                if (_ieChkListBox.CheckedItemsCount > 0)
                {
                    IEPage.AllowNext = true;
                }
            }
        }
        private void IEPageProcess()
        {
            if (_ieChkListBox != null && _ieChkListBox.CheckedItemsCount > 0)
            {
                ImportThreads.WareLength = _ieChkListBox.CheckedItemsCount;
                List<WebSiteModel> sites = new List<WebSiteModel>();
                foreach (CheckedListBoxItem item in _ieChkListBox.CheckedItems)
                {
                    sites.Add(new WebSiteModel() { url = item.Value.ToString() });
                }

                ImportProcess4AllTypes(sites);
            }
        }

        #endregion IEPage

        #region FirePage

        #endregion FirePage

        #region WatchPage
        private void btnGetWare_Click(object sender, EventArgs e)
        {
            GetJDWatchWare();
        }

        /// <summary>
        /// 更新多选列表数据
        /// </summary>
        /// <param name="websites"></param>
        private void UpdateWareChkBox(List<WebSiteModel> websites)
        {
            if (_jdChkListBox.InvokeRequired)
            {
                _jdChkListBox.Invoke(new Action<List<WebSiteModel>>(UpdateWareChkBox), websites);
            }
            else
            {
                _jdChkListBox.Visible = true;
                //memoMsg.Visible = false;
                _jdChkListBox.BeginUpdate();
                foreach (var item in websites)
                {
                    if (Regex.Match(item.url, "http://item.jd.com/(\\d{1,14}).html", RegexOptions.IgnoreCase).Success)
                    {
                        _jdChkListBox.Items.Add(item.url, true);
                    }
                }
                _jdChkListBox.EndUpdate();
                if (_jdChkListBox.CheckedItemsCount > 0)
                {
                    WatchPage.AllowNext = true;
                    //_jdChkListBox.Items.Count
                    grpWatch.Text = string.Format("商品列表({0})", _jdChkListBox.Items.Count);
                    //grpWatch.Text = "商品列表(" + _jdChkListBox.CheckedItemsCount.ToString() + ")";
                }
                
            }

        }

        /// <summary>
        /// 获取关注商品线程
        /// </summary>
        private void GetWatchWares()
        {
            try
            {
                //优化
                //在关注数据很多时，会造成页面卡死现象；使用多线程+进度条方案进行优化
                List<WebSiteModel> iesites = JDKiller.GetInstance().GetWatchList();
                if (iesites != null && iesites.Count > 0)
                {
                    
                    UpdateWareChkBox(iesites);
                }
                else
                {
                    MessageBox.Show("未获取到当前用户关注商品！" + ImportThreads.LastMsg, "系统提示");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 重载验证码图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picAuthcode_DoubleClick(object sender, EventArgs e)
        {
            JDKiller.GetInstance().ReGetAuthCode();
            ShwoAuthCodeBox(JDKiller.GetInstance().ImageAuthCode);
        }

        /// <summary>
        /// 登录京东并获取关注数据
        /// </summary>
        /// <param name="authcode"></param>
        private void LoginAndGetWatch(string authcode)
        {
            //注册委托时间
            JDKiller.GetInstance().ShowMessage = new Action<string>(ShowMessage);
            JDKiller.GetInstance().InitProcess = new Action<int>(InitProcess2);
            JDKiller.GetInstance().ShowStep = new Action<int>(ShowGetStep);
            JDKiller.GetInstance().EndProcess = new Action<bool>(EndGetProcess);

            if (JDKiller.GetInstance().Login4JD(authcode))
            {
                //InitProcess2();
                Thread watchThread = new Thread(GetWatchWares) { Name = "watchThread", IsBackground = true };
                watchThread.Start();
            }
            else
            {
                MessageBox.Show("登录失败！" + ImportThreads.LastMsg);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAuthcode.Text))
            {
                LoginAndGetWatch(txtAuthcode.Text);
            }
            grpAuthCode.Hide();
        }

        CheckedListBoxControl _jdChkListBox;
        /// <summary>
        /// 显示验证码
        /// </summary>
        /// <param name="authcodeimg"></param>
        private void ShwoAuthCodeBox(Image authcodeimg)
        {
            try
            {
                if (authcodeimg != null)
                {
                    picAuthcode.Image = authcodeimg;
                    grpAuthCode.Show();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 获取京东关注商品
        /// </summary>
        private void GetJDWatchWare()
        {
            string uName = txtUser.Text;
            string uPass = txtPass.Text;
            if (!string.IsNullOrEmpty(uName) && !string.IsNullOrEmpty(uPass))
            {
                JDKiller.GetInstance().InitLogin(uName, uPass);
                //判断是否需要验证码
                if (JDKiller.GetInstance().IsAuthcode)
                {
                    ShwoAuthCodeBox(JDKiller.GetInstance().ImageAuthCode);
                    return;
                }
                LoginAndGetWatch("");
            }
            else
            {
                MessageBox.Show("请检查用户名或密码是否正常输入！","系统提示");
            }
        }

        #region 委托方法定义
        /// <summary>
        /// 初始化关注导入进度条
        /// </summary>
        /// <param name="iMax"></param>
        private void InitProcess2(int iMax)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(InitProcess2), iMax);
            }
            else
            {
                //设置一个最小值
                progressBarControl2.Properties.Minimum = 0;
                //设置一个最大值
                progressBarControl2.Properties.Maximum = iMax;
                //设置步长，即每次增加的数
                progressBarControl2.Properties.Step = 1;
                //当前位置
                progressBarControl2.Position = 0;
                //设置进度条的样式
                progressBarControl2.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
                progressBarControl2.Visible = true;
                progressBarControl2.BringToFront();

                memoGetMsg.Text = "";
                memoGetMsg.Visible = true;
                memoGetMsg.Dock = DockStyle.Fill;
                if (_jdChkListBox == null)
                {
                    _jdChkListBox = new CheckedListBoxControl();
                    grpWatch.Controls.Add(_jdChkListBox);
                    _jdChkListBox.Dock = DockStyle.Fill;
                    _jdChkListBox.Visible = false;
                }

                grpWatch.Text = "商品列表";
                _isProcessing = true;
            }
        }

        /// <summary>
        /// 显示进度
        /// </summary>
        /// <param name="iStep">当前进度位置</param>
        private void ShowGetStep(int iStep)
        {
            
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(ShowGetStep), iStep);
            }
            else
            {
                //主要用于对进度条进行进度控制
                if (_isProcessing)
                {
                    if (iStep < progressBarControl2.Properties.Maximum)
                    {
                        //处理当前消息队列中的所有windows消息
                        Application.DoEvents();
                        //执行步长
                        progressBarControl2.PerformStep();
                        progressBarControl2.Update();
                    }
                }
            }
        }

        /// <summary>
        /// 控制获取关注消息框输出
        /// </summary>
        /// <param name="msg">输出消息</param>
        private void ShowMessage(string msg)
        {
            if (memoGetMsg.InvokeRequired)
            {
                Action<string> ac = new Action<string>(ShowMessage);
                memoGetMsg.Invoke(ac, msg);
            }
            else
            {
                memoGetMsg.Text = memoGetMsg.Text + (DateTime.Now.ToString() + " " + msg + "\r\n");
                memoGetMsg.SelectionStart = memoGetMsg.Text.Length;
                memoGetMsg.ScrollToCaret();
            }
        }

        /// <summary>
        /// 结束进度显示
        /// </summary>
        /// <param name="bRtn"></param>
        private void EndGetProcess(bool bRtn)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(EndGetProcess), bRtn);
            }
            else
            {
                progressBarControl2.Visible = !bRtn;
                memoGetMsg.Visible = !bRtn;
                WatchPage.AllowNext = bRtn;
            }
        }

        #endregion 委托方法定义

        private void WatchPageProcess()
        {
            if (_jdChkListBox != null && _jdChkListBox.CheckedItemsCount > 0)
            {
                ImportThreads.WareLength = _jdChkListBox.CheckedItemsCount;
                List<WebSiteModel> sites = new List<WebSiteModel>();
                foreach (CheckedListBoxItem item in _jdChkListBox.CheckedItems)
                {
                    sites.Add(new WebSiteModel() { url = item.Value.ToString() });
                }
                
                ImportProcess4AllTypes(sites);
            }
        }
        #endregion WatchPage

        #region ChoseTypePage

        private void cmbMainType_EditValueChanged(object sender, EventArgs e)
        {
            cmbSubType.Properties.DataSource = null;
            if (cmbMainType.EditValue != null)
            {
                string wherestr = string.Format("and TypeLevel = 1 and TopID = {0}", cmbMainType.EditValue);
                List<JDWareType> wareTypes = DBHelper.GetInstance().GetWareJDTypes(wherestr);
                if (wareTypes != null && wareTypes.Count > 0)
                {
                    cmbSubType.Properties.DataSource = wareTypes;
                    cmbSubType.Properties.DisplayMember = "TypeName";
                    cmbSubType.Properties.ValueMember = "TypeID";
                    cmbSubType.ItemIndex = 0;
                }
                ChoseTypePage.AllowNext = true;
            }
            
        }

        private void cmbSubType_EditValueChanged(object sender, EventArgs e)
        {
            cmbLastType.Properties.DataSource = null;
            if (cmbSubType.EditValue != null)
            {
                string wherestr = string.Format("and TypeLevel = 2 and TopID = {0}", cmbSubType.EditValue);
                List<JDWareType> wareTypes = DBHelper.GetInstance().GetWareJDTypes(wherestr);
                if (wareTypes != null && wareTypes.Count > 0)
                {
                    cmbLastType.Properties.DataSource = wareTypes;
                    cmbLastType.Properties.DisplayMember = "TypeName";
                    cmbLastType.Properties.ValueMember = "TypeID";
                    cmbLastType.ItemIndex = 0;
                }
                ChoseTypePage.AllowNext = true;
            }

        }

        /// <summary>
        /// 填充京东商品类别数据
        /// </summary>
        private void GetJDTypes()
        {
            List<JDWareType> wareTypes = DBHelper.GetInstance().GetWareJDTypes("and TypeLevel = 0");
            if (wareTypes != null && wareTypes.Count > 0)
            {
                cmbMainType.Properties.DataSource = wareTypes;
                cmbMainType.Properties.DisplayMember = "TypeName";
                cmbMainType.Properties.ValueMember = "TypeID";
                cmbMainType.ItemIndex = 1;
            }
        }

        /// <summary>
        /// 填充杀京东分类数据
        /// </summary>
        private void GetMyTypes()
        {
            List<ProductType> wareTypes = DBHelper.GetInstance().WareTypeGet();
            if (wareTypes != null && wareTypes.Count > 0)
            {
                cmbTypes.Properties.DataSource = wareTypes;
                cmbTypes.Properties.DisplayMember = "Name";
                cmbTypes.Properties.ValueMember = "TID";
                cmbTypes.ItemIndex = 0;
            }
        }

        CheckedListBoxControl _typesChkListBox;

        /// <summary>
        /// 获取京东首页所有商品分类数据
        /// </summary>
        private void GetJDTypesData()
        {
            //progressTypes
            
            //if (_typesChkListBox == null)
            //{
            //    _typesChkListBox = new CheckedListBoxControl()
            //    {
            //        Dock = DockStyle.Fill,
            //        Visible = true
            //    };

            //}
            //TypeItemPage.Controls.Add(_typesChkListBox);
            memoTypeMsg.Text = "";

            Thread wareTypeThread = new Thread(delegate() { JDTypesProcess(); }) { Name = "wareTypeThread", IsBackground = true };
            wareTypeThread.Start();
        }

        private void JDTypesProcess()
        {
            //http://list.jd.com/list.html?cat=9987,653,655
            string wareCat = string.Format("{0},{1},{2}", cmbMainType.EditValue, cmbSubType.EditValue, cmbLastType.EditValue);
            string kType = cmbTypes.EditValue.ToString();

            JDKiller.GetInstance().InitProcess = InitTypesProgress;
            JDKiller.GetInstance().ShowStep = ShowTypesStep;
            JDKiller.GetInstance().ShowMessage = ShowTypeMessage;
            JDKiller.GetInstance().EndProcess = EndTypesProgress;

            JDKiller.GetInstance().GetWareTypeData(wareCat, kType);
        }

        private void ShowTypeMessage(string msg)
        {
            if (memoTypeMsg.InvokeRequired)
            {
                Action<string> ac = new Action<string>(ShowTypeMessage);
                memoTypeMsg.Invoke(ac, msg);
            }
            else
            {
                memoTypeMsg.Text = memoTypeMsg.Text + (DateTime.Now.ToString() + " " + msg + "\r\n");
                memoTypeMsg.SelectionStart = memoTypeMsg.Text.Length;
                memoTypeMsg.ScrollToCaret();
                //memoTypeMsg.Update();
            }
        }

        private void ShowTypesStep(int iStep)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(ShowTypesStep), iStep);
            }
            else
            {
                //主要用于对进度条进行进度控制
                if (_isProcessing)
                {
                    if (iStep < progressTypes.Properties.Maximum)
                    {
                        //处理当前消息队列中的所有windows消息
                        Application.DoEvents();
                        //执行步长
                        progressTypes.PerformStep();
                        progressTypes.Update();
                    }
                }
            }
        }

        private void EndTypesProgress(bool bRtn)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(EndTypesProgress), bRtn);
            }
            else
            {
                progressTypes.Visible = !bRtn;
                TypeItemPage.AllowNext = bRtn;
            }
        }

        private void InitTypesProgress(int iMax)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(InitTypesProgress), iMax);
            }
            else
            {
                //设置一个最小值
                progressTypes.Properties.Minimum = 0;
                //设置一个最大值
                progressTypes.Properties.Maximum = iMax;
                //设置步长，即每次增加的数
                progressTypes.Properties.Step = 1;
                //当前位置
                progressTypes.Position = 0;
                //设置进度条的样式
                progressTypes.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;

                _isProcessing = true;
            }
            
        }

        #endregion ChoseTypePage

        #region wartImportProcess
        /// <summary>
        /// 商品导入主线程
        /// </summary>
        private void wartImportProcess()
        {
            
            switch (_typeIndex)
            {
                case WareImportType.Text:
                    TextPagesProcess();
                    break;
                case WareImportType.InternetExplore:
                    IEPageProcess();
                    break;
                case WareImportType.FireFox:
                    break;
                case WareImportType.JDWatch:
                    WatchPageProcess();
                    break;
                //case WareImportType.JDType:
                //    JDTypesProcess();
                //    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 初始化进度条
        /// </summary>
        private void InitProcess(int iMax)
        {
            //设置一个最小值
            progressBarControl1.Properties.Minimum = 0;
            //设置一个最大值
            progressBarControl1.Properties.Maximum = iMax * 2;
            //设置步长，即每次增加的数
            progressBarControl1.Properties.Step = 1;
            //当前位置
            progressBarControl1.Position = 0;
            //设置进度条的样式
            progressBarControl1.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;

            memoImport.Visible = true;

            _isProcessing = true;
        }

        private void ShowImportStep(int iStep)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(ShowImportStep), iStep);
            }
            else
            {
                //主要用于对进度条进行进度控制
                if (_isProcessing)
                {
                    if (iStep < progressBarControl1.Properties.Maximum)
                    {
                        //处理当前消息队列中的所有windows消息
                        Application.DoEvents();
                        progressBarControl1.Text = iStep.ToString();
                        progressBarControl1.EditValue = iStep;
                        //执行步长
                        //progressBarControl1.PerformStep();
                        progressBarControl1.Update();
                    }
                }
            }
        }

        private void ShowImportMsg(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(ShowImportMsg), msg);
            }
            else
            {
                memoImport.Text = memoImport.Text + (DateTime.Now.ToString() + " " + msg + "\r\n");
            }
        }

        private void EndImportProcess(bool bRtn)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(EndImportProcess), bRtn);
            }
            else
            {
                _isImport = !bRtn;
                importPage.AllowNext = bRtn;
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //主要用于对进度条进行进度控制
            //if (ImportThreads.WareLength > 1 && !_isProcessing)
            //{
            //    progressBarControl1.Properties.Maximum = ImportThreads.WareLength * 2;
            //    progressBarControl1.Position = 0;
            //    _isProcessing = true;
            //}

            ////TO-DO:判断有问题
            //if (_isProcessing)
            //{
            //    if (ImportThreads.WareStep < progressBarControl1.Properties.Maximum)
            //    {
            //        //处理当前消息队列中的所有windows消息
            //        Application.DoEvents();
            //        //执行步长
            //        progressBarControl1.PerformStep();
            //        progressBarControl1.Update();
            //        Debug.WriteLine(ImportThreads.LastMsg + ImportThreads.WareID);
            //    }
            //}
            //if (ImportThreads.WareEnd)
            //{
            //    while ((int)progressBarControl1.EditValue < progressBarControl1.Properties.Maximum)
            //    {
            //        progressBarControl1.PerformStep();
            //        progressBarControl1.Update();
            //    }
            //}

            //_isInport = !ImportThreads.WareEnd;
            //timer1.Enabled = !ImportThreads.WareEnd;
            //importPage.AllowNext = ImportThreads.WareEnd;
        }

        /// <summary>
        /// 获取列表中商品数据并入库
        /// </summary>
        /// <param name="sites"></param>
        private void ImportProcess4AllTypes(List<WebSiteModel> sites)
        {
            if (sites != null && sites.Count > 0)
            {
                InitProcess(sites.Count);
                WareImport.GetInstance().ShowMessage = ShowImportMsg;
                WareImport.GetInstance().ShowStep = ShowImportStep;
                WareImport.GetInstance().EndProcess = EndImportProcess;

                Thread exportThread = new Thread(delegate() { WareImport.GetInstance().ImportWareList(sites); }) { Name = "exportThread", IsBackground = true };
                exportThread.Start();

                _isImport = true;
            }
        }

        
        #endregion wartImportProcess
        
    }
}
