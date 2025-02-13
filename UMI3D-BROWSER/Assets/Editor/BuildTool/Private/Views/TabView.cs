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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class TabView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TabView, UxmlTraits> { }

        const string buttonID = "button__";
        const string viewID = "__tab-view";
        Button infoView;
        Button buildView;
        Button scenesView;
        Button featuresView;
        Button SettingsView;

        T Fetch<T>(string name = null, string className = null) where T : VisualElement
        {
            return this.Q<T>(name, className);
        }

        public void Init()
        {
            // Fetching
            infoView = Fetch<Button>(buttonID + "info" + viewID);
            buildView = Fetch<Button>(buttonID + "build" + viewID);
            scenesView = Fetch<Button>(buttonID + "scenes" + viewID);
            featuresView = Fetch<Button>(buttonID + "features" + viewID);
            SettingsView = Fetch<Button>(buttonID + "settings" + viewID);


        }
    }
}