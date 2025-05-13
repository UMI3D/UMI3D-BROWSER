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

using System;
using TMPro;
using umi3d.browserRuntime.ui.inputField;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    public class SimpleInputFieldBuilder : InputFieldBuilder
    {
        public InputFieldFactory factory {  get; private set; }

        GameObject control;
        InputFieldModel model;

        string label;
        string value;
        string placeholder;
        bool isMultiLine;
        int nbLine;
        TMP_InputField.ContentType contentType;
        bool isPin;


        public SimpleInputFieldBuilder(
            InputFieldFactory factory, 
            string label, 
            string value, 
            string placeholder, 
            bool isMultiLine, 
            int nbLine, 
            TMP_InputField.ContentType contentType, 
            bool isPin
        )
        {
            this.factory = factory;
            this.label = label;
            this.value = value;
            this.placeholder = placeholder;
            this.isMultiLine = isMultiLine;
            this.nbLine = nbLine;
            this.contentType = contentType;
            this.isPin = isPin;
        }

        public void Build(Transform parent)
        {
            if (isMultiLine)
            {
                factory.TryToGetOrCreateMultiLine(out control, out model, parent);
            }
            else
            {
                factory.TryToGetOrCreateSingleLine(out control, out model, parent);
            }
        }

        public void BuildContentType()
        {
            model.SetContentType(contentType);
            model.SetIsPin(isPin);
        }

        public void BuildLabel()
        {
            model.SetLabel(label);
        }

        public void BuildLine()
        {
            if (nbLine < 1) { nbLine = 1; }
            if (nbLine != 1) { model.SetNbrLines(isMultiLine, nbLine); }
        }

        public void BuildPlaceholder()
        {
            model.SetPlaceholder(placeholder);
        }

        public void BuildValue()
        {
            model.SetValue(value);
        }

        public GameObject GetControl()
        {
            control.SetActive(true);
            return control;
        }
    }
}