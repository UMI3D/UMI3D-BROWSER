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
using com.inetum.unitygeckowebview;
using System;
using System.Collections;
using umi3d.cdk;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.UI;

namespace QuestBrowser.WebView
{
    public class WebView : AbstractUMI3DWebView
    {
        [SerializeField] Canvas webViewCanvas = null;
        [SerializeField] CanvasScaler webViewCanvasScaler = null;
        [SerializeField] RectTransform bottomBarContainer = null;
        [SerializeField] RectTransform backButton = null;
        [SerializeField] RectTransform homeButton = null;
        [SerializeField] RectTransform synchronizeRectTransform = null;
        [SerializeField] RectTransform forwardButton = null;
        [SerializeField] RectTransform topBarContainer = null;
        [SerializeField] RectTransform searchButton = null;
        [SerializeField] RectTransform container = null;
        [SerializeField] RectTransform textureTransform = null;
        [SerializeField] RectTransform searchFieldTransform;
        [SerializeField] InputField searchField;

        [Space]

        [SerializeField, Tooltip("Delay to send current url and scroll offset when user synchronizes his content. In seconds")]
        float synchronizationDelay = 2f;

        [SerializeField, Tooltip("A feedback to show user he's currently sharing its content")]
        GameObject syncFeedback;

        AndroidJavaWebview javaWebview;
        UnityGeckoWebViewRendering webviewRendering;
        UnityGeckoWebViewInput input;

        bool isSynchronizing;
        bool IsSynchronizing
        {
            get => isSynchronizing;
            set
            {
                isSynchronizing = value;
                syncFeedback.SetActive(value);
            }
        }
        int currentScrollXPosition, currentScrollYPosition;
       
        string previousUrl, lastLoadedUrl;
        ulong id;
        bool isInit = false;

        #region Methods

        public override void Init(UMI3DWebViewDto dto)
        {
            base.Init(dto);

            id = dto.id;
        }

        void Awake()
        {
            javaWebview = GetComponentInChildren<AndroidJavaWebview>();
            webviewRendering = GetComponentInChildren<UnityGeckoWebViewRendering>();
            input = GetComponentInChildren<UnityGeckoWebViewInput>();
        }

        protected void Start()
        {
            webViewCanvasScaler.dynamicPixelsPerUnit = 3;
            webViewCanvas.sortingOrder = 1;

            synchronizeRectTransform.gameObject.SetActive(false);

            StartCoroutine(SynchronizationCoroutine());

            IsSynchronizing = false;
        }

        /// <summary>
        /// If user is synchronizing his view, send his current url and scroll offset.
        /// </summary>
        /// <returns></returns>
        IEnumerator SynchronizationCoroutine()
        {
            var wait = new WaitForSeconds(synchronizationDelay);

            while (true)
            {

                int scrollY = javaWebview.GetScrollY();
                int scrollX = javaWebview.GetScrollX();

                if (currentScrollXPosition != scrollX || currentScrollYPosition != scrollY)
                {
                    currentScrollXPosition = scrollX;
                    currentScrollYPosition = scrollY;

                    var request = new WebViewUrlChangedRequestDto
                    {
                        url = lastLoadedUrl,
                        webViewId = id,
                        scrollOffset = new() { X = scrollX, Y = scrollY },
                    };

                    UMI3DClientServer.SendRequest(request, true);
                }

                yield return wait;
            }
        }

        

        protected override void OnCanInteractChanged(bool canInteract)
        {
            input.enabled = canInteract;

            bottomBarContainer.gameObject.SetActive(canInteract);
            topBarContainer.gameObject.SetActive(canInteract);
        }

        protected override void OnSizeChanged(Vector2 size)
        {
            container.localScale = new Vector3(size.x, size.y, 1);

            var corners = new Vector3[4];

            textureTransform.GetWorldCorners(corners);

            bottomBarContainer.position = (corners[1] + corners[2]) / 2f;
            topBarContainer.position = (corners[0] + corners[3]) / 2f;

            topBarContainer.localScale = new Vector3(topBarContainer.localScale.x,
                topBarContainer.localScale.y / container.localScale.y, topBarContainer.localScale.z);

            searchFieldTransform.localScale = new Vector3(
                searchFieldTransform.localScale.x / container.localScale.x,
                searchFieldTransform.localScale.y,
                searchFieldTransform.localScale.z
            );

            searchButton.localScale = new Vector3(searchButton.localScale.x / container.localScale.x,
                searchButton.localScale.y, searchButton.localScale.z);

            synchronizeRectTransform.localScale = new Vector3(synchronizeRectTransform.localScale.x / container.localScale.x,
                    synchronizeRectTransform.localScale.y, synchronizeRectTransform.localScale.z);

            bottomBarContainer.localScale = new Vector3(bottomBarContainer.localScale.x,
                  bottomBarContainer.localScale.y / container.localScale.y, bottomBarContainer.localScale.z);

            backButton.localScale = new Vector3(backButton.localScale.x / container.localScale.x,
                  backButton.localScale.y, backButton.localScale.z);

            forwardButton.localScale = new Vector3(forwardButton.localScale.x / container.localScale.x,
                forwardButton.localScale.y, forwardButton.localScale.z);

            homeButton.localScale = new Vector3(homeButton.localScale.x / container.localScale.x,
                homeButton.localScale.y, homeButton.localScale.z);
        }

        protected override void OnAdminStatusChanged(bool status)
        {
            synchronizeRectTransform.gameObject.SetActive(status);
        }

        protected override void OnTextureSizeChanged(Vector2 size)
        {
            webviewRendering.ChangeTextureSize((int)size.x, (int)size.y);
        }

        protected async override void OnUrlChanged(string url)
        {
            IsSynchronizing = false;

            while (!isInit)
                await UMI3DAsyncManager.Yield();

            javaWebview.LoadUrl(url);

            if (url == previousUrl)
            {
                return;
            }

            previousUrl = url;
            searchField.text = url;
        }

        protected override async void OnScrollOffsetChanged(Vector2 scroll)
        {
            try
            {
                javaWebview.SetScroll((int)scroll.x, (int)scroll.y);
            }
            catch (Exception ex)
            {
                Debug.LogError("Impossible to set offset url " + url);
                Debug.LogException(ex);

                await UMI3DAsyncManager.Delay(5000);
            }
        }

        

        

        public void OnUrlLoaded(string url)
        {
            isInit = true;

            lastLoadedUrl = url;

            UMI3DClientServer.SendRequest(
                new WebViewUrlChangedRequestDto
                {
                    url = url,
                    webViewId = id
                }, 
                true
            );
        }

        

        public void ToggleSynchronization()
        {
            IsSynchronizing = !IsSynchronizing;

            var request = new WebViewSynchronizationRequestDto
            {
                webViewId = id
            };

            UMI3DClientServer.SendRequest(request, true);
        }

        #endregion
    }
}