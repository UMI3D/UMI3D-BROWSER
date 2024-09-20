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

namespace com.inetum.unitygeckowebview
{
    public class TopBottomBarContainer : MonoBehaviour
    {
        enum BarType
        {
            Top,
            Bottom
        }

        [SerializeField] BarType barType;

        RectTransform rectTransform;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                GeckoWebViewNotificationKeys.SizeChanged,
                SizeChanged
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, GeckoWebViewNotificationKeys.SizeChanged);
        }

        void SizeChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.Vector2, out Vector2 size))
            {
                return;
            }

            if (!notification.TryGetInfoT(GeckoWebViewNotificationKeys.Info.CornersPosition, out Vector3[] corners))
            {
                return;
            }

            switch (barType)
            {
                case BarType.Top:
                    rectTransform.position = (corners[0] + corners[3]) / 2f;
                    break;
                case BarType.Bottom:
                    rectTransform.position = (corners[1] + corners[2]) / 2f;
                    break;
                default:
                    break;
            }

            rectTransform.localScale = new Vector3(
                rectTransform.localScale.x,
                rectTransform.localScale.y / size.y,
                rectTransform.localScale.z
            );
        }
    }
}