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
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolConfirmationWindow : EditorWindow
    {
        [SerializeField] VisualTreeAsset ui;
        [SerializeField] VisualTreeAsset target_VTA = default;

        UMI3DBuildToolConfirmationView confirmationView;

        //[MenuItem("Tools/Confirmation")]
        public static void OpenConfirmationWindow()
        {
            UMI3DBuildToolConfirmationWindow wnd = GetWindow<UMI3DBuildToolConfirmationWindow>();
            wnd.titleContent = new GUIContent("Build Confirmation");
            wnd.maxSize = new(1000f, 300f);
            wnd.minSize = new(1000f, 300f);
        }

        public void CreateGUI()
        {
            Assert.IsNotNull(
                ui,
                "[UMI3D] BuildTool: confirmation ui is null."
            );

            confirmationView = new(
                rootVisualElement,
                ui,
                target_VTA
            );
            confirmationView.Bind();
            confirmationView.Set();
        }
    }
}