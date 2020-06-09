using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{
	public class FleetPresetData : APIWrapper, IIdentifiable
	{
		public int PresetID => (int)this.RawData.api_preset_no;

		public string Name => this.RawData.api_name;

		private int[] _members;
		public ReadOnlyCollection<int> Members => Array.AsReadOnly(this._members);

		public IEnumerable<ShipData> MembersInstance => this.Members.Select(id => KCDatabase.Instance.Ships[id]);


		public override void LoadFromResponse(string apiname, dynamic data)
		{
            this._members = (int[])data.api_ship;
			base.LoadFromResponse(apiname, (object)data);
		}


		public int ID => this.PresetID;
	}
}
