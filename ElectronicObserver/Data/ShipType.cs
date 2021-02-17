using DynaJson;
using ElectronicObserver.Window;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{

	/// <summary>
	/// 艦種
	/// </summary>
	public class ShipType : ResponseWrapper, IIdentifiable
	{

		/// <summary>
		/// 艦種ID
		/// </summary>
		public int TypeID => (int)this.RawData.api_id;

		/// <summary>
		/// 並べ替え順
		/// </summary>
		public int SortID => (int)this.RawData.api_sortno;

        /// <summary>
        /// 艦種名 번역됨
        /// </summary>
        //public string Name => RawData.api_name;
        public string Name
        {
            get { return FormMain.Instance.Translator.GetTranslation(this.RawData.api_name, Utility.TranslateType.ShipType); }
        }



        /// <summary>
        /// 入渠時間係数
        /// </summary>
        public int RepairTime => (int)this.RawData.api_scnt;


        //TODO: api_kcnt


        /// <summary>
        /// 装備可否フラグ
        /// </summary>
        private int[] _equippableCategories;
        public ReadOnlyCollection<int> EquippableCategories => Array.AsReadOnly(this._equippableCategories);


        /// <summary>
        /// 艦種ID
        /// </summary>
        public ShipTypes Type => (ShipTypes)this.TypeID;


		public int ID => this.TypeID;
		public override string ToString() => $"[{this.TypeID}] {this.Name}";



		public ShipType()
			: base()
		{

            this._equippableCategories = new int[0];
        }

		public override void LoadFromResponse(string apiname, dynamic data)
		{

			// api_equip_type の置換処理
			// checkme: 無駄が多い気がするのでもっといい案があったら是非
			data = JsonObject.Parse(Regex.Replace(data.ToString(), @"""(?<id>\d+?)""", @"""api_id_${id}"""));

			base.LoadFromResponse(apiname, (object)data);


			if (this.IsAvailable)
			{
                IEnumerable<int> getType()
                {
                    foreach (KeyValuePair<string, object> type in this.RawData.api_equip_type)
                    {
                        if ((double)type.Value != 0)
                            yield return Convert.ToInt32(type.Key.Substring(7));     //skip api_id_
                    }
                }

                this._equippableCategories = getType().ToArray();
            }
		}

	}

}
