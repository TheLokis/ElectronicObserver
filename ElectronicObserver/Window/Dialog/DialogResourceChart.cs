using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ElectronicObserver.Window.Dialog
{

	public partial class DialogResourceChart : Form
	{


		private enum ChartType
		{
			Resource,
			ResourceDiff,
			Material,
			MaterialDiff,
			Experience,
			ExperienceDiff,
		}

		private enum ChartSpan
		{
			Day,
			Week,
			Month,
			Season,
			Year,
			All,
			WeekFirst,
			MonthFirst,
			SeasonFirst,
			YearFirst,
		}



		private ChartType SelectedChartType => (ChartType)this.GetSelectedMenuStripIndex(this.Menu_Graph);

		private ChartSpan SelectedChartSpan => (ChartSpan)this.GetSelectedMenuStripIndex(this.Menu_Span);




		public DialogResourceChart()
		{
            this.InitializeComponent();
		}



		private void DialogResourceChart_Load(object sender, EventArgs e)
		{

			if (!RecordManager.Instance.Resource.Record.Any())
			{
				MessageBox.Show("기록 데이터가 존재하지 않습니다.\n모항화면으로 한번 이동해주세요.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
				return;
			}


			{
				int i = 0;
				foreach (var span in this.Menu_Span.DropDownItems.OfType<ToolStripMenuItem>())
				{
					span.Tag = i;
					i++;
				}
			}


            this.SwitchMenuStrip(this.Menu_Graph, 0);
            this.SwitchMenuStrip(this.Menu_Span, 2);


            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormResourceChart]);

            this.UpdateChart();
		}



		private void SetResourceChart()
		{

            this.ResourceChart.ChartAreas.Clear();
			var area = this.ResourceChart.ChartAreas.Add("ResourceChartArea");
			area.AxisX = this.CreateAxisX(this.SelectedChartSpan);
			area.AxisY = this.CreateAxisY(2000);
			area.AxisY2 = this.CreateAxisY(200);

            this.ResourceChart.Legends.Clear();
			var legend = this.ResourceChart.Legends.Add("ResourceLegend");
			legend.Font = this.Font;


            this.ResourceChart.Series.Clear();

			var fuel = this.ResourceChart.Series.Add("ResourceSeries_Fuel");
			var ammo = this.ResourceChart.Series.Add("ResourceSeries_Ammo");
			var steel = this.ResourceChart.Series.Add("ResourceSeries_Steel");
			var bauxite = this.ResourceChart.Series.Add("ResourceSeries_Bauxite");
			var instantRepair = this.ResourceChart.Series.Add("ResourceSeries_InstantRepair");

			var setSeries = new Action<Series>(s =>
			{
				s.ChartType = SeriesChartType.Line;
				s.Font = this.Font;
				s.XValueType = ChartValueType.DateTime;
			});

			setSeries(fuel);
			fuel.Color = Color.FromArgb(0, 128, 0);
			fuel.LegendText = "연료";

			setSeries(ammo);
			ammo.Color = Color.FromArgb(255, 128, 0);
			ammo.LegendText = "탄약";

			setSeries(steel);
			steel.Color = Color.FromArgb(64, 64, 64);
			steel.LegendText = "강재";

			setSeries(bauxite);
			bauxite.Color = Color.FromArgb(255, 0, 0);
			bauxite.LegendText = "보키";

			setSeries(instantRepair);
			instantRepair.Color = Color.FromArgb(32, 128, 255);
			instantRepair.LegendText = "고속수복재";
			instantRepair.YAxisType = AxisType.Secondary;


			//データ設定
			{
				var record = this.GetRecords();

				if (record.Any())
				{
					var prev = record.First();
					foreach (var r in record)
					{

						if (this.ShouldSkipRecord(r.Date - prev.Date))
							continue;

						fuel.Points.AddXY(r.Date.ToOADate(), r.Fuel);
						ammo.Points.AddXY(r.Date.ToOADate(), r.Ammo);
						steel.Points.AddXY(r.Date.ToOADate(), r.Steel);
						bauxite.Points.AddXY(r.Date.ToOADate(), r.Bauxite);
						instantRepair.Points.AddXY(r.Date.ToOADate(), r.InstantRepair);

						prev = r;
					}
				}
			}


            this.SetYBounds();
		}


		private void SetResourceDiffChart()
		{

            this.ResourceChart.ChartAreas.Clear();
			var area = this.ResourceChart.ChartAreas.Add("ResourceChartArea");
			area.AxisX = this.CreateAxisX(this.SelectedChartSpan);
			area.AxisY = this.CreateAxisY(200);
			area.AxisY2 = this.CreateAxisY(20);

            this.ResourceChart.Legends.Clear();
			var legend = this.ResourceChart.Legends.Add("ResourceLegend");
			legend.Font = this.Font;


            this.ResourceChart.Series.Clear();

			var fuel = this.ResourceChart.Series.Add("ResourceSeries_Fuel");
			var ammo = this.ResourceChart.Series.Add("ResourceSeries_Ammo");
			var steel = this.ResourceChart.Series.Add("ResourceSeries_Steel");
			var bauxite = this.ResourceChart.Series.Add("ResourceSeries_Bauxite");
			var instantRepair = this.ResourceChart.Series.Add("ResourceSeries_InstantRepair");

			var setSeries = new Action<Series>(s =>
			{
				s.ChartType = SeriesChartType.Area;
				//s.SetCustomProperty( "PointWidth", "1.0" );		//棒グラフの幅
				//s.Enabled = false;	//表示するか
				s.Font = this.Font;
				s.XValueType = ChartValueType.DateTime;
			});

			setSeries(fuel);
			fuel.Color = Color.FromArgb(64, 0, 128, 0);
			fuel.BorderColor = Color.FromArgb(255, 0, 128, 0);
			fuel.LegendText = "연료";

			setSeries(ammo);
			ammo.Color = Color.FromArgb(64, 255, 128, 0);
			ammo.BorderColor = Color.FromArgb(255, 255, 128, 0);
			ammo.LegendText = "탄약";

			setSeries(steel);
			steel.Color = Color.FromArgb(64, 64, 64, 64);
			steel.BorderColor = Color.FromArgb(255, 64, 64, 64);
			steel.LegendText = "강재";

			setSeries(bauxite);
			bauxite.Color = Color.FromArgb(64, 255, 0, 0);
			bauxite.BorderColor = Color.FromArgb(255, 255, 0, 0);
			bauxite.LegendText = "보키";

			setSeries(instantRepair);
			instantRepair.Color = Color.FromArgb(64, 32, 128, 255);
			instantRepair.BorderColor = Color.FromArgb(255, 32, 128, 255);
			instantRepair.LegendText = "고속수복재";
			instantRepair.YAxisType = AxisType.Secondary;


			//データ設定
			{
				var record = this.GetRecords();

				ResourceRecord.ResourceElement prev = null;

				if (record.Any())
				{
					prev = record.First();
					foreach (var r in record)
					{

						if (this.ShouldSkipRecord(r.Date - prev.Date))
							continue;

						double[] ys = new double[] {
							r.Fuel - prev.Fuel,
							r.Ammo - prev.Ammo,
							r.Steel - prev.Steel,
							r.Bauxite - prev.Bauxite,
							r.InstantRepair - prev.InstantRepair };

						if (this.Menu_Option_DivideByDay.Checked)
						{
							for (int i = 0; i < 4; i++)
								ys[i] /= Math.Max((r.Date - prev.Date).TotalDays, 1.0 / 1440.0);
						}

						fuel.Points.AddXY(prev.Date.ToOADate(), ys[0]);
						ammo.Points.AddXY(prev.Date.ToOADate(), ys[1]);
						steel.Points.AddXY(prev.Date.ToOADate(), ys[2]);
						bauxite.Points.AddXY(prev.Date.ToOADate(), ys[3]);
						instantRepair.Points.AddXY(prev.Date.ToOADate(), ys[4]);

						fuel.Points.AddXY(r.Date.ToOADate(), ys[0]);
						ammo.Points.AddXY(r.Date.ToOADate(), ys[1]);
						steel.Points.AddXY(r.Date.ToOADate(), ys[2]);
						bauxite.Points.AddXY(r.Date.ToOADate(), ys[3]);
						instantRepair.Points.AddXY(r.Date.ToOADate(), ys[4]);


						prev = r;
					}
				}
			}


            this.SetYBounds();
		}



		private void SetMaterialChart()
		{

            this.ResourceChart.ChartAreas.Clear();
			var area = this.ResourceChart.ChartAreas.Add("ResourceChartArea");
			area.AxisX = this.CreateAxisX(this.SelectedChartSpan);
			area.AxisY = this.CreateAxisY(50, 200);

            this.ResourceChart.Legends.Clear();
			var legend = this.ResourceChart.Legends.Add("ResourceLegend");
			legend.Font = this.Font;


            this.ResourceChart.Series.Clear();

			var instantConstruction = this.ResourceChart.Series.Add("ResourceSeries_InstantConstruction");
			var instantRepair = this.ResourceChart.Series.Add("ResourceSeries_InstantRepair");
			var developmentMaterial = this.ResourceChart.Series.Add("ResourceSeries_DevelopmentMaterial");
			var moddingMaterial = this.ResourceChart.Series.Add("ResourceSeries_ModdingMaterial");

			var setSeries = new Action<Series>(s =>
			{
				s.ChartType = SeriesChartType.Line;
				s.Font = this.Font;
				s.XValueType = ChartValueType.DateTime;
			});

			setSeries(instantConstruction);
			instantConstruction.Color = Color.FromArgb(255, 128, 0);
			instantConstruction.LegendText = "고속건조재";

			setSeries(instantRepair);
			instantRepair.Color = Color.FromArgb(0, 128, 0);
			instantRepair.LegendText = "고속수복재";

			setSeries(developmentMaterial);
			developmentMaterial.Color = Color.FromArgb(0, 0, 255);
			developmentMaterial.LegendText = "개발자재";

			setSeries(moddingMaterial);
			moddingMaterial.Color = Color.FromArgb(64, 64, 64);
			moddingMaterial.LegendText = "개수자재";


			//データ設定
			{
				var record = this.GetRecords();

				if (record.Any())
				{
					var prev = record.First();
					foreach (var r in record)
					{

						if (this.ShouldSkipRecord(r.Date - prev.Date))
							continue;

						instantConstruction.Points.AddXY(r.Date.ToOADate(), r.InstantConstruction);
						instantRepair.Points.AddXY(r.Date.ToOADate(), r.InstantRepair);
						developmentMaterial.Points.AddXY(r.Date.ToOADate(), r.DevelopmentMaterial);
						moddingMaterial.Points.AddXY(r.Date.ToOADate(), r.ModdingMaterial);

						prev = r;
					}
				}


				if (instantConstruction.Points.Count > 0)
				{
					int min = (int)new[] { instantConstruction.Points.Min(p => p.YValues[0]), instantRepair.Points.Min(p => p.YValues[0]), developmentMaterial.Points.Min(p => p.YValues[0]), moddingMaterial.Points.Min(p => p.YValues[0]) }.Min();
					area.AxisY.Minimum = Math.Floor(min / 200.0) * 200;

					int max = (int)new[] { instantConstruction.Points.Max(p => p.YValues[0]), instantRepair.Points.Max(p => p.YValues[0]), developmentMaterial.Points.Max(p => p.YValues[0]), moddingMaterial.Points.Max(p => p.YValues[0]) }.Max();
					area.AxisY.Maximum = Math.Ceiling(max / 200.0) * 200;
				}
			}


            this.SetYBounds();
		}


		private void SetMateialDiffChart()
		{

            this.ResourceChart.ChartAreas.Clear();
			var area = this.ResourceChart.ChartAreas.Add("ResourceChartArea");
			area.AxisX = this.CreateAxisX(this.SelectedChartSpan);
			area.AxisY = this.CreateAxisY(5, 20);

            this.ResourceChart.Legends.Clear();
			var legend = this.ResourceChart.Legends.Add("ResourceLegend");
			legend.Font = this.Font;


            this.ResourceChart.Series.Clear();

			var instantConstruction = this.ResourceChart.Series.Add("ResourceSeries_InstantConstruction");
			var instantRepair = this.ResourceChart.Series.Add("ResourceSeries_InstantRepair");
			var developmentMaterial = this.ResourceChart.Series.Add("ResourceSeries_DevelopmentMaterial");
			var moddingMaterial = this.ResourceChart.Series.Add("ResourceSeries_ModdingMaterial");

			var setSeries = new Action<Series>(s =>
			{
				s.ChartType = SeriesChartType.Area;
				//s.SetCustomProperty( "PointWidth", "1.0" );		//棒グラフの幅
				//s.Enabled = false;	//表示するか
				s.Font = this.Font;
				s.XValueType = ChartValueType.DateTime;
			});

			setSeries(instantConstruction);
			instantConstruction.Color = Color.FromArgb(64, 255, 128, 0);
			instantConstruction.BorderColor = Color.FromArgb(255, 255, 128, 0);
			instantConstruction.LegendText = "고속건조재";

			setSeries(instantRepair);
			instantRepair.Color = Color.FromArgb(64, 0, 128, 0);
			instantRepair.BorderColor = Color.FromArgb(255, 0, 128, 0);
			instantRepair.LegendText = "고속수복재";

			setSeries(developmentMaterial);
			developmentMaterial.Color = Color.FromArgb(64, 0, 0, 255);
			developmentMaterial.BorderColor = Color.FromArgb(255, 0, 0, 255);
			developmentMaterial.LegendText = "개발자재";

			setSeries(moddingMaterial);
			moddingMaterial.Color = Color.FromArgb(64, 64, 64, 64);
			moddingMaterial.BorderColor = Color.FromArgb(255, 64, 64, 64);
			moddingMaterial.LegendText = "개수자재";


			//データ設定
			{
				var record = this.GetRecords();

				ResourceRecord.ResourceElement prev = null;

				if (record.Any())
				{
					prev = record.First();
					foreach (var r in record)
					{

						if (this.ShouldSkipRecord(r.Date - prev.Date))
							continue;

						double[] ys = new double[] {
							r.InstantConstruction - prev.InstantConstruction ,
							r.InstantRepair - prev.InstantRepair,
							r.DevelopmentMaterial - prev.DevelopmentMaterial ,
							r.ModdingMaterial - prev.ModdingMaterial };

						if (this.Menu_Option_DivideByDay.Checked)
						{
							for (int i = 0; i < 4; i++)
								ys[i] /= Math.Max((r.Date - prev.Date).TotalDays, 1.0 / 1440.0);
						}

						instantConstruction.Points.AddXY(prev.Date.ToOADate(), ys[0]);
						instantRepair.Points.AddXY(prev.Date.ToOADate(), ys[1]);
						developmentMaterial.Points.AddXY(prev.Date.ToOADate(), ys[2]);
						moddingMaterial.Points.AddXY(prev.Date.ToOADate(), ys[3]);

						instantConstruction.Points.AddXY(r.Date.ToOADate(), ys[0]);
						instantRepair.Points.AddXY(r.Date.ToOADate(), ys[1]);
						developmentMaterial.Points.AddXY(r.Date.ToOADate(), ys[2]);
						moddingMaterial.Points.AddXY(r.Date.ToOADate(), ys[3]);

						prev = r;
					}
				}


				if (instantConstruction.Points.Count > 0)
				{
					int min = (int)new[] { instantConstruction.Points.Min(p => p.YValues[0]), instantRepair.Points.Min(p => p.YValues[0]), developmentMaterial.Points.Min(p => p.YValues[0]), moddingMaterial.Points.Min(p => p.YValues[0]) }.Min();
					area.AxisY.Minimum = Math.Floor(min / 20.0) * 20;

					int max = (int)new[] { instantConstruction.Points.Max(p => p.YValues[0]), instantRepair.Points.Max(p => p.YValues[0]), developmentMaterial.Points.Max(p => p.YValues[0]), moddingMaterial.Points.Max(p => p.YValues[0]) }.Max();
					area.AxisY.Maximum = Math.Ceiling(max / 20.0) * 20;
				}
			}


            this.SetYBounds();
		}



		private void SetExperienceChart()
		{

            this.ResourceChart.ChartAreas.Clear();
			var area = this.ResourceChart.ChartAreas.Add("ResourceChartArea");
			area.AxisX = this.CreateAxisX(this.SelectedChartSpan);
			area.AxisY = this.CreateAxisY(20000);

            this.ResourceChart.Legends.Clear();
			var legend = this.ResourceChart.Legends.Add("ResourceLegend");
			legend.Font = this.Font;


            this.ResourceChart.Series.Clear();

			var exp = this.ResourceChart.Series.Add("ResourceSeries_Experience");

			var setSeries = new Action<Series>(s =>
			{
				s.ChartType = SeriesChartType.Line;
				s.Font = this.Font;
				s.XValueType = ChartValueType.DateTime;
			});

			setSeries(exp);
			exp.Color = Color.FromArgb(0, 0, 255);
			exp.LegendText = "제독경험치";


			//データ設定
			{
				var record = this.GetRecords();

				if (record.Any())
				{
					var prev = record.First();
					foreach (var r in record)
					{

						if (this.ShouldSkipRecord(r.Date - prev.Date))
							continue;

						exp.Points.AddXY(r.Date.ToOADate(), r.HQExp);
						prev = r;
					}
				}


				if (exp.Points.Count > 0)
				{
					int min = (int)exp.Points.Min(p => p.YValues[0]);
					area.AxisY.Minimum = Math.Floor(min / 100000.0) * 100000;

					int max = (int)exp.Points.Max(p => p.YValues[0]);
					area.AxisY.Maximum = Math.Ceiling(max / 100000.0) * 100000;
				}
			}


            this.SetYBounds();
		}


		private void SetExperienceDiffChart()
		{

            this.ResourceChart.ChartAreas.Clear();
			var area = this.ResourceChart.ChartAreas.Add("ResourceChartArea");
			area.AxisX = this.CreateAxisX(this.SelectedChartSpan);
			area.AxisY = this.CreateAxisY(2000);

            this.ResourceChart.Legends.Clear();
			var legend = this.ResourceChart.Legends.Add("ResourceLegend");
			legend.Font = this.Font;


            this.ResourceChart.Series.Clear();

			var exp = this.ResourceChart.Series.Add("ResourceSeries_Experience");


			var setSeries = new Action<Series>(s =>
			{
				s.ChartType = SeriesChartType.Area;
				//s.SetCustomProperty( "PointWidth", "1.0" );		//棒グラフの幅
				//s.Enabled = false;	//表示するか
				s.Font = this.Font;
				s.XValueType = ChartValueType.DateTime;
			});

			setSeries(exp);
			exp.Color = Color.FromArgb(64, 0, 0, 255);
			exp.BorderColor = Color.FromArgb(255, 0, 0, 255);
			exp.LegendText = "제독경험치";


			//データ設定
			{
				var record = this.GetRecords();

				ResourceRecord.ResourceElement prev = null;

				if (record.Any())
				{
					prev = record.First();
					foreach (var r in record)
					{

						if (this.ShouldSkipRecord(r.Date - prev.Date))
							continue;

						double ys = r.HQExp - prev.HQExp;

						if (this.Menu_Option_DivideByDay.Checked)
							ys /= Math.Max((r.Date - prev.Date).TotalDays, 1.0 / 1440.0);

						exp.Points.AddXY(prev.Date.ToOADate(), ys);

						exp.Points.AddXY(r.Date.ToOADate(), ys);

						prev = r;
					}
				}


				if (exp.Points.Count > 0)
				{
					int min = (int)exp.Points.Min(p => p.YValues[0]);
					area.AxisY.Minimum = Math.Floor(min / 10000.0) * 10000;

					int max = (int)exp.Points.Max(p => p.YValues[0]);
					area.AxisY.Maximum = Math.Ceiling(max / 10000.0) * 10000;
				}
			}


            this.SetYBounds();
		}


		private Axis CreateAxisX(ChartSpan span)
		{

			Axis axis = new Axis();

			switch (span)
			{
				case ChartSpan.Day:
					axis.Interval = 2;
					axis.IntervalOffsetType = DateTimeIntervalType.Hours;
					axis.IntervalType = DateTimeIntervalType.Hours;
					axis.LabelStyle.Format = "MM/dd HH:mm";
					break;
				case ChartSpan.Week:
				case ChartSpan.WeekFirst:
					axis.Interval = 12;
					axis.IntervalOffsetType = DateTimeIntervalType.Hours;
					axis.IntervalType = DateTimeIntervalType.Hours;
					axis.LabelStyle.Format = "MM/dd HH:mm";
					break;
				case ChartSpan.Month:
				case ChartSpan.MonthFirst:
					axis.Interval = 3;
					axis.IntervalOffsetType = DateTimeIntervalType.Days;
					axis.IntervalType = DateTimeIntervalType.Days;
					axis.LabelStyle.Format = "yyyy/MM/dd";
					break;
				case ChartSpan.Season:
				case ChartSpan.SeasonFirst:
					axis.Interval = 7;
					axis.IntervalOffsetType = DateTimeIntervalType.Days;
					axis.IntervalType = DateTimeIntervalType.Days;
					axis.LabelStyle.Format = "yyyy/MM/dd";
					break;
				case ChartSpan.Year:
				case ChartSpan.YearFirst:
				case ChartSpan.All:
					axis.Interval = 1;
					axis.IntervalOffsetType = DateTimeIntervalType.Months;
					axis.IntervalType = DateTimeIntervalType.Months;
					axis.LabelStyle.Format = "yyyy/MM/dd";
					break;
			}

			axis.LabelStyle.Font = this.Font;
			axis.MajorGrid.LineColor = Color.FromArgb(192, 192, 192);

			return axis;
		}


		private Axis CreateAxisY(int minorInterval, int majorInterval)
		{

			Axis axis = new Axis();

			axis.LabelStyle.Font = this.Font;
			axis.IsStartedFromZero = true;
			axis.Interval = majorInterval;
			axis.MajorGrid.LineColor = Color.FromArgb(192, 192, 192);
			axis.MinorGrid.Enabled = true;
			axis.MinorGrid.Interval = minorInterval;
			axis.MinorGrid.LineDashStyle = ChartDashStyle.Dash;
			axis.MinorGrid.LineColor = Color.FromArgb(224, 224, 224);

			return axis;
		}

		private Axis CreateAxisY(int interval)
		{
			return this.CreateAxisY(interval, interval * 5);
		}



		private void ResourceChart_GetToolTipText(object sender, ToolTipEventArgs e)
		{

			if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
			{
				var dp = e.HitTestResult.Series.Points[e.HitTestResult.PointIndex];

				switch (this.SelectedChartType)
				{
					case ChartType.Resource:
					case ChartType.Material:
					case ChartType.Experience:
						e.Text = string.Format("{0:yyyy\\/MM\\/dd HH\\:mm}\n{1} {2:F0}",
							DateTime.FromOADate(dp.XValue),
							e.HitTestResult.Series.LegendText,
							dp.YValues[0]);
						break;
					case ChartType.ResourceDiff:
					case ChartType.MaterialDiff:
					case ChartType.ExperienceDiff:
						e.Text = string.Format("{0:yyyy\\/MM\\/dd HH\\:mm}\n{1} {2:+0;-0;±0}{3}",
							DateTime.FromOADate(dp.XValue),
							e.HitTestResult.Series.LegendText,
							dp.YValues[0],
                            this.Menu_Option_DivideByDay.Checked ? " / day" : "");
						break;
				}
			}

		}


		private void SwitchMenuStrip(ToolStripMenuItem parent, int index)
		{

			//すべての子アイテムに対して
			var items = parent.DropDownItems.OfType<ToolStripMenuItem>();
			int c = 0;

			foreach (var item in items)
			{
				if (index == c)
				{

					item.Checked = true;

				}
				else
				{

					item.Checked = false;
				}

				c++;
			}

			parent.Tag = index;
		}


		private int GetSelectedMenuStripIndex(ToolStripMenuItem parent)
		{
			return parent.Tag as int? ?? -1;
		}


		private void UpdateChart()
		{

			switch (this.SelectedChartType)
			{
				case ChartType.Resource:
                    this.SetResourceChart();
					break;
				case ChartType.ResourceDiff:
                    this.SetResourceDiffChart();
					break;
				case ChartType.Material:
                    this.SetMaterialChart();
					break;
				case ChartType.MaterialDiff:
                    this.SetMateialDiffChart();
					break;
				case ChartType.Experience:
                    this.SetExperienceChart();
					break;
				case ChartType.ExperienceDiff:
                    this.SetExperienceDiffChart();
					break;
			}

		}

		private IEnumerable<ResourceRecord.ResourceElement> GetRecords()
		{

			var border = DateTime.MinValue;
			var now = DateTime.Now;

			switch (this.SelectedChartSpan)
			{
				case ChartSpan.Day:
					border = now.AddDays(-1);
					break;
				case ChartSpan.Week:
					border = now.AddDays(-7);
					break;
				case ChartSpan.Month:
					border = now.AddMonths(-1);
					break;
				case ChartSpan.Season:
					border = now.AddMonths(-3);
					break;
				case ChartSpan.Year:
					border = now.AddYears(-1);
					break;

				case ChartSpan.WeekFirst:
					border = now.AddDays(now.DayOfWeek == DayOfWeek.Sunday ? -6 : (1 - (int)now.DayOfWeek));
					break;
				case ChartSpan.MonthFirst:
					border = new DateTime(now.Year, now.Month, 1);
					break;
				case ChartSpan.SeasonFirst:
					{
						int m = now.Month / 3 * 3;
						if (m == 0)
							m = 12;
						border = new DateTime(now.Year - (now.Month < 3 ? 1 : 0), m, 1);
					}
					break;
				case ChartSpan.YearFirst:
					border = new DateTime(now.Year, 1, 1);
					break;
			}

			foreach (var r in RecordManager.Instance.Resource.Record)
			{
				if (r.Date >= border)
					yield return r;
			}

			var material = KCDatabase.Instance.Material;
			var admiral = KCDatabase.Instance.Admiral;
			if (material.IsAvailable && admiral.IsAvailable)
			{
				yield return new ResourceRecord.ResourceElement(
					material.Fuel, material.Ammo, material.Steel, material.Bauxite,
					material.InstantConstruction, material.InstantRepair, material.DevelopmentMaterial, material.ModdingMaterial,
					admiral.Level, admiral.Exp);
			}

		}


		private bool ShouldSkipRecord(TimeSpan span)
		{

			if (this.Menu_Option_ShowAllData.Checked)
				return false;

			if (span.Ticks == 0)        //初回のデータ( prev == First )は無視しない
				return false;

			switch (this.SelectedChartSpan)
			{
				case ChartSpan.Day:
				case ChartSpan.Week:
				case ChartSpan.WeekFirst:
				default:
					return false;

				case ChartSpan.Month:
				case ChartSpan.MonthFirst:
					return span.TotalHours < 12.0;

				case ChartSpan.Season:
				case ChartSpan.SeasonFirst:
				case ChartSpan.Year:
				case ChartSpan.YearFirst:
				case ChartSpan.All:
					return span.TotalDays < 1.0;
			}

		}



		private void SetYBounds(double min, double max)
		{

			int order = (int)Math.Log10(Math.Max(max - min, 1));
			double powered = Math.Pow(10, order);
			double unitbase = Math.Round((max - min) / powered);
			double unit = powered * (
				unitbase < 2 ? 0.2 :
				unitbase < 5 ? 0.5 :
				unitbase < 7 ? 1.0 : 2.0);

            this.ResourceChart.ChartAreas[0].AxisY.Minimum = Math.Floor(min / unit) * unit;
            this.ResourceChart.ChartAreas[0].AxisY.Maximum = Math.Ceiling(max / unit) * unit;

            this.ResourceChart.ChartAreas[0].AxisY.Interval = unit;
            this.ResourceChart.ChartAreas[0].AxisY.MinorGrid.Interval = unit / 2;

			if (this.ResourceChart.Series.Where(s => s.Enabled).Any(s => s.YAxisType == AxisType.Secondary)) {
                this.ResourceChart.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
				if (this.ResourceChart.Series.Count(s => s.Enabled) == 1) {
                    this.ResourceChart.ChartAreas[0].AxisY2.MajorGrid.Enabled = true;
                    this.ResourceChart.ChartAreas[0].AxisY2.MinorGrid.Enabled = true;
				} else {
                    this.ResourceChart.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
                    this.ResourceChart.ChartAreas[0].AxisY2.MinorGrid.Enabled = false;
				}
                this.ResourceChart.ChartAreas[0].AxisY2.Minimum = this.ResourceChart.ChartAreas[0].AxisY.Minimum / 100;
                this.ResourceChart.ChartAreas[0].AxisY2.Maximum = this.ResourceChart.ChartAreas[0].AxisY.Maximum / 100;
                this.ResourceChart.ChartAreas[0].AxisY2.Interval = unit / 100;
                this.ResourceChart.ChartAreas[0].AxisY2.MinorGrid.Interval = unit / 200;
			} else {
                this.ResourceChart.ChartAreas[0].AxisY2.Enabled = AxisEnabled.False;
			}

		}

		private void SetYBounds()
		{
            this.SetYBounds(
				!this.ResourceChart.Series.Any(s => s.Enabled) || this.SelectedChartType == ChartType.ExperienceDiff ? 0 : this.ResourceChart.Series.Where(s => s.Enabled).Select(s => s.YAxisType == AxisType.Secondary ? s.Points.Min(p => p.YValues[0] * 100) : s.Points.Min(p => p.YValues[0])).Min(),
				!this.ResourceChart.Series.Any(s => s.Enabled) ? 0 : this.ResourceChart.Series.Where(s => s.Enabled).Select(s => s.YAxisType == AxisType.Secondary ? s.Points.Max(p => p.YValues[0] * 100) : s.Points.Max(p => p.YValues[0])).Max()
				);
		}


		private void ResourceChart_CustomizeLegend(object sender, CustomizeLegendEventArgs e)
		{

			e.LegendItems.Clear();

			foreach (var series in this.ResourceChart.Series)
			{

				var legendItem = new LegendItem
				{
					SeriesName = series.Name,
					ImageStyle = LegendImageStyle.Rectangle,
					BorderColor = Color.Empty,
					Name = series.Name + "_legendItem"
				};

				legendItem.Cells.Add(LegendCellType.SeriesSymbol, "", ContentAlignment.MiddleCenter);
				legendItem.Cells.Add(LegendCellType.Text, series.LegendText, ContentAlignment.MiddleLeft);

				var col = series.BorderColor != Color.Empty ? series.BorderColor : series.Color;

				if (series.Enabled)
				{
					legendItem.Color = col;
					legendItem.Cells[1].ForeColor = SystemColors.ControlText;
				}
				else
				{
					legendItem.Color = Color.FromArgb(col.A / 4, col);
					legendItem.Cells[1].ForeColor = SystemColors.GrayText;
				}
				e.LegendItems.Add(legendItem);
			}
		}


		private void ResourceChart_MouseDown(object sender, MouseEventArgs e)
		{

			if (e.Button != System.Windows.Forms.MouseButtons.Left)
				return;

			var hittest = this.ResourceChart.HitTest(e.X, e.Y, ChartElementType.LegendItem);

			if (hittest.Object != null)
			{

				var legend = (LegendItem)hittest.Object;
                this.ResourceChart.Series[legend.SeriesName].Enabled ^= true;
			}

            this.SetYBounds();
		}






		private void Menu_Graph_Resource_Click(object sender, EventArgs e)
		{
            this.SwitchMenuStrip(this.Menu_Graph, 0);
            this.UpdateChart();
		}

		private void Menu_Graph_ResourceDiff_Click(object sender, EventArgs e)
		{
            this.SwitchMenuStrip(this.Menu_Graph, 1);
            this.UpdateChart();
		}

		private void Menu_Graph_Material_Click(object sender, EventArgs e)
		{
            this.SwitchMenuStrip(this.Menu_Graph, 2);
            this.UpdateChart();
		}

		private void Menu_Graph_MaterialDiff_Click(object sender, EventArgs e)
		{
            this.SwitchMenuStrip(this.Menu_Graph, 3);
            this.UpdateChart();
		}

		private void Menu_Graph_Experience_Click(object sender, EventArgs e)
		{
            this.SwitchMenuStrip(this.Menu_Graph, 4);
            this.UpdateChart();
		}

		private void Menu_Graph_ExperienceDiff_Click(object sender, EventArgs e)
		{
            this.SwitchMenuStrip(this.Menu_Graph, 5);
            this.UpdateChart();
		}


		private void Menu_Span_Menu_Click(object sender, EventArgs e)
		{
            this.SwitchMenuStrip(this.Menu_Span, (int)((ToolStripMenuItem)sender).Tag);
            this.UpdateChart();
		}


		private void Menu_Option_ShowAllData_Click(object sender, EventArgs e)
		{
            this.UpdateChart();
		}

		private void Menu_Option_DivideByDay_Click(object sender, EventArgs e)
		{
            this.UpdateChart();
		}



		private void Menu_File_SaveImage_Click(object sender, EventArgs e)
		{

			if (this.SaveImageDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				try
				{

                    this.ResourceChart.SaveImage(this.SaveImageDialog.FileName, ChartImageFormat.Png);

				}
				catch (Exception ex)
				{
					Utility.ErrorReporter.SendErrorReport(ex, "자원 차트 이미지 저장에 실패했습니다.");
				}
			}
		}



		private void DialogResourceChart_FormClosed(object sender, FormClosedEventArgs e)
		{
			ResourceManager.DestroyIcon(this.Icon);
		}


	}

}
