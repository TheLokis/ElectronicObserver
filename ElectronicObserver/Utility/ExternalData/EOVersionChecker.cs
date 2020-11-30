using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

public enum UpdateState
{
    None,
    Available,
    OnlyManual,
}

public static class EOVersionChecker
{
    private static string _hubsite = "https://thelokis.github.io/EOTranslation/Translation/";
    //private static string _hubsite = "http://172.30.1.20:8080/Translations/";

    private static int _currentVersionCode = 29;

    public static void CheckFileUpdates(System.Action<UpdateState> callBack)
    {
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        var versionManifest = new JObject();
        WebRequest rq = HttpWebRequest.Create(EOVersionChecker._hubsite + "EOVersion.json");
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

        var versionState = System.Enum.Parse(typeof(UpdateState), versionManifest["UpdateState"].ToString());
        switch (versionState)
        {
            case UpdateState.None:
                {
                    callBack.Invoke(UpdateState.None);
                    break;
                }

            case UpdateState.Available:
                {
                    if (int.Parse(versionManifest["Version"].ToString()) >= EOVersionChecker._currentVersionCode)
                    {
                        callBack.Invoke(UpdateState.Available);
                    }
                    else
                    {
                        callBack.Invoke(UpdateState.None);
                    }

                    break;
                }
            case UpdateState.OnlyManual:
                {
                    callBack.Invoke(UpdateState.OnlyManual);
                    break;
                }

            default:
                break;
        }
    }
}
