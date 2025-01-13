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
using System.Collections.Generic;
using System.IO;
using umi3d.browserRuntime.libraries;
using umi3d.cdk;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace umi3d.browserRuntime.ui.libraries
{
    [RequireComponent(typeof(LocalizeStringEvent))]
    public class LibrariesDiskSizeText : MonoBehaviour
    {
        private LocalizeStringEvent text;

        private void Awake()
        {
            text = GetComponent<LocalizeStringEvent>();
            NotificationHub.Default.Subscribe(this, LibraryNotificationKeys.LibraryDeleted, UpdateText);
        }

        private void OnEnable()
        {
            UpdateText();
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void UpdateText()
        {
            long totalSize = 0;
            foreach (var lib in UMI3DResourcesManager.Libraries)
                foreach (var file in lib.files)
                    totalSize += new FileInfo(file.path).Length;

            UpdateArguments(new (){ { "size", totalSize } });
        }

        private void UpdateArguments(Dictionary<string, System.Object> arguments)
        {
            text.SetEntry("empty"); // To force refresh the text
            text.StringReference.Arguments = new object[] { arguments };
            text.SetEntry("byte");
        }

    }
}