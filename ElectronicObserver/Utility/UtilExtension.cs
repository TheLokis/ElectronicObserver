using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ElectronicObserver.Utility
{
    public static class UtilExtension
    {
        public static IDictionary<T, VALUE> Union<T, VALUE>(this IDictionary<T, VALUE> dic, IDictionary<T,VALUE> target)
        {
            foreach(var val in target)
            {
                if(dic.ContainsKey(val.Key) == true)
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
            for(int i = 0; i < array.Length;i++)
            {
                action.Invoke(array[i]);
            }
        }

        public static void Foreach(this JArray array, Action<JToken> action)
        {
            for(int i = 0; i < array.Count;i++)
            {
                action.Invoke(array[i]);
            }
        }

        public static string DataTypeToName(this DataType type)
        {
            switch (type)
            {
                case DataType.AkashiData:
                    return "개수공창 목록";
                case DataType.Equipment:
                    return "장비 번역";
                case DataType.EquipmentType:
                    return "장비 종류 번역";
                case DataType.ShipName:
                    return "함선 이름 번역";
                case DataType.ShipSuffix:
                    return "함선 개장명 번역";
                case DataType.ShipType:
                    return "함선종류 번역";
                case DataType.OperationMap:
                    return "해역 이름 번역";
                case DataType.OperationSortie:
                    return "적 함대 번역";
                case DataType.QuestDetail:
                    return "퀘스트 정보 번역";
                case DataType.QuestTitle:
                    return "퀘스트 이름 번역";
                case DataType.ExpeditionDetail:
                    return "원정 상세 번역";
                case DataType.ExpeditionTitle:
                    return "원정 이름 번역";
                case DataType.ExpeditionData:
                    return "원정 데이터 목록";
                case DataType.Items:
                    return "아이템 이름 번역";
                case DataType.FitData:
                    return "피트 정보 목록";
                case DataType.NodeData:
                    return "해역 노드 목록";
                case DataType.None:
                default:
                    return "???";
            }
        }
    }
}
