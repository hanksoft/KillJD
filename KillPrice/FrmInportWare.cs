using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WareDealer;

namespace KillPrice
{
    public partial class FrmInportWare : Form
    {
        public FrmInportWare()
        {
            InitializeComponent();
        }

        private void btnOpenFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                btnOpenFile.Text = fileName;
            }
        }

        /// <summary>
        /// 是否正在导入 导入时不允许退出当前窗体
        /// </summary>
        bool _isInport = false;
        bool _isProcessing = false;

        private void btnInport_Click(object sender, EventArgs e)
        {
            if (!File.Exists(btnOpenFile.Text))
            {
                return;
            }
            InitProcess();
            string fileName = btnOpenFile.Text;
            Thread exportThread = new Thread(delegate() { WareInport.GetInstance().InportWareList(fileName); }) { Name = "exportThread", IsBackground = true };
            exportThread.Start();

            _isInport = true;
        }

        private void InitProcess()
        {
            layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //设置一个最小值
            progressBarControl1.Properties.Minimum = 0;
            //设置一个最大值
            progressBarControl1.Properties.Maximum = 100;
            //设置步长，即每次增加的数
            progressBarControl1.Properties.Step = 1;
            //设置进度条的样式
            progressBarControl1.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;

            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (InportThreads.WareLength > 1 && !_isProcessing)
            {
                progressBarControl1.Properties.Maximum = InportThreads.WareLength * 2;
                progressBarControl1.Position = 0;
                _isProcessing = true;
            }

            if (_isProcessing)
            {
                if (InportThreads.WareStep < progressBarControl1.Properties.Maximum)
                {
                    //处理当前消息队列中的所有windows消息
                    Application.DoEvents();
                    //执行步长
                    progressBarControl1.PerformStep();
                    progressBarControl1.Update();
                }
                
            }

            _isInport = !InportThreads.WareEnd;
            timer1.Enabled = !InportThreads.WareEnd;
        }

        private void FrmInportWare_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isInport)
            {
                MessageBox.Show("正在导入商品，不能退出！", "系统提示");
                e.Cancel = _isInport;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmInportWare_Load(object sender, EventArgs e)
        {
            InportThreads.WareEnd = false;
        }
    }
}
