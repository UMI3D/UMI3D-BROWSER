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

        private bool useSearchInput = false;

        private string previousUrl;

        private ulong id;

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

            Vector3[] corners = new Vector3[4];

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


            bottomBarContainer.localScale = new Vector3(bottomBarContainer.localScale.x,
                  bottomBarContainer.localScale.y / container.localScale.y, bottomBarContainer.localScale.z);

            backButton.localScale = new Vector3(backButton.localScale.x / container.localScale.x,
                  backButton.localScale.y, backButton.localScale.z);

            forwardButton.localScale = new Vector3(forwardButton.localScale.x / container.localScale.x,
                forwardButton.localScale.y, forwardButton.localScale.z);

            homeButton.localScale = new Vector3(homeButton.localScale.x / container.localScale.x,
                homeButton.localScale.y, homeButton.localScale.z);
        }

        protected override void OnTextureSizeChanged(Vector2 size)
        {
            webView.ChangeTextureSize((int)size.x, (int)size.y);
        }

        protected override void OnUrlChanged(string url)
        {
            webView.LoadUrl(url);

            if (url == previousUrl)
            {
                return;
            }

            previousUrl = url;

            searchField.text = url;

            var request = new WebViewUrlChangedRequestDto
            {
                url = url,
                webViewId = id
            };

            UMI3DClientServer.SendRequest(request, true);
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
            if (searchField != null)
            {
                searchField.text = url;
            }
        }

        public void OnPointerDown(Vector2 pointer)
        {
            useSearchInput = false;
        }

        #endregion
    }
}