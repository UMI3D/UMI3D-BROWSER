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
using static umi3d.browserEditor.BuildTool.PluginExt;

public class PluginTests
{
    public class GetLoaderNameTest
    {
        [Test]
        public void GivenValidPluginTypeOpenXR_WhenGetLoaderNameIsCalled_ThenReturnsOpenXRLoader()
        {
            // Given
            var plugin = E_Plugin.OpenXR;

            // When
            var result = plugin.GetLoaderName();

            // Then
            Assert.AreEqual(LOADER_OPEN_XR, result);
        }

        [Test]
        public void GivenValidPluginTypeOculus_WhenGetLoaderNameIsCalled_ThenReturnsOculusLoader()
        {
            // Given
            var plugin = E_Plugin.Oculus;

            // When
            var result = plugin.GetLoaderName();

            // Then
            Assert.AreEqual(LOADER_OCULUS, result);
        }

        [Test]
        public void GivenValidPluginTypeOpenVR_WhenGetLoaderNameIsCalled_ThenReturnsOpenVRLoader()
        {
            // Given
            var plugin = E_Plugin.OpenVR;

            // When
            var result = plugin.GetLoaderName();

            // Then
            Assert.AreEqual(LOADER_OPEN_VR, result);
        }

        [Test]
        public void GivenValidPluginTypePicoXR_WhenGetLoaderNameIsCalled_ThenReturnsPicoLoader()
        {
            // Given
            var plugin = E_Plugin.PicoXR;

            // When
            var result = plugin.GetLoaderName();

            // Then
            Assert.AreEqual(LOADER_PICO, result);
        }

        [Test]
        public void GivenValidPluginTypeWaveXR_WhenGetLoaderNameIsCalled_ThenReturnsWaveXRLoader()
        {
            // Given
            var plugin = E_Plugin.WaveXR;

            // When
            var result = plugin.GetLoaderName();

            // Then
            Assert.AreEqual(LOADER_WAVE_XR, result);
        }
    }
}