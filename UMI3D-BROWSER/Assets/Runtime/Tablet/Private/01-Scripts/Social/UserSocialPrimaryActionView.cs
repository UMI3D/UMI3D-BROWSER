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
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.tablet.social
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class UserSocialPrimaryActionView : MonoBehaviour
    {
        int _actionIndex => transform.GetSiblingIndex();

        Button _button;
        Image _image;

        UserSocialModelContainer _modelContainer;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();

            _modelContainer = GetComponentInParent<UserSocialModelContainer>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<UserSocialNotificationKeys.UserSocialSet>(),
                (Callback)UserSocialSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        async void UserSocialSet(Notification notification)
        {
            if (!notification.TryGetInfoT(UserSocialNotificationKeys.UserSocialSet.PrimaryActions, out List<UserAction> actions))
                return;

            if (actions.Count <= _actionIndex)
            {
                gameObject.SetActive(false);
                return;
            }

            // Image
            var texture = await actions[_actionIndex].GetTexture();
            if (texture != null)
                _image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            // Button
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(actions[_actionIndex].Call);

            gameObject.SetActive(true);
        }
    }
}