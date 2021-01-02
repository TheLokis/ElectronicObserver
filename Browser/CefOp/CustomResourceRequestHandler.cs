using CefSharp;
using CefSharp.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browser.CefOp
{
    public class CustomResourceRequestHandler : ResourceRequestHandler
    {
        private bool _pixiSettingEnabled { get; set; }

        public CustomResourceRequestHandler(bool pixiSettingEnabled) : base()
        {
            this._pixiSettingEnabled = pixiSettingEnabled;
        }

        /// <summary>
        /// レスポンスの置換制御を行います。
        /// </summary>
        protected override IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            if (this._pixiSettingEnabled && request.Url.Contains(@"/kcs2/index.php"))
                return new ResponseFilterPixiSetting();

            return base.GetResourceResponseFilter(chromiumWebBrowser, browser, frame, request, response);
        }

        /// <summary>
        /// 特定の通信をブロックします。
        /// </summary>
        protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            // ログイン直後に勝手に遷移させられ、ブラウザがホワイトアウトすることがあるためブロックする
            if (request.Url.Contains(@"/rt.gsspat.jp/"))
            {
                return CefReturnValue.Cancel;
            }

            return base.OnBeforeResourceLoad(chromiumWebBrowser, browser, frame, request, callback);
        }

    }
}
