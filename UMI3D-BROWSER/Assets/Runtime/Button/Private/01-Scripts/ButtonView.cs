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
using UnityEngine.UI;

namespace umi3d.browserRuntime.button
{
    [RequireComponent(typeof(Button)), ExecuteInEditMode]
    internal class ButtonView : MonoBehaviour
    {
        ButtonModelContainer _modelContainer;
        Button _button;

        void Awake()
        {
            _modelContainer= GetComponent<ButtonModelContainer>();
            _button = GetComponent<Button>();

            _button.onClick.AddListener(_modelContainer.Model.Click);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ButtonNotificationKeys.ButtonSet>(),
                (Callback)ButtonSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
            _button.onClick.RemoveListener(_modelContainer.Model.Click);
        }


        void ButtonSet(Notification notification)
        {
            if (notification.TryGetInfoT(ButtonNotificationKeys.ButtonSet.Sprite, out Sprite sprite, false))
                _button.image.sprite = sprite;
            if (notification.TryGetInfoT(ButtonNotificationKeys.ButtonSet.ColorBlock, out ColorBlock colorBlock, false))
                _button.colors = colorBlock;

            if (notification.TryGetInfoT(ButtonNotificationKeys.ButtonSet.Position, out Vector3 position, false))
                transform.position = position;
            if (notification.TryGetInfoT(ButtonNotificationKeys.ButtonSet.Size, out Vector2 size, false))
                ((RectTransform)transform).sizeDelta = size;

            if (notification.TryGetInfoT(ButtonNotificationKeys.ButtonSet.AnchorMin, out Vector2 anchorMin, false))
                ((RectTransform)transform).anchorMin = anchorMin;
            if (notification.TryGetInfoT(ButtonNotificationKeys.ButtonSet.AnchorMax, out Vector2 anchorMax, false))
                ((RectTransform)transform).anchorMax = anchorMax;
            if (notification.TryGetInfoT(ButtonNotificationKeys.ButtonSet.Pivot, out Vector2 pivot, false))
                ((RectTransform)transform).pivot = pivot;
        }
    }
}