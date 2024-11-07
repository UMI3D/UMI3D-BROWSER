/*
Copyright 2019 - 2024 Inetum

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

using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using umi3d.browserRuntime.ui.elements.dropdown;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.inGame.tablet.social
{
    public class SocialScreen : MonoBehaviour
    {
        [SerializeField] Transform content;
        [SerializeField] GameObject socialPrefab;
        [SerializeField] RectTransform userActionContainer;
        [SerializeField] TMP_InputField searchField;
        [SerializeField] TMP_Text numberOfParticipantText;
        [SerializeField] TMP_Text timeSpentText;
        [SerializeField] TMP_Dropdown sortByDropdown;
        [SerializeField] ToggleDropdown filterDropdown;

        List<UMI3DUser> users = new();
        List<UMI3DUser> filteredUser = new();

        List<SocialElement> activatedElements = new();
        List<SocialElement> deactivatedElements = new();
        Dictionary<ulong, SocialElement> _allUsersRemembered = new();

        DateTime _startTime;

        ToggleDropdownItem MuteFilter;
        ToggleDropdownItem UnMuteFilter;

        ToggleGroup toggleGroup;

        void Awake()
        {
            NotificationHub.Default.Subscribe(this, TabletNotificationKeys.OpenSocial, Open);
            NotificationHub.Default.Subscribe(this, TabletNotificationKeys.CloseScreens, Close);

            // Reset
            _Reset();
            UMI3DEnvironmentClient.EnvironmentLoaded.AddListener(_Reset);
            UMI3DCollaborationEnvironmentLoader.Instance.OnUpdateJoinedUserList += ResetLists;

            //UMI3DUser.OnUserMicrophoneStatusUpdated.AddListener(UpdateUserList);

            sortByDropdown.options.Add(new TMP_Dropdown.OptionData("A to Z"));
            sortByDropdown.options.Add(new TMP_Dropdown.OptionData("Z to A"));
            sortByDropdown.value = 0;

            searchField.onValueChanged.AddListener(SearchValueChanged);
            sortByDropdown.onValueChanged.AddListener(SortAZValueChanged);

            toggleGroup = gameObject.GetOrAddComponent<ToggleGroup>();
            toggleGroup.allowSwitchOff = true;
        }

        void Start()
        {
            MuteFilter = filterDropdown.AddOption("Mute");
            UnMuteFilter = filterDropdown.AddOption("UnMute");
            MuteFilter.OnToggle += FilterMuteUserValueChanged;
            UnMuteFilter.OnToggle += FilterUnmuteUserValueChanged;

            userActionContainer.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void Update()
        {
            if (!enabled)
                return;

            var time = DateTime.Now - _startTime;
            timeSpentText.text = $" {time.ToString("hh")}:{time.ToString("mm")}:{time.ToString("ss")}";
        }

        void _Reset()
        {
            _startTime = DateTime.Now;
            ResetLists();
        }

        void ResetLists()
        {
            users.Clear();
            users = UMI3DCollaborationEnvironmentLoader
                .Instance.JoinedUserList
                .Where(u => !u.isClient).ToList();

            UpdateUserList();
        }

        void UpdateUserList()
        {
            ApplyFilterAndSearch();
            numberOfParticipantText.text = $" {users.Count + 1}";
        }

        void ApplyFilterAndSearch()
        {
            filteredUser.Clear();
            ClearSocialList();

            for (int i = 0; i < users.Count; i++)
            {
                UMI3DUser user = users[i];

                if (!IsIncludeBySearch(user))
                {
                    continue;
                }

                bool isInclude = true;
                if (FilterEnabled)
                {
                    isInclude = false;
                    if (IsIncludeByMuteFilter(user))
                    {
                        isInclude = true;
                    }
                    if (IsIncludeByUnmuteFilter(user))
                    {
                        isInclude = true;
                    }
                }

                if (isInclude)
                {
                    filteredUser.Add(user);
                }
            }

            SortAZ();

            CreateSocialList();

            UpdateHierarchy();
        }

        bool FilterEnabled => mute || unMute;

        bool mute = false;
        void FilterMuteUserValueChanged(bool mute)
        {
            this.mute = mute;
            ApplyFilterAndSearch();
        }
        bool IsIncludeByMuteFilter(UMI3DUser user)
        {
            return !mute || user.microphoneStatus;
        }

        bool unMute = false;
        void FilterUnmuteUserValueChanged(bool unMute)
        {
            this.unMute = unMute;
            ApplyFilterAndSearch();
        }
        bool IsIncludeByUnmuteFilter(UMI3DUser user)
        {
            return !unMute || !user.microphoneStatus;
        }

        string search = null;
        void SearchValueChanged(string name)
        {
            search = name;
            ApplyFilterAndSearch();
        }
        bool IsIncludeBySearch(UMI3DUser user)
        {
            return string.IsNullOrEmpty(search) || user.login.ToLower().Contains(search.ToLower());
        }

        int sortAZIndex = 0;
        void SortAZValueChanged(int value)
        {
            sortAZIndex = value;
            SortAZ();

            UpdateHierarchy();
        }
        void SortAZ()
        {
            users.Sort((user0, user1) => string.Compare(user0.login, user1.login));
            if (sortAZIndex == 1)
            {
                users.Reverse();
            }
        }

        void CreateSocialList()
        {
            foreach (UMI3DUser user in filteredUser)
            {
                SocialElement socialElement;
                if (deactivatedElements.Count > 0)
                {
                    socialElement = deactivatedElements[deactivatedElements.Count - 1];
                    deactivatedElements.RemoveAt(deactivatedElements.Count - 1);
                    socialElement.gameObject.SetActive(true);
                }
                else
                {
                    socialElement = CreateSocialElement();
                }
                activatedElements.Add(socialElement);
                SetSocialElement(socialElement, user);
            }
        }

        void ClearSocialList()
        {
            for (int i = activatedElements.Count - 1; i >= 0; i--)
            {
                SocialElement socialElement = activatedElements[i];
                activatedElements.RemoveAt(i);
                deactivatedElements.Add(socialElement);
                socialElement.gameObject.SetActive(false);
            }
        }

        void UpdateHierarchy()
        {
            foreach (SocialElement u in activatedElements)
            {
                u.transform.SetAsLastSibling();
            }
        }

        SocialElement CreateSocialElement()
        {
            UnityEngine.Debug.Log($"CreateSocialElement");
            GameObject socialElementGO = Instantiate(socialPrefab);
            SocialElement socialElement = socialElementGO.GetComponent<SocialElement>();
            socialElementGO.transform.SetParent(content.transform, false);
            return socialElement;
        }

        void SetSocialElement(SocialElement socialElement, UMI3DUser user)
        {
            socialElement.User = user;
            if (_allUsersRemembered.TryGetValue(user.id, out SocialElement elt))
            {
                if (elt == null)
                {
                    UnityEngine.Debug.LogError($"[UMI3D Browser] Social: null ref SocialElement.");
                }
                socialElement.UserVolume = elt?.UserVolume ?? 100f;
                socialElement.IsMute = elt?.IsMute ?? false;
                _allUsersRemembered[user.id] = socialElement;
            }
            else
            {
                socialElement.UserVolume = 100f;
                socialElement.IsMute = false;
                _allUsersRemembered.Add(user.id, socialElement);
            }

            socialElement.ToggleGroup = toggleGroup;
            socialElement.nonPrimaryActionContainer = userActionContainer;
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        [ContextMenu("Add Test User")]
        void AddTestUser()
        {
            ulong userID = UMI3DCollaborationClientServer.Instance.GetUserId() + 1;
            common.collaboration.dto.signaling.UserDto dto = new() { id = userID, login = "Test User" };

            UMI3DUser testUser = new UMI3DUser(0, dto);

            users.Add(testUser);

            UpdateUserList();
        }
#endif
    }
}