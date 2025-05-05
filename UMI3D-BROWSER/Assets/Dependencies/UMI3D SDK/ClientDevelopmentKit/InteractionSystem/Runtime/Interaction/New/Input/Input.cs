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
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public sealed class Input 
    {
        internal Input(IInputSystem inputSystem)
        {
            this.inputSystem = inputSystem;
        }

        public bool isAvailable { get; internal set; } = true;

        public Controller controller { get; private set; }
        internal bool Associate(Controller controller)
        {
            if (this.controller != null) { return false; }

            this.controller = controller;
            return true;
        }
        internal void DissociateFromController()
        {
            this.controller = null;
        }

        public IInputSystem inputSystem { get; private set; }

        IEventInputSystem _eventInput = new NullObjectEventInput();
        public IEventInputSystem eventInput
        {
            get => _eventInput;
            set
            {
                if (value == null)
                {
                    _eventInput = new NullObjectEventInput();
                    return;
                }

                _eventInput = value;
            }
        }

        IParameterInputSystem<bool> _booleanParameterInput = new NullObjectParameterInput<bool>();
        public IParameterInputSystem<bool> booleanParameterInput
        {
            get => _booleanParameterInput;
            set
            {
                if (value == null)
                {
                    _booleanParameterInput = new NullObjectParameterInput<bool>();
                    return;
                }

                _booleanParameterInput = value;
            }
        }

        IParameterInputSystem<float> _floatParameterInput = new NullObjectParameterInput<float>();
        public IParameterInputSystem<float> floatParameterInput
        {
            get => _floatParameterInput;
            set
            {
                if (value == null)
                {
                    _floatParameterInput = new NullObjectParameterInput<float>();
                    return;
                }

                _floatParameterInput = value;
            }
        }

        IParameterInputSystem<int> _intParameterInput = new NullObjectParameterInput<int>();
        public IParameterInputSystem<int> intParameterInput
        {
            get => _intParameterInput;
            set
            {
                if (value == null)
                {
                    _intParameterInput = new NullObjectParameterInput<int>();
                    return;
                }

                _intParameterInput = value;
            }
        }

        IParameterInputSystem<Vector2> _vector2ParameterInput = new NullObjectParameterInput<Vector2>();
        public IParameterInputSystem<Vector2> vector2ParameterInput
        {
            get => _vector2ParameterInput;
            set
            {
                if (value == null)
                {
                    _vector2ParameterInput = new NullObjectParameterInput<Vector2>();
                    return;
                }

                _vector2ParameterInput = value;
            }
        }

        IParameterInputSystem<Vector3> _vector3ParameterInput = new NullObjectParameterInput<Vector3>();
        public IParameterInputSystem<Vector3> vector3ParameterInput
        {
            get => _vector3ParameterInput;
            set
            {
                if (value == null)
                {
                    _vector3ParameterInput = new NullObjectParameterInput<Vector3>();
                    return;
                }

                _vector3ParameterInput = value;
            }
        }

        IParameterInputSystem<Vector4> _vector4ParameterInput = new NullObjectParameterInput<Vector4>();
        public IParameterInputSystem<Vector4> vector4ParameterInput
        {
            get => _vector4ParameterInput;
            set
            {
                if (value == null)
                {
                    _vector4ParameterInput = new NullObjectParameterInput<Vector4>();
                    return;
                }

                _vector4ParameterInput = value;
            }
        }

        IParameterInputSystem<string> _stringParameterInput = new NullObjectParameterInput<string>();
        public IParameterInputSystem<string> stringParameterInput
        {
            get => _stringParameterInput;
            set
            {
                if (value == null)
                {
                    _stringParameterInput = new NullObjectParameterInput<string>();
                    return;
                }

                _stringParameterInput = value;
            }
        }

        IParameterInputSystem<Color> _colorParameterInput = new NullObjectParameterInput<Color>();
        public IParameterInputSystem<Color> colorParameterInput
        {
            get => _colorParameterInput;
            set
            {
                if (value == null)
                {
                    _colorParameterInput = new NullObjectParameterInput<Color>();
                    return;
                }

                _colorParameterInput = value;
            }
        }

        IParameterInputSystem<LocalInfoRequestParameterValue> _localInfoParameterInput = new NullObjectParameterInput<LocalInfoRequestParameterValue>();
        public IParameterInputSystem<LocalInfoRequestParameterValue> localInfoParameterInput
        {
            get => _localInfoParameterInput;
            set
            {
                if (value == null)
                {
                    _localInfoParameterInput = new NullObjectParameterInput<LocalInfoRequestParameterValue>();
                    return;
                }

                _localInfoParameterInput = value;
            }
        }

        IUploadFileParameterInputSystem _uploadFileParameterInput = new NullObjectUploadFileParameterInput();
        public IUploadFileParameterInputSystem uploadFileParameterInput
        {
            get => _uploadFileParameterInput;
            set
            {
                if (value == null)
                {
                    _uploadFileParameterInput = new NullObjectUploadFileParameterInput();
                    return;
                }

                _uploadFileParameterInput = value;
            }
        }

        public string debugDescription
        {
            get
            {
                string description = "";

                description += $"---- Input ----\n";
                description += $"{controller?.id ?? "No controller"}, {isAvailable}, {inputSystem.id}\n";
                description += "\n";

                return description;
            }
        }
    }
}