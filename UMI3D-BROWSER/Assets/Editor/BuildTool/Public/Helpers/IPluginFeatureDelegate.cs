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

using umi3d.common.core.target;

namespace umi3d.browserEditor.BuildTool
{
    public interface IPluginFeatureDelegate 
    {
        /// <summary>
        /// This method is called when a plugin has been enabled.
        /// </summary>
        /// <param name="plugin"></param>
        void PluginHasBeenEnabled(Plugin plugin) {}

        /// <summary>
        /// This method is called when a plugin has been disabled.
        /// </summary>
        /// <param name="plugin"></param>
        void PluginHasBeenDisabled(Plugin plugin) {}

        /// <summary>
        /// This method is called if the process of setting a plugin has failed.
        /// </summary>
        /// <param name="plugin"></param>
        void SettingPluginRaisedError(Plugin plugin) {}

        /// <summary>
        /// This method is called when a feature has been enabled.
        /// </summary>
        /// <param name="feature"></param>
        void FeatureHasBeenEnabled(Feature feature) {}

        /// <summary>
        /// This method is called when a feature has been disabled.
        /// </summary>
        /// <param name="feature"></param>
        void FeatureHasBeenDisabled(Feature feature) {}

        /// <summary>
        /// This method is called if the process of setting a feature has failed.
        /// </summary>
        /// <param name="feature"></param>
        void SettingFeatureFailed(Feature feature) {}
    }
}