using System;
using System.Collections;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace com.inetum.unitygeckowebview
{
    public class UnityGeckoWebView : MonoBehaviour
    {
        #region Fields

        #region Parameters

        [SerializeField, Tooltip("Start rendering on awake ? If false use StartRenderingMethod to start the process.")]
        private bool initOnStart = true;

        [SerializeField, Tooltip("Start rendering on awake ? If false use StartRenderingMethod to start the process. If true, init on start must be true too")]
        private bool startRenderingOnAwake = false;

        [field: SerializeField, Tooltip("Texture width of the webview")]
        public int width { get; private set; } = 1280;

        [field: SerializeField, Tooltip("Texture height of the webview")]
        public int height { get; private set; } = 720;

        [SerializeField, Range(0, 60), Tooltip("Target render framerate of the webview.")]
        public float fps = 30f;

        [SerializeField, Tooltip("If true, webview will try to open native keyboard to edit focus text inputs.")]
        private bool useNativeKeyboard = true;

        [Space]

        [SerializeField, Tooltip("Image which displays the webview")]
        public RawImage image;

        #endregion

        #region Data

        /// <summary>
        /// Texture used for <see cref="image"/>.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// Webview handled natively.
        /// </summary>
        private AndroidJavaObject webView;

        /// <summary>
        /// Coroutine used to render webview.
        /// </summary>
        private Coroutine renderCoroutine = null;

        /// <summary>
        /// Object which can be sent to native plugin.
        /// </summary>
        private UnityGeckoWebViewCallback unityCallback;

        private bool isInit = false;

        #endregion

        #region Profiling

        static readonly ProfilerMarker profilerNativeRenderCodeMarker = new ProfilerMarker("UnityGeckoWebView.NativeRender");
        static readonly ProfilerMarker profileProcessRenderCodeMarker = new ProfilerMarker("UnityGeckoWebView.ProcessRender");

        #endregion

        public static System.Collections.Generic.Queue<Action> actionsToRunOnMainThread = new();

        #endregion

        #region Events

        /// <summary>
        /// Notifies that a text input has been selected within the webview.
        /// </summary>
        public UnityEvent OnTextInputSelected = new();

        /// <summary>
        /// Notifies that a web page has started loading.
        /// </summary>
        public UnityEvent<string> OnUrlStartedLoading = new();

        #endregion

        #region Methods

        #region Unity callbacks

        private void Start()
        {
            Debug.Assert(image != null, "Raw image can not be null");

            if (initOnStart)
                Init(startRenderingOnAwake);
        }

        /// <summary>
        /// Inits the webview
        /// </summary>
        /// <param name="startRendering"></param>
        public void Init(bool startRendering)
        {
            if (isInit) return;

            try
            {
                texture = new Texture2D(width, height, TextureFormat.RGB565, false);

                unityCallback = new(this);

                using (var baseClass = new AndroidJavaClass("com.inetum.unitygeckowebview.UnityGeckoWebView"))
                {
                    webView = baseClass.CallStatic<AndroidJavaObject>("createWebView", width, height, useNativeKeyboard, unityCallback);

                    if (startRendering)
                        StartRendering();
                }

                isInit = true;
            }
            catch (Exception ex)
            {
                Debug.LogError("Impossible to init webView.");
                Debug.LogException(ex);
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause && webView != null)
            {
                webView.Call("notifyOnResume");
            }
        }

        private void OnDestroy()
        {
            webView?.Call("destroy");
            webView?.Dispose();
        }

        private void Update()
        {
            if (actionsToRunOnMainThread.Count > 0)
            {
                while(actionsToRunOnMainThread.Count > 0)
                {
                    actionsToRunOnMainThread.Dequeue().Invoke();
                }
            }
        }

        #endregion

        #region Rendering

        /// <summary>
        /// Changes texture size.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ChangeTextureSize(int width, int height)
        {
            this.width = width;
            this.height = height;

            if (isInit)
            {
                texture = new Texture2D(width, height, TextureFormat.RGB565, false);
                webView?.Call("changeTextureSize", width, height);
            }
        }

        /// <summary>
        /// Coroutine to render webview every <see cref="fps"/>.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RenderCoroutine()
        {
            sbyte[] signedBitmap = null;
            byte[] bitmap = null;

            var wait = new WaitForSeconds(1f / fps);

            while (true)
            {
                try
                {
                    Render(signedBitmap, bitmap);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                yield return wait;
            }
        }

        /// <summary>
        /// Renders <see cref="webView"/> to <see cref="texture"/> to apply it to <see cref="image"/>.
        /// </summary>
        /// <param name="signedBitmap"></param>
        /// <param name="bitmap"></param>
        private void Render(sbyte[] signedBitmap, byte[] bitmap)
        {
            using (profilerNativeRenderCodeMarker.Auto())
            {
                signedBitmap = webView?.Call<sbyte[]>("render");
            }

            using (profileProcessRenderCodeMarker.Auto())
            {
                if (bitmap == null)
                    bitmap = new byte[signedBitmap.Length];

                if (bitmap.Length > 0)
                {
                    Buffer.BlockCopy(signedBitmap, 0, bitmap, 0, signedBitmap.Length);

                    if (bitmap != null)
                    {
                        texture.LoadRawTextureData(bitmap);
                        texture.Apply();

                        image.texture = texture;
                    }
                }
            }
        }

        /// <summary>
        /// Starts rendering the webview.
        /// </summary>
        public void StartRendering()
        {
            if (!isInit)
            {
                Debug.LogWarning("Impossible to start rendering webview. Call Init() before");
                return;
            }

#if !UNITY_EDITOR
            if (renderCoroutine == null)
            {
                renderCoroutine = StartCoroutine(RenderCoroutine());
            }
#endif
        }

        /// <summary>
        /// Stops rendering the webview.
        /// </summary>
        public void StopRendering()
        {
            if (renderCoroutine != null)
            {
                StopCoroutine(renderCoroutine);
                renderCoroutine = null;
            }
        }

        #endregion

        #region Interactions

        public void GoBack()
        {
            webView?.Call("goBack");
        }

        public void GoForward()
        {
            webView.Call("goForward");
        }

        /// <summary>
        /// Loads an url.
        /// </summary>
        /// <param name="url"></param>
        public void LoadUrl(string url)
        {
            webView?.Call("loadUrl", url);
        }

        /// <summary>
        /// Enters text in webview.
        /// </summary>
        /// <param name="text"></param>
        public void EnterText(string text)
        {
            webView?.Call("enterText", text);
        }

        public void DeleteCharacter()
        {
            webView?.Call("deleteCharacter");
        }

        public void EnterCharacter()
        {
            webView?.Call("enterKey");
        }

        public void Click(float x, float y, int pointerId)
        {
            webView?.Call("click", x, y, pointerId);
        }

        public void PointerDown(float x, float y, int pointerId)
        {
            webView?.Call("pointerDown", x, y, pointerId);
        }

        public void PointerUp(float x, float y, int pointerId)
        {
            webView?.Call("pointerUp", x, y, pointerId);
        }

        public void PointerMove(float x, float y, int pointerId)
        {
            webView?.Call("pointerMove", x, y, pointerId);
        }

        public void Scroll(int scrollX, int scrollY)
        {
            webView?.Call("scroll", scrollX, scrollY);
        }

        #endregion

        #endregion
    }
}


