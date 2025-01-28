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

namespace umi3d.browserRuntime.ui.tablet.userNotification
{
    [RequireComponent(typeof(Image))]
    public class UserNotificationBadgeView : MonoBehaviour
    {
        [SerializeField] Color _colorNew;
        [SerializeField] Color _colorRead;

         Image _image;
        UserNotificationModelContainer _modelContainer;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _modelContainer = GetComponentInParent<UserNotificationModelContainer>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<UserNotificationNotificationKeys.UserNotificationSet>(),
                (Callback)UserNotifiactionSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));

            NotificationHub.Default.Subscribe(this,
                ID.FromType<UserNotificationNotificationKeys.UserNotificationUpdate>(),
                (Callback)UserNotifiactionUpdated,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        private void OnDisable()
        {
            Debug.Log("TESSST");
            _modelContainer.Model.Seen();
        }

        void UserNotifiactionSet(Notification notification)
        {
            if (notification.TryGetInfoT(UserNotificationNotificationKeys.UserNotificationSet.IsSeen, out bool isSeen))
            {
                _image.color = isSeen ? _colorRead : _colorNew;
            }
        }

        void UserNotifiactionUpdated(Notification notification)
        {
            if (notification.TryGetInfoT(UserNotificationNotificationKeys.UserNotificationUpdate.IsSeen, out bool isSeen))
            {
                _image.color = isSeen ? _colorRead : _colorNew;
            }
        }
    }
}