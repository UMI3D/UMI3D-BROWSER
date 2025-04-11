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
using System.Collections.ObjectModel;
using System.Linq;
using umi3d.cdk.interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = umi3d.cdk.interaction.Input;

namespace umi3d.browserRuntime.interactions
{
    public class BrowserControllerManager : ISelectorDelegate, IControllerDelegate
    {
        public const string UI_ID = "UI";
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

            SelectorManager.@default.serverSelector.@delegate = new ServerSelectorDataDelegate();

            SelectorManager.@default.TryToInstantiateSelector(out mouseSelector, MOUSE_ID);
            mouseSelector.@delegate = new MouseSelectorDataDelegate();

            SelectorManager.@default.TryToInstantiateSelector(out leftVRSelector, LEFT_ID + VR_ID);
            leftVRSelector.@delegate = new LeftVRSelectorDataDelegate();

            SelectorManager.@default.TryToInstantiateSelector(out rightVRSelector, RIGHT_ID + VR_ID);
            rightVRSelector.@delegate = new RightVRSelectorDataDelegate();

            // TODO
            //SelectorManager.@default.TryToInstantiateSelector(out leftHandSelector, LEFT_ID + HAND_ID);
            //SelectorManager.@default.TryToInstantiateSelector(out rightHandSelector, RIGHT_ID + HAND_ID);

            /*
             * CONTROLLERS
             */

            ControllerManager.@default.delegates.Add(this);
            
            ControllerManager.@default.TryToInstantiateController(out uiDeviceController, UI_ID);
            uiDeviceController.@delegate = new UIControllerDataDelegate();

            ControllerManager.@default.TryToInstantiateController(out mouseController, MOUSE_ID);
            mouseController.@delegate = new MouseControllerDataDelegate();

            ControllerManager.@default.TryToInstantiateController(out keyboardController, KEYBOARD_ID);
            keyboardController.@delegate = new KeyboardControllerDataDelegate();

            ControllerManager.@default.TryToInstantiateController(out leftVRController, LEFT_ID + VR_ID);
            //leftVRController.@delegate = new KeyboardControllerDataDelegate();

            ControllerManager.@default.TryToInstantiateController(out rightVRController, RIGHT_ID + VR_ID);
            //leftVRController.@delegate = new KeyboardControllerDataDelegate();


            // TODO
            //ControllerManager.@default.TryToInstantiateController(out leftHandController, LEFT_ID + HAND_ID);
            //leftHandController.@delegate = new KeyboardControllerDataDelegate();

            //ControllerManager.@default.TryToInstantiateController(out rightHandController, RIGHT_ID + HAND_ID);
            //rightHandController.@delegate = new KeyboardControllerDataDelegate();

            mouseSelector.Add(uiDeviceController);
            mouseSelector.Add(mouseController);
            mouseSelector.Add(keyboardController);

            leftVRSelector.Add(uiDeviceController);
            leftVRSelector.Add(leftVRController);

            rightVRSelector.Add(uiDeviceController);
            rightVRSelector.Add(rightVRController);

            leftHandSelector.Add(uiDeviceController);
            leftHandSelector.Add(leftHandController);

            rightHandSelector.Add(uiDeviceController);
            rightHandSelector.Add(rightHandController);

            SelectorManager.@default.serverSelector.Add(uiDeviceController);
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

        Controller uiDeviceController;
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

    #region ISelectorDataDelegate

    class ServerSelectorDataDelegate : ISelectorDataDelegate
    {
        public bool TryGetInputForBooleanParameterDto(out Input input, out Controller controller, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            return false
#if UMI3D_PC    

            || ControllerManager.@default.TryGetInputForBooleanParameterDto(
                out input,
                out controller,
                BrowserControllerManager.MOUSE_ID,
                controllers
            ) // First try to find input from mouse.

            || ControllerManager.@default.TryGetInputForBooleanParameterDto(
                out input,
                out controller,
                BrowserControllerManager.KEYBOARD_ID,
                controllers
            ) // then try to find input from keyboard.

#elif UMI3D_VR

            || ControllerManager.@default.TryGetInputForBooleanParameterDto(
                out input,
                out controller,
                BrowserControllerManager.LEFT_ID + BrowserControllerManager.VR_ID,
                controllers
            ) // First try to find input from left VR controller.

            || ControllerManager.@default.TryGetInputForBooleanParameterDto(
                out input,
                out controller,
                BrowserControllerManager.RIGHT_ID + BrowserControllerManager.VR_ID,
                controllers
            ) // then try to find input from right VR controller.

            || ControllerManager.@default.TryGetInputForBooleanParameterDto(
                out input,
                out controller,
                BrowserControllerManager.LEFT_ID + BrowserControllerManager.HAND_ID,
                controllers
            ) // First try to find input from left Hand controller.

            || ControllerManager.@default.TryGetInputForBooleanParameterDto(
                out input,
                out controller,
                BrowserControllerManager.RIGHT_ID + BrowserControllerManager.HAND_ID,
                controllers
            ) // then try to find input from right Hand controller.

#endif
            || ControllerManager.@default.TryGetInputForBooleanParameterDto(
                out input,
                out controller,
                BrowserControllerManager.UI_ID,
                controllers
            ) // finally try to find input from ui.
            ;
        }

        public bool TryGetInputForEventDto(out Input input, out Controller controller, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            return false
#if UMI3D_PC    

            || ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
                BrowserControllerManager.MOUSE_ID,
                controllers
            ) // First try to find input from mouse.

            || ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
                BrowserControllerManager.KEYBOARD_ID,
                controllers
            ) // then try to find input from keyboard.

#elif UMI3D_VR

            || ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
                BrowserControllerManager.LEFT_ID + BrowserControllerManager.VR_ID,
                controllers
            ) // First try to find input from left VR controller.

            || ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
                BrowserControllerManager.RIGHT_ID + BrowserControllerManager.VR_ID,
                controllers
            ) // then try to find input from right VR controller.

            || ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
                BrowserControllerManager.LEFT_ID + BrowserControllerManager.HAND_ID,
                controllers
            ) // First try to find input from left Hand controller.

            || ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
                BrowserControllerManager.RIGHT_ID + BrowserControllerManager.HAND_ID,
                controllers
            ) // then try to find input from right Hand controller.

#endif
            || ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
                BrowserControllerManager.UI_ID,
                controllers
            ) // finally try to find input from ui.
            ;
        }
    }

    class MouseSelectorDataDelegate : ISelectorDataDelegate
    {
        public bool TryGetInputForBooleanParameterDto(out Input input, out Controller controller, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            return ControllerManager.@default.TryGetInputForBooleanParameterDto(
               out input,
               out controller,
              BrowserControllerManager.MOUSE_ID,
              controllers
           ) // First try to find input from mouse.

           || ControllerManager.@default.TryGetInputForBooleanParameterDto(
               out input,
               out controller,
              BrowserControllerManager.KEYBOARD_ID,
              controllers
           ) // then try to find input from keyboard.

           || ControllerManager.@default.TryGetInputForBooleanParameterDto(
               out input,
               out controller,
              BrowserControllerManager.UI_ID,
              controllers
           ) // finally try to find input from ui.
           ;
        }

        public bool TryGetInputForEventDto(out Input input, out Controller controller, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            return ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
               BrowserControllerManager.MOUSE_ID,
               controllers
            ) // First try to find input from mouse.
                
            || ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
               BrowserControllerManager.KEYBOARD_ID,
               controllers
            ) // then try to find input from keyboard.

            || ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
               BrowserControllerManager.UI_ID,
               controllers
            ) // finally try to find input from ui.
            ;
        }
    }

    class LeftVRSelectorDataDelegate : ISelectorDataDelegate
    {
        public bool TryGetInputForBooleanParameterDto(out Input input, out Controller controller, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputForEventDto(out Input input, out Controller controller, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            return ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
               BrowserControllerManager.LEFT_ID + BrowserControllerManager.VR_ID,
               controllers
            ) // First try to find input from left VR controller.

            || ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
               BrowserControllerManager.UI_ID,
               controllers
            ) // finally try to find input from ui.
            ;
        }
    }

    class RightVRSelectorDataDelegate : ISelectorDataDelegate
    {
        public bool TryGetInputForBooleanParameterDto(out Input input, out Controller controller, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputForEventDto(out Input input, out Controller controller, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            return ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
               BrowserControllerManager.RIGHT_ID + BrowserControllerManager.VR_ID,
               controllers
            ) // First try to find input from right VR controller.

            || ControllerManager.@default.TryGetInputForEventDto(
                out input,
                out controller,
               BrowserControllerManager.UI_ID,
               controllers
            ) // finally try to find input from ui.
            ;
        }
    }

    #endregion

    #region IControllerDataDelegate

    class UIControllerDataDelegate : IControllerDataDelegate
    {
        public int ToolCountLimitation => 1;

        public bool TryGetInputForBooleanParameterDto(out Input input, Controller controller)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputForEventDto(out Input input, Controller controller)
        {
            UIDevice uIDevice = UIDevice.current;
            if (uIDevice == null)
            {
                input = null;
                return false;
            }

            // TODO: Refacto for even more buttons.
            return InputManager.@default.TryGetInput(
                out input,
                controller,
                uIDevice.button1,
                InputActionType.Button
            ) 
                
            || InputManager.@default.TryGetInput(
                out input,
                controller,
                uIDevice.button2,
                InputActionType.Button
            ) 
            
            || InputManager.@default.TryGetInput(
                out input,
                controller,
                uIDevice.button3,
                InputActionType.Button
            ) 
           
           || InputManager.@default.TryGetInput(
               out input,
               controller,
               uIDevice.button4,
               InputActionType.Button
           ) 
           
           || InputManager.@default.TryGetInput(
               out input,
               controller,
               uIDevice.button5,
               InputActionType.Button
           )
           ;
        }
    }

    class MouseControllerDataDelegate: IControllerDataDelegate
    {
        public int ToolCountLimitation => 1;

        public bool TryGetInputForBooleanParameterDto(out Input input, Controller controller)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputForEventDto(out Input input, Controller controller)
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                input = null;
                return false;
            }

            return InputManager.@default.TryGetInput(
                out input, 
                controller, 
                mouse.leftButton, 
                InputActionType.Button
            );
        }
    }

    class KeyboardControllerDataDelegate : IControllerDataDelegate
    {
        public int ToolCountLimitation => 1;

        public bool TryGetInputForBooleanParameterDto(out Input input, Controller controller)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputForEventDto(out Input input, Controller controller)
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                input = null;
                return false;
            }

            return InputManager.@default.TryGetInput(
                out input,
                controller,
                keyboard.qKey,
                InputActionType.Button
            ) // First try to find input for the Q key (on QWERTY)
                
            || InputManager.@default.TryGetInput(
                out input,
                controller,
                keyboard.eKey,
                InputActionType.Button
            ) // The try to find input for the E key (on QWERTY)

            || InputManager.@default.TryGetInput(
                out input,
                controller,
                keyboard.rKey,
                InputActionType.Button
            ) // Then try to find input for the R key (on QWERTY)

            || InputManager.@default.TryGetInput(
                out input,
                controller,
                keyboard.fKey,
                InputActionType.Button
            ) // Then try to find input for the F key (on QWERTY)

            || InputManager.@default.TryGetInput(
                out input,
                controller,
                keyboard.gKey,
                InputActionType.Button
            ) // Then try to find input for the G key (on QWERTY)
            ;
        }
    }

    #endregion
}