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
using UnityEngine;
using UnityEngine.TestTools;
using umi3d.browserRuntime.interactions;
using UnityEngine.InputSystem;
using Input = umi3d.cdk.interaction.Input;
using umi3d.cdk.interaction;

public class UIDeviceTests
{
    public class GetInputFromTests
    {
        Controller uiDeviceController;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ControllerManager.@default.TryToInstantiateController(out uiDeviceController, BrowserControllerManager.UI_ID);

            UIDevice.InstantiateNewUIDevice();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            for (int i = UIDevice.allUIDevices.Count -1; i >= 0; i--)
            {
                InputSystem.RemoveDevice(UIDevice.allUIDevices[i]);
            }

            foreach (var input in InputManager.@default.inputs)
            {
                UIDevice.ReleaseInput(input);
            }
        }

        [Test]
        public void GetInputFrom_ButtonPlaceholder()
        {
            InputControl placeHolder = UIDevice.GetButtonPlaceholder();

            string result = "";
            for (int i = 0; i < 5; i++)
            {
                NewInputSystem inputSystem = new(placeHolder, InputActionType.Button);
                Input input = UIDevice.GetInputFrom(inputSystem);
                result += input.debugDescription;
            }

            string expected =
                "---- Input ----\n" +
                "UI, True, /UIDevice0/button2\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/button3\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/button4\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/button5\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice1/button1\n\n";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetInputFrom_AxisPlaceholder()
        {
            InputControl placeHolder = UIDevice.GetAxisPlaceholder();

            string result = "";
            for (int i = 0; i < 5; i++)
            {
                NewInputSystem inputSystem = new(placeHolder, InputActionType.Button);
                Input input = UIDevice.GetInputFrom(inputSystem);
                result += input.debugDescription;
            }

            string expected =
                "---- Input ----\n" +
                "UI, True, /UIDevice0/axis2\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/axis3\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/axis4\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/axis5\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice1/axis1\n\n";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetInputFrom_IntegerPlaceholder()
        {
            InputControl placeHolder = UIDevice.GetIntegerPlaceholder();

            string result = "";
            for (int i = 0; i < 5; i++)
            {
                NewInputSystem inputSystem = new(placeHolder, InputActionType.Button);
                Input input = UIDevice.GetInputFrom(inputSystem);
                result += input.debugDescription;
            }

            string expected =
                "---- Input ----\n" +
                "UI, True, /UIDevice0/integer2\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/integer3\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/integer4\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/integer5\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice1/integer1\n\n";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetInputFrom_DoublePlaceholder()
        {
            InputControl placeHolder = UIDevice.GetDoublePlaceholder();

            string result = "";
            for (int i = 0; i < 5; i++)
            {
                NewInputSystem inputSystem = new(placeHolder, InputActionType.Button);
                Input input = UIDevice.GetInputFrom(inputSystem);
                result += input.debugDescription;
            }

            string expected =
                "---- Input ----\n" +
                "UI, True, /UIDevice0/double2\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/double3\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/double4\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/double5\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice1/double1\n\n";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetInputFrom_VectorTwoPlaceholder()
        {
            InputControl placeHolder = UIDevice.GetVectorTwoPlaceholder();

            string result = "";
            for (int i = 0; i < 5; i++)
            {
                NewInputSystem inputSystem = new(placeHolder, InputActionType.Button);
                Input input = UIDevice.GetInputFrom(inputSystem);
                result += input.debugDescription;
            }

            string expected =
                "---- Input ----\n" +
                "UI, True, /UIDevice0/vectorTwo2\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/vectorTwo3\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/vectorTwo4\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/vectorTwo5\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice1/vectorTwo1\n\n";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetInputFrom_VectorThreePlaceholder()
        {
            InputControl placeHolder = UIDevice.GetVectorThreePlaceholder();

            string result = "";
            for (int i = 0; i < 5; i++)
            {
                NewInputSystem inputSystem = new(placeHolder, InputActionType.Button);
                Input input = UIDevice.GetInputFrom(inputSystem);
                result += input.debugDescription;
            }

            string expected =
                "---- Input ----\n" +
                "UI, True, /UIDevice0/vectorThree2\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/vectorThree3\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/vectorThree4\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/vectorThree5\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice1/vectorThree1\n\n";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetInputFrom_QuaternionPlaceholder()
        {
            InputControl placeHolder = UIDevice.GetQuaternionPlaceholder();

            string result = "";
            for (int i = 0; i < 5; i++)
            {
                NewInputSystem inputSystem = new(placeHolder, InputActionType.Button);
                Input input = UIDevice.GetInputFrom(inputSystem);
                result += input.debugDescription;
            }

            string expected =
                "---- Input ----\n" +
                "UI, True, /UIDevice0/quaternion2\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/quaternion3\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/quaternion4\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice0/quaternion5\n\n" +
                "---- Input ----\n" +
                "UI, True, /UIDevice1/quaternion1\n\n";

            Assert.AreEqual(expected, result);
        }
    }
}