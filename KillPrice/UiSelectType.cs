using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using WareDealer.Helper;
using WareDealer.Mode;

namespace KillPrice
{
    public partial class UiSelectType : DevExpress.XtraEditors.XtraUserControl
    {
        public UiSelectType()
        {
            InitializeComponent();
        }

        string _myType;

        public string WareType
        {
            get { return _myType; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cmbTypes.EditValue != null)
            {
                _myType = cmbTypes.EditValue.ToString();
                this.FindForm().Close();
            }
            else
            {
                MessageBox.Show("请选择分类", "系统提示");
            }
        }

        private void UiSelectTyp_Load(object sender, EventArgs e)
        {
            cmbMainType.Text = "京东";
            InitTypes();
        }

        private void InitTypes()
        {
            try
            {
                List<ProductType> typsLst = DBHelper.GetInstance().WareTypeGet();
                if (typsLst != null && typsLst.Count > 0)
                {
                    cmbTypes.Properties.DataSource = typsLst;
                    cmbTypes.Properties.DisplayMember = "Name";
                    cmbTypes.Properties.ValueMember = "TID";
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }
    }
}
