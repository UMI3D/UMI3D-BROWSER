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
using System.Linq;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = umi3d.cdk.interaction.Input;

namespace umi3d.browserRuntime.interactions
{
    public class BrowserControllerManager : IControllerDelegate, IProjectionDelegate
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

            SelectorManager.@default.TryToInstantiateSelector(out mouseSelector, MOUSE_ID);
            mouseSelector.dataDelegate = new MouseSelectorDataDelegate();

            SelectorManager.@default.TryToInstantiateSelector(out leftVRSelector, LEFT_ID + VR_ID);
            leftVRSelector.dataDelegate = new LeftVRSelectorDataDelegate();

            SelectorManager.@default.TryToInstantiateSelector(out rightVRSelector, RIGHT_ID + VR_ID);
            rightVRSelector.dataDelegate = new RightVRSelectorDataDelegate();

            // TODO
            //SelectorManager.@default.TryToInstantiateSelector(out leftHandSelector, LEFT_ID + HAND_ID);
            //SelectorManager.@default.TryToInstantiateSelector(out rightHandSelector, RIGHT_ID + HAND_ID);

            SelectorManager.@default.serverSelector = new ServerSelector();

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
            //leftVRController.@dataDelegate = new KeyboardControllerDataDelegate();

            ControllerManager.@default.TryToInstantiateController(out rightVRController, RIGHT_ID + VR_ID);
            //leftVRController.@dataDelegate = new KeyboardControllerDataDelegate();

            // TODO
            //ControllerManager.@default.TryToInstantiateController(out leftHandController, LEFT_ID + HAND_ID);
            //leftHandController.@dataDelegate = new KeyboardControllerDataDelegate();

            //ControllerManager.@default.TryToInstantiateController(out rightHandController, RIGHT_ID + HAND_ID);
            //rightHandController.@dataDelegate = new KeyboardControllerDataDelegate();
            
            /*
             * Bind SELECTORS and CONTROLLERS
             */

            mouseSelector.Add(uiDeviceController);
            mouseSelector.Add(mouseController);
            mouseSelector.Add(keyboardController);

            leftVRSelector.Add(uiDeviceController);
            leftVRSelector.Add(leftVRController);

            rightVRSelector.Add(uiDeviceController);
            rightVRSelector.Add(rightVRController);
            
            // TODO
            //leftHandSelector.Add(uiDeviceController);
            //leftHandSelector.Add(leftHandController);

            //rightHandSelector.Add(uiDeviceController);
            //rightHandSelector.Add(rightHandController);

            ProjectionManager.@default.delegates.Add(this);
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

        #region IControllerDelegate

        public void OnChangeOfIsActive(bool active, Controller controller)
        {

        }

        #endregion

        #region IProjectionDelegate

        public void OnReleased(Projection projection)
        {
            if (projection.input.controller.id == UI_ID)
            {
                UIDevice.ReleaseInput(projection.input);
            }
        }

        #endregion
    }

    class ServerSelector : ISelector
    {
        public void Select(Tool tool)
        {
            // TODO

            Selector deselectedSelector = SelectorManager.@default.lastSelectorDeselected;
            if (deselectedSelector != null && deselectedSelector.canProjectMoreTool)
            {
                deselectedSelector.Select(tool);
                return;
            }


        }

        public void Deselect(Tool tool)
        {
            if (tool.selector == null)
            {
                UnityEngine.Debug.LogError($"[ServerSelector] Error: Try to deselect tool by tool's selector is null.");
                return;
            }

            tool.selector.Deselect(tool);
        }

        public void Switch(Tool toolToRelease, Tool toolToProject)
        {
            if (toolToRelease.selector == null)
            {
                UnityEngine.Debug.LogError($"[ServerSelector] Error: Try to switch tool by tool's selector is null.");
                return;
            }

            toolToRelease.selector.Switch(toolToRelease, toolToProject);
        }
    }


    #region ISelectorDataDelegate

    class MouseSelectorDataDelegate : ISelectorDataDelegate
    {
        public int toolCountLimitation => 1;

        /// <summary>
        /// For each interactions find the best possible input.<br/> 
        /// <paramref name="inputsByInteractions"/> is a dictionary of interactions where values are the possible inputs. <br/>
        /// <paramref name="associations"/> is a collection of associations that should be fill with the new associations at the end of the method.<br/>
        /// <br/>
        /// 
        /// The associations between interactions and inputs must follow these rules:
        /// <list type="number">
        /// <item>
        /// EventDto Association Order.<br/>
        /// The eventDto must be associated in the following order: Left mouse click, then Keyboard keys: Q, E, R, F, G.
        /// </item>
        /// <item>
        /// Priority for EventDto with the hold Property.<br/>
        /// If one or more eventDto have the hold property set to true, these interactions must be prioritized when assigning inputs.
        /// </item>
        /// <item>
        /// Handling Already Assigned Inputs.<br/>
        /// If the left mouse click and all keyboard keys (Q, E, R, F, G) are already associated with eventDto, the next eventDto must be assigned to UI inputs.
        /// </item>
        /// <item>
        /// Other type of interactions.<br/>
        /// Interactions that are not <see cref="EventDto"/> must be assigned to UI Input.
        /// </item>
        /// <item>
        /// Special Association for UI Inputs.<br/>
        /// If UI inputs are associated with interactions, the left mouse click must be reserved for a special interaction that allows opening or closing the UI menu. <br/>
        /// In this case, if among interactions there are <see cref="EventDto"/> those have to be reassigned without including the left mouse click in the association process.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="associations"></param>
        /// <param name="inputsByInteractions"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        /// <see cref="EventDto"/> is a subclass of <see cref="AbstractInteractionDto"/>. To check if an eventDto is hold: <see cref="EventDto.hold"/>. To check if an input is a UI input: <see cref="Input.controller"/>.id == <see cref="BrowserControllerManager.UI_ID"/>. Interactions are subclasses of <see cref="AbstractInteractionDto"/>, among them <see cref="EventDto"/> and a lot of other classes.
        /// </remarks>
        public void AssociateInteractionAndInput(
            List<(AbstractInteractionDto interaction, Input input)> associations, 
            ReadOnlyDictionary<AbstractInteractionDto, ReadOnlyCollection<Input>> inputsByInteractions
        )
        {
            // Separate interactions into EventDto and others
            List<EventDto> eventDtos = new();
            List<AbstractInteractionDto> otherInteractions = new();
            foreach (var interaction in inputsByInteractions.Keys)
            {
                if (interaction is EventDto eventDto) { eventDtos.Add(eventDto); }
                else { otherInteractions.Add(interaction); }
            }
            // Sort EventDtos by hold property and input order
            eventDtos = eventDtos.OrderByDescending(e => e.hold).ToList();

            bool hasUIInput = false;
            // Assign UI inputs to other interactions
            foreach (var interaction in otherInteractions)
            {
                Input placeholder = inputsByInteractions[interaction].First();
                if (placeholder == null)
                {
                    UnityEngine.Debug.LogError($"[MouseSelectorDataDelegate] Error: no ui input for {interaction.GetType()}, {interaction.name}");
                    continue;
                }

                Input input = UIDevice.GetInputFrom(placeholder.control);
                if (input == null)
                {
                    UnityEngine.Debug.LogError($"[MouseSelectorDataDelegate] Error: no ui input from placeholder for {interaction.GetType()}, {interaction.name}");
                    continue;
                }
                associations.Add((interaction, input));
                hasUIInput = true;
            }

            // Track assigned inputs
            List<(AbstractInteractionDto interaction, Input input)> assignedInputs = new();

            // Assign inputs to EventDtos
            void AssignEventDtos()
            {
                foreach (var eventDto in eventDtos)
                {
                    for (int i = 0; i < inputsByInteractions[eventDto].Count; i++)
                    {
                        Input input = inputsByInteractions[eventDto][i];

                        if (assignedInputs.FindIndex(association => association.input == input) >= 0) { continue; }
                        else if (hasUIInput && input.control == Mouse.current.leftButton) { continue; }
                        else if (input.controller.id == BrowserControllerManager.UI_ID)
                        {
                            hasUIInput = true;
                            input = UIDevice.GetInputFrom(input.control);
                        }

                        assignedInputs.Add((eventDto, input));
                        break;
                    }
                }
            }

            void AssignEventDtosWithUI()
            {
                AssignEventDtos();

                // TODO assign input to open or close menu.
            }

            if (hasUIInput) { AssignEventDtosWithUI(); }
            else
            {
                AssignEventDtos();

                if (hasUIInput)
                {
                    IEnumerable<(AbstractInteractionDto interaction, Input input)> uiAssociations = assignedInputs
                        .Where(association => association.input.controller.id == BrowserControllerManager.UI_ID);
                    foreach (var association in uiAssociations)
                    {
                        UIDevice.ReleaseInput(association.input);
                    }
                    assignedInputs.Clear();

                    AssignEventDtosWithUI();
                }
            }

            associations.AddRange(assignedInputs);
        }

        public bool TryGetInputForBooleanParameterDto(out Input input, out Controller controller, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            return ControllerManager.@default.TryGetInputForBooleanParameterDto(
               out input,
               out controller,
              BrowserControllerManager.MOUSE_ID,
              controllers
           );
        }

        public bool TryGetInputsForEventDto(out ReadOnlyCollection<Input> inputs, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            List<Input> _inputs = new();

            ControllerManager.@default.TryGetInputsForEventDto(
                _inputs,
                BrowserControllerManager.MOUSE_ID,
                controllers
            );

            ControllerManager.@default.TryGetInputsForEventDto(
                _inputs,
                BrowserControllerManager.KEYBOARD_ID,
                controllers
            );

            ControllerManager.@default.TryGetInputsForEventDto(
                _inputs,
                BrowserControllerManager.UI_ID,
                controllers
            );

            inputs = _inputs.AsReadOnly();

            return inputs.Count > 0;
        }
    }

    class LeftVRSelectorDataDelegate : ISelectorDataDelegate
    {
        public int toolCountLimitation => 1;

        public void AssociateInteractionAndInput(List<(AbstractInteractionDto interaction, Input input)> associations, ReadOnlyDictionary<AbstractInteractionDto, ReadOnlyCollection<Input>> inputsByInteractions)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetInputForBooleanParameterDto(out Input input, out Controller controller, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForEventDto(out ReadOnlyCollection<Input> inputs, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            //return ControllerManager.@default.TryGetInputForEventDto(
            //    out input,
            //    out controller,
            //   BrowserControllerManager.LEFT_ID + BrowserControllerManager.VR_ID,
            //   controllers
            //) // First try to find input from left VR controller.

            //|| ControllerManager.@default.TryGetInputForEventDto(
            //    out input,
            //    out controller,
            //   BrowserControllerManager.UI_ID,
            //   controllers
            //) // finally try to find input from ui.
            //;

            throw new System.NotImplementedException();
        }
    }

    class RightVRSelectorDataDelegate : ISelectorDataDelegate
    {
        public int toolCountLimitation => 1;

        public void AssociateInteractionAndInput(List<(AbstractInteractionDto interaction, Input input)> associations, ReadOnlyDictionary<AbstractInteractionDto, ReadOnlyCollection<Input>> inputsByInteractions)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetInputForBooleanParameterDto(out Input input, out Controller controller, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForEventDto(out ReadOnlyCollection<Input> inputs, Selector selector, ReadOnlyCollection<Controller> controllers)
        {
            //return ControllerManager.@default.TryGetInputForEventDto(
            //    out input,
            //    out controller,
            //   BrowserControllerManager.RIGHT_ID + BrowserControllerManager.VR_ID,
            //   controllers
            //) // First try to find input from right VR controller.

            //|| ControllerManager.@default.TryGetInputForEventDto(
            //    out input,
            //    out controller,
            //   BrowserControllerManager.UI_ID,
            //   controllers
            //) // finally try to find input from ui.
            //;

            throw new System.NotImplementedException();
        }
    }

    #endregion

    #region IControllerDataDelegate

    class UIControllerDataDelegate : IControllerDataDelegate
    {
        public bool TryGetInputForBooleanParameterDto(out Input input, Controller controller)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForEventDto(List<Input> inputs, Controller controller)
        {
            Input input;
            InputManager.@default.TryGetInput(
                out input,
                controller,
                UIDevice.GetButtonPlaceholder(),
                InputActionType.Button
            );

            inputs.Add(input);
            return true;
        }
    }

    class MouseControllerDataDelegate: IControllerDataDelegate
    {
        public bool TryGetInputForBooleanParameterDto(out Input input, Controller controller)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForEventDto(List<Input> inputs, Controller controller)
        {
            Mouse mouse = Mouse.current;
            if (mouse == null) { return false; }

            Input input;
            InputManager.@default.TryGetInput(
                out input, 
                controller, 
                mouse.leftButton, 
                InputActionType.Button
            );

            inputs.Add(input);
            return true;
        }
    }

    class KeyboardControllerDataDelegate : IControllerDataDelegate
    {
        public bool TryGetInputForBooleanParameterDto(out Input input, Controller controller)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForEventDto(List<Input> inputs, Controller controller)
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) { return false; }

            Input input;
            InputManager.@default.TryGetInput(
                out input,
                controller,
                keyboard.qKey,
                InputActionType.Button
            );
            inputs.Add(input);

            InputManager.@default.TryGetInput(
                out input,
                controller,
                keyboard.eKey,
                InputActionType.Button
            );
            inputs.Add(input);

            InputManager.@default.TryGetInput(
                out input,
                controller,
                keyboard.rKey,
                InputActionType.Button
            );
            inputs.Add(input);

            InputManager.@default.TryGetInput(
                out input,
                controller,
                keyboard.fKey,
                InputActionType.Button
            );
            inputs.Add(input);

            InputManager.@default.TryGetInput(
                out input,
                controller,
                keyboard.gKey,
                InputActionType.Button
            );
            inputs.Add(input);

            return true;
        }
    }

    #endregion
}