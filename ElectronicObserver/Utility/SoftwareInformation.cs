using ElectronicObserver.Utility.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Utility
{

	/// <summary>
	/// ソフトウェアの情報を保持します。
	/// </summary>
	public static class SoftwareInformation
	{

		/// <summary>
		/// ソフトウェア名(日本語)
		/// </summary>
		public static string SoftwareNameJapanese => "七四式電子観測儀";

        public static string SoftwareNameKorean => "74식 전자관측의 KRTL_Beta";

        /// <summary>
        /// ソフトウェア名(英語)
        /// </summary>
        public static string SoftwareNameEnglish => "ElectronicObserver";


		/// <summary>
		/// バージョン(日本語, ソフトウェア名を含みます)
		/// </summary>
		public static string VersionJapanese => SoftwareNameJapanese + "三一型改二";


		/// <summary>
		/// バージョン(英語)
		/// </summary>
		public static string VersionEnglish => "3.1.2";

        public static string VersionKorean => "3.1.2";

        /// <summary>
        /// 更新日時
        /// </summary>
        public static DateTime UpdateTime => DateTimeHelper.CSVStringToTime("2018/05/19 20:00:00");




		private static System.Net.WebClient client;
		private static readonly Uri uri = new Uri("https://thelokis.github.io/EOTranslation/Translations/softwareversion.txt");

		public static void CheckUpdate()
		{

			if (!Utility.Configuration.Config.Life.CheckUpdateInformation)
				return;

			if (client == null)
			{
				client = new System.Net.WebClient
				{
					Encoding = new System.Text.UTF8Encoding(false)
				};
				client.DownloadStringCompleted += DownloadStringCompleted;
			}

			if (!client.IsBusy)
				client.DownloadStringAsync(uri);
		}

		private static void DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
		{

			if (e.Error != null)
			{

				Utility.ErrorReporter.SendErrorReport(e.Error, "업데이트 정보를 가져오는데 실패했습니다.");
				return;

			}

			if (e.Result.StartsWith("<!DOCTYPE html>"))
			{

				Utility.Logger.Add(3, "업데이트 정보의 URI가 잘못되었습니다.");
				return;

			}


			try
			{

				using (var sr = new System.IO.StringReader(e.Result))
				{

					DateTime date = DateTimeHelper.CSVStringToTime(sr.ReadLine());
					string version = sr.ReadLine();
					string description = sr.ReadToEnd();

					if (UpdateTime < date)
					{

						Utility.Logger.Add(3, "새 버전이 출시되었습니다! : " + version);

						var result = System.Windows.Forms.MessageBox.Show(
							string.Format("새 버전이 출시되었습니다! : {0}\r\n업데이트 내용 : \r\n{1}\r\n다운로드 페이지로 이동하시겠습니까?？\r\n(취소할경우 이후 표시하지 않습니다.)",
							version, description),
							"업데이트 정보", System.Windows.Forms.MessageBoxButtons.YesNoCancel, System.Windows.Forms.MessageBoxIcon.Information,
							System.Windows.Forms.MessageBoxDefaultButton.Button1);


						if (result == System.Windows.Forms.DialogResult.Yes)
						{

							System.Diagnostics.Process.Start("http://thelokis.egloos.com/");

						}
						else if (result == System.Windows.Forms.DialogResult.Cancel)
						{

							Utility.Configuration.Config.Life.CheckUpdateInformation = false;

						}

					}
					else
					{

						Utility.Logger.Add(1, "현재 버전이 최신 버전입니다.");

					}

				}

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, "업데이트에 실패했습니다.");
			}

		}

	}

}
