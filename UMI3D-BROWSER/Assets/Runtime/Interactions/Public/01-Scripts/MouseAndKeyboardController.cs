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
using umi3d.cdk.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.interactions
{
    public class MouseAndKeyboardController : ISelectorDelegate
    {
        public const string MOUSE_ID = "Mouse";
        public const string KEYBOARD_ID = "keyboard";

        #region Initialize

        static Lazy<MouseAndKeyboardController> _default = new(() => new());
        public static MouseAndKeyboardController @default => _default.Value;

        MouseAndKeyboardController()
        {
            SelectorManager.@default.delegates.Add(this);
            mouseSelector = SelectorManager.@default.InstantiateSelector();
            mouseSelector.id = MOUSE_ID;
            // TODO: set selector.

            mouseController = ControllerManager.@default.InstantiateController();
            mouseController.id = MOUSE_ID;
            // TODO: set controller.

            keyboardController = ControllerManager.@default.InstantiateController();
            keyboardController.id = KEYBOARD_ID;
            // TODO: set controller.
        }

        #endregion

        Selector mouseSelector;
        Controller mouseController;
        Controller keyboardController;

        #region ISelectorDelegate

        public void ToolSelected(Tool tool, Selector selector)
        {
            if (selector != this.mouseSelector) { return; }

            if (mouseController.TryToProject(tool, selector)) { return; }
            if (keyboardController.TryToProject(tool, selector)) { return; }
        }

        #endregion
    }
}