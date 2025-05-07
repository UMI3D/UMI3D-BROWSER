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
            {
                colorBlock.colorMultiplier = _button.colors.colorMultiplier;
                colorBlock.fadeDuration = _button.colors.fadeDuration;
                _button.colors = colorBlock;
            }
        }
    }
}