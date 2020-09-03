using ElectronicObserver.Resource;
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

	public partial class FormLog : DockContent
	{


		public FormLog(FormMain parent)
		{
            this.InitializeComponent();

            this.ConfigurationChanged();
		}

		private void FormLog_Load(object sender, EventArgs e)
		{

			foreach (var log in Utility.Logger.Log)
			{
				if (log.Priority >= Utility.Configuration.Config.Log.LogLevel)
                    this.LogList.Items.Add(log.ToString());
			}
            this.LogList.TopIndex = this.LogList.Items.Count - 1;

			Utility.Logger.Instance.LogAdded += new Utility.LogAddedEventHandler((Utility.Logger.LogData data) =>
			{
				if (this.InvokeRequired)
				{
                    // Invokeはメッセージキューにジョブを投げて待つので、別のBeginInvokeされたジョブが既にキューにあると、
                    // それを実行してしまい、BeginInvokeされたジョブの順番が保てなくなる
                    // GUIスレッドによる処理は、順番が重要なことがあるので、GUIスレッドからInvokeを呼び出してはいけない
                    this.Invoke(new Utility.LogAddedEventHandler(this.Logger_LogAdded), data);
				}
				else
				{
                    this.Logger_LogAdded(data);
				}
			});

			Utility.Configuration.Instance.ConfigurationChanged += this.ConfigurationChanged;

            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormLog]);
		}


		void ConfigurationChanged()
		{

            this.LogList.Font = this.Font = Utility.Configuration.Config.UI.MainFont;

            this.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
            this.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.LogList.ForeColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.MainFontColor);
            this.LogList.BackColor = Utility.ThemeManager.GetColor(Utility.ThemeColors.BackgroundColor);
        }


		void Logger_LogAdded(Utility.Logger.LogData data)
		{

			int index = this.LogList.Items.Add(data.ToString());
            this.LogList.TopIndex = index;

		}



		private void ContextMenuLog_Clear_Click(object sender, EventArgs e)
		{

            this.LogList.Items.Clear();

		}



		protected override string GetPersistString()
		{
			return "Log";
		}


	}
}
