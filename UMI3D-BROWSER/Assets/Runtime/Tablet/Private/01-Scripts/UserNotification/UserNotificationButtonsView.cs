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
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.tablet.userNotification
{

    public class UserNotificationButtonsView : MonoBehaviour
    {
        [SerializeField] Button _buttonPrefab;
        [SerializeField] int _buttonPadding;

        UserNotificationModelContainer _modelContainer;

        private void Awake()
        {
            _modelContainer = GetComponentInParent<UserNotificationModelContainer>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<UserNotificationNotificationKeys.UserNotificationSet>(),
                (Callback)UserNotifiactionSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        void UserNotifiactionSet(Notification notification)
        {
            if (notification.TryGetInfoT(UserNotificationNotificationKeys.UserNotificationSet.Buttons, out List<Tuple<string, Action>> buttons))
            {
                foreach (var button in buttons)
                {
                    var buttonObject = Instantiate(_buttonPrefab, transform);
                    buttonObject.onClick.AddListener(() => button.Item2?.Invoke());
                    buttonObject.GetComponent<TMP_Text>().text = button.Item1;
                }
            }
        }
    }
}