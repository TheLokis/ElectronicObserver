using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility;
using ElectronicObserver.Window.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ElectronicObserver.Window
{

	public partial class FormQuest : DockContent
	{

		private DataGridViewCellStyle CSDefaultLeft, CSDefaultCenter;
		private DataGridViewCellStyle[] CSCategories;
		private bool IsLoaded = false;

		public FormQuest(FormMain parent)
		{
            this.InitializeComponent();

            ControlHelper.SetDoubleBuffered(this.QuestView);

            this.ConfigurationChanged();


            #region set cellstyle

            this.CSDefaultLeft = new DataGridViewCellStyle
			{
				Alignment = DataGridViewContentAlignment.MiddleLeft
			};

            this.CSDefaultLeft.BackColor =
            this.CSDefaultLeft.SelectionBackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            this.CSDefaultLeft.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.CSDefaultLeft.SelectionForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.CSDefaultLeft.WrapMode = DataGridViewTriState.False;


            this.CSDefaultCenter = new DataGridViewCellStyle(this.CSDefaultLeft)
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
            };
            this.CSCategories = new DataGridViewCellStyle[10];
			for (int i = 0; i < this.CSCategories.Length; i++)
			{
                this.CSCategories[i] = new DataGridViewCellStyle(this.CSDefaultCenter);

                Color c;
				switch (i + 1)
				{
					case 1:     //編成
						c = Color.FromArgb(0xAA, 0xFF, 0xAA);
						break;
					case 2:     //出撃
						c = Color.FromArgb(0xFF, 0xCC, 0xCC);
						break;
					case 3:     //演習
						c = Color.FromArgb(0xDD, 0xFF, 0xAA);
						break;
					case 4:     //遠征
						c = Color.FromArgb(0xCC, 0xFF, 0xFF);
						break;
					case 5:     //補給/入渠
						c = Color.FromArgb(0xFF, 0xFF, 0xCC);
						break;
					case 6:     //工廠
						c = Color.FromArgb(0xDD, 0xCC, 0xBB);
						break;
					case 7:     //改装
						c = Color.FromArgb(0xDD, 0xCC, 0xFF);
						break;
					case 8:     //出撃(2)
						c = Color.FromArgb(0xFF, 0xCC, 0xCC);
						break;
                    case 9:     //出撃(3)
                        c = Color.FromArgb(0xFF, 0xCC, 0xCC);
                        break;
                    case 10:     //その他
                    default:
						c = this.CSDefaultCenter.BackColor;
						break;
				}

                this.CSCategories[i].ForeColor =
                this.CSCategories[i].SelectionForeColor = SystemColors.ControlText;
                this.CSCategories[i].BackColor =
                this.CSCategories[i].SelectionBackColor = c;
            }

            for (int i = 0; i < this.CSCategories.Length; i++)
            {

            }

            this.QuestView.DefaultCellStyle = this.CSDefaultCenter;
            this.QuestView_Category.DefaultCellStyle = this.CSCategories[this.CSCategories.Length - 1];
            this.QuestView_Name.DefaultCellStyle = this.CSDefaultLeft;
            this.QuestView_Progress.DefaultCellStyle = this.CSDefaultLeft;

			#endregion


			SystemEvents.SystemShuttingDown += this.SystemEvents_SystemShuttingDown;
		}



		private void FormQuest_Load(object sender, EventArgs e)
		{

			/*/
			APIObserver o = APIObserver.Instance;

			APIReceivedEventHandler rec = ( string apiname, dynamic data ) => Invoke( new APIReceivedEventHandler( APIUpdated ), apiname, data );

			o.APIList["api_req_quest/clearitemget"].RequestReceived += rec;

			o.APIList["api_get_member/questlist"].ResponseReceived += rec;
			//*/

			KCDatabase.Instance.Quest.QuestUpdated += this.Updated;


            this.ClearQuestView();

			try
			{
				int sort = Utility.Configuration.Config.FormQuest.SortParameter;

                this.QuestView.Sort(this.QuestView.Columns[sort >> 1], (sort & 1) == 0 ? ListSortDirection.Ascending : ListSortDirection.Descending);

			}
			catch (Exception)
			{

                this.QuestView.Sort(this.QuestView_Name, ListSortDirection.Ascending);
			}


			Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormQuest]);

            this.IsLoaded = true;
		}


		void ConfigurationChanged()
		{

			var c = Utility.Configuration.Config;

            this.QuestView.Font = this.Font = c.UI.MainFont;
            this.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            this.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.QuestView.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.QuestView.BackgroundColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);

            this.MenuMain_ShowRunningOnly.Checked = c.FormQuest.ShowRunningOnly;
            this.MenuMain_ShowOnce.Checked = c.FormQuest.ShowOnce;
            this.MenuMain_ShowDaily.Checked = c.FormQuest.ShowDaily;
            this.MenuMain_ShowWeekly.Checked = c.FormQuest.ShowWeekly;
            this.MenuMain_ShowMonthly.Checked = c.FormQuest.ShowMonthly;
            this.MenuMain_ShowOther.Checked = c.FormQuest.ShowOther;

			if (c.FormQuest.ColumnFilter == null || ((List<bool>)c.FormQuest.ColumnFilter).Count != this.QuestView.Columns.Count)
			{
				c.FormQuest.ColumnFilter = Enumerable.Repeat(true, this.QuestView.Columns.Count).ToList();
			}
			if (c.FormQuest.ColumnWidth == null || ((List<int>)c.FormQuest.ColumnWidth).Count != this.QuestView.Columns.Count)
			{
				c.FormQuest.ColumnWidth = this.QuestView.Columns.Cast<DataGridViewColumn>().Select(column => column.Width).ToList();
			}
			{
				List<bool> list = c.FormQuest.ColumnFilter;
				List<int> width = c.FormQuest.ColumnWidth;

				for (int i = 0; i < this.QuestView.Columns.Count; i++)
				{
                    this.QuestView.Columns[i].Visible =
					((ToolStripMenuItem)this.MenuMain_ColumnFilter.DropDownItems[i]).Checked = list[i];
                    this.QuestView.Columns[i].Width = width[i];
				}
			}

			foreach (DataGridViewColumn column in this.QuestView.Columns)
			{
				column.SortMode = c.FormQuest.AllowUserToSortRows ? DataGridViewColumnSortMode.Automatic : DataGridViewColumnSortMode.NotSortable;
			}

			if (c.UI.IsLayoutFixed)
			{
                this.QuestView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                this.QuestView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			}
			else
			{
                this.QuestView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
                this.QuestView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			}

			foreach (DataGridViewRow row in this.QuestView.Rows)
			{
				row.Height = 21;
			}

            this.Updated();

		}


		void SystemEvents_SystemShuttingDown()
		{

			try
			{

				if (this.QuestView.SortedColumn != null)
					Utility.Configuration.Config.FormQuest.SortParameter = this.QuestView.SortedColumn.Index << 1 | (this.QuestView.SortOrder == SortOrder.Ascending ? 0 : 1);

				Utility.Configuration.Config.FormQuest.ColumnWidth = this.QuestView.Columns.Cast<DataGridViewColumn>().Select(c => c.Width).ToList();

			}
			catch (Exception)
			{
				// *ぷちっ*				
			}
		}



		void Updated()
		{

			if (!KCDatabase.Instance.Quest.IsLoaded) return;

            this.QuestView.SuspendLayout();

            this.QuestView.Rows.Clear();

			foreach (var q in KCDatabase.Instance.Quest.Quests.Values)
			{

				if (this.MenuMain_ShowRunningOnly.Checked && !(q.State == 2 || q.State == 3))
					continue;

				switch (q.Type)
				{
					case 1:
						if (!this.MenuMain_ShowDaily.Checked) continue;
						break;
					case 2:
						if (!this.MenuMain_ShowWeekly.Checked) continue;
						break;
					case 3:
						if (!this.MenuMain_ShowMonthly.Checked) continue;
						break;
					case 4:
					default:
						if (!this.MenuMain_ShowOnce.Checked) continue;
						break;
					case 5:
						if (q.QuestID == 211 || q.QuestID == 212)
						{   // 空母3か輸送5
							if (!this.MenuMain_ShowDaily.Checked) continue;
						}
						else
						{
							if (!this.MenuMain_ShowOther.Checked) continue;
						}
						break;
				}


				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(this.QuestView);
				row.Height = 21;
				row.Cells[this.QuestView_State.Index].Value		= (q.State == 3) ? ((bool?)null) : (q.State == 2);
                row.Cells[this.QuestView_Type.Index].Value		= q.LabelType >= 100 ? q.LabelType : q.Type;
                row.Cells[this.QuestView_Category.Index].Value	= q.Category;
				row.Cells[this.QuestView_Category.Index].Style	= this.CSCategories[Math.Min(q.Category - 1, 8 - 1)]; //아마도이거
				row.Cells[this.QuestView_Name.Index].Value		= q.QuestID;
				{
					var progress = KCDatabase.Instance.QuestProgress[q.QuestID];
					row.Cells[this.QuestView_Name.Index].ToolTipText = $"{q.QuestID} : {q.Name}\r\n{q.Description}\r\n{progress?.GetClearCondition() ?? ""}";
				}
				{
					string value;
					double tag;

					if (q.State == 3)
					{
						value = "달성!";
						tag = 1.0;

					}
					else
					{

						if (KCDatabase.Instance.QuestProgress.Progresses.ContainsKey(q.QuestID))
						{
							var p = KCDatabase.Instance.QuestProgress[q.QuestID];

							value = p.ToString();
							tag = p.ProgressPercentage;

						}
						else
						{

							switch (q.Progress)
							{
								case 0:
									value = "-";
									tag = 0.0;
									break;
								case 1:
									value = "50%이상";
									tag = 0.5;
									break;
								case 2:
									value = "80%이상";
									tag = 0.8;
									break;
								default:
									value = "???";
									tag = 0.0;
									break;
							}
						}
					}

					row.Cells[this.QuestView_Progress.Index].Value = value;
					row.Cells[this.QuestView_Progress.Index].Tag = tag;
				}

                this.QuestView.Rows.Add(row);
			}


			if (KCDatabase.Instance.Quest.Quests.Count < KCDatabase.Instance.Quest.Count)
			{
				int index = this.QuestView.Rows.Add();
                this.QuestView.Rows[index].Cells[this.QuestView_State.Index].Value = null;
                this.QuestView.Rows[index].Cells[this.QuestView_Name.Index].Value = string.Format("(받지않은임무 x {0})", (KCDatabase.Instance.Quest.Count - KCDatabase.Instance.Quest.Quests.Count));
			}

			if (KCDatabase.Instance.Quest.Quests.Count == 0)
			{
				int index = this.QuestView.Rows.Add();
                this.QuestView.Rows[index].Cells[this.QuestView_State.Index].Value = null;
                this.QuestView.Rows[index].Cells[this.QuestView_Name.Index].Value = "(임무완수!)";
			}

			//更新時にソートする
			if (this.QuestView.SortedColumn != null)
                this.QuestView.Sort(this.QuestView.SortedColumn, this.QuestView.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);


            this.QuestView.ResumeLayout();
		}


		private void QuestView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{

			if (e.Value is int)
			{
				if (e.ColumnIndex == this.QuestView_Type.Index)
				{
					e.Value = Constants.GetQuestType((int)e.Value);
					e.FormattingApplied = true;

				}
				else if (e.ColumnIndex == this.QuestView_Category.Index)
				{
					e.Value = Constants.GetQuestCategory((int)e.Value);
					e.FormattingApplied = true;

				}
				else if (e.ColumnIndex == this.QuestView_Name.Index)
				{
					var quest = KCDatabase.Instance.Quest[(int)e.Value];
					e.Value = quest != null ? quest.Name : "???";
					e.FormattingApplied = true;

				}

			}

		}



		private void QuestView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{
			if (e.Column.Index == this.QuestView_State.Index)
			{
				e.SortResult = (e.CellValue1 == null ? 2 : ((bool)e.CellValue1 ? 1 : 0)) -
					(e.CellValue2 == null ? 2 : ((bool)e.CellValue2 ? 1 : 0));
			}
			else
			{
				e.SortResult = (e.CellValue1 as int? ?? 99999999) - (e.CellValue2 as int? ?? 99999999);
			}

			if (e.SortResult == 0)
			{
				e.SortResult = (this.QuestView.Rows[e.RowIndex1].Tag as int? ?? 0) - (this.QuestView.Rows[e.RowIndex2].Tag as int? ?? 0);
			}

			e.Handled = true;
		}

		private void QuestView_Sorted(object sender, EventArgs e)
		{

			for (int i = 0; i < this.QuestView.Rows.Count; i++)
			{
                this.QuestView.Rows[i].Tag = i;
			}

		}


		private void QuestView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{

			if (e.ColumnIndex != this.QuestView_Progress.Index ||
				e.RowIndex < 0 ||
				(e.PaintParts & DataGridViewPaintParts.Background) == 0)
				return;


			using (var bback = new SolidBrush(e.CellStyle.BackColor))
			{

				Color col;
				double rate = this.QuestView.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag as double? ?? 0.0;

				if (rate < 0.5)
				{
					col = Color.FromArgb(0xFF, 0x88, 0x00);

				}
				else if (rate < 0.8)
				{
					col = Color.FromArgb(0x00, 0xCC, 0x00);

				}
				else if (rate < 1.0)
				{
					col = Color.FromArgb(0x00, 0x88, 0x00);

				}
				else
				{
					col = Color.FromArgb(0x00, 0x88, 0xFF);

				}

				using (var bgauge = new SolidBrush(col))
				{

					const int thickness = 4;

					e.Graphics.FillRectangle(bback, e.CellBounds);
					e.Graphics.FillRectangle(bgauge, new Rectangle(e.CellBounds.X, e.CellBounds.Bottom - thickness, (int)(e.CellBounds.Width * rate), thickness));
				}
			}

			e.Paint(e.ClipBounds, e.PaintParts & ~DataGridViewPaintParts.Background);
			e.Handled = true;

		}



		private void MenuMain_ShowRunningOnly_Click(object sender, EventArgs e)
		{
			Utility.Configuration.Config.FormQuest.ShowRunningOnly = this.MenuMain_ShowRunningOnly.Checked;
            this.Updated();
		}


		private void MenuMain_ShowOnce_Click(object sender, EventArgs e)
		{
			Utility.Configuration.Config.FormQuest.ShowOnce = this.MenuMain_ShowOnce.Checked;
            this.Updated();
		}

		private void MenuMain_ShowDaily_Click(object sender, EventArgs e)
		{
			Utility.Configuration.Config.FormQuest.ShowDaily = this.MenuMain_ShowDaily.Checked;
            this.Updated();
		}

		private void MenuMain_ShowWeekly_Click(object sender, EventArgs e)
		{
			Utility.Configuration.Config.FormQuest.ShowWeekly = this.MenuMain_ShowWeekly.Checked;
            this.Updated();
		}

		private void MenuMain_ShowMonthly_Click(object sender, EventArgs e)
		{
			Utility.Configuration.Config.FormQuest.ShowMonthly = this.MenuMain_ShowMonthly.Checked;
            this.Updated();
		}

		private void MenuMain_ShowOther_Click(object sender, EventArgs e)
		{
			Utility.Configuration.Config.FormQuest.ShowOther = this.MenuMain_ShowOther.Checked;
            this.Updated();
		}



		private void MenuMain_Initialize_Click(object sender, EventArgs e)
		{

			if (MessageBox.Show("임무 데이터를 초기화합니다.\r\n데이터 충돌이 발생하는 경우 이외에는 권장하지 않습니다.\r\n진행하시겠습니까?", "임무초기화 확인",
				MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
			{

				KCDatabase.Instance.Quest.Clear();
				KCDatabase.Instance.QuestProgress.Clear();
                this.ClearQuestView();
			}

		}


		private void ClearQuestView()
		{

            this.QuestView.Rows.Clear();

			{
				DataGridViewRow row = new DataGridViewRow();
				row.CreateCells(this.QuestView);
				row.SetValues(null, null, null, "(미취득)", null);
                this.QuestView.Rows.Add(row);
            }

		}


		private void MenuMain_ColumnFilter_Click(object sender, EventArgs e)
		{

			var menu = sender as ToolStripMenuItem;
			if (menu == null) return;

			int index = -1;
			for (int i = 0; i < this.MenuMain_ColumnFilter.DropDownItems.Count; i++)
			{
				if (sender == this.MenuMain_ColumnFilter.DropDownItems[i])
				{
					index = i;
					break;
				}
			}

			if (index == -1) return;

            this.QuestView.Columns[index].Visible =
			Utility.Configuration.Config.FormQuest.ColumnFilter.List[index] = menu.Checked;
		}


		private void QuestView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
		{

			if (this.IsLoaded)
				Utility.Configuration.Config.FormQuest.ColumnWidth = this.QuestView.Columns.Cast<DataGridViewColumn>().Select(c => c.Width).ToList();

		}




		private void QuestView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{

			if (e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex >= 0)
			{
                this.QuestView.ClearSelection();
                this.QuestView.Rows[e.RowIndex].Selected = true;
			}

		}

		private void MenuProgress_Increment_Click(object sender, EventArgs e)
		{

			int id = this.GetSelectedRowQuestID();

			var quest = KCDatabase.Instance.Quest[id];
			var progress = KCDatabase.Instance.QuestProgress[id];

			if (id != -1 && quest != null && progress != null)
			{

				try
				{
					progress.Increment();
                    this.Updated();

				}
				catch (Exception)
				{
					Utility.Logger.Add(3, string.Format("임무『{0}』의 진행도 변경을 할 수 없습니다.", quest.Name));
					System.Media.SystemSounds.Hand.Play();
				}
			}
		}

		private void MenuProgress_Decrement_Click(object sender, EventArgs e)
		{

			int id = this.GetSelectedRowQuestID();
			var quest = KCDatabase.Instance.Quest[id];
			var progress = KCDatabase.Instance.QuestProgress[id];

			if (id != -1 && quest != null && progress != null)
			{

				try
				{
					progress.Decrement();
                    this.Updated();

				}
				catch (Exception)
				{
					Utility.Logger.Add(3, string.Format("임무『{0}』의 진행도 변경을 할 수 없습니다.", quest.Name));
					System.Media.SystemSounds.Hand.Play();
				}
			}
		}

		private void MenuProgress_Reset_Click(object sender, EventArgs e)
		{

			int id = this.GetSelectedRowQuestID();

			var quest = KCDatabase.Instance.Quest[id];
			var progress = KCDatabase.Instance.QuestProgress[id];

			if (id != -1 && (quest != null || progress != null))
			{

				if (MessageBox.Show("임무" + (quest != null ? ("『" + quest.Name + "』") : ("ID: " + id.ToString() + " ")) + "목록에서 삭제하고 진행도를 리셋합니다.\r\n진행하시겠습니까?\r\n(칸코레 임무 화면을 열면 다시 업데이트됩니다.)", "임무 삭제 확인",
					MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
				{

					if (quest != null)
						KCDatabase.Instance.Quest.Quests.Remove(quest);

					if (progress != null)
						KCDatabase.Instance.QuestProgress.Progresses.Remove(progress);

                    this.Updated();
				}
			}

		}


		// デフォルトのツールチップは消える時間が速すぎるので、自分で制御する
		private void QuestView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
		{

			if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.RowIndex >= this.QuestView.RowCount || e.ColumnIndex >= this.QuestView.ColumnCount)
			{
                this.ToolTipInfo.SetToolTip(this.QuestView, null);
				return;
			}

			if (!string.IsNullOrWhiteSpace(this.QuestView[e.ColumnIndex, e.RowIndex].ToolTipText))
			{
                this.ToolTipInfo.SetToolTip(this.QuestView, this.QuestView[e.ColumnIndex, e.RowIndex].ToolTipText);

			}
			else if (e.ColumnIndex == this.QuestView_Progress.Index && this.QuestView[e.ColumnIndex, e.RowIndex].Value != null)
			{
                this.ToolTipInfo.SetToolTip(this.QuestView, this.QuestView[e.ColumnIndex, e.RowIndex].Value.ToString());

			}
			else
			{
                this.ToolTipInfo.SetToolTip(this.QuestView, null);
			}

		}

		private void QuestView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
		{
            this.ToolTipInfo.SetToolTip(this.QuestView, null);
		}


		private void MenuMain_Opening(object sender, CancelEventArgs e)
		{

			var quest = KCDatabase.Instance.Quest[this.GetSelectedRowQuestID()];

			if (quest != null)
			{
                this.MenuMain_GoogleQuest.Enabled = true;
                this.MenuMain_GoogleQuest.Text = string.Format("『{0}』으로 구글 검색(&G)", quest.Name);
			}
			else
			{
                this.MenuMain_GoogleQuest.Enabled = false;
                this.MenuMain_GoogleQuest.Text = "임무명으로 구글검색(&G)";
			}
		}

		private void MenuMain_GoogleQuest_Click(object sender, EventArgs e)
		{
			var quest = KCDatabase.Instance.Quest[this.GetSelectedRowQuestID()];

			if (quest != null)
			{
				try
				{
                    // google <任務名> 艦これ
                    System.Diagnostics.Process.Start(@"https://www.google.co.jp/search?q=%22" + Uri.EscapeDataString(quest.Name.Replace("+", "＋")) + "%22+%E8%89%A6%E3%81%93%E3%82%8C");

                }
                catch (Exception ex)
				{
					Utility.ErrorReporter.SendErrorReport(ex, "임무명 구글 검색에 실패했습니다.");
				}
			}

		}

		private int GetSelectedRowQuestID()
		{
			var rows = this.QuestView.SelectedRows;

			if (rows != null && rows.Count > 0 && rows[0].Index != -1)
			{

				return rows[0].Cells[this.QuestView_Name.Index].Value as int? ?? -1;
			}

			return -1;
		}


		protected override string GetPersistString()
		{
			return "Quest";
		}


	}
}
