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
    public class BrowserControllerManager : IProjectionDelegate
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

            ControllerManager.@default.TryToInstantiateController(out uiDeviceController, UI_ID);

            ControllerManager.@default.TryToInstantiateController(out mouseController, MOUSE_ID);

            ControllerManager.@default.TryToInstantiateController(out keyboardController, KEYBOARD_ID);

            ControllerManager.@default.TryToInstantiateController(out leftVRController, LEFT_ID + VR_ID);

            ControllerManager.@default.TryToInstantiateController(out rightVRController, RIGHT_ID + VR_ID);

            // TODO
            //ControllerManager.@default.TryToInstantiateController(out leftHandController, LEFT_ID + HAND_ID);

            //ControllerManager.@default.TryToInstantiateController(out rightHandController, RIGHT_ID + HAND_ID);
            

            ProjectionManager.@default.delegates.Add(this);
        }

        #endregion

        public readonly Selector mouseSelector;
        public readonly Selector leftVRSelector;
        public readonly Selector rightVRSelector;
        public readonly Selector leftHandSelector;
        public readonly Selector rightHandSelector;

        public readonly Controller uiDeviceController;
        public readonly Controller mouseController;
        public readonly Controller keyboardController;
        public readonly Controller leftVRController;
        public readonly Controller rightVRController;
        public readonly Controller leftHandController;
        public readonly Controller rightHandController;

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

            bool TryToGetUIInput(out Input input, AbstractInteractionDto interaction, IInputSystem inputSystem)
            {
                input = UIDevice.GetInputFrom(inputSystem);
                if (input == null)
                {
                    UnityEngine.Debug.LogError($"[MouseSelectorDataDelegate] Error: no ui input from placeholder for {interaction.GetType()}, {interaction.name}");
                   return false;
                }
                return true;
            }

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

                if (!TryToGetUIInput(out Input input, interaction, placeholder.inputSystem)) { continue; }

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
                        else if (hasUIInput && input.inputSystem.id == Mouse.current.leftButton.path) { continue; }
                        else if (input.controller.id == BrowserControllerManager.UI_ID)
                        {
                            if (!TryToGetUIInput(out input, eventDto, input.inputSystem)) { continue; }
                            hasUIInput = true;
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

        List<Input> _eventDtoInputs = new();
        public bool TryGetInputsForEventDto(out ReadOnlyCollection<Input> inputs)
        {
            _eventDtoInputs.Clear();

            Mouse mouse = Mouse.current;
            if (mouse != null)
            {
                BrowserControllerManager
                    .@default
                    .mouseController
                    .TryToAddInput(_eventDtoInputs, mouse.leftButton, InputActionType.Button);
            }

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                BrowserControllerManager
                    .@default
                    .keyboardController
                    .TryToAddInput(_eventDtoInputs, keyboard.qKey, InputActionType.Button);
                BrowserControllerManager
                    .@default
                    .keyboardController
                    .TryToAddInput(_eventDtoInputs, keyboard.eKey, InputActionType.Button);
                BrowserControllerManager
                    .@default
                    .keyboardController
                    .TryToAddInput(_eventDtoInputs, keyboard.rKey, InputActionType.Button);
                BrowserControllerManager
                    .@default
                    .keyboardController
                    .TryToAddInput(_eventDtoInputs, keyboard.fKey, InputActionType.Button);
                BrowserControllerManager
                    .@default
                    .keyboardController
                    .TryToAddInput(_eventDtoInputs, keyboard.gKey, InputActionType.Button);
            }

            BrowserControllerManager
                .@default
                .uiDeviceController
                .TryToAddInput(_eventDtoInputs, UIDevice.GetButtonPlaceholder(), InputActionType.Button);

            inputs = _eventDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }

        public bool TryGetInputsForDrawingInteractionDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        List<Input> _booleanParameterDtoInputs = new();
        public bool TryGetInputsForBooleanParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            _booleanParameterDtoInputs.Clear();

            BrowserControllerManager
                .@default
                .uiDeviceController
                .TryToAddInput(_booleanParameterDtoInputs, UIDevice.GetButtonPlaceholder(), InputActionType.Button);

            inputs = _booleanParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }

        List<Input> _floatParameterDtoInputs = new();
        public bool TryGetInputsForFloatParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            _floatParameterDtoInputs.Clear();

            BrowserControllerManager
                .@default
                .uiDeviceController
                .TryToAddInput(_floatParameterDtoInputs, UIDevice.GetDoublePlaceholder(), InputActionType.PassThrough);

            inputs = _floatParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }

        public bool TryGetInputsForIntegerParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForStringParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForColorParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForVector2ParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForVector3ParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForVector4ParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForEnumParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForFloatRangeParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForIntegerRangeParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForUploadFileParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForLocalInfoRequestParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }
    }

    class LeftVRSelectorDataDelegate : ISelectorDataDelegate
    {
        public int toolCountLimitation => 1;

        public void AssociateInteractionAndInput(List<(AbstractInteractionDto interaction, Input input)> associations, ReadOnlyDictionary<AbstractInteractionDto, ReadOnlyCollection<Input>> inputsByInteractions)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetInputsForBooleanParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForColorParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForDrawingInteractionDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForEnumParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForEventDto(out ReadOnlyCollection<Input> inputs)
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

        public bool TryGetInputsForFloatParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForFloatRangeParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForIntegerParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForIntegerRangeParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForLocalInfoRequestParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForStringParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForUploadFileParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForVector2ParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForVector3ParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForVector4ParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }
    }

    class RightVRSelectorDataDelegate : ISelectorDataDelegate
    {
        public int toolCountLimitation => 1;

        public void AssociateInteractionAndInput(List<(AbstractInteractionDto interaction, Input input)> associations, ReadOnlyDictionary<AbstractInteractionDto, ReadOnlyCollection<Input>> inputsByInteractions)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetInputsForBooleanParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForColorParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForDrawingInteractionDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForEnumParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForEventDto(out ReadOnlyCollection<Input> inputs)
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

        public bool TryGetInputsForFloatParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForFloatRangeParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForIntegerParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForIntegerRangeParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForLocalInfoRequestParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForStringParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForUploadFileParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForVector2ParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForVector3ParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsForVector4ParameterDto(out ReadOnlyCollection<Input> inputs)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}