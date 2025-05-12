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
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.dropdown
{
    public class ContextualMenuDropdownFactory : DropdownFactory
    {
        protected override void Awake()
        {
            base.Awake();

            _simpleCreateBehaviour = new SimpleCreateBehaviour(this);
            _contextMenuCreateBehaviour = new ContextualMenuCreateBehaviour(this);
        }
    }

    public class ContextualMenuCreateBehaviour : IContextualMenuCreateBehaviour
    {
        DropdownFactory _dropdownFactory;

        public ContextualMenuCreateBehaviour(DropdownFactory dropdownFactory)
        {
            _dropdownFactory = dropdownFactory;
        }

        public bool TryToGetOrCreate(out GameObject control, Transform parent, EnumParameterDto<string> dto)
        {
            var succeeded = _dropdownFactory.TryToGetOrCreate(out control, parent, dto.name, dto.possibleValues, dto.value);
            if (!succeeded) { return false; }

            var controller = control.GetComponent<DropdownController>();

            controller.submitted += () =>
            {
                IDropdownModel model = controller.model;
                dto.value = model.value;
                UMI3DClientServer.SendRequest(new ParameterSettingRequestDto() {
                    id = dto.id,
                    parameter = dto,
                }, true);
            };

            // TODO
            //NotificationHub.Default.Subscribe(this,
            //    ID.FromType<ContextualMenuNotificationKeys.Submit>(),
            //    (Callback)controller.Submit);

            return true;
        }

        public bool TryToGetOrCreate(out GameObject control, Transform parent, EnumParameterDto<string> dto, IParameterInputSystem<string> inputSystem)
        {
            var succeeded = _dropdownFactory.TryToGetOrCreate(out control, parent, dto.name, dto.possibleValues, dto.value);
            if (!succeeded) { return false; }

            var controller = control.GetComponent<DropdownController>();

            controller.submitted += () =>
            {
                IDropdownModel model = controller.model;
                inputSystem.Perform(model.value);
            };

            // TODO
            //NotificationHub.Default.Subscribe(this,
            //    ID.FromType<ContextualMenuNotificationKeys.Submit>(),
            //    (Callback)controller.Submit);

            return true;
        }
    }
}