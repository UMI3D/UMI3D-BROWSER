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
using static umi3d.browserRuntime.image.ImageFactory.Settings;

namespace umi3d.browserRuntime.image
{
    public class ImageModel
    {
        public Sprite Sprite { get; private set; } = null;
        public Color Color { get; private set; } = Color.white;
        public Vector3 Position { get; private set; } = Vector3.zero;
        public Vector2 Size { get; private set; } = Vector2.one;
        public Vector2 AnchorMin { get; private set; } = new Vector2(0.5f, 0.5f);
        public Vector2 AnchorMax { get; private set; } = new Vector2(0.5f, 0.5f);
        public Vector2 Pivot { get; private set; } = new Vector2(0.5f, 0.5f);

        private Notifier _setNotifier;

        public ImageModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<ImageNotificationKeys.ImageSet>());
        }

        public void SetSprite(Sprite sprite)
        {
            Sprite = sprite;
            _setNotifier[ImageNotificationKeys.ImageSet.Sprite] = Sprite;
            _setNotifier.Notify();
        }

        public void SetColor(Color? color)
        {
            Color = color ?? Color.white;
            _setNotifier[ImageNotificationKeys.ImageSet.Color] = Color;
            _setNotifier.Notify();
        }

        public void SetPosition(Vector3? position)
        {
            if (position.HasValue)
            {
                Position = position.Value;
                _setNotifier[ImageNotificationKeys.ImageSet.Position] = Position;
                _setNotifier.Notify();
            }
        }

        public void SetSize(Vector2? size)
        {
            if (size.HasValue)
            {
                Size = size.Value;
                _setNotifier[ImageNotificationKeys.ImageSet.Size] = Size;
                _setNotifier.Notify();
            }
        }

        public void SetAnchor(Vector2? anchorMin, Vector2? anchorMax, Vector2? pivot)
        {
            bool hasChanged = false;
            if (anchorMin.HasValue)
            {
                AnchorMin = anchorMin.Value;
                _setNotifier[ImageNotificationKeys.ImageSet.AnchorMin] = AnchorMin;
                hasChanged = true;
            }
            if (anchorMax.HasValue)
            {
                AnchorMax = anchorMax.Value;
                _setNotifier[ImageNotificationKeys.ImageSet.AnchorMax] = AnchorMax;
                hasChanged = true;
            }
            if (pivot.HasValue)
            {
                Pivot = pivot.Value;
                _setNotifier[ImageNotificationKeys.ImageSet.Pivot] = Pivot;
                hasChanged = true;
            }

            if (hasChanged)
                _setNotifier.Notify();
        }
    }
}