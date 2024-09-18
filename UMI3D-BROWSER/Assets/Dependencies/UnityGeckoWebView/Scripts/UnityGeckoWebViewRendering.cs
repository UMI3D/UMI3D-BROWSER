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

using inetum.unityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

namespace com.inetum.unitygeckowebview
{
    [RequireComponent(typeof(AndroidJavaWebview))]
    public class UnityGeckoWebViewRendering : MonoBehaviour
    {
        [Tooltip("Init the java web view component on start ?")]
        [SerializeField] bool initOnStart = true;
        [Tooltip("If false use StartRenderingMethod to start the process. If true, init on start must be true too")]
        [SerializeField] bool startRenderingOnInit = false;

        [Space]

        [SerializeField, Range(0, 60), Tooltip("Target render framerate of the webview.")]
        public float fps = 30f;

        [field: SerializeField, Tooltip("Texture width of the webview")]
        public int width { get; private set; } = 1280;

        [field: SerializeField, Tooltip("Texture height of the webview")]
        public int height { get; private set; } = 720;

        [Space]

        [SerializeField, Tooltip("Image which displays the webview")]
        public RawImage image;

        [Space]

        [SerializeField, Tooltip("If true, webview will try to open native keyboard to edit focus text inputs.")]
        bool useNativeKeyboard = false;

        /// <summary>
        /// The android webview.
        /// </summary>
        AndroidJavaWebview javaWebview;
        bool isInit = false;
        /// <summary>
        /// Coroutine used to render webview.
        /// </summary>
        Coroutine renderCoroutine = null;
        /// <summary>
        /// Byte buffer used by java plugin to render the texture.
        /// </summary>
        NativeArray<byte> byteBuffer;
        /// <summary>
        /// <see cref="byteBuffer"/> converted as Java Object.
        /// </summary>
        AndroidJavaObject byteBufferJavaObject;
        /// <summary>
        /// Texture used for <see cref="image"/>.
        /// </summary>
        Texture2D texture;

        static readonly ProfilerMarker profilerNativeRenderCodeMarker = new ProfilerMarker("UnityGeckoWebView.NativeRender");
        static readonly ProfilerMarker profileProcessRenderCodeMarker = new ProfilerMarker("UnityGeckoWebView.ProcessRender");

        void Awake()
        {
            if (Application.isEditor)
            {
                UnityEngine.Debug.LogError($"The Gecko Web View only works in android.");
                Destroy(this);
                return;
            }
            javaWebview = GetComponent<AndroidJavaWebview>();
        }

        void Start()
        {
            Debug.Assert(image != null, "Raw image can not be null");

            if (initOnStart)
            {
                Init(startRenderingOnInit);
            }
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.Rendering,
                RenderingProcess
            );

            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.TextureSizeChanged,
                TextureSizeChanged
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.Rendering);

            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.TextureSizeChanged);
        }

        void OnDestroy()
        {
            javaWebview?.Destroy();
            javaWebview?.Dispose();

            byteBufferJavaObject?.Dispose();
            byteBuffer.Dispose();
        }

        /// <summary>
        /// Inits the webview
        /// <param name="startRendering"></param>
        /// </summary>
        public void Init(bool startRendering)
        {
            if (isInit)
            {
                return;
            }

            try
            {
                ChangeTextureSizeInternal();

                javaWebview.Init(width, height, useNativeKeyboard, byteBufferJavaObject);

                if (startRendering)
                {
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
                byteBuffer.Dispose();
                byteBufferJavaObject.Dispose();

                ChangeTextureSizeInternal();

                javaWebview.ChangeTextureSize(width, height, byteBufferJavaObject);
            }
        }

        void ChangeTextureSizeInternal()
        {
            byteBuffer = new NativeArray<byte>(2 * width * height, Allocator.Persistent);
            byteBufferJavaObject = new AndroidJavaObject(AndroidJNI.NewDirectByteBuffer(byteBuffer));

            texture = new Texture2D(width, height, TextureFormat.RGB565, false);
        }

        /// <summary>
        /// Coroutine to render webview every <see cref="fps"/>.
        /// </summary>
        /// <returns></returns>
        IEnumerator RenderCoroutine()
        {
            var wait = new WaitForSeconds(1f / fps);

            while (true)
            {
                try
                {
                    // Renders webView to texture to apply it to image.
                    using (profilerNativeRenderCodeMarker.Auto())
                    {
                        javaWebview.Render();
                    }

                    using (profileProcessRenderCodeMarker.Auto())
                    {
                        texture.LoadRawTextureData(byteBuffer);
                        texture.Apply();

                        image.texture = texture;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                yield return wait;
            }
        }

        void RenderingProcess(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.RenderingProcess, out Rendering process))
            {
                return;
            }

            switch (process)
            {
                case Rendering.Start:
                    StartRendering();
                    break;
                case Rendering.Stop:
                    StopRendering();
                    break;
                default:
                    break;
            }
        }

        void TextureSizeChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.Vector2, out Vector2 size))
            {
                return;
            }

            ChangeTextureSize((int)size.x, (int)size.y);
        }
    }
}