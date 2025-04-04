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
    public class BrowserControllerManager : ISelectorDelegate, IControllerDelegate
    {
        public const string CONTEXTUAL_MENU_ID = "ContextualMenu";
        public const string MOUSE_ID = "Mouse";
        public const string KEYBOARD_ID = "keyboard";
        public const string VR_ID = "VR";
        public const string HAND_ID = "Hand";
        public const string LEFT_ID = "Left";
        public const string RIGHT_ID = "Right";

        #region Initialize

        static Lazy<BrowserControllerManager> _default = new(() => new());
        public static BrowserControllerManager @default => _default.Value;

        BrowserControllerManager()
        {
            /*
             * SELECTORS
             */

            SelectorManager.@default.delegates.Add(this);
            SelectorManager.@default.TryToInstantiateSelector(out mouseSelector, MOUSE_ID);
            SelectorManager.@default.TryToInstantiateSelector(out leftVRSelector, LEFT_ID + VR_ID);
            SelectorManager.@default.TryToInstantiateSelector(out rightVRSelector, RIGHT_ID + VR_ID);
            SelectorManager.@default.TryToInstantiateSelector(out leftHandSelector, LEFT_ID + HAND_ID);
            SelectorManager.@default.TryToInstantiateSelector(out rightHandSelector, RIGHT_ID + HAND_ID);

            /*
             * CONTROLLERS
             */

            ControllerManager.@default.delegates.Add(this);
            
            ControllerManager.@default.TryToInstantiateController(out contextualMenuController, CONTEXTUAL_MENU_ID);
            //contextualMenuController.@delegate = new MouseControllerDataDelegate();

            ControllerManager.@default.TryToInstantiateController(out mouseController, MOUSE_ID);
            mouseController.@delegate = new MouseControllerDataDelegate();

            ControllerManager.@default.TryToInstantiateController(out keyboardController, KEYBOARD_ID);
            keyboardController.@delegate = new KeyboardControllerDataDelegate();

            ControllerManager.@default.TryToInstantiateController(out leftVRController, LEFT_ID + VR_ID);
            //leftVRController.@delegate = new KeyboardControllerDataDelegate();

            ControllerManager.@default.TryToInstantiateController(out rightVRController, RIGHT_ID + VR_ID);
            //leftVRController.@delegate = new KeyboardControllerDataDelegate();

            ControllerManager.@default.TryToInstantiateController(out leftHandController, LEFT_ID + HAND_ID);
            //leftHandController.@delegate = new KeyboardControllerDataDelegate();

            ControllerManager.@default.TryToInstantiateController(out rightHandController, RIGHT_ID + HAND_ID);
            //rightHandController.@delegate = new KeyboardControllerDataDelegate();

            mouseSelector.Add(contextualMenuController);
            mouseSelector.Add(mouseController);
            mouseSelector.Add(keyboardController);

            leftVRSelector.Add(contextualMenuController);
            leftVRSelector.Add(leftVRController);

            rightVRSelector.Add(contextualMenuController);
            rightVRSelector.Add(rightVRController);

            leftHandSelector.Add(contextualMenuController);
            leftHandSelector.Add(leftHandController);

            rightHandSelector.Add(contextualMenuController);
            rightHandSelector.Add(rightHandController);

            SelectorManager.@default.serverSelector.Add(contextualMenuController);
            SelectorManager.@default.serverSelector.Add(mouseController);
            SelectorManager.@default.serverSelector.Add(keyboardController);
            SelectorManager.@default.serverSelector.Add(leftVRController);
            SelectorManager.@default.serverSelector.Add(rightVRController);
            SelectorManager.@default.serverSelector.Add(leftHandController);
            SelectorManager.@default.serverSelector.Add(rightHandController);
        }

        #endregion

        Selector mouseSelector;
        Selector leftVRSelector;
        Selector rightVRSelector;
        Selector leftHandSelector;
        Selector rightHandSelector;

        Controller contextualMenuController;
        Controller mouseController;
        Controller keyboardController;
        Controller leftVRController;
        Controller rightVRController;
        Controller leftHandController;
        Controller rightHandController;

        #region ISelectorDelegate

        public void ToolSelected(Tool tool, Selector selector)
        {
            
        }

        #endregion

        #region IControllerDelegate

        public void OnChangeOfIsActive(bool active, Controller controller)
        {

        }

        #endregion
    }

    class MouseControllerDataDelegate: IControllerDataDelegate
    {
        public int ToolCountLimitation => 1;

        public bool CanProjectToolWhenSelected(Tool tool, Selector selector)
        {
            return selector.id == BrowserControllerManager.MOUSE_ID;
        }
    }

    class KeyboardControllerDataDelegate : IControllerDataDelegate
    {
        public int ToolCountLimitation => 1;

        public bool CanProjectToolWhenSelected(Tool tool, Selector selector)
        {
            return selector.id == BrowserControllerManager.MOUSE_ID;
        }
    }
}