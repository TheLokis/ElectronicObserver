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
        static FileManager _manager = new FileManager();
        static private Mutex _thisMutex;

        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Contains("ManifestCreateMode") == true)
                    {
                        _manager.OnCreateVersionManifest();
                    }
                }

                return;
            }

            _thisMutex = new Mutex(true, "ElectronicObserverFileDownloader", out bool isNewInstance);
            if (isNewInstance == true)
            {
                Console.WriteLine("자동 업데이트 프로그램을 실행합니다..");

                _manager.VersionCheck();

                Console.Read();
            }
            
        }
    }
}
