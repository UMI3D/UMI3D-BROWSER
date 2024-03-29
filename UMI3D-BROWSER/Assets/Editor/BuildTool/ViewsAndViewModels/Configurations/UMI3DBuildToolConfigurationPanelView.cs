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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolConfigurationPanelView 
    {
        public VisualElement root;
        public UMI3DBuildToolTarget_SO buildToolTarget_SO;
        public UMI3DBuildToolKeystore_SO buildToolKeystore_SO;
        public UMI3DBuildToolSettings_SO buildToolSettings_SO;

        public UMI3DBuildToolConfigurationViewModel viewModel;

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

        public UMI3DBuildToolConfigurationPanelView(
            VisualElement root,
            UMI3DBuildToolTarget_SO buildToolTarget_SO,
            UMI3DBuildToolKeystore_SO buildToolKeystore_SO,
            UMI3DBuildToolSettings_SO buildToolSettings_SO
        )
        {
            this.root = root;
            this.buildToolTarget_SO = buildToolTarget_SO;
            this.buildToolKeystore_SO = buildToolKeystore_SO;
            this.buildToolSettings_SO = buildToolSettings_SO;
            viewModel = new(
                buildToolTarget_SO,
                buildToolKeystore_SO
            );
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
               = (buildToolSettings_SO?.useOneBuildFolder ?? true)
                   ? DisplayStyle.Flex
                   : DisplayStyle.None;
            (TF_BuildFolder.labelElement as INotifyValueChanged<string>)
                .SetValueWithoutNotify("Build Folder");
            TF_BuildFolder
                .SetValueWithoutNotify(buildToolTarget_SO.buildFolder);
           
            T_SBF.SetValueWithoutNotify(buildToolSettings_SO.useOneBuildFolder);

            (TF_Installer.labelElement as INotifyValueChanged<string>)
                .SetValueWithoutNotify("Installer");
            TF_Installer
                .SetValueWithoutNotify(buildToolTarget_SO.installer);

            (TF_License.labelElement as INotifyValueChanged<string>)
                .SetValueWithoutNotify("License");
            TF_License
                .SetValueWithoutNotify(buildToolTarget_SO.license);

            (TF_Keystore.labelElement as INotifyValueChanged<string>)
                .SetValueWithoutNotify("Keystore");
            TF_Keystore
                .SetValueWithoutNotify(buildToolKeystore_SO.path);
            TF_KeystorePW
                .SetValueWithoutNotify(buildToolKeystore_SO.password);
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
            viewModel.UpdateBuildFolder(value.newValue);
        }

        void BrowseBuildFolder()
        {
            viewModel.BrowseBuildFolder(path =>
            {
                TF_BuildFolder.SetValueWithoutNotify(path);
            });
        }

        void InstallerFolderValueChanged(ChangeEvent<string> value)
        {
            viewModel.UpdateInstaller(value.newValue);
        }

        void BrowseInstallerFolder()
        {
            viewModel.BrowseInstaller(path =>
            {
                TF_Installer.SetValueWithoutNotify(path);
            });
        }

        void LicenseFolderValueChanged(ChangeEvent<string> value)
        {
            viewModel.UpdateLicense(value.newValue);
        }

        void BrowseLicenseFolder()
        {
            viewModel.BrowseLicense(path =>
            {
                TF_License.SetValueWithoutNotify(path);
            });
        }

        void KeystoreFolderValueChanged(ChangeEvent<string> value)
        {
            viewModel.UpdateKeystorePath(value.newValue);
        }

        void BrowseKeystoreFolder()
        {
            viewModel.BrowseKeystorePath(path =>
            {
                TF_Keystore.SetValueWithoutNotify(path);
            });
        }

        void KeystorePasswordValueChanged(ChangeEvent<string> value)
        {
            viewModel.UpdateKeystorePassword(value.newValue);
        }
    }
}