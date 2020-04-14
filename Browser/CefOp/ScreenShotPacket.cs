using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browser.CefOp
{
    public class ScreenShotPacket
    {
        public string ID { get; }
        public string DataUrl;
        public TaskCompletionSource<ScreenShotPacket> TaskSource { get; }

        public ScreenShotPacket() : this("ss_" + Guid.NewGuid().ToString("N")) { }
        public ScreenShotPacket(string id)
        {
            this.ID = id;
            this.TaskSource = new TaskCompletionSource<ScreenShotPacket>();
        }

        public void Complete(string dataUrl)
        {
            this.DataUrl = dataUrl;
            this.TaskSource.SetResult(this);
        }

        public Bitmap GetImage() => ConvertToImage(this.DataUrl);


        public static Bitmap ConvertToImage(string dataUrl)
        {
            if (dataUrl == null || !dataUrl.StartsWith("data:image/png"))
                return null;

            var s = dataUrl.Substring(dataUrl.IndexOf(',') + 1);
            var bytes = Convert.FromBase64String(s);

            Bitmap bitmap;
            using (var ms = new MemoryStream(bytes))
                bitmap = new Bitmap(ms);

            return bitmap;
        }
    }
}