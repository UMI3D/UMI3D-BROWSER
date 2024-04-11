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
    public class UMI3DBuildToolPanelView 
    {
        public VisualElement root;

        public Button B_Back;
        public Label L_Title;

        // ViewModel
        public Action backHandler;
        public Action<string> changeTitleHandler;

        public void SetTitle(string title)
        {
            changeTitleHandler?.Invoke(title);
        }

        public UMI3DBuildToolPanelView(VisualElement root)
        {
            this.root = root;
        }

        public void Bind()
        {
            B_Back = root.Q<Button>("B_Back");
            L_Title = root.Q<Label>("L_Title");
        }

        public void Set()
        {
            B_Back.clicked += backHandler;
            changeTitleHandler += TitleChanged;
        }

        public void Unbind()
        {
            B_Back.clicked -= backHandler;
            changeTitleHandler -= TitleChanged;
        }

        void TitleChanged(string title)
        {
            L_Title.text = title;
        }
    }
}