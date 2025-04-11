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
using NUnit.Framework;
using System.Collections.Generic;
using umi3d.common.interaction;
using umi3dBrowsers.container;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.forms
{
    public class FormParamFactory : MonoBehaviour
    {
        
        [SerializeField] internal Transform _content;
        [SerializeField] internal Button _submitButton;
        [SerializeField] internal FormDropdownFactory _dropdownFactory;
        [SerializeField] internal FormInputFieldFactory _inputFieldFactory;
        [SerializeField] internal FormTextFactory _textFactory;
        [SerializeField] internal TabManager _tabManager;

        internal FormAnswerDto _formAnswerDto = new FormAnswerDto();
        private Notifier _sendAnswerNotifier;

        private void Awake()
        {
            _sendAnswerNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<FormNotificationKeys.SendAnswer>());

            _submitButton.onClick.AddListener(SendAnswer);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<FormNotificationKeys.CreateForm>(),
                (Callback)CreateForm);
        }

        private void OnDestroy()
        {
            _submitButton.onClick.RemoveListener(SendAnswer);
        }

        private void CreateForm(Notification notification)
        {
            if (notification.TryGetInfoT(FormNotificationKeys.CreateForm.FormDto, out ConnectionFormDto connectionFormDto, false))
                CreateForm(connectionFormDto);
        }

        public void CreateForm(ConnectionFormDto connectionFormDto)
        {
            if (!_content)
                _content = transform;

            if (_content.childCount > 0)
                Clear();

            _formAnswerDto.id = connectionFormDto.id;

            if (connectionFormDto.fields != null)
                AddFields(connectionFormDto.fields);

            _submitButton.gameObject.SetActive(true);
        }

        internal void AddFields(List<AbstractParameterDto> parameterDtos)
        {
            var container = _tabManager.AddNewTabForParamForm(parameterDtos[0].name, false).transform;

            for (int i = 0; i < parameterDtos.Count; i++)
            {
                var parameterDto = parameterDtos[i];
                switch (parameterDto)
                {
                    case EnumParameterDto<string> enumDto:
                        _dropdownFactory.CreateDropdown(enumDto, container, _formAnswerDto);
                        break;
                    case StringParameterDto stringDto:
                        if (parameterDto.name == "OR" && parameterDto.tag == null)
                            break;
                        if (parameterDto.isDisplayer)
                            _textFactory.CreateText(stringDto, container);
                        else
                            _inputFieldFactory.CreateInputField(stringDto, container, _formAnswerDto);
                            break;
                    default:
                        break;
                }

                if (parameterDto.name == "OR" && parameterDto.tag == null)
                    container = _tabManager.AddNewTabForParamForm(parameterDtos[i+1].name, false).transform;
            }
        }

        internal void SendAnswer()
        {
            _sendAnswerNotifier[FormNotificationKeys.SendAnswer.FormAnswerDto] = _formAnswerDto;
            _sendAnswerNotifier.Notify();

            Clear();
        }

        public void Clear()
        {
#if UNITY_EDITOR
            for (int i = _content.childCount - 1; i >= 0; i--)
                DestroyImmediate(_content.GetChild(i).gameObject);
            _tabManager.Clear();
#else
            for (int i = _content.childCount - 1; i >= 0; i--)
                Destroy(_content.GetChild(i).gameObject);
            _tabManager.Clear();
#endif
            _formAnswerDto = new();
            _submitButton.gameObject.SetActive(false);
        }
    }
}