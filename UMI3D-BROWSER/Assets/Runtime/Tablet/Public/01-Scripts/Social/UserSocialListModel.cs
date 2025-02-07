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
        public string SearchString { get; private set; } = "";
        public bool MuteFilter { get; private set; } = false;
        public bool UnmuteFilter { get; private set; } = false;
        public UserSocialSortingMethode SortMethode { get; private set; } = UserSocialSortingMethode.AToZ;

        public Dictionary<UMI3DUser, UserSocialModelContainer> Users { get; private set; } = new ();

        bool IsFilterEnabled => MuteFilter || UnmuteFilter;

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

            ApplyFilters();
            ApplySorting();
        }

        public void SetSearch(string search)
        {
            SearchString = search;
            ApplyFilters();
        }

        public void SetMuteFilter(bool muteFilter)
        {
            MuteFilter = muteFilter;
            ApplyFilters();
        }

        public void SetUnmuteFilter(bool unmuteFilter)
        {
            UnmuteFilter = unmuteFilter;
            ApplyFilters();
        }

        internal void ApplyFilters()
        {
            foreach (var (user, modelContainer) in Users)
            {
                modelContainer.gameObject.SetActive(false);

                // Search
                if (!string.IsNullOrEmpty(SearchString) && !user.login.ToLower().Contains(SearchString.ToLower()))
                    continue;

                // If no filter
                if (!IsFilterEnabled)
                {
                    modelContainer.gameObject.SetActive(true);
                    continue;
                }

                // Mute Filter
                if (MuteFilter && user.microphoneStatus)
                    modelContainer.gameObject.SetActive(true);

                // Unmute Filter
                if (UnmuteFilter && !user.microphoneStatus)
                    modelContainer.gameObject.SetActive(true);
            }
        }

        public void SetSortingMethode(UserSocialSortingMethode sortMethode)
        {
            SortMethode = sortMethode;
            ApplySorting();
        }

        internal void ApplySorting()
        {
            var users = Users.Keys.ToList();

            // Sorting
            switch (SortMethode)
            {
                case UserSocialSortingMethode.AToZ:
                    users.Sort((user0, user1) => string.Compare(user0.login.Trim(), user1.login.Trim()));
                    break;
                case UserSocialSortingMethode.ZToA:
                    users.Sort((user0, user1) => string.Compare(user0.login.Trim(), user1.login.Trim()));
                    users.Reverse();
                    break;
                default:
                    break;
            }

            // Replacing elements
            foreach (var user in users)
                Users[user].gameObject.transform.SetAsLastSibling();
        }
    }
}