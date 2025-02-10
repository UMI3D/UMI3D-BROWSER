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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using umi3d.browserEditor.BuildTool;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.TestTools;

public class RenderingHelperTests
{
    public class GetAllRenderingAssetsTest
    {
        [Test]
        public void GivenValidPath_WhenGetAllRenderingAssets_ThenReturnsCorrectNumberOfAssets()
        {
            // When
            UniversalRenderPipelineAsset[] assets = RenderingHelper.@default.GetAllRenderingAssets("Assets/URPSettings/BrowserSettings");

            // Then
            // There are 4 rendering assets: custom, low, medium, high.
            Assert.AreEqual(4, assets.Length, "The number of rendering assets should be 4.");
        }

        [Test]
        public void GivenNullPath_WhenGetAllRenderingAssets_ThenReturnsCorrectNumberOfAssets()
        {
            // When
            UniversalRenderPipelineAsset[] assets = RenderingHelper.@default.GetAllRenderingAssets(null);

            // Then
            // There are 4 rendering assets: custom, low, medium, high.
            Assert.AreEqual(4, assets.Length, "The number of rendering assets should be 4.");
        }

        [Test]
        public void GivenEmptyPath_WhenGetAllRenderingAssets_ThenReturnsCorrectNumberOfAssets()
        {
            // When
            UniversalRenderPipelineAsset[] assets = RenderingHelper.@default.GetAllRenderingAssets("");

            // Then
            Assert.AreEqual(4, assets.Length, "The number of rendering assets should be 4.");
        }

        [Test]
        public void GivenInvalidPath_WhenGetAllRenderingAssets_ThenReturnsZeroAssets()
        {
            // When
            UniversalRenderPipelineAsset[] assets = RenderingHelper.@default.GetAllRenderingAssets("Assets/InvalidPath");

            // Then
            Assert.AreEqual(0, assets.Length, "The number of rendering assets should be 0 for an invalid path.");
        }
    }

    public class SetDefaultPipelineRendererDataTests
    {
        [Test]
        public void GivenValidURPAssetAndIndex_WhenSetDefaultPipelineRendererData_ThenSetsRendererIndex()
        {
            // Given
            UniversalRenderPipelineAsset urpAsset = RenderingHelper.@default.GetAllRenderingAssets()[0];

            // When
            RenderingHelper.@default.SetDefaultPipelineRendererData(urpAsset, 0);

            // Then
            FieldInfo rendererIndexField = typeof(UniversalRenderPipelineAsset).GetField(
                "m_DefaultRendererIndex",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            int defaultIndex = (int)rendererIndexField.GetValue(urpAsset);
            Assert.AreEqual(0, defaultIndex, "The default renderer index should be set to the new index.");
        }

        [Test]
        public void GivenNullURPAsset_WhenSetDefaultPipelineRendererData_ThenLogsError()
        {
            // When
            RenderingHelper.@default.SetDefaultPipelineRendererData(null, 1);

            // Then
            LogAssert.Expect(LogType.Error, "[RenderingHelper] Error: urpAsset should not be null.");
        }

        [Test]
        public void GivenURPAssetWithNullRenderers_WhenSetDefaultPipelineRendererData_ThenLogsError()
        {
            // Given
            var urpAsset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
            var renderersField = typeof(UniversalRenderPipelineAsset).GetField("m_Renderers", BindingFlags.NonPublic | BindingFlags.Instance);
            renderersField.SetValue(urpAsset, null);

            // When
            RenderingHelper.@default.SetDefaultPipelineRendererData(urpAsset, 0);

            // Then
            LogAssert.Expect(LogType.Error, "[RenderingHelper] Error: renderers is null or empty.");
        }

        [Test]
        public void GivenURPAssetWithEmptyRenderers_WhenSetDefaultPipelineRendererData_ThenLogsError()
        {
            // Given
            var urpAsset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
            var renderersField = typeof(UniversalRenderPipelineAsset).GetField("m_Renderers", BindingFlags.NonPublic | BindingFlags.Instance);
            renderersField.SetValue(urpAsset, new ScriptableRenderer[] { });

            // When
            RenderingHelper.@default.SetDefaultPipelineRendererData(urpAsset, 1);

            // Then
            LogAssert.Expect(LogType.Error, "[RenderingHelper] Error: renderers is null or empty.");
        }

        [Test]
        public void GivenURPAssetWithInvalidRendererIndex_WhenSetDefaultPipelineRendererData_ThenLogsError()
        {
            // Given
            UniversalRenderPipelineAsset urpAsset = RenderingHelper.@default.GetAllRenderingAssets()[0];

            // When
            RenderingHelper.@default.SetDefaultPipelineRendererData(urpAsset, 2);

            // Then
            LogAssert.Expect(LogType.Error, "[RenderingHelper] Error: new index [2] is out of range (count: [2]).");
        }

        [Test]
        public void GivenURPAssetWithNullRendererAtIndex_WhenSetDefaultPipelineRendererData_ThenLogsError()
        {
            // Given
            var urpAsset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
            var renderersField = typeof(UniversalRenderPipelineAsset).GetField("m_Renderers", BindingFlags.NonPublic | BindingFlags.Instance);
            renderersField.SetValue(urpAsset, new ScriptableRenderer[] { null });

            // When
            RenderingHelper.@default.SetDefaultPipelineRendererData(urpAsset, 0);

            // Then
            LogAssert.Expect(LogType.Error, "[RenderingHelper] Error: Renderer at index [0] is null.");
        }
    }
}