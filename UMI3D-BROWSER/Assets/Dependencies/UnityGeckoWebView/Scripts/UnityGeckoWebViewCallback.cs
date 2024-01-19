using UnityEngine;

namespace com.inetum.unitygeckowebview
{
    /// <summary>
    /// Class which represents com.inetum.unitygeckowebview.UnityGeckoWebViewCallback class of the Android plugin.
    /// So this object can be sent directly to the native plugin.
    /// </summary>
    public class UnityGeckoWebViewCallback : AndroidJavaProxy
    {
        private UnityGeckoWebView webView;

        public UnityGeckoWebViewCallback(UnityGeckoWebView webView) : base("com.inetum.unitygeckowebview.UnityGeckoWebViewCallback")
        {
            this.webView = webView;
        }

        /// <summary>
        /// Notifies that a text input has been selected within the webview.
        /// </summary>
        public void onTextInputSelected()
        {
            UnityGeckoWebView.actionsToRunOnMainThread.Enqueue(() => webView.OnTextInputSelected?.Invoke());
        }

        /// <summary>
        /// Notifies that a web page has started loading.
        /// </summary>
        public void onUrlStartedLoading(string url)
        {
            UnityGeckoWebView.actionsToRunOnMainThread.Enqueue(() => webView.OnUrlStartedLoading?.Invoke(url));
        }
    }
}