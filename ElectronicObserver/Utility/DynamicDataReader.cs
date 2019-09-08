using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ElectronicObserver.Data;

namespace ElectronicObserver.Utility
{
    public class DynamicDataReader
    {
        private XDocument shipsXml;
        private XDocument shipTypesXml;
        private XDocument equipmentXml;
        private XDocument equipTypesXML;
        private XDocument operationsXml;
        private XDocument questsXml;
        private XDocument expeditionsXml;
        private XDocument expeditionsdataXml;
        private XDocument ItemsXml;
        private XDocument versionManifest;
        private JObject Akashi_Data;
        private JObject Akashi_Day;
        private JObject Node_Data;
        private JObject Fit_Data;
        private string AkashiDataversion;
        private string AkashiDayversion;
        private string shipsVersion;
        private string shipTypesVersion;
        private string equipmentVersion;
        private string equipTypesVersion;
        private string operationsVersion;
        private string questsVersion;
        private string expeditionsVersion;
        private string ItemsVersion;
        private string expeditionsdataVersion;
        private string nodedataversion;
        private string fitdataversion;

        private static string hubsite = "https://thelokis.github.io/EOTranslation/Translations/";

        public JObject Master_Akashi_Data { get { return Akashi_Data; } }

        public JObject Master_Akashi_Day { get { return Akashi_Day; } }

        public JObject Master_Node_Data { get { return Node_Data; } }

        public JObject Master_Fit_Data { get { return Fit_Data; } }

        #region Singleton

        private static readonly DynamicDataReader instance = new DynamicDataReader();

        public static DynamicDataReader Instance = instance;

        #endregion

        internal DynamicDataReader()
        {
            try
            {
                if (File.Exists("Translations\\Ships.xml")) this.shipsXml = XDocument.Load("Translations\\Ships.xml");
                if (File.Exists("Translations\\ShipTypes.xml")) this.shipTypesXml = XDocument.Load("Translations\\ShipTypes.xml");
                if (File.Exists("Translations\\Equipment.xml")) this.equipmentXml = XDocument.Load("Translations\\Equipment.xml");
                if (File.Exists("Translations\\EquipmentTypes.xml")) this.equipTypesXML = XDocument.Load("Translations\\EquipmentTYpes.xml");
                if (File.Exists("Translations\\Operations.xml")) this.operationsXml = XDocument.Load("Translations\\Operations.xml");
                if (File.Exists("Translations\\Quests.xml")) this.questsXml = XDocument.Load("Translations\\Quests.xml");
                if (File.Exists("Translations\\Expeditions.xml")) this.expeditionsXml = XDocument.Load("Translations\\Expeditions.xml");
                if (File.Exists("Translations\\Items.xml")) this.ItemsXml = XDocument.Load("Translations\\Items.xml");
                if (File.Exists("Translations\\ExpeditionData.xml")) this.expeditionsdataXml = XDocument.Load("Translations\\ExpeditionData.xml");
                if (File.Exists("Translations\\akashidata.json"))
                    Akashi_Data = JObject.Parse(File.ReadAllText("Translations\\akashidata.json"));

                if (File.Exists("Translations\\akashi_day.json"))
                    Akashi_Day = JObject.Parse(File.ReadAllText("Translations\\akashi_day.json"));

                if (File.Exists("Translations\\node_data.json"))
                    Node_Data = JObject.Parse(File.ReadAllText("Translations\\node_data.json"));

                if (File.Exists("Translations\\fit_data.json"))
                    Fit_Data = JObject.Parse(File.ReadAllText("Translations\\fit_data.json"));

            }
            catch (Exception ex)
            {
                Logger.Add(3, "Could not load translation file: " + ex.Message);
            }

            GetVersions();
            CheckForUpdates();
        }

        private void GetVersions()
        {
            try
            {
                this.shipsVersion = this.shipsXml.Root.Attribute("Version").Value;
            }
            catch (NullReferenceException)
            {
                this.shipsVersion = "0.0.0";
            }

            try
            {
                this.shipTypesVersion = this.shipTypesXml.Root.Attribute("Version").Value;
            }
            catch (NullReferenceException)
            {
                this.shipTypesVersion = "0.0.0";
            }

            try
            {
                this.equipmentVersion = this.equipmentXml.Root.Attribute("Version").Value;
            }
            catch (NullReferenceException)
            {
                this.equipmentVersion = "0.0.0";
            }

            try
            {
                this.equipTypesVersion = this.equipTypesXML.Root.Attribute("Version").Value;
            }
            catch (NullReferenceException)
            {
                this.equipTypesVersion = "0.0.0";
            }

            try
            {
                this.operationsVersion = this.operationsXml.Root.Attribute("Version").Value;
            }
            catch (NullReferenceException)
            {
                this.operationsVersion = "0.0.0";
            }

            try
            {
                this.questsVersion = this.questsXml.Root.Attribute("Version").Value;
            }
            catch (NullReferenceException)
            {
                this.questsVersion = "0.0.0";
            }

            try
            {
                this.expeditionsVersion = this.expeditionsXml.Root.Attribute("Version").Value;
            }
            catch (NullReferenceException)
            {
                this.expeditionsVersion = "0.0.0";
            }

            try
            {
                this.ItemsVersion = this.ItemsXml.Root.Attribute("Version").Value;
            }
            catch (NullReferenceException)
            {
                this.ItemsVersion = "0.0.0";
            }

            try
            {
                this.expeditionsdataVersion = this.expeditionsdataXml.Root.Attribute("Version").Value;
            }
            catch (NullReferenceException)
            {
                this.expeditionsdataVersion = "0.0.0";
            }

            try
            {
                this.AkashiDataversion = this.Akashi_Data["version"].ToString();
            }
            catch (NullReferenceException)
            {
                this.AkashiDataversion = "0.0.0";
            }

            try
            {
                this.AkashiDayversion = this.Akashi_Day["version"].ToString();
            }
            catch (NullReferenceException)
            {
                this.AkashiDayversion = "0.0.0";
            }

            try
            {
                this.nodedataversion = this.Node_Data["version"].ToString();
            }
            catch (NullReferenceException)
            {
                this.nodedataversion = "0.0.0";
            }

            try
            {
                this.fitdataversion = this.Fit_Data["version"].ToString();
            }
            catch (NullReferenceException)
            {
                this.fitdataversion = "0.0.0";
            }
        }

        private void CheckForUpdates()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Directory.CreateDirectory("Translations");
            string locale = Thread.CurrentThread.CurrentCulture.Name;
            WebRequest rq = HttpWebRequest.Create(hubsite + "VersionManifest.xml");
            using (WebResponse resp = rq.GetResponse())
            {
                Stream responseStream = resp.GetResponseStream();
                this.versionManifest = XDocument.Load(responseStream);
            }

            string newShipVer = versionManifest.Root.Element("Ships").Attribute("version").Value;
            string newShipTypeVer = versionManifest.Root.Element("ShipTypes").Attribute("version").Value;
            string newEquipVer = versionManifest.Root.Element("Equipment").Attribute("version").Value;
            string newEquipTypeVer = versionManifest.Root.Element("EquipmentTypes").Attribute("version").Value;
            string newOperationVer = versionManifest.Root.Element("Operations").Attribute("version").Value;
            string newQuestVer = versionManifest.Root.Element("Quests").Attribute("version").Value;
            string newExpedVer = versionManifest.Root.Element("Expeditions").Attribute("version").Value;
            string newItemVer = versionManifest.Root.Element("Items").Attribute("version").Value;
            string newExpeddataVer = versionManifest.Root.Element("ExpeditionData").Attribute("version").Value;
            string newAkashiDayver = versionManifest.Root.Element("AkashiDay").Attribute("version").Value;
            string newAkashiDataver = versionManifest.Root.Element("AkashiData").Attribute("version").Value;
            string newNodeDataver = versionManifest.Root.Element("NodeData").Attribute("version").Value;
            string newFitDataver = versionManifest.Root.Element("FitData").Attribute("version").Value;

            if (newShipVer != shipsVersion)
            {
                shipsXml = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "/Ships.xml");
                using (WebResponse resp = r2.GetResponse())
                {
                    Stream responseStream = resp.GetResponseStream();
                    this.shipsXml = XDocument.Load(responseStream);
                    shipsXml.Save("Translations\\Ships.xml");
                }
                Logger.Add(2, "함선 이름 번역이 업데이트 되었습니다. 신규 버전 : " + newShipVer + ".");
            }
            if (newShipTypeVer != shipTypesVersion)
            {
                shipTypesXml = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "ShipTypes.xml");
                using (WebResponse resp = r2.GetResponse())
                {
                    Stream responseStream = resp.GetResponseStream();
                    this.shipTypesXml = XDocument.Load(responseStream);
                    shipTypesXml.Save("Translations\\ShipTypes.xml");
                }
                Logger.Add(2, "함종 번역이 업데이트 되었습니다. 신규 버전 : " + newShipTypeVer + ".");
            }
            if (newEquipVer != equipmentVersion)
            {
                equipmentXml = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "Equipment.xml");
                using (WebResponse resp = r2.GetResponse())
                {
                    Stream responseStream = resp.GetResponseStream();
                    this.equipmentXml = XDocument.Load(responseStream);
                    equipmentXml.Save("Translations\\Equipment.xml");
                }
                Logger.Add(2, "장비 번역이 업데이트 되었습니다. 신규 버전 : " + newEquipVer + ".");
            }
            if (newEquipTypeVer != equipTypesVersion)
            {
                equipTypesXML = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "EquipmentTypes.xml");
                using (WebResponse resp = r2.GetResponse())
                {
                    Stream responseStream = resp.GetResponseStream();
                    this.equipTypesXML = XDocument.Load(responseStream);
                    equipTypesXML.Save("Translations\\EquipmentTypes.xml");
                }
                Logger.Add(2, "장비 종류 번역이 업데이트 되었습니다. 신규 버전 : " + newEquipTypeVer + ".");
            }
            if (newOperationVer != operationsVersion)
            {
                operationsXml = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "Operations.xml");
                using (WebResponse resp = r2.GetResponse())
                {
                    Stream responseStream = resp.GetResponseStream();
                    this.operationsXml = XDocument.Load(responseStream);
                    operationsXml.Save("Translations\\Operations.xml");
                }
                Logger.Add(2, "해역 이름 번역이 업데이트 되었습니다. 신규 버전 : " + newOperationVer + ".");
            }
            if (newQuestVer != questsVersion)
            {
                questsXml = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "Quests.xml");
                using (WebResponse resp = r2.GetResponse())
                {
                    Stream responseStream = resp.GetResponseStream();
                    this.questsXml = XDocument.Load(responseStream);
                    questsXml.Save("Translations\\Quests.xml");
                }
                Logger.Add(2, "임무 번역이 업데이트 되었습니다. 신규 버전 : " + newQuestVer + ".");
            }
            if (newExpedVer != expeditionsVersion)
            {
                expeditionsXml = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "Expeditions.xml");
                using (WebResponse resp = r2.GetResponse())
                {
                    Stream responseStream = resp.GetResponseStream();
                    this.expeditionsXml = XDocument.Load(responseStream);
                    expeditionsXml.Save("Translations\\Expeditions.xml");
                }
                Logger.Add(2, "원정 번역이 업데이트 되었습니다. 신규 버전 : " + newExpedVer + ".");
            }

            if (newExpeddataVer != expeditionsdataVersion)
            {
                expeditionsdataXml = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "ExpeditionData.xml");
                using (WebResponse resp = r2.GetResponse())
                {
                    Stream responseStream = resp.GetResponseStream();
                    this.expeditionsdataXml = XDocument.Load(responseStream);
                    expeditionsdataXml.Save("Translations\\ExpeditionData.xml");
                }
                Logger.Add(2, "원정 데이터가 업데이트 되었습니다. 신규 버전 : " + newExpeddataVer + ".");
            }


            if (newItemVer != ItemsVersion)
            {
                ItemsXml = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "Items.xml");
                using (WebResponse resp = r2.GetResponse())
                {
                    Stream responseStream = resp.GetResponseStream();
                    this.ItemsXml = XDocument.Load(responseStream);
                    ItemsXml.Save("Translations\\Items.xml");
                }
                Logger.Add(2, "아이템 번역이 업데이트 되었습니다. 신규 버전 : " + newItemVer + ".");
            }


            if (newAkashiDataver != AkashiDataversion)
            {
                Akashi_Data = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "akashidata.json");

                using (WebResponse resp = r2.GetResponse())
                {
                    using (Stream output = File.OpenWrite("Translations\\akashidata.json"))
                    using (Stream input = resp.GetResponseStream())
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                        }
                    }

                    Akashi_Data = JObject.Parse(File.ReadAllText("Translations\\akashidata.json"));
                }

                Logger.Add(2, "개수공창 목록이 업데이트 되었습니다. 신규 버전 : " + newAkashiDataver + ".");
            }


            if (newAkashiDayver != AkashiDayversion)
            {
                Akashi_Day = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "akashi_day.json");

                using (WebResponse resp = r2.GetResponse())
                {
                    using (Stream output = File.OpenWrite("Translations\\akashi_day.json"))
                    using (Stream input = resp.GetResponseStream())
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                        }
                    }

                    Akashi_Data = JObject.Parse(File.ReadAllText("Translations\\akashi_day.json"));
                }

                Logger.Add(2, "개수공창 목록이 업데이트 되었습니다. 신규 버전 : " + newAkashiDayver + ".");
            }

            if (newNodeDataver != nodedataversion)
            {
                Node_Data = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "node_data.json");

                using (WebResponse resp = r2.GetResponse())
                {
                    using (Stream output = File.OpenWrite("Translations\\node_data.json"))
                    using (Stream input = resp.GetResponseStream())
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                        }
                    }

                    Node_Data = JObject.Parse(File.ReadAllText("Translations\\node_data.json"));
                }

                Logger.Add(2, "해역 정보가 업데이트 되었습니다. 신규 버전 : " + newNodeDataver + ".");
            }

            if (newFitDataver != fitdataversion)
            {
                Fit_Data = null;
                WebRequest r2 = HttpWebRequest.Create(hubsite + "fit_data.json");

                using (WebResponse resp = r2.GetResponse())
                {
                    using (Stream output = File.OpenWrite("Translations\\fit_data.json"))
                    using (Stream input = resp.GetResponseStream())
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                        }
                    }

                    Fit_Data = JObject.Parse(File.ReadAllText("Translations\\fit_data.json"));
                }

                Logger.Add(2, "주포 피트정보가 업데이트 되었습니다. 신규 버전 : " + newFitDataver + ".");
            }

            GetVersions();
        }

        private IEnumerable<XElement> GetTranslationList(TranslationType type)
        {
            switch (type)
            {
                case TranslationType.Ships:
                    if (this.shipsXml != null)
                        return this.shipsXml.Descendants("Ship");
                    break;
                case TranslationType.ShipTypes:
                    if (this.shipTypesXml != null)
                        return this.shipTypesXml.Descendants("Type");
                    break;
                case TranslationType.Equipment:
                    if (this.equipmentXml != null)
                        return this.equipmentXml.Descendants("Item");
                    break;
                case TranslationType.EquipmentType:
                    if (this.equipTypesXML != null)
                        return this.equipTypesXML.Descendants("Type");
                    break;
                case TranslationType.OperationMaps:
                    if (this.operationsXml != null)
                        return this.operationsXml.Descendants("Map");
                    break;
                case TranslationType.OperationSortie:
                    if (this.operationsXml != null)
                        return this.operationsXml.Descendants("Sortie");
                    break;
                case TranslationType.Quests:
                case TranslationType.QuestTitle:
                case TranslationType.QuestDetail:
                    if (this.questsXml != null)
                        return this.questsXml.Descendants("Quest");
                    break;
                case TranslationType.Expeditions:
                case TranslationType.ExpeditionTitle:
                case TranslationType.ExpeditionDetail:
                    if (this.expeditionsXml != null)
                        return this.expeditionsXml.Descendants("Expedition");
                    break;
                case TranslationType.ExpeditionData:
                    if (this.expeditionsdataXml != null)
                        return this.expeditionsdataXml.Descendants("Expedition");
                    break;
                case TranslationType.Items:
                    if (this.ItemsXml != null)
                        return this.ItemsXml.Descendants("Item");
                    break;
                default:
                    return null;
            }
            return null;
        }

        public IEnumerable<XElement> GetExpeditionData()
        {
            return this.expeditionsdataXml.Descendants("Expedition");
        }


        public string GetTranslation(string jpString, TranslationType type, int id = -1)
        {
            try
            {
                IEnumerable<XElement> translationList = this.GetTranslationList(type);

                if (translationList == null)
                {
                    return jpString;
                }

                string jpChildElement = "JP-Name";
                string trChildElement = "TR-Name";

                if (type == TranslationType.QuestDetail || type == TranslationType.ExpeditionDetail)
                {
                    jpChildElement = "JP-Detail";
                    trChildElement = "TR-Detail";
                }

                string translated = jpString;
                if (this.GetTranslation(jpString, translationList, jpChildElement, trChildElement, id, ref translated, type))
                {
                    return translated;
                }
                else
                {
                    /*
                    if(type == TranslationType.Operations)
                        ErrorReporter.SendErrorReport(new Exception(), jpString);

                    if (type == TranslationType.OperationMaps)
                        ErrorReporter.SendErrorReport(new Exception(), jpString);

                    if (type == TranslationType.OperationSortie)
                        ErrorReporter.SendErrorReport(new Exception(), jpString);
                        */
                }
            }
            catch (Exception e)
            {
                Logger.Add(3, "Can't output translation: " + e.Message);
            }

            return jpString;
        }

        public bool GetTranslation(string jpString, IEnumerable<XElement> translationList, string jpChildElement, string trChildElement, int id, ref string translate, TranslationType type)
        {
            IEnumerable<XElement> foundTranslation = translationList.Where(el =>
            {
                try
                {
                    if (id >= 0)
                    {
                        if (el.Element(jpChildElement).Value.Equals(jpString) || Convert.ToInt32(el.Element("ID").Value) == id)
                            return true;
                    }
                    else
                    {
                        if (el.Element(jpChildElement).Value.Equals(jpString))
                            return true;
                    }

                    if (el.Attribute("mode") != null)
                    {
                        if (el.Attribute("mode").Value.Equals("suffix"))
                        {
                            int sl = el.Element(jpChildElement).Value.Length;
                            if (jpString.Length > sl)
                            {
                                if (el.Element(jpChildElement).Value.Equals(jpString.Substring(jpString.Length - sl)))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    return false;
                }

                return false;
            }
            );
            bool foundWrongID = false;
            int n;
            foreach (XElement el in foundTranslation)
            {
                if (el.Attribute("mode") != null && !el.Attribute("mode").Value.Equals("normal"))
                {
                    if (el.Attribute("mode").Value.Equals("suffix"))
                    {
                        try
                        {
                            string t = jpString.Substring(0, jpString.Length - el.Element(jpChildElement).Value.Length);
                            if (this.GetTranslation(t, translationList, jpChildElement, trChildElement, -1, ref t, type))
                            {
                                if ((el.Attribute("suffixType") != null) && el.Attribute("suffixType").Value.Equals("pre")) translate = el.Element(trChildElement).Value + t;
                                else translate = t + el.Element(trChildElement).Value;
                                return true;
                            }
                        }
                        catch (NullReferenceException)
                        {
                        }
                    }
                    continue;
                }

                try
                {
                    if (id >= 0)
                    {
                        if (!Int32.TryParse(el.Element("ID").Value, out n))
                        {
                            foundWrongID = true;
                            translate = el.Element(trChildElement).Value;
                        }
                        else
                        {
                            translate = el.Element(trChildElement).Value;
                            return true;
                        }
                    }
                    else
                    {
                        translate = el.Element(trChildElement).Value;
                        return true;
                    }
                }
                catch (NullReferenceException)
                {
                }
            }

            if (foundWrongID)
            {
                return true;
            }

            return false;
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

                if (Check_WeightData(Master_Shipid))
                {
                    Master_Shipid = Fit_Data[Master_Shipid.ToString()].ToObject<int>();
                }

                var WeightData = Find_WeightData(Master_Shipid);

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

        public bool Check_WeightData(int Shipid)
        {
            var Fit_Info = Fit_Data[Shipid.ToString()];

            if (Fit_Info == null)
                return false;

            if (Fit_Info.Type == JTokenType.Integer)
                return true;

            return false;
        }

        public JObject Find_WeightData(int Shipid)
        {
            var Fit_Info = Fit_Data[Shipid.ToString()];

            if (Fit_Info != null)
                return (JObject)Fit_Info;
            else
                return null;
        }
    }


    public enum TranslationType
    {
        App,
        Equipment,
        EquipmentType,
        Operations,
        Quests,
        Ships,
        ShipTypes,
        OperationMaps,
        OperationSortie,
        QuestDetail,
        QuestTitle,
        Expeditions,
        ExpeditionDetail,
        ExpeditionTitle,
        ExpeditionData,
        Items
    }
}
