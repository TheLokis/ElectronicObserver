using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ElectronicObserver.Utility
{
    public static class UtilExtension
    {
        public static string DataTypeToName(this TranslateType type)
        {
            switch (type)
            {
                case TranslateType.AkashiData:
                    return "개수공창 목록";
                case TranslateType.Equipment:
                    return "장비 번역";
                case TranslateType.EquipmentType:
                    return "장비 종류 번역";
                case TranslateType.ShipName:
                    return "함선 이름 번역";
                case TranslateType.ShipSuffix:
                    return "함선 개장명 번역";
                case TranslateType.ShipType:
                    return "함선종류 번역";
                case TranslateType.OperationMap:
                    return "해역 이름 번역";
                case TranslateType.OperationSortie:
                    return "적 함대 번역";
                case TranslateType.QuestDetail:
                    return "퀘스트 정보 번역";
                case TranslateType.QuestTitle:
                    return "퀘스트 이름 번역";
                case TranslateType.ExpeditionDetail:
                    return "원정 상세 번역";
                case TranslateType.ExpeditionTitle:
                    return "원정 이름 번역";
                case TranslateType.ExpeditionData:
                    return "원정 데이터 목록";
                case TranslateType.Items:
                    return "아이템 이름 번역";
                case TranslateType.FitData:
                    return "피트 정보 목록";
                case TranslateType.NodeData:
                    return "해역 노드 목록";
                case TranslateType.None:
                default:
                    return "???";
            }
        }
    }
}
