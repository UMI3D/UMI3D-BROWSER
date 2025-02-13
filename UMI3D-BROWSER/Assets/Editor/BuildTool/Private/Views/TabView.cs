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
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class TabView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TabView, UxmlTraits> { }

        BuildToolData data;

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
            data = BuildToolData.@default;

            // Fetching
            infoView = Fetch<Button>(buttonID + "info-view" + viewID);
            buildView = Fetch<Button>(buttonID + "build-view" + viewID);
            scenesView = Fetch<Button>(buttonID + "scenes-view" + viewID);
            featuresView = Fetch<Button>(buttonID + "features-view" + viewID);
            SettingsView = Fetch<Button>(buttonID + "settings-view" + viewID);

            infoView.clicked += SelectInfoView;
            buildView.clicked += SelectBuildView;
            scenesView.clicked += SelectScenesView;
            featuresView.clicked += SelectFeaturesView;
            SettingsView.clicked += SelectSettingsView;

            View currentView = data.currentSelectedView;
            if (currentView.Equals(View.InfoView)) { SelectInfoView(); }
            else if (currentView.Equals(View.BuildView)) { SelectBuildView(); }
            else if (currentView.Equals(View.ScenesView)) { SelectScenesView(); }
            else if (currentView.Equals(View.FeaturesView)) { SelectFeaturesView(); }
            else if (currentView.Equals(View.SettingsView)) { SelectSettingsView(); }
            else 
            {
                UnityEngine.Debug.LogError($"[TabView] Error: Unhandled case [{currentView.name}]");
                SelectInfoView(); 
            }
        }

        void SelectInfoView()
        {
            data.currentSelectedView = View.InfoView;
        }

        void SelectBuildView()
        {
            data.currentSelectedView = View.BuildView;
        }

        void SelectScenesView()
        {
            data.currentSelectedView = View.ScenesView;
        }

        void SelectFeaturesView()
        {
            data.currentSelectedView = View.FeaturesView;
        }

        void SelectSettingsView()
        {
            data.currentSelectedView = View.SettingsView;
        }
    }
}