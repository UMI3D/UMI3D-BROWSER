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
        UMI3DBuildToolKeystore_SO buildToolKeystore_SO;
        UMI3DBuildToolVersion_SO buildToolVersion_SO;
        UMI3DBuildToolTarget_SO buildToolTarget_SO;
        UMI3DBuildToolScene_SO buildToolScene_SO;
        UMI3DBuildToolSettings_SO buildToolSettings_SO;

        [SerializeField] UMI3DCollabLoadingParameters loadingParameters;
        IBuilToolComponent _uMI3DConfigurator = null;

        UMI3DBuildToolView buildView;

        public enum E_BuildToolPanel
        {
            Main,
            Configuration,
            History,
            Build
        }

        [MenuItem("Tools/Build Tool")]
        public static void OpenBuildToolWindow()
        {
            UMI3DBuildTool wnd = GetWindow<UMI3DBuildTool>();
            wnd.titleContent = new GUIContent("UMI3D Build Tool");
            wnd.buildView.ChangePanel(E_BuildToolPanel.Main);
        }

        [MenuItem("Tools/Build Tool Configuration")]
        public static void OpenBuildToolConfigurationWindow()
        {
            UMI3DBuildTool wnd = GetWindow<UMI3DBuildTool>();
            wnd.titleContent = new GUIContent("UMI3D Build Tool Config");
            wnd.buildView.ChangePanel(E_BuildToolPanel.Configuration);
        }

        // TODO
        //[MenuItem("Tools/Build Tool History")]
        //public static void OpenBuildToolHistoryWindow()
        //{
        //    UMI3DBuildTool wnd = GetWindow<UMI3DBuildTool>();
        //    wnd.titleContent = new GUIContent("UMI3D Build Tool History");
        //    wnd.buildView.ChangePanel(E_BuildToolPanel.History);
        //}

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

            UMI3DBuildToolDataCreation.GetPath();
            UMI3DBuildToolDataCreation.CreateExcludedFolderIfNecessary();
            UMI3DBuildToolDataCreation.GetFiles();
            buildToolKeystore_SO = UMI3DBuildToolDataCreation.GetSO<UMI3DBuildToolKeystore_SO>("Keystore");
            buildToolVersion_SO = UMI3DBuildToolDataCreation.GetSO<UMI3DBuildToolVersion_SO>("Version");
            buildToolScene_SO = UMI3DBuildToolDataCreation.GetSO<UMI3DBuildToolScene_SO>("Scenes");
            buildToolTarget_SO = UMI3DBuildToolDataCreation.GetSO<UMI3DBuildToolTarget_SO>("Target");
            buildToolSettings_SO = UMI3DBuildToolDataCreation.GetSO<UMI3DBuildToolSettings_SO>("Settings");
            UMI3DBuildToolDataCreation.SaveAndRefresh();

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

            buildView = new(
                rootVisualElement,
                ui, target_VTA, path_VTA, scene_VTA,
                buildToolKeystore_SO, buildToolVersion_SO, buildToolTarget_SO, buildToolScene_SO, buildToolSettings_SO,
                ApplyScenes,
                ApplyTargetOptions,
                BuildSelectedTargets
            );
            buildView.Bind();
            buildView.Set();
        }

        void ApplyTargetOptions(E_Target target)
        {
            ApplyScenes();

            // Switch target if needed and toggle options.
            _uMI3DConfigurator.HandleTarget(target);
            BuildTargetHelper.SwitchTarget(target);
            PluginHelper.SwitchPlugins(target);
            FeatureHelper.SwitchFeatures(target);
        }

        void ApplyScenes()
        {
            EditorBuildSettings.scenes = buildToolScene_SO.GetScenesForTarget(
                buildToolTarget_SO.currentTarget
            ).Select(scene =>
            {
                return new EditorBuildSettingsScene(scene.path, true);
            }).ToArray();
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
            // Update App name, Version and Android.BundleVersion.
            PlayerSettings.productName = BuildToolHelper.GetApplicationName(target);
            PlayerSettings.applicationIdentifier = BuildToolHelper.GetPackageName(target);
            PlayerSettings.bundleVersion = $"{target.releaseCycle.GetReleaseInitial()}_{buildToolVersion_SO.newVersion.VersionFromNow} Sdk: {buildToolVersion_SO.sdkVersion.Version}";
            PlayerSettings.Android.bundleVersionCode = buildToolVersion_SO.newVersion.BundleVersion;

            BuildToolHelper.SetKeystore(buildToolKeystore_SO.password, buildToolKeystore_SO.path);

            InstallerHelper.UpdateInstaller(
                buildToolTarget_SO.installer,
                buildToolTarget_SO.license,
                buildToolVersion_SO.newVersion,
                buildToolVersion_SO.sdkVersion,
                target
            );
            var report = BuildToolHelper.BuildPlayer(
                buildToolVersion_SO.newVersion,
                buildToolVersion_SO.sdkVersion,
                target
            );
            BuildToolHelper.DeleteBurstDebugInformationFolder(report);
            var reportInt = BuildToolHelper.Report(report, revealInFinder);
            if (reportInt == 1)
            {
                BuildToolHelper.CopyLicense(
                    buildToolTarget_SO.license,
                    buildToolVersion_SO.newVersion,
                    buildToolVersion_SO.sdkVersion,
                    target
                );
            }
            return reportInt;
        }

        void BuildSelectedTargets(params TargetDto[] target)
        {
            for (int i = 0; i < target.Length; i++)
            {
                ApplyTargetOptions(target[i].Target);
                BuildTarget(target[i], i == target.Length - 1);
            }
        }
    }
}
