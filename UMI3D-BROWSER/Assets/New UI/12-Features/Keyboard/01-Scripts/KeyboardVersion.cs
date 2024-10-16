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
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardVersion : MonoBehaviour
    {
        [SerializeField] KeyboardLocalisationVersion version;

        void Awake()
        {
            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.ChangeVersion,
                null,
                VersionChanged
            );
        }

        void VersionChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.Version, out string _version))
            {
                UnityEngine.Debug.LogError($"[KeyboardVersion] notification information does not contain {KeyboardNotificationKeys.Info.Version}.");
                return;
            }

            gameObject.SetActive(version.ToString() == _version);
        }

#if UNITY_EDITOR
        static KeyboardLocalisationVersion currentVersion = KeyboardLocalisationVersion.QWERTY;

        [ContextMenu("TestSwitch")]
        void TestSwitch()
        {
            currentVersion = currentVersion == KeyboardLocalisationVersion.QWERTY ? KeyboardLocalisationVersion.AZERTY : KeyboardLocalisationVersion.QWERTY;
            UnityEngine.Debug.Log($"test switch version to {currentVersion}");
            NotificationHub.Default.Notify(
                this,
                KeyboardNotificationKeys.ChangeVersion,
                new()
                {
                    { KeyboardNotificationKeys.Info.Version, currentVersion.ToString() }
                }
            );
        }
#endif
    }
}