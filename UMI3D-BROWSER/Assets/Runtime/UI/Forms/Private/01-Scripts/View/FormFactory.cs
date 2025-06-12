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

using inetum.unityUtils.observation;
using umi3d.browserRuntime.forms;
using umi3d.common.interaction.form;
using UnityEngine;

namespace umi3d.browserRuntime.ui.formMenu
{
    public class FormFactory : FormMenuFactory
    {
        [SerializeField] internal FormGroupFactory _groupFactory;
        [SerializeField] internal FormImageFactory _imageFactory;
        [SerializeField] internal FormThumbnailFactory _thumbnailFactory;

        internal FormAnswerDto _formAnswerDto = new FormAnswerDto();
        protected override object formAnswerDtoObject => _formAnswerDto;

        public override void DisplayForm(FormDto formDto)
        {
            if (!_content) { _content = transform; }

            if (_content.childCount > 0) { Clear(); }

            _formAnswerDto.formId = formDto.guid;

            if (formDto.FirstChildren != null)
            {
                foreach (var child in formDto.FirstChildren)
                {
                    AddDiv(child, new Container(_content));
                }
            }
        }

        async void AddDiv(DivDto divDto, Container container)
        {
            if (divDto == null || container == null) { return; }

            void BuildInputField(IInputFieldBuilder builder)
            {
                builder.Build(container.Transform);
                builder.BuildLabel();
                builder.BuildPlaceholder();
                builder.BuildContentType();
                builder.BuildLine();
                builder.BuildValue();
                builder.GetControl();
                _inputFieldBuilders.Add(builder);

            }

            void BuildSlider(ISliderBuilder builder)
            {
                builder.Build(container.Transform);
                builder.BuildLabel();
                builder.BuildRange();
                builder.BuildValue();
                builder.GetControl();
                _sliderBuilders.Add(builder);
            }

            // TODO: Dropdown
            switch (divDto)
            {
                case GroupDto groupDto:
                    {
                        var group = _groupFactory.CreateGroup(groupDto, container.Transform);
                        if (groupDto.FirstChildren != null)
                            foreach (var child in groupDto.FirstChildren)
                                AddDiv(child, new(group.transform));
                        break;
                    }

                case InputDto<string> inputStringDto:
                    {
                        FormInputFieldBuilder<string> builder = new(_inputFieldFactory, inputStringDto, _formAnswerDto);
                        BuildInputField(builder);
                        builder.BuildStyle();
                        break;
                    }

                case InputDto<int> inputIntDto:
                    {
                        FormInputFieldBuilder<int> builder = new(_inputFieldFactory, inputIntDto, _formAnswerDto);
                        BuildInputField(builder);
                        builder.BuildStyle();
                        break;
                    }

                case RangeDto<int> rangeIntDto:
                    {
                        FormSliderBuilder<int> builder = new(_sliderFactory, rangeIntDto, _formAnswerDto);
                        BuildSlider(builder);
                        builder.BuildStyle();
                        break;
                    }

                case RangeDto<float> rangeFloatDto:
                    {
                        FormSliderBuilder<float> builder = new(_sliderFactory, rangeFloatDto, _formAnswerDto);
                        BuildSlider(builder);
                        builder.BuildStyle();
                        break;
                    }

                case ButtonDto buttonDto:
                    {
                        FormButtonBuilder builder = new(_buttonFactory, buttonDto);
                        builder.Build(container.Transform);
                        builder.BuildLabel();
                        builder.BuildImage();
                        builder.BuildCallback(() =>
                        {
                            switch (buttonDto.buttonType)
                            {
                                case ButtonType.Submit:
                                    SendAnswer(buttonDto.guid);
                                    break;
                                case ButtonType.Back:
                                    _formAnswerDto.isBack = true;
                                    SendAnswer(buttonDto.guid);
                                    break;
                                case ButtonType.Cancel:
                                    NotificationHub.Default.Notify(this, ID.FromType<FormNotificationKeys.Cancel>());
                                    break;
                            }
                        });
                        builder.GetControl();
                        _buttonBuilders.Add(builder);
                        break;
                    }

                case LabelDto labelDto:
                    {
                        _textFactory.CreateText(labelDto, container.Transform);
                        break;
                    }

                case ImageDto imageDto:
                    {
                        if (imageDto.FirstChildren == null || imageDto.FirstChildren.Count == 0) // Normal Image
                            await _imageFactory.CreateImage(imageDto, container.Transform);
                        else // Thumbnail
                            await _thumbnailFactory.CreateThumbnail(imageDto, container, SendAnswer);
                        break;
                    }

                case PageDto pageDto:
                    {
                        var content = _tabManager.AddNewTab(pageDto.name, false, () => _formAnswerDto.pageId = pageDto.guid);
                        Container pageContainer = new(content.transform);
                        if (pageDto.FirstChildren != null)
                            foreach (var child in pageDto.FirstChildren)
                                if (child != null)
                                    AddDiv(child, pageContainer);
                        _tabManager.InitSelectedButtonById();
                        break;
                    }

                default:
                    {
                        Debug.LogError("[FormFactory] Div Dto not supported!");
                        break;
                    }
            }
        }

        void SendAnswer(string submitId)
        {
            _formAnswerDto.submitId = submitId;
            SendAnswer();
        }

        internal static T ReplaceContainerWithPrefab<T>(Container container, T prefab) where T : MonoBehaviour
        {
            var newObject = Instantiate(prefab);
            newObject.transform.SetParent(container.Transform.parent, false);
            foreach (Transform child in container.Transform)
                child.SetParent(newObject.transform, false);

#if UNITY_EDITOR
            DestroyImmediate(container.Transform.gameObject);
#else
            Destroy(container.Transform.gameObject);
#endif
            container.Transform = newObject.transform;

            return newObject;
        }

        public override void DisplayLegacyForm(common.interaction.ConnectionFormDto legacyFormDto)
        {
        }

        protected override void ResetFormAnser()
        {
            _formAnswerDto = new();
        }

        internal class Container
        {
            public Transform Transform { get; set; }

            public Container(Transform content)
            {
                this.Transform = content;
            }
        }
    }
}