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

namespace umi3d.browserRuntime.ui.tablet.social
{
    [RequireComponent(typeof(Image))]
    public class UserSocialOtherActionIconView : MonoBehaviour
    {
        Image _image;

        UserSocialOtherActionModelContainer _modelContainer;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _modelContainer = GetComponentInParent<UserSocialOtherActionModelContainer>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<UserSocialNotificationKeys.UserSocialOtherActionSet>(),
                (Callback)UserSocialOtherActionSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void UserSocialOtherActionSet(Notification notification)
        {
            if (notification.TryGetInfoT(UserSocialNotificationKeys.UserSocialOtherActionSet.Texture, out Texture2D texture))
            {
                if (texture != null) 
                    _image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        }
    }
}