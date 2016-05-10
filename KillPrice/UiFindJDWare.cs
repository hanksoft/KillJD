using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WareDealer;
using Hank.BrowserParse;
using DevExpress.XtraEditors.Controls;
using System.Threading;

namespace KillPrice
{
    public partial class UiFindJDWare : DevExpress.XtraEditors.XtraUserControl
    {
        public UiFindJDWare()
        {
            InitializeComponent();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            SearchEvent();
        }

        private void SearchEvent()
        {
            if (_chkList != null)
            {
                _chkList.Dock = DockStyle.Fill;
                _chkList.Items.Clear();
                _chkList.Visible = true;
                progressImport.Visible = false;
                txtImportMessage.Visible = false;
            }
            progressSearch.Show();
            Thread searchThread = new Thread(delegate() { GetSearchResultThread(); }) { Name = "searchThread", IsBackground = true };
            searchThread.Start();
        }

        private void GetSearchResultThread()
        {
            if (string.IsNullOrEmpty(txtSearchCondition.Text))
            {
                return;
            }
            List<WebSiteModel> sites = WareService.GetInstance().GetSearchList(txtSearchCondition.Text);
            InvokeControls(sites);
        }

        private void InvokeControls(List<WebSiteModel> sites)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<List<WebSiteModel>>(InvokeControls), sites);
            }
            else
            {
                if (sites != null)
                {
                    foreach (var item in sites)
                    {
                        _chkList.Items.Add(item.url, item.title, CheckState.Checked, true);
                    }
                    grpSearchResult.Text = string.Format("搜索结果({0})", sites.Count);
                    btnImport.Enabled = true;

                }
                else
                {
                    grpSearchResult.Text = "搜索结果";
                    btnImport.Enabled = false;
                }
                progressSearch.Hide();
            }
        }

        DevExpress.XtraEditors.CheckedListBoxControl _chkList;

        private void InitCheckListBox()
        {
            _chkList = new DevExpress.XtraEditors.CheckedListBoxControl();
            _chkList.Dock = DockStyle.Fill;
            grpSearchResult.Controls.Add(_chkList);
        }

        private void UiFindJDWare_Load(object sender, EventArgs e)
        {
            InitCheckListBox();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            List<WebSiteModel> sites = new List<WebSiteModel>();
            foreach (CheckedListBoxItem item in _chkList.CheckedItems)
            {
                sites.Add(new WebSiteModel() { url = "http:"+item.Value.ToString(),title = item.Description });
            }
            if (sites != null && sites.Count > 0)
            {
                InitImportProcess(sites.Count);
                WareImport.GetInstance().ShowStep = ShowImportStep;
                WareImport.GetInstance().ShowMessage = ShowImportMsg;
                WareImport.GetInstance().EndProcess = EndImportProcess;

                Thread importThread = new Thread(delegate(){CheckedWareImportThread(sites);}){Name = "searchWareImport", IsBackground = true};
                importThread.Start();
            }
        }

        #region ImportProcess
        private bool _isImport = false;
        private bool _isProcessing = false;

        private void InitImportProcess(int iMax)
        {
            _chkList.Dock = DockStyle.None;
            _chkList.Visible = false;
            //设置一个最小值
            progressImport.Properties.Minimum = 0;
            //设置一个最大值
            progressImport.Properties.Maximum = iMax * 2;
            //设置步长，即每次增加的数
            progressImport.Properties.Step = 1;
            //当前位置
            progressImport.Position = 0;
            //设置进度条的样式
            progressImport.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
            progressImport.Visible = true;
            txtImportMessage.Visible = true;
            txtImportMessage.Text = "";
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
                    if (iStep < progressImport.Properties.Maximum)
                    {
                        //处理当前消息队列中的所有windows消息
                        Application.DoEvents();
                        progressImport.Text = iStep.ToString();
                        progressImport.EditValue = iStep;
                        //执行步长
                        //progressBarControl1.PerformStep();
                        progressImport.Update();
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
                txtImportMessage.Text = txtImportMessage.Text + (DateTime.Now.ToString() + " " + msg + "\r\n");
                txtImportMessage.SelectionStart = txtImportMessage.Text.Length;
                txtImportMessage.ScrollToCaret();
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
            }

        }
        #endregion ImportProcess

        private void CheckedWareImportThread(List<WebSiteModel> sites)
        {
            _isImport = true;
            _isProcessing = true;
            WareImport.GetInstance().ImportWareList(sites);
        }

        private void txtSearchCondition_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                
            }
        }

        private void txtSearchCondition_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchEvent();
            }
        }
    }
}
