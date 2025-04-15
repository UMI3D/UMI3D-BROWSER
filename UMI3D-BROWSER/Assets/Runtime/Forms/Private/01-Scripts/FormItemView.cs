/*
Copyright 2019 - 2025 Inetum

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

using inetum.unityUtils.observation;
using UnityEngine;

namespace umi3d.browserRuntime.forms
{
    [ExecuteAlways]
    internal class FormItemView : MonoBehaviour
    {
        FormItemModelContainer _modelContainer;

        void Awake()
        {
            _modelContainer = GetComponent<FormItemModelContainer>();
            NotificationHub.Default.Subscribe(this,
                ID.FromType<FormNotificationKeys.ItemSet>(),
                (Callback)ButtonSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void ButtonSet(Notification notification)
        {
            if (notification.TryGetInfoT(FormNotificationKeys.ItemSet.Position, out Vector3 position, false))
                ((RectTransform)transform).anchoredPosition = position;
            if (notification.TryGetInfoT(FormNotificationKeys.ItemSet.Size, out Vector2 size, false))
                ((RectTransform)transform).sizeDelta = size;

            if (notification.TryGetInfoT(FormNotificationKeys.ItemSet.AnchorMin, out Vector2 anchorMin, false))
                ((RectTransform)transform).anchorMin = anchorMin;
            if (notification.TryGetInfoT(FormNotificationKeys.ItemSet.AnchorMax, out Vector2 anchorMax, false))
                ((RectTransform)transform).anchorMax = anchorMax;
            if (notification.TryGetInfoT(FormNotificationKeys.ItemSet.Pivot, out Vector2 pivot, false))
                ((RectTransform)transform).pivot = pivot;
        }
    }
}