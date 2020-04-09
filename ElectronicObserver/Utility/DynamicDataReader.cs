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
    public class ExternalData
    {
        public string Version { get; set; }
        public JObject Data { get; set; }
        public DataType DataType { get; set; }
    }

    public enum DataType
    {
        Equipment,
        EquipmentType,
        ShipName,
        ShipSuffix,
        ShipType,
        OperationMap,
        OperationSortie,
        QuestDetail,
        QuestTitle,
        ExpeditionDetail,
        ExpeditionTitle,
        ExpeditionData,
        Items,
        FitData,
        NodeData,
        AkashiData,
        None,
    }

    public class DynamicDataReader
    {
        private List<ExternalData> _externalDatas = new List<ExternalData>();
        private static string _hubsite = "https://thelokis.github.io/EOTranslation/Translation/";
        //private static string _hubsite = "http://172.30.1.20:8080/Translations/";

        #region Singleton
        private static readonly DynamicDataReader _instance = new DynamicDataReader();
        public static DynamicDataReader Instance = _instance;
        #endregion

        internal DynamicDataReader()
        {
            for (int i = 0; i < (int)DataType.None; i++)
            {
                var data = new ExternalData();
                var dataType = (DataType)i;
                data.DataType = dataType;

                if (File.Exists($"Translation\\{dataType}.json") == true)
                {
                    try
                    {
                        data.Data = JObject.Parse(File.ReadAllText($"Translation\\{dataType}.json"));
                        if (data.Data.ContainsKey("version") == true)
                        {
                            data.Version = "1.0A";
                        }
                        else
                        {
                            data.Version = data.Data["Version"].ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        data.Version = "1.0A";
                        data.Data = null;

                        Logger.Add(2, "번역 파일 적용에 실패했습니다." + (DataType)i);
                    }
                }

                this._externalDatas.Add(data);
            }

            this.CheckFileUpdates();
        }

        private void CheckFileUpdates()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                if (Directory.Exists("Translations") == true)
                {
                    Logger.Add(2, "R23 버전 이전 번역 파일 폴더를 삭제합니다..");
                    Directory.Delete("Translations", true);

                    Logger.Add(2, "R23 버전 이전 번역 파일 폴더를 삭제했습니다.");
                }
            } 
            catch (Exception e)
            {
                ErrorReporter.SendErrorReport(e, "R23 버전 이전 번역 파일 디렉토리 삭제에 실패했습니다. 수동으로 삭제해주세요.");
            }

            Directory.CreateDirectory("Translation");

            var versionManifest = new JObject();
            string locale = Thread.CurrentThread.CurrentCulture.Name;
            WebRequest rq = HttpWebRequest.Create(_hubsite + "VersionManifest.json");
            using (WebResponse resp = rq.GetResponse())
            {
                using (Stream responseStream = resp.GetResponseStream())
                {
                    var versionInfoText = "";
                    var reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    while (!reader.EndOfStream)
                    {
                        versionInfoText += reader.ReadLine();
                    }

                    versionManifest = JObject.Parse(versionInfoText);
                }
            }

            this._externalDatas.ForEach(i =>
            {
                var newVer = versionManifest[i.DataType.ToString()].ToString();
                var fileDownloadCompleted = false;
                var currentFileName = $"{i.DataType}.json";

                if (i.Version == null || i.Version.Equals(newVer) == false)
                {
                    WebRequest r2 = HttpWebRequest.Create($"{_hubsite}/{currentFileName}");
                    using (WebResponse resp = r2.GetResponse())
                    {
                        using (Stream output = File.OpenWrite($"Translation\\{currentFileName}_temp"))
                        {
                            using (Stream input = resp.GetResponseStream())
                            {
                                byte[] buffer = new byte[8192];
                                int bytesRead;
                                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    output.Write(buffer, 0, bytesRead);
                                }

                                fileDownloadCompleted = true;
                                input.Close();
                                resp.Close();
                            }

                            output.Close();
                        }
                    }
                }

                try
                {
                    if (fileDownloadCompleted == true)
                    {
                        if (File.Exists($"Translation\\{currentFileName}") == true)
                        {
                            File.Delete($"Translation\\{currentFileName}");
                        }

                        if (File.Exists($"Translation\\{currentFileName}") == false)
                        {
                            File.Move($"Translation\\{currentFileName}_temp", $"Translation\\{currentFileName}");
                        }

                        Logger.Add(2, $"{this.DataTypeToName(i.DataType)}이 업데이트 되었습니다.");
                    }

                    i.Data = JObject.Parse(File.ReadAllText($"Translation\\{currentFileName}"));
                }
                catch (Exception e)
                {
                    Logger.Add(2, "번역 파일 업데이트에 실패했습니다. 파일 : " + i.DataType + ":" + e.GetBaseException());
                }
            });
        }

        public JObject GetData(DataType type)
        {
            return this._externalDatas.Find(i => i.DataType == type).Data;
        }

        public JToken GetExpeditionData(string id)
        {
            var obj = this.GetData(DataType.ExpeditionData)["Expedition"] as JArray;
            if (obj == null) return null;

            for (int i = 0; i < obj.Count; i++)
            {
                var data = obj[i];
                if (data["ID"].ToString().Equals(id) == true)
                {
                    return data;
                }
            }

            return null;
        }

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

        public void Get_Fit(Dictionary<string, string> data)
        {
            int Shipid = int.Parse(data["api_id"]);
            int Equipmentid = int.Parse(data["api_item_id"]);
            if (Equipmentid == -1)
                return;
            KCDatabase db = KCDatabase.Instance;
            if (db.Equipments[Equipmentid].MasterEquipment.IsLargeGun)
            {
                bool Married = db.Ships[Shipid].Level >= 100;
                int Master_Shipid = db.Ships[Shipid].MasterShip.ID;
                int Master_Itemid = db.Equipments[Equipmentid].MasterEquipment.ID;
                if (this.Check_WeightData(Master_Shipid))
                {
                    Master_Shipid = this.GetData(DataType.FitData)[Master_Shipid.ToString()].ToObject<int>();
                }
                var WeightData = this.Find_WeightData(Master_Shipid);
                if (WeightData == null)
                {
                    Window.FormMain.Instance.fInformation.Show_FitInfo(Master_Shipid, Master_Itemid);
                    return;
                }
                JObject Class_Data = (JObject)WeightData["weightClass"];
                JObject Accuracy_Data = (JObject)WeightData["accuracy"];
                string Weight_Type = "unknown";
                foreach (var WeightClass in Class_Data)
                {
                    int[] Weights = WeightClass.Value.ToObject<int[]>();
                    if (Weights.Contains(Master_Itemid))
                    {
                        Weight_Type = WeightClass.Key;
                        break;
                    }
                }
                if (Weight_Type.Equals("unknown"))
                {
                    Window.FormMain.Instance.fInformation.Show_FitInfo(Master_Shipid, Master_Itemid);
                }
                else
                {
                    int[] Fit_Bonus = new int[] { Accuracy_Data[Weight_Type]["day"].ToObject<int>(), Accuracy_Data[Weight_Type]["night"].ToObject<int>() };
                    float Marry_Data = 0;
                    if (Accuracy_Data[Weight_Type]["married"] != null)
                        Marry_Data = Accuracy_Data[Weight_Type]["married"].ToObject<float>();
                    Window.FormMain.Instance.fInformation.Show_FitInfo(db.Ships[Shipid].Name, db.MasterEquipments[Master_Itemid].Name, Fit_Bonus, Marry_Data, Married);
                }
            }
        }

        public string DataTypeToName(DataType type)
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

        public bool Check_WeightData(int Shipid)
        {
            var Fit_Info = this.GetData(DataType.FitData)[Shipid.ToString()];
            if (Fit_Info == null)
                return false;

            if (Fit_Info.Type == JTokenType.Integer)
                return true;

            return false;
        }

        public JObject Find_WeightData(int Shipid)
        {
            var Fit_Info = this.GetData(DataType.FitData)[Shipid.ToString()];
            if (Fit_Info != null)
                return (JObject)Fit_Info;
            else
                return null;
        }

        public void ForeachExternalData(System.Action<ExternalData> action)
        {
            this._externalDatas.ForEach(action);
        }
    }
}