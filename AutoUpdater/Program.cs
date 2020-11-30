using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace AutoUpdater
{
    class Program
    {
        static FileDownloader _downloader = new FileDownloader();
        static private Mutex _thisMutex;

        static void Main(string[] args)
        {
            _thisMutex = new Mutex(true, "Apk Installer", out bool isNewInstance);
            if (isNewInstance == true)
            {
                Console.WriteLine("자동 업데이트 프로그램을 실행합니다..");

                _downloader.VersionCheck();

                Console.Read();
            }
        }
    }
}
