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
using umi3d.browserRuntime.ui;
using umi3d.common.interaction.form;
using UnityEngine;

namespace umi3d.browserRuntime.forms
{
    public class FormInputFieldBuilder<T> : IInputFieldBuilder
    {
        public InputFieldFactory factory { get; private set; }

        GameObject control;
        InputFieldModel model;

        InputDto<T> dto;
        FormAnswerDto formAnswerDto;
        InputAnswerDto answerDto;

        public FormInputFieldBuilder(InputFieldFactory factory, InputDto<T> dto, FormAnswerDto formAnswerDto)
        {
            this.factory = factory;
            this.dto = dto;
            this.formAnswerDto = formAnswerDto;

            if (formAnswerDto.inputs == null)
            {
                formAnswerDto.inputs = new();
            }
            answerDto = new InputAnswerDto() { inputId = dto.guid, value = dto.Value };
            formAnswerDto.inputs.Add(answerDto);
            model.valueChanged += value => answerDto.value = value;
        }

        public void Build(Transform parent)
        {
            factory.TryToGetOrCreateSingleLine(out control, out model, parent);
            control.transform.SetParent(parent, false);
        }

        public void BuildStyle()
        {
            var style = dto.GetStyle();

            var formItemController = control.GetComponent<FormItemModelContainer>();
            FormItemModel model = formItemController.Model;

            model.SetPosition(style.Position);
            model.SetSize(style.Size);
            model.SetAnchor(style.AnchorMin, style.AnchorMax, style.Pivot);
            model.SetTextStyle(style.FontSize, style.FontColor, style.FontStyles, style.FontAlignmentOptions);
        }

        public void BuildContentType()
        {
            (var contentType, var isPin) = TmpContentTypeFrom(dto.TextType);
            model.SetContentType(contentType);
            model.SetIsPin(isPin);
        }

        public void BuildLabel()
        {
            if (!string.IsNullOrEmpty(dto.Name))
            {
                model.SetLabel(dto.Name);
            }
        }

        public void BuildLine()
        {
            model.SetNbrLines(false, 1);
        }

        public void BuildPlaceholder()
        {
            model.SetPlaceholder(dto.PlaceHolder?.ToString());
        }

        public void BuildValue()
        {
            if (!string.IsNullOrEmpty(dto.Value?.ToString()))
            {
                model.SetValue(dto.Value?.ToString());
            }
        }

        public void BuildValueChange(Action<string> action)
        {
            model.valueChanged += action;
        }

        public GameObject GetControl()
        {
            control.SetActive(true);
            return control;
        }

        static (TMP_InputField.ContentType, bool) TmpContentTypeFrom(TextType type)
        {
            switch (type)
            {
                case TextType.Text:
                    return (TMP_InputField.ContentType.Standard, false);
                case TextType.Mail:
                    return (TMP_InputField.ContentType.EmailAddress, false);
                case TextType.Password:
                    return (TMP_InputField.ContentType.Password, false);
                case TextType.Phone:
                    return (TMP_InputField.ContentType.IntegerNumber, false);
                case TextType.URL:
                    return (TMP_InputField.ContentType.Standard, false);
                case TextType.Number:
                    return (TMP_InputField.ContentType.IntegerNumber, false);
                case TextType.Pin:
                    return (TMP_InputField.ContentType.IntegerNumber, true);
            }
            return (TMP_InputField.ContentType.Standard, false);
        }

        public void BuildSubmit(ISubmitSubject subject, Action onSubmit)
        {
            subject.Subscribe(model);
            model.submit += onSubmit;
        }

        public void Clear(ISubmitSubject subject)
        {
            subject?.Unsubscribe(model);
            factory.Return(control);
            control = null;
        }
    }
}