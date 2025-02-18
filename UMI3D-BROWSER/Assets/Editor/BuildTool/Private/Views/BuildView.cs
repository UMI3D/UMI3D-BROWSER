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
using umi3d.common.core.target;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class BuildView : VisualElement
    {
        const string visualElementID = "visual-element__";
        const string dropdownID = "dropdown__";
        const string labelID = "label__";

        public new class UxmlFactory : UxmlFactory<BuildView, UxmlTraits> { }

        BuildToolData data;

        DropdownField immersiveType;
        DropdownField operatingSystem;
        DropdownField releaseCycle;

        T Fetch<T>(string name = null, string className = null) where T : VisualElement
        {
            return this.Q<T>(name, className);
        }

        public void Init()
        {
            data = BuildToolData.@default;

            // Fetching.
            immersiveType = Fetch<DropdownField>(dropdownID + "immersive-typ");
            operatingSystem = Fetch<DropdownField>(dropdownID + "operating-system");
            releaseCycle = Fetch<DropdownField>(dropdownID + "release-cycle");

            // Initialization.
            immersiveType.choices = new() { "Non-immersive", "immersive" };
            //immersiveType.SetValueWithoutNotify(data.im)
        }
    }
}