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
using System.Linq;
using UnityEngine;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(SettingsDropdownControl))]
    public class GraphicsResolutionSettings : MonoBehaviour
    {
        SettingsDropdownControl dropdownControl;

        Resolution selectedResolutions;
        Resolution[] resolutions;

        void Awake()
        {
            dropdownControl = GetComponent<SettingsDropdownControl>();

            dropdownControl.indexToItem = index => ResolutionToString(resolutions[index]);
            dropdownControl.valueChanged += ValueChanged;
        }

        void Start()
        {

            selectedResolutions = Screen.currentResolution;
            resolutions = Screen.resolutions;

            dropdownControl.selectedIndex = Array.IndexOf(resolutions, selectedResolutions);
            dropdownControl.optionsCount = resolutions.Count();
            dropdownControl.SetOptions();
        }

        string ResolutionToString(Resolution resolution)
        {
            return $"{resolution.width}x{resolution.height}";
        }

        void ValueChanged(int index)
        {
            var resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
}