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

namespace umi3d.browserRuntime.forms
{
    [RequireComponent(typeof(TMP_Text)), ExecuteAlways]
    internal class FormItemLabelView : MonoBehaviour
    {
        FormItemModelContainer _modelContainer;
        TMP_Text _text;

        void Awake()
        {
            _modelContainer = GetComponentInParent<FormItemModelContainer>();
            _text = GetComponent<TMP_Text>();

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
            if (notification.TryGetInfoT(FormNotificationKeys.ItemSet.TextFontSize, out int textFontSize, false))
                _text.fontSize = textFontSize;
            if (notification.TryGetInfoT(FormNotificationKeys.ItemSet.TextColor, out Color textColor, false))
                _text.color = textColor;
            if (notification.TryGetInfoT(FormNotificationKeys.ItemSet.TextStyles, out FontStyles textStyles, false))
                _text.fontStyle = textStyles;
            if (notification.TryGetInfoT(FormNotificationKeys.ItemSet.TextAlignmentOptions, out TextAlignmentOptions textAlignementOptions, false))
                _text.alignment = textAlignementOptions;
        }
    }
}