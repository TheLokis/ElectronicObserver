using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronicObserver.Window;

namespace ElectronicObserver.Data
{

	/// <summary>
	/// 遠征データを保持します。
	/// </summary>
	public class MissionData : APIWrapper, IIdentifiable
	{

        /// 遠征ID
        /// </summary>
        public int MissionID => (int)RawData.api_id;

        /// <summary>
        /// 表示される遠征ID
        /// </summary>
        public string DisplayID => RawData.api_disp_no;

        /// <summary>
        /// 海域カテゴリID
        /// </summary>
        public int MapAreaID => (int)RawData.api_maparea_id;

		/// <summary>
		/// 遠征名 번역됨
		/// </summary>
		public string Name
        {
            get { return FormMain.Instance.Translator.GetTranslation(RawData.api_name, Utility.TranslationType.ExpeditionTitle); }
        }

        /// <summary>
        /// 説明文 번역됨
        /// </summary>
        public string Detail
        {
            get { return FormMain.Instance.Translator.GetTranslation(RawData.api_name, Utility.TranslationType.ExpeditionDetail); }
        }

        /// <summary>
        /// 遠征時間(分単位)
        /// </summary>
        public int Time => (int)RawData.api_time;

		/// <summary>
		/// 難易度
		/// </summary>
		public int Difficulty => (int)RawData.api_difficulty;

		/// <summary>
		/// 消費燃料割合
		/// </summary>
		public double Fuel => RawData.api_use_fuel;

		/// <summary>
		/// 消費弾薬割合
		/// </summary>
		public double Ammo => RawData.api_use_bull;

		//win_item<n>

		/// <summary>
		/// 遠征中断・強制帰投可能かどうか
		/// </summary>
		public bool Cancelable => (int)RawData.api_return_flag != 0;



		public int ID => MissionID;
		public override string ToString() => $"[{MissionID}] {Name}";
	}

}
