/*
Copyright 2019 - 2024 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
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

        [SerializeField, Tooltip("If true, webview will use a whitelist")]
        private bool useWhitelist = false;

        [SerializeField, Tooltip("List of domains to whitelist")]
        private string[] whitelist = new string[0];

        [SerializeField, Tooltip("If true, webview will use a blacklist")]
        private bool useBlacklist = false;

        [SerializeField, Tooltip("List of domains to blacklist")]
        private string[] blacklist = new string[0];

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

        /// <summary>
        /// Byte buffer used by java plugin to render the texture.
        /// </summary>
        private NativeArray<byte> byteBuffer;

        /// <summary>
        /// <see cref="byteBuffer"/> converted as Java Object.
        /// </summary>
        private AndroidJavaObject byteBufferJavaObject;

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
                byteBuffer = new NativeArray<byte>(2 * width * height, Allocator.Persistent);
                byteBufferJavaObject = new AndroidJavaObject(AndroidJNI.NewDirectByteBuffer(byteBuffer));

                texture = new Texture2D(width, height, TextureFormat.RGB565, false);

                unityCallback = new(this);

                using (var baseClass = new AndroidJavaClass("com.inetum.unitygeckowebview.UnityGeckoWebView"))
                {
                    webView = baseClass.CallStatic<AndroidJavaObject>("createWebView", width, height, useNativeKeyboard, unityCallback, byteBufferJavaObject);

                    UseWhiteList(useWhitelist);
                    UseBlackList(useBlacklist);
                    SetWhiteList(whitelist);
                    SetBlackList(blacklist);

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

            byteBufferJavaObject.Dispose();
            byteBuffer.Dispose();
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
            // Make the Application crash.
            return;
            this.width = width;
            this.height = height;

            if (isInit)
            {
                byteBuffer.Dispose();
                byteBufferJavaObject.Dispose();

                byteBuffer = new NativeArray<byte>(2 * width * height , Allocator.Persistent);
                byteBufferJavaObject = new AndroidJavaObject(AndroidJNI.NewDirectByteBuffer(byteBuffer));

                texture = new Texture2D(width, height, TextureFormat.RGB565, false);
                webView?.Call("changeTextureSize", width, height, byteBufferJavaObject);
            }
        }

        /// <summary>
        /// Coroutine to render webview every <see cref="fps"/>.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RenderCoroutine()
        {
            var wait = new WaitForSeconds(1f / fps);

            while (true)
            {
                try
                {
                    Render();
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
        private void Render()
        {
            using (profilerNativeRenderCodeMarker.Auto())
            {
                webView?.Call("render");
            }

            using (profileProcessRenderCodeMarker.Auto())
            {
                texture.LoadRawTextureData(byteBuffer);
                texture.Apply();

                image.texture = texture;
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

        #region Whitelist/Blacklist

        public void UseWhiteList(bool useWhitelist)
        {
            webView?.Call("setUseWhiteList", useWhitelist);
        }

        public void UseBlackList(bool useBlacklist)
        {
            webView?.Call("setUseBlackList", useBlacklist);
        }

        /// <summary>
        /// Set domains to whitelist.
        /// </summary>
        /// <param name="domains"></param>
        public void SetWhiteList(IEnumerable<string> domains)
        {
            if (domains == null)
                return;

            webView?.Call("setWhiteList", domains.ToArray());
        }

        /// <summary>
        /// Set domains to blacklist.
        /// </summary>
        /// <param name="domains"></param>
        public void SetBlackList(IEnumerable<string> domains)
        {
            if (domains == null)
                return;

            webView?.Call("setBlackList", domains.ToArray());
        }

        #endregion

        #endregion
    }
}


