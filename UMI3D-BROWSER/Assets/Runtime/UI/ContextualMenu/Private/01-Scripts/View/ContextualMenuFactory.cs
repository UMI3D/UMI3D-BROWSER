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

using inetum.unityUtils.observation;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    internal class ContextualMenuFactory : MonoBehaviour, IContextualMenuActivationObserver, IContextualMenuDisplayParameterObserver
    {
        [SerializeField] Transform _content;

        ButtonFactory _buttonFactory;
        List<GameObject> _buttons = new();

        DropdownFactory _dropdownFactory;
        List<IDropdownBuilder> _dropdownBuilders = new();

        SliderFactory _sliderFactory;
        List<ISliderBuilder> _sliderBuilders = new();

        ToggleFactory _toggleFactory;
        List<IToggleBuilder> _toggleBuilders = new();

        InputFieldFactory _inputFieldFactory;
        List<IInputFieldBuilder> _inputFieldBuilders = new();

        ContextualMenuController _controller;
        ContextualMenuModel _model;

        private void Awake()
        {
            _dropdownFactory = GetComponent<DropdownFactory>();
            _sliderFactory = GetComponent<SliderFactory>();
            _buttonFactory = GetComponent<ButtonFactory>();
            _toggleFactory = GetComponent<ToggleFactory>();
            _inputFieldFactory = GetComponent<InputFieldFactory>();

            _controller = GetComponentInParent<ContextualMenuController>();
            _model = _controller.model;
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        public void Display(AbstractParameterDto parameter, Projection projection)
        {
            void SendParameterRequest()
            {
                UMI3DClientServer.SendRequest(new ParameterSettingRequestDto()
                {
                    id = parameter.id,
                    parameter = parameter,
                }, true);
            }

            void BuildSlider(ISliderBuilder builder)
            {
                builder.Build(_content);
                builder.BuildLabel();
                builder.BuildRange();
                builder.BuildValue();
                builder.BuildSubmit(_model, () =>
                {
                    SendParameterRequest();
                });
                builder.GetControl();
                _sliderBuilders.Add(builder);
            }

            switch (parameter)
            {
                case StringParameterDto stringParameter:
                    {
                        ContextualMenuInputFieldBuilder builder = new(_inputFieldFactory, stringParameter);
                        builder.Build(_content);
                        builder.BuildLabel();
                        builder.BuildPlaceholder();
                        builder.BuildValue();
                        builder.BuildLine();
                        builder.BuildContentType();
                        builder.BuildSubmit(_model, () =>
                        {
                            SendParameterRequest();
                        });
                        builder.GetControl();
                        _inputFieldBuilders.Add(builder);
                        break;
                    }

                case BooleanParameterDto booleanParameter:
                    {
                        ContextualMenuToggleBuilder builder = new(_toggleFactory, booleanParameter);
                        builder.Build(_content);
                        builder.BuildLabel();
                        builder.BuildValue();
                        builder.BuildSubmit(_model, () =>
                        {
                            SendParameterRequest();
                        });
                        builder.GetControl();
                        _toggleBuilders.Add(builder);
                        break;
                    }

                case FloatRangeParameterDto floatRangeParameter:
                    {
                        ContextualMenuSliderBuilder<float> builder = new(_sliderFactory, floatRangeParameter);
                        BuildSlider(builder);
                        break;
                    }

                case IntegerRangeParameterDto intRangeParameter:
                    {
                        ContextualMenuSliderBuilder<int> builder = new(_sliderFactory, intRangeParameter);
                        BuildSlider(builder);
                        break;
                    }

                case EnumParameterDto<string> stringEnumParameter:
                    {
                        ContextualMenuDropdownBuilder builder = new(_dropdownFactory, stringEnumParameter);
                        builder.Build(_content);
                        builder.BuildLabel();
                        builder.BuildOptions();
                        builder.BuildValue();
                        builder.BuildSubmit(_model, () =>
                        {
                            SendParameterRequest();
                        });
                        builder.GetControl();
                        _dropdownBuilders.Add(builder);
                        break;
                    }
            }
        }

        public void UpdateActivation(bool isActive)
        {
            if (!isActive) { Clear(); }
        }

        void Clear()
        {
            foreach (var dropdownBuilder in _dropdownBuilders)
            {
                dropdownBuilder.Clear(_model);
            }
            _dropdownBuilders.Clear();

            foreach (var inputFieldBuilder in _inputFieldBuilders)
            {
                inputFieldBuilder.Clear(_model);
            }
            _inputFieldBuilders.Clear();

            foreach (var sliderBuilder in _sliderBuilders)
            {
                sliderBuilder.Clear(_model);
            }
            _sliderBuilders.Clear();

            foreach(var toggleBuilder in _toggleBuilders)
            {
                toggleBuilder.Clear(_model);
            }
            _toggleBuilders.Clear();
        }
    }
}