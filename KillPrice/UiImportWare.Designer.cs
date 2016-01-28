namespace KillPrice
{
    partial class UiImportWare
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wizardControl1 = new DevExpress.XtraWizard.WizardControl();
            this.welcome1 = new DevExpress.XtraWizard.WelcomeWizardPage();
            this.ChoseSourcePage = new DevExpress.XtraWizard.WizardPage();
            this.completionWizardPage1 = new DevExpress.XtraWizard.CompletionWizardPage();
            this.radioGroup1 = new DevExpress.XtraEditors.RadioGroup();
            this.wizardPage1 = new DevExpress.XtraWizard.WizardPage();
            this.btnOpenFile = new DevExpress.XtraEditors.ButtonEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.wizardPage2 = new DevExpress.XtraWizard.WizardPage();
            this.wizardPage3 = new DevExpress.XtraWizard.WizardPage();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).BeginInit();
            this.wizardControl1.SuspendLayout();
            this.ChoseSourcePage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).BeginInit();
            this.wizardPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnOpenFile.Properties)).BeginInit();
            this.wizardPage2.SuspendLayout();
            this.wizardPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.SuspendLayout();
            // 
            // wizardControl1
            // 
            this.wizardControl1.CancelText = "取消";
            this.wizardControl1.Controls.Add(this.welcome1);
            this.wizardControl1.Controls.Add(this.ChoseSourcePage);
            this.wizardControl1.Controls.Add(this.completionWizardPage1);
            this.wizardControl1.Controls.Add(this.wizardPage1);
            this.wizardControl1.Controls.Add(this.wizardPage2);
            this.wizardControl1.Controls.Add(this.wizardPage3);
            this.wizardControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardControl1.FinishText = "结束";
            this.wizardControl1.Location = new System.Drawing.Point(0, 0);
            this.wizardControl1.Name = "wizardControl1";
            this.wizardControl1.NextText = "下一步";
            this.wizardControl1.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[] {
            this.welcome1,
            this.ChoseSourcePage,
            this.wizardPage1,
            this.wizardPage2,
            this.wizardPage3,
            this.completionWizardPage1});
            this.wizardControl1.PreviousText = "上一步";
            this.wizardControl1.Size = new System.Drawing.Size(535, 448);
            // 
            // welcome1
            // 
            this.welcome1.AllowBack = false;
            this.welcome1.IntroductionText = "该向导用于引导你快捷的导入京东商品数据";
            this.welcome1.Name = "welcome1";
            this.welcome1.ProceedText = "请点击“下一步”继续操作";
            this.welcome1.Size = new System.Drawing.Size(318, 315);
            this.welcome1.Text = "欢迎使用导入商品功能";
            // 
            // ChoseSourcePage
            // 
            this.ChoseSourcePage.Controls.Add(this.radioGroup1);
            this.ChoseSourcePage.DescriptionText = "请选择导入商品来源";
            this.ChoseSourcePage.Name = "ChoseSourcePage";
            this.ChoseSourcePage.Size = new System.Drawing.Size(503, 303);
            this.ChoseSourcePage.Text = "选择来源";
            // 
            // completionWizardPage1
            // 
            this.completionWizardPage1.FinishText = "商品导入完成";
            this.completionWizardPage1.Name = "completionWizardPage1";
            this.completionWizardPage1.ProceedText = "";
            this.completionWizardPage1.Size = new System.Drawing.Size(318, 315);
            this.completionWizardPage1.Text = "导入向导结束";
            // 
            // radioGroup1
            // 
            this.radioGroup1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioGroup1.EditValue = ((short)(1));
            this.radioGroup1.Location = new System.Drawing.Point(0, 0);
            this.radioGroup1.Name = "radioGroup1";
            this.radioGroup1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(((short)(0)), "本地文件"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(((short)(1)), "Internet Explore浏览器"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(((short)(2)), "FireFox浏览器"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(((short)(3)), "京东关注", false)});
            this.radioGroup1.Size = new System.Drawing.Size(503, 303);
            this.radioGroup1.TabIndex = 0;
            // 
            // wizardPage1
            // 
            this.wizardPage1.Controls.Add(this.labelControl1);
            this.wizardPage1.Controls.Add(this.btnOpenFile);
            this.wizardPage1.DescriptionText = "将商品访问地址复制到本地Text文档中进行导入操作";
            this.wizardPage1.Name = "wizardPage1";
            this.wizardPage1.Size = new System.Drawing.Size(503, 303);
            this.wizardPage1.Text = "本地文件导入";
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(147, 22);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.btnOpenFile.Size = new System.Drawing.Size(283, 20);
            this.btnOpenFile.TabIndex = 2;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(35, 25);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(96, 14);
            this.labelControl1.TabIndex = 3;
            this.labelControl1.Text = "选择商品列表文件";
            // 
            // wizardPage2
            // 
            this.wizardPage2.Controls.Add(this.groupControl1);
            this.wizardPage2.DescriptionText = "系统自动获取IE浏览器中京东商品数据";
            this.wizardPage2.Name = "wizardPage2";
            this.wizardPage2.Size = new System.Drawing.Size(503, 303);
            this.wizardPage2.Text = "IE浏览器导入";
            // 
            // wizardPage3
            // 
            this.wizardPage3.Controls.Add(this.groupControl2);
            this.wizardPage3.DescriptionText = "系统自动获取火狐浏览器京东商品页面数据";
            this.wizardPage3.Name = "wizardPage3";
            this.wizardPage3.Size = new System.Drawing.Size(503, 303);
            this.wizardPage3.Text = "FireFox浏览器导入";
            // 
            // groupControl1
            // 
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(503, 303);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "IE浏览器京东商品列表";
            // 
            // groupControl2
            // 
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl2.Location = new System.Drawing.Point(0, 0);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(503, 303);
            this.groupControl2.TabIndex = 1;
            this.groupControl2.Text = "火狐浏览器京东商品列表";
            // 
            // UiImportWare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.wizardControl1);
            this.Name = "UiImportWare";
            this.Size = new System.Drawing.Size(535, 448);
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).EndInit();
            this.wizardControl1.ResumeLayout(false);
            this.ChoseSourcePage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).EndInit();
            this.wizardPage1.ResumeLayout(false);
            this.wizardPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnOpenFile.Properties)).EndInit();
            this.wizardPage2.ResumeLayout(false);
            this.wizardPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraWizard.WizardControl wizardControl1;
        private DevExpress.XtraWizard.WelcomeWizardPage welcome1;
        private DevExpress.XtraWizard.WizardPage ChoseSourcePage;
        private DevExpress.XtraWizard.CompletionWizardPage completionWizardPage1;
        private DevExpress.XtraEditors.RadioGroup radioGroup1;
        private DevExpress.XtraWizard.WizardPage wizardPage1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.ButtonEdit btnOpenFile;
        private DevExpress.XtraWizard.WizardPage wizardPage2;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraWizard.WizardPage wizardPage3;
        private DevExpress.XtraEditors.GroupControl groupControl2;
    }
}
