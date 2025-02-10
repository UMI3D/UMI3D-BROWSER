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

using System.Collections.Generic;
using System.Linq;
using umi3d.cdk.collaboration;
using umi3d.common.collaboration.dto.signaling;
using UnityEngine;

namespace umi3d.browserRuntime.ui.tablet.social
{
    [RequireComponent(typeof(UserSocialListModelContainer))]
    public class UserSocialFactory : MonoBehaviour
    {
        [SerializeField] Transform _content;
        [SerializeField] UserSocialModelContainer _userSocialPrefab;

        Queue<UserSocialModelContainer> _availablePrefabs = new();

        UserSocialListModelContainer _listModelContainer;

        private void Awake()
        {
            _listModelContainer = GetComponent<UserSocialListModelContainer>();

            _listModelContainer.Model.AddUser += CreateUser;
            _listModelContainer.Model.RemoveUser += RemoveUser;
        }

        private void OnDestroy()
        {
            _listModelContainer.Model.AddUser -= CreateUser;
            _listModelContainer.Model.RemoveUser -= RemoveUser;
        }

        internal void CreateUser(UMI3DUser user)
        {
            if (!_availablePrefabs.TryDequeue(out var modelContainer))
                modelContainer = Instantiate(_userSocialPrefab, _content);

            modelContainer.gameObject.SetActive(true);
            modelContainer.Model.SetUser(user);

            _listModelContainer.Model.Users.Add(user, modelContainer);
        }

        internal void RemoveUser(UMI3DUser user)
        {
            var modelContainer = _listModelContainer.Model.Users[user];
            modelContainer.gameObject.SetActive(false);
            _availablePrefabs.Enqueue(modelContainer);

            _listModelContainer.Model.Users.Remove(user);
        }

#if UNITY_EDITOR
        [ContextMenu("Add Test User")]
        void AddTestUser()
        {
            ulong userID = UMI3DCollaborationClientServer.Instance.GetUserId() + 1 + (ulong)(1 * _listModelContainer.Model.Users.Count);
            UserDto dto = new() { id = userID, login = $"Test User {1 * _listModelContainer.Model.Users.Count}" };

            UMI3DUser testUser = new UMI3DUser(0, dto);
            testUser.userActions.Add(new UserAction(0, new UserActionDto() {
                name = "Primary",
                description = "Test",
                isPrimary = true
            }));
            testUser.userActions.Add(new UserAction(0, new UserActionDto() {
                name = "Other",
                description = "Test",
                isPrimary = false
            }));
            testUser.userActions.Add(new UserAction(0, new UserActionDto() {
                name = "Primary",
                description = "Test",
                isPrimary = true
            }));
            testUser.userActions.Add(new UserAction(0, new UserActionDto() {
                name = "Primary",
                description = "Test",
                isPrimary = true
            }));
            testUser.userActions.Add(new UserAction(0, new UserActionDto() {
                name = "Primary",
                description = "Test",
                isPrimary = true
            }));
            CreateUser(testUser);
        }

        [ContextMenu("Remove Test User")]
        void RemoveTestUser()
        {
            RemoveUser(_listModelContainer.Model.Users.Keys.Last());
        }
#endif
    }
}