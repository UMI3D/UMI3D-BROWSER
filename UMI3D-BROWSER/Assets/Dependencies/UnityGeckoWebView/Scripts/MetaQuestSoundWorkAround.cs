using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace com.inetum.unitygeckowebview
{
    /// <summary>
    /// On Meta Quest, when GeckoSession is stopped, its mute the application.
    /// Another webview plugin (by Vuplex has the same issue) : <see href="https://support.vuplex.com/articles/oculus-quest-app-audio-stops"/>.
    /// They submit a Mozilla Bug <see href="https://bugzilla.mozilla.org/show_bug.cgi?id=1766545"/>.
    /// Other Unity devs have the issue <see href="https://communityforums.atmeta.com/t5/Unity-VR-Development/Requesting-Audio-Focus-on-Quest-2/td-p/1050109"/>
    /// 
    /// Workaround : create a background webview which never rendered but running in background (with gecko version <= 95).
    /// </summary>
    public class MetaQuestSoundWorkAround : MonoBehaviour
    {
        private NativeArray<byte> byteBuffer;

        private AndroidJavaObject webView;

        private static bool isWorkaroundInit = false;

        private static MetaQuestSoundWorkAround instance;

        public static void SetAudioWorkAround()
        {
            if (!isWorkaroundInit)
                instance.DeployBackgroundWebView();
        }

        private void Awake()
        {
            instance = this;
        }

        private void DeployBackgroundWebView()
        {
            isWorkaroundInit = true;

            byteBuffer = new NativeArray<byte>(2, Allocator.Persistent);
            AndroidJavaObject byteBufferJavaObject = new AndroidJavaObject(AndroidJNI.NewDirectByteBuffer(byteBuffer));

            var callback = new UnityGeckoWebViewCallback(null);

            using (var baseClass = new AndroidJavaClass("com.inetum.unitygeckowebview.UnityGeckoWebView"))
            {
                webView = baseClass.CallStatic<AndroidJavaObject>("createWebView", 1, 1, false, callback, byteBufferJavaObject);
            }
        }

        private void OnDestroy()
        {
            webView?.Dispose();
            byteBuffer.Dispose();
        }
    }
}
