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

namespace umi3d.browserRuntime.ui.tablet
{
    [RequireComponent(typeof(Button))]
    public class UserSocialOpenButton : MonoBehaviour
    {
        private Button _button;

        private Notifier _openSocialNotifier;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _openSocialNotifier = NotificationHub.Default.GetNotifier(this, ID.FromType<TabletNotificationKeys.TabletUpdate>());
            _openSocialNotifier[TabletNotificationKeys.TabletUpdate.Menu] = TabletMenu.Social;

            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            NotificationHub.Default.Notify(this, ID.FromType<TabletNotificationKeys.Open>());
            _openSocialNotifier.Notify();
        }
    }
}