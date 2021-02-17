using System.Collections.Generic;

public static class ContainerExtension
{
    public static IDictionary<T, VALUE> Union<T, VALUE>(this IDictionary<T, VALUE> dic, IDictionary<T, VALUE> target)
    {
        foreach (var val in target)
        {
            if (dic.ContainsKey(val.Key) == true)
            {
                continue;
            }

            dic.Add(val.Key, val.Value);
        }

        return dic;
    }

    public static void ForEach<T, VALUE>(this IDictionary<T, VALUE> dic, Action<T, VALUE> action)
    {
        foreach (var i in dic)
        {
            action.Invoke(i.Key, i.Value);
        }
    }

    public static void Foreach<VALUE>(this VALUE[] array, Action<VALUE> action)
    {
        for (int i = 0; i < array.Length; i++)
        {
            action.Invoke(array[i]);
        }
    }

    public static void Foreach(this JArray array, Action<JToken> action)
    {
        for (int i = 0; i < array.Count; i++)
        {
            action.Invoke(array[i]);
        }
    }

    public static void Foreach<TKey, TValue>(this IDictionary<TKey, TValue> dic, System.Action<TKey, TValue> action)
    {
        foreach (var i in dic)
        {
            action.Invoke(i.Key, i.Value);
        }
    }
}
