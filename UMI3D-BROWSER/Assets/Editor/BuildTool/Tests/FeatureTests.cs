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
using NUnit.Framework;
using umi3d.browserEditor.BuildTool;
using static umi3d.browserEditor.BuildTool.FeatureExt;

public class FeatureTests
{
    public class GetFeaturesTest
    {
        [Test]
        public void GivenMetaFeature_WhenGetFeatures_ThenReturnsMetaFeatures()
        {
            // Given
            E_Feature feature = E_Feature.Meta;

            // When
            string[] result = feature.GetFeatures();

            // Then
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Length);
            Assert.Contains(FEATURE_META_QUEST, result);
            Assert.Contains(INPUT_METAQUEST_PRO, result);
            Assert.Contains(INPUT_OCULUS_TOUCH, result);
        }

        [Test]
        public void GivenPicoFeature_WhenGetFeatures_ThenReturnsPicoFeatures()
        {
            // Given
            E_Feature feature = E_Feature.Pico;

            // When
            string[] result = feature.GetFeatures();

            // Then
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Length);
            Assert.Contains(FEATURE_PICO_OPENXR, result);
            Assert.Contains(FEATURE_PICO_SUPPORT, result);
            Assert.Contains(INPUT_PICO4_TOUCH, result);
            Assert.Contains(INPUT_PICONeo3_TOUCH, result);
        }

        [Test]
        public void GivenViveFeature_WhenGetFeatures_ThenReturnsViveFeatures()
        {
            // Given
            E_Feature feature = E_Feature.Vive;

            // When
            string[] result = feature.GetFeatures();

            // Then
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.Contains(FEATURE_VIVE_SUPPORT, result);
            Assert.Contains(INPUT_VIVEFocus3, result);
        }
    }

    public class GetAllFeaturesExceptTest
    {
        [Test]
        public void GivenMetaFeature_WhenGetAllFeaturesExcept_ThenReturnsAllExceptMeta()
        {
            // Given
            E_Feature feature = E_Feature.Meta;

            // When
            string[] result = feature.GetAllFeaturesExcept();

            // Then
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Length);
            Assert.Contains(FEATURE_PICO_OPENXR, result);
            Assert.Contains(FEATURE_PICO_SUPPORT, result);
            Assert.Contains(INPUT_PICO4_TOUCH, result);
            Assert.Contains(INPUT_PICONeo3_TOUCH, result);
            Assert.Contains(FEATURE_VIVE_SUPPORT, result);
            Assert.Contains(INPUT_VIVEFocus3, result);
        }

        [Test]
        public void GivenPicoFeature_WhenGetAllFeaturesExcept_ThenReturnsAllExceptPico()
        {
            // Given
            E_Feature feature = E_Feature.Pico;

            // When
            string[] result = feature.GetAllFeaturesExcept();

            // Then
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
            Assert.Contains(FEATURE_META_QUEST, result);
            Assert.Contains(INPUT_METAQUEST_PRO, result);
            Assert.Contains(INPUT_OCULUS_TOUCH, result);
            Assert.Contains(FEATURE_VIVE_SUPPORT, result);
            Assert.Contains(INPUT_VIVEFocus3, result);
        }

        [Test]
        public void GivenViveFeature_WhenGetAllFeaturesExcept_ThenReturnsAllExceptVive()
        {
            // Given
            E_Feature feature = E_Feature.Vive;

            // When
            string[] result = feature.GetAllFeaturesExcept();

            // Then
            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.Length);
            Assert.Contains(FEATURE_META_QUEST, result);
            Assert.Contains(INPUT_METAQUEST_PRO, result);
            Assert.Contains(INPUT_OCULUS_TOUCH, result);
            Assert.Contains(FEATURE_PICO_OPENXR, result);
            Assert.Contains(FEATURE_PICO_SUPPORT, result);
            Assert.Contains(INPUT_PICO4_TOUCH, result);
            Assert.Contains(INPUT_PICONeo3_TOUCH, result);
        }
    }
}