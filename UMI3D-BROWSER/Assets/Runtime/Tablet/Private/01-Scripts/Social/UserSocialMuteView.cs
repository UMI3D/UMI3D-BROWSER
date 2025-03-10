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
    [RequireComponent(typeof(Button))]
    public class UserSocialMuteView : MonoBehaviour
    {
        [SerializeField] private Sprite _muteSprite;
        [SerializeField] private Sprite _unmuteSprite;

        Image _image;
        Button _button;

        UserSocialModelContainer _modelContainer;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _button = GetComponent<Button>();
            _modelContainer = GetComponentInParent<UserSocialModelContainer>();

            _button.onClick.AddListener(OnClick);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<UserSocialNotificationKeys.UserSocialSet>(),
                (Callback)UserSocialSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));

            NotificationHub.Default.Subscribe(this,
                ID.FromType<UserSocialNotificationKeys.UserSocialUpdate>(),
                (Callback)UserSocialUpdate,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
            NotificationHub.Default.Unsubscribe(this);
        }

        void OnClick()
        {
            _modelContainer.Model.UpdateMute(!_modelContainer.Model.IsMute);
        }

        void UserSocialSet(Notification notification)
        {
            if (notification.TryGetInfoT(UserSocialNotificationKeys.UserSocialSet.IsMute, out bool isMute))
                SetSprite(isMute);
        }

        void UserSocialUpdate(Notification notification)
        {
            if (notification.TryGetInfoT(UserSocialNotificationKeys.UserSocialUpdate.IsMute, out bool isMute))
                SetSprite(isMute);
        }

        void SetSprite(bool isMute)
        {
            _image.sprite = isMute ? _muteSprite : _unmuteSprite;
        }
    }
}