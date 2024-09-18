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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.inetum.unitygeckowebview
{
    [RequireComponent(typeof(Button))]
    public class SearchButton : MonoBehaviour
    {
        Button button;
        RectTransform rectTransform;

        void Awake()
        {
            button = GetComponent<Button>();

            rectTransform = GetComponent<RectTransform>();
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.SizeChanged,
                SizeChanged
            );

            NotificationHub.Default.Subscribe(
               this,
               GeckoWebViewNotificationKeys.InteractibilityChanged,
               InteractibilityChanged
           );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.SizeChanged);

            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.InteractibilityChanged);
        }

        void SizeChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.Vector2, out Vector2 size))
            {
                return;
            }

            rectTransform.localScale = new Vector3(
                rectTransform.localScale.x / size.x,
                rectTransform.localScale.y,
                rectTransform.localScale.z
            );
        }

        void InteractibilityChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.Interactable, out bool interactable))
            {
                return;
            }

            button.interactable = interactable;
        }
    }
}