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

namespace umi3d.browserRuntime.image
{
    public class ImageModel
    {
        public Sprite Sprite { get; private set; } = null;
        public Color Color { get; private set; } = Color.white;

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
    }
}