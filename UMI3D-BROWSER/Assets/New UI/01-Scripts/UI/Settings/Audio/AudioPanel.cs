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
using UnityEngine;

namespace umi3d.browserRuntime.ui.settings.audio
{
    public class AudioPanel : MonoBehaviour
    {
        [SerializeField] private GameObject[] deactivateOnVr;

        private void Awake()
        {
#if UMI3D_XR
            foreach (GameObject go in deactivateOnVr)
                go.SetActive(false);
#endif

            NotificationHub.Default.Subscribe(this, SettingsNotificationKeys.CloseAll, Close);
            NotificationHub.Default.Subscribe(this, SettingsNotificationKeys.OpenAudio, Open);
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void Open()
        {
            gameObject.SetActive(true);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }
    }
}