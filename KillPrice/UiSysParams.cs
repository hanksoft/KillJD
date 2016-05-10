using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WareDealer.Mode;
using WareDealer.Helper;

namespace KillPrice
{
    public partial class UiSysParams : DevExpress.XtraEditors.XtraUserControl
    {
        List<TabSysParams> _myParams = new List<TabSysParams>();

        public UiSysParams()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //this.FindForm().Close();
        }

        private void UiSysParams_Load(object sender, EventArgs e)
        {
            ShowParams();
            InitCombox();
            InitUiData();

            this.FindForm().CancelButton = btnCancel;
        }

        /// <summary>
        /// 初始化界面元素值
        /// </summary>
        private void InitUiData()
        {
            txtUserName.Text = SysParams.UserName;
            txtUserPass.Text = SysParams.UserPass;
            txtJDUserName.Text = SysParams.JDUserName;
            txtJDUserPass.Text = SysParams.JDUserPass;

            chkAllowAutoGetWebUrl.Checked = SysParams.AllowAutoGetWebUrl;
            chkAllowAutoUpdateVersion.Checked = SysParams.AllowAutoUpdateVersion;
            
            chkAllowLoginVerify.Checked = SysParams.AllowLoginVerify;
            chkCheckNetState.Checked = SysParams.CheckNetState;
            chkAllowRunAtSystemStart.Checked = SysParams.AllowRunAtSystemStart;
            chkAllowAutoGetFocusWare.Checked = SysParams.AllowAutoGetFocusWare;

            chkAllowAutoUpdateWareinfo.Checked = SysParams.AllowAutoUpdateWareInfo;
            chkAllowGetPrice.Checked = SysParams.AllowGetPrice;
            chkAllowGetAPPPrice.Checked = SysParams.AllowGetMobilePrice;
            chkAllowGetQQPrice.Checked = SysParams.AllowGetQQPrice;
            chkAllowGetWXPrice.Checked = SysParams.AllowGetWXPrice;
            chkAllowGetStock.Checked = SysParams.AllowGetStock;
            chkAllowGetCoupon.Checked = SysParams.AllowGetCoupon;
            chkAllowGetPostMessage.Checked = SysParams.AllowGetPostMessage;
            chkAllowGetPromo.Checked = SysParams.AllowGetPromo;

            chkWareBaseInfo.Checked = SysParams.GetWareBaseInfo;
            chkWarePrice.Checked = SysParams.GetWarePrice;
            chkWareStock.Checked = SysParams.GetWareStock;
            chkWareCopon.Checked = SysParams.GetWareCoupon;
            chkWarePicture.Checked = SysParams.GetWarePicture;
            chkWarePostMessage.Checked = SysParams.GetWarePostMessage;
            chkGatherJD.Checked = SysParams.GatherJDWare;
            chkGatherQJ.Checked = SysParams.GatherQJWare;
            chkGather3.Checked = SysParams.Gather3Ware;

            switch (SysParams.AutoUpdateTime)
            {
                case "840":
                    cmbUpdateTime.Text = "14分钟";
                    break;
                case "300":
                    cmbUpdateTime.Text = "5分钟";
                    break;
                case "600":
                    cmbUpdateTime.Text = "10分钟";
                    break;
                case "1800":
                    cmbUpdateTime.Text = "30分钟";
                    break;
                case "3600":
                    cmbUpdateTime.Text = "60分钟";
                    break;
                case "10800":
                    cmbUpdateTime.Text = "3小时";
                    break;
                case "86400":
                    cmbUpdateTime.Text = "每天";
                    break;
                default:
                    cmbUpdateTime.Text = "30分钟";
                    break;
            }
            //cmbUpdateTime.Text = SysParams.AutoUpdateTime;
            switch (SysParams.AutoGetFocusTime)
            {
                case "840":
                    cmbFocusTime.Text = "14分钟";
                    break;
                case "300":
                    cmbFocusTime.Text = "5分钟";
                    break;
                case "600":
                    cmbFocusTime.Text = "10分钟";
                    break;
                case "1800":
                    cmbFocusTime.Text = "30分钟";
                    break;
                case "3600":
                    cmbFocusTime.Text = "60分钟";
                    break;
                case "14400":
                    cmbFocusTime.Text = "4小时";
                    break;
                case "86400":
                    cmbFocusTime.Text = "每天";
                    break;
                default:
                    cmbFocusTime.Text = "30分钟";
                    break;
            }
            //cmbFocusTime.Text = SysParams.AutoGetFocusTime;

            cmb1.Text = SysParams.Level1Area;
            cmb2.Text = SysParams.Level2Area;
            cmb3.Text = SysParams.Level3Area;
        }

        private void InitCombox()
        {
            //cmb1
            List<DispatchArea> areas1 = DBHelper.GetInstance().GetWareArea(-1,0);
            if (areas1 != null && areas1.Count > 0)
            {
                cmb1.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo() { FieldName = "Name", Caption = "名称" });
                cmb2.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo() { FieldName = "Name", Caption = "名称" });
                cmb3.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo() { FieldName = "Name", Caption = "名称" });
                cmb1.Properties.DataSource = areas1;
                cmb1.Properties.DisplayMember = "Name";
                cmb1.Properties.ValueMember = "id";
            }
        }

        private void InitCombox2(int id)
        {
            List<DispatchArea> areas = DBHelper.GetInstance().GetWareArea(id, 1);
            if (areas != null && areas.Count > 0)
            {
                cmb2.Properties.DataSource = areas;
                cmb2.Properties.DisplayMember = "Name";
                cmb2.Properties.ValueMember = "id";
            }
        }

        private void InitCombox3(int id)
        {
            List<DispatchArea> areas = DBHelper.GetInstance().GetWareArea(id, 2);
            if (areas != null && areas.Count > 0)
            {
                cmb3.Properties.DataSource = areas;
                cmb3.Properties.DisplayMember = "Name";
                cmb3.Properties.ValueMember = "id";
            }
        }

        private void cmb1_EditValueChanged(object sender, EventArgs e)
        {
            int id = (int)cmb1.EditValue;
            InitCombox2(id);
            cmb3.Properties.DataSource = null;
        }

        private void cmb2_EditValueChanged(object sender, EventArgs e)
        {
            int id = (int)cmb2.EditValue;
            InitCombox3(id);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmb1.EditValue == null || cmb2.EditValue == null || cmb3.EditValue == null)
            {
                MessageBox.Show("配送区域不能为空！", "系统提示");
                return;
            }
            SysParams.UserName = txtUserName.Text;
            SysParams.UserPass = string.IsNullOrEmpty(txtUserPass.Text) ? "123456" : PassWordHelper.GetInstance().DesStr(txtUserPass.Text, "isarahan", "wolfstud");
            SysParams.JDUserName = txtJDUserName.Text;
            SysParams.JDUserPass = string.IsNullOrEmpty(txtJDUserPass.Text) ? "123456" : PassWordHelper.GetInstance().DesStr(txtJDUserPass.Text, "isarahan", "wolfstud");

            SysParams.AllowAutoGetWebUrl = chkAllowAutoGetWebUrl.Checked;
            SysParams.AllowAutoGetFocusWare = chkAllowAutoGetFocusWare.Checked;
            SysParams.AutoGetFocusTime = cmbFocusTime.Text;
            
            SysParams.AllowAutoGC = true;

            #region Gather Params
            SysParams.GatherJDWare = chkGatherJD.Checked;
            SysParams.GatherQJWare = chkGatherQJ.Checked;
            SysParams.Gather3Ware = chkGather3.Checked;

            SysParams.GetWareBaseInfo = chkWareBaseInfo.Checked;
            SysParams.GetWarePrice = chkWarePrice.Checked;
            SysParams.GetWareStock = chkWareStock.Checked;
            SysParams.GetWarePostMessage = chkWarePostMessage.Checked;
            SysParams.GetWareCoupon = chkWareCopon.Checked;
            SysParams.GetWarePicture = chkWarePicture.Checked;
            #endregion Gather Params

            #region Monitor Params
            SysParams.AllowAutoUpdateWareInfo = chkAllowAutoUpdateWareinfo.Checked;
            SysParams.AutoUpdateTime = cmbUpdateTime.Text;

            SysParams.AllowGetPrice = chkAllowGetPrice.Checked;
            SysParams.AllowGetMobilePrice = chkAllowGetAPPPrice.Checked;
            SysParams.AllowGetQQPrice = chkAllowGetQQPrice.Checked;
            SysParams.AllowGetWXPrice = chkAllowGetWXPrice.Checked;

            SysParams.AllowGetStock = chkAllowGetStock.Checked;
            SysParams.AllowGetCoupon = chkAllowGetCoupon.Checked;
            SysParams.AllowGetPostMessage = chkAllowGetPostMessage.Checked;
            SysParams.AllowGetPromo = chkAllowGetPromo.Checked;
            #endregion Monitor Params

            SysParams.Level1Area = cmb1.Text;
            SysParams.Level2Area = cmb2.Text;
            SysParams.Level3Area = cmb3.Text;

            int id1 = (int)cmb1.EditValue;
            int id2 = (int)cmb2.EditValue;
            int id3 = (int)cmb3.EditValue;
            SysParams.DispathArea = string.Format("{0}_{1}_{2}_0", id1, id2, id3);

            SysParams.AllowRunAtSystemStart = chkAllowRunAtSystemStart.Checked;
            SysParams.AllowLoginVerify = chkAllowLoginVerify.Checked;
            SysParams.CheckNetState = chkCheckNetState.Checked;
            SysParams.AllowAutoUpdateVersion = chkAllowAutoUpdateVersion.Checked;

            PubClass.GetInstance().AssemblyParams();
            this.FindForm().Close();
        }

        private void ShowParams()
        {
            PubClass.GetInstance().GetSysParams();
        }

        private void layGroupCJ1_DoubleClick(object sender, EventArgs e)
        {
            _iChkStat++;
            int iOut = 0;
            //求余 % 或 Math.DivRem
            Math.DivRem(_iChkStat, 2, out iOut);
            ChangeGatherState(iOut == 1);
        }

        private void ChangeGatherState(bool state)
        {
            chkGatherJD.Checked = state;
            chkGatherQJ.Checked = state;
            chkGather3.Checked = state;
        }

        private void layGroupCJ2_DoubleClick(object sender, EventArgs e)
        {
            _iChkStat++;
            int iOut = 0;
            Math.DivRem(_iChkStat, 2, out iOut);
            ChangeWareState(iOut == 1);
        }

        private void ChangeWareState(bool state)
        {
            chkWareBaseInfo.Checked = state;
            chkWarePicture.Checked = state;
            chkWareCopon.Checked = state;
            chkWarePostMessage.Checked = state;
            chkWarePrice.Checked = state;
            chkWareStock.Checked = state;
        }

        private int _iChkStat = 0;

        private void layGroupMonitor_DoubleClick(object sender, EventArgs e)
        {
            _iChkStat++;
            int iOut = 0;
            Math.DivRem(_iChkStat, 2, out iOut);

            ChangeGetCheckState(iOut == 1);
        }

        private void ChangeGetCheckState(bool state)
        {
            chkAllowGetAPPPrice.Checked = state;
            chkAllowGetCoupon.Checked = state;
            chkAllowGetPostMessage.Checked = state;
            chkAllowGetPrice.Checked = state;
            chkAllowGetQQPrice.Checked = state;
            chkAllowGetStock.Checked = state;
            chkAllowGetWXPrice.Checked = state;
            chkAllowGetPromo.Checked = state;
        }

    }
}
