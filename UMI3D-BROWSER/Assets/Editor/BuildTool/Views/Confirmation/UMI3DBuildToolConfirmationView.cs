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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolConfirmationView 
    {
        public VisualElement root;

        SubGlobal subGlobal = new("BuildTool");
        UMI3DBuildToolVersion_SO versionModel;
        UMI3DBuildToolTarget_SO targetModel;
        UMI3DBuildToolScene_SO sceneModel;
        UMI3DBuildToolKeystore_SO keystoreModel;
        UMI3DBuildToolSettings_SO settingModel;
        Label version;
        Label currentBranch;
        ListView targets;

        public UMI3DBuildToolConfirmationView(
            VisualElement root,
            VisualTreeAsset ui
        )
        {
            this.root = root;

            root.Add(ui.Instantiate());

            subGlobal.TryGet(out versionModel);
            subGlobal.TryGet(out targetModel);
            subGlobal.TryGet(out sceneModel);
            subGlobal.TryGet(out keystoreModel);
            subGlobal.TryGet(out settingModel);
        }

        public void Bind()
        {
            version = root.Q<Label>("L_Version");
            currentBranch = root.Q<Label>("L_Branch");
            targets = root.Q<ListView>("LV_Targets");
        }

        public void Set()
        {
            version.text = $"Version: {versionModel.newVersion.VersionFromNow}";
            currentBranch.text = $"Current branch: TODO";
        }
    }
}