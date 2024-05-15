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
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolConfigurationPanelView 
    {
        SubGlobal subGlobal = new("BuildTool");

        public VisualElement root;

        public UMI3DBuildToolTarget_SO targetModel;
        public UMI3DBuildToolKeystore_SO keystoreModel;
        public UMI3DBuildToolSettings_SO settingModel;

        public TemplateContainer T_BuildFolder;
        public TextField TF_BuildFolder;
        public Button B_BuildFolder;
        public Toggle T_SBF;

        // PC
        public TemplateContainer T_Installer;
        public TextField TF_Installer;
        public Button B_Installer;
        public TemplateContainer T_License;
        public TextField TF_License;
        public Button B_License;

        // Android
        public TemplateContainer T_Keystore;
        public TextField TF_Keystore;
        public Button B_Keystore;
        public TextField TF_KeystorePW;

        public UMI3DBuildToolConfigurationPanelView(VisualElement root)
        {
            this.root = root;

            subGlobal.TryGet(out targetModel);
            subGlobal.TryGet(out keystoreModel);
            subGlobal.TryGet(out settingModel);
        }

        public void Bind()
        {
            T_BuildFolder 
                = root.Q<TemplateContainer>("T_SingleBuildFolder");
            TF_BuildFolder = T_BuildFolder.Q<TextField>();
            B_BuildFolder = T_BuildFolder.Q<Button>();
            TF_BuildFolder.RegisterValueChangedCallback(BuildFolderValueChanged);
            B_BuildFolder.clicked += BrowseBuildFolder;
            T_SBF = root.Q<Toggle>("T_SBF");

            T_Installer = root.Q<TemplateContainer>("T_Installer");
            TF_Installer = T_Installer.Q<TextField>();
            B_Installer = T_Installer.Q<Button>();
            TF_Installer.RegisterValueChangedCallback(InstallerFolderValueChanged);
            B_Installer.clicked += BrowseInstallerFolder;

            T_License = root.Q<TemplateContainer>("T_License");
            TF_License = T_License.Q<TextField>();
            B_License = T_License.Q<Button>();
            TF_License.RegisterValueChangedCallback(LicenseFolderValueChanged);
            B_License.clicked += BrowseLicenseFolder;

            T_Keystore = root.Q<TemplateContainer>("T_Keystore");
            TF_Keystore = T_Keystore.Q<TextField>();
            B_Keystore = T_Keystore.Q<Button>();
            TF_Keystore.RegisterValueChangedCallback(KeystoreFolderValueChanged);
            B_Keystore.clicked += BrowseKeystoreFolder;
            TF_KeystorePW = root.Q<TextField>("TF_KeystorePW");
            TF_KeystorePW.RegisterValueChangedCallback(KeystorePasswordValueChanged);
        }

        public void Set()
        {
            T_BuildFolder.style.display
               = (settingModel?.useOneBuildFolder ?? true)
                   ? DisplayStyle.Flex
                   : DisplayStyle.None;
            (TF_BuildFolder.labelElement as INotifyValueChanged<string>)
                .SetValueWithoutNotify("Build Folder");
            TF_BuildFolder
                .SetValueWithoutNotify(targetModel.buildFolder);
           
            T_SBF.SetValueWithoutNotify(settingModel.useOneBuildFolder);

            (TF_Installer.labelElement as INotifyValueChanged<string>)
                .SetValueWithoutNotify("Installer");
            TF_Installer
                .SetValueWithoutNotify(targetModel.installer);

            (TF_License.labelElement as INotifyValueChanged<string>)
                .SetValueWithoutNotify("License");
            TF_License
                .SetValueWithoutNotify(targetModel.license);

            (TF_Keystore.labelElement as INotifyValueChanged<string>)
                .SetValueWithoutNotify("Keystore");
            TF_Keystore
                .SetValueWithoutNotify(keystoreModel.path);
            TF_KeystorePW
                .SetValueWithoutNotify(keystoreModel.password);
        }

        public void Unbind()
        {
            TF_BuildFolder.UnregisterValueChangedCallback(BuildFolderValueChanged);
            B_BuildFolder.clicked -= BrowseBuildFolder;
            TF_Installer.UnregisterValueChangedCallback(InstallerFolderValueChanged);
            B_Installer.clicked -= BrowseInstallerFolder;
            TF_License.UnregisterValueChangedCallback(LicenseFolderValueChanged);
            B_License.clicked -= BrowseLicenseFolder;
            TF_Keystore.UnregisterValueChangedCallback(KeystoreFolderValueChanged);
            B_Keystore.clicked -= BrowseKeystoreFolder;
            TF_KeystorePW.UnregisterValueChangedCallback(KeystorePasswordValueChanged);
        }

        void BuildFolderValueChanged(ChangeEvent<string> value)
        {
            targetModel.UpdateBuildFolder(value.newValue);
        }

        void BrowseBuildFolder()
        {
            targetModel.BrowseBuildFolder(path =>
            {
                TF_BuildFolder.SetValueWithoutNotify(path);
            });
        }

        void InstallerFolderValueChanged(ChangeEvent<string> value)
        {
            targetModel.UpdateInstaller(value.newValue);
        }

        void BrowseInstallerFolder()
        {
            targetModel.BrowseInstaller(path =>
            {
                TF_Installer.SetValueWithoutNotify(path);
            });
        }

        void LicenseFolderValueChanged(ChangeEvent<string> value)
        {
            targetModel.UpdateLicense(value.newValue);
        }

        void BrowseLicenseFolder()
        {
            targetModel.BrowseLicense(path =>
            {
                TF_License.SetValueWithoutNotify(path);
            });
        }

        void KeystoreFolderValueChanged(ChangeEvent<string> value)
        {
            keystoreModel.UpdateKeystorePath(value.newValue);
        }

        void BrowseKeystoreFolder()
        {
            keystoreModel.BrowseKeystorePath(path =>
            {
                TF_Keystore.SetValueWithoutNotify(path);
            });
        }

        void KeystorePasswordValueChanged(ChangeEvent<string> value)
        {
            keystoreModel.UpdateKeystorePassword(value.newValue);
        }
    }
}