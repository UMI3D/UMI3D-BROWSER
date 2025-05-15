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

            SelectorManager.@default.InstantiateOrGet(out mouseSelector, MOUSE_ID);
            mouseSelector.dataDelegate = new MouseSelectorDataDelegate();

            SelectorManager.@default.InstantiateOrGet(out leftVRSelector, LEFT_ID + VR_ID);
            leftVRSelector.dataDelegate = new VRSelectorDataDelegate() { isRightSelector = false };

            SelectorManager.@default.InstantiateOrGet(out rightVRSelector, RIGHT_ID + VR_ID);
            rightVRSelector.dataDelegate = new VRSelectorDataDelegate() { isRightSelector = true };

            // TODO
            //SelectorManager.@default.TryToInstantiateSelector(out leftHandSelector, LEFT_ID + HAND_ID);
            //SelectorManager.@default.TryToInstantiateSelector(out rightHandSelector, RIGHT_ID + HAND_ID);

            SelectorManager.@default.serverSelector = new ServerSelector();

            /*
             * CONTROLLERS
             */

            ControllerManager.@default.InstantiateOrGet(out uiDeviceController, UI_ID);

            ControllerManager.@default.InstantiateOrGet(out mouseController, MOUSE_ID);

            ControllerManager.@default.InstantiateOrGet(out keyboardController, KEYBOARD_ID);

            ControllerManager.@default.InstantiateOrGet(out leftVRController, LEFT_ID + VR_ID);

            ControllerManager.@default.InstantiateOrGet(out rightVRController, RIGHT_ID + VR_ID);

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
            if (projection.controller.id == UI_ID)
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
            List<(Interaction interaction, Input input)> associations, 
            ReadOnlyDictionary<Interaction, ReadOnlyCollection<Input>> inputsByInteractions
        )
        {
            // Separate interactions into EventDto and others
            List<Interaction> eventDtos = new();
            List<Interaction> otherInteractions = new();
            foreach (var interaction in inputsByInteractions.Keys)
            {
                if (interaction.dto is EventDto) { eventDtos.Add(interaction); }
                else { otherInteractions.Add(interaction); }
            }
            // Sort EventDtos by hold property and input order
            eventDtos = eventDtos.OrderByDescending(e => (e.dto as EventDto).hold).ToList();

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
                    UnityEngine.Debug.LogError($"[MouseSelectorDataDelegate] Error: no ui input for {interaction.dto.GetType()}, {interaction.dto.name}");
                    continue;
                }

                if (!TryToGetUIInput(out Input input, interaction.dto, placeholder.inputSystem)) { continue; }

                associations.Add((interaction, input));
                hasUIInput = true;
            }

            // Track assigned inputs
            List<(Interaction interaction, Input input)> assignedInputs = new();

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
                            if (!TryToGetUIInput(out input, eventDto.dto, input.inputSystem)) { continue; }
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
                    IEnumerable<(Interaction interaction, Input input)> uiAssociations = assignedInputs
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
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, EventDto interaction)
        {
            _eventDtoInputs.Clear();

            Mouse mouse = Mouse.current;
            if (mouse != null)
            {
                BrowserControllerManager
                    .@default
                    .mouseController
                    .TryToAddInput(_eventDtoInputs, mouse.leftButton);
            }

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                BrowserControllerManager
                    .@default
                    .keyboardController
                    .TryToAddInput(_eventDtoInputs, keyboard.qKey);
                BrowserControllerManager
                    .@default
                    .keyboardController
                    .TryToAddInput(_eventDtoInputs, keyboard.eKey);
                BrowserControllerManager
                    .@default
                    .keyboardController
                    .TryToAddInput(_eventDtoInputs, keyboard.rKey);
                BrowserControllerManager
                    .@default
                    .keyboardController
                    .TryToAddInput(_eventDtoInputs, keyboard.fKey);
                BrowserControllerManager
                    .@default
                    .keyboardController
                    .TryToAddInput(_eventDtoInputs, keyboard.gKey);
            }

            BrowserControllerManager
                .@default
                .uiDeviceController
                .TryToAddInput(_eventDtoInputs, UIDevice.GetButtonPlaceholder());

            inputs = _eventDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, DrawingDto interaction)
        {
            throw new NotImplementedException();
        }


        List<Input> _booleanParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, BooleanParameterDto interaction)
        {
            _booleanParameterDtoInputs.Clear();

            BrowserControllerManager
                .@default
                .uiDeviceController
                .TryToAddInput(_booleanParameterDtoInputs, UIDevice.GetButtonPlaceholder());

            inputs = _booleanParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }

        List<Input> _floatParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, FloatParameterDto interaction)
        {
            _floatParameterDtoInputs.Clear();

            BrowserControllerManager
                .@default
                .uiDeviceController
                .TryToAddInput(_floatParameterDtoInputs, UIDevice.GetDoublePlaceholder());

            inputs = _floatParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }
        List<Input> _integerParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, IntegerParameterDto interaction)
        {
            inputs = _integerParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }

        List<Input> _floatRangeParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, FloatRangeParameterDto interaction)
        {
            inputs = _floatRangeParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }
        List<Input> _integerRangeParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, IntegerRangeParameterDto interaction)
        {
            inputs = _integerRangeParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }

        List<Input> _vector2ParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, Vector2ParameterDto interaction)
        {
            inputs = _vector2ParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }
        List<Input> _vector3ParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, Vector3ParameterDto interaction)
        {
            inputs = _vector3ParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }
        List<Input> _vector4ParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, Vector4ParameterDto interaction)
        {
            inputs = _vector4ParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }

        List<Input> _enumStringParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, EnumParameterDto<string> interaction)
        {
            inputs = _enumStringParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }
        List<Input> _stringParameterDtoInput = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, StringParameterDto interaction)
        {
            inputs = _stringParameterDtoInput.AsReadOnly();

            return inputs.Count > 0;
        }
        List<Input> _colorParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, ColorParameterDto interaction)
        {
            inputs = _colorParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }
        List<Input> _uploadFileParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, UploadFileParameterDto interaction)
        {
            inputs = _uploadFileParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }
        List<Input> _localInfoParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, LocalInfoRequestParameterDto interaction)
        {
            inputs = _localInfoParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }
    }

    class VRSelectorDataDelegate : ISelectorDataDelegate
    {
        public bool isRightSelector;

        public int toolCountLimitation => 1;

        public void AssociateInteractionAndInput(List<(Interaction interaction, Input input)> associations, ReadOnlyDictionary<Interaction, ReadOnlyCollection<Input>> inputsByInteractions)
        {
            throw new System.NotImplementedException();
        }

        List<Input> _eventDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, EventDto interaction)
        {
            _eventDtoInputs.Clear();

            throw new System.NotImplementedException();
        }
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, DrawingDto interaction)
        {
            throw new NotImplementedException();
        }


        List<Input> _booleanParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, BooleanParameterDto interaction)
        {
            _booleanParameterDtoInputs.Clear();

            throw new System.NotImplementedException();
        }

        List<Input> _floatParameterDtoInputs = new();
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, FloatParameterDto interaction)
        {
            _floatParameterDtoInputs.Clear();

            BrowserControllerManager
                .@default
                .uiDeviceController
                .TryToAddInput(_floatParameterDtoInputs, UIDevice.GetDoublePlaceholder());

            inputs = _floatParameterDtoInputs.AsReadOnly();

            return inputs.Count > 0;
        }
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, IntegerParameterDto interaction)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, FloatRangeParameterDto interaction)
        {
            throw new NotImplementedException();
        }
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, IntegerRangeParameterDto interaction)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, Vector2ParameterDto interaction)
        {
            throw new NotImplementedException();
        }
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, Vector3ParameterDto interaction)
        {
            throw new NotImplementedException();
        }
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, Vector4ParameterDto interaction)
        {
            throw new NotImplementedException();
        }

        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, EnumParameterDto<string> interaction)
        {
            throw new NotImplementedException();
        }
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, StringParameterDto interaction)
        {
            throw new NotImplementedException();
        }
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, ColorParameterDto interaction)
        {
            throw new NotImplementedException();
        }
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, UploadFileParameterDto interaction)
        {
            throw new NotImplementedException();
        }
        public bool TryGetInputsFor(out ReadOnlyCollection<Input> inputs, LocalInfoRequestParameterDto interaction)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}