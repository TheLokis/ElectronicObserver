using CefSharp;
using CefSharp.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browser.CefOp
{
	/// <summary>
	/// レスポンスの置換制御を行います。
	/// </summary>
	public class RequestHandler : DefaultRequestHandler
	{
        public delegate void RenderProcessTerminatedEventHandler(string message);
        public event RenderProcessTerminatedEventHandler RenderProcessTerminated;

        bool pixiSettingEnabled;

		public RequestHandler(bool pixiSettingEnabled) : base() {
			this.pixiSettingEnabled = pixiSettingEnabled;
		}

        /// <summary>
        /// レスポンスの置換制御を行います。
        /// </summary>
        public override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
		{
			if (this.pixiSettingEnabled && request.Url.Contains(@"/kcs2/index.php"))
				return new ResponseFilterPixiSetting();

            return base.GetResourceResponseFilter(browserControl, browser, frame, request, response);
		}

        /// <summary>
		/// 特定の通信をブロックします。
		/// </summary>
		public override CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            // ログイン直後に勝手に遷移させられ、ブラウザがホワイトアウトすることがあるためブロックする
            if (request.Url.Contains(@"/rt.gsspat.jp/"))
            {
                return CefReturnValue.Cancel;
            }

            request.Url = request.Url.Replace("203.104.209.7/gadget_html5", "luckyjervis.com/gadget_html5");
            request.Url = request.Url.Replace("203.104.209.7/html", "luckyjervis.com/html");

            return base.OnBeforeResourceLoad(browserControl, browser, frame, request, callback);
        }

        /// <summary>
        /// 戻る/進む操作をブロックします。
        /// </summary>
        public override bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            if ((request.TransitionType & TransitionType.ForwardBack) != 0)
            {
                return true;
            }

            return base.OnBeforeBrowse(browserControl, browser, frame, request, userGesture, isRedirect);
        }

        /// <summary>
        /// 描画プロセスが何らかの理由で落ちた際の処理を行います。
        /// </summary>
        public override void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
        {
            // note: out of memory (例外コード: 0xe0000008) でクラッシュした場合、このイベントは呼ばれない
            string ret = "브라우저의 렌더링 프로세스가";
            switch (status)
            {
                case CefTerminationStatus.AbnormalTermination:
                    ret += "정상 종료 되지 않았습니다.";
                    break;
                case CefTerminationStatus.ProcessWasKilled:
                    ret += "다른 프로그램에 의해 종료되었습니다.";
                    break;
                case CefTerminationStatus.ProcessCrashed:
                    ret += "충돌했습니다.";
                    break;
                default:
                    ret += "예기치 않게 종료되었습니다.";
                    break;
            }
            ret += "다시 로드합니다.";
            RenderProcessTerminated(ret);
        }
    }
}
