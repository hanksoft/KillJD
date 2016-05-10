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
using WareDealer.Helper;
using WareDealer.Mode;

namespace KillPrice
{
    public partial class UiTypes : DevExpress.XtraEditors.XtraUserControl
    {
        #region Field Define
        private ProductType _wareType;
        private bool _isEdit = false;
        #endregion Field Define

        public UiTypes()
        {
            InitializeComponent();
        }

        public UiTypes(ProductType wType)
        {
            InitializeComponent();
            _wareType = wType ?? new ProductType();
            _isEdit = wType != null;
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                if (_isEdit)
                {
                    _wareType.Name = txtName.Text;
                    DBHelper.GetInstance().WareTypeUpdate(_wareType);
                    MessageBox.Show("商品类别修改成功！", "系统提示");
                    this.FindForm().Close();
                }
                else
                {
                    _wareType = new ProductType()
                    {
                        TID = Guid.NewGuid().ToString(),
                        Name = txtName.Text,
                        CreateTime = DateTime.Now,
                        BEnable = true
                    };
                    DBHelper.GetInstance().WareTypeInsert(_wareType);
                    if (MessageBox.Show("商品类别保存成功！是否继续录入分类？", "系统提示",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
                    {
                        this.FindForm().Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("请输入类别名称","系统提示");
            }
        }

        private void UiTypes_Load(object sender, EventArgs e)
        {
            txtName.TabIndex = 0;
            if (_isEdit)
            {
                txtName.Text = _wareType.Name;
            }
        }
    }
}
