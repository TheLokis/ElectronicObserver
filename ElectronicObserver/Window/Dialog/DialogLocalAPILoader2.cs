using ElectronicObserver.Observer;
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
	public partial class DialogLocalAPILoader2 : Form
	{


		private string CurrentPath { get; set; }


		public DialogLocalAPILoader2()
		{
            this.InitializeComponent();
		}

		private void DialogLocalAPILoader2_Load(object sender, EventArgs e)
		{
            this.LoadFiles(Utility.Configuration.Config.Connection.SaveDataPath);
		}


		private void Menu_File_OpenFolder_Click(object sender, EventArgs e)
		{

            this.FolderBrowser.SelectedPath = Utility.Configuration.Config.Connection.SaveDataPath;

			if (this.FolderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
                this.LoadFiles(this.FolderBrowser.SelectedPath);
			}

		}

		private void Menu_File_Reload_Click(object sender, EventArgs e)
		{
			if (Directory.Exists(this.CurrentPath))
                this.LoadFiles(this.CurrentPath);
			else
				MessageBox.Show("폴더가 지정되지 않았거나 존재하지 않습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void ViewMenu_Execute_Click(object sender, EventArgs e)
		{

			/*/
			var rows = APIView.SelectedRows.Cast<DataGridViewRow>().OrderBy( r => r.Cells[APIView_FileName.Index].Value );

			foreach ( DataGridViewRow row in rows ) {
				ExecuteAPI( (string)row.Cells[APIView_FileName.Index].Value );
			}
			/*/
			if (!this.APICaller.IsBusy)
                this.APICaller.RunWorkerAsync(this.APIView.SelectedRows.Cast<DataGridViewRow>().Select(row => row.Cells[this.APIView_FileName.Index].Value as string).OrderBy(s => s));
			else
				if (MessageBox.Show("이미 실행 중입니다.\n중단 하시겠습니까?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
					== System.Windows.Forms.DialogResult.Yes)
			{
                this.APICaller.CancelAsync();
			}
			//*/
		}

		private void ViewMenu_Delete_Click(object sender, EventArgs e)
		{

			foreach (DataGridViewRow row in this.APIView.SelectedRows)
			{
                this.APIView.Rows.Remove(row);
			}
		}


		private void ButtonExecuteNext_Click(object sender, EventArgs e)
		{

			if (this.APIView.SelectedRows.Count == 1)
			{

				var row = this.APIView.SelectedRows[0];
				int index = this.APIView.SelectedRows[0].Index;

                this.ExecuteAPI((string)row.Cells[this.APIView_FileName.Index].Value);

                this.APIView.ClearSelection();
				if (index < this.APIView.Rows.Count - 1)
				{
                    this.APIView.Rows[index + 1].Selected = true;
                    this.APIView.FirstDisplayedScrollingRowIndex = index + 1;
				}
			}
			else
			{
				MessageBox.Show("행 1개만 선택해주세요.", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);

			}

		}



		private void LoadFiles(string path)
		{

			if (!Directory.Exists(path)) return;

            this.CurrentPath = path;

            this.APIView.Rows.Clear();

			var rows = new LinkedList<DataGridViewRow>();

			foreach (string file in Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly))
			{

				var row = new DataGridViewRow();
				row.CreateCells(this.APIView);

				row.SetValues(Path.GetFileName(file));
				rows.AddLast(row);

			}

            this.APIView.Rows.AddRange(rows.ToArray());
            this.APIView.Sort(this.APIView_FileName, ListSortDirection.Ascending);

		}


		//filename format: yyyyMMdd_hhmmssff[Q|S]@apipath@apiname.json
		private string GetAPIName(string fileName)
		{
			int indexa = fileName.IndexOf('@') + 1, indexb = fileName.LastIndexOf('.');
			return fileName.Substring(indexa).Substring(0, indexb - indexa).Replace('@', '/');
		}

		private string GetAPITime(string filename)
		{
			return filename.Substring(0, filename.IndexOf('@') - 1);
		}


		private bool IsRequest(string filename)
		{
			return char.ToLower(filename[filename.IndexOf('@') - 1]) == 'q';
		}

		private bool IsResponse(string filename)
		{
			return char.ToLower(filename[filename.IndexOf('@') - 1]) == 's';
		}


		private void ExecuteAPI(string filename)
		{

			if (APIObserver.Instance.APIList.ContainsKey(this.GetAPIName(filename)))
			{

				string data;

				try
				{
					using (var sr = new System.IO.StreamReader(this.CurrentPath + "\\" + filename))
					{
						data = sr.ReadToEnd();
					}

				}
				catch (Exception ex)
				{
					Utility.Logger.Add(3, string.Format("API파일 {0} 로드에 실패했습니다. {1}", filename, ex.Message));
					return;
				}


				if (this.IsRequest(filename))
					APIObserver.Instance.LoadRequest("/kcsapi/" + this.GetAPIName(filename), data);

				if (this.IsResponse(filename))
					APIObserver.Instance.LoadResponse("/kcsapi/" + this.GetAPIName(filename), data);
			}

		}



		private void APICaller_DoWork(object sender, DoWorkEventArgs e)
		{

			var files = e.Argument as IOrderedEnumerable<string>;
			var act = new Action<string>(this.ExecuteAPI);

			foreach (var file in files)
			{
                this.Invoke(act, file);
				System.Threading.Thread.Sleep(10);      //ゆるして

				if (this.APICaller.CancellationPending)
				{
					e.Result = file;
					break;
				}
			}

		}

		private void APICaller_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{

			if (e.Result != null)
			{   //canceled	
				int count = this.APIView.Rows.Count;
				int result = -1;
				string canceledFile = e.Result as string;

				for (int i = 0; i < count; i++)
				{
					if (this.APIView[this.APIView_FileName.Index, i].Value.ToString() == canceledFile)
					{
						result = i + 1;     // 探知結果までは処理済みのため
						break;
					}
				}

				if (result != -1 && result < count)
				{
                    this.APIView.ClearSelection();
                    this.APIView.Rows[result].Selected = true;
                    this.APIView.FirstDisplayedScrollingRowIndex = result;
				}
			}
		}


		private void DialogLocalAPILoader2_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.APICaller.IsBusy)
			{
				e.Cancel = true;
				System.Media.SystemSounds.Exclamation.Play();
			}
		}


		private void ButtonSearch_Click(object sender, EventArgs e)
		{

			int count = this.APIView.Rows.Count;
			int index;
			int result = -1;
			if (this.APIView.SelectedRows.Count > 0)
				index = this.APIView.SelectedRows[0].Index + 1;
			else
				index = 0;

			if (index >= count)
				index = 0;

			for (int i = index; i < count; i++)
			{
				if (this.APIView[this.APIView_FileName.Index, i].Value.ToString().ToLower().Contains(this.TextFilter.Text.ToLower()))
				{
					result = i;
					break;
				}
			}

			if (result != -1)
			{
                this.APIView.ClearSelection();
                this.APIView.Rows[result].Selected = true;
                this.APIView.FirstDisplayedScrollingRowIndex = result;
			}
			else
			{
				System.Media.SystemSounds.Asterisk.Play();
			}
		}

		private void ButtonSearchPrev_Click(object sender, EventArgs e)
		{

			int count = this.APIView.Rows.Count;
			int index;
			int result = -1;
			if (this.APIView.SelectedRows.Count > 0)
				index = this.APIView.SelectedRows[0].Index - 1;
			else
				index = count - 1;

			if (index < 0)
				index = count - 1;

			for (int i = index; i >= 0; i--)
			{
				if (this.APIView[this.APIView_FileName.Index, i].Value.ToString().ToLower().Contains(this.TextFilter.Text.ToLower()))
				{
					result = i;
					break;
				}
			}

			if (result != -1)
			{
                this.APIView.ClearSelection();
                this.APIView.Rows[result].Selected = true;
                this.APIView.FirstDisplayedScrollingRowIndex = result;
			}
			else
			{
				System.Media.SystemSounds.Asterisk.Play();
			}
		}



		private void ButtonSearchLastStart2_Click(object sender, EventArgs e)
		{
			for (int i = this.APIView.Rows.Count - 1; i >= 0; i--)
			{
                if (this.APIView[this.APIView_FileName.Index, i].Value.ToString().ToLower().Contains("s@api_start2@getData."))
                {
                    this.APIView.ClearSelection();
                    this.APIView.Rows[i].Selected = true;
                    this.APIView.FirstDisplayedScrollingRowIndex = i;
					return;
				}
			}

			//failed
			System.Media.SystemSounds.Asterisk.Play();
		}


		private void TextFilter_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				e.SuppressKeyPress = true;
                this.ButtonSearch.PerformClick();
			}
		}


		private void APIView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(this.CurrentPath + "\\" + this.APIView.SelectedCells.OfType<DataGridViewCell>().First().Value.ToString());
			}
			catch (Exception ex)
			{
				Utility.Logger.Add(1, $"API 파일의 시작에 실패했습니다. {ex.GetType().Name}: {ex.Message}");
			}
		}
	}
}
