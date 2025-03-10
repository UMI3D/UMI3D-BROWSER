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
    public class UserSocialOtherActionDropdown : MonoBehaviour
    {
        [SerializeField] Button _arrow;
        [SerializeField] Transform _contentainer;
        [SerializeField] Transform _content;
        [SerializeField] UserSocialOtherActionModelContainer _otherActionPrefab;

        Queue<UserSocialOtherActionModelContainer> _objectAvailable = new Queue<UserSocialOtherActionModelContainer>();
        List<UserSocialOtherActionModelContainer> _objects = new List<UserSocialOtherActionModelContainer>();

        UserSocialModelContainer _modelContainer;

        private void Awake()
        {
            _modelContainer = GetComponentInParent<UserSocialModelContainer>();

            _arrow.onClick.AddListener(() => _contentainer.gameObject.SetActive(!_contentainer.gameObject.activeSelf));

            NotificationHub.Default.Subscribe(this,
                ID.FromType<UserSocialNotificationKeys.UserSocialSet>(),
                (Callback)UserSocialSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.Model));
        }

        private void OnDestroy()
        {
            _arrow.onClick.RemoveAllListeners();
            NotificationHub.Default.Unsubscribe(this);
        }

        void UserSocialSet(Notification notification)
        {
            if (!notification.TryGetInfoT(UserSocialNotificationKeys.UserSocialSet.OtherActions, out List<UserAction> actions))
                return;

            // Register object to queue
            foreach (var o in _objects) 
            {
                o.gameObject.SetActive(false);
                _objectAvailable.Enqueue(o);
                _objects.Remove(o);
            }

            // Display only if thers's actions
            gameObject.SetActive(actions.Count > 0);

            // If no action don't continue
            if (actions.Count == 0)
                return;

            // Get or create new actions
            foreach (var userAction in actions)
            {
                if (!_objectAvailable.TryDequeue(out var otherActionModelContainer))
                    otherActionModelContainer = Instantiate(_otherActionPrefab, _content);

                otherActionModelContainer.Model.SetUserAction(userAction);
                otherActionModelContainer.gameObject.SetActive(true);
                _objects.Add(otherActionModelContainer);
            }
        }
    }
}