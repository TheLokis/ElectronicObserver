using ElectronicObserver.Resource;
using ElectronicObserver.Utility;
using ElectronicObserver.Window.Integrate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Control
{

	public partial class WindowCaptureButton : Button
	{

		private FormCapturing CapturingImageWindow = new FormCapturing();
		private FormCandidate CandidateBoxWindow = new FormCandidate();

		private bool selectingWindow = false;
		private IntPtr currentCandidate;

		public delegate void WindowCapturedDelegate(IntPtr hWnd);
		public event WindowCapturedDelegate WindowCaptured = delegate { };

		public WindowCaptureButton()
		{
            this.InitializeComponent();
		}

		private void OnMouseMoved()
		{
			Point cursor = System.Windows.Forms.Cursor.Position;

            this.CapturingImageWindow.Location = new Point(
				cursor.X - this.Image.Width / 2,
				cursor.Y - this.Image.Height / 2
				);

			IntPtr newCandidate = this.RootWindowFromPoint(cursor);
			if (this.currentCandidate != newCandidate)
			{
				if (newCandidate == IntPtr.Zero)
				{
                    this.CandidateBoxWindow.Visible = false;
				}
				else
				{
					// ウィンドウ選択が変わったので移動
					WinAPI.GetWindowRect(newCandidate, out WinAPI.RECT candidateRect);
                    this.CandidateBoxWindow.Bounds = new Rectangle(candidateRect.left, candidateRect.top,
						candidateRect.right - candidateRect.left, candidateRect.bottom - candidateRect.top);
					if (!this.CandidateBoxWindow.Visible)
					{
                        this.CandidateBoxWindow.Visible = true;
					}
				}
                this.currentCandidate = newCandidate;
			}
		}

		private void OnMouseUp()
		{

			IntPtr selected = this.currentCandidate;
            this.OnCanceled();

			if (selected != IntPtr.Zero)
			{
				WindowCaptured(selected);
				/*
				int capacity = WinAPI.GetWindowTextLength( selected ) * 2;
				StringBuilder stringBuilder = new StringBuilder( capacity );
				WinAPI.GetWindowText( selected, stringBuilder, stringBuilder.Capacity );

				MessageBox.Show( stringBuilder.ToString() );
				 * */
			}
		}

		private void OnCanceled()
		{
            this.CapturingImageWindow.Visible = false;
            this.CandidateBoxWindow.Visible = false;
            this.currentCandidate = IntPtr.Zero;
            this.selectingWindow = false;
		}

		private IntPtr RootWindowFromPoint(Point cursor)
		{
			StringBuilder className = new StringBuilder(256);
			StringBuilder windowText = new StringBuilder(256);
			IntPtr result = IntPtr.Zero;
			int currentProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
			WinAPI.EnumWindows((WinAPI.EnumWindowsDelegate)((hWnd, lparam) =>
			{
				if (this.CapturingImageWindow.Handle != hWnd &&
                    this.CandidateBoxWindow.Handle != hWnd)
				{
					WinAPI.GetClassName(hWnd, className, className.Capacity);
					WinAPI.GetWindowText(hWnd, windowText, windowText.Capacity);
					WinAPI.GetWindowThreadProcessId(hWnd, out uint processId);
					if (className.Length > 0 &&
						windowText.Length > 0 &&
						WinAPI.IsWindowVisible(hWnd) &&
						windowText.ToString() != "Program Manager" &&
						processId != currentProcessId)
					{
						WinAPI.GetWindowRect(hWnd, out WinAPI.RECT rect);
						if (rect.left <= cursor.X && cursor.X <= rect.right && rect.top <= cursor.Y && cursor.Y <= rect.bottom)
						{
							result = hWnd;
							return false;
						}
					}
				}
				return true;
			}), IntPtr.Zero);
			return result;
		}

		protected override void WndProc(ref Message m)
		{
			if (this.selectingWindow)
			{
				// マウスをキャプチャしている時だけ
				switch (m.Msg)
				{
					case WinAPI.WM_MOUSEMOVE:
                        this.OnMouseMoved();
						break;
					case WinAPI.WM_LBUTTONUP:
                        this.OnMouseUp();
						break;
					case WinAPI.WM_CANCELMODE:
					case WinAPI.WM_CAPTURECHANGED:
                        this.OnCanceled();
						break;
				}
				return;
			}
			base.WndProc(ref m);
		}

		protected override void OnMouseDown(MouseEventArgs mevent)
		{
			base.OnMouseDown(mevent);
            this.Capture = true;
            this.selectingWindow = true;

			Point cursor = System.Windows.Forms.Cursor.Position;
            this.CapturingImageWindow.Location = new Point(
				cursor.X - this.Image.Width / 2,
				cursor.Y - this.Image.Height / 2
				);

            this.CapturingImageWindow.BackgroundImage = this.Image;
            this.CapturingImageWindow.Show();
            this.CapturingImageWindow.Size = this.Image.Size;
		}
	}
}
