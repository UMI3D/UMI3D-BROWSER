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

using System.Collections.Generic;
using System.Linq;
using TMPro;
using umi3d.cdk.collaboration;
using UnityEngine;

public class SocialScreen : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject socialPrefab;
    [SerializeField] private TMP_InputField searchField;

    private List<SocialElement> _users;
    private Dictionary<ulong, SocialElement> _allUsersRemembered;

    private void Awake()
    {
        Clear();

        UMI3DEnvironmentClient.EnvironementJoinned.AddListener(Clear);
        UMI3DUser.OnUserMicrophoneStatusUpdated.AddListener(UpdateUserList);
        UMI3DUser.OnUserAvatarStatusUpdated.AddListener(UpdateUserList);
        UMI3DUser.OnUserAttentionStatusUpdated.AddListener(UpdateUserList);
        UMI3DUser.OnRemoveUser.AddListener(Remove);

        searchField.onValueChanged.AddListener(Search);
    }

    private void Clear()
    {
        if (_users != null)
            foreach (var u in _users)
                Destroy(u.gameObject);

        _users = new List<SocialElement>();
    }

    private void UpdateUserList(UMI3DUser user)
    {
        _users = UMI3DCollaborationEnvironmentLoader.Instance.JoinnedUserList
            .Where(u => !u.isClient)
            .Select(CreateUser)
            .ToList();

        Filter();
    }

    private void Filter()
    {
        _users.Sort((user0, user1) => string.Compare(user0.UserName, user1.UserName));
        UpdateHierarchy();
    }

    private void UpdateHierarchy()
    {
        foreach (var u in _users)
            u.transform.SetAsLastSibling();
    }

    private void Search(string name)
    {
        if (name == null || name == string.Empty)
        {
            foreach (var user in _users)
                user.gameObject.SetActive(true);
            return;
        }

        foreach (var user in _users)
            user.gameObject.SetActive(user.UserName.Contains(name));
    }

    private SocialElement CreateUser(UMI3DUser user)
    {
        var socialElement = Instantiate(socialPrefab, content).GetComponent<SocialElement>();
        socialElement.User = user;
        if (_allUsersRemembered.ContainsKey(user.id))
            socialElement.UserVolume = _allUsersRemembered[user.id].UserVolume;
        else
        {
            socialElement.UserVolume = 100f;
            _allUsersRemembered.Add(user.id, socialElement);
        }

        return socialElement;
    }

    private void Remove(UMI3DUser user)
    {
        if (_users == null)
            return;

        var userToRemove = _users.FirstOrDefault(u => {  return u?.User == user; });
        _users.Remove(userToRemove);
        Destroy(userToRemove.gameObject);
    }
}
