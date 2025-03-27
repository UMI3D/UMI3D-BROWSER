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
using UnityEngine.UI;
using UnityEngine;

namespace umi3d.browserRuntime.button
{
    public class ButtonModel 
    {
        public string Label { get; private set; } = string.Empty;
        public Action Callback { get; private set; } = null;

        public Sprite Sprite { get; private set; } = null;
        public ColorBlock ColorBlock { get; private set; } = new();

        public Vector3 Position { get; private set; } = Vector3.zero;
        public Vector3 Size { get; private set; } = Vector3.one;

        public Vector2 AnchorMin { get; private set; } = new Vector2(.5f, .5f);
        public Vector2 AnchorMax { get; private set; } = new Vector2(.5f, .5f);
        public Vector2 Pivot { get; private set; } = new Vector2(.5f, .5f);

        public int TextFontSize { get; private set; } = 12;
        public Color TextColor { get; private set; } = Color.white;
        public FontStyles TextStyles { get; private set; } = FontStyles.Normal;
        public TextAlignmentOptions TextAlignementOptions { get; private set; } = TextAlignmentOptions.MidlineLeft;

        internal Action _callback;

        Notifier _setNotifier;

        public ButtonModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<ButtonNotificationKeys.ButtonSet>());
        }

        public void SetLabel(string label)
        {
            Label = label ?? string.Empty;
            _setNotifier[ButtonNotificationKeys.ButtonSet.Label] = Label;
            _setNotifier.Notify();
        }

        public void SetCallback(Action callback)
        {
            _callback = callback;
        }

        public void SetImage(ColorBlock colors, Sprite sprite = null)
        {
            ColorBlock = colors;
            Sprite = sprite;
            _setNotifier[ButtonNotificationKeys.ButtonSet.Sprite] = Sprite;
            _setNotifier[ButtonNotificationKeys.ButtonSet.ColorBlock] = ColorBlock;
            _setNotifier.Notify();
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
            _setNotifier[ButtonNotificationKeys.ButtonSet.Position] = Position;
            _setNotifier.Notify();
        }

        public void SetSize(Vector3 size)
        {
            Size = size;
            _setNotifier[ButtonNotificationKeys.ButtonSet.Size] = Size;
            _setNotifier.Notify();
        }

        public void SetAnchor(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            AnchorMin = anchorMin;
            AnchorMax = anchorMax;
            Pivot = pivot;
            _setNotifier[ButtonNotificationKeys.ButtonSet.AnchorMin] = AnchorMin;
            _setNotifier[ButtonNotificationKeys.ButtonSet.AnchorMax] = AnchorMax;
            _setNotifier[ButtonNotificationKeys.ButtonSet.Pivot] = Pivot;
            _setNotifier.Notify();
        }

        public void SetTextStyle(int textFontSize, Color textColor, FontStyles textFontStyles, TextAlignmentOptions textAlignmentOptions)
        {
            TextFontSize = textFontSize;
            TextColor = textColor;
            TextStyles = textFontStyles;
            TextAlignementOptions = textAlignmentOptions;
            _setNotifier[ButtonNotificationKeys.ButtonSet.TextFontSize] = TextFontSize;
            _setNotifier[ButtonNotificationKeys.ButtonSet.TextColor] = TextColor;
            _setNotifier[ButtonNotificationKeys.ButtonSet.TextStyles] = TextStyles;
            _setNotifier[ButtonNotificationKeys.ButtonSet.TextAlignementOptions] = TextAlignementOptions;
            _setNotifier.Notify();
        }

        public void Click()
        {
            _callback?.Invoke();
        }
    }
}