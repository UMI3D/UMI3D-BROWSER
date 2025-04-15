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

namespace umi3d.browserRuntime.image
{
    [RequireComponent(typeof(Image)), ExecuteInEditMode]
    internal class ImageView : MonoBehaviour
    {
        ImageModelContainer _modelContainer;
        Image _image;

        void Awake()
        {
            _modelContainer = GetComponent<ImageModelContainer>();
            _image = GetComponent<Image>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ImageNotificationKeys.ImageSet>(),
                (Callback)TextSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void TextSet(Notification notification)
        {
            if (notification.TryGetInfoT(ImageNotificationKeys.ImageSet.Sprite, out Sprite sprite, false))
                _image.sprite = sprite;
            if (notification.TryGetInfoT(ImageNotificationKeys.ImageSet.Color, out Color color, false))
                _image.color = color;
        }
    }
}