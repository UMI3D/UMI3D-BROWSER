/*
Copyright 2019 - 2023 Inetum

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

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui
{
    [RequireComponent(typeof(UMI3DSelectable))]
    [RequireComponent(typeof(UMI3DKeyboardBinderLegacy))]
    public class UMI3DInputFieldLegacy : InputField
    {
        public UMI3DSelectable selectable;
        public UMI3DKeyboardBinderLegacy keyboardBinder;

        protected override void Start()
        {
            base.Start();
            selectable = GetComponent<UMI3DSelectable>();
            keyboardBinder = GetComponent<UMI3DKeyboardBinderLegacy>();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            selectable.OnSelectEvent?.Invoke();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            selectable.OnDeselectEvent?.Invoke();
            if (keyboardBinder.keyboard != null)
            {
                keyboardBinder.keyboard.OpenKeyboard(this, res =>
                {
                    text = res;
                });
            }
            else
            {
                Debug.Log("Set keyboard of this input before enabling users to use it.");
            }
        }
    }
}