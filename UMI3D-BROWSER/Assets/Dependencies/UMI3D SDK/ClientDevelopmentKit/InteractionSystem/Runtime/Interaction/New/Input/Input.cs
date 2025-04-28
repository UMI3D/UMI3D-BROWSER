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

namespace umi3d.cdk.interaction
{
    public sealed class Input 
    {
        internal Input(IInputSystem inputSystem)
        {
            this.inputSystem = inputSystem;

            inputSystem.started += OnStarted;
            inputSystem.performed += OnPerformed;
            inputSystem.canceled += OnCanceled;
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

        void OnStarted(object obj)
        {
            onStarted?.Invoke(obj);
        }
        void OnPerformed(object obj)
        {
            onPerformed?.Invoke(obj);
        }
        void OnCanceled(object obj)
        {
            onCanceled?.Invoke(obj);
        }

        public Action<object> onStarted;
        public Action<object> onPerformed;
        public Action<object> onCanceled;

        public void WriteStructValue<T>(T value) where T : struct
        {
            inputSystem.WriteStructValue(value);
        }
        public void WriteClassValue<T>(T value) where T : class
        {
            inputSystem.WriteClassValue(value);
        }
        public T ReadValue<T>()
        {
            return inputSystem.ReadValue<T>();
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