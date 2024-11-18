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
using System.IO;
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
        }

        private void OnEnable()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            long totalSize = 0;
            foreach (var lib in UMI3DResourcesManager.Libraries)
                foreach (var file in lib.files)
                    totalSize += new FileInfo(file.path).Length;

            UpdateArguments(new (){ { "size", totalSize } });
        }
        void UpdateArguments(Dictionary<string, System.Object> arguments)
        {
            text.StringReference.Arguments = new object[] { arguments };
            text.SetEntry("byte");
        }

    }
}