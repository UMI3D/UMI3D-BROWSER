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
        static void LogBrowserIdentity()
        {
            UnityEngine.Debug.Log(BrowserIdentity());
        }

        public static string BrowserIdentity()
        {
            // Application.version format : [Browser Version] Sdk: [SDK Version]
            var versions = Application.version.Split("Sdk: ");

            var log =
                $"==================== Start UMI3D Report ====================\n" +
                $"\n" +
                $"[UMI3D] Company: {Application.companyName}\n" +
                $"[UMI3D] {Application.productName}\n" +
                $"[UMI3D] Version: {versions[0]}\n" +
                $"[UMI3D] SDK Version: {versions[1]}\n" +
                $"\n" +
                $"==================== End UMI3D Report ====================\n" +
                $"\n";


            return log;
        }
    }
}