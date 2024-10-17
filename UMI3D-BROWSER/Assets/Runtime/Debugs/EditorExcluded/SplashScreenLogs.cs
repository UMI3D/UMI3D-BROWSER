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

using UnityEngine;

namespace umi3d.browserRuntime.debug
{
    public class SplashScreenLogs 
    {
        /// <summary>
        /// Log Browser identity report on game start.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void LogAtStart()
        {
            UnityEngine.Debug.Log(BrowserIdentity() + DeviceConfig());

            Application.lowMemory += LowMemory;
            Application.quitting += Quitting;
        }

        static void LowMemory()
        {
            UnityEngine.Debug.LogError($"[UMI3D] Low memory callback.\n");
        }

        static void Quitting()
        {
            UnityEngine.Debug.Log($"[UMI3D] Application is quitting");
        }

        static string BrowserIdentity()
        {
            // Application.version format : [Browser Version] Sdk: [SDK Version]
            var versions = Application.version.Split("Sdk: ");

            return
                $"==================== Start UMI3D Report ====================\n" +
                $"\n" +
                $"[UMI3D] Company: {Application.companyName}\n" +
                $"[UMI3D] {Application.productName}\n" +
                $"[UMI3D] Version: {versions[0]}\n" +
                $"[UMI3D] SDK Version: {versions[1]}\n" +
                $"\n" +
                $"===================== End UMI3D Report =====================\n" +
                $"\n";
        }

        static string DeviceConfig()
        {
            return
                $"================ Start Device Config Report ================\n" +
                $"\n" +
                $"[UMI3D] OS: {SystemInfo.operatingSystem}\n" +
                $"\n" +
                $"[UMI3D] Device model: {SystemInfo.deviceModel}\n" +
                $"[UMI3D] Device type: {SystemInfo.deviceType}\n" +
                $"\n" +
                $"[UMI3D] Processor Count: {SystemInfo.processorCount}\n" +
                $"[UMI3D] Processor frequency: {SystemInfo.processorFrequency}\n" +
                $"[UMI3D] Processor type: {SystemInfo.processorType}\n" +
                $"\n" +
                $"[UMI3D] Graphics device name: {SystemInfo.graphicsDeviceName}\n" +
                $"[UMI3D] Graphics device type: {SystemInfo.graphicsDeviceType}\n" +
                $"[UMI3D] Graphics memory size: {SystemInfo.graphicsMemorySize}\n" +
                $"\n" +
                $"[UMI3D] System memory size: {SystemInfo.systemMemorySize}\n" +
                $"\n" +
                $"================= End Device Config Report =================\n" +
                $"\n";
        }
    }
}