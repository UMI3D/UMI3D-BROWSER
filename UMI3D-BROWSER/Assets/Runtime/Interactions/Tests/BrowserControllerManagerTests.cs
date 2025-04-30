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
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using umi3d.browserRuntime.interactions;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class BrowserControllerManagerTests
{
    public class MouseSelectorTests
    {
        class ClientServerCommunicationSelectorFake: IClientServerCommunicationSelectorDelegate
        {
            public List<AbstractBrowserRequestDto> requestDtos = new();

            public AbstractBrowserRequestDto Pull()
            {
                var request = requestDtos.FirstOrDefault();
                requestDtos.RemoveAt(0);
                return request;
            }

            public void SendRequest(AbstractBrowserRequestDto dto, bool reliable)
            {
                requestDtos.Add(dto);
            }

            public async void Animate(ulong environmentId, ulong animationId)
            {
                await Task.CompletedTask;
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
            mouseSelector = BrowserControllerManager.@default.mouseSelector;

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
                InteractionManager.@default.InstantiateOrGet(
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
                InteractionManager.@default.InstantiateOrGet(
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

            InteractionManager.@default.InstantiateOrGet(
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

            NewInputSystemManager.@default.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            mouseSelector.Deselect(tool);
            ToolManager.@default.TryToRemoveTool(environmentId, 100);
            clientSelector.requestDtos.Clear();

            foreach (var input in InputManager.@default.inputs)
            {
                UIDevice.ReleaseInput(input);
            }
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
            ToolManager.@default.InstantiateOrGet(out tool, environmentId, dto);

            mouseSelector.Select(tool);

            // Check that the request tool projected has been send to the server.
            var toolProjectedRequest = clientSelector.Pull() as ToolProjectedDto;
            var expectedToolProjectedRequest = "{\r\n" +
                "  \"$type\": \"umi3d.common.interaction.ToolProjectedDto, UMI3D.Common.InteractionSystem\",\r\n" +
                "  \"toolId\": 100,\r\n" +
                "  \"boneType\": 0,\r\n" +
                "  \"environmentId\": 1\r\n" +
                "}";
            Assert.AreEqual(expectedToolProjectedRequest, toolProjectedRequest.ToJson());

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";

                // Check that when the user interact with this input a request is send the server.
                var inputSystem = projection.input.inputSystem as NewInputSystem;
                inputSystem.OnStartedForTest(1f);
                var request = clientSelector.Pull();
                var expectedRequest = $"{{\r\n" +
                    $"  \"$type\": \"umi3d.common.interaction.EventTriggeredDto, UMI3D.Common.InteractionSystem\",\r\n" +
                    $"  \"toolId\": 100,\r\n" +
                    $"  \"id\": {projection.interaction.dto.id},\r\n" +
                    $"  \"hoveredObjectId\": 0,\r\n" +
                    $"  \"boneType\": 0,\r\n" +
                    $"  \"bonePosition\": {{\r\n" +
                    $"    \"$type\": \"umi3d.common.Vector3Dto, UMI3D.Common.Core\",\r\n" +
                    $"    \"X\": 0.0,\r\n" +
                    $"    \"Y\": 0.0,\r\n" +
                    $"    \"Z\": 0.0\r\n" +
                    $"  }},\r\n" +
                    $"  \"boneRotation\": {{\r\n" +
                    $"    \"$type\": \"umi3d.common.Vector4Dto, UMI3D.Common.Core\",\r\n" +
                    $"    \"X\": 0.0,\r\n" +
                    $"    \"Y\": 0.0,\r\n" +
                    $"    \"Z\": 0.0,\r\n" +
                    $"    \"W\": 0.0\r\n" +
                    $"  }},\r\n" +
                    $"  \"environmentId\": 1\r\n" +
                    $"}}";
                Assert.AreEqual(expectedRequest, request.ToJson());
            }

            // Check all the projections.
            // Selector, Controller, tool's name, interaction's name, control's name
            string expectation = "---- Projection ----\n" +
                "Mouse, Mouse, eventsTool, Event0, /Mouse/leftButton\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event1, /Keyboard/q\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event2, /Keyboard/e\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event3, /Keyboard/r\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event4, /Keyboard/f\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event5, /Keyboard/g\n\n";

            Assert.AreEqual(expectation, result);
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
            ToolManager.@default.InstantiateOrGet(out tool, environmentId, dto);

            mouseSelector.Select(tool);

            // Check that the request tool projected has been send to the server.
            var toolProjectedRequest = clientSelector.Pull() as ToolProjectedDto;
            var expectedToolProjectedRequest = "{\r\n" +
                "  \"$type\": \"umi3d.common.interaction.ToolProjectedDto, UMI3D.Common.InteractionSystem\",\r\n" +
                "  \"toolId\": 100,\r\n" +
                "  \"boneType\": 0,\r\n" +
                "  \"environmentId\": 1\r\n" +
                "}";
            Assert.AreEqual(expectedToolProjectedRequest, toolProjectedRequest.ToJson());

            string result = "";
            for (int i = 0; i < ProjectionManager.@default.projections.Count; i++)
            {
                Projection projection = ProjectionManager.@default.projections[i];
                result += $"{projection.debugDescription}";

                // Check that when the user interact with this input a request is send the server.
                var inputSystem = projection.input.inputSystem as NewInputSystem;
                inputSystem.OnStartedForTest(1f);
                var request = clientSelector.Pull();

                string expectedRequest = "";
                if (i == 0)
                {
                    expectedRequest = $"{{\r\n" +
                        $"  \"$type\": \"umi3d.common.interaction.EventStateChangedDto, UMI3D.Common.InteractionSystem\",\r\n" +
                        $"  \"active\": true,\r\n" +
                        $"  \"toolId\": 100,\r\n" +
                        $"  \"id\": {projection.interaction.dto.id},\r\n" +
                        $"  \"hoveredObjectId\": 0,\r\n" +
                        $"  \"boneType\": 0,\r\n" +
                        $"  \"bonePosition\": {{\r\n" +
                        $"    \"$type\": \"umi3d.common.Vector3Dto, UMI3D.Common.Core\",\r\n" +
                        $"    \"X\": 0.0,\r\n" +
                        $"    \"Y\": 0.0,\r\n" +
                        $"    \"Z\": 0.0\r\n" +
                        $"  }},\r\n" +
                        $"  \"boneRotation\": {{\r\n" +
                        $"    \"$type\": \"umi3d.common.Vector4Dto, UMI3D.Common.Core\",\r\n" +
                        $"    \"X\": 0.0,\r\n" +
                        $"    \"Y\": 0.0,\r\n" +
                        $"    \"Z\": 0.0,\r\n" +
                        $"    \"W\": 0.0\r\n" +
                        $"  }},\r\n" +
                        $"  \"environmentId\": 1\r\n" +
                        $"}}";
                    Assert.AreEqual(expectedRequest, request.ToJson());

                    inputSystem.OnCanceledForTest(0f);
                    request = clientSelector.Pull();
                    expectedRequest = $"{{\r\n" +
                        $"  \"$type\": \"umi3d.common.interaction.EventStateChangedDto, UMI3D.Common.InteractionSystem\",\r\n" +
                        $"  \"active\": false,\r\n" +
                        $"  \"toolId\": 100,\r\n" +
                        $"  \"id\": {projection.interaction.dto.id},\r\n" +
                        $"  \"hoveredObjectId\": 0,\r\n" +
                        $"  \"boneType\": 0,\r\n" +
                        $"  \"bonePosition\": {{\r\n" +
                        $"    \"$type\": \"umi3d.common.Vector3Dto, UMI3D.Common.Core\",\r\n" +
                        $"    \"X\": 0.0,\r\n" +
                        $"    \"Y\": 0.0,\r\n" +
                        $"    \"Z\": 0.0\r\n" +
                        $"  }},\r\n" +
                        $"  \"boneRotation\": {{\r\n" +
                        $"    \"$type\": \"umi3d.common.Vector4Dto, UMI3D.Common.Core\",\r\n" +
                        $"    \"X\": 0.0,\r\n" +
                        $"    \"Y\": 0.0,\r\n" +
                        $"    \"Z\": 0.0,\r\n" +
                        $"    \"W\": 0.0\r\n" +
                        $"  }},\r\n" +
                        $"  \"environmentId\": 1\r\n" +
                        $"}}";
                    Assert.AreEqual(expectedRequest, request.ToJson());
                }
                else
                {
                    expectedRequest = $"{{\r\n" +
                        $"  \"$type\": \"umi3d.common.interaction.EventTriggeredDto, UMI3D.Common.InteractionSystem\",\r\n" +
                        $"  \"toolId\": 100,\r\n" +
                        $"  \"id\": {projection.interaction.dto.id},\r\n" +
                        $"  \"hoveredObjectId\": 0,\r\n" +
                        $"  \"boneType\": 0,\r\n" +
                        $"  \"bonePosition\": {{\r\n" +
                        $"    \"$type\": \"umi3d.common.Vector3Dto, UMI3D.Common.Core\",\r\n" +
                        $"    \"X\": 0.0,\r\n" +
                        $"    \"Y\": 0.0,\r\n" +
                        $"    \"Z\": 0.0\r\n" +
                        $"  }},\r\n" +
                        $"  \"boneRotation\": {{\r\n" +
                        $"    \"$type\": \"umi3d.common.Vector4Dto, UMI3D.Common.Core\",\r\n" +
                        $"    \"X\": 0.0,\r\n" +
                        $"    \"Y\": 0.0,\r\n" +
                        $"    \"Z\": 0.0,\r\n" +
                        $"    \"W\": 0.0\r\n" +
                        $"  }},\r\n" +
                        $"  \"environmentId\": 1\r\n" +
                        $"}}";
                    Assert.AreEqual(expectedRequest, request.ToJson());
                }
            }

            // Check all the projections.
            // Selector, Controller, tool's name, interaction's name, control's name
            string expectation = "---- Projection ----\n" +
                "Mouse, Mouse, eventsTool, Event10, /Mouse/leftButton\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event0, /Keyboard/q\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event1, /Keyboard/e\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event2, /Keyboard/r\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event4, /Keyboard/f\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event5, /Keyboard/g\n\n";

            Assert.AreEqual(expectation, result);
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
            ToolManager.@default.InstantiateOrGet(out tool, environmentId, dto);

            mouseSelector.Select(tool);

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";
            }

            // Selector, Controller, tool's name, interaction's name, control's name
            string expectation = "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event0, /Keyboard/q\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event1, /Keyboard/e\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event2, /Keyboard/r\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event3, /Keyboard/f\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event4, /Keyboard/g\n\n" +
                "---- Projection ----\n" +
                "Mouse, UI, eventsTool, Event5, /UIDevice0/button2\n\n" +
                "---- Projection ----\n" +
                "Mouse, UI, eventsTool, Event6, /UIDevice0/button3\n\n";

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
            ToolManager.@default.InstantiateOrGet(out tool, environmentId, dto);

            mouseSelector.Select(tool);

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";
            }

            // Selector, Controller, tool's name, interaction's name, control's name
            string expectation = "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event10, /Keyboard/q\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event0, /Keyboard/e\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event1, /Keyboard/r\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event2, /Keyboard/f\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event4, /Keyboard/g\n\n" +
                "---- Projection ----\n" +
                "Mouse, UI, eventsTool, Event5, /UIDevice0/button2\n\n" +
                "---- Projection ----\n" +
                "Mouse, UI, eventsTool, Event6, /UIDevice0/button3\n\n";

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
            ToolManager.@default.InstantiateOrGet(out tool, environmentId, dto);

            mouseSelector.Select(tool);

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";
            }

            // Selector, Controller, tool's name, interaction's name, control's name
            string expectation = "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event10, /Keyboard/q\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event0, /Keyboard/e\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event1, /Keyboard/r\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event2, /Keyboard/f\n\n" +
                "---- Projection ----\n" +
                "Mouse, keyboard, eventsTool, Event4, /Keyboard/g\n\n" +
                "---- Projection ----\n" +
                "Mouse, UI, eventsTool, Event5, /UIDevice0/button2\n\n" +
                "---- Projection ----\n" +
                "Mouse, UI, eventsTool, Event6, /UIDevice0/button3\n\n";

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