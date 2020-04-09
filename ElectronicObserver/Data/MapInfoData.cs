using ElectronicObserver.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{

	/// <summary>
	/// 海域情報を保持します。
	/// </summary>
	public class MapInfoData : APIWrapper, IIdentifiable
	{

		/// <summary>
		/// 海域ID
		/// </summary>
		public int MapID => (int)this.RawData.api_id;

		/// <summary>
		/// 海域カテゴリID
		/// </summary>
		public int MapAreaID => (int)this.RawData.api_maparea_id;

		/// <summary>
		/// 海域カテゴリ内番号
		/// </summary>
		public int MapInfoID => (int)this.RawData.api_no;

		/// <summary>
		/// 海域名
		/// </summary>
		public string Name
        {
            get { return FormMain.Instance.Translator.GetTranslation(this.RawData.api_name, Utility.DataType.OperationMap); }
        }

		/// <summary>
		/// 難易度
		/// </summary>
		public int Difficulty => (int)this.RawData.api_level;

		/// <summary>
		/// 作戦名
		/// </summary>
		public string OperationName => this.RawData.api_opetext;

		/// <summary>
		/// 作戦情報
		/// </summary>
		public string Information => ((string)this.RawData.api_infotext).Replace("<br>", "");

        /// <summary>
        /// クリアに必要な撃破回数(主にEO海域)
        /// 存在しなければ -1
        /// </summary>
        public int RequiredDefeatedCount { get; private set; }




        /// <summary>
        /// 攻略済みかどうか
        /// </summary>
        public bool IsCleared { get; private set; }

		/// <summary>
		/// 現在の撃破回数
		/// </summary>
		public int CurrentDefeatedCount { get; private set; }

		/// <summary>
		/// 現在の海域HP
		/// </summary>
		public int MapHPCurrent { get; private set; }

		/// <summary>
		/// 海域HPの最大値
		/// </summary>
		public int MapHPMax { get; private set; }

		/// <summary>
		/// 現在選択されている難易度(甲乙丙丁)
		/// </summary>
		public int EventDifficulty { get; private set; }

		/// <summary>
		/// 海域ゲージの種別
		/// 2=HP制(デフォルト), 3=TP制
		/// </summary>
		public int GaugeType { get; private set; }
		
		/// <summary>
		/// 現在のゲージ本数　未指定なら 0
		/// </summary>
		public int CurrentGaugeIndex { get; private set; }



		public MapInfoData()
			: base()
		{
            this.RequiredDefeatedCount = -1;
            this.IsCleared = false;
            this.CurrentDefeatedCount = 0;
            this.MapHPCurrent = this.MapHPMax = 0;
            this.EventDifficulty = -1;
            this.GaugeType = 2;
		}


		public override void LoadFromResponse(string apiname, dynamic data)
		{

			switch (apiname)
			{
				case "api_start2/getData":
					base.LoadFromResponse(apiname, (object)data);
					break;

				case "api_get_member/mapinfo":
                    this.IsCleared = (int)data.api_cleared != 0;
                    this.RequiredDefeatedCount = data.api_required_defeat_count() ? (int)data.api_required_defeat_count : -1;
                    this.CurrentDefeatedCount = data.api_defeat_count() ? (int)data.api_defeat_count : 0;
					if (data.api_eventmap())
					{
                        this.MapHPCurrent = data.api_eventmap.api_now_maphp() ? (int)data.api_eventmap.api_now_maphp : 0;
                        this.MapHPMax = data.api_eventmap.api_max_maphp() ? (int)data.api_eventmap.api_max_maphp : 0;
                        this.EventDifficulty = data.api_eventmap.api_selected_rank() ? (int)data.api_eventmap.api_selected_rank : -1;
					}
                    this.GaugeType = data.api_gauge_type() ? (int)data.api_gauge_type : 2;
                    this.CurrentGaugeIndex = data.api_gauge_num() ? (int)data.api_gauge_num : 0;
                    break;
			}

		}


		public override void LoadFromRequest(string apiname, Dictionary<string, string> data)
		{
			base.LoadFromRequest(apiname, data);

			switch (apiname)
			{
				case "api_req_map/select_eventmap_rank":
                    this.EventDifficulty = int.Parse(data["api_rank"]);
					break;
			}
		}



		public int ID => this.MapID;

		public override string ToString() => $"[{this.MapID}] {this.Name}";
	}


}
