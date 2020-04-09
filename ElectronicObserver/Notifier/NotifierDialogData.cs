using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Notifier
{

	public class NotifierDialogData
	{

		/// <summary>
		/// 通知用の画像
		/// </summary>
		public Bitmap Image { get; protected set; }

		/// <summary>
		/// 画像のパス
		/// </summary>
		public string ImagePath { get; private set; }

		/// <summary>
		/// 通知メッセージ
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// 通知のタイトル
		/// </summary>
		public string Title { get; set; }


		/// <summary>
		/// 画像を描画するか
		/// </summary>
		public bool DrawsImage { get; set; }

		/// <summary>
		/// 通知メッセージを描画するか
		/// </summary>
		public bool DrawsMessage { get; set; }


		/// <summary>
		/// 自動で閉じるまでの時間(ミリ秒, 0=閉じない)
		/// </summary>
		public int ClosingInterval { get; set; }


		/// <summary>
		/// マウスポインタがフォーム上を動いたとき自動的に閉じる
		/// </summary>
		public bool CloseOnMouseMove { get; set; }

		/// <summary>
		/// 閉じるマウスボタンのフラグ
		/// </summary>
		public NotifierDialogClickFlags ClickFlag { get; set; }


		/// <summary>
		/// 通知ダイアログの出現位置設定
		/// </summary>
		public NotifierDialogAlignment Alignment { get; set; }

		/// <summary>
		/// 通知ダイアログの出現位置
		/// </summary>
		public Point Location { get; set; }

		/// <summary>
		/// 通知ダイアログに枠をつけるか
		/// </summary>
		public bool HasFormBorder { get; set; }

		/// <summary>
		/// 最前面に表示する
		/// </summary>
		public bool TopMost { get; set; }

		/// <summary>
		/// 表示時にアクティベートするか
		/// </summary>
		public bool ShowWithActivation { get; set; }

		/// <summary>
		/// 前景色
		/// </summary>
		public Color ForeColor { get; set; }

		/// <summary>
		/// 背景色
		/// </summary>
		public Color BackColor { get; set; }



		/// <summary>
		/// イベント発動時に全ウィンドウを閉じる
		/// </summary>
		public event EventHandler CloseAll = delegate { };




		public NotifierDialogData()
		{

            this.Image = null;

		}

		public NotifierDialogData(Utility.Configuration.ConfigurationData.ConfigNotifierBase config)
		{

            this.Image = null;
            this.ImagePath = "";
			if (config.DrawsImage && config.ImagePath != null && config.ImagePath != "")
                this.LoadImage(config.ImagePath);
            this.DrawsImage = config.DrawsImage;
            this.DrawsMessage = config.DrawsMessage;
            this.ClosingInterval = config.ClosingInterval;
            this.CloseOnMouseMove = config.CloseOnMouseMove;
            this.ClickFlag = config.ClickFlag;
            this.Alignment = config.Alignment;
            this.Location = config.Location;
            this.HasFormBorder = config.HasFormBorder;
            this.TopMost = config.TopMost;
            this.ShowWithActivation = config.ShowWithActivation;
            this.ForeColor = config.ForeColor;
            this.BackColor = config.BackColor;
		}


		public NotifierDialogData Clone()
		{
			return (NotifierDialogData)this.MemberwiseClone();
		}


		#region 通知画像

		/// <summary>
		/// 通知画像を読み込みます。
		/// </summary>
		/// <param name="path">画像ファイルへのパス。</param>
		/// <returns>成功すれば true 、失敗すれば false を返します。</returns>
		public bool LoadImage(string path)
		{

			try
			{

                this.DisposeImage();
				using (System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
				{
                    this.Image = new Bitmap(stream);
                    this.ImagePath = path;
				}

				return true;

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, string.Format("알림: 알림 이미지 {0} 의 로드에 실패했습니다.", path));
                this.DisposeImage();

			}

			return false;
		}

		/// <summary>
		/// 通知画像を破棄します。
		/// </summary>
		public void DisposeImage()
		{
			if (this.Image != null)
			{
                this.Image.Dispose();
                this.Image = null;
			}
            this.ImagePath = "";
		}

		#endregion


		public void ApplyToConfiguration(Utility.Configuration.ConfigurationData.ConfigNotifierBase config)
		{

			config.ImagePath = this.ImagePath;
			config.DrawsImage = this.DrawsImage;
			config.DrawsMessage = this.DrawsMessage;
			config.ClosingInterval = this.ClosingInterval;
			config.CloseOnMouseMove = this.CloseOnMouseMove;
			config.ClickFlag = this.ClickFlag;
			config.Alignment = this.Alignment;
			config.Location = this.Location;
			config.HasFormBorder = this.HasFormBorder;
			config.TopMost = this.TopMost;
			config.ShowWithActivation = this.ShowWithActivation;
			config.ForeColor = this.ForeColor;
			config.BackColor = this.BackColor;

		}


		public void OnCloseAll()
		{
			CloseAll(this, new EventArgs());
		}

	}


	/// <summary>
	/// 通知ダイアログの出現位置を表します。
	/// </summary>
	public enum NotifierDialogAlignment
	{

		/// <summary>未設定です。</summary>
		NotSet,

		/// <summary>左上に配置されます。</summary>
		TopLeft,

		/// <summary>上中央に配置されます。</summary>
		TopCenter,

		/// <summary>右上に配置されます。</summary>
		TopRight,

		/// <summary>左中央に配置されます。</summary>
		MiddleLeft,

		/// <summary>中央に配置されます。</summary>
		MiddleCenter,

		/// <summary>右中央に配置されます。</summary>
		MiddleRight,

		/// <summary>左下に配置されます。</summary>
		BottomLeft,

		/// <summary>下中央に配置されます。</summary>
		BottomCenter,

		/// <summary>右下に配置されます。</summary>
		BottomRight,

		/// <summary>ユーザーが設定した座標に配置されます。</summary>
		Custom,

		/// <summary>ユーザーが設定した座標に配置されます(ブラウザウィンドウの中心を原点とする相対座標)。</summary>
		CustomRelative,
	}


	/// <summary>
	/// 通知ダイアログのクリック種別を示します。
	/// </summary>
	[Flags]
	public enum NotifierDialogClickFlags
	{

		None = 0x0,
		Left = 0x1,
		LeftDouble = 0x2,
		Right = 0x4,
		RightDouble = 0x8,
		Middle = 0x10,
		MiddleDouble = 0x20,

		/// <summary>最高位ビット</summary>
		HighestBit = 6,
	}
}
