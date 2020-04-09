using ElectronicObserver.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Battle
{

    /// <summary>
    /// 戦闘結果のデータを扱います。
    /// </summary>
    public class BattleResultData : ResponseWrapper
    {

        /// <summary>
        /// 現在保存されているAPIの名前
        /// </summary>
        private string _APIName { get; set; }



        // :(
        /// <summary>
        /// 演習の結果かどうか
        /// </summary>
        private bool IsPractice => this._APIName == "api_req_practice/battle_result";




        /// <summary>
        /// 勝利ランク
        /// </summary>
        public string Rank => this.RawData.api_win_rank;

        /// <summary>
        /// 獲得提督経験値
        /// </summary>
        public int AdmiralExp => (int)this.RawData.api_get_exp;

        /// <summary>
        /// MVP艦のインデックス (1-6, -1=なし)
        /// </summary>
        public int MVPIndex => (int)this.RawData.api_mvp;

        /// <summary>
        /// 随伴艦隊 MVP 艦のインデックス (1-6, -1=なし)
        /// </summary>
        public int MVPIndexCombined => this.RawData.api_mvp_combined() && this.RawData.api_mvp_combined != null ? (int)this.RawData.api_mvp_combined : -1;


        /// <summary>
        /// 獲得基本経験値
        /// </summary>
        public int BaseExp => (int)this.RawData.api_get_base_exp;


        /// <summary>
        /// 主力艦隊の入手経験値リスト [0-5]
        /// 欠番は -1
        /// </summary>
        public int[] ExpList
        {
            get
            {
                if (!this.RawData.api_get_ship_exp())
                    return new int[6];

                var src = (int[])this.RawData.api_get_ship_exp;
                var ret = new int[Math.Max(src.Length - 1, 6)];
                Array.Copy(src, 1, ret, 0, src.Length - 1);
                return ret;
            }
        }

        /// <summary>
        /// 随伴艦隊の入手経験値リスト [0-5]
        /// 欠番は -1
        /// </summary>
        public int[] ExpListCombined
        {
            get
            {
                if (!this.RawData.api_get_ship_exp_combined())
                    return new int[6];

                var src = (int[])this.RawData.api_get_ship_exp_combined;
                var ret = new int[Math.Max(src.Length - 1, 6)];
                Array.Copy(src, 1, ret, 0, src.Length - 1);
                return ret;
            }
        }

        /// <summary>
        /// 主力艦隊のレベルアップリスト [所属艦船数]
        /// [0]=現在のexp, [1]=(あれば)次のレベルの経験値, [2]=(あれば)その次のレベルの経験値, ...
        /// </summary>
        public int[][] LevelUpList
        {
            get
            {
                if (!this.RawData.api_get_exp_lvup())
                    return new int[0][];

                var ret = new List<int[]>();
                foreach (var data in this.RawData.api_get_exp_lvup)
                {
                    ret.Add((int[])data);
                }
                return ret.ToArray();
            }
        }

        /// <summary>
        /// 随伴艦隊のレベルアップリスト [所属艦船数]
        /// [0]=現在のexp, [1]=(あれば)次のレベルの経験値, [2]=(あれば)その次のレベルの経験値, ...
        /// </summary>
        public int[][] LevelUpListCombined
        {
            get
            {
                if (!this.RawData.api_get_exp_lvup_combined())
                    return new int[0][];

                var ret = new List<int[]>();
                foreach (var data in this.RawData.api_get_exp_lvup_combined)
                {
                    ret.Add((int[])data);
                }
                return ret.ToArray();
            }
        }


        //lostflag


        /// <summary>
        /// 敵艦隊名 번역됨
        /// </summary>
        public string EnemyFleetName
        { get
            { return FormMain.Instance.Translator.GetTranslation(this.RawData.api_enemy_info.api_deck_name, Utility.DataType.OperationSortie); }
        }
    //undone: 複数の battleresult に対応させる


    /// <summary>
    /// ドロップした艦船のID
    /// </summary>
    public int DroppedShipID
		{
			get
			{
				if (this.IsPractice)
					return -1;
				if ((int)this.RawData.api_get_flag[1] == 0)
					return -1;

				return (int)this.RawData.api_get_ship.api_ship_id;
			}
		}


		/// <summary>
		/// ドロップしたアイテムのID
		/// </summary>
		public int DroppedItemID
		{
			get
			{
				if (this.IsPractice)
					return -1;
				if ((int)this.RawData.api_get_flag[0] == 0)
					return -1;

				return (int)this.RawData.api_get_useitem.api_useitem_id;
			}
		}


		/// <summary>
		/// ドロップした装備のID(現在未使用)
		/// </summary>
		public int DroppedEquipmentID
		{
			get
			{
				if (this.IsPractice)
					return -1;
				if ((int)this.RawData.api_get_flag[2] == 0)
					return -1;

				return (int)this.RawData.api_get_slotitem.api_slotitem_id;
			}
		}




		/// <summary>
		/// 護衛退避可能か
		/// </summary>
		public bool CanEscape
		{
			get
			{
				if (!this.RawData.api_escape())
				{
					return false;
				}
				else
				{
					return (int)this.RawData.api_escape != 0;
				}
			}
		}

		/// <summary>
		/// 退避艦のインデックス
		/// </summary>
		public IEnumerable<int> EscapingShipIndex
		{
			get
			{
				if (!this.RawData.api_escape())
				{
					return null;
				}
				else if (!this.RawData.api_escape.api_tow_idx())
				{
					return new int[1] { (int)this.RawData.api_escape.api_escape_idx[0] };
				}
				else
				{
					return new int[2] { (int)this.RawData.api_escape.api_escape_idx[0], (int)this.RawData.api_escape.api_tow_idx[0] };
				}

			}
		}


		public override void LoadFromResponse(string apiname, dynamic data)
		{
			base.LoadFromResponse(apiname, (object)data);

            this._APIName = apiname;

		}
	}

}

