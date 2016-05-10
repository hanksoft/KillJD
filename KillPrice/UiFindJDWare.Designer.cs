namespace KillPrice
{
    partial class UiFindJDWare
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnImport = new DevExpress.XtraEditors.SimpleButton();
            this.btnFind = new DevExpress.XtraEditors.SimpleButton();
            this.txtSearchCondition = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.grpSearchResult = new DevExpress.XtraEditors.GroupControl();
            this.progressSearch = new DevExpress.XtraWaitForm.ProgressPanel();
            this.txtImportMessage = new DevExpress.XtraEditors.MemoEdit();
            this.progressImport = new DevExpress.XtraEditors.ProgressBarControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchCondition.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpSearchResult)).BeginInit();
            this.grpSearchResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtImportMessage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressImport.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnImport);
            this.panelControl1.Controls.Add(this.btnFind);
            this.panelControl1.Controls.Add(this.txtSearchCondition);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(592, 43);
            this.panelControl1.TabIndex = 5;
            // 
            // btnImport
            // 
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(505, 10);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 8;
            this.btnImport.Text = "导入";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(426, 10);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(75, 23);
            this.btnFind.TabIndex = 7;
            this.btnFind.Text = "搜索";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtSearchCondition
            // 
            this.txtSearchCondition.Location = new System.Drawing.Point(80, 13);
            this.txtSearchCondition.Name = "txtSearchCondition";
            this.txtSearchCondition.Size = new System.Drawing.Size(336, 20);
            this.txtSearchCondition.TabIndex = 6;
            this.txtSearchCondition.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchCondition_KeyDown);
            this.txtSearchCondition.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearchCondition_KeyPress);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(16, 16);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 14);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "搜索条件";
            // 
            // grpSearchResult
            // 
            this.grpSearchResult.Controls.Add(this.progressSearch);
            this.grpSearchResult.Controls.Add(this.txtImportMessage);
            this.grpSearchResult.Controls.Add(this.progressImport);
            this.grpSearchResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSearchResult.Location = new System.Drawing.Point(0, 43);
            this.grpSearchResult.Name = "grpSearchResult";
            this.grpSearchResult.Size = new System.Drawing.Size(592, 403);
            this.grpSearchResult.TabIndex = 6;
            this.grpSearchResult.Text = "搜索结果";
            // 
            // progressSearch
            // 
            this.progressSearch.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.progressSearch.Appearance.Options.UseBackColor = true;
            this.progressSearch.AppearanceCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.progressSearch.AppearanceCaption.Options.UseFont = true;
            this.progressSearch.AppearanceDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.progressSearch.AppearanceDescription.Options.UseFont = true;
            this.progressSearch.Caption = "请稍候";
            this.progressSearch.Description = "正在获取商品数据 ...";
            this.progressSearch.Location = new System.Drawing.Point(170, 94);
            this.progressSearch.Name = "progressSearch";
            this.progressSearch.Size = new System.Drawing.Size(246, 66);
            this.progressSearch.TabIndex = 0;
            this.progressSearch.Text = "progressPanel1";
            this.progressSearch.Visible = false;
            // 
            // txtImportMessage
            // 
            this.txtImportMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtImportMessage.EditValue = "开始导入";
            this.txtImportMessage.Location = new System.Drawing.Point(2, 40);
            this.txtImportMessage.Name = "txtImportMessage";
            this.txtImportMessage.Properties.Appearance.BackColor = System.Drawing.Color.Black;
            this.txtImportMessage.Properties.Appearance.ForeColor = System.Drawing.Color.Red;
            this.txtImportMessage.Properties.Appearance.Options.UseBackColor = true;
            this.txtImportMessage.Properties.Appearance.Options.UseForeColor = true;
            this.txtImportMessage.Properties.ReadOnly = true;
            this.txtImportMessage.Size = new System.Drawing.Size(588, 361);
            this.txtImportMessage.TabIndex = 3;
            this.txtImportMessage.UseOptimizedRendering = true;
            this.txtImportMessage.Visible = false;
            // 
            // progressImport
            // 
            this.progressImport.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressImport.Location = new System.Drawing.Point(2, 22);
            this.progressImport.Name = "progressImport";
            this.progressImport.Properties.ShowTitle = true;
            this.progressImport.Size = new System.Drawing.Size(588, 18);
            this.progressImport.TabIndex = 2;
            this.progressImport.Visible = false;
            // 
            // UiFindJDWare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpSearchResult);
            this.Controls.Add(this.panelControl1);
            this.Name = "UiFindJDWare";
            this.Size = new System.Drawing.Size(592, 446);
            this.Load += new System.EventHandler(this.UiFindJDWare_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchCondition.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpSearchResult)).EndInit();
            this.grpSearchResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtImportMessage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressImport.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnImport;
        private DevExpress.XtraEditors.SimpleButton btnFind;
        private DevExpress.XtraEditors.TextEdit txtSearchCondition;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.GroupControl grpSearchResult;
        private DevExpress.XtraWaitForm.ProgressPanel progressSearch;
        private DevExpress.XtraEditors.MemoEdit txtImportMessage;
        private DevExpress.XtraEditors.ProgressBarControl progressImport;

    }
}
