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

using System;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk.collaboration;

namespace umi3d.browserRuntime.ui.tablet.social
{
    public class UserSocialListModel 
    {
        public string SearchString { get; private set; }

        public Dictionary<UMI3DUser, UserSocialModelContainer> Users { get; private set; } = new ();

        public Action<UMI3DUser> AddUser;
        public Action<UMI3DUser> RemoveUser;

        public UserSocialListModel()
        {
            UMI3DEnvironmentClient.EnvironmentLoaded.AddListener(UpdateList);
            UMI3DCollaborationEnvironmentLoader.Instance.OnUpdateJoinedUserList += UpdateList;
        }

        ~UserSocialListModel()
        {
            UMI3DEnvironmentClient.EnvironmentLoaded.RemoveListener(UpdateList);
            UMI3DCollaborationEnvironmentLoader.Instance.OnUpdateJoinedUserList -= UpdateList;
        }

        internal void UpdateList()
        {
            var users = UMI3DCollaborationEnvironmentLoader.Instance
                .JoinedUserList
                .Where(u => !u.isClient).ToList();

            // Remove Users
            foreach (var user in Users.Keys)
                if (!users.Contains(user))
                    RemoveUser?.Invoke(user);

            // Add Users
            foreach (var user in users)
                if (!Users.ContainsKey(user))
                    AddUser?.Invoke(user);
        }

        public void SetSearch(string search)
        {
            SearchString = search;
            ApplyFilters();
        }

        internal void ApplyFilters()
        {
            foreach (var (user, modelContainer) in Users)
            {
                modelContainer.gameObject.SetActive(true);
                if (!string.IsNullOrEmpty(SearchString) && !user.login.ToLower().Contains(SearchString.ToLower()))
                    modelContainer.gameObject.SetActive(false);
            }
        }
    }
}