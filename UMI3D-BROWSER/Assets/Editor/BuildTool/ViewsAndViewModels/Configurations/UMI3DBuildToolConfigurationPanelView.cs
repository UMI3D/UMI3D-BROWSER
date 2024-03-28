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
        public UMI3DBuildToolSettings_SO buildToolSettings_SO;

        public UMI3DBuildToolViewModel viewModel;
        public UMI3DBuildToolPanelView panelView;
        public TemplateContainer T_Installer;
        public TextField TF_Installer;
        public Button B_Installer;
        public TemplateContainer T_License;
        public TextField TF_License;
        public Button B_License;
        public TemplateContainer T_BuildFolder;
        public TextField TF_BuildFolder;
        public Button B_BuildFolder;

        public UMI3DBuildToolConfigurationPanelView(
            VisualElement root,
            UMI3DBuildToolTarget_SO buildToolTarget_SO,
            UMI3DBuildToolSettings_SO buildToolSettings_SO
        )
        {
            this.root = root;
            this.buildToolTarget_SO = buildToolTarget_SO;
            this.buildToolSettings_SO = buildToolSettings_SO;
            viewModel = new(buildToolTarget_SO);
            panelView = new(root);
        }

        public void Bind()
        {
            panelView.Bind();
            T_Installer = root.Q<TemplateContainer>("T_Installer");
            TF_Installer = T_Installer.Q<TextField>();
            B_Installer = T_Installer.Q<Button>();
            T_License = root.Q<TemplateContainer>("T_License");
            TF_License = T_License.Q<TextField>();
            B_License = T_License.Q<Button>();
            T_BuildFolder 
                = root.Q<TemplateContainer>("T_SingleBuildFolder");
            TF_BuildFolder = T_BuildFolder.Q<TextField>();
            B_BuildFolder = T_BuildFolder.Q<Button>();
        }

        public void Set()
        {
            panelView.SetTitle("Configuration Panel");

            (TF_Installer.labelElement as INotifyValueChanged<string>)
                .SetValueWithoutNotify("Installer");
            TF_Installer
                .SetValueWithoutNotify(buildToolTarget_SO.installer);
            TF_Installer.RegisterValueChangedCallback(
                value =>
                {
                    viewModel.UpdateInstaller(value.newValue);
                }
            );
            B_Installer.clicked += () =>
            {
                viewModel.BrowseInstaller(path =>
                {
                    TF_Installer.SetValueWithoutNotify(path);
                });
            };

            (TF_License.labelElement as INotifyValueChanged<string>)
                .SetValueWithoutNotify("License");
            TF_License
                .SetValueWithoutNotify(buildToolTarget_SO.license);
            TF_License.RegisterValueChangedCallback(
                value =>
                {
                    viewModel.UpdateLicense(value.newValue);
                }
            );
            B_License.clicked += () =>
            {
                viewModel.BrowseLicense(path =>
                {
                    TF_License.SetValueWithoutNotify(path);
                });
            };

            T_BuildFolder.style.display 
                = (buildToolSettings_SO?.useOneBuildFolder ?? true)
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            (TF_BuildFolder.labelElement as INotifyValueChanged<string>)
                .SetValueWithoutNotify("Build Folder");
            TF_BuildFolder
                .SetValueWithoutNotify(buildToolTarget_SO.buildFolder);
            TF_BuildFolder.RegisterValueChangedCallback(
                value =>
                {
                    viewModel.UpdateBuildFolder(value.newValue);
                }
            );
            B_BuildFolder.clicked += () =>
            {
                viewModel.BrowseBuildFolder(path =>
                {
                    TF_BuildFolder.SetValueWithoutNotify(path);
                });
            };
        }

        public void UnBind()
        {
            panelView.UnBind();
        }
    }
}