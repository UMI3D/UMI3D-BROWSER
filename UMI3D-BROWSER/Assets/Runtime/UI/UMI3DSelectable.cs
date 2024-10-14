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
using UnityEngine.Events;

namespace umi3d.browserRuntime.ui
{
    public class UMI3DSelectable : MonoBehaviour
    {
        /// <summary>
        /// Event raised when the input is selected.
        /// </summary>
        public UnityEvent OnSelectEvent = new UnityEvent();

        /// <summary>
        /// Event raised when the input is deselected.
        /// </summary>
        public UnityEvent OnDeselectEvent = new UnityEvent();
    }
}