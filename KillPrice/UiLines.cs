using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using DevExpress.XtraCharts;
using DevExpress.Utils;
using WareDealer.Mode;

namespace KillPrice
{
    public partial class UiLines : UserControl
    {
        public UiLines()
        {
            InitializeComponent();
        }

        private string _wareID;
        private string _wareName;

        public UiLines(string wID,string wName)
        {
            InitializeComponent();
            _wareID = wID;
            _wareName = wName;
        }

        private void xChartLines()
        {
            try
            {
                InitXChart();

                if (!string.IsNullOrEmpty(_wareID))
                {
                    GetDataSetByWareID(_wareID);
                }
                else
                {
                    MessageBox.Show("错误的商品编号！", "系统错误");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void btnLines_Click(object sender, EventArgs e)
        {
            xChartLines();
        }

        private void CreateChart(DataTable dt)
        {
            #region Series
            //创建几个图形的对象
            Series series1 = CreateSeries("员工人数", ViewType.Line, dt, 0);
            Series series2 = CreateSeries("人均月薪", ViewType.Line, dt, 1);
            Series series3 = CreateSeries("成本TEU", ViewType.Line, dt, 2);
            Series series4 = CreateSeries("人均生产率", ViewType.Line, dt, 3);
            Series series5 = CreateSeries("占2005年3月人数比例", ViewType.Line, dt, 4);
            #endregion

            List<Series> list = new List<Series>() { series1, series2, series3, series4, series5 };
            chartControl1.Series.AddRange(list.ToArray());
            chartControl1.Legend.Visible = false;
            chartControl1.SeriesTemplate.LabelsVisibility = DefaultBoolean.True;

            for (int i = 0; i < list.Count; i++)
            {
                //list[i].View.Color = colorList[i];

                CreateAxisY(list[i]);
            }
        }

        private void InitXChart()
        {
            //出现滚动条？
            DevExpress.XtraCharts.XYDiagram xyDiagram1 = (XYDiagram)this.chartControl1.Diagram;
            xyDiagram1.AxisX.VisualRange.SetMinMaxValues(-0.5D,3);

            //xyDiagram1.AxisX.Range.MaxValueInternal = 3; //这个属性在设计视图里面是看不到的，只有代码里面才可以设置。
            //xyDiagram1.AxisX.Range.MinValueInternal = -0.5D;

            //AxisX ax = (XYDiagram)chartControl1.Diagram;
            //ax.GridSpacingAuto = false;
            //ax.DateTimeMeasureUnit = DateTimeMeasurementUnit.Minute;//这个可以根据你自己的情况设置
            //ax.DateTimeGridAlignment = DateTimeMeasurementUnit.Second; //这个是间隔单位
            //ax.GridSpacing = 10; // 每10秒为一个间隔。
        }

        private void GetDataSetByWareID(string wID)
        {
            try
            {
                List<ProductPriceHistory> prices = WareDealer.Helper.DBHelper.GetInstance().WarePriceHistoryGetMore(wID);
                CreateChart(prices);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void CreateChart(List<ProductPriceHistory> prices)
        {
            Series m = CreateSeries("询价日期", ViewType.Line, prices);

            chartControl1.Series.Add(m);
            chartControl1.Legend.Visible = false;
            chartControl1.SeriesTemplate.LabelsVisibility = DefaultBoolean.True;

            //for (int i = 0; i < list.Count; i++)
            //{
            //    //list[i].View.Color = colorList[i];

            //    CreateAxisY(list[i]);
            //}
        }

        private DataTable CreateData(List<ProductPriceHistory> prices)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("询价日期"));
            foreach (var item in prices)
            {
                dt.Columns.Add(new DataColumn(item.PriceDate.ToString(), typeof(decimal)));
                dt.Rows.Add(new object[] { "历史价格", item.Price });
            }

            return dt;
        }

        /// <summary>
        /// 准备数据内容
        /// </summary>
        /// <returns></returns>
        private DataTable CreateData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("类型"));
            dt.Columns.Add(new DataColumn("2005-1月", typeof(decimal)));
            dt.Columns.Add(new DataColumn("2005-2月", typeof(decimal)));
            dt.Columns.Add(new DataColumn("2005-3月", typeof(decimal)));
            dt.Columns.Add(new DataColumn("2005-4月", typeof(decimal)));
            dt.Columns.Add(new DataColumn("2005-5月", typeof(decimal)));
            dt.Columns.Add(new DataColumn("2005-6月", typeof(decimal)));

            dt.Rows.Add(new object[] { "员工人数", 437, 437, 414, 397, 387, 378 });
            dt.Rows.Add(new object[] { "人均月薪", 3964, 3961, 3979, 3974, 3967, 3972 });
            dt.Rows.Add(new object[] { "成本TEU", 3104, 1339, 3595.8, 3154.5, 2499.8, 3026 });
            dt.Rows.Add(new object[] { "人均生产率", 7.1, 3.06, 8.69, 7.95, 6.46, 8.01 });
            dt.Rows.Add(new object[] { "占2005年3月人数比例", 1.06, 1.06, 1, 0.96, 0.93, 0.91 });

            return dt;
        }

        private Series CreateSeries(string caption, ViewType viewType, List<ProductPriceHistory> prices)
        {
            //只取部分，重复的价格将抛弃部分点
            Series series = new Series(caption, viewType);
            for (int i = 1; i < prices.Count; i++)
            {
                string argument = prices[i].PriceDate.ToString("yyyy-MM-dd HH点");//参数名称
                decimal value = (decimal)prices[i].Price;//参数值
                series.Points.Add(new SeriesPoint(argument, value));
            }

            //必须设置ArgumentScaleType的类型，否则显示会转换为日期格式，导致不是希望的格式显示
            //也就是说，显示字符串的参数，必须设置类型为ScaleType.Qualitative
            series.ArgumentScaleType = ScaleType.Qualitative;
            series.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;//显示标注标签

            return series;
        }

        /// <summary>
        /// 根据数据创建一个图形展现
        /// </summary>
        /// <param name="caption">图形标题</param>
        /// <param name="viewType">图形类型</param>
        /// <param name="dt">数据DataTable</param>
        /// <param name="rowIndex">图形数据的行序号</param>
        /// <returns></returns>
        private Series CreateSeries(string caption, ViewType viewType, DataTable dt, int rowIndex)
        {
            Series series = new Series(caption, viewType);
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                string argument = dt.Columns[i].ColumnName;//参数名称
                decimal value = (decimal)dt.Rows[rowIndex][i];//参数值
                series.Points.Add(new SeriesPoint(argument, value));
            }

            //必须设置ArgumentScaleType的类型，否则显示会转换为日期格式，导致不是希望的格式显示
            //也就是说，显示字符串的参数，必须设置类型为ScaleType.Qualitative
            series.ArgumentScaleType = ScaleType.Qualitative;
            series.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;//显示标注标签

            return series;
        }

        /// <summary>
        /// 创建图表的第二坐标系
        /// </summary>
        /// <param name="series">Series对象</param>
        /// <returns></returns>
        private SecondaryAxisY CreateAxisY(Series series)
        {
            SecondaryAxisY myAxis = new SecondaryAxisY(series.Name);
            ((XYDiagram)chartControl1.Diagram).SecondaryAxesY.Add(myAxis);
            ((LineSeriesView)series.View).AxisY = myAxis;
            myAxis.Title.Text = series.Name;
            myAxis.Title.Alignment = StringAlignment.Far; //顶部对齐
            myAxis.Title.Visible = true; //显示标题
            myAxis.Title.Font = new Font("宋体", 9.0f);

            Color color = series.View.Color;//设置坐标的颜色和图表线条颜色一致

            myAxis.Title.TextColor = color;
            myAxis.Label.TextColor = color;
            myAxis.Color = color;

            return myAxis;
        }

        private void XChartLines()
        {
            #region 仅供参考（不需要）
            //控制X、Y轴显示
            //XYDiagram diagram = (XYDiagram)chartControl.Diagram;
            //diagram.AxisX.Label.Staggered = true;
            //diagram.AxisY.Label.BeginText = "Axis value = ";
            //diagram.AxisY.Label.Angle = -30;
            //diagram.AxisY.Label.Antialiasing = true;

            //XYDiagram diagram = (XYDiagram)chartControl.Diagram; 
            //diagram.AxisX.DateTimeMeasureUnit = DateTimeMeasurementUnit.Second; 
            //diagram.AxisX.DateTimeOptions.Format = DateTimeFormat.Custom; diagram.AxisX.DateTimeOptions.FormatString = "HH:mm:ss";

            //((XYDiagram)myChartControl.Diagram).AxisX.Interlaced = true;
            //((XYDiagram)myChartControl.Diagram).AxisX.GridSpacing = 10;
            //((XYDiagram)myChartControl.Diagram).AxisX.Label.Angle = -30;
            //((XYDiagram)myChartControl.Diagram).AxisX.Label.Antialiasing = true;
            //((XYDiagram)myChartControl.Diagram).AxisX.DateTimeOptions.Format = DateTimeFormat.MonthAndDay;

            ////XYDiagram xyDiagram1 = new XYDiagram();
            ////xyDiagram1.AxisX.Range.Auto = false; //要开启滚动条必须将其设置为false
            //////xyDiagram1.AxisX.Range.MaxValueInternal = 30.5D > (cnt + 1) ? (cnt + 1) : 30.5D;//在不拉到滚动条的时候，X轴显示多个值，既固定的X轴长度。
            //////xyDiagram1.AxisX.Range.MinValueInternal = -0.5D;
            ////xyDiagram1.AxisX.Range.ScrollingRange.Auto = false;
            ////xyDiagram1.AxisX.MinorCount = 9; //显示X轴间隔数量
            ////xyDiagram1.AxisX.Tickmarks.MinorVisible = true;//是否显示X轴间隔
            //xyDiagram1.AxisY.MinorCount = 1;//显示Y轴间隔数量
            //xyDiagram1.AxisY.Tickmarks.MinorVisible = true;//是否显示Y轴间隔

            //xyDiagram1.AxisX.Range.ScrollingRange.MaxValueSerializable = (cnt + 1).ToString();//整个X轴最多显示多多少个值
            // xyDiagram1.AxisX.Range.ScrollingRange.MinValueSerializable = "0";
            //xyDiagram1.AxisX.DateTimeMeasureUnit = DateTimeMeasurementUnit.Second;
            //xyDiagram1.AxisX.DateTimeOptions.Format = DateTimeFormat.Custom;
            //xyDiagram1.AxisX.DateTimeOptions.FormatString = "yyyy:MM:HH";
            //xyDiagram1.AxisX.Range.ScrollingRange.SideMarginsEnabled = true;//是否从X轴原点开始显示
            //xyDiagram1.AxisX.Range.SideMarginsEnabled = false;
            ////xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
            ////xyDiagram1.AxisY.NumericOptions.Format = DevExpress.XtraCharts.NumericFormat.Percent;//显示为百分数
            //xyDiagram1.AxisY.Range.Auto = false;
            ////xyDiagram1.AxisY.Range.MaxValueSerializable = "1.02";
            ////xyDiagram1.AxisY.Range.MinValueSerializable = "0.5";
            //xyDiagram1.AxisY.Range.ScrollingRange.SideMarginsEnabled = true;
            //xyDiagram1.AxisY.Range.SideMarginsEnabled = true;
            ////xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
            //xyDiagram1.EnableScrolling = true;//启用滚动条

            //获取Diagram必须在ChartControl中已经加入了Series之后
            //((XYDiagram)chartControl.Diagram).Rotated = false;
            #endregion

            //图标位置
            //myChartControl.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.Right;
            //myChartControl.Legend.AlignmentVertical = LegendAlignmentVertical.Top;

            //ChartTitle chartTitle = new ChartTitle();
            //chartTitle.Text = this.Text;//标题内容
            //chartTitle.TextColor = System.Drawing.Color.Black;//字体颜色
            //chartTitle.Font = new Font("Tahoma", 8);//字体类型字号
            //chartTitle.Dock = ChartTitleDockStyle.Bottom;//标题对齐方式
            //chartTitle.Alignment = StringAlignment.Far;
            //myChartControl.Titles.Clear();//清理标题
            //myChartControl.Titles.Add(chartTitle);//加载标题


            //Series series1 = new Series(this.Text, ViewType.Spline);
            //series1.ArgumentScaleType = ScaleType.DateTime;//x轴类型
            //series1.ValueScaleType = ScaleType.Numerical;//y轴类型
            ////X轴的数据字段
            //series1.ArgumentDataMember = "StatisticsTime";
            ////Y轴的数据字段
            //series1.ValueDataMembers[0] = "StatisticsSum";
            ////定义线条上点的标识形状是否需要
            //((LineSeriesView)series1.View).LineMarkerOptions.Visible = false;
            ////定义线条上点的标识形状
            //((LineSeriesView)series1.View).LineMarkerOptions.Kind = MarkerKind.Circle;          
            ////不显示X、Y轴上面的交点的值
            //((PointSeriesLabel)series1.Label).Visible = false;
            ////线条的类型，虚线，实线
            //((LineSeriesView)series1.View).LineStyle.DashStyle = DashStyle.Solid
        }

        private void UiLines_Load(object sender, EventArgs e)
        {
            txtWareID.Text = _wareID;
            txtWareName.Text = _wareName;

            xChartLines();
        }
    }
}
