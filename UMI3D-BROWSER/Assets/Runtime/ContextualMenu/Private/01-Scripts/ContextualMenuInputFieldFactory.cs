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
using umi3d.browserRuntime.ui.inputField;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    [RequireComponent(typeof(InputFieldFactory))]
    public class ContextualMenuInputFieldFactory : MonoBehaviour
    {
        InputFieldFactory _inputFieldFactory;

        private void Awake()
        {
            _inputFieldFactory = GetComponent<InputFieldFactory>();
        }

        public GameObject GetOrCreate(Transform parent, StringParameterDto dto)
        {
            var inputFieldGameobject = _inputFieldFactory.GetOrCreateInputField(parent, dto.IsMultiLine);
            var model = inputFieldGameobject.GetComponent<InputFieldParameterModelContainer>().parameterModel;
            model.SetDto(dto);

            NotificationHub.Default.Subscribe(inputFieldGameobject,
                ID.FromType<ContextualMenuNotificationKeys.Submit>(),
                (Callback)model.Submit);

            return inputFieldGameobject;
        }

        public void Return(GameObject inputFieldModelContainer)
        {
            NotificationHub.Default.Unsubscribe(inputFieldModelContainer);
            var model = inputFieldModelContainer.GetComponent<InputFieldParameterModelContainer>().parameterModel;
            if (model != null)
                model.ReleaseDto();
            _inputFieldFactory.Return(inputFieldModelContainer);
        }
    }
}