using SimpleJSON;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;

public class FileManager
{
    private string _hubsite = "https://thelokis.github.io/EOTranslation/";
    private JSONNode _versionManifest = null;

    public void VersionCheck()
    {
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        WebRequest rq = HttpWebRequest.Create(this._hubsite + "Translation/EOVersion.json");
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

                this._versionManifest = JSON.Parse(versionInfoText);
            }
        }

        this.EODownload();
    }

    public void EODownload()
    {
        if (string.IsNullOrEmpty(this._versionManifest["Version"].ToString()) == false)
        {
            string targetFileName = $"ElectronicObserver_KRTL_R{this._versionManifest["Version"].Value}.zip";

            Console.WriteLine($"74식 전자관측의 버전 {this._versionManifest["Version"].Value} 을 다운받습니다..");

            bool fileDownloaded = false;

            Console.WriteLine(this._hubsite + "Version/" + targetFileName);
            WebRequest eoRq = HttpWebRequest.Create(this._hubsite + "Version/" + targetFileName);
            using (WebResponse resp = eoRq.GetResponse())
            {
                using (Stream output = File.OpenWrite(targetFileName))
                {
                    using (Stream input = resp.GetResponseStream())
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                        }

                        fileDownloaded = true;

                        input.Close();
                        resp.Close();
                    }

                    output.Close();
                }
            }

            if (fileDownloaded == true)
            {
                using (var eoZip = ZipFile.OpenRead(targetFileName))
                {
                    foreach (ZipArchiveEntry entry in eoZip.Entries)
                    {
                        Console.WriteLine($"update {entry.FullName}..");
                        entry.ExtractToFile(Path.Combine(Directory.GetCurrentDirectory(), entry.FullName), true);
                    }
                }

                File.Delete(targetFileName);
                Console.WriteLine("업데이트가 완료되었습니다.");
                Console.ReadKey();
            }
        }
    }

    public void OnCreateVersionManifest()
    {
        string root = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        DirectoryInfo info = new DirectoryInfo(root);

        var files = info.GetFiles();
        Console.WriteLine(files.Length);
        JSONNode DateData = JSON.Parse("{ \"CreateDate\" : \"" + DateTime.Now + "\" }");
        JSONArray fileDataArray = new JSONArray();

        foreach (var file in files)
        {
            fileDataArray.Add(file.Name);
        }

        DateData.Add("FileList", fileDataArray);

        var jsonData = JSON.Parse(DateData.ToString());

        File.WriteAllText("EOVersion.json", jsonData.ToString());
    }
}
