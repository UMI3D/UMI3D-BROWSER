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
using System.Reflection;
using UnityEditor;
using UnityEngine.Rendering.Universal;

namespace umi3d.browserEditor.BuildTool
{
    public class RenderingHelper 
    {
        /// <summary>
        /// Path of the folder that contains rendering settings.
        /// </summary>
        public const string FOLDER_PATH = "Assets/URPSettings/BrowserSettings";

        /// <summary>
        /// The default instance of <see cref="RenderingHelper"/>.
        /// </summary>
        public static RenderingHelper @default => _default.Value;
        static readonly Lazy<RenderingHelper> _default = new(() => new());
        RenderingHelper() { }

        public UniversalRenderPipelineAsset[] GetAllRenderingAssets(string path = null)
        {
            if (string.IsNullOrEmpty(path)) {  path = FOLDER_PATH; }

            string[] GUIDs = AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset", new[] { path });
            return GUIDs.Select(guid =>
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                return AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
            }).ToArray();
        }

        public void SetDefaultPipelineRendererData(UniversalRenderPipelineAsset urpAsset, int newRendererIndex)
        {
            if (urpAsset == null)
            {
                UnityEngine.Debug.LogError($"[RenderingHelper] Error: urpAsset should not be null.");
                return;
            }

            FieldInfo renderersField = typeof(UniversalRenderPipelineAsset).GetField(
                "m_RendererDataList",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            ScriptableRendererData[] renderers = renderersField.GetValue(urpAsset) as ScriptableRendererData[];

            if (renderers == null || renderers.Length == 0)
            {
                UnityEngine.Debug.LogError($"[RenderingHelper] Error: renderers is null or empty on asset [{urpAsset.name}].");
                return;
            }

            if (newRendererIndex < 0 || renderers.Length <= newRendererIndex)
            {
                UnityEngine.Debug.LogError($"[RenderingHelper] Error: new index [{newRendererIndex}] is out of range (count: [{renderers.Length}]) on asset [{urpAsset.name}].");
                return;
            }

            if (renderers[newRendererIndex] == null)
            {
                UnityEngine.Debug.LogError($"[RenderingHelper] Error: Renderer at index [{newRendererIndex}] is null on asset [{urpAsset.name}].");
                return;
            }

            FieldInfo rendererIndexField = typeof(UniversalRenderPipelineAsset).GetField(
                "m_DefaultRendererIndex",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            rendererIndexField.SetValue(urpAsset, newRendererIndex);
        }
    }
}