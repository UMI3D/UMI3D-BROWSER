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

namespace umi3d.common.core.target
{
    public struct Plugin
    {
        public readonly string name;

        public readonly string loader;

        public static IReadOnlyList<Plugin> allCases => _allCases.Value;
        static Lazy<Plugin[]> _allCases = new(() =>
        {
            return new[] { OpenXR, Oculus, ARCore, ARKit, MockHMDLoader, XRSimulation, PICOLivePreview };
        });

        /// <summary>
        /// Plugin for Meta, Pico, Focus and most of the XR devices.
        /// </summary>
        public static readonly Plugin OpenXR = new Plugin("OpenXR Loader", "UnityEngine.XR.OpenXR.OpenXRLoader");
        /// <summary>
        /// Plugin for Oculus Quest device only.<br/>
        /// <b>This plugin and the <see cref="OpenXR"/> plugin cannot be use together.</b>
        /// </summary>
        public static readonly Plugin Oculus = new Plugin("Oculus", "Unity.XR.Oculus.OculusLoader");
        public static readonly Plugin ARCore = new Plugin("ARCore", "UnityEngine.XR.ARCore.ARCoreLoader");
        public static readonly Plugin ARKit = new Plugin("ARKit", "UnityEngine.XR.ARKit.ARKitLoader");
        public static readonly Plugin MockHMDLoader = new Plugin("Mock HMD Loader", "Unity.XR.MockHMD.MockHMDLoader");
        public static readonly Plugin XRSimulation = new Plugin("XR Simulation", "UnityEngine.XR.Simulation.SimulationLoader");
        public static readonly Plugin PICOLivePreview = new Plugin("PICO Live Preview", "Unity.XR.PICO.LivePreview.PXR_PTLoader");

        public Plugin(string name, string loader)
        {
            this.name = name;
            this.loader = loader;
        }
    }
}

