using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace com.inetum.unitygeckowebview.samples
{

    public class UnityGeckoWebViewSearch : MonoBehaviour
    {

        /// <summary>
        /// Regex to check if a string is an url or not.
        /// </summary>
        private Regex validateURLRegex = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");

        [SerializeField]
        private UnityGeckoWebView webView;

        [SerializeField]
        private InputField searchField;

        public void Search()
        {
            Debug.Assert(searchField != null);
            Debug.Assert(webView != null);

            if (validateURLRegex.IsMatch(searchField.text))
                webView.LoadUrl(searchField.text);
            else if (searchField.text.EndsWith(".com") || searchField.text.EndsWith(".net") || searchField.text.EndsWith(".fr") || searchField.text.EndsWith(".org"))
                webView.LoadUrl("http://" + searchField.text);
            else
                webView.LoadUrl("https://www.google.com/search?q=" + searchField.text);
        }

        public void OnUrlLoaded(string url)
        {
            if (searchField != null)
            {
                searchField.SetTextWithoutNotify(url);
            }
        }
    }
}