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
using System.Collections.Generic;

namespace umi3d.browserEditor.BuildTool
{
    [Serializable]
    public struct View 
    {
        public readonly string name;

        public View(string name)
        {
            this.name = name;
        }

        public static IReadOnlyList<View> allCases => _allCases.Value;
        static Lazy<View[]> _allCases = new(() =>
        {
            return new[] { InfoView, BuildView, ScenesView, FeaturesView, SettingsView };
        });

        public static readonly View InfoView = new View("Info View");
        public static readonly View BuildView = new View("Build View");
        public static readonly View ScenesView = new View("Scenes View");
        public static readonly View FeaturesView = new View("Features View");
        public static readonly View SettingsView = new View("Settings View");
    }
}