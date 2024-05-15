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

using inetum.unityUtils;
using System.Linq;
using umi3d.cdk.collaboration;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using DataCreation = umi3d.browserEditor.BuildTool.UMI3DBuildToolDataCreation;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset ui = default;
        [SerializeField] private VisualTreeAsset target_VTA = default;
        [SerializeField] private VisualTreeAsset scene_VTA = default;

        UMI3DBuildToolVersion_SO versionModel;
        UMI3DBuildToolTarget_SO targetModel;
        UMI3DBuildToolScene_SO sceneModel;
        UMI3DBuildToolKeystore_SO keystoreModel;
        UMI3DBuildToolSettings_SO settingModel;

        [SerializeField] UMI3DCollabLoadingParameters loadingParameters;
        UMI3DConfigurator _uMI3DConfigurator = null;

        UMI3DBuildToolView buildView;

        SubGlobal subGlobal = new("BuildTool");

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
            UMI3DBuildToolWindow wnd = GetWindow<UMI3DBuildToolWindow>();
            wnd.titleContent = new GUIContent("UMI3D Build Tool");
            wnd.buildView.ChangePanel(E_BuildToolPanel.Main);
            wnd.maxSize = new(1000f, 300f);
            wnd.minSize = new(1000f, 300f);
        }

        [MenuItem("Tools/Build Tool Configuration")]
        public static void OpenBuildToolConfigurationWindow()
        {
            UMI3DBuildToolWindow wnd = GetWindow<UMI3DBuildToolWindow>();
            wnd.titleContent = new GUIContent("UMI3D Build Tool Config");
            wnd.buildView.ChangePanel(E_BuildToolPanel.Configuration);
            wnd.maxSize = new(1000f, 300f);
            wnd.minSize = new(1000f, 300f);
        }

        // TODO
        //[MenuItem("Tools/Build Tool History")]
        //public static void OpenBuildToolHistoryWindow()
        //{
        //    UMI3DBuildToolWindow wnd = GetWindow<UMI3DBuildToolWindow>();
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
                scene_VTA,
                "[UMI3D] BuildTool: scene_VTA is null."
            );

            DataCreation.GetPath();
            DataCreation.CreateExcludedFolderIfNecessary();
            DataCreation.GetFiles();
            versionModel = DataCreation.GetSO<UMI3DBuildToolVersion_SO>("Version");
            sceneModel = DataCreation.GetSO<UMI3DBuildToolScene_SO>("Scenes");
            targetModel = DataCreation.GetSO<UMI3DBuildToolTarget_SO>("Target");
            keystoreModel = DataCreation.GetSO<UMI3DBuildToolKeystore_SO>("Keystore");
            settingModel = DataCreation.GetSO<UMI3DBuildToolSettings_SO>("Settings");
            DataCreation.SaveAndRefresh();

            Assert.IsNotNull(
                versionModel,
                "[UMI3D] BuildTool: versionModel is null."
            );
            Assert.IsNotNull(
                targetModel,
                "[UMI3D] BuildTool: targetModel is null."
            );
            Assert.IsNotNull(
                sceneModel,
                "[UMI3D] BuildTool: sceneModel is null."
            );
            Assert.IsNotNull(
                keystoreModel,
                "[UMI3D] BuildTool: keystoreModel is null.\n" +
                "Create a [Build Tool Keystore] scriptable object in an EXCLUDED folder that is excluded from git."
            );
            Assert.IsNotNull(
                settingModel,
                "[UMI3D] BuildTool: settingModel is null."
            );
            _uMI3DConfigurator = new UMI3DConfigurator(loadingParameters);

            targetModel.applyTargetOptionsHandler += ApplyTargetOptions;
            targetModel.buildSelectedTargetHandler += BuildSelectedTargets;
            targetModel.target_VTA = target_VTA;

            sceneModel.SelectedScenesChanged += ApplyScenes;
            sceneModel.scene_VTA = scene_VTA;

            subGlobal.Add(versionModel);
            subGlobal.Add(targetModel);
            subGlobal.Add(sceneModel);
            subGlobal.Add(keystoreModel);
            subGlobal.Add(settingModel);

            buildView = new(
                rootVisualElement,
                ui
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
            EditorBuildSettings.scenes = sceneModel.GetScenesForTarget(
                targetModel.currentTarget
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
            PlayerSettings.bundleVersion = $"{target.releaseCycle.GetReleaseInitial()}_{versionModel.newVersion.VersionFromNow} Sdk: {versionModel.sdkVersion.Version}";
            PlayerSettings.Android.bundleVersionCode = versionModel.newVersion.BundleVersion;

            BuildToolHelper.SetKeystore(keystoreModel.password, keystoreModel.path);

            InstallerHelper.UpdateInstaller(
                targetModel.installer,
                targetModel.license,
                versionModel.newVersion,
                versionModel.sdkVersion,
                target
            );
            var report = BuildToolHelper.BuildPlayer(
                versionModel.newVersion,
                versionModel.sdkVersion,
                target
            );
            BuildToolHelper.DeleteBurstDebugInformationFolder(report);
            var reportInt = BuildToolHelper.Report(report, revealInFinder);
            if (reportInt == 1)
            {
                BuildToolHelper.CopyLicense(
                    targetModel.license,
                    versionModel.newVersion,
                    versionModel.sdkVersion,
                    target
                );
            }
            return reportInt;
        }

        void BuildSelectedTargets(params TargetDto[] target)
        {
            versionModel.UpdateOldVersion();
            for (int i = 0; i < target.Length; i++)
            {
                ApplyTargetOptions(target[i].Target);
                BuildTarget(target[i], i == target.Length - 1);
            }
        }
    }
}
