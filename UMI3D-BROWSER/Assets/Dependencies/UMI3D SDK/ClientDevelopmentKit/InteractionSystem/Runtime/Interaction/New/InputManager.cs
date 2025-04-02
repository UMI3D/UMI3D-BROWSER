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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public sealed class InputManager 
    {
        #region Initialize

        static Lazy<InputManager> _default = new(() => new());
        public static InputManager @default => _default.Value;

        InputManager()
        {

        }

        #endregion

        List<Input> _inputs = new List<Input>();
        ReadOnlyCollection<Input> inputs => _inputs.AsReadOnly();

        public Input InstantiateInput()
        {
            Input input = new();
            _inputs.Add(input);

            return input;
        }
    }
}