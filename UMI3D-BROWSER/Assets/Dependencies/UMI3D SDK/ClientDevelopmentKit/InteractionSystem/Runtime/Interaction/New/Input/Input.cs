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
using UnityEngine.InputSystem;

namespace umi3d.cdk.interaction
{
    public sealed class Input 
    {
        internal Input(InputControl control, InputActionType actionType)
        {
            this.control = control;

            action = new InputAction(control.name, actionType);
            action.AddBinding(control.path);
        }

        public bool isAvailable { get; private set; }

        public InputAction action { get; private set; }
        public InputControl control { get; private set; }

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

        public Tool projectedTool { get; private set; }
        public AbstractInteractionDto projectedInteraction { get; private set; }
        public Selector selector { get; private set; }
        internal void Associate(Tool tool, AbstractInteractionDto interaction, Selector selector)
        {
            if (!isAvailable) { return; }
            isAvailable = false;

            projectedTool = tool;
            projectedInteraction = interaction;
            this.selector = selector;
        }
        internal void DissociateFromInteraction()
        {
            projectedTool = null;
            projectedInteraction = null;
            this.selector = null;

            isAvailable = false;
        }
    }
}