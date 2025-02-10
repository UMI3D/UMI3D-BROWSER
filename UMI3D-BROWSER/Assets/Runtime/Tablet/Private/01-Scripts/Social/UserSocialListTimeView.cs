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
using TMPro;
using umi3d.browserRuntime.ui.tablet.social;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d
{
    [RequireComponent(typeof(TMP_Text))]
    public class UserSocialListTimeView : MonoBehaviour
    {
        TMP_Text _text;
        DateTime _startTime;

        UserSocialListModelContainer _modelContainer;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _modelContainer = GetComponentInParent<UserSocialListModelContainer>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<UserSocialNotificationKeys.UserSocialListSet>(),
                (Callback)UserSocialSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        private void Update()
        {
            if (_startTime == null)
                return;

            var time = (DateTime.Now - _startTime);
            _text.text = $" {time.ToString("hh")}:{time.ToString("mm")}:{time.ToString("ss")}";
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void UserSocialSet(Notification notification)
        {
            if (notification.TryGetInfoT(UserSocialNotificationKeys.UserSocialListSet.Time, out DateTime time))
            {
                _startTime = time;
            }
        }
    }
}