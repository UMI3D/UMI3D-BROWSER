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
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class BrowserControllerManagerTests
{
    public class MouseSelectorTests
    {
        ulong environmentId;
        Selector mouseSelector;
        Tool eventsTool;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Instantiate BrowserControllerManager.
            // This will register the selectors, as well as the controllers.
            var _ = BrowserControllerManager.@default;

            SelectorManager.@default.TryToGetSelector(out mouseSelector, BrowserControllerManager.MOUSE_ID);
            Assert.NotNull(mouseSelector);

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
        }

        [TearDown]
        public void TearDown()
        {
            mouseSelector.Deselect(eventsTool);
            ToolManager.@default.TryToRemoveTool(environmentId, 100);
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
            ToolManager.@default.TryToInstantiateTool(out eventsTool, environmentId, dto);

            mouseSelector.Select(eventsTool);

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";
                result += "\n";
            }

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
            ToolManager.@default.TryToInstantiateTool(out eventsTool, environmentId, dto);

            mouseSelector.Select(eventsTool);

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";
                result += "\n";
            }

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
            ToolManager.@default.TryToInstantiateTool(out eventsTool, environmentId, dto);

            mouseSelector.Select(eventsTool);

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";
                result += "\n";
            }

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
            ToolManager.@default.TryToInstantiateTool(out eventsTool, environmentId, dto);

            mouseSelector.Select(eventsTool);

            string result = "";
            foreach (Projection projection in ProjectionManager.@default.projections)
            {
                result += $"{projection.debugDescription}";
                result += "\n";
            }

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
        }
    }
}