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

using UnityEditor;

namespace umi3d.browserEditor.BuildTool
{
    public interface ITargetDelegate 
    {
        /// <summary>
        /// This method is called when the defined symbols have changed for the given target.
        /// </summary>
        /// <param name="oldSymbols"></param>
        /// <param name="newSymbols"></param>
        void SymbolsHaveChanged(string[] oldSymbols, string[] newSymbols, UnityEditor.Build.NamedBuildTarget target) {}

        /// <summary>
        /// This method is called when the build target has changed.
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        /// <param name="buildTarget"></param>
        void BuildTargetHasChanged(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget) {}

        /// <summary>
        /// This method is called if the process of changing the build target has failed.
        /// </summary>
        void BuildTargetFailedToChange() {}
    }
}