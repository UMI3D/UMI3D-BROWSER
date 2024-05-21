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
        #region Methods

        [SerializeField]
        private UnityGeckoWebViewInput input = null;

        [SerializeField]
        private RectTransform bottomBarContainer = null;

        [SerializeField]
        private RectTransform backButton = null;

        [SerializeField]
        private RectTransform homeButton = null;

        [SerializeField]
        private RectTransform synchronizeRectTransform = null;

        [SerializeField]
        private RectTransform forwardButton = null;

        [SerializeField]
        private RectTransform topBarContainer = null;

        [SerializeField]
        private RectTransform searchButton = null;

        [SerializeField]
        private UnityGeckoWebView webView = null;

        [SerializeField]
        private RectTransform container = null;

        [SerializeField]
        private RectTransform textureTransform = null;

        [SerializeField]
        private RectTransform searchFieldTransform;

        [SerializeField]
        private InputField searchField;

        [SerializeField]
        private RectTransform keyboard;

        [Space]
        [SerializeField, Tooltip("Delay to send current url and scroll offset when user synchronizes his content. In seconds")]
        float synchronizationDelay = 2f;

        [SerializeField, Tooltip("A feedback to show user he's currently sharing its content")]
        GameObject syncFeedback;

        private bool isSynchronizing;
        private bool IsSynchronizing
        {
            get => isSynchronizing;
            set
            {
                isSynchronizing = value;
                syncFeedback.SetActive(value);
            }
        }

        private int currentScrollXPosition, currentScrollYPosition;

        private bool useSearchInput = false;

        private string previousUrl, lastLoadedUrl;

        private ulong id;

        private bool isInit = false;

        #endregion

        #region Methods

        public override void Init(UMI3DWebViewDto dto)
        {
            base.Init(dto);

            id = dto.id;
        }

        protected void Start()
        {
            //keyboard.Hide();

            GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 3;
            GetComponent<Canvas>().sortingOrder = 1;

            synchronizeRectTransform.gameObject.SetActive(false);

            StartCoroutine(SynchronizationCoroutine());

            IsSynchronizing = false;
        }

        /// <summary>
        /// If user is synchronizing his view, send his current url and scroll offset.
        /// </summary>
        /// <returns></returns>
        private IEnumerator SynchronizationCoroutine()
        {
            var wait = new WaitForSeconds(synchronizationDelay);

            while (true)
            {

                int scrollY = webView.GetScrollY();
                int scrollX = webView.GetScrollX();

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

        public void ToggleOnSearchInput()
        {
            useSearchInput = true;
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

            keyboard.position = new Vector3(
                keyboard.position.x,
                bottomBarContainer.position.y,
                keyboard.position.z
            );

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
            webView.ChangeTextureSize((int)size.x, (int)size.y);
        }

        protected async override void OnUrlChanged(string url)
        {
            IsSynchronizing = false;

            while (!isInit)
                await UMI3DAsyncManager.Yield();

            webView.LoadUrl(url);

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
                webView.SetScroll((int)scroll.x, (int)scroll.y);
            }
            catch (Exception ex)
            {
                Debug.LogError("Impossible to set offset url " + url);
                Debug.LogException(ex);

                await UMI3DAsyncManager.Delay(5000);
            }
        }

        public void EnterText(string text)
        {
            if (!useSearchInput)
                webView.EnterText(text);
        }

        public void DeleteCharacter()
        {
            if (!useSearchInput)
                webView.DeleteCharacter();
        }

        public void EnterCharacter()
        {
            if (!useSearchInput)
                webView.EnterCharacter();
        }

        public void OnUrlLoaded(string url)
        {
            isInit = true;

            if (searchField != null)
            {
                searchField.text = url;
            }

            var request = new WebViewUrlChangedRequestDto
            {
                url = url,
                webViewId = id
            };

            lastLoadedUrl = url;

            UMI3DClientServer.SendRequest(request, true);
        }

        public void OnPointerDown(Vector2 pointer)
        {
            useSearchInput = false;
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