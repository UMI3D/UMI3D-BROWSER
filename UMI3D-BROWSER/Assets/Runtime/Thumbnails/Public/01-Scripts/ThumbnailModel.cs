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
using UnityEngine;

namespace umi3d.browserRuntime.thumbnails
{
    public class ThumbnailModel
    {
        public string Name { get; private set; } = string.Empty;
        public Sprite Image { get; private set; } = null;

        public Color NormalColor { get; private set; } = Color.white;
        public Color HoverColor { get; private set; } = Color.white;

        public Vector3 Position { get; private set; } = Vector3.zero;
        public Vector2 Size { get; private set; } = Vector2.one;

        public Vector2 AnchorMin { get; private set; } = Vector2.one;
        public Vector2 AnchorMax { get; private set; } = Vector2.one;
        public Vector2 Pivot { get; private set; } = Vector2.one;

        public bool Hover { get; private set; } = false;

        internal Action _callback = null;

        private readonly Notifier _setNotifier;
        private readonly Notifier _updateNotifier;

        public ThumbnailModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this, ID.FromType<ThumbnailNotificationKeys.ThumbnailSet>());
            _updateNotifier = NotificationHub.Default.GetNotifier(this, ID.FromType<ThumbnailNotificationKeys.ThumbnailUpdated>());
        }

        public void SetName(string name)
        {
            Name = name ?? string.Empty;
            _setNotifier[ThumbnailNotificationKeys.ThumbnailSet.Name] = Name;
            _setNotifier.Notify();
        }

        public void UpdateName(string name)
        {
            Name = name ?? string.Empty;
            _updateNotifier[ThumbnailNotificationKeys.ThumbnailUpdated.Name] = Name;
            _updateNotifier.Notify();
        }

        public void SetImage(Sprite image)
        {
            Image = image;
            _setNotifier[ThumbnailNotificationKeys.ThumbnailSet.Image] = Image;
            _setNotifier.Notify();
        }

        public void SetColors(Color? normalColor, Color? hoverColor)
        {
            NormalColor = normalColor.HasValue ? normalColor.Value : Color.white;
            HoverColor = hoverColor.HasValue ? hoverColor.Value : Color.white;
            _setNotifier[ThumbnailNotificationKeys.ThumbnailSet.Color] = NormalColor;
            _setNotifier.Notify();
        }

        public void SetPosition(Vector3? position)
        {
            if (position.HasValue)
            {
                Position = position.Value;
                _setNotifier[ThumbnailNotificationKeys.ThumbnailSet.Position] = Position;
                _setNotifier.Notify();
            }
        }

        public void SetSize(Vector2? size)
        {
            if (size.HasValue)
            {
                Size = size.Value;
                _setNotifier[ThumbnailNotificationKeys.ThumbnailSet.Size] = Size;
                _setNotifier.Notify();
            }
        }

        public void SetAnchor(Vector2? anchorMin, Vector2? anchorMax, Vector2? pivot)
        {
            bool hasChanged = false;
            if (anchorMin.HasValue)
            {
                AnchorMin = anchorMin.Value;
                _setNotifier[ThumbnailNotificationKeys.ThumbnailSet.AnchorMin] = AnchorMin;
                hasChanged = true;
            }
            if (anchorMax.HasValue)
            {
                AnchorMax = anchorMax.Value;
                _setNotifier[ThumbnailNotificationKeys.ThumbnailSet.AnchorMax] = AnchorMax;
                hasChanged = true;
            }
            if (pivot.HasValue)
            {
                Pivot = pivot.Value;
                _setNotifier[ThumbnailNotificationKeys.ThumbnailSet.Pivot] = Pivot;
                hasChanged = true;
            }

            if (hasChanged)
                _setNotifier.Notify();
        }

        public void SetCallback(Action callback)
        {
            _callback = callback;
        }

        public void Click()
        {
            _callback?.Invoke();
        }

        internal void UpdateHover(bool isHover)
        {
            Hover = isHover;
            _setNotifier[ThumbnailNotificationKeys.ThumbnailUpdated.Color] = Hover ? HoverColor : NormalColor;
        }
    }
}