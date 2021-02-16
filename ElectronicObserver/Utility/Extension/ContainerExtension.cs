using System.Collections.Generic;

public static class ContainerExtension
{
    public static void Foreach<TKey, TValue>(this IDictionary<TKey, TValue> dic, System.Action<TKey, TValue> action)
    {
        foreach (var i in dic)
        {
            action.Invoke(i.Key, i.Value);
        }
    }
}
