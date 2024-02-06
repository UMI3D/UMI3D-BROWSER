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
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace umi3d.browserEditor.BuildTool
{
    public class BuildToolHelper
    {
        public static void UpdateApplicationName(TargetDto target)
        {
            string name = $"UMI3D";

            switch (target.Target)
            {
                case E_Target.Quest:
                    break;
                case E_Target.Focus:
                    break;
                case E_Target.Pico:
                    break;
                case E_Target.SteamXR:
                    name = $" {target.Target}";
                    break;
                default:
                    break;
            }

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

            PlayerSettings.productName = name;
        }

        public static BuildPlayerOptions GetPlayerBuildOptions(VersionDTO version, TargetDto target)
        {
            BuildPlayerOptions pbo = new();

            pbo.scenes = EditorBuildSettings.scenes.Select(scene =>
            {
                return scene.path;
            }).ToArray();
            pbo.locationPathName = 
                $"{target.BuildFolder}/" +
                $"{target.releaseCycle}/" +
                $"{version.VersionFromNow}_SDK2_8_0_240208/" +
                $"UMI3D {target.Target} Browser";
            switch (target.Target)
            {
                case E_Target.Quest:
                case E_Target.Focus:
                case E_Target.Pico:
                    pbo.locationPathName += ".apk";
                    break;
                case E_Target.SteamXR:
                    pbo.locationPathName += ".exe";
                    break;
                default:
                    break;
            }
            pbo.target = target.Target.GetBuildTarget();
            pbo.options = BuildOptions.None;

            UnityEngine.Debug.Log($"path = {pbo.locationPathName}");

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

        public static void Report(BuildReport report)
        {
            BuildSummary summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Unknown:
                    break;
                case BuildResult.Succeeded:
                    UnityEngine.Debug.Log($"[UMI3D] BuildTool: Build succeeded: {summary.outputPath}");
                    break;
                case BuildResult.Failed:
                    UnityEngine.Debug.Log($"[UMI3D] BuildTool: Build failed: {summary.outputPath}");
                    break;
                case BuildResult.Cancelled:
                    UnityEngine.Debug.Log($"[UMI3D] BuildTool: Build Canceled: {summary.outputPath}");
                    break;
                default:
                    break;
            }
        }
    }
}

