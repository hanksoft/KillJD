using System;
using DevExpress.Utils.About;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraBars.Customization;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraNavBar;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraVerticalGrid.Localization;
using DevExpress.XtraLayout.Localization;

namespace KillPrice
{
    public class DevCHS
    {
        /// <summary>
        /// DEV控件汉化处理
        /// </summary>
        public DevCHS()
        {
            Localizer.Active = new XtraEditors_CN();
            GridLocalizer.Active = new XtraGrid_CN();
            BarLocalizer.Active = new XtraBar_CN();
            BarLocalizer.Active.Customization = new XtraBarsCustomizationLocalization_CN();
            NavBarLocalizer.Active = new XtraNavBar_CN();
            PreviewLocalizer.Active = new XtraPrinting_CN();
            ReportLocalizer.Active = new XtraReports_CN();
            TreeListLocalizer.Active = new XtraTreeList_CN();
            VGridLocalizer.Active = new XtraVerticalGrid_CN();
            DocumentManagerLocalizer.Active = new DocumentManager_CN();
            DockManagerResXLocalizer.Active = new DockManager_CN(); 
            //string a = DevExpress.Utils.About.AboutForm.CopyRight;
        }
    }

    public class DockManager_CN :DockManagerResXLocalizer
    {
        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }

        public override string GetLocalizedString(DockManagerStringId id)
        {
            switch (id)
            {
                case DockManagerStringId.CommandAutoHide:
                    return "自动隐藏";
                case DockManagerStringId.CommandClose:
                    return "关闭";
                case DockManagerStringId.CommandFloat:
                    return "浮动模式";
                case DockManagerStringId.CommandDock:
                    return "停靠模式";
                case DockManagerStringId.CommandDockAsTabbedDocument:
                    return "Tab模式";
            }
            return base.GetLocalizedString(id);
        }
    }

    public class DocumentManager_CN : DocumentManagerLocalizer
    {
        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }

        public override string GetLocalizedString(DocumentManagerStringId id)
        {
            switch (id)
            {
                case DocumentManagerStringId.CommandCloseAllButPinned:
                    return "关闭其他";
                case DocumentManagerStringId.CommandCloseAll:
                    return "关闭所有";
                case DocumentManagerStringId.CommandCloseAllButThis:
                    return "关闭其他";
                case DocumentManagerStringId.CommandClose:
                    return "关闭当前";
                case DocumentManagerStringId.CommandFloat:
                    return "浮动";
                case DocumentManagerStringId.CommandNewHorizontalDocumentGroup:
                    return "水平布局";
                case DocumentManagerStringId.CommandNewVerticalDocumentGroup:
                    return "垂直布局";
                case DocumentManagerStringId.CommandMoveToNextDocumentGroup:
                    return "移动到下一布局";
                case DocumentManagerStringId.CommandMoveToMainDocumentGroup:
                    return "移动到原始布局";
                case DocumentManagerStringId.CommandMoveToPrevDocumentGroup:
                    return "移动到前一布局";
            }
            return base.GetLocalizedString(id);
        }
    }


    public class XtraLayout_CN : LayoutLocalizer
    {
        public XtraLayout_CN()
        {
        }

        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }

        public override string GetLocalizedString(LayoutStringId id)
        {
            //switch (id)
            //{
            //}
            return base.GetLocalizedString(id);
        }
    }

    public class XtraEditors_CN : Localizer
    {
        public XtraEditors_CN()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }

        public override string GetLocalizedString(StringId id)
        {
            switch (id)
            {
                case StringId.TextEditMenuCopy: return "复制(&C)";
                case StringId.TextEditMenuCut: return "剪切(&T)";
                case StringId.TextEditMenuDelete: return "删除(&D)";
                case StringId.TextEditMenuPaste: return "粘贴(&P)";
                case StringId.TextEditMenuSelectAll: return "全选(&A)";
                case StringId.TextEditMenuUndo: return "撤消(&U)";
                case StringId.UnknownPictureFormat: return "未知图片格式";
                case StringId.DateEditToday: return "今天";
                case StringId.DateEditClear: return "清空";
                case StringId.DataEmpty: return "无图像";
                case StringId.ColorTabWeb: return "网页";
                case StringId.ColorTabSystem: return "系统";
                case StringId.ColorTabCustom: return "自定义";
                case StringId.CheckUnchecked: return "未选择";
                case StringId.CheckIndeterminate: return "不确定";
                case StringId.CheckChecked: return "已选择";
                case StringId.CaptionError: return "标题错误";
                case StringId.Cancel: return "取消";
                case StringId.CalcError: return "计算错误";
                case StringId.CalcButtonBack: return base.GetLocalizedString(id);
                case StringId.CalcButtonC: return base.GetLocalizedString(id);
                case StringId.CalcButtonCE: return base.GetLocalizedString(id); ;
                case StringId.CalcButtonMC: return base.GetLocalizedString(id);
                case StringId.CalcButtonMR: return base.GetLocalizedString(id);
                case StringId.CalcButtonMS: return base.GetLocalizedString(id);
                case StringId.CalcButtonMx: return base.GetLocalizedString(id);
                case StringId.CalcButtonSqrt: return base.GetLocalizedString(id);
                case StringId.OK: return "确定";
                case StringId.PictureEditMenuCopy: return "复制(&C)";
                case StringId.PictureEditMenuCut: return "剪切(&T)";
                case StringId.PictureEditMenuDelete: return "删除(&D)";
                case StringId.PictureEditMenuLoad: return "加载(&L)";
                case StringId.PictureEditMenuPaste: return "粘贴(&P)";
                case StringId.PictureEditMenuSave: return "保存(&S)";
                case StringId.PictureEditOpenFileError: return "错误图片格式";
                case StringId.PictureEditOpenFileErrorCaption: return "打开错误";
                case StringId.PictureEditOpenFileFilter: return "位图文件(*.bmp)|*.bmp|GIF动画 (*.gif)|*.gif|JPEG(*.jpg;*.jpeg)|*.jpg;*.jpeg|ICO(*.ico)|*.ico|所有图片文件|*.bmp;*.gif;*.jpeg;*.jpg;*.ico|所有文件(*.*)|*.*";
                case StringId.PictureEditOpenFileTitle: return "打开";
                case StringId.PictureEditSaveFileFilter: return "位图文件(*.bmp)|*.bmp|GIF动画(*.gif)|*.gif|JPEG(*.jpg)|*.jpg";
                case StringId.PictureEditSaveFileTitle: return "保存为";
                case StringId.TabHeaderButtonClose: return "关闭";
                case StringId.TabHeaderButtonNext: return "下一页";
                case StringId.TabHeaderButtonPrev: return "上一页";
                case StringId.XtraMessageBoxAbortButtonText: return "中断(&A)";
                case StringId.XtraMessageBoxCancelButtonText: return "取消(&C)";
                case StringId.XtraMessageBoxIgnoreButtonText: return "忽略(&I)";
                case StringId.XtraMessageBoxNoButtonText: return "否(&N)";
                case StringId.XtraMessageBoxOkButtonText: return "确定(&O)";
                case StringId.XtraMessageBoxRetryButtonText: return "重试(&R)";
                case StringId.XtraMessageBoxYesButtonText: return "是(&Y)";
                case StringId.ImagePopupEmpty: return "(空)";
                case StringId.ImagePopupPicture: return "(图片)";
                case StringId.InvalidValueText: return "无效的值";
                case StringId.LookUpEditValueIsNull: return "[无数据]";
                case StringId.LookUpInvalidEditValueType: return "无效的数据类型";
                case StringId.MaskBoxValidateError: return "输入数值不完整. 是否须要修改? 是 - 回到编辑模式以修改数值. 否 - 保持原来数值. 取消 - 回复原来数值. ";
                case StringId.NavigatorAppendButtonHint: return "添加";
                case StringId.NavigatorCancelEditButtonHint: return "取消编辑";
                case StringId.NavigatorEditButtonHint: return "编辑";
                case StringId.NavigatorEndEditButtonHint: return "结束编辑";
                case StringId.NavigatorFirstButtonHint: return "第一条";
                case StringId.NavigatorLastButtonHint: return "最后一条";
                case StringId.NavigatorNextButtonHint: return "下一条";
                case StringId.NavigatorNextPageButtonHint: return "下一页";
                case StringId.NavigatorPreviousButtonHint: return "上一条";
                case StringId.NavigatorPreviousPageButtonHint: return "上一页";
                case StringId.NavigatorRemoveButtonHint: return "删除";
                case StringId.NavigatorTextStringFormat: return "记录{0}/{1}";
                case StringId.None: return "";
                case StringId.NotValidArrayLength: return "无效的数组长度.";
            }
            return base.GetLocalizedString(id);
        }

    }


    public class XtraGrid_CN : GridLocalizer
    {
        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }

        public override string GetLocalizedString(GridStringId id)
        {
            switch (id)
            {
                case GridStringId.CardViewNewCard: return "新卡片";
                case GridStringId.CardViewQuickCustomizationButton: return "自定义格式";
                case GridStringId.CardViewQuickCustomizationButtonFilter: return "筛选";
                case GridStringId.CardViewQuickCustomizationButtonSort: return "排序";
                case GridStringId.ColumnViewExceptionMessage: return "是否确定修改?";
                case GridStringId.CustomFilterDialog2FieldCheck: return "字段";
                case GridStringId.CustomFilterDialogCancelButton: return "取消";
                case GridStringId.CustomFilterDialogCaption: return "条件为:";
                case GridStringId.FindControlClearButton: return "清空条件";
                case GridStringId.FindControlFindButton: return "查询";
                //case GridStringId.CustomFilterDialogConditionBlanks: return "空值";
                //case GridStringId.CustomFilterDialogConditionEQU: return "等于=";
                //case GridStringId.CustomFilterDialogConditionGT: return "大于>";
                //case GridStringId.CustomFilterDialogConditionGTE: return "大于或等于>=";
                //case GridStringId.CustomFilterDialogConditionLike: return "包含";
                //case GridStringId.CustomFilterDialogConditionLT: return "小于<";
                //case GridStringId.CustomFilterDialogConditionLTE: return "小于或等于>=";
                //case GridStringId.CustomFilterDialogConditionNEQ: return "不等于<>";
                //case GridStringId.CustomFilterDialogConditionNonBlanks: return "非空值";
                //case GridStringId.CustomFilterDialogConditionNotLike: return "不包含";
                case GridStringId.CustomFilterDialogFormCaption: return "清除筛选条件(&L)";
                case GridStringId.CustomFilterDialogOkButton: return "确定(&O)";
                case GridStringId.CustomFilterDialogRadioAnd: return "和(&A)";
                case GridStringId.CustomFilterDialogRadioOr: return "或者(&O)";
                case GridStringId.CustomizationBands: return "分区";
                case GridStringId.CustomizationCaption: return "自定义显示字段";
                case GridStringId.CustomizationColumns: return "列";
                case GridStringId.FileIsNotFoundError: return "文件{0}没找到!";
                case GridStringId.FilterBuilderApplyButton: return "提交";
                case GridStringId.FilterBuilderCancelButton: return "取消";
                case GridStringId.FilterBuilderCaption: return "筛选";
                case GridStringId.FilterBuilderOkButton: return "确定";
                case GridStringId.FilterPanelCustomizeButton: return "筛选";
                case GridStringId.GridGroupPanelText: return "拖曳一列页眉在此进行排序";
                case GridStringId.GridNewRowText: return "单击这里新增一行";
                case GridStringId.GridOutlookIntervals: return "一个月以上;上个月;三周前;两周前;上周;;;;;;;;昨天;今天;明天; ;;;;;;;下周;两周后;三周后;下个月;一个月之后;";
                case GridStringId.MenuColumnBestFit: return "自动调整字段宽度";
                case GridStringId.MenuColumnBestFitAllColumns: return "自动调整所有字段宽度";
                case GridStringId.MenuColumnClearFilter: return "清除筛选条件";
                case GridStringId.MenuColumnColumnCustomization: return "显示/隐藏字段";
                case GridStringId.MenuColumnFilter: return "筛选";
                case GridStringId.MenuColumnFilterEditor: return "筛选框";
                case GridStringId.MenuColumnGroup: return "按此列分组";
                case GridStringId.MenuColumnGroupBox: return "分组区";
                case GridStringId.MenuColumnRemoveColumn: return "删除该列";
                case GridStringId.MenuColumnSortAscending: return "升序排序";
                case GridStringId.MenuColumnSortDescending: return "降序排序";
                case GridStringId.MenuColumnClearSorting: return "取消排序";
                case GridStringId.MenuColumnUnGroup: return "取消分组";
                case GridStringId.MenuFooterAverage: return "平均";
                case GridStringId.MenuFooterAverageFormat: return "平均={0:#.##}";
                case GridStringId.MenuFooterCount: return "计数";
                case GridStringId.MenuFooterCountFormat: return "{0}";
                case GridStringId.MenuFooterMax: return "最大值";
                case GridStringId.MenuFooterMaxFormat: return "最大值={0}";
                case GridStringId.MenuFooterMin: return "最小";
                case GridStringId.MenuFooterMinFormat: return "最小值={0}";
                case GridStringId.MenuFooterNone: return "没有";
                case GridStringId.MenuFooterSum: return "合计";
                case GridStringId.MenuFooterSumFormat: return "求和={0:#.##}";
                case GridStringId.MenuGroupPanelClearGrouping: return "取消所有分组";
                case GridStringId.MenuGroupPanelFullCollapse: return "收缩全部分组";
                case GridStringId.MenuGroupPanelFullExpand: return "展开全部分组";
                case GridStringId.MenuGroupPanelHide: return "隐藏分组显示框";
                case GridStringId.MenuGroupPanelShow: return "显示分组显示框";
                case GridStringId.PopupFilterAll: return "(所有)";
                case GridStringId.PopupFilterBlanks: return "(空值)";
                case GridStringId.PopupFilterCustom: return "(自定义)";
                case GridStringId.PopupFilterNonBlanks: return "(非空值)";
                case GridStringId.PrintDesignerBandedView: return "打印设置(区域模式)";
                case GridStringId.PrintDesignerBandHeader: return "区标题";
                case GridStringId.PrintDesignerCardView: return "打印设置(卡片模式)";
                case GridStringId.PrintDesignerDescription: return "为当前视图设置不同的打印选项.";
                case GridStringId.PrintDesignerGridView: return "打印设置(表格模式)";
                case GridStringId.WindowErrorCaption: return "错误";
                //case GridStringId.CustomFilterDialogOkButton: return "确定";
            }
            return base.GetLocalizedString(id);
        }
    }


    public class XtraBar_CN : BarLocalizer
    {
        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }

        public override string GetLocalizedString(BarString id)
        {
            switch (id)
            {
                case BarString.AddOrRemove: return "新增或删除按钮(&A)";
                case BarString.CustomizeButton: return "自定义(&C)";
                case BarString.CustomizeWindowCaption: return "自定义";
                case BarString.MenuAnimationFade: return "减弱";
                case BarString.MenuAnimationNone: return "空";
                case BarString.MenuAnimationRandom: return "任意";
                case BarString.MenuAnimationSlide: return "滑动";
                case BarString.MenuAnimationSystem: return "(系统默认值)";
                case BarString.MenuAnimationUnfold: return "展开";
                case BarString.NewToolbarCaption: return "新建工具栏";
                case BarString.None: return "";
                case BarString.RenameToolbarCaption: return "重新命名";
                case BarString.ResetBar: return "是否确实要重置对 '{0}' 工具栏所作的修改？";
                case BarString.ResetBarCaption: return "自定义";
                case BarString.ResetButton: return "重设工具栏(&R)";
                case BarString.ToolBarMenu: return "重设(&R)$删除(&D)$!命名(&N)$!标准(&L)$总使用文字(&T)$在菜单中只用文字(&O)$图像与文本(&A)$!开始一组(&G)$常用的(&M)";
                case BarString.ToolbarNameCaption: return "工具栏名称(&T):";

            }
            return base.GetLocalizedString(id);
        }
    }


    public class XtraBarsCustomizationLocalization_CN : CustomizationControl
    {
        private System.ComponentModel.Container components = null;

        public XtraBarsCustomizationLocalization_CN()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.tpOptions.SuspendLayout();
            this.tpCommands.SuspendLayout();
            this.tpToolbars.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolBarsList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbCommands)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbCategories)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbOptionsShowFullMenus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbOptions_showFullMenusAfterDelay.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbOptions_showTips.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbOptions_ShowShortcutInTips.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbNBDlgName.Properties)).BeginInit();
            this.pnlNBDlg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbOptions_largeIcons.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbOptions_MenuAnimation.Properties)).BeginInit();
            this.SuspendLayout();


            this.btClose.Text = "关闭";
            this.btResetBar.Text = "重新设置(&R)";
            this.btRenameBar.Text = "重命名(&E)";
            this.btNewBar.Text = "新建(&N)";
            this.btDeleteBar.Text = "删除(&D)";
            this.btOptions_Reset.Text = "重置惯用数据(&R)";
            this.btNBDlgCancel.Text = "取消";
            this.btNBDlgOk.Text = "确定";
            this.tpOptions.Size = new System.Drawing.Size(354, 246);
            this.tpOptions.Text = "选项(&O)";
            this.tpCommands.Text = "命令(&C)";
            this.tpToolbars.Text = "工具栏(&B)";
            this.cbOptionsShowFullMenus.Properties.Caption = "始终显示整个菜单(&N)";
            this.cbOptions_showFullMenusAfterDelay.Properties.Caption = "鼠标指针短暂停留后显示完整菜单(&U)";
            this.cbOptions_largeIcons.Properties.Caption = "大图标(&L)";
            this.cbOptions_showTips.Properties.Caption = "显示关于工具栏屏幕提示(&T)";
            this.cbOptions_ShowShortcutInTips.Properties.Caption = "在屏幕提示中显示快捷键(&H)";
            this.lbDescCaption.Text = "详细说明";
            this.lbOptions_Other.Text = "其它";
            this.lbOptions_PCaption.Text = "个性化菜单和工具栏";
            this.lbCategoriesCaption.Text = "类别:";
            this.lbCommandsCaption.Text = "命令:";
            this.lbToolbarCaption.Text = "工具栏:";
            this.lbOptions_MenuAnimation.Text = "菜单动画设置(&M):";
            this.lbNBDlgCaption.Text = "工具栏名称(&T)";
            this.lbCommands.Appearance.BackColor = System.Drawing.SystemColors.Window;
            this.lbCommands.Appearance.Options.UseBackColor = true;
            this.Name = "XtraBarsCustomizationLocalization_CN";
            this.tpOptions.ResumeLayout(false);
            this.tpCommands.ResumeLayout(false);
            this.tpToolbars.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.toolBarsList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbCommands)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbCategories)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbOptionsShowFullMenus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbOptions_showFullMenusAfterDelay.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbOptions_showTips.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbOptions_ShowShortcutInTips.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbNBDlgName.Properties)).EndInit();
            this.pnlNBDlg.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbOptions_largeIcons.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbOptions_MenuAnimation.Properties)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
    }


    public class XtraNavBar_CN : NavBarLocalizer
    {
        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }

        public override string GetLocalizedString(NavBarStringId id)
        {
            switch (id)
            {
                case NavBarStringId.NavPaneChevronHint: return "配置按钮";
                case NavBarStringId.NavPaneMenuAddRemoveButtons: return "添加或删除按钮(&A)";
                case NavBarStringId.NavPaneMenuShowFewerButtons: return "显示较少的按钮(&F)";
                case NavBarStringId.NavPaneMenuShowMoreButtons: return "显示更多的按钮(&M)";
            }
            return base.GetLocalizedString(id);
        }
    }


    public class XtraPrinting_CN : PreviewLocalizer
    {
        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }

        public override string GetLocalizedString(PreviewStringId id)
        {
            switch (id)
            {
                case PreviewStringId.Button_Apply: return "应用";
                case PreviewStringId.Button_Cancel: return "取消";
                case PreviewStringId.Button_Help: return "帮助";
                case PreviewStringId.Button_Ok: return "确定";
                case PreviewStringId.EMail_From: return "From";
                case PreviewStringId.Margin_BottomMargin: return "下边界";
                case PreviewStringId.Margin_Inch: return "英寸";
                case PreviewStringId.Margin_LeftMargin: return "左边界";
                case PreviewStringId.Margin_Millimeter: return "毫米";
                case PreviewStringId.Margin_RightMargin: return "右边界";
                case PreviewStringId.Margin_TopMargin: return "上边界";
                case PreviewStringId.MenuItem_BackgrColor: return "颜色(&C)";
                case PreviewStringId.MenuItem_Background: return "背景(&B)";
                case PreviewStringId.MenuItem_CsvDocument: return "CSV文件";
                case PreviewStringId.MenuItem_Exit: return "退出(&X)";
                case PreviewStringId.MenuItem_Export: return "导出(&E)";
                case PreviewStringId.MenuItem_File: return "文件(&F)";
                case PreviewStringId.MenuItem_GraphicDocument: return "图片文件";
                case PreviewStringId.MenuItem_HtmDocument: return "HTML文件";
                case PreviewStringId.MenuItem_MhtDocument: return "MHT文件";
                case PreviewStringId.MenuItem_PageSetup: return "页面设置(&U)";
                case PreviewStringId.MenuItem_PdfDocument: return "PDF文件";
                case PreviewStringId.MenuItem_Print: return "打印(&P)";
                case PreviewStringId.MenuItem_PrintDirect: return "直接打印(&R)";
                case PreviewStringId.MenuItem_RtfDocument: return "RTF文件";
                case PreviewStringId.MenuItem_Send: return "发送(&D)";
                case PreviewStringId.MenuItem_TxtDocument: return "TXT文件";
                case PreviewStringId.MenuItem_View: return "视图(&V)";
                case PreviewStringId.MenuItem_ViewStatusbar: return "状态栏(&S)";
                case PreviewStringId.MenuItem_ViewToolbar: return "工具栏(&T)";
                case PreviewStringId.MenuItem_Watermark: return "水印(&W)";
                case PreviewStringId.MenuItem_XlsDocument: return "XLS文件";
                case PreviewStringId.MPForm_Lbl_Pages: return "页";
                case PreviewStringId.Msg_CreatingDocument: return "正在生成文件";
                case PreviewStringId.Msg_CustomDrawWarning: return "警告!";
                case PreviewStringId.Msg_EmptyDocument: return "此文件没有页面.";
                case PreviewStringId.Msg_FontInvalidNumber: return "字体大小不能为0或负数";
                case PreviewStringId.Msg_IncorrectPageRange: return "设置的页边界不正确";
                case PreviewStringId.Msg_NeedPrinter: return "没有安装打印机.";
                case PreviewStringId.Msg_NotSupportedFont: return "这种字体不被支持";
                case PreviewStringId.Msg_PageMarginsWarning: return "一个或以上的边界超出了打印范围.是否继续？";
                case PreviewStringId.Msg_WrongPageSettings: return "打印机不支持所选的纸张大小. 是否继续打印？";
                case PreviewStringId.Msg_WrongPrinter: return "无效的打印机名称.请检查打印机的设置是否正确.";
                case PreviewStringId.PageInfo_PageDate: return "[Date Printed]";
                case PreviewStringId.PageInfo_PageNumber: return "[Page #]";
                case PreviewStringId.PageInfo_PageNumberOfTotal: return "[Page # of Pages #]";
                case PreviewStringId.PageInfo_PageTime: return "[Time Printed]";
                case PreviewStringId.PageInfo_PageUserName: return "[User Name]";
                case PreviewStringId.PreviewForm_Caption: return "预览";
                case PreviewStringId.SaveDlg_FilterBmp: return "BMP Bitmap Format";
                case PreviewStringId.SaveDlg_FilterCsv: return "CSV文件";
                case PreviewStringId.SaveDlg_FilterGif: return "GIF Graphics Interchange Format";
                case PreviewStringId.SaveDlg_FilterHtm: return "HTML文件";
                case PreviewStringId.SaveDlg_FilterJpeg: return "JPEG File Interchange Format";
                case PreviewStringId.SaveDlg_FilterMht: return "MHT文件";
                case PreviewStringId.SaveDlg_FilterPdf: return "PDF文件";
                case PreviewStringId.SaveDlg_FilterPng: return "PNG Portable Network Graphics Format";
                case PreviewStringId.SaveDlg_FilterRtf: return "RTF文件";
                case PreviewStringId.SaveDlg_FilterTiff: return "TIFF Tag Image File Format";
                case PreviewStringId.SaveDlg_FilterTxt: return "TXT文件";
                case PreviewStringId.SaveDlg_FilterWmf: return "WMF Windows Metafile";
                case PreviewStringId.SaveDlg_FilterXls: return "Excel文件";
                case PreviewStringId.SaveDlg_Title: return "另存为";
                case PreviewStringId.SB_CurrentPageNo: return "目前页码:";
                case PreviewStringId.SB_PageInfo: return "{0}/{1}";
                case PreviewStringId.SB_PageNone: return "无";
                case PreviewStringId.SB_TotalPageNo: return "总页码:";
                case PreviewStringId.SB_ZoomFactor: return "缩放系数:";
                case PreviewStringId.ScrollingInfo_Page: return "页";
                case PreviewStringId.TB_TTip_Backgr: return "背景色";
                case PreviewStringId.TB_TTip_Close: return "退出";
                case PreviewStringId.TB_TTip_Customize: return "自定义";
                case PreviewStringId.TB_TTip_EditPageHF: return "页眉页脚";
                case PreviewStringId.TB_TTip_Export: return "导出文件";
                case PreviewStringId.TB_TTip_FirstPage: return "首页";
                case PreviewStringId.TB_TTip_HandTool: return "手掌工具";
                case PreviewStringId.TB_TTip_LastPage: return "尾页";
                case PreviewStringId.TB_TTip_Magnifier: return "放大/缩小";
                case PreviewStringId.TB_TTip_Map: return "文档视图";
                case PreviewStringId.TB_TTip_MultiplePages: return "多页";
                case PreviewStringId.TB_TTip_NextPage: return "下一页";
                case PreviewStringId.TB_TTip_PageSetup: return "页面设置";
                case PreviewStringId.TB_TTip_PreviousPage: return "上一页";
                case PreviewStringId.TB_TTip_Print: return "打印";
                case PreviewStringId.TB_TTip_PrintDirect: return "直接打印";
                case PreviewStringId.TB_TTip_Search: return "搜索";
                case PreviewStringId.TB_TTip_Send: return "发送E-Mail";
                case PreviewStringId.TB_TTip_Watermark: return "水印";
                case PreviewStringId.TB_TTip_Zoom: return "缩放";
                case PreviewStringId.TB_TTip_ZoomIn: return "放大";
                case PreviewStringId.TB_TTip_ZoomOut: return "缩小";
                case PreviewStringId.WMForm_Direction_BackwardDiagonal: return "反向倾斜";
                case PreviewStringId.WMForm_Direction_ForwardDiagonal: return "正向倾斜";
                case PreviewStringId.WMForm_Direction_Horizontal: return "横向";
                case PreviewStringId.WMForm_Direction_Vertical: return "纵向";
                case PreviewStringId.WMForm_HorzAlign_Center: return "置中";
                case PreviewStringId.WMForm_HorzAlign_Left: return "靠左";
                case PreviewStringId.WMForm_HorzAlign_Right: return "靠右";
                case PreviewStringId.WMForm_ImageClip: return "剪辑";
                case PreviewStringId.WMForm_ImageStretch: return "伸展";
                case PreviewStringId.WMForm_ImageZoom: return "缩放";
                case PreviewStringId.WMForm_PageRangeRgrItem_All: return "全部";
                case PreviewStringId.WMForm_PageRangeRgrItem_Pages: return "页码";
                case PreviewStringId.WMForm_PictureDlg_Title: return "选择图片";
                case PreviewStringId.WMForm_VertAlign_Bottom: return "底端";
                case PreviewStringId.WMForm_VertAlign_Middle: return "中间";
                case PreviewStringId.WMForm_VertAlign_Top: return "顶端";
                case PreviewStringId.WMForm_Watermark_Asap: return "ASAP";
                case PreviewStringId.WMForm_Watermark_Confidential: return "CONFIDENTIAL";
                case PreviewStringId.WMForm_Watermark_Copy: return "COPY";
                case PreviewStringId.WMForm_Watermark_DoNotCopy: return "DO NOT COPY";
                case PreviewStringId.WMForm_Watermark_Draft: return "DRAFT";
                case PreviewStringId.WMForm_Watermark_Evaluation: return "EVALUATION";
                case PreviewStringId.WMForm_Watermark_Original: return "ORIGINAL";
                case PreviewStringId.WMForm_Watermark_Personal: return "PERSONAL";
                case PreviewStringId.WMForm_Watermark_Sample: return "SAMPLE";
                case PreviewStringId.WMForm_Watermark_TopSecret: return "TOP SECRET";
                case PreviewStringId.WMForm_Watermark_Urgent: return "URGENT";
                case PreviewStringId.WMForm_ZOrderRgrItem_Behind: return "在后面";
                case PreviewStringId.WMForm_ZOrderRgrItem_InFront: return "在前面";
            }
            return base.GetLocalizedString(id);
        }
    }


    public class XtraReports_CN : ReportLocalizer
    {
        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }

        public override string GetLocalizedString(ReportStringId id)
        {
            switch (id)
            {
                case ReportStringId.BandDsg_QuantityPerPage: return "一个页面集合";
                case ReportStringId.BandDsg_QuantityPerReport: return "一个报表集合";
                case ReportStringId.BCForm_Lbl_Binding: return "结合";
                case ReportStringId.BCForm_Lbl_Property: return "属性";
                case ReportStringId.CatAppearance: return "版面";
                case ReportStringId.CatBehavior: return "状态";
                case ReportStringId.CatData: return "数据";
                case ReportStringId.CatLayout: return "布局";
                case ReportStringId.CatNavigation: return "导航";
                case ReportStringId.CatPageSettings: return "页面设置";
                case ReportStringId.Cmd_AlignToGrid: return "对齐网格线";
                case ReportStringId.Cmd_BottomMargin: return "底端边缘";
                case ReportStringId.Cmd_BringToFront: return "移到最上层";
                case ReportStringId.Cmd_Copy: return "复制";
                case ReportStringId.Cmd_Cut: return "剪贴";
                case ReportStringId.Cmd_Delete: return "删除";
                case ReportStringId.Cmd_Detail: return "详细";
                case ReportStringId.Cmd_DetailReport: return "详细报表";
                case ReportStringId.Cmd_GroupFooter: return "群组尾";
                case ReportStringId.Cmd_GroupHeader: return "群组首";
                case ReportStringId.Cmd_InsertBand: return "插入区段";
                case ReportStringId.Cmd_InsertDetailReport: return "插入详细报表";
                case ReportStringId.Cmd_InsertUnboundDetailReport: return "非绑定";
                case ReportStringId.Cmd_PageFooter: return "页尾";
                case ReportStringId.Cmd_PageHeader: return "页首";
                case ReportStringId.Cmd_Paste: return "粘贴";
                case ReportStringId.Cmd_Properties: return "属性";
                case ReportStringId.Cmd_ReportFooter: return "报表尾";
                case ReportStringId.Cmd_ReportHeader: return "报表首";
                case ReportStringId.Cmd_RtfClear: return "清除";
                case ReportStringId.Cmd_RtfLoad: return "加载文件";
                case ReportStringId.Cmd_SendToBack: return "移到最下层";
                case ReportStringId.Cmd_TableDelete: return "删除(&L)";
                case ReportStringId.Cmd_TableDeleteCell: return "单元格(&L)";
                case ReportStringId.Cmd_TableDeleteColumn: return "列(&C)";
                case ReportStringId.Cmd_TableDeleteRow: return "行(&R)";
                case ReportStringId.Cmd_TableInsert: return "插入(&I)";
                case ReportStringId.Cmd_TableInsertCell: return "单元格(&C)";
                case ReportStringId.Cmd_TableInsertColumnToLeft: return "左列(&L)";
                case ReportStringId.Cmd_TableInsertColumnToRight: return "右列(&R)";
                case ReportStringId.Cmd_TableInsertRowAbove: return "上行(&A)";
                case ReportStringId.Cmd_TableInsertRowBelow: return "下行(&B)";
                case ReportStringId.Cmd_TopMargin: return "顶端边缘";
                case ReportStringId.Cmd_ViewCode: return "检视代码";
                case ReportStringId.FindForm_Msg_FinishedSearching: return "搜索文件完成";
                case ReportStringId.FindForm_Msg_TotalFound: return "合计查找:";
                //case ReportStringId.FSForm_Btn_Delete: return "删除";
                //case ReportStringId.FSForm_GrBox_Sample: return "范例";
                //case ReportStringId.FSForm_Lbl_Category: return "类别";
                //case ReportStringId.FSForm_Lbl_CustomGeneral: return "一般格式不包含特殊数字格式";
                //case ReportStringId.FSForm_Lbl_Prefix: return "上标";
                //case ReportStringId.FSForm_Lbl_Suffix: return "下标";
                //case ReportStringId.FSForm_Msg_BadSymbol: return "损坏的符号";
                //case ReportStringId.FSForm_Tab_Custom: return "自定义";
                //case ReportStringId.FSForm_Tab_StandardTypes: return "标准类型";
                case ReportStringId.Msg_CantFitBarcodeToControlBounds: return "条形码控件的边界太小";
                case ReportStringId.Msg_CreateReportInstance: return "您试图打开一个不同类型的报表来编辑。是否确定建立实例？";
                case ReportStringId.Msg_CreateSomeInstance: return "在窗体中不能建立两个实例类。";
                case ReportStringId.Msg_CyclicBoormarks: return "报表循环书签";
                case ReportStringId.Msg_DontSupportMulticolumn: return "详细报表不支持多字段。";
                case ReportStringId.Msg_FileCorrupted: return "不能加载报表，文件可能被破坏或者报表组件丢失。";
                case ReportStringId.Msg_FileNotFound: return "文件没有找到";
                case ReportStringId.Msg_FillDataError: return "数据加载时发生错误。错误为：";
                case ReportStringId.Msg_IncorrectArgument: return "参数值输入不正确";
                case ReportStringId.Msg_IncorrectBandType: return "无效的带型";
                case ReportStringId.Msg_InvalidBarcodeText: return "在文本中有无效的字符";
                case ReportStringId.Msg_InvalidBarcodeTextFormat: return "无效的文本格式";
                case ReportStringId.Msg_InvalidMethodCall: return "对象的当前状态下不能调用此方法";
                case ReportStringId.Msg_InvalidReportSource: return "无法设置原报表";
                case ReportStringId.Msg_InvPropName: return "无效的属性名";
                case ReportStringId.Msg_ScriptError: return "在脚本中发现错误： {0}";
                case ReportStringId.Msg_ScriptExecutionError: return "在脚本执行过程中发现错误 {0}:  {1} 过程 {0} 被运行，将不能再被调用。";
                case ReportStringId.Msg_WrongReportClassName: return "一个错误发生在并行化期间 - 可能是报表类名错误";
                case ReportStringId.MultiColumnDesignMsg1: return "重复列之间的空位";
                case ReportStringId.MultiColumnDesignMsg2: return "控件位置不正确，将会导致打印错误";
                case ReportStringId.PanelDesignMsg: return "在此可放置不同控件";
                case ReportStringId.RepTabCtl_Designer: return "设计";
                case ReportStringId.RepTabCtl_HtmlView: return "HTML视图";
                case ReportStringId.RepTabCtl_Preview: return "预览";
                case ReportStringId.SSForm_Btn_Close: return "关闭";
                case ReportStringId.SSForm_Caption: return "式样单编辑";
                case ReportStringId.SSForm_Msg_FileFilter: return "Report StyleSheet files (*.repss)|*.repss|All files (*.*)|*.*";
                case ReportStringId.SSForm_Msg_InvalidFileFormat: return "无效的文件格式";
                case ReportStringId.SSForm_Msg_MoreThanOneStyle: return "你选择了多过一个以上的式样";
                case ReportStringId.SSForm_Msg_NoStyleSelected: return "没有式样被选中";
                case ReportStringId.SSForm_Msg_SelectedStylesText: return "被选中的式样";
                case ReportStringId.SSForm_Msg_StyleNamePreviewPostfix: return "式样";
                case ReportStringId.SSForm_Msg_StyleSheetError: return "StyleSheet错误";
                case ReportStringId.SSForm_TTip_AddStyle: return "添加式样";
                case ReportStringId.SSForm_TTip_ClearStyles: return "清除式样";
                case ReportStringId.SSForm_TTip_LoadStyles: return "从文件中读入式样";
                case ReportStringId.SSForm_TTip_PurgeStyles: return "清除式样";
                case ReportStringId.SSForm_TTip_RemoveStyle: return "移除式样";
                case ReportStringId.SSForm_TTip_SaveStyles: return "保存式样到文件";
                case ReportStringId.UD_FormCaption: return "XtraReport设计";
                case ReportStringId.UD_Msg_ReportChanged: return "报表内容已被修改，是否须要储存？";
                case ReportStringId.UD_ReportDesigner: return "XtraReport设计";
                case ReportStringId.UD_TTip_AlignBottom: return "对齐主控项的下缘";
                case ReportStringId.UD_TTip_AlignHorizontalCenters: return "对齐主控项的垂直中间";
                case ReportStringId.UD_TTip_AlignLeft: return "对齐主控项的左缘";
                case ReportStringId.UD_TTip_AlignRight: return "对齐主控项的右缘";
                case ReportStringId.UD_TTip_AlignToGrid: return "对齐网格线";
                case ReportStringId.UD_TTip_AlignTop: return "对齐主控项的上缘";
                case ReportStringId.UD_TTip_AlignVerticalCenters: return "对齐主控项的水平中央";
                case ReportStringId.UD_TTip_BringToFront: return "移到最上层";
                case ReportStringId.UD_TTip_CenterHorizontally: return "水平置中";
                case ReportStringId.UD_TTip_CenterVertically: return "垂直置中";
                case ReportStringId.UD_TTip_EditCopy: return "复制";
                case ReportStringId.UD_TTip_EditCut: return "剪贴";
                case ReportStringId.UD_TTip_EditPaste: return "粘贴";
                case ReportStringId.UD_TTip_FileOpen: return "打开文件";
                case ReportStringId.UD_TTip_FileSave: return "保存文件";
                case ReportStringId.UD_TTip_FormatAlignLeft: return "左对齐";
                case ReportStringId.UD_TTip_FormatAlignRight: return "右对齐";
                case ReportStringId.UD_TTip_FormatBackColor: return "背景颜色";
                case ReportStringId.UD_TTip_FormatBold: return "粗体";
                case ReportStringId.UD_TTip_FormatCenter: return "居中";
                case ReportStringId.UD_TTip_FormatFontName: return "字体";
                case ReportStringId.UD_TTip_FormatFontSize: return "大小";
                case ReportStringId.UD_TTip_FormatForeColor: return "前景颜色";
                case ReportStringId.UD_TTip_FormatItalic: return "斜体";
                case ReportStringId.UD_TTip_FormatJustify: return "两端对齐";
                case ReportStringId.UD_TTip_FormatUnderline: return "下划线";
                case ReportStringId.UD_TTip_HorizSpaceConcatenate: return "移除水平间距";
                case ReportStringId.UD_TTip_HorizSpaceDecrease: return "减少水平间距";
                case ReportStringId.UD_TTip_HorizSpaceIncrease: return "增加水平间距";
                case ReportStringId.UD_TTip_HorizSpaceMakeEqual: return "将垂直间距设为相等";
                case ReportStringId.UD_TTip_Redo: return "恢复";
                case ReportStringId.UD_TTip_SendToBack: return "移到最下层";
                case ReportStringId.UD_TTip_SizeToControl: return "设置成相同大小";
                case ReportStringId.UD_TTip_SizeToControlHeight: return "设置成相同高度";
                case ReportStringId.UD_TTip_SizeToControlWidth: return "设置成相同宽度";
                case ReportStringId.UD_TTip_SizeToGrid: return "依网格线调整大小";
                case ReportStringId.UD_TTip_Undo: return "撤消";
                case ReportStringId.UD_TTip_VertSpaceConcatenate: return "移除垂直间距";
                case ReportStringId.UD_TTip_VertSpaceDecrease: return "减少垂直间距";
                case ReportStringId.UD_TTip_VertSpaceIncrease: return "增加垂直间距";
                case ReportStringId.UD_TTip_VertSpaceMakeEqual: return "将垂直间距设为相等";
            }
            return base.GetLocalizedString(id);
        }
    }


    public class XtraTreeList_CN : TreeListLocalizer
    {
        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }

        public override string GetLocalizedString(TreeListStringId id)
        {
            switch (id)
            {
                case TreeListStringId.ColumnCustomizationText: return "自定显示字段";
                case TreeListStringId.ColumnNamePrefix: return "列名首标";
                case TreeListStringId.InvalidNodeExceptionText: return "是否确定要修改?";
                case TreeListStringId.MenuColumnBestFit: return "最佳匹配";
                case TreeListStringId.MenuColumnBestFitAllColumns: return "最佳匹配(所有列)";
                case TreeListStringId.MenuColumnColumnCustomization: return "列选择";
                case TreeListStringId.MenuColumnSortAscending: return "升序排序";
                case TreeListStringId.MenuColumnSortDescending: return "降序排序";
                case TreeListStringId.MenuFooterAllNodes: return "全部节点";
                case TreeListStringId.MenuFooterAverage: return "平均";
                case TreeListStringId.MenuFooterAverageFormat: return "平均值={0:#.##}";
                case TreeListStringId.MenuFooterCount: return "计数";
                case TreeListStringId.MenuFooterCountFormat: return "{0}";
                case TreeListStringId.MenuFooterMax: return "最大值";
                case TreeListStringId.MenuFooterMaxFormat: return "最大值={0}";
                case TreeListStringId.MenuFooterMin: return "最小值";
                case TreeListStringId.MenuFooterMinFormat: return "最小值={0}";
                case TreeListStringId.MenuFooterNone: return "无";
                case TreeListStringId.MenuFooterSum: return "合计";
                case TreeListStringId.MenuFooterSumFormat: return "合计={0:#.##}";
                case TreeListStringId.MultiSelectMethodNotSupported: return "OptionsBehavior.MultiSelect未激活时，指定方法不能工作.";
                case TreeListStringId.PrintDesignerDescription: return "为当前的树状列表设置不同的打印选项.";
                case TreeListStringId.PrintDesignerHeader: return "打印设置";
            }
            return base.GetLocalizedString(id);
        }
    }


    public class XtraVerticalGrid_CN : VGridLocalizer
    {
        public override string Language
        {
            get
            {
                return "简体中文";
            }
        }
        public override string GetLocalizedString(VGridStringId id)
        {
            switch (id)
            {
                case VGridStringId.InvalidRecordExceptionText: return "是否确定修改?";
                case VGridStringId.RowCustomizationDeleteCategoryText: return "删除";
                case VGridStringId.RowCustomizationNewCategoryFormLabelText: return "标题";
                case VGridStringId.RowCustomizationNewCategoryFormText: return "新增类别";
                case VGridStringId.RowCustomizationNewCategoryText: return "新增";
                case VGridStringId.RowCustomizationTabPageCategoriesText: return "类别";
                case VGridStringId.RowCustomizationTabPageRowsText: return "行";
                case VGridStringId.RowCustomizationText: return "自定义";
                case VGridStringId.StyleCreatorName: return "自定义式样";
            }
            return base.GetLocalizedString(id);
        }
    }

}

