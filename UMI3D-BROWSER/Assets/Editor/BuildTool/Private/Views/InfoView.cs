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
using umi3d.common.core.target;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class InfoView : VisualElement
    {
        public class ListEntry : Label
        {
            public ListEntry(string name) : base(name)
            {
                
            }
        }

        public new class UxmlFactory : UxmlFactory<InfoView, UxmlTraits> { }

        BuildToolData data;

        const string visualElementID = "visual-element__";
        const string dropdownID = "dropdown__";
        const string labelID = "label__";

        DropdownField platform;
        DropdownField releaseCycle;

        Label operatingSystem;
        VisualElement scenes;
        VisualElement features;
        Label browserVersion;
        Label sdkVersion;

        T Fetch<T>(string name = null, string className = null) where T: VisualElement
        {
            return this.Q<T>(name, className);
        }

        public void Init()
        {
            data = BuildToolData.@default;

            // Fetching.
            platform = Fetch<DropdownField>(dropdownID + "platform");
            releaseCycle = Fetch<DropdownField>(dropdownID + "release-cycle");
            operatingSystem = Fetch<Label>(labelID + "operating-system");
            scenes = Fetch<VisualElement>(visualElementID + "scenes");
            features = Fetch<VisualElement>(visualElementID + "features");
            browserVersion = Fetch<Label>(labelID + "browser-version");
            sdkVersion = Fetch<Label>(labelID + "sdk-version");

            // Initialization.
            platform.choices = Platform.allCases.Select(platform => platform.name).ToList();
            SetPlatform();
            platform.RegisterValueChangedCallback(value =>
            {
                data.currentPlatform = Platform.allCases.Where(platform => platform.name == value.newValue).First();
            });

            releaseCycle.choices = ReleaseCycle.allCases.Select(releaseCycle => releaseCycle.name).ToList();
            SetReleaseCycle();
            releaseCycle.RegisterValueChangedCallback(value =>
            {
                data.currentReleaseCycle = ReleaseCycle.allCases.Where(releaseCycle => releaseCycle.name == value.newValue).First();
            });

            SetOperatingSystem();

            SetScenes();

            SetFeatures();

            SetBrowserVersion();
            SetSDKVersion();
        }

        public void SetPlatform()
        {
            platform.SetValueWithoutNotify(data.currentPlatform.name);
        }

        public void SetReleaseCycle()
        {
            releaseCycle.SetValueWithoutNotify(data.currentReleaseCycle.name);
        }

        public void SetOperatingSystem()
        {
            operatingSystem.text = data.GetOperatingSystemFrom(data.currentPlatform).name;
        }

        public void SetScenes()
        {
            for (int i = 1; i < scenes.childCount; i++)
            {
                scenes.RemoveAt(i);
            }
            foreach (var scene in data.GetScenesFor(data.currentPlatform, data.currentReleaseCycle))
            {
                ListEntry sceneEntry = new ListEntry($"- {scene.path}");
                scenes.Add(sceneEntry);
            }
        }

        public void SetFeatures()
        {
            // TODO: Features.
        }

        public void SetBrowserVersion()
        {
            browserVersion.text = data.currentBrowserVersion.ToString();
        }

        public void SetSDKVersion()
        {
            sdkVersion.text = data.currentSDKVersion.ToString();
        }
    }
}