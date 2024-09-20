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
    public class KeyboardVersionTest : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Dropdown dropdown;

        Notifier versionNotifier;

        void Start()
        {
            versionNotifier = NotificationHub.Default.GetNotifier(
                this,
                KeyboardNotificationKeys.ChangeVersion
            );

            int index = dropdown != null ? dropdown.value : 0;

            versionNotifier[KeyboardNotificationKeys.Info.Version] = index == 0 ? "AZERTY" : "QWERTY";
            versionNotifier.Notify();

        }

        void OnEnable()
        {
            dropdown?.onValueChanged.AddListener(ValueChanged);
        }

        void OnDisable()
        {
            dropdown?.onValueChanged.RemoveListener(ValueChanged);
        }

        public void ValueChanged(int index)
        {
            versionNotifier[KeyboardNotificationKeys.Info.Version] = index == 0 ? "AZERTY" : "QWERTY";
            versionNotifier.Notify();
        }

#if UNITY_EDITOR
        [ContextMenu("Test Switch To Azerty")]
        void TestSwitchToAzerty()
        {
            UnityEngine.Debug.Log($"test switch to Azerty");
            versionNotifier[KeyboardNotificationKeys.Info.Version] = "AZERTY";
            versionNotifier.Notify();
        }

        [ContextMenu("Test Switch To Qwerty")]
        void TestSwitchToQwerty()
        {
            UnityEngine.Debug.Log($"test switch to Qwerty");
            versionNotifier[KeyboardNotificationKeys.Info.Version] = "QWERTY";
            versionNotifier.Notify();
        }
#endif
    }
}