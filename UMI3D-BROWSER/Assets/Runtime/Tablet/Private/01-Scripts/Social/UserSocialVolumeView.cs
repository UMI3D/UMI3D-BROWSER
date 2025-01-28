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
    [RequireComponent(typeof(Slider))]
    public class UserSocialVolumeView : MonoBehaviour
    {
        Slider _slider;

        UserSocialModelContainer _modelContainer;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _modelContainer = GetComponentInParent<UserSocialModelContainer>();

            _slider.onValueChanged.AddListener(OnValueChanged);

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
            _slider.onValueChanged.RemoveListener(OnValueChanged);
            NotificationHub.Default.Unsubscribe(this);
        }

        void OnValueChanged(float newValue)
        {
            _modelContainer.Model.UpdateVolume(newValue);
        }

        void UserSocialSet(Notification notification)
        {
            if (notification.TryGetInfoT(UserSocialNotificationKeys.UserSocialSet.Volume, out float volume))
                SetValue(volume);
        }

        void UserSocialUpdate(Notification notification)
        {
            if (notification.TryGetInfoT(UserSocialNotificationKeys.UserSocialUpdate.Volume, out float volume))
                SetValue(volume);
        }

        void SetValue(float volume)
        {
            _slider.value = volume;
        }
    }
}