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
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    [RequireComponent(typeof(ContextualMenuInputFieldFactory))]
    [RequireComponent(typeof(ContextualMenuToggleFactory))]
    [RequireComponent(typeof(ContextualMenuSliderFactory))]
    [RequireComponent(typeof(ContextualMenuDropdownFactory))]
    public class ContextualMenuFactory : MonoBehaviour
    {
        [SerializeField] Transform _content;

        ContextualMenuInputFieldFactory _inputFieldFactory;
        List<GameObject> _inputFields;
        ContextualMenuToggleFactory _toggleFactory;
        List<GameObject> _toggles;
        ContextualMenuSliderFactory _sliderFactory;
        List<GameObject> _sliders;
        ContextualMenuDropdownFactory _dropdownFactory;
        List<GameObject> _dropdowns;

        private void Awake()
        {
            _inputFieldFactory = GetComponent<ContextualMenuInputFieldFactory>();
            _inputFields = new List<GameObject>();
            _toggleFactory = GetComponent<ContextualMenuToggleFactory>();
            _toggles = new List<GameObject>();
            _sliderFactory = GetComponent<ContextualMenuSliderFactory>();
            _sliders = new List<GameObject>();
            _dropdownFactory = GetComponent<ContextualMenuDropdownFactory>();
            _dropdowns = new List<GameObject>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ContextualMenuNotificationKeys.AddParameter>(), 
                (Callback)AddParameter);
            NotificationHub.Default.Subscribe(this,
                ID.FromType<ContextualMenuNotificationKeys.Close>(), 
                (Callback)Clean);
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
                    _inputFields.Add(_inputFieldFactory.GetOrCreate(_content, stringParameter));
                    break;
                }
                case BooleanParameterDto booleanParameter:
                {
                    _toggles.Add(_toggleFactory.GetOrCreate(_content, booleanParameter));
                    break;
                }
                case FloatRangeParameterDto floatRangeParameter:
                {
                    _sliders.Add(_sliderFactory.GetOrCreate(_content, floatRangeParameter));
                    break;
                }
                case IntegerRangeParameterDto intRangeParameter:
                {
                    _sliders.Add(_sliderFactory.GetOrCreate(_content, intRangeParameter));
                    break;
                }
                case EnumParameterDto<string> stringEnumParameter:
                {
                    _dropdowns.Add(_dropdownFactory.GetOrCreate(_content, stringEnumParameter));
                    break;
                }
            }
        }

        public void Clean()
        {
            foreach (var inputField in _inputFields)
                _inputFieldFactory.Return(inputField);
            foreach (var toggle in _toggles)
                _toggleFactory.Return(toggle);
            foreach (var slider in _sliders)
                _sliderFactory.Return(slider);
            foreach (var dropdown in _dropdowns)
                _dropdownFactory.Return(dropdown);
        }
    }
}