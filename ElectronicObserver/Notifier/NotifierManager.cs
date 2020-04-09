using ElectronicObserver.Window;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Notifier
{

	public sealed class NotifierManager
	{

		#region Singleton

		private static readonly NotifierManager instance = new NotifierManager();

		public static NotifierManager Instance => instance;

		#endregion

		private FormMain _parentForm;


		public NotifierExpedition Expedition { get; private set; }
		public NotifierConstruction Construction { get; private set; }
		public NotifierRepair Repair { get; private set; }
		public NotifierCondition Condition { get; private set; }
		public NotifierDamage Damage { get; private set; }
		public NotifierAnchorageRepair AnchorageRepair { get; private set; }
        public NotifierBaseAirCorps BaseAirCorps { get; private set; }

        private NotifierManager()
		{
		}


		public void Initialize(FormMain parent)
		{

            this._parentForm = parent;

			var c = Utility.Configuration.Config;

            this.Expedition = new NotifierExpedition(c.NotifierExpedition);
            this.Construction = new NotifierConstruction(c.NotifierConstruction);
            this.Repair = new NotifierRepair(c.NotifierRepair);
            this.Condition = new NotifierCondition(c.NotifierCondition);
            this.Damage = new NotifierDamage(c.NotifierDamage);
            this.AnchorageRepair = new NotifierAnchorageRepair(c.NotifierAnchorageRepair);
            this.BaseAirCorps = new NotifierBaseAirCorps(c.NotifierBaseAirCorps);
        }

		public void ApplyToConfiguration()
		{

			var c = Utility.Configuration.Config;

            this.Expedition.ApplyToConfiguration(c.NotifierExpedition);
            this.Construction.ApplyToConfiguration(c.NotifierConstruction);
            this.Repair.ApplyToConfiguration(c.NotifierRepair);
            this.Condition.ApplyToConfiguration(c.NotifierCondition);
            this.Damage.ApplyToConfiguration(c.NotifierDamage);
            this.AnchorageRepair.ApplyToConfiguration(c.NotifierAnchorageRepair);
            this.BaseAirCorps.ApplyToConfiguration(c.NotifierBaseAirCorps);
        }

		public void ShowNotifier(ElectronicObserver.Window.Dialog.DialogNotifier form)
		{

			if (form.DialogData.Alignment == NotifierDialogAlignment.CustomRelative)
			{       //cloneしているから書き換えても問題ないはず
				Point p = this._parentForm.fBrowser.PointToScreen(new Point(this._parentForm.fBrowser.ClientSize.Width / 2, this._parentForm.fBrowser.ClientSize.Height / 2));
				p.Offset(new Point(-form.Width / 2, -form.Height / 2));
				p.Offset(form.DialogData.Location);

				form.DialogData.Location = p;
			}

			form.Show();
		}

		public IEnumerable<NotifierBase> GetNotifiers()
		{
			yield return this.Expedition;
			yield return this.Construction;
			yield return this.Repair;
			yield return this.Condition;
			yield return this.Damage;
			yield return this.AnchorageRepair;
            yield return this.BaseAirCorps;
        }

	}

}
