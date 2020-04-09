using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{

	/// <summary>
	/// 消費アイテムのデータを保持します。
	/// </summary>
	public class UseItem : ResponseWrapper, IIdentifiable
	{

		/// <summary>
		/// アイテムID
		/// </summary>
		public int ItemID => (int)this.RawData.api_id;

		/// <summary>
		/// 個数
		/// </summary>
		public int Count => (int)this.RawData.api_count;


		public UseItemMaster MasterUseItem => KCDatabase.Instance.MasterUseItems[this.ItemID];


		public int ID => this.ItemID;
		public override string ToString() => $"[{this.ItemID}] {this.MasterUseItem.Name} x {this.Count}";
	}

}
