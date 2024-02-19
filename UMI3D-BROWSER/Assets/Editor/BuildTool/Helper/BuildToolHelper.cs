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

using NAudio.SoundFont;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine.Assertions;

namespace umi3d.browserEditor.BuildTool
{
    public class BuildToolHelper
    {
        public static string GetApplicationName(TargetDto target)
        {
            string name = $"UMI3D {target.Target}";

            switch (target.releaseCycle)
            {
                case E_ReleaseCycle.Alpha:
                    name += $" {target.releaseCycle}";
                    break;
                case E_ReleaseCycle.Beta:
                    name += $" {target.releaseCycle}";
                    break;
                case E_ReleaseCycle.Production:
                    break;
                default:
                    break;
            }

            return name;
        }

        /// <summary>
        /// doc: https://docs.unity3d.com/Manual/cus-naming.html
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string GetPackageName(TargetDto target)
        {
            return target.Target switch
            {
                E_Target.Quest => "com.inetum.OculusQuestBrowser",
                _ => "com.inetum.umi3d_browser"
            };
        }

        public static string GetExeName(TargetDto target, VersionDTO version, bool withExtension)
        {
            string name = $"UMI3D_{target.Target}_Browser_{version.VersionFromNow}";
            if (withExtension)
            {
                switch (target.Target)
                {
                    case E_Target.Quest:
                    case E_Target.Focus:
                    case E_Target.Pico:
                        name += ".apk";
                        break;
                    case E_Target.SteamXR:
                        name += ".exe";
                        break;
                    default:
                        break;
                }
            }
            return name;
        }

        public static string GetBuildPath(VersionDTO version, VersionDTO sdkVersion, TargetDto target, bool addExeDir)
        {
            string path = 
                $"{target.BuildFolder}/" +
                $"{target.releaseCycle}/" +
                $"{version.VersionFromNow}_SDK{sdkVersion.Version}/";

            if (addExeDir)
            {
                path += $"{GetExeName(target, version, withExtension: false)}/";
            }

            return path;
        }

        public static void CreateBuildPath(VersionDTO version, VersionDTO sdkVersion, TargetDto target, bool overwrite)
        {
            string path = GetBuildPath(
                version, 
                sdkVersion, 
                target, 
                addExeDir: EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Standalone
            );
            if (Directory.Exists(path))
            {
                if (overwrite)
                {
                    Directory.Delete(path, true);
                    Directory.CreateDirectory(path);
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void CopyLicense(string licensePath, VersionDTO version, VersionDTO sdkVersion, TargetDto target)
        {
            if (EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Standalone)
            {
                return;
            }
            File.Copy(
                licensePath, 
                $"{GetBuildPath(version, sdkVersion, target, EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Standalone)}license.txt", 
                true
            );
        }

        public static void SetKeystore(string password, string path)
        {
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = path;
            PlayerSettings.keyaliasPass = password;
            PlayerSettings.keystorePass = password;
        }

        public static BuildReport BuildPlayer(VersionDTO version, VersionDTO sdkVersion, TargetDto target)
        {
            return BuildPipeline.BuildPlayer(
                GetPlayerBuildOptions(
                    version,
                    sdkVersion,
                    target
                )
            );
        }

        public static BuildPlayerOptions GetPlayerBuildOptions(VersionDTO version, VersionDTO sdkVersion, TargetDto target)
        {
            BuildPlayerOptions pbo = new();

            pbo.scenes = EditorBuildSettings.scenes.Select(scene =>
            {
                return scene.path;
            }).ToArray();
            pbo.locationPathName = 
                GetBuildPath(
                    version, 
                    sdkVersion, 
                    target, 
                    addExeDir: EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Standalone
                ) 
                + GetExeName(target, version, withExtension: true);
            pbo.target = target.Target.GetBuildTarget();
            pbo.options = BuildOptions.None;

            return pbo;
        }

        public static void DeleteBurstDebugInformationFolder(BuildReport buildReport)
        {
            try
            {
                string outputPath = buildReport.summary.outputPath;
                string applicationName = Path.GetFileNameWithoutExtension(outputPath);
                string outputFolder = Path.GetDirectoryName(outputPath);
                Assert.IsNotNull(outputFolder);

                outputFolder = Path.GetFullPath(outputFolder);

                string burstDebugInformationDirectoryPath = Path.Combine(outputFolder, $"{applicationName}_BurstDebugInformation_DoNotShip");

                if (Directory.Exists(burstDebugInformationDirectoryPath))
                {
                    UnityEngine.Debug.Log($"[UMI3D] BuildTool: Deleting Burst debug information folder");

                    Directory.Delete(burstDebugInformationDirectoryPath, true);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[UMI3D] BuildTool: An unexpected exception occurred while performing build cleanup: {e}");
            }
        }

        /// <summary>
        /// Return -2 if the build has failed.<br/>
        /// Return -1 if the build has been cancelled.<br/>
        /// Return 0 if the build result is unknown.<br/>
        /// Return 1 if the build has succeeded.<br/>
        /// </summary>
        /// <param name="report"></param>
        /// <param name="revealInFinder"></param>
        /// <returns></returns>
        public static int Report(BuildReport report, bool revealInFinder)
        {
            BuildSummary summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Unknown:
                    return 0;
                case BuildResult.Succeeded:
                    UnityEngine.Debug.Log($"[UMI3D] BuildTool: Build succeeded: {summary.outputPath}");
                    if (revealInFinder)
                    {
                        EditorUtility.RevealInFinder(report.summary.outputPath);
                    }
                    return 1;
                case BuildResult.Failed:
                    UnityEngine.Debug.Log($"[UMI3D] BuildTool: Build failed: {summary.outputPath}");
                    return -2;
                case BuildResult.Cancelled:
                    UnityEngine.Debug.Log($"[UMI3D] BuildTool: Build Canceled: {summary.outputPath}");
                    return -1;
                default:
                    return 0;
            }
        }
    }
}

