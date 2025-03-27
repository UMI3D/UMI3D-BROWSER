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
using TMPro;
using UnityEngine;

namespace umi3d.browserRuntime.text
{
    [RequireComponent(typeof(TMP_Text)), ExecuteInEditMode]
    internal class TextView : MonoBehaviour
    {
        TextModelContainer _modelContainer;
        TMP_Text _text;

        void Awake()
        {
            _modelContainer = GetComponentInParent<TextModelContainer>();
            _text = GetComponent<TMP_Text>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<TextNotificationKeys.TextSet>(),
                (Callback)TextSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void TextSet(Notification notification)
        {
            if (notification.TryGetInfoT(TextNotificationKeys.TextSet.Text, out string text, false))
                _text.text = text;

            if (notification.TryGetInfoT(TextNotificationKeys.TextSet.Position, out Vector3 position, false))
                transform.position = position;
            if (notification.TryGetInfoT(TextNotificationKeys.TextSet.Size, out Vector3 size, false))
                transform.localScale = size;

            if (notification.TryGetInfoT(TextNotificationKeys.TextSet.AnchorMin, out Vector2 anchorMin, false))
                ((RectTransform)transform).anchorMin = anchorMin;
            if (notification.TryGetInfoT(TextNotificationKeys.TextSet.AnchorMax, out Vector2 anchorMax, false))
                ((RectTransform)transform).anchorMax = anchorMax;
            if (notification.TryGetInfoT(TextNotificationKeys.TextSet.Pivot, out Vector2 pivot, false))
                ((RectTransform)transform).pivot = pivot;

            if (notification.TryGetInfoT(TextNotificationKeys.TextSet.TextFontSize, out int textFontSize, false))
                _text.fontSize = textFontSize;
            if (notification.TryGetInfoT(TextNotificationKeys.TextSet.TextColor, out Color textColor, false))
                _text.color = textColor;
            if (notification.TryGetInfoT(TextNotificationKeys.TextSet.TextStyles, out FontStyles textStyles, false))
                _text.fontStyle = textStyles;
            if (notification.TryGetInfoT(TextNotificationKeys.TextSet.TextAlignementOptions, out TextAlignmentOptions textAlignementOptions, false))
                _text.alignment = textAlignementOptions;
        }
    }
}