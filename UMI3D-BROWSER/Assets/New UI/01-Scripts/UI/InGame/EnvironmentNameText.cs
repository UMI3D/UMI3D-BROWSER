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

using inetum.unityUtils.observation;
using TMPro;
using umi3d.browserRuntime.forms;
using umi3d.cdk.collaboration;
using umi3dBrowsers.linker;
using UnityEngine;

namespace umi3d.browserRuntime.ui.inGame
{
    [RequireComponent(typeof(TMP_Text))]
    public class EnvironmentNameText : MonoBehaviour
    {
        [SerializeField] private ConnectionToImmersiveLinker connectionToImmersiveLinker;
        private TMP_Text text;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<FormNotificationKeys.Cancel>(),
                (Callback)OnEnvironmentLeaved);
            connectionToImmersiveLinker.OnLeave += OnEnvironmentLeaved;
            UMI3DEnvironmentClient.EnvironmentJoined.AddListener(OnEnvironmentJoined);
        }

        private void OnDestroy()
        {
            connectionToImmersiveLinker.OnLeave -= OnEnvironmentLeaved;
            UMI3DEnvironmentClient.EnvironmentJoined.RemoveListener(OnEnvironmentJoined);
        }

        private void OnEnvironmentJoined()
        {
            text.text = UMI3DCollaborationClientServer.Environement?.name ?? "";
        }

        private void OnEnvironmentLeaved()
        {
            text.text = "";
        }
    }
}