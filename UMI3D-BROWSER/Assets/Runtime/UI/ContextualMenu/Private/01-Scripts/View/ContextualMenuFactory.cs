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
using NUnit.Framework;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    internal class ContextualMenuFactory : MonoBehaviour
    {
        [SerializeField] Transform _content;

        DropdownFactory _dropdownFactory;
        List<GameObject> _dropdowns = new();

        SliderFactory _sliderFactory;
        List<GameObject> _sliders = new();

        ButtonFactory _buttonFactory;
        List<GameObject> _buttons = new();

        ToggleFactory _toggleFactory;
        List<GameObject> _toggles = new();

        InputFieldFactory _inputFieldFactory;
        List<GameObject> _inputFields = new();

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

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ContextualMenuNotificationKeys.AddParameter>(), 
                (Callback)AddParameter);
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        public void AddParameter(Notification notification)
        {
            if (!notification.TryGetInfoT(ContextualMenuNotificationKeys.AddParameter.Parameter, out AbstractParameterDto parameter))
                return;

            switch (parameter)
            {
                case StringParameterDto stringParameter:
                    {
                        LegacyContextualMenuInputFieldBuilder builder = new(_inputFieldFactory, stringParameter);
                        builder.Build(_content);
                        builder.BuildLabel();
                        builder.BuildPlaceholder();
                        builder.BuildValue();
                        builder.BuildLine();
                        builder.BuildContentType();
                        // TODO: Build Submit
                        _inputFields.Add(builder.GetControl());
                        break;
                    }

                case BooleanParameterDto booleanParameter:
                    {
                        LegacyContextualMenuToggleBuilder builder = new(_toggleFactory, booleanParameter);
                        builder.Build(_content);
                        builder.BuildLabel();
                        builder.BuildValue();
                        // TODO: Build Submit
                        _toggles.Add(builder.GetControl());
                        break;
                    }

                case FloatRangeParameterDto floatRangeParameter:
                    {
                        LegacyContextualMenuSliderBuilder<float> builder = new(_sliderFactory, floatRangeParameter);
                        builder.Build(_content);
                        builder.BuildLabel();
                        builder.BuildRange();
                        builder.BuildValue();
                        // TODO: Build Submit
                        _sliders.Add(builder.GetControl());
                        break;
                    }

                case IntegerRangeParameterDto intRangeParameter:
                    {
                        LegacyContextualMenuSliderBuilder<int> builder = new(_sliderFactory, intRangeParameter);
                        builder.Build(_content);
                        builder.BuildLabel();
                        builder.BuildRange();
                        builder.BuildValue();
                        // TODO: Build Submit
                        _sliders.Add(builder.GetControl());
                        break;
                    }

                case EnumParameterDto<string> stringEnumParameter:
                    {
                        LegacyContextualMenuDropdownBuilder builder = new(_dropdownFactory, stringEnumParameter);
                        builder.Build(_content);
                        builder.BuildLabel();
                        builder.BuildOptions();
                        builder.BuildValue();
                        builder.BuildSubmit(_model, () =>
                        {
                            UMI3DClientServer.SendRequest(new ParameterSettingRequestDto()
                            {
                                id = parameter.id,
                                parameter = parameter,
                            }, true);
                        });
                        _dropdowns.Add(builder.GetControl());
                        break;
                    }
            }
        }
    }
}