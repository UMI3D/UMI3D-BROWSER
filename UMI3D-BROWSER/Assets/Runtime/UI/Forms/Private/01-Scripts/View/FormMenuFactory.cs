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
using System.Collections.Generic;
using umi3d.browserRuntime.forms;
using umi3dBrowsers.container;
using UnityEngine;

namespace umi3d.browserRuntime.ui.formMenu
{
    public abstract class FormMenuFactory : MonoBehaviour, IFormMenuDisplayFormObserver
    {
        [SerializeField] internal Transform _content;
        [SerializeField] internal TabManager _tabManager;

        [Header("Factories")]
        [SerializeField] internal FormTextFactory _textFactory;

        [SerializeField] internal ButtonFactory _buttonFactory;
        protected List<IButtonBuilder> _buttonBuilders = new();

        [SerializeField] internal DropdownFactory _dropdownFactory;
        protected List<IDropdownBuilder> _dropdownBuilder = new();

        [SerializeField] internal InputFieldFactory _inputFieldFactory;
        protected List<IInputFieldBuilder> _inputFieldBuilders = new();

        [SerializeField] internal SliderFactory _sliderFactory;
        protected List<ISliderBuilder> _sliderBuilders = new();

        [SerializeField] internal ToggleFactory _toggleFactory;
        protected List<IToggleBuilder> _toggleBuilders = new();

        Notifier _sendAnswerNotifier;
        protected abstract object formAnswerDtoObject { get; }

        protected FormMenuController _controller;
        protected FormMenuModel _model;

        protected virtual void Awake()
        {
            _controller = GetComponentInParent<FormMenuController>();
            _model = _controller.model;
        }

        public abstract void DisplayForm(common.interaction.form.FormDto formDto);

        public abstract void DisplayLegacyForm(common.interaction.ConnectionFormDto legacyFormDto);

        protected void SendAnswer()
        {
            _sendAnswerNotifier[FormNotificationKeys.SendAnswer.FormAnswerDto] = formAnswerDtoObject;
            _sendAnswerNotifier.Notify();

            Clear();
        }

        protected virtual void Clear()
        {
            DestroyChildren();
            _tabManager.Clear();
            ResetFormAnser();
        }

        void ReturnElements()
        {
            foreach (var builder in _buttonBuilders)
            {
                builder.Clear();
            }
            _buttonBuilders.Clear();

            foreach (var builder in _dropdownBuilder) 
            {
                builder.Clear(_model);
            }
            _dropdownBuilder.Clear();

            foreach (var builder in _inputFieldBuilders) 
            {
                builder.Clear(_model);
            }
            _inputFieldBuilders.Clear();

            foreach (var builder in _toggleBuilders)
            {
                builder.Clear(_model);
            }
            _toggleBuilders.Clear();
        }

        protected void DestroyChildren()
        {
            for (int i = _content.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(_content.GetChild(i).gameObject);
#else
                Destroy(_content.GetChild(i).gameObject);
#endif
            }
        }
        protected abstract void ResetFormAnser();
    }
}