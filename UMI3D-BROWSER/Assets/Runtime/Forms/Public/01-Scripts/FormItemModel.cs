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
using System;
using TMPro;
using umi3d.browserRuntime.button;
using UnityEngine;

namespace umi3d.browserRuntime.forms
{
    public class FormItemModel
    {
        public Vector3 Position { get; private set; } = Vector3.zero;
        public Vector2 Size { get; private set; } = Vector2.one;

        public Vector2 AnchorMin { get; private set; } = new Vector2(.5f, .5f);
        public Vector2 AnchorMax { get; private set; } = new Vector2(.5f, .5f);
        public Vector2 Pivot { get; private set; } = new Vector2(.5f, .5f);

        public int TextFontSize { get; private set; } = 12;
        public Color TextColor { get; private set; } = Color.white;
        public FontStyles TextStyles { get; private set; } = FontStyles.Normal;
        public TextAlignmentOptions TextAlignmentOptions { get; private set; } = TextAlignmentOptions.MidlineLeft;

        private readonly Notifier _setNotifier;

        public FormItemModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<FormNotificationKeys.ItemSet>());
        }

        public void SetPosition(Vector3? position)
        {
            if (position.HasValue)
            {
                Position = position.Value;
                _setNotifier[FormNotificationKeys.ItemSet.Position] = Position;
                _setNotifier.Notify();
            }
        }

        public void SetSize(Vector2? size)
        {
            if (size.HasValue)
            {
                Size = size.Value;
                _setNotifier[FormNotificationKeys.ItemSet.Size] = Size;
                _setNotifier.Notify();
            }
        }

        public void SetAnchor(Vector2? anchorMin, Vector2? anchorMax, Vector2? pivot)
        {
            bool hasChanged = false;
            if (anchorMin.HasValue)
            {
                AnchorMin = anchorMin.Value;
                _setNotifier[FormNotificationKeys.ItemSet.AnchorMin] = AnchorMin;
                hasChanged = true;
            }
            if (anchorMax.HasValue)
            {
                AnchorMax = anchorMax.Value;
                _setNotifier[FormNotificationKeys.ItemSet.AnchorMax] = AnchorMax;
                hasChanged = true;
            }
            if (pivot.HasValue)
            {
                Pivot = pivot.Value;
                _setNotifier[FormNotificationKeys.ItemSet.Pivot] = Pivot;
                hasChanged = true;
            }

            if (hasChanged)
                _setNotifier.Notify();
        }

        public void SetTextStyle(int? textFontSize, Color? textColor, FontStyles? textFontStyles, TextAlignmentOptions? textAlignmentOptions)
        {
            bool hasChanged = false;
            if (textFontSize.HasValue)
            {
                TextFontSize = textFontSize.Value;
                _setNotifier[FormNotificationKeys.ItemSet.TextFontSize] = TextFontSize;
                hasChanged = true;
            }
            if (textColor.HasValue)
            {
                TextColor = textColor.Value;
                _setNotifier[FormNotificationKeys.ItemSet.TextColor] = TextColor;
                hasChanged = true;
            }
            if (textFontStyles.HasValue)
            {
                TextStyles = textFontStyles.Value;
                _setNotifier[FormNotificationKeys.ItemSet.TextStyles] = TextStyles;
                hasChanged = true;
            }
            if (textAlignmentOptions.HasValue)
            {
                TextAlignmentOptions = textAlignmentOptions.Value;
                _setNotifier[FormNotificationKeys.ItemSet.TextAlignmentOptions] = TextAlignmentOptions;
                hasChanged = true;
            }

            if (hasChanged)
                _setNotifier.Notify();
        }
    }
}