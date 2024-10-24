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

using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.UX;
using UnityEngine;
using umi3d.browserRuntime.NotificationKeys;
using inetum.unityUtils;
using umi3d.cdk.notification;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardPivot : MonoBehaviour
    {
        [SerializeField] Transform target;

        LazyRotationAndTranslation lazyRotationAndTranslation;
        Request playerRequest;

        void Awake()
        {
            lazyRotationAndTranslation = GetComponent<LazyRotationAndTranslation>();

            playerRequest = RequestHub.Default.SubscribeAsClient<UMI3DClientRequestKeys.PlayerRequest>(this);

        }

        void OnEnable()
        {
            playerRequest.supplierChanged += PlayerRequest_supplierChanged;

            NotificationHub.Default.Subscribe<KeyboardNotificationKeys.TextFieldSelected>(
                this,
                TextFieldSelected
            );
        }

        void OnDisable()
        {
            playerRequest.supplierChanged -= PlayerRequest_supplierChanged;

            NotificationHub.Default.Unsubscribe<KeyboardNotificationKeys.TextFieldSelected>(this);
        }

        void PlayerRequest_supplierChanged()
        {
            if (target != null)
            {
                return;
            }

            if (!playerRequest.TryGetInfoT(UMI3DClientRequestKeys.PlayerRequest.Camera, out Camera camera))
            {
                return;
            }
            target = camera.transform;

            lazyRotationAndTranslation.target = target;
            lazyRotationAndTranslation.Rest();
        }

        void TextFieldSelected(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.TextFieldSelected.IsPreviewBar, out bool isPreviewBar) || isPreviewBar)
            {
                return;
            }

            lazyRotationAndTranslation.Rest();
        }
    }
}