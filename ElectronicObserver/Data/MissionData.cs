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
        public int MissionID	=> (int)this.RawData.api_id;

        /// <summary>
        /// 表示される遠征ID
        /// </summary>
        public string DisplayID => this.RawData.api_disp_no;

        /// <summary>
        /// 海域カテゴリID
        /// </summary>
        public int MapAreaID	=> (int)this.RawData.api_maparea_id;

		/// <summary>
		/// 遠征名 번역됨
		/// </summary>
		public string Name
        {
            get { return FormMain.Instance.Translator.GetTranslation(string.Empty, Utility.DataType.ExpeditionTitle, (int)this.RawData.api_id); }
        }

        /// <summary>
        /// 説明文 번역됨
        /// </summary>
        public string Detail
        {
            get { return FormMain.Instance.Translator.GetTranslation(this.RawData.api_name, Utility.DataType.ExpeditionDetail); }
        }

        /// <summary>
        /// 遠征時間(分単位)
        /// </summary>
        public int Time			=> (int)this.RawData.api_time;

		/// <summary>
		/// 難易度
		/// </summary>
		public int Difficulty	=> (int)this.RawData.api_difficulty;

		/// <summary>
		/// 消費燃料割合
		/// </summary>
		public double Fuel		=> this.RawData.api_use_fuel;

		/// <summary>
		/// 消費弾薬割合
		/// </summary>
		public double Ammo		=> this.RawData.api_use_bull;

		//win_item<n>

		/// <summary>
		/// 遠征中断・強制帰投可能かどうか
		/// </summary>
		public bool Cancelable	=> (int)this.RawData.api_return_flag != 0;



		public int ID => this.MissionID;
		public override string ToString() => $"[{this.MissionID}] {this.Name}";
	}

}
