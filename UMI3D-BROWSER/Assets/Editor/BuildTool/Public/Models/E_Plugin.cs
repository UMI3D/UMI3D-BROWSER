/*
Copyright 2019 - 2024 Inetum

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

namespace umi3d.browserEditor.BuildTool
{
    public enum E_Plugin
    {
        OpenXR,
        Oculus,
        OpenVR,
        PicoXR,
        WaveXR
    }

    public static class PluginExt
    {
        internal const string LOADER_OPEN_XR = "Unity.XR.OpenXR.OpenXRLoader";
        internal const string LOADER_OCULUS = "Unity.XR.Oculus.OculusLoader";
        internal const string LOADER_OPEN_VR = "Unity.XR.OpenVR.OpenVRLoader";
        internal const string LOADER_PICO = "Unity.XR.PXR.PXR_Loader";
        internal const string LOADER_WAVE_XR = "Wave.XR.Loader.WaveXRLoader";

        /// <summary>
        /// This method returns the loader name corresponding to the given plugin type.<br/>
        /// <br/>
        /// <example>
        /// Given a valid plugin type when GetLoaderName is called then it returns the corresponding loader name.<br/>
        /// <br/>
        /// <code>
        /// var result = E_Plugin.OpenXR.GetLoaderName();
        /// // Return <see cref="LOADER_OPEN_XR"/>.
        /// </code> 
        /// </example>
        /// </summary>
        /// <param name="plugin">The plugin type.</param>
        /// <returns>The loader name corresponding to the given plugin type.</returns>
        /// <exception cref="NotImplementedException">Thrown when the plugin type is not implemented.</exception>
        public static string GetLoaderName(this E_Plugin plugin) => plugin switch
        {
            E_Plugin.OpenXR => LOADER_OPEN_XR,
            E_Plugin.Oculus => LOADER_OCULUS,
            E_Plugin.OpenVR => LOADER_OPEN_VR,
            E_Plugin.PicoXR => LOADER_PICO,
            E_Plugin.WaveXR => LOADER_WAVE_XR,
            _ => throw new NotImplementedException()
        };
    }
}

