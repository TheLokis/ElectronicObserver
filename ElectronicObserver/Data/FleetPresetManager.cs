using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{
	public class FleetPresetManager : APIWrapper
	{
		public IDDictionary<FleetPresetData> Presets { get; private set; }

		public int MaximumCount { get; private set; }


		public event Action PresetChanged;


		public FleetPresetManager()
		{
            this.Presets = new IDDictionary<FleetPresetData>();
		}


		public override void LoadFromRequest(string apiname, Dictionary<string, string> data)
		{
			switch (apiname)
			{
				case "api_req_hensei/preset_delete":
                    this.Presets.Remove(int.Parse(data["api_preset_no"]));
					PresetChanged();
					break;
			}
		}

		public override void LoadFromResponse(string apiname, dynamic data)
		{
			switch (apiname)
			{
				case "api_get_member/preset_deck":
					{
                        this.MaximumCount = (int)data.api_max_num;

                        this.Presets.Clear();

						foreach (KeyValuePair<string, dynamic> elem in data.api_deck)
						{
							var preset = new FleetPresetData();
							preset.LoadFromResponse(apiname, elem.Value);
                            this.Presets.Add(preset);
						}
						PresetChanged();
					}
					break;

				case "api_req_hensei/preset_register":
					{
						int id = (int)data.api_preset_no;
						if (this.Presets.ContainsKey(id))
						{
                            this.Presets[id].LoadFromResponse(apiname, data);
						}
						else
						{
							var preset = new FleetPresetData();
							preset.LoadFromResponse(apiname, data);
                            this.Presets.Add(preset);
						}
						PresetChanged();
					}
					break;
			}

			base.LoadFromResponse(apiname, (object)data);
		}


		public FleetPresetData this[int index] => this.Presets[index];

	}
}
