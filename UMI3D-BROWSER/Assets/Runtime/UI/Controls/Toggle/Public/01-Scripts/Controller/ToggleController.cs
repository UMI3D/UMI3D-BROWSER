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

using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    public class ToggleController : MonoBehaviour
    {
        public ToggleModel model { get; private set; }

        void Awake()
        {
            model = new();
        }

        public void ValueUpdated(bool value)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            model.Clear();
        }
    }
}