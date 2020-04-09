using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data
{

	/// <summary>
	/// IDを持つデータのリストを保持します。
	/// </summary>
	/// <typeparam name="TData"></typeparam>
	public class IDDictionary<TData> : IReadOnlyDictionary<int, TData> where TData : class, IIdentifiable
	{

		private readonly IDictionary<int, TData> dict;

		public IDDictionary()
			: this(new List<TData>())
		{
		}

		public IDDictionary(IEnumerable<TData> source)
		{
            this.dict = source.ToDictionary(x => x.ID);
		}


		internal void Add(TData data)
		{
            this.dict.Add(data.ID, data);
		}

		internal void Remove(TData data)
		{
            this.dict.Remove(data.ID);
		}

		internal void Remove(int id)
		{
            this.dict.Remove(id);
		}

		internal int RemoveAll(Predicate<TData> predicate)
		{
			var removekeys = this.dict.Values.Where(elem => predicate(elem)).Select(elem => elem.ID).ToArray();

			foreach (var key in removekeys)
			{
                this.dict.Remove(key);
			}

			return removekeys.Count();
		}

		internal void Clear()
		{
            this.dict.Clear();
		}


		public bool ContainsKey(int key)
		{
			return this.dict.ContainsKey(key);
		}

		public IEnumerable<int> Keys => this.dict.Keys;

		public bool TryGetValue(int key, out TData value)
		{
			return this.dict.TryGetValue(key, out value);
		}

		public IEnumerable<TData> Values => this.dict.Values;

		public TData this[int key] => this.dict.ContainsKey(key) ? this.dict[key] : null;

		public int Count => this.dict.Count;

		public IEnumerator<KeyValuePair<int, TData>> GetEnumerator()
		{
			return this.dict.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.dict.GetEnumerator();
		}
	}


}
