using ElectronicObserver.Data;
using ElectronicObserver.Data.ShipGroup;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Window.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog
{
	public partial class DialogShipGroupFilter : Form
	{

		private ShipGroupData _group;


		#region DataTable
		private DataTable _dtAndOr;
		private DataTable _dtLeftOperand;
		private DataTable _dtOperator;
		private DataTable _dtOperator_bool;
		private DataTable _dtOperator_number;
		private DataTable _dtOperator_string;
		private DataTable _dtOperator_array;
		private DataTable _dtRightOperand_bool;
		private DataTable _dtRightOperand_shipname;
		private DataTable _dtRightOperand_shiptype;
		private DataTable _dtRightOperand_range;
		private DataTable _dtRightOperand_speed;
		private DataTable _dtRightOperand_rarity;
		private DataTable _dtRightOperand_equipment;
		#endregion

		public DialogShipGroupFilter(ShipGroupData group)
		{
            this.InitializeComponent();

			{
				// 一部の列ヘッダを中央揃えにする
				var headercenter = new DataGridViewCellStyle(this.ExpressionView_Enabled.HeaderCell.Style)
				{
					Alignment = DataGridViewContentAlignment.MiddleCenter
				};
                this.ExpressionView_Enabled.HeaderCell.Style =
                this.ExpressionView_InternalAndOr.HeaderCell.Style =
                this.ExpressionView_ExternalAndOr.HeaderCell.Style =
                this.ExpressionView_Inverse.HeaderCell.Style =
                this.ExpressionView_Up.HeaderCell.Style =
                this.ExpressionView_Down.HeaderCell.Style =
                this.ExpressionDetailView_Enabled.HeaderCell.Style =
                this.ConstFilterView_Up.HeaderCell.Style =
                this.ConstFilterView_Down.HeaderCell.Style =
                this.ConstFilterView_Delete.HeaderCell.Style =
				headercenter;
			}


			#region init DataTable
			{
                this._dtAndOr = new DataTable();
                this._dtAndOr.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( bool ) ),
					new DataColumn( "Display", typeof( string ) ) });
                this._dtAndOr.Rows.Add(true, "And");
                this._dtAndOr.Rows.Add(false, "Or");
                this._dtAndOr.AcceptChanges();

                this.ExpressionView_InternalAndOr.ValueMember = "Value";
                this.ExpressionView_InternalAndOr.DisplayMember = "Display";
                this.ExpressionView_InternalAndOr.DataSource = this._dtAndOr;

                this.ExpressionView_ExternalAndOr.ValueMember = "Value";
                this.ExpressionView_ExternalAndOr.DisplayMember = "Display";
                this.ExpressionView_ExternalAndOr.DataSource = this._dtAndOr;
			}
			{
                this._dtLeftOperand = new DataTable();
                this._dtLeftOperand.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( string ) ),
					new DataColumn( "Display", typeof( string ) ) });
				foreach (var lont in ExpressionData.LeftOperandNameTable)
                    this._dtLeftOperand.Rows.Add(lont.Key, lont.Value);
                this._dtLeftOperand.AcceptChanges();

                this.LeftOperand.ValueMember = "Value";
                this.LeftOperand.DisplayMember = "Display";
                this.LeftOperand.DataSource = this._dtLeftOperand;
			}
			{
                this._dtOperator = new DataTable();
                this._dtOperator.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( ExpressionData.ExpressionOperator ) ),
					new DataColumn( "Display", typeof( string ) ) });
				foreach (var ont in ExpressionData.OperatorNameTable)
                    this._dtOperator.Rows.Add(ont.Key, ont.Value);
                this._dtOperator.AcceptChanges();

                this.Operator.ValueMember = "Value";
                this.Operator.DisplayMember = "Display";
                this.Operator.DataSource = this._dtOperator;
			}
			{
                this._dtOperator_bool = new DataTable();
                this._dtOperator_bool.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( ExpressionData.ExpressionOperator ) ),
					new DataColumn( "Display", typeof( string ) ) });
                this._dtOperator_bool.Rows.Add(ExpressionData.ExpressionOperator.Equal, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.Equal]);
                this._dtOperator_bool.Rows.Add(ExpressionData.ExpressionOperator.NotEqual, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.NotEqual]);
                this._dtOperator_bool.AcceptChanges();
			}
			{
                this._dtOperator_number = new DataTable();
                this._dtOperator_number.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( ExpressionData.ExpressionOperator ) ),
					new DataColumn( "Display", typeof( string ) ) });
                this._dtOperator_number.Rows.Add(ExpressionData.ExpressionOperator.Equal, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.Equal]);
                this._dtOperator_number.Rows.Add(ExpressionData.ExpressionOperator.NotEqual, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.NotEqual]);
                this._dtOperator_number.Rows.Add(ExpressionData.ExpressionOperator.LessThan, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.LessThan]);
                this._dtOperator_number.Rows.Add(ExpressionData.ExpressionOperator.LessEqual, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.LessEqual]);
                this._dtOperator_number.Rows.Add(ExpressionData.ExpressionOperator.GreaterThan, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.GreaterThan]);
                this._dtOperator_number.Rows.Add(ExpressionData.ExpressionOperator.GreaterEqual, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.GreaterEqual]);
                this._dtOperator_number.AcceptChanges();
			}
			{
                this._dtOperator_string = new DataTable();
                this._dtOperator_string.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( ExpressionData.ExpressionOperator ) ),
					new DataColumn( "Display", typeof( string ) ) });
                this._dtOperator_string.Rows.Add(ExpressionData.ExpressionOperator.Equal, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.Equal]);
                this._dtOperator_string.Rows.Add(ExpressionData.ExpressionOperator.NotEqual, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.NotEqual]);
                this._dtOperator_string.Rows.Add(ExpressionData.ExpressionOperator.Contains, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.Contains]);
                this._dtOperator_string.Rows.Add(ExpressionData.ExpressionOperator.NotContains, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.NotContains]);
                this._dtOperator_string.Rows.Add(ExpressionData.ExpressionOperator.BeginWith, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.BeginWith]);
                this._dtOperator_string.Rows.Add(ExpressionData.ExpressionOperator.NotBeginWith, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.NotBeginWith]);
                this._dtOperator_string.Rows.Add(ExpressionData.ExpressionOperator.EndWith, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.EndWith]);
                this._dtOperator_string.Rows.Add(ExpressionData.ExpressionOperator.NotEndWith, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.NotEndWith]);
                this._dtOperator_string.AcceptChanges();
			}
			{
                this._dtOperator_array = new DataTable();
                this._dtOperator_array.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( ExpressionData.ExpressionOperator ) ),
					new DataColumn( "Display", typeof( string ) ) });
                this._dtOperator_array.Rows.Add(ExpressionData.ExpressionOperator.ArrayContains, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.ArrayContains]);
                this._dtOperator_array.Rows.Add(ExpressionData.ExpressionOperator.ArrayNotContains, ExpressionData.OperatorNameTable[ExpressionData.ExpressionOperator.ArrayNotContains]);
                this._dtOperator_array.AcceptChanges();
			}
			{
                this._dtRightOperand_bool = new DataTable();
                this._dtRightOperand_bool.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( bool ) ),
					new DataColumn( "Display", typeof( string ) ) });
                this._dtRightOperand_bool.Rows.Add(true, "○");
                this._dtRightOperand_bool.Rows.Add(false, "×");
                this._dtRightOperand_bool.AcceptChanges();
			}
			{
                this._dtRightOperand_shipname = new DataTable();
                this._dtRightOperand_shipname.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( int ) ),
					new DataColumn( "Display", typeof( string ) ) });
				foreach (var s in KCDatabase.Instance.MasterShips.Values.Where(s => !s.IsAbyssalShip).OrderBy(s => s.NameWithClass).OrderBy(s => s.NameReading))
                    this._dtRightOperand_shipname.Rows.Add(s.ShipID, s.Name);
                this._dtRightOperand_shipname.AcceptChanges();
			}
			{
                this._dtRightOperand_shiptype = new DataTable();
                this._dtRightOperand_shiptype.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( int ) ),
					new DataColumn( "Display", typeof( string ) ) });
				foreach (var st in KCDatabase.Instance.MasterShips.Values
					.Where(s => !s.IsAbyssalShip)
					.Select(s => (int)s.ShipType)
					.Distinct()
					.OrderBy(i => i)
					.Select(i => KCDatabase.Instance.ShipTypes[i]))
                    this._dtRightOperand_shiptype.Rows.Add(st.TypeID, st.Name);
                this._dtRightOperand_shiptype.AcceptChanges();
			}
			{
                this._dtRightOperand_range = new DataTable();
                this._dtRightOperand_range.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( int ) ),
					new DataColumn( "Display", typeof( string ) ) });
				for (int i = 0; i <= 4; i++)
                    this._dtRightOperand_range.Rows.Add(i, Constants.GetRange(i));
                this._dtRightOperand_range.AcceptChanges();
			}
			{
                this._dtRightOperand_speed = new DataTable();
                this._dtRightOperand_speed.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( int ) ),
					new DataColumn( "Display", typeof( string ) ) });
                this._dtRightOperand_speed.Rows.Add(0, Constants.GetSpeed(0));
                this._dtRightOperand_speed.Rows.Add(5, Constants.GetSpeed(5));
                this._dtRightOperand_speed.Rows.Add(10, Constants.GetSpeed(10));
                this._dtRightOperand_speed.Rows.Add(15, Constants.GetSpeed(15));
                this._dtRightOperand_speed.Rows.Add(20, Constants.GetSpeed(20));
                this._dtRightOperand_speed.AcceptChanges();
			}
			{
                this._dtRightOperand_rarity = new DataTable();
                this._dtRightOperand_rarity.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( int ) ),
					new DataColumn( "Display", typeof( string ) ) });
				for (int i = 1; i <= 8; i++)
                    this._dtRightOperand_rarity.Rows.Add(i, Constants.GetShipRarity(i));
                this._dtRightOperand_rarity.AcceptChanges();
			}
			{
                this._dtRightOperand_equipment = new DataTable();
                this._dtRightOperand_equipment.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( int ) ),
					new DataColumn( "Display", typeof( string ) ) });
                this._dtRightOperand_equipment.Rows.Add(-1, "(없음)");
				foreach (var eq in KCDatabase.Instance.MasterEquipments.Values.Where(eq => !eq.IsAbyssalEquipment).OrderBy(eq => eq.CategoryType))
                    this._dtRightOperand_equipment.Rows.Add(eq.EquipmentID, eq.Name);
                this._dtRightOperand_equipment.Rows.Add(0, "(미개방)");
                this._dtRightOperand_equipment.AcceptChanges();
			}

            this.RightOperand_ComboBox.ValueMember = "Value";
            this.RightOperand_ComboBox.DisplayMember = "Display";
            this.RightOperand_ComboBox.DataSource = this._dtRightOperand_bool;

            this.SetExpressionSetter(ExpressionData.LeftOperandNameTable.Keys.First());

            #endregion


            this.ConstFilterSelector.SelectedIndex = 0;

            this.ImportGroupData(group);
		}

		private void DialogShipGroupFilter_Load(object sender, EventArgs e)
		{
			if (this.Owner != null)
                this.Icon = this.Owner.Icon;
		}



		/// <summary>
		/// グループデータをコピーし、UIを初期化します。
		/// </summary>
		/// <param name="group">対象となるグループ。コピーされるためこのインスタンスには変更は適用されません。</param>
		public void ImportGroupData(ShipGroupData group)
		{

            this._group = group.Clone();

            this.UpdateExpressionView();
            this.UpdateConstFilterView();
		}


		/// <summary>
		/// 編集したグループデータを出力します。
		/// </summary>
		public ShipGroupData ExportGroupData()
		{

			return this._group;
		}


		private DataGridViewRow GetExpressionViewRow(ExpressionList exp)
		{
			var row = new DataGridViewRow();
			row.CreateCells(this.ExpressionView);

			row.SetValues(
				exp.Enabled,
				exp.ExternalAnd,
				exp.Inverse,
				exp.InternalAnd,
				exp.ToString()
				);

			return row;
		}

		private DataGridViewRow GetExpressionDetailViewRow(ExpressionData exp)
		{
			var row = new DataGridViewRow();
			row.CreateCells(this.ExpressionDetailView);

			row.SetValues(
				exp.Enabled,
				exp.LeftOperand,
				exp.RightOperand,
				exp.Operator
				);

			return row;
		}


		private int GetSelectedRow(DataGridView dgv)
		{
			return dgv.SelectedRows.Count == 0 ? -1 : dgv.SelectedRows[0].Index;
		}



		private void UpdateExpressionView()
		{

            this.ExpressionView.Rows.Clear();


			var rows = new DataGridViewRow[this._group.Expressions.Expressions.Count];
			for (int i = 0; i < rows.Length; i++)
			{
				rows[i] = this.GetExpressionViewRow(this._group.Expressions.Expressions[i]);
			}

            this.ExpressionView.Rows.AddRange(rows.ToArray());

            this.ExpressionDetailView.Rows.Clear();

            this.LabelResult.Tag = false;
            this.UpdateExpressionLabel();

		}


		/// <summary>
		/// 包含/除外フィルタの表示を更新します。
		/// </summary>
		private void UpdateConstFilterView()
		{

			List<int> values = this.ConstFilterSelector.SelectedIndex == 0 ? this._group.InclusionFilter : this._group.ExclusionFilter;

            this.ConstFilterView.Rows.Clear();

			var rows = new DataGridViewRow[values.Count];
			for (int i = 0; i < rows.Length; i++)
			{
				rows[i] = new DataGridViewRow();
				rows[i].CreateCells(this.ConstFilterView);

				var ship = KCDatabase.Instance.Ships[values[i]];
				rows[i].SetValues(values[i], ship?.NameWithLevel ?? "(미등록)");
			}

            this.ConstFilterView.Rows.AddRange(rows);

		}

		private List<int> GetConstFilterFromUI()
		{
			return this.ConstFilterSelector.SelectedIndex == 0 ? this._group.InclusionFilter : this._group.ExclusionFilter;
		}


		/// <summary>
		/// 指定された式から、式UIを初期化します。
		/// </summary>
		/// <param name="left">左辺値。</param>
		/// <param name="right">右辺値。指定しなければ null。</param>
		/// <param name="ope">演算子。指定しなければ null。</param>
		private void SetExpressionSetter(string left, object right = null, ExpressionData.ExpressionOperator? ope = null)
		{

			Type lefttype = ExpressionData.GetLeftOperandType(left);

			bool isenumerable = lefttype != null && lefttype != typeof(string) && lefttype.GetInterface("IEnumerable") != null;
			if (isenumerable)
				lefttype = lefttype.GetElementType() ?? lefttype.GetGenericArguments().First();

            this.Description.Text = "";

            this.LeftOperand.SelectedValue = left;

			// 特殊判定(決め打ち)シリーズ
			if (left == ".MasterShip.NameWithClass")
			{
                this.RightOperand_ComboBox.Visible = true;
                this.RightOperand_ComboBox.Enabled = true;
                this.RightOperand_ComboBox.DropDownStyle = ComboBoxStyle.DropDown;
                this.RightOperand_NumericUpDown.Visible = false;
                this.RightOperand_NumericUpDown.Enabled = false;
                this.RightOperand_TextBox.Visible = false;
                this.RightOperand_TextBox.Enabled = false;
                this.Operator.Enabled = true;
                this.Operator.DataSource = this._dtOperator_string;

                this.RightOperand_ComboBox.DataSource = this._dtRightOperand_shipname;
                this.RightOperand_ComboBox.Text = (string)(right ?? this._dtRightOperand_shipname.AsEnumerable().First()["Display"]);

			}
			else if (left == ".MasterShip.ShipType")
			{
                this.RightOperand_ComboBox.Visible = true;
                this.RightOperand_ComboBox.Enabled = true;
                this.RightOperand_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                this.RightOperand_NumericUpDown.Visible = false;
                this.RightOperand_NumericUpDown.Enabled = false;
                this.RightOperand_TextBox.Visible = false;
                this.RightOperand_TextBox.Enabled = false;
                this.Operator.Enabled = true;
                this.Operator.DataSource = this._dtOperator_bool;

                this.RightOperand_ComboBox.DataSource = this._dtRightOperand_shiptype;
                this.RightOperand_ComboBox.SelectedValue = right ?? 2;

			}
			else if (left.Contains("SlotMaster"))
			{
                this.RightOperand_ComboBox.Visible = true;
                this.RightOperand_ComboBox.Enabled = true;
                this.RightOperand_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                this.RightOperand_NumericUpDown.Visible = false;
                this.RightOperand_NumericUpDown.Enabled = false;
                this.RightOperand_TextBox.Visible = false;
                this.RightOperand_TextBox.Enabled = false;
                this.Operator.Enabled = true;
                this.Operator.DataSource = this._dtOperator_bool;

                this.RightOperand_ComboBox.DataSource = this._dtRightOperand_equipment;
                this.RightOperand_ComboBox.SelectedValue = right ?? 1;

			}
			else if (left == ".Range")
			{
                this.RightOperand_ComboBox.Visible = true;
                this.RightOperand_ComboBox.Enabled = true;
                this.RightOperand_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                this.RightOperand_NumericUpDown.Visible = false;
                this.RightOperand_NumericUpDown.Enabled = false;
                this.RightOperand_TextBox.Visible = false;
                this.RightOperand_TextBox.Enabled = false;
                this.Operator.Enabled = true;
                this.Operator.DataSource = this._dtOperator_number;

                this.RightOperand_ComboBox.DataSource = this._dtRightOperand_range;
                this.RightOperand_ComboBox.SelectedValue = right ?? 1;

			}
			else if (left == ".Speed" || left == ".MasterShip.Speed")
			{
                this.RightOperand_ComboBox.Visible = true;
                this.RightOperand_ComboBox.Enabled = true;
                this.RightOperand_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                this.RightOperand_NumericUpDown.Visible = false;
                this.RightOperand_NumericUpDown.Enabled = false;
                this.RightOperand_TextBox.Visible = false;
                this.RightOperand_TextBox.Enabled = false;
                this.Operator.Enabled = true;
                this.Operator.DataSource = this._dtOperator_number;

                this.RightOperand_ComboBox.DataSource = this._dtRightOperand_speed;
                this.RightOperand_ComboBox.SelectedValue = right ?? 10;

			}
			else if (left == ".MasterShip.Rarity")
			{
                this.RightOperand_ComboBox.Visible = true;
                this.RightOperand_ComboBox.Enabled = true;
                this.RightOperand_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                this.RightOperand_NumericUpDown.Visible = false;
                this.RightOperand_NumericUpDown.Enabled = false;
                this.RightOperand_TextBox.Visible = false;
                this.RightOperand_TextBox.Enabled = false;
                this.Operator.Enabled = true;
                this.Operator.DataSource = this._dtOperator_number;

                this.RightOperand_ComboBox.DataSource = this._dtRightOperand_rarity;
                this.RightOperand_ComboBox.SelectedValue = right ?? 1;


				// 以下、汎用判定
			}
			else if (lefttype == null)
			{
                this.RightOperand_ComboBox.Visible = false;
                this.RightOperand_ComboBox.Enabled = false;
                this.RightOperand_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                this.RightOperand_NumericUpDown.Visible = false;
                this.RightOperand_NumericUpDown.Enabled = false;
                this.RightOperand_TextBox.Visible = true;
                this.RightOperand_TextBox.Enabled = false;
                this.Operator.Enabled = false;
                this.Operator.DataSource = this._dtOperator;

                this.RightOperand_TextBox.Text = right == null ? "" : right.ToString();

			}
			else if (lefttype == typeof(int))
			{
                this.RightOperand_ComboBox.Visible = false;
                this.RightOperand_ComboBox.Enabled = false;
                this.RightOperand_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                this.RightOperand_NumericUpDown.Visible = true;
                this.RightOperand_NumericUpDown.Enabled = true;
                this.RightOperand_TextBox.Visible = false;
                this.RightOperand_TextBox.Enabled = false;
                this.Operator.Enabled = true;
                this.Operator.DataSource = this._dtOperator_number;

                this.RightOperand_NumericUpDown.DecimalPlaces = 0;
                this.RightOperand_NumericUpDown.Increment = 1m;

				switch (left)
				{
					case ".MasterID":
                        this.RightOperand_NumericUpDown.Minimum = 0;
                        this.RightOperand_NumericUpDown.Maximum = 999999;
						break;
					case ".Level":
                        this.RightOperand_NumericUpDown.Minimum = 1;
                        this.RightOperand_NumericUpDown.Maximum = ExpTable.ShipMaximumLevel;
						break;
					case ".ExpTotal":
					case ".ExpNextRemodel":
                        this.RightOperand_NumericUpDown.Minimum = 0;
                        this.RightOperand_NumericUpDown.Maximum = 4360000;
						break;
					case ".ExpNext":
                        this.RightOperand_NumericUpDown.Minimum = 0;
                        this.RightOperand_NumericUpDown.Maximum = 195000;
						break;
					case ".HPCurrent":
					case ".HPMax":
                        this.RightOperand_NumericUpDown.Minimum = 0;
                        this.RightOperand_NumericUpDown.Maximum = 999;
						break;
					case ".Condition":
                        this.RightOperand_NumericUpDown.Minimum = 0;
                        this.RightOperand_NumericUpDown.Maximum = 100;
						break;
					case ".RepairingDockID":
                        this.RightOperand_NumericUpDown.Minimum = -1;
                        this.RightOperand_NumericUpDown.Maximum = 4;
                        this.Description.Text = "-1=미입거, 1～4=입거중(도크번호)";
						break;
					case ".RepairTime":
                        this.RightOperand_NumericUpDown.Minimum = 0;
                        this.RightOperand_NumericUpDown.Maximum = int.MaxValue;
                        this.RightOperand_NumericUpDown.Increment = 60000;
						break;
					case ".SlotSize":
                        this.RightOperand_NumericUpDown.Minimum = 0;
                        this.RightOperand_NumericUpDown.Maximum = 5;
						break;
					default:
                        this.RightOperand_NumericUpDown.Minimum = 0;
                        this.RightOperand_NumericUpDown.Maximum = 9999;
						break;
				}
                this.RightOperand_NumericUpDown.Value = right == null ? this.RightOperand_NumericUpDown.Minimum : (int)right;
                this.UpdateDescriptionFromNumericUpDown();

			}
			else if (lefttype == typeof(double))
			{
                this.RightOperand_ComboBox.Visible = false;
                this.RightOperand_ComboBox.Enabled = false;
                this.RightOperand_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                this.RightOperand_NumericUpDown.Visible = true;
                this.RightOperand_NumericUpDown.Enabled = true;
                this.RightOperand_TextBox.Visible = false;
                this.RightOperand_TextBox.Enabled = false;
                this.Operator.Enabled = true;
                this.Operator.DataSource = this._dtOperator_number;

				switch (left)
				{
					case ".HPRate":
					case ".AircraftRate[0]":
					case ".AircraftRate[1]":
					case ".AircraftRate[2]":
					case ".AircraftRate[3]":
					case ".AircraftRate[4]":
					case ".AircraftTotalRate":
					case ".FuelRate":
					case ".AmmoRate":
                        this.RightOperand_NumericUpDown.Minimum = 0;
                        this.RightOperand_NumericUpDown.Maximum = 1;
                        this.RightOperand_NumericUpDown.DecimalPlaces = 2;
                        this.RightOperand_NumericUpDown.Increment = 0.01m;
						break;
					default:
                        this.RightOperand_NumericUpDown.Maximum = int.MaxValue;
                        this.RightOperand_NumericUpDown.Minimum = int.MinValue;
                        this.RightOperand_NumericUpDown.DecimalPlaces = 0;
                        this.RightOperand_NumericUpDown.Increment = 1m;
						break;
				}
                this.RightOperand_NumericUpDown.Value = right == null ? this.RightOperand_NumericUpDown.Minimum : Convert.ToDecimal(right);
                this.UpdateDescriptionFromNumericUpDown();

			}
			else if (lefttype == typeof(bool))
			{
                this.RightOperand_ComboBox.Visible = true;
                this.RightOperand_ComboBox.Enabled = true;
                this.RightOperand_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                this.RightOperand_NumericUpDown.Visible = false;
                this.RightOperand_NumericUpDown.Enabled = false;
                this.RightOperand_TextBox.Visible = false;
                this.RightOperand_TextBox.Enabled = false;
                this.Operator.Enabled = true;
                this.Operator.DataSource = this._dtOperator_bool;

                this.RightOperand_ComboBox.DataSource = this._dtRightOperand_bool;
                this.RightOperand_ComboBox.SelectedValue = right ?? true;

			}
			else if (lefttype.IsEnum)
			{
                this.RightOperand_ComboBox.Visible = true;
                this.RightOperand_ComboBox.Enabled = true;
                this.RightOperand_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                this.RightOperand_NumericUpDown.Visible = false;
                this.RightOperand_NumericUpDown.Enabled = false;
                this.RightOperand_TextBox.Visible = false;
                this.RightOperand_TextBox.Enabled = false;
                this.Operator.Enabled = true;
                this.Operator.DataSource = this._dtOperator_bool;

				DataTable dt = new DataTable();
				dt.Columns.AddRange(new DataColumn[]{
					new DataColumn( "Value", typeof( string ) ),
					new DataColumn( "Display", typeof( string ) ) });
				var names = lefttype.GetEnumNames();
				var values = lefttype.GetEnumValues();
				for (int i = 0; i < names.Length; i++)
					dt.Rows.Add(values.GetValue(i), names[i]);
				dt.AcceptChanges();
                this.RightOperand_ComboBox.DataSource = dt;
                this.RightOperand_ComboBox.SelectedValue = right;

			}
			else
			{
                this.RightOperand_ComboBox.Visible = false;
                this.RightOperand_ComboBox.Enabled = false;
                this.RightOperand_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                this.RightOperand_NumericUpDown.Visible = false;
                this.RightOperand_NumericUpDown.Enabled = false;
                this.RightOperand_TextBox.Visible = true;
                this.RightOperand_TextBox.Enabled = true;
                this.Operator.Enabled = true;
                this.Operator.DataSource = this._dtOperator_string;

                this.RightOperand_TextBox.Text = right == null ? "" : right.ToString();

			}


			if (isenumerable)
			{
                this.Operator.DataSource = this._dtOperator_array;
			}


			if (this.Operator.DataSource as DataTable != null)
			{
				if (ope == null)
				{
                    this.Operator.SelectedValue = ((DataTable)this.Operator.DataSource).AsEnumerable().First()["Value"];
				}
				else
				{
                    this.Operator.SelectedValue = (ExpressionData.ExpressionOperator)ope;
				}
			}
		}



		/// <summary>
		/// 選択された行をもとに、 ExpressionDetailView を更新します。
		/// </summary>
		/// <param name="index">対象となる行のインデックス。</param>
		private void UpdateExpressionDetailView(int index)
		{

			if (index < 0 || this._group.Expressions.Expressions.Count <= index) return;

			var ex = this._group.Expressions.Expressions[index];


            // detail の更新と expression の初期化

            this.ExpressionDetailView.Rows.Clear();

			var rows = new DataGridViewRow[ex.Expressions.Count];
			for (int i = 0; i < rows.Length; i++)
			{
				rows[i] = this.GetExpressionDetailViewRow(ex.Expressions[i]);
			}

            this.ExpressionDetailView.Rows.AddRange(rows);
		}


		// 選択を基にUIの更新
		private void ExpressionView_SelectionChanged(object sender, EventArgs e)
		{

            this.UpdateExpressionDetailView(this.ExpressionView.SelectedRows.Count == 0 ? -1 : this.ExpressionView.SelectedRows[0].Index);

		}

		private void ExpressionDetailView_SelectionChanged(object sender, EventArgs e)
		{

			int index = this.ExpressionView.SelectedRows.Count == 0 ? -1 : this.ExpressionView.SelectedRows[0].Index;
			int detailIndex = this.ExpressionDetailView.SelectedRows.Count == 0 ? -1 : this.ExpressionDetailView.SelectedRows[0].Index;

			if (index < 0 || this._group.Expressions.Expressions.Count <= index ||
				detailIndex < 0 || this._group.Expressions[index].Expressions.Count <= detailIndex) return;

			ExpressionData exp = this._group.Expressions[index][detailIndex];

            this.SetExpressionSetter(exp.LeftOperand, exp.RightOperand, exp.Operator);

		}




		// Expression のボタン操作
		private void Expression_Add_Click(object sender, EventArgs e)
		{

			int insertrow = this.GetSelectedRow(this.ExpressionView);
			if (insertrow == -1) insertrow = this.ExpressionView.Rows.Count - 1;

			var exp = new ExpressionList();

            this._group.Expressions.Expressions.Insert(insertrow + 1, exp);
            this.ExpressionView.Rows.Insert(insertrow + 1, this.GetExpressionViewRow(exp));

            this.ExpressionUpdated();
		}

		private void Expression_Delete_Click(object sender, EventArgs e)
		{

			int selectedrow = this.GetSelectedRow(this.ExpressionView);

			if (selectedrow == -1)
			{
				MessageBox.Show("대상 행을 선택하십시오.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}

            this.ExpressionDetailView.Rows.Clear();

            this._group.Expressions.Expressions.RemoveAt(selectedrow);
            this.ExpressionView.Rows.RemoveAt(selectedrow);


            this.ExpressionUpdated();
		}


		private void ButtonOK_Click(object sender, EventArgs e)
		{

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void ButtonCancel_Click(object sender, EventArgs e)
		{

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}




		/// <summary>
		/// UIの設定値からExpressionDataを構築します。
		/// </summary>
		private ExpressionData BuildExpressionDataFromUI()
		{

			var exp = new ExpressionData
			{
				LeftOperand = (string)this.LeftOperand.SelectedValue ?? this.LeftOperand.Text,
				Operator = (ExpressionData.ExpressionOperator)this.Operator.SelectedValue
			};

			Type type = exp.GetLeftOperandType();
			if (type != null && type != typeof(string) && type.GetInterface("IEnumerable") != null)
				type = type.GetElementType() ?? type.GetGenericArguments().First();
			if (type.IsEnum)
				type = type.GetEnumUnderlyingType();

			if (this.RightOperand_ComboBox.Enabled)
			{
				if (this.RightOperand_ComboBox.DropDownStyle == ComboBoxStyle.DropDownList)
					exp.RightOperand = Convert.ChangeType(this.RightOperand_ComboBox.SelectedValue ?? this.RightOperand_ComboBox.Text, type);
				else
					exp.RightOperand = Convert.ChangeType(this.RightOperand_ComboBox.Text, type);

			}
			else if (this.RightOperand_NumericUpDown.Enabled)
			{
				exp.RightOperand = Convert.ChangeType(this.RightOperand_NumericUpDown.Value, type);

			}
			else if (this.RightOperand_TextBox.Enabled)
			{
				exp.RightOperand = Convert.ChangeType(this.RightOperand_TextBox.Text, type);

			}
			else
			{
				exp.RightOperand = null;
			}

			return exp;
		}



		// ExpressionDetail のボタン操作
		private void ExpressionDetail_Add_Click(object sender, EventArgs e)
		{

			int procrow = this.GetSelectedRow(this.ExpressionView);
			if (procrow == -1)
			{
				MessageBox.Show("대상이 되는 식(왼쪽) 행을 선택하십시오.\r\n행이 없는 경우 추가하십시오.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}

			var exp = this.BuildExpressionDataFromUI();

            this._group.Expressions.Expressions[procrow].Expressions.Add(exp);
            this.ExpressionDetailView.Rows.Add(this.GetExpressionDetailViewRow(exp));

            this.UpdateExpressionViewRow(procrow);
		}


		private void ExpressionDetail_Edit_Click(object sender, EventArgs e)
		{

			int procrow = this.GetSelectedRow(this.ExpressionView);
			if (procrow == -1)
			{
				MessageBox.Show("대상이 되는 식(왼쪽) 행을 선택하십시오.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}

			int selectedrow = this.GetSelectedRow(this.ExpressionDetailView);
			if (selectedrow == -1)
			{
				MessageBox.Show("대상이 되는 행을 선택하십시오.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}

			var exp = this.BuildExpressionDataFromUI();

            this._group.Expressions.Expressions[procrow].Expressions[selectedrow] = exp;
            this.ExpressionDetailView.Rows.Insert(selectedrow, this.GetExpressionDetailViewRow(exp));
            this.ExpressionDetailView.Rows.RemoveAt(selectedrow + 1);

            this.UpdateExpressionViewRow(procrow);
		}


		private void ExpressionDetail_Delete_Click(object sender, EventArgs e)
		{

			int procrow = this.GetSelectedRow(this.ExpressionView);
			if (procrow == -1)
			{
                MessageBox.Show("대상이 되는 식(왼쪽) 행을 선택하십시오.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
			}

			int selectedrow = this.GetSelectedRow(this.ExpressionDetailView);
			if (selectedrow == -1)
			{
                MessageBox.Show("대상이 되는 행을 선택하십시오.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
			}

            this._group.Expressions.Expressions[procrow].Expressions.RemoveAt(selectedrow);
            this.ExpressionDetailView.Rows.RemoveAt(selectedrow);

            this.UpdateExpressionViewRow(procrow);
		}


		// 左辺値変更時のUI変更
		private void LeftOperand_SelectedValueChanged(object sender, EventArgs e)
		{
            this.SetExpressionSetter((string)this.LeftOperand.SelectedValue ?? this.LeftOperand.Text);
		}




		// チェックボックスの更新を即時反映する
		// コンボボックスも可能だけど今回は省略
		private void ExpressionView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{

            if (this.ExpressionView.IsCurrentCellDirty)
                this.ExpressionView.CommitEdit(DataGridViewDataErrorContexts.Commit);

        }

		private void ExpressionDetailView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
            if (this.ExpressionDetailView.IsCurrentCellDirty)
                this.ExpressionDetailView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }


		// UI 操作(チェックボックス/コンボボックス)の反映
		private void ExpressionView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{

			if (e.RowIndex < 0) return;

			if (e.ColumnIndex == this.ExpressionView_Enabled.Index)
			{
                this._group.Expressions[e.RowIndex].Enabled = (bool)this.ExpressionView[e.ColumnIndex, e.RowIndex].Value;

			}
			else if (e.ColumnIndex == this.ExpressionView_ExternalAndOr.Index)
			{
                this._group.Expressions[e.RowIndex].ExternalAnd = (bool)this.ExpressionView[e.ColumnIndex, e.RowIndex].Value;

			}
			else if (e.ColumnIndex == this.ExpressionView_Inverse.Index)
			{
                this._group.Expressions[e.RowIndex].Inverse = (bool)this.ExpressionView[e.ColumnIndex, e.RowIndex].Value;

			}
			else if (e.ColumnIndex == this.ExpressionView_InternalAndOr.Index)
			{
                this._group.Expressions[e.RowIndex].InternalAnd = (bool)this.ExpressionView[e.ColumnIndex, e.RowIndex].Value;

			}

            this.UpdateExpressionViewRow(e.RowIndex);
		}

		private void ExpressionDetailView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{

			if (e.RowIndex < 0) return;

			int procrow = this.GetSelectedRow(this.ExpressionView);
			if (procrow == -1)
			{
				return;
			}

			if (e.ColumnIndex == this.ExpressionDetailView_Enabled.Index)
			{
                this._group.Expressions[procrow].Expressions[e.RowIndex].Enabled = (bool)this.ExpressionDetailView[e.ColumnIndex, e.RowIndex].Value;
			}

            this.UpdateExpressionViewRow(procrow);
		}


		/// <summary>
		/// ExpressionView の指定された行の式表示を更新します。
		/// </summary>
		/// <param name="index">行インデックス。</param>
		private void UpdateExpressionViewRow(int index)
		{
            this.ExpressionView[this.ExpressionView_Expression.Index, index].Value = this._group.Expressions[index].ToString();
            this.ExpressionUpdated();
		}

		/// <summary>
		/// 式が更新されたときの動作を行います。
		/// </summary>
		private void ExpressionUpdated()
		{
            this.UpdateExpressionLabel();
		}

		private void UpdateExpressionLabel()
		{
			if (this.LabelResult.Tag != null && (bool)this.LabelResult.Tag)
			{
                this._group.Expressions.Compile();
                this.LabelResult.Text = this._group.Expressions.ToExpressionString();
			}
			else
			{
                this.LabelResult.Text = this._group.Expressions.ToString();
			}
		}



		// ボタン処理
		private void ExpressionView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

			if (e.RowIndex < 0) return;

			// fixme: 非選択セルで上下させると選択がちょっとちらつく  :(

			if (e.ColumnIndex == this.ExpressionView_Up.Index && e.RowIndex > 0)
			{
                this._group.Expressions.Expressions.Insert(e.RowIndex - 1, this._group.Expressions[e.RowIndex]);
                this._group.Expressions.Expressions.RemoveAt(e.RowIndex + 1);

				ControlHelper.RowMoveUp(this.ExpressionView, e.RowIndex);
                this.ExpressionView.Rows[e.RowIndex - 1].Selected = true;

                this.ExpressionUpdated();


			}
			else if (e.ColumnIndex == this.ExpressionView_Down.Index && e.RowIndex < this.ExpressionView.Rows.Count - 1)
			{
                this._group.Expressions.Expressions.Insert(e.RowIndex + 2, this._group.Expressions[e.RowIndex]);
                this._group.Expressions.Expressions.RemoveAt(e.RowIndex);

				ControlHelper.RowMoveDown(this.ExpressionView, e.RowIndex);
                this.ExpressionView.Rows[e.RowIndex + 1].Selected = true;

                this.ExpressionUpdated();

			}


			if (this.ExpressionView.SelectedRows.Count > 0)
                this.UpdateExpressionDetailView(this.ExpressionView.SelectedRows[0].Index);
		}

		private void ConstFilterView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

			// 上下動に意味があるかはおいておいて
			if (e.ColumnIndex == this.ConstFilterView_Up.Index && e.RowIndex > 0)
			{
				var list = this.GetConstFilterFromUI();
				list.Insert(e.RowIndex - 1, list[e.RowIndex]);
				list.RemoveAt(e.RowIndex + 1);

				ControlHelper.RowMoveUp(this.ConstFilterView, e.RowIndex);

			}
			else if (e.ColumnIndex == this.ConstFilterView_Down.Index && e.RowIndex < this.ConstFilterView.Rows.Count - 1)
			{
				var list = this.GetConstFilterFromUI();
				list.Insert(e.RowIndex + 2, list[e.RowIndex]);
				list.RemoveAt(e.RowIndex);

				ControlHelper.RowMoveDown(this.ConstFilterView, e.RowIndex);

			}
			else if (e.ColumnIndex == this.ConstFilterView_Delete.Index && e.RowIndex >= 0)
			{
				var list = this.GetConstFilterFromUI();
				list.RemoveAt(e.RowIndex);

                this.ConstFilterView.Rows.RemoveAt(e.RowIndex);
			}

		}


		// コンボボックスの即選択
		private void ExpressionView_CellClick(object sender, DataGridViewCellEventArgs e)
		{

			if (e.RowIndex < 0) return;

			if (this.ExpressionView.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
			{
                this.ExpressionView.BeginEdit(false);
				var edit = this.ExpressionView.EditingControl as DataGridViewComboBoxEditingControl;
				edit.DroppedDown = true;
			}

		}


		private void ExpressionDetailView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{

			if (e.RowIndex < 0) return;

			int procrow = this.GetSelectedRow(this.ExpressionView);
			if (procrow < 0 || procrow >= this._group.Expressions.Expressions.Count ||
				e.RowIndex >= this._group.Expressions[procrow].Expressions.Count)
			{
				return;
			}

			if (e.ColumnIndex == this.ExpressionDetailView_LeftOperand.Index)
			{
				e.Value = this._group.Expressions[procrow].Expressions[e.RowIndex].LeftOperandToString();
				e.FormattingApplied = true;

			}
			else if (e.ColumnIndex == this.ExpressionDetailView_Operator.Index)
			{
				e.Value = this._group.Expressions[procrow].Expressions[e.RowIndex].OperatorToString();
				e.FormattingApplied = true;

			}
			else if (e.ColumnIndex == this.ExpressionDetailView_RightOperand.Index)
			{
				e.Value = this._group.Expressions[procrow].Expressions[e.RowIndex].RightOperandToString();
				e.FormattingApplied = true;
			}

		}



		// Description の変更
		private void RightOperand_NumericUpDown_ValueChanged(object sender, EventArgs e)
		{

            this.UpdateDescriptionFromNumericUpDown();
		}


		private void UpdateDescriptionFromNumericUpDown()
		{

			string left = ((string)this.LeftOperand.SelectedValue) ?? this.LeftOperand.Text;
			int intvalue = (int)this.RightOperand_NumericUpDown.Value;

			switch (left)
			{
				case ".MasterID":
					{
						var ship = KCDatabase.Instance.Ships[intvalue];
						if (ship != null)
						{
                            this.Description.Text = ship.NameWithLevel;
						}
						else
						{
                            this.Description.Text = "(미등록)";
						}
					}
					break;

				case ".ShipID":
					{
						var ship = KCDatabase.Instance.MasterShips[intvalue];
						if (ship != null)
						{
                            this.Description.Text = ship.ShipTypeName + " " + ship.NameWithClass;
						}
						else
						{
                            this.Description.Text = "(불명)";
						}
					}
					break;

				case ".RepairTime":
					{
                        this.Description.Text = string.Format("(밀리초) {0}", DateTimeHelper.ToTimeRemainString(DateTimeHelper.FromAPITimeSpan(intvalue)));
					}
					break;

				case ".MasterShip.AlbumNo":
					{
						var ship = KCDatabase.Instance.MasterShips.Values.FirstOrDefault(s => s.AlbumNo == intvalue);
						if (ship == null)
                            this.Description.Text = "(불명)";
						else
                            this.Description.Text = ship.ShipTypeName + " " + ship.NameWithClass;

					}
					break;

				case ".MasterShip.RemodelBeforeShipID":
					{
						if (intvalue == 0)
						{
                            this.Description.Text = "(미개장)";
						}
						else
						{
							var ship = KCDatabase.Instance.MasterShips[intvalue];
							if (ship == null)
                                this.Description.Text = "(불명)";
							else
							{
								var before = ship.RemodelBeforeShip;
                                this.Description.Text = ship.NameWithClass + " ← " + (before == null ? "×" : before.NameWithClass);
							}
						}
					}
					break;

				case ".MasterShip.RemodelAfterShipID":
					{
						if (intvalue == 0)
						{
                            this.Description.Text = "(최종개장)";
						}
						else
						{
							var ship = KCDatabase.Instance.MasterShips[intvalue];
							if (ship == null)
                                this.Description.Text = "(불명)";
							else
							{
								var after = ship.RemodelAfterShip;
                                this.Description.Text = ship.NameWithClass + " → " + (after == null ? "×" : after.NameWithClass);
							}
						}
					}
					break;
			}

			if (left.Contains("Rate"))
			{
                this.Description.Text = this.RightOperand_NumericUpDown.Value.ToString("P0");
			}

		}

		private void LabelResult_Click(object sender, EventArgs e)
		{
            this.LabelResult.Tag = !(bool)this.LabelResult.Tag;
            this.UpdateExpressionLabel();
		}





		// ConstFilter 関連
		private void ConstFilterSelector_SelectedIndexChanged(object sender, EventArgs e)
		{

			if (this._group != null)
			{
                this.UpdateConstFilterView();
			}

		}

		private void OptimizeConstFilter_Click(object sender, EventArgs e)
		{

			if (this.ConstFilterSelector.SelectedIndex == 0)
			{

                this._group.InclusionFilter = this._group.InclusionFilter.Intersect(KCDatabase.Instance.Ships.Keys).ToList();

			}
			else
			{

                this._group.ExclusionFilter = this._group.ExclusionFilter.Intersect(KCDatabase.Instance.Ships.Keys).ToList();
			}

            this.UpdateConstFilterView();
		}

		private void ClearConstFilter_Click(object sender, EventArgs e)
		{

			if (MessageBox.Show(this.ConstFilterSelector.Text + " 를 초기화합니다.\r\n진행 하시겠습니까?", "초기화 확인",
				MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
				== System.Windows.Forms.DialogResult.Yes)
			{

				if (this.ConstFilterSelector.SelectedIndex == 0)
				{
                    this._group.InclusionFilter.Clear();

				}
				else
				{
                    this._group.ExclusionFilter.Clear();
				}

                this.UpdateConstFilterView();
			}
		}

		private void ConvertToExpression_Click(object sender, EventArgs e)
		{

			if (MessageBox.Show("현재의 포함 / 제외 목록을 수식으로 변환합니다.\r\n역변환은 할 수 없습니다. \r\n진행 하시겠습니까?", "확인",
					MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
					== System.Windows.Forms.DialogResult.Yes)
			{

				if (this._group.InclusionFilter.Count > 0)
				{
                    this._group.Expressions.Expressions.Add(new ExpressionList(false, false, false));
					var exlist = this._group.Expressions.Expressions.Last();
					foreach (var id in this._group.InclusionFilter)
					{
						exlist.Expressions.Add(new ExpressionData(".MasterID", ExpressionData.ExpressionOperator.Equal, id));
					}
                    this._group.InclusionFilter.Clear();
				}
				if (this._group.ExclusionFilter.Count > 0)
				{
                    this._group.Expressions.Expressions.Add(new ExpressionList(false, true, true));
					var exlist = this._group.Expressions.Expressions.Last();

					foreach (var id in this._group.ExclusionFilter)
					{
						exlist.Expressions.Add(new ExpressionData(".MasterID", ExpressionData.ExpressionOperator.Equal, id));
					}
                    this._group.ExclusionFilter.Clear();
				}


                this.UpdateExpressionView();
                this.UpdateConstFilterView();

			}
		}


		private void ButtonMenu_Click(object sender, EventArgs e)
		{
            this.SubMenu.Show(this.ButtonMenu, this.ButtonMenu.Width / 2, this.ButtonMenu.Height / 2);
		}

		private void Menu_ImportFilter_Click(object sender, EventArgs e)
		{


			if (MessageBox.Show("클립 보드에서 필터를 가져옵니다.\r\n현재 필터는 삭제됩니다.(포함/제외 필터는 유지됩니다.)\r\n실행하시겠습니까?\r\n",
					"필터 가져오기 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
				== System.Windows.Forms.DialogResult.No)
				return;

			string data = Clipboard.GetText();

			if (string.IsNullOrEmpty(data))
			{
				MessageBox.Show("클립 보드가 비어 있습니다.\r\n필터 데이터를 복사 한 후 다시 선택해주세요.\r\n",
					"에러", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			try
			{

				using (var str = new StringReader(data))
				{
					var exp = (ExpressionManager)this._group.Expressions.Load(str);
					if (exp == null)
						throw new ArgumentException("가져올 수 없는 데이터 형식입니다.");
					else
                        this._group.Expressions = exp;
				}

                this.UpdateExpressionView();

			}
			catch (Exception ex)
			{

				MessageBox.Show("필터 불러오기에 실패했습니다.\r\n" + ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

		private void Menu_ExportFilter_Click(object sender, EventArgs e)
		{

			try
			{

				StringBuilder str = new StringBuilder();
                this._group.Expressions.Save(str);

				Clipboard.SetText(str.ToString());

				MessageBox.Show("필터를 클립보드에 복사했습니다.\r\n'필터 가져오기'에서 가져오거나 \r\n메모장 등에 붙여넣어 저장하세요.\r\n",
					"필터 내보내기", MessageBoxButtons.OK, MessageBoxIcon.Information);

			}
			catch (Exception ex)
			{

				MessageBox.Show("필터 내보내기에 실패했습니다.\r\n" + ex.Message, "내보낼 수 없습니다.", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}


	}
}
