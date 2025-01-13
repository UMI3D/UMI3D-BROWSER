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

using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace umi3d.browserEditor.BuildTool
{
    public static class InstallerHelper 
    {
        /// <summary>
        /// Update the installer with the information of the target, version, ect.
        /// </summary>
        /// <remarks>This method is only called for UNITY_STANDALONE.</remarks>
        /// <param name="InstallerPath"></param>
        /// <param name="licensePath"></param>
        /// <param name="appId"></param>
        /// <param name="version"></param>
        /// <param name="sdkVersion"></param>
        /// <param name="target"></param>
        [Conditional("UNITY_STANDALONE")]
        public static void UpdateInstaller(
            string InstallerPath,
            string licensePath,
            string appId,
            VersionDTO version,
            VersionDTO sdkVersion,
            TargetDto target
        )
        {
            if (string.IsNullOrEmpty(InstallerPath))
            {
                UnityEngine.Debug.LogError($"[UMI3D] Build Tool: Installer path is empty");
                return;
            }
            if (string.IsNullOrEmpty(licensePath))
            {
                UnityEngine.Debug.LogError($"[UMI3D] Build Tool: License path is empty");
                return;
            }

            if (!File.Exists(InstallerPath))
            {
                UnityEngine.Debug.LogError($"[UMI3D] Build Tool: installer not found.");
                return;
            }

            string appName = BuildToolHelper.GetApplicationName(target);
            string exeName = BuildToolHelper.GetBuiltFileName(target, version, true);
            string outputDir = BuildToolHelper.GetBuildPath(version, sdkVersion, target, false);
            string buildPath = BuildToolHelper.GetBuildPath(version, sdkVersion, target, true);

            string setupText = File.ReadAllText(InstallerPath);
            setupText = Regex.Replace(
                input: setupText,
                pattern: "#define MyAppId \"(.*)?\"",
                replacement: $"#define MyAppId \"{appId}\""
            );
            setupText = Regex.Replace(
                input: setupText,
                pattern: "#define MyTarget \"(.*)?\"",
                replacement: $"#define MyTarget \"{target.Target}\""
            );
            setupText = Regex.Replace(
                input: setupText,
                pattern: "#define MyAppName \"(.*)?\"",
                replacement: $"#define MyAppName \"{appName}\""
            );
            setupText = Regex.Replace(
                input: setupText, 
                pattern: "#define MyAppVersion \"(.*)?\"", 
                replacement: $"#define MyAppVersion \"{version.VersionFromNow()}\""
            );
            setupText = Regex.Replace(
               input: setupText,
               pattern: "#define MyAppLicense \"(.*)?\"",
               replacement: $"#define MyAppLicense \"{licensePath}\""
            );
            setupText = Regex.Replace(
                input: setupText,
                pattern: "#define MyAppExeName \"(.*)?\"",
                replacement: $"#define MyAppExeName \"{exeName}\""
            );
            setupText = Regex.Replace(
                input: setupText,
                pattern: "#define MyAppOutputDir \"(.*)?\"",
                replacement: $"#define MyAppOutputDir \"{outputDir}\""
            );
            setupText = Regex.Replace(
                input: setupText,
                pattern: "#define MyBuildPath \"(.*)?\"",
                replacement: $"#define MyBuildPath \"{buildPath}\""
            );
            File.WriteAllText(InstallerPath, setupText);


            //Regex DirReg = new Regex(patternOutputDir, RegexOptions.Multiline | RegexOptions.Singleline);
            //var md = DirReg.Match(setupText);
            //string path = md.Groups[1].Captures[0].Value + "\\" + md.Groups[2].Captures[0].Value + ".exe";
            //return (path, md.Groups[2].Captures[0].Value + ".exe");
        }
    }
}