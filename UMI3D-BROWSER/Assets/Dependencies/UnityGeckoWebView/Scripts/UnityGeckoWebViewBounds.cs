using UnityEngine;

namespace com.inetum.unitygeckowebview
{
    /// <summary>
    /// Represents <see cref="UnityGeckoWebView"/> bounds to optimize its rendering (only renderer it if visible). 
    /// </summary>
    public class UnityGeckoWebViewBounds : MonoBehaviour
    {
        [SerializeField]
        private UnityGeckoWebView webview;

        private void Start()
        {
            Debug.Assert(webview != null, "Web view should not be null");
        }

        private void OnBecameVisible()
        {
            webview?.StartRendering();
        }

        private void OnBecameInvisible()
        {
            webview?.StopRendering();
        }
    }
}