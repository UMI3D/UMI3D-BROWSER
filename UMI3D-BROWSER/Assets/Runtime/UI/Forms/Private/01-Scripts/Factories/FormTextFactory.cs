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

using umi3d.browserRuntime.text;
using umi3d.common.interaction;
using umi3d.common.interaction.form;
using UnityEngine;

namespace umi3d.browserRuntime.forms
{
    [RequireComponent(typeof(TextFactory)), ExecuteAlways]
    internal class FormTextFactory : MonoBehaviour
    {
        private TextFactory _textFactory;

        private void Awake()
        {
            _textFactory = GetComponent<TextFactory>();
        }

        public GameObject CreateText(LabelDto labelDto, Transform parent)
        {
            var style = labelDto.GetStyle();

            var textGameObject = _textFactory.GetOrCreateText(parent, labelDto.text);

            var formItemModelContainer = textGameObject.GetComponent<FormItemModelContainer>();

            formItemModelContainer.Model.SetPosition(style.Position);
            formItemModelContainer.Model.SetSize(style.Size);
            formItemModelContainer.Model.SetAnchor(style.AnchorMin, style.AnchorMax, style.Pivot);
            formItemModelContainer.Model.SetTextStyle(style.FontSize, style.FontColor, style.FontStyles, style.FontAlignmentOptions);

            return textGameObject;
        }

        public GameObject CreateText(StringParameterDto stringDto, Transform parent)
        {
            return _textFactory.GetOrCreateText(parent, stringDto.name);
        }
    }
}