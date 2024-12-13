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

using umi3d.browserRuntime.inputField;
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
            inputFieldGameobject.GetComponent<InputFieldParameter>().parameterModel.SetDto(dto);
            return inputFieldGameobject;
        }

        public void Return(GameObject inputFieldModelContainer)
        {
            _inputFieldFactory.Return(inputFieldModelContainer);
        }
    }
}