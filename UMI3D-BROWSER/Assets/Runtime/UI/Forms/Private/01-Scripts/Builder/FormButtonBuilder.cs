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
using umi3d.browserRuntime.ui;
using umi3d.common.interaction.form;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.forms
{
    public class FormButtonBuilder : IButtonBuilder
    {
        public ButtonFactory factory {  get; private set; }

        GameObject control;
        ButtonModel model;

        ButtonDto dto;

        public FormButtonBuilder(ButtonFactory factory, ButtonDto dto)
        {
            this.factory = factory;
            this.dto = dto;
        }

        public void Build(Transform parent)
        {
            factory.TryToGetOrCreate(out control, out model, parent);
        }

        public async void BuildImage()
        {
            var style = dto.GetStyle();

            var colors = new ColorBlock();
            if (style.Color.HasValue)
            {
                colors.normalColor = style.Color.Value;
                colors.highlightedColor = style.Color.Value;
                colors.pressedColor = style.Color.Value;
                colors.selectedColor = style.Color.Value;
            }
            if (style.HoverColor.HasValue)
            {
                colors.highlightedColor = style.HoverColor.Value;
                colors.pressedColor = style.HoverColor.Value;
            }

            model.SetImage(colors, await dto.GetSprite());

            var formItemController = control.GetComponent<FormItemModelContainer>();
            FormItemModel formItemModel = formItemController.Model;

            formItemModel.SetPosition(style.Position);
            formItemModel.SetSize(style.Size);
            formItemModel.SetAnchor(style.AnchorMin, style.AnchorMax, style.Pivot);
            formItemModel.SetTextStyle(style.FontSize, style.FontColor, style.FontStyles, style.FontAlignmentOptions);
        }

        public void BuildLabel()
        {
            model.SetLabel(dto.Text);
        }

        public GameObject GetControl()
        {
            control.SetActive(true);
            return control;
        }

        public void BuildCallback(Action callback)
        {
            model.SetCallback(callback);
        }

        public void Clear()
        {
            factory.Return(control);
            control = null;
        }
    }
}