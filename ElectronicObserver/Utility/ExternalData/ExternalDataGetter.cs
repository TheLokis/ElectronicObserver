using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;
using ElectronicObserver.Data;

namespace ElectronicObserver.Utility
{
    public partial class ExternalDataReader
    {
        public string GetTranslation(string jpString, DataType type, int id = -1)
        {
            try
            {
                string translated = jpString;
                var translationList = this.GetData(type);
                if (translationList == null)
                {
                    return translated;
                }

                if (id != -1)
                {
                    if (this.GetTranslation(id, translationList, ref translated, type) == true)
                    {
                        return translated;
                    }
                }
                else
                {
                    if (this.GetTranslation(jpString, translationList, ref translated, type) == true)
                    {
                        return translated;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Add(3, "번역 파일을 불러오는데 실패했습니다. : " + e.Message + ":" + type);
                return jpString;
            }

            return jpString;
        }

        public bool GetTranslation(int id, JObject translationList, ref string translate, DataType type)
        {
            var founded = translationList.TryGetValue(id.ToString(), out JToken value);
            if (founded == false || value == null)
            {
                return false;
            }

            translate = value.ToString();
            return true;
        }

        public bool GetTranslation(string jpString, JObject translationList, ref string translate, DataType type, string shipSuffix = "")
        {
            var founded = translationList.TryGetValue(jpString, out JToken value);
            if (founded == false || value == null)
            {
                if (type == DataType.ShipName)
                {
                    var suffixList = this.GetData(DataType.ShipSuffix);
                    foreach (var suffix in suffixList)
                    {
                        if (jpString.Contains(suffix.Key.ToString()) == true)
                        {
                            translate = jpString.Remove(jpString.Length - suffix.Key.ToString().Length);
                            return this.GetTranslation(translate, translationList, ref translate, DataType.ShipName, suffix.Value.ToString());
                        }
                    }
                }

                translate = jpString;
                return false;
            }

            translate = value.ToString() + shipSuffix;
            return true;
        }

        public bool CheckWeightData(int shipId)
        {
            var fitInfo = this.GetData(DataType.FitData)[shipId.ToString()];
            if (fitInfo == null)
                return false;

            if (fitInfo.Type == JTokenType.Integer)
                return true;

            return false;
        }

        public JToken GetExpeditionData(string id)
        {
            var obj = this.GetData(DataType.ExpeditionData)["Expedition"] as JArray;
            if (obj == null) return null;

            JToken value = null;

            obj.Foreach(data =>
            {
                if (data["ID"].ToString().Equals(id) == true)
                {
                    value = data;
                }
            });

            return value;
        }

        public void GetFit(Dictionary<string, string> data)
        {
            int shipId = int.Parse(data["api_id"]);
            int eqId = int.Parse(data["api_item_id"]);
            if (eqId == -1)
                return;
            
            KCDatabase db = KCDatabase.Instance;
            if (db.Equipments[eqId].MasterEquipment.IsLargeGun)
            {
                bool married        = db.Ships[shipId].Level >= 100;
                int masterShipId    = db.Ships[shipId].MasterShip.ID;
                int masterEquipId   = db.Equipments[eqId].MasterEquipment.ID;
                if (this.CheckWeightData(masterShipId) == true)
                {
                    masterShipId = this.GetData(DataType.FitData)[masterShipId.ToString()].ToObject<int>();
                }

                var WeightData = this.GetData(DataType.FitData)[masterShipId.ToString()];
                if (WeightData == null)
                {
                    Window.FormMain.Instance.fInformation.ShowFitInfo(masterShipId, masterEquipId);
                    return;
                }

                var classData       = (JObject)WeightData["weightClass"];
                var accuracyData    = (JObject)WeightData["accuracy"];
                string weightType   = "unknown";
                foreach (var weightClass in classData)
                {
                    int[] weights = weightClass.Value.ToObject<int[]>();
                    if (weights.Contains(masterEquipId))
                    {
                        weightType = weightClass.Key;
                        break;
                    }
                }

                if (weightType.Equals("unknown"))
                {
                    Window.FormMain.Instance.fInformation.ShowFitInfo(masterShipId, masterEquipId);
                }
                else
                {
                    int[] fitBonus = new int[] { accuracyData[weightType]["day"].ToObject<int>(), accuracyData[weightType]["night"].ToObject<int>() };
                    float marryData = 0;
                    if (accuracyData[weightType]["married"] != null)
                        marryData = accuracyData[weightType]["married"].ToObject<float>();

                    Window.FormMain.Instance.fInformation.ShowFitInfo(db.Ships[shipId].Name, db.MasterEquipments[masterEquipId].Name, fitBonus, marryData, married);
                }
            }
        }

        public JObject GetData(DataType type)
        {
            return this._externalDatas.Find(i => i.DataType == type).Data;
        }
    }
}
