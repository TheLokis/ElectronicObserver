using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace ElectronicObserver.Utility
{
    public class ExternalDataInfo
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

    public partial class ExternalDataReader
    {
        private List<ExternalDataInfo> _externalDatas = new List<ExternalDataInfo>();
        private static string _hubsite = "https://thelokis.github.io/EOTranslation/Translation/";
        //private static string _hubsite = "http://172.30.1.20:8080/Translations/";

        private static readonly ExternalDataReader _instance = new ExternalDataReader();
        public static ExternalDataReader Instance => _instance;

        internal ExternalDataReader()
        {
            for (int i = 0; i < (int)DataType.None; i++)
            {
                var data = new ExternalDataInfo();
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

                        Logger.Add(2, "번역 파일 적용에 실패했습니다." + (DataType)i + " / 에러 : " + e);
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
                    while (reader.EndOfStream == false)
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

                        Logger.Add(2, $"{i.DataType.DataTypeToName()}이 업데이트 되었습니다.");
                    }

                    i.Data = JObject.Parse(File.ReadAllText($"Translation\\{currentFileName}"));
                }
                catch (Exception e)
                {
                    Logger.Add(2, "번역 파일 업데이트에 실패했습니다. 파일 : " + i.DataType + ":" + e.GetBaseException());
                }
            });
        }
    }
}