using ElectronicObserver.Utility.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Resource.Record
{

	/// <summary>
	/// レコードの基底です。
	/// </summary>
	public abstract class RecordBase
	{


		/// <summary>
		/// レコードの要素の基底です。
		/// </summary>
		public abstract class RecordElementBase
		{

			public abstract void LoadLine(string line);
			public abstract string SaveLine();

		}



		/// <summary>
		/// ファイルからレコードを読み込みます。
		/// </summary>
		/// <param name="path">ファイルが存在するフォルダのパス。</param>
		public virtual bool Load(string path)
		{

			path = this.GetFilePath(path);

			try
			{

				using (StreamReader sr = new StreamReader(path, Utility.Configuration.Config.Log.FileEncoding))
				{

                    this.ClearRecord();

					string line;
					sr.ReadLine();          //ヘッダを読み飛ばす

					while ((line = sr.ReadLine()) != null)
					{
                        this.LoadLine(line);
					}

				}

                this.UpdateLastSavedIndex();
				return true;

			}
			catch (FileNotFoundException)
			{
				Utility.Logger.Add(1, "기록 " + path + " 은 존재하지않습니다.");

			}
			catch (Exception ex)
			{
				Utility.ErrorReporter.SendErrorReport(ex, "기록 " + path + " 로드에 실패했습니다.");
			}

			return false;
		}


		/// <summary>
		/// ファイルに全てのレコードを書き込みます。
		/// </summary>
		/// <param name="path">ファイルが存在するフォルダのパス。</param>
		public virtual bool SaveAll(string path)
		{

			path = this.GetFilePath(path);

			try
			{

				using (StreamWriter sw = new StreamWriter(path, false, Utility.Configuration.Config.Log.FileEncoding))
				{

					sw.WriteLine(this.RecordHeader);
					sw.Write(this.SaveLinesAll());

				}

                this.UpdateLastSavedIndex();
				return true;

			}
			catch (Exception ex)
			{
				Utility.ErrorReporter.SendErrorReport(ex, "기록 " + path + " 저장에 실패했습니다.");
			}

			return false;
		}


		/// <summary>
		/// ファイルに前回からの差分を追記します。
		/// </summary>
		/// <param name="path">ファイルが存在するフォルダのパス。</param>
		public virtual bool SavePartial(string path)
		{

			if (!this.SupportsPartialSave)
				return false;


			path = this.GetFilePath(path);
			bool exists = File.Exists(path);

			try
			{

				using (StreamWriter sw = new StreamWriter(path, true, Utility.Configuration.Config.Log.FileEncoding))
				{

					if (!exists)
						sw.WriteLine(this.RecordHeader);

					sw.Write(this.SaveLinesPartial());
				}

                this.UpdateLastSavedIndex();
				return true;

			}
			catch (Exception ex)
			{
				Utility.ErrorReporter.SendErrorReport(ex, "기록 " + path + " 저장에 실패했습니다.");
			}

			return false;
		}


		protected string GetFilePath(string path)
		{
			return path.Trim(@" \\""".ToCharArray()) + "\\" + this.FileName;
		}


		/// <summary>
		/// ファイルから読み込んだデータを解析し、レコードに追加します。
		/// </summary>
		/// <param name="line">読み込んだ一行分のデータ。</param>
		protected abstract void LoadLine(string line);

		/// <summary>
		/// レコードのデータをファイルに書き込める文字列に変換します。
		/// </summary>
		protected abstract string SaveLinesAll();

		/// <summary>
		/// レコードの差分データをファイルに書き込める文字列に変換します。
		/// </summary>
		protected abstract string SaveLinesPartial();

		public abstract bool SupportsPartialSave { get; }


		/// <summary>
		/// レコードをクリアします。ロード直前に呼ばれます。
		/// </summary>
		protected abstract void ClearRecord();

		protected abstract void UpdateLastSavedIndex();

		public abstract bool NeedToSave { get; }


		public abstract void RegisterEvents();


		/// <summary>
		/// レコードのヘッダを取得します。
		/// </summary>
		public abstract string RecordHeader { get; }

		/// <summary>
		/// 保存するファイル名を取得します。
		/// </summary>
		public abstract string FileName { get; }

	}

}
