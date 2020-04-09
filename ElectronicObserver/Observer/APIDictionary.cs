using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Observer
{


	public class APIDictionary : IReadOnlyDictionary<string, APIBase>
	{

		private readonly IDictionary<string, APIBase> dict;

		public APIDictionary()
			: this(new List<APIBase>())
		{
		}

		public APIDictionary(IEnumerable<APIBase> source)
		{
            this.dict = source.ToDictionary(x => x.APIName);
		}


		internal void Add(APIBase data)
		{
            this.dict.Add(data.APIName, data);
		}

		internal void Remove(APIBase data)
		{
            this.dict.Remove(data.APIName);
		}

		internal void Remove(string apiname)
		{
            this.dict.Remove(apiname);
		}

		internal void Clear()
		{
            this.dict.Clear();
		}


		public void OnRequestReceived(string apiname, Dictionary<string, string> data)
		{
			if (this.dict.ContainsKey(apiname) && this.dict[apiname].IsRequestSupported)
			{
                this.dict[apiname].OnRequestReceived(data);
			}
		}

		public void OnResponseReceived(string apiname, dynamic data)
		{
			if (this.dict.ContainsKey(apiname) && this.dict[apiname].IsResponseSupported)
			{
                this.dict[apiname].OnResponseReceived(data);
			}
		}


		public bool ContainsKey(string key)
		{
			return this.dict.ContainsKey(key);
		}

		public IEnumerable<string> Keys => this.dict.Keys;

		public bool TryGetValue(string key, out APIBase value)
		{
			return this.dict.TryGetValue(key, out value);
		}

		public IEnumerable<APIBase> Values => this.dict.Values;

		public APIBase this[string key] => this.dict.ContainsKey(key) ? this.dict[key] : null;

		public int Count => this.dict.Count;

		public IEnumerator<KeyValuePair<string, APIBase>> GetEnumerator()
		{
			return this.dict.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.dict.GetEnumerator();
		}
	}

}
