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
            // This will register the mouseSelector, as well as the mouseController, the keyboardController and the UIController.
            var _ = BrowserControllerManager.@default;

            SelectorManager.@default.TryToGetSelector(out mouseSelector, BrowserControllerManager.MOUSE_ID);
            Assert.NotNull(mouseSelector);

            UIDevice.InstantiateNewUIDevice();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            for (int i = UIDevice.allUIDevices.Count -1; i >= 0; i--)
            {
                InputSystem.RemoveDevice(UIDevice.allUIDevices[i]);
            }
        }

        [SetUp]
        public void SetUp()
        {
            environmentId = 1;
            UMI3DEnvironmentLoader.DeclareNewEnvironment(environmentId, "NewEnvironment");

            EventDto event1 = new()
            {
                id = 10,
                name = "Event1",
                hold = false,
            };
            InteractionManager.@default.TryToInstantiateInteraction(out Interaction _, environmentId, event1);

            EventDto event2 = new()
            {
                id = 11,
                name = "Event2",
                hold = false,
            };
            InteractionManager.@default.TryToInstantiateInteraction(out Interaction _, environmentId, event2);

            EventDto event3 = new()
            {
                id = 12,
                name = "Event3",
                hold = false,
            };
            InteractionManager.@default.TryToInstantiateInteraction(out Interaction _, environmentId, event3);

            EventDto event4 = new()
            {
                id = 13,
                name = "Event4",
                hold = true,
            };
            InteractionManager.@default.TryToInstantiateInteraction(out Interaction _, environmentId, event4);

            EventDto event5 = new()
            {
                id = 14,
                name = "Event5",
                hold = false,
            };
            InteractionManager.@default.TryToInstantiateInteraction(out Interaction _, environmentId, event5);

            InteractableDto dto = new()
            {
                id = 100,
                nodeId = 2,
                name = "eventsTool",
                HoverEnterAnimationId = 3,
                HoverExitAnimationId = 4,
                interactions = new() { 10, 11, 12, 13, 14 }
            };
            ToolManager.@default.TryToInstantiateTool(out eventsTool, environmentId, dto);
        }

        [TearDown]
        public void TearDown()
        {
            InteractionManager.@default.TryToRemoveInteraction(environmentId, 10);
            InteractionManager.@default.TryToRemoveInteraction(environmentId, 11);
            InteractionManager.@default.TryToRemoveInteraction(environmentId, 12);
            InteractionManager.@default.TryToRemoveInteraction(environmentId, 13);
            InteractionManager.@default.TryToRemoveInteraction(environmentId, 14);
            ToolManager.@default.TryToRemoveTool(environmentId, 100);
        }

        [Test]
        public void TestScriptSimplePasses()
        {
            mouseSelector.Select(eventsTool);
        }
    }
}