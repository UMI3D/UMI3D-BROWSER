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

using System.Linq;
using umi3d.cdk.collaboration;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildTool : EditorWindow
    {
        [SerializeField] private VisualTreeAsset ui = default;
        [SerializeField] private VisualTreeAsset target_VTA = default;
        [SerializeField] private VisualTreeAsset path_VTA = default;
        [SerializeField] private VisualTreeAsset scene_VTA = default;
        [SerializeField] UMI3DBuildToolKeystore_SO buildToolKeystore_SO;
        [SerializeField] UMI3DBuildToolVersion_SO buildToolVersion_SO;
        [SerializeField] UMI3DBuildToolTarget_SO buildToolTarget_SO;
        [SerializeField] UMI3DBuildToolScene_SO buildToolScene_SO;

        [SerializeField] UMI3DCollabLoadingParameters loadingParameters;
        IBuilToolComponent _uMI3DConfigurator = null;

        TargetDto[] selectedTargets;
        TargetDto targetDTO;
        VersionDTO versionDTO;

        [MenuItem("Tools/BuildTool")]
        public static void OpenWindow()
        {
            UMI3DBuildTool wnd = GetWindow<UMI3DBuildTool>();
            wnd.titleContent = new GUIContent("UMI3D Build Tool");
        }

        public void CreateGUI()
        {
            Assert.IsNotNull(
                ui,
                "[UMI3D] BuildTool: ui is null."
            );
            Assert.IsNotNull(
                target_VTA,
                "[UMI3D] BuildTool: target_VTA is null."
            );
            Assert.IsNotNull(
                path_VTA,
                "[UMI3D] BuildTool: path_VTA is null."
            );
            Assert.IsNotNull(
                scene_VTA,
                "[UMI3D] BuildTool: scene_VTA is null."
            );

            Assert.IsNotNull(
                buildToolKeystore_SO,
                "[UMI3D] BuildTool: buildToolKeystore_SO is null.\n" +
                "Create a [Build Tool Keystore] scriptable object in an EXCLUDED folder that is excluded from git."
            );
            Assert.IsNotNull(
                buildToolVersion_SO,
                "[UMI3D] BuildTool: buildToolVersion_SO is null."
            );
            Assert.IsNotNull(
                buildToolTarget_SO,
                "[UMI3D] BuildTool: buildToolTarget_SO is null."
            );
            Assert.IsNotNull(
                buildToolScene_SO,
                "[UMI3D] BuildTool: buildToolScene_SO is null."
            );
            _uMI3DConfigurator = new UMI3DConfigurator(loadingParameters);

            UMI3DBuildToolView buildView = new(
                rootVisualElement,
                ui, target_VTA, path_VTA, scene_VTA,
                buildToolKeystore_SO, buildToolVersion_SO, buildToolTarget_SO, buildToolScene_SO,
                updateVersion: newVersion =>
                {
                    versionDTO = newVersion;
                },
                updateTarget: newTarget =>
                {
                    targetDTO = newTarget;
                },
                ApplyTargetOptions,
                BuildTarget,
                BuildSelectedTargets
            );
            buildView.Bind();
            buildView.Set();
        }

        void ApplyTargetOptions(TargetDto target)
        {
            // Update App name, Version and Android.BundleVersion.
            BuildToolHelper.UpdateApplicationName(target);
            PlayerSettings.bundleVersion = $"{target.releaseCycle.GetReleaseInitial()}_{versionDTO.VersionFromNow} Sdk: {buildToolVersion_SO.sdkVersion.Version}";
            PlayerSettings.Android.bundleVersionCode = versionDTO.BundleVersion;

            // Switch target if needed and toggle options.
            _uMI3DConfigurator.HandleTarget(target.Target);
            BuildTargetHelper.SwitchTarget(target);
            PluginHelper.SwitchPlugins(target);
            FeatureHelper.SwitchFeatures(target);

            EditorBuildSettings.scenes = buildToolScene_SO.GetScenesForTarget(target.Target).Select(scene =>
            {
                return new EditorBuildSettingsScene(scene.path, true);
            }).ToArray();
            BuildToolHelper.SetKeystore(buildToolKeystore_SO.password, buildToolKeystore_SO.path);
        }

        void BuildTarget(TargetDto target)
        {
            BuildTarget(target, true);
        }

        /// <summary>
        /// Build.<br/>
        /// Return -2 if the build has failed.<br/>
        /// Return -1 if the build has been cancelled.<br/>
        /// Return 0 if the build result is unknown.<br/>
        /// Return 1 if the build has succeeded.<br/>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="revealInFinder"></param>
        /// <returns></returns>
        int BuildTarget(TargetDto target, bool revealInFinder)
        {
            var report = BuildToolHelper.BuildPlayer(
                versionDTO,
                buildToolVersion_SO.sdkVersion,
                target
            );
            BuildToolHelper.DeleteBurstDebugInformationFolder(report);
            return BuildToolHelper.Report(report, revealInFinder);
        }

        void BuildSelectedTargets(params TargetDto[] target)
        {
            for (int i = 0; i < target.Length; i++)
            {
                UnityEngine.Debug.Log($"{target[i].Target}");
                ApplyTargetOptions(target[i]);
                BuildTarget(target[i], i == target.Length - 1);
            }
        }
    }
}
