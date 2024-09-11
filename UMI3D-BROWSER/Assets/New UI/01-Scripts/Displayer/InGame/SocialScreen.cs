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
using umi3d.cdk.collaboration;
using UnityEngine;

public class SocialScreen : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject socialPrefab;

    private List<SocialElement> _users;

    private void Awake()
    {
        Clear();
        UMI3DEnvironmentClient.EnvironementJoinned.AddListener(Clear);
        UMI3DUser.OnUserMicrophoneStatusUpdated.AddListener(UpdateUserList);
        UMI3DUser.OnUserAvatarStatusUpdated.AddListener(UpdateUserList);
        UMI3DUser.OnUserAttentionStatusUpdated.AddListener(UpdateUserList);
        UMI3DUser.OnRemoveUser.AddListener(Remove);
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

    private SocialElement CreateUser(UMI3DUser u)
    {
        return Instantiate(socialPrefab, content).GetComponent<SocialElement>();
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
