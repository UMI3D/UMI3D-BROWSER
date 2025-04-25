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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.interactions;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class BrowserControllerManagerTests
{
    public class MouseSelectorTests
    {
        class ClientServerCommunicationSelectorFake: IClientServerCommunicationSelectorDelegate
        {
            public List<AbstractBrowserRequestDto> requestDtos = new();

            public void SendRequest(AbstractBrowserRequestDto dto, bool reliable)
            {
                requestDtos.Add(dto);
            }

            public async void Animate(Tool tool, ulong animationId)
            {

            }
        }

        class BoneRepresentableFake : IBoneRepresentable
        {
            public uint bone => 0;

            public Vector3 bonePosition => Vector3.zero;

            public Vector4 boneRotation => Vector4.zero;
        }

        ulong environmentId;
        Selector mouseSelector;
        ClientServerCommunicationSelectorFake clientSelector;
        BoneRepresentableFake boneRepresentable;
        Tool tool;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Instantiate BrowserControllerManager.
            // This will register the selectors, as well as the controllers.
            var _ = BrowserControllerManager.@default;

            SelectorManager.@default.TryToGetSelector(out mouseSelector, BrowserControllerManager.MOUSE_ID);
            Assert.NotNull(mouseSelector);
            clientSelector = new();
            mouseSelector.clientServerCommunicationSelectorDelegate = clientSelector;
            boneRepresentable = new BoneRepresentableFake();
            mouseSelector.boneRepresentable = boneRepresentable;

            UIDevice.InstantiateNewUIDevice();

            environmentId = 1;
            UMI3DEnvironmentLoader.DeclareNewEnvironment(environmentId, "NewEnvironment");

            ulong count = 10;
            for (ulong i =  0; i < count; i++)
            {
                InteractionManager.@default.TryToInstantiateInteraction(
                    out Interaction _, 
                    environmentId, 
                    new EventDto()
                    {
                        id = 10 + i,
                        name = $"Event{i}",
                        hold = false,
                    }
                );
            }

            for (ulong i =  0; i < count; i++)
            {
                InteractionManager.@default.TryToInstantiateInteraction(
                    out Interaction _, 
                    environmentId, 
                    new EventDto()
                    {
                        id = 10 + count + i,
                        name = $"Event{count + i}",
                        hold = true,
                    }
                );
            }

            InteractionManager.@default.TryToInstantiateInteraction(
                out Interaction _, 
                environmentId, 
                new BooleanParameterDto()
                {
                    id = 40,
                    name = $"Boolean",
                    value = true
                }
            );
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            for (int i = UIDevice.allUIDevices.Count -1; i >= 0; i--)
            {
                InputSystem.RemoveDevice(UIDevice.allUIDevices[i]);
            }

            for (ulong i = 0; i < 20; i++)
            {
                InteractionManager.@default.TryToRemoveInteraction(environmentId, 10 + i);
            }

            InteractionManager.@default.TryToRemoveInteraction(environmentId, 40);
        }

        [TearDown]
        public void TearDown()
        {
            mouseSelector.Deselect(tool);
            ToolManager.@default.TryToRemoveTool(environmentId, 100);
            clientSelector.requestDtos.Clear();
        }

        [Test]
        public void GivenEvents_WhenSelectedWithMouse_ThenProjectedOnMouseAndKeyboard()
        {
            InteractableDto dto = new()
            {
                id = 100,
                nodeId = 2,
                name = "eventsTool",
                HoverEnterAnimationId = 3,
                HoverExitAnimationId = 4,
                interactions = new() { 10, 11, 12, 13, 14, 15 }
            };
            ToolManager.@default.TryToInstantiateTool(out tool, environmentId, dto);

            mouseSelector.Select(tool);

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";
                result += "\n";
            }

            // Selector, Controller, tool's name, interaction's name, control's name
            string expectation = "---- Projection ----\n" +
                "Mouse, Mouse, eventsTool, Event0, leftButton\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event1, q\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event2, e\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event3, r\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event4, f\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event5, g\n\n";

            Assert.AreEqual(expectation, result);

            var toolProjectedRequest = clientSelector.requestDtos[0] as ToolProjectedDto;
            var expectedToolProjectedRequest = "{\r\n" +
                "  \"$type\": \"umi3d.common.interaction.ToolProjectedDto, UMI3D.Common.InteractionSystem\",\r\n" +
                "  \"toolId\": 100,\r\n" +
                "  \"boneType\": 0,\r\n" +
                "  \"environmentId\": 1\r\n" +
                "}";

            Assert.AreEqual(expectedToolProjectedRequest, toolProjectedRequest.ToJson());
        }

        [Test]
        public void GivenEventsAndOneHold_WhenSelectedWithMouse_ThenProjectedOnMouseAndKeyboard()
        {
            InteractableDto dto = new()
            {
                id = 100,
                nodeId = 2,
                name = "eventsTool",
                HoverEnterAnimationId = 3,
                HoverExitAnimationId = 4,
                interactions = new() { 10, 11, 12, 20, 14, 15 }
            };
            ToolManager.@default.TryToInstantiateTool(out tool, environmentId, dto);

            mouseSelector.Select(tool);

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";
                result += "\n";
            }

            // Selector, Controller, tool's name, interaction's name, control's name
            string expectation = "---- Projection ----\n" +
                "Mouse, Mouse, eventsTool, Event10, leftButton\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event0, q\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event1, e\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event2, r\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event4, f\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event5, g\n\n";

            Assert.AreEqual(expectation, result);
            
            var toolProjectedRequest = clientSelector.requestDtos[0] as ToolProjectedDto;
            var expectedToolProjectedRequest = "{\r\n" +
                "  \"$type\": \"umi3d.common.interaction.ToolProjectedDto, UMI3D.Common.InteractionSystem\",\r\n" +
                "  \"toolId\": 100,\r\n" +
                "  \"boneType\": 0,\r\n" +
                "  \"environmentId\": 1\r\n" +
                "}";

            Assert.AreEqual(expectedToolProjectedRequest, toolProjectedRequest.ToJson());
        }

        [Test]
        public void GivenMoreEvents_WhenSelectedWithMouse_ThenProjectedOnKeyboardAndUI()
        {
            InteractableDto dto = new()
            {
                id = 100,
                nodeId = 2,
                name = "eventsTool",
                HoverEnterAnimationId = 3,
                HoverExitAnimationId = 4,
                interactions = new() { 10, 11, 12, 13, 14, 15, 16 }
            };
            ToolManager.@default.TryToInstantiateTool(out tool, environmentId, dto);

            mouseSelector.Select(tool);

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";
                result += "\n";
            }

            // Selector, Controller, tool's name, interaction's name, control's name
            string expectation = "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event0, q\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event1, e\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event2, r\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event3, f\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event4, g\n\n" +
                "---- Projection ----\n" +
                "Mouse, UI, eventsTool, Event5, button2\n\n" +
                "---- Projection ----\n" +
                "Mouse, UI, eventsTool, Event6, button3\n\n";

            Assert.AreEqual(expectation, result);
            
            var toolProjectedRequest = clientSelector.requestDtos[0] as ToolProjectedDto;
            var expectedToolProjectedRequest = "{\r\n" +
                "  \"$type\": \"umi3d.common.interaction.ToolProjectedDto, UMI3D.Common.InteractionSystem\",\r\n" +
                "  \"toolId\": 100,\r\n" +
                "  \"boneType\": 0,\r\n" +
                "  \"environmentId\": 1\r\n" +
                "}";

            Assert.AreEqual(expectedToolProjectedRequest, toolProjectedRequest.ToJson());
        }

        [Test]
        public void GivenMoreEventsAndOneHold_WhenSelectedWithMouse_ThenProjectedOnKeyboardAndUI()
        {
            InteractableDto dto = new()
            {
                id = 100,
                nodeId = 2,
                name = "eventsTool",
                HoverEnterAnimationId = 3,
                HoverExitAnimationId = 4,
                interactions = new() { 10, 11, 12, 20, 14, 15, 16 }
            };
            ToolManager.@default.TryToInstantiateTool(out tool, environmentId, dto);

            mouseSelector.Select(tool);

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";
                result += "\n";
            }

            // Selector, Controller, tool's name, interaction's name, control's name
            string expectation = "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event10, q\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event0, e\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event1, r\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event2, f\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event4, g\n\n" +
                "---- Projection ----\n" +
                "Mouse, UI, eventsTool, Event5, button2\n\n" +
                "---- Projection ----\n" +
                "Mouse, UI, eventsTool, Event6, button3\n\n";

            Assert.AreEqual(expectation, result);
            
            var toolProjectedRequest = clientSelector.requestDtos[0] as ToolProjectedDto;
            var expectedToolProjectedRequest = "{\r\n" +
                "  \"$type\": \"umi3d.common.interaction.ToolProjectedDto, UMI3D.Common.InteractionSystem\",\r\n" +
                "  \"toolId\": 100,\r\n" +
                "  \"boneType\": 0,\r\n" +
                "  \"environmentId\": 1\r\n" +
                "}";

            Assert.AreEqual(expectedToolProjectedRequest, toolProjectedRequest.ToJson());
        }

        [Test]
        public void GivenParameterDtos_WhenSelectedWithMouse_ThenProjectedOnUI()
        {
            InteractableDto dto = new()
            {
                id = 100,
                nodeId = 2,
                name = "ParametersTool",
                HoverEnterAnimationId = 3,
                HoverExitAnimationId = 4,
                interactions = new() { 40 } 
            };
            ToolManager.@default.TryToInstantiateTool(out tool, environmentId, dto);

            mouseSelector.Select(tool);

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";
                result += "\n";
            }

            // Selector, Controller, tool's name, interaction's name, control's name
            string expectation = "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event10, q\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event0, e\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event1, r\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event2, f\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event4, g\n\n" +
                "---- Projection ----\n" +
                "Mouse, UI, eventsTool, Event5, button2\n\n" +
                "---- Projection ----\n" +
                "Mouse, UI, eventsTool, Event6, button3\n\n";

            //Assert.AreEqual(expectation, result);
            UnityEngine.Debug.Log($"{result}");
            
            var toolProjectedRequest = clientSelector.requestDtos[0] as ToolProjectedDto;
            var expectedToolProjectedRequest = "{\r\n" +
                "  \"$type\": \"umi3d.common.interaction.ToolProjectedDto, UMI3D.Common.InteractionSystem\",\r\n" +
                "  \"toolId\": 100,\r\n" +
                "  \"boneType\": 0,\r\n" +
                "  \"environmentId\": 1\r\n" +
                "}";

            Assert.AreEqual(expectedToolProjectedRequest, toolProjectedRequest.ToJson());
        }
    }
}