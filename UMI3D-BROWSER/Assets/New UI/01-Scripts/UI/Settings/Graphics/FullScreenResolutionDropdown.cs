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
using umi3dBrowsers.displayer;
using UnityEngine;
using inetum.unityUtils;

namespace umi3d.browserRuntime.ui.settings.graphics
{
    public class FullScreenResolutionDropdown : MonoBehaviour
    {
        [SerializeField] private GridDropDown dropdown;

        private void Start()
        {
            Select(Screen.currentResolution.ToString());

            List<GridDropDownItemCell> microphones = new();

            var i = 0;
            var indexCurrent = 0;
            Screen.resolutions.ForEach(resolution => {
                GridDropDownItemCell cell = new($"{resolution.width}x{resolution.height}");
                microphones.Add(cell);
                if (resolution.Equals(Screen.currentResolution))
                    indexCurrent = i;
                i++;
            });

            dropdown.Init(microphones, indexCurrent);

            dropdown.OnClick += () => {
                Select(dropdown.GetValue());
            };
        }

        private void Select(string value)
        {
            var resolution = value.Split('x');
            int.TryParse(resolution[0], out var width);
            int.TryParse(resolution[1], out var height);
            Screen.SetResolution(width, height, Screen.fullScreen);
        }
    }
}