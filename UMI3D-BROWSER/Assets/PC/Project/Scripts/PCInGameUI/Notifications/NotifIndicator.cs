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

using umi3dBrowsers.displayer;
using UnityEngine;

namespace umi3dBrowsers.ingame_ui.notifications
{
    public class NotifIndicator : MonoBehaviour
    {
        [SerializeField] private ButtonStyle buttonStyle;
        [Header("Linker")]
        [SerializeField] private NotificationScreen notificationScreen;

        [Header("Color")]
        [SerializeField] private Color overrideColor;
        private Color _normalColor;
        private Color _hoverColor;
        private Color _selectedColor;

        private void Awake()
        {
            _normalColor = buttonStyle.NormalColor;
            _hoverColor = buttonStyle.HoverColor;
            _selectedColor = buttonStyle.SelectedColor;

            notificationScreen.OnNotificationReceived += () => OverrideColors();
            notificationScreen.OnNotificationScreenOpened += () => ResetColors();
        }

        private void ResetColors()
        {
            buttonStyle.SetNormalColor(_normalColor);
            buttonStyle.SetHoverColor(_hoverColor);
            buttonStyle.SetSelectedColor(_selectedColor);

            buttonStyle.applyColor();
        }

        private void OverrideColors()
        {
            buttonStyle.SetNormalColor(overrideColor);
            buttonStyle.SetHoverColor(overrideColor);
            buttonStyle.SetSelectedColor(overrideColor);

            buttonStyle.applyColor();
        }
    }
}

