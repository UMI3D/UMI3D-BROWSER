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
using System;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.dropdown
{
    public class FormDropdownFactory : DropdownFactory
    {
        protected override void Awake()
        {
            base.Awake();

            _formCreateBehaviour = new FormCreateBehaviour(this);
            _simpleCreateBehaviour = new SimpleCreateBehaviour(this);
        }
    }

    public class FormCreateBehaviour : IFormCreateBehaviour
    {
        DropdownFactory dropdownFactory;

        public FormCreateBehaviour(DropdownFactory dropdownFactory)
        {
            this.dropdownFactory = dropdownFactory;
        }

        public bool TryToGetOrCreate(out GameObject control, Transform parent, EnumParameterDto<string> dto, FormAnswerDto formAnswerDto)
        {
            var succeeded = dropdownFactory.TryToGetOrCreate(out control, parent, dto.name, dto.possibleValues, dto.value);
            if (!succeeded) { return false; }

            if (formAnswerDto.answers == null)
            {
                formAnswerDto.answers = new(); 
            }
            ParameterSettingRequestDto paramRequestDto = new ParameterSettingRequestDto() { id = dto.id };
            formAnswerDto.answers.Add(paramRequestDto);

            var modelContainer = control.GetComponent<DropdownController>();
            NotificationHub.Default.Subscribe(this,
                ID.FromType<DropdownNotificationKeys.DropdownUpdated>(),
                (Callback)UpdateAnswer,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == modelContainer.model));

            return true;

            void UpdateAnswer(Notification notification)
            {
                if (notification.TryGetInfoT(DropdownNotificationKeys.DropdownUpdated.Value, out string value))
                    paramRequestDto.parameter = value;
            }
        }
    }
}