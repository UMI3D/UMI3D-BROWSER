/*
Copyright 2019 - 2025 Inetum

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

using inetum.unityUtils.ui.canvas;
using UnityEngine.UI;

namespace umi3d.browserRuntime.interactions
{
    internal class NameView : Image, IView, IInteractableUIDelegate
    {
        TMPro.TMP_Text textTMP;

        InteractableUIModel model;

        IView view => this;

        protected override void Awake()
        {
            base.Awake();

            view.Set(ref textTMP, 0);
        }

        void SetText(string text)
        {
            if (!view.Set(ref textTMP, 0)) 
            {
                throw new System.Exception("Text view cannot be set."); 
            }

            textTMP.text = text;
        }

        public void SetModel(InteractableUIModel model)
        {
            if (this.model != null)
            {
                this.model.delegates.Remove(this);
            }

            this.model = model;
            this.model.delegates.Add(this);

            SetText(model.dataDelegate?.interactableName);
        }

        public void OnChangeOfInteractableName(string oldName, string newName)
        {
            SetText(newName);
        }
    }
}