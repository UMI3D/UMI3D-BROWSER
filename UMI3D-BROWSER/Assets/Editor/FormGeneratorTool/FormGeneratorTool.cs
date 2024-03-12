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
using System.Collections.Generic;
using umi3d;
using umi3d.common.interaction;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace form_generator
{
    public class FormGeneratorTool : EditorWindow
    {
        [SerializeField] private VisualTreeAsset ui = default;

        private Toggle t_newUI;
        private VisualElement FormGeneration;
        private VisualElement FormResult;
        private Button SubmitForm;
        private Button ResetButton;

        private OldFormListElementsOptions _setOldFormListView;
        private OldConnectionFormMaker _oldConnectionFormMaker;

        MainContainer mainContainer;

        [MenuItem("Tools/FormGeneratorTool")]
        public static void ShowExample()
        {
            FormGeneratorTool wnd = GetWindow<FormGeneratorTool>();
            wnd.titleContent = new GUIContent("FormGeneratorTool");
        }

        public void CreateGUI()
        {
            //UnityEditor.EditorApplication.isPlaying = true;

            mainContainer = FindAnyObjectByType<MainContainer>();

            if (mainContainer == null) return;

            rootVisualElement.Add(ui.Instantiate());
            BuildUI();
            BindUI();
            BindServices();
        }

        private void BuildUI()
        {
            t_newUI = rootVisualElement.Q<Toggle>("t_newUI");
            FormGeneration = rootVisualElement.Q<VisualElement>("FormGeneration");
            FormResult = rootVisualElement.Q<VisualElement>("FormResult");

            SubmitForm = rootVisualElement.Q<Button>("SubmitForm");
            ResetButton = rootVisualElement.Q<Button>("ResetButton");

            _setOldFormListView = new(FormGeneration);
            _oldConnectionFormMaker = new OldConnectionFormMaker(FormResult);
        }
        private void BindUI()
        {
            SubmitForm.clicked += () =>
            {
                mainContainer.ToolAccessProcessForm(_oldConnectionFormMaker.ConnectionForm);
            };

            ResetButton.clicked += () =>
            {
                _oldConnectionFormMaker.Reset();
            };
        }

        private void BindServices()
        {
            _setOldFormListView.OnFormElementSelected += (oldFormParam) =>
                _oldConnectionFormMaker.AddAnFormParam(oldFormParam);
        }

        private void HandleValidationButton()
        {
            ConnectionFormDto connectionFormDto = new ConnectionFormDto();
            StringParameterDto stringParameterDto = new StringParameterDto();
            stringParameterDto.value = "hey";
            stringParameterDto.name = "Name";
            connectionFormDto.fields.Add(stringParameterDto);

            mainContainer.ToolAccessProcessForm(connectionFormDto);
        }
    }
}
