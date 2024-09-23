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
using inetum.unityUtils;
using System.Collections;
using umi3d.cdk;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.android
{
    public class UMI3DUnityGeckoWebView : AbstractUMI3DWebView
    {
        [SerializeField] RectTransform webviewRT = null;
       
        string lastLoadedUrl;
        ulong id;
        bool isInit = false;

        Notifier searchNotifier;
        Notifier textureSizeChangedNotifier;
        Notifier sizeChangedNotifier;
        Notifier ScrollNotifier;
        Notifier interactibilityNotifier;
        Notifier synchronizationNotifier;

        #region Methods

        void Awake()
        {
            UnityEngine.Debug.Assert(webviewRT != null, $"Web view rectTransform is not referenced.");

            searchNotifier = NotificationHub.Default.GetNotifier(
                this,
                GeckoWebViewNotificationKeys.Search
            );
            textureSizeChangedNotifier = NotificationHub.Default.GetNotifier(
                this,
                GeckoWebViewNotificationKeys.TextureSizeChanged
            );
            sizeChangedNotifier = NotificationHub.Default.GetNotifier(
                this,
                GeckoWebViewNotificationKeys.SizeChanged
            );
            ScrollNotifier = NotificationHub.Default.GetNotifier(
                this,
                GeckoWebViewNotificationKeys.ScrollChanged
            );
            interactibilityNotifier = NotificationHub.Default.GetNotifier(
                this,
                GeckoWebViewNotificationKeys.InteractibilityChanged
            );
            synchronizationNotifier = NotificationHub.Default.GetNotifier(
                this,
                GeckoWebViewNotificationKeys.SynchronizationChanged
            );
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.Loading,
                UrlLoaded
            );

            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.SynchronizationChanged,
                new FilterByRef(FilterType.AcceptAllExcept, this),
                Synchronize
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.Loading);

            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.SynchronizationChanged);
        }

        public override void Init(UMI3DWebViewDto dto)
        {
            base.Init(dto);

            id = dto.id;
        }

        protected override void OnCanInteractChanged(bool canInteract)
        {
            interactibilityNotifier[GeckoWebViewNotificationKeys.Info.Interactable] = canInteract;
            interactibilityNotifier.Notify();
        }

        protected override void OnSizeChanged(Vector2 size)
        {
            Vector3[] corners = new Vector3[4];
            webviewRT.GetWorldCorners(corners);

            sizeChangedNotifier[GeckoWebViewNotificationKeys.Info.Vector2] = size;
            sizeChangedNotifier[GeckoWebViewNotificationKeys.Info.CornersPosition] = corners;
            sizeChangedNotifier.Notify();
        }

        protected override void OnTextureSizeChanged(Vector2 size)
        {
            textureSizeChangedNotifier[GeckoWebViewNotificationKeys.Info.Vector2] = size;
            textureSizeChangedNotifier.Notify();
        }

        /// <summary>
        /// Method called when the url changed because of a synchronisation.
        /// </summary>
        /// <param name="url"></param>
        protected async override void OnUrlChanged(string url)
        {
            synchronizationNotifier[GeckoWebViewNotificationKeys.Info.IsDesynchronized] = false;
            synchronizationNotifier.Notify();

            while (!isInit)
            {
                await UMI3DAsyncManager.Yield();
            }

            searchNotifier[GeckoWebViewNotificationKeys.Info.URL] = url;
            searchNotifier.Notify();
        }

        protected override void OnAdminStatusChanged(bool status)
        {
            synchronizationNotifier[GeckoWebViewNotificationKeys.Info.IsAdmin] = status;
            synchronizationNotifier.Notify();
        }

        protected override void OnScrollOffsetChanged(Vector2 scroll)
        {
            ScrollNotifier[GeckoWebViewNotificationKeys.Info.Vector2] = scroll;
            ScrollNotifier.Notify();
        }

        void UrlLoaded(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.URL, out string url))
            {
                return;
            }

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

        void Synchronize(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.IsSynchronizing, out bool isSynchronizing))
            {
                return;
            }

            if (isSynchronizing)
            {
                UMI3DClientServer.SendRequest(
                    new WebViewSynchronizationRequestDto
                    {
                        webViewId = id
                    },
                    true
                );
                return;
            }

            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.Vector2, out Vector2 scroll))
            {
                return;
            }

            if (scroll != new Vector2(float.NaN, float.NaN))
            {
                UMI3DClientServer.SendRequest(
                    new WebViewUrlChangedRequestDto
                    {
                        url = lastLoadedUrl,
                        webViewId = id,
                        scrollOffset = new() { X = scroll.x, Y = scroll.y },
                    },
                    true
                );
                return;
            }
        }

        #endregion
    }
}