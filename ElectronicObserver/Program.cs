using ElectronicObserver.Window;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace ElectronicObserver
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			bool allowMultiInstance = args.Contains("-m") || args.Contains("--multi-instance");

			using (var mutex = new Mutex(false, Application.ExecutablePath.Replace('\\', '/'), out var created))
			{
				EOVersionChecker.CheckFileUpdates(result =>
				{
					switch (result)
                    {
						case UpdateState.None: // 업데이트 없음
                            {
                                if (result == UpdateState.None)
                                {
                                    if (created == false && allowMultiInstance == false)
                                    {
                                        // 多重起動禁止
                                        MessageBox.Show("이미 74식 전자관측의가 실행되고 있습니다. " +
                                            "\r\n실행중이 아닐때 이 메시지가 뜨는 경우, 명령줄에서 -m 을 사용해 실행하십시오.",
                                            "74식 전자관측의",
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                                        return;
                                    }
                                }

                                break;
                            }
                        case UpdateState.Available: // 자동 업데이트 가능
                            {
                                if (File.Exists("AutoUpdater.exe") == false)
                                {
                                    MessageBox.Show("자동 업데이트용 파일을 찾을 수 없습니다. " +
                                                    "\r\n업데이트를 진행하지않고 실행합니다.",
                                                    "74식 전자관측의",
                                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                else
                                {
                                    DialogResult agreed =
                                    MessageBox.Show("업데이트 사항이 있습니다." +
                                                    "\r\n자동 업데이트를 진행하시겠습니까?",
                                                    "74식 전자관측의",
                                                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                                    if (agreed == DialogResult.OK)
                                    {
                                        System.Diagnostics.Process.Start("AutoUpdater.exe");
                                        return;
                                    }
                                }

                                break;
                            }
                        case UpdateState.OnlyManual: // 수동 업데이트만 가능
                            {
                                DialogResult agreed =
                                    MessageBox.Show("업데이트 사항이 있습니다." +
                                    "\r\n수동 업데이트만 가능합니다." +
                                    "확인 버튼을 누르시면 번역자 페이지로 이동합니다.",
                                    "74식 전자관측의",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                                if (agreed == DialogResult.OK)
                                {
                                    System.Diagnostics.Process.Start("http://thelokis.egloos.com/");
                                    return;
                                }

                                break;
                            }
                    }

                    RunEO();
                });

            }
        }

		static void RunEO()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
