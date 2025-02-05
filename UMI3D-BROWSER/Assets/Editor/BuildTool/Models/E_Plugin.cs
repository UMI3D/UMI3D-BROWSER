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
        public static string GetLoaderName(this E_Plugin plugin) => plugin switch
        {
            E_Plugin.OpenXR => BuildStaticNames.LOADER_OPEN_XR,
            E_Plugin.Oculus => BuildStaticNames.LOADER_OCULUS,
            E_Plugin.OpenVR => BuildStaticNames.LOADER_OPEN_VR,
            E_Plugin.PicoXR => BuildStaticNames.LOADER_PICO,
            E_Plugin.WaveXR => BuildStaticNames.LOADER_WAVE_XR,
            _ => throw new NotImplementedException()
        };
    }
}

