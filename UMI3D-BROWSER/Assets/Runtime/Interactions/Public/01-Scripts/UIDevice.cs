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
using umi3d.cdk.interaction;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using Input = umi3d.cdk.interaction.Input;
using InputManager = umi3d.cdk.interaction.InputManager;

namespace umi3d.browserRuntime.interactions
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [InputControlLayout(displayName = "UIDevice", stateType = typeof(UIDeviceState), isGenericTypeOfDevice = true)]
    public class UIDevice : InputDevice
    {
        #region Current and All Devices

        static UIDevice first { get; set; }
        public static UIDevice current { get; private set; }

        static List<UIDevice> _allUIDevices = new();
        public static ReadOnlyCollection<UIDevice> allUIDevices => _allUIDevices.AsReadOnly();

        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }

        protected override void OnAdded()
        {
            base.OnAdded();
            if (_allUIDevices.Count == 0)
            {
                first = this;
            }
            _allUIDevices.Add(this);
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            _allUIDevices.Remove(this);
            if (_allUIDevices.Count == 0)
            {
                first = null;
            }
        }

        #endregion

        #region Initialization

        static UIDevice()
        {
            InputSystem.RegisterLayout<UIDevice>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeInPlayer() { }

        public static UIDevice InstantiateNewUIDevice()
        {
            return InputSystem.AddDevice<UIDevice>($"UIDevice{_allUIDevices.Count}");
        }

        #endregion

        #region Input management

        static Controller uiController;
        static List<IInputSystem> _activeInputControls = new();
        public static Input GetInputFrom(IInputSystem inputSystem)
        {
            if (uiController == null)
            {
                uiController = BrowserControllerManager.@default.uiDeviceController;
            }

            if (inputSystem is NewInputSystem newInputSystem)
            {
                return GetInputFrom(newInputSystem);
            }
            else
            {
                // TODO
                UnityEngine.Debug.LogError($"Error: unhandled case.");
                return null;
            }
        }
        static Input GetInputFrom(NewInputSystem inputSystem)
        {
            bool TryToFindInput(out Input input, InputControl control)
            {
                NewInputSystemManager.@default.TryToInstantiateInput(
                    out NewInputSystem concreteInputSystem, 
                    control
                );
                if (!_activeInputControls.Contains(concreteInputSystem))
                {
                    // TODO: improve

                    InputManager.@default.InstantiateOrGet(
                        out input,
                        concreteInputSystem
                    );

                    uiController.Add(input);
                    _activeInputControls.Add(concreteInputSystem);
                    return true;
                }

                input = null;
                return false;
            }

            Input input;
            Input FindInput(Func<UIDevice, InputControl[]> getControls)
            {
                foreach (var device in _allUIDevices)
                {
                    if (device != first && TryToFindInput(out input, getControls(device)[0]))
                    {
                        return input;
                    }
                    else
                    {
                        for (int i = 1; i < getControls(device).Length; i++)
                        {
                            if (TryToFindInput(out input, getControls(device)[i]))
                            {
                                return input;
                            }
                        }
                    }
                }

                UIDevice newDevice = InstantiateNewUIDevice();
                if (TryToFindInput(out input, getControls(newDevice)[0]))
                {
                    return input;
                }
                else
                {
                    throw new Exception("No input found.");
                }
            }

            InputControl placeholder = inputSystem.control;
            if (placeholder == first.button1)
            {
                return FindInput(getControls: device => device.buttons);
            }
            else if (placeholder == first.axis1)
            {
                return FindInput(getControls: device => device.axes);
            }
            else if (placeholder == first.integer1)
            {
                return FindInput(getControls: device => device.integers);
            }
            else if (placeholder == first.double1)
            {
                return FindInput(getControls: device => device.doubles);
            }
            else if (placeholder == first.vectorTwo1)
            {
                return FindInput(getControls: device => device.vectorTwos);
            }
            else if (placeholder == first.vectorThree1)
            {
                return FindInput(getControls: device => device.vectorThrees);
            }
            else if (placeholder == first.quaternion1)
            {
                return FindInput(getControls: device => device.quaternions);
            }
            else
            {
                throw new Exception($"[UIDevice] Exception: unhandled case {placeholder.name}");
            }
        }
        public static void ReleaseInput(Input input)
        {
            _activeInputControls.Remove(input.inputSystem);
        }

        public static ButtonControl GetButtonPlaceholder()
        {
            return first.button1;
        }

        public static AxisControl GetAxisPlaceholder()
        {
            return first.axis1;
        }

        public static IntegerControl GetIntegerPlaceholder()
        {
            return first.integer1;
        }

        public static DoubleControl GetDoublePlaceholder()
        {
            return first.double1;
        }

        public static Vector2Control GetVectorTwoPlaceholder()
        {
            return first.vectorTwo1;
        }

        public static Vector3Control GetVectorThreePlaceholder()
        {
            return first.vectorThree1;
        }

        public static QuaternionControl GetQuaternionPlaceholder()
        {
            return first.quaternion1;
        }

        #endregion

        public ButtonControl button1 { get; private set; }
        public ButtonControl button2 { get; private set; }
        public ButtonControl button3 { get; private set; }
        public ButtonControl button4 { get; private set; }
        public ButtonControl button5 { get; private set; }
        ButtonControl[] buttons;

        public AxisControl axis1 { get; private set; }
        public AxisControl axis2 { get; private set; }
        public AxisControl axis3 { get; private set; }
        public AxisControl axis4 { get; private set; }
        public AxisControl axis5 { get; private set; }
        AxisControl[] axes;

        public IntegerControl integer1 { get; private set; }
        public IntegerControl integer2 { get; private set; }
        public IntegerControl integer3 { get; private set; }
        public IntegerControl integer4 { get; private set; }
        public IntegerControl integer5 { get; private set; }
        IntegerControl[] integers;

        public DoubleControl double1 { get; private set; }
        public DoubleControl double2 { get; private set; }
        public DoubleControl double3 { get; private set; }
        public DoubleControl double4 { get; private set; }
        public DoubleControl double5 { get; private set; }
        DoubleControl[] doubles;

        public Vector2Control vectorTwo1 { get; private set; }
        public Vector2Control vectorTwo2 { get; private set; }
        public Vector2Control vectorTwo3 { get; private set; }
        public Vector2Control vectorTwo4 { get; private set; }
        public Vector2Control vectorTwo5 { get; private set; }
        Vector2Control[] vectorTwos;

        public Vector3Control vectorThree1 { get; private set; }
        public Vector3Control vectorThree2 { get; private set; }
        public Vector3Control vectorThree3 { get; private set; }
        public Vector3Control vectorThree4 { get; private set; }
        public Vector3Control vectorThree5 { get; private set; }
        Vector3Control[] vectorThrees;

        public QuaternionControl quaternion1 { get; private set; }
        public QuaternionControl quaternion2 { get; private set; }
        public QuaternionControl quaternion3 { get; private set; }
        public QuaternionControl quaternion4 { get; private set; }
        public QuaternionControl quaternion5 { get; private set; }
        QuaternionControl[] quaternions;

        protected override void FinishSetup()
        {
            button1 = GetChildControl<ButtonControl>("button1");
            button2 = GetChildControl<ButtonControl>("button2");
            button3 = GetChildControl<ButtonControl>("button3");
            button4 = GetChildControl<ButtonControl>("button4");
            button5 = GetChildControl<ButtonControl>("button5");
            buttons = new ButtonControl[5] { button1, button2, button3, button4, button5 };

            axis1 = GetChildControl<AxisControl>("axis1");
            axis2 = GetChildControl<AxisControl>("axis2");
            axis3 = GetChildControl<AxisControl>("axis3");
            axis4 = GetChildControl<AxisControl>("axis4");
            axis5 = GetChildControl<AxisControl>("axis5");
            axes = new AxisControl[5] { axis1, axis2, axis3, axis4, axis5 };

            integer1 = GetChildControl<IntegerControl>("integer1");
            integer2 = GetChildControl<IntegerControl>("integer2");
            integer3 = GetChildControl<IntegerControl>("integer3");
            integer4 = GetChildControl<IntegerControl>("integer4");
            integer5 = GetChildControl<IntegerControl>("integer5");
            integers = new IntegerControl[5] { integer1, integer2, integer3, integer4, integer5 };

            double1 = GetChildControl<DoubleControl>("double1");
            double2 = GetChildControl<DoubleControl>("double2");
            double3 = GetChildControl<DoubleControl>("double3");
            double4 = GetChildControl<DoubleControl>("double4");
            double5 = GetChildControl<DoubleControl>("double5");
            doubles = new DoubleControl[5] { double1, double2, double3, double4, double5 };

            vectorTwo1 = GetChildControl<Vector2Control>("vectorTwo1");
            vectorTwo2 = GetChildControl<Vector2Control>("vectorTwo2");
            vectorTwo3 = GetChildControl<Vector2Control>("vectorTwo3");
            vectorTwo4 = GetChildControl<Vector2Control>("vectorTwo4");
            vectorTwo5 = GetChildControl<Vector2Control>("vectorTwo5");
            vectorTwos = new Vector2Control[5] { vectorTwo1, vectorTwo2, vectorTwo3, vectorTwo4, vectorTwo5 };

            vectorThree1 = GetChildControl<Vector3Control>("vectorThree1");
            vectorThree2 = GetChildControl<Vector3Control>("vectorThree2");
            vectorThree3 = GetChildControl<Vector3Control>("vectorThree3");
            vectorThree4 = GetChildControl<Vector3Control>("vectorThree4");
            vectorThree5 = GetChildControl<Vector3Control>("vectorThree5");
            vectorThrees = new Vector3Control[5] { vectorThree1, vectorThree2, vectorThree3, vectorThree4, vectorThree5 };

            quaternion1 = GetChildControl<QuaternionControl>("quaternion1");
            quaternion2 = GetChildControl<QuaternionControl>("quaternion2");
            quaternion3 = GetChildControl<QuaternionControl>("quaternion3");
            quaternion4 = GetChildControl<QuaternionControl>("quaternion4");
            quaternion5 = GetChildControl<QuaternionControl>("quaternion5");
            quaternions = new QuaternionControl[5] { quaternion1, quaternion2, quaternion3, quaternion4, quaternion5 };

            base.FinishSetup();
        }
    }

    public struct UIDeviceState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('U', 'I', 'C', 'T');

        [InputControl(name = "button1", layout = "Button", bit = 0u)]
        [InputControl(name = "button2", layout = "Button", bit = 1u)]
        [InputControl(name = "button3", layout = "Button", bit = 2u)]
        [InputControl(name = "button4", layout = "Button", bit = 3u)]
        [InputControl(name = "button5", layout = "Button", bit = 4u)]
        public ushort buttons;

        [InputControl(name = "axis1", layout = "Analog", defaultState = 1f)]
        public float axis1;
        [InputControl(name = "axis2", layout = "Analog", defaultState = 1f)]
        public float axis2;
        [InputControl(name = "axis3", layout = "Analog", defaultState = 1f)]
        public float axis3;
        [InputControl(name = "axis4", layout = "Analog", defaultState = 1f)]
        public float axis4;
        [InputControl(name = "axis5", layout = "Analog", defaultState = 1f)]
        public float axis5;

        [InputControl(name = "integer1", layout = "Integer")]
        public int integer1;
        [InputControl(name = "integer2", layout = "Integer")]
        public int integer2;
        [InputControl(name = "integer3", layout = "Integer")]
        public int integer3;
        [InputControl(name = "integer4", layout = "Integer")]
        public int integer4;
        [InputControl(name = "integer5", layout = "Integer")]
        public int integer5;

        [InputControl(name = "double1", layout = "Double")]
        public double double1;
        [InputControl(name = "double2", layout = "Double")]
        public double double2;
        [InputControl(name = "double3", layout = "Double")]
        public double double3;
        [InputControl(name = "double4", layout = "Double")]
        public double double4;
        [InputControl(name = "double5", layout = "Double")]
        public double double5;

        [InputControl(name = "vectorTwo1", layout = "Vector2")]
        public Vector2 vectorTwo1;
        [InputControl(name = "vectorTwo2", layout = "Vector2")]
        public Vector2 vectorTwo2;
        [InputControl(name = "vectorTwo3", layout = "Vector2")]
        public Vector2 vectorTwo3;
        [InputControl(name = "vectorTwo4", layout = "Vector2")]
        public Vector2 vectorTwo4;
        [InputControl(name = "vectorTwo5", layout = "Vector2")]
        public Vector2 vectorTwo5;

        [InputControl(name = "vectorThree1", layout = "Vector3")]
        public Vector3 vectorThree1;
        [InputControl(name = "vectorThree2", layout = "Vector3")]
        public Vector3 vectorThree2;
        [InputControl(name = "vectorThree3", layout = "Vector3")]
        public Vector3 vectorThree3;
        [InputControl(name = "vectorThree4", layout = "Vector3")]
        public Vector3 vectorThree4;
        [InputControl(name = "vectorThree5", layout = "Vector3")]
        public Vector3 vectorThree5;

        [InputControl(name = "quaternion1", layout = "Quaternion")]
        public Quaternion quaternion1;
        [InputControl(name = "quaternion2", layout = "Quaternion")]
        public Quaternion quaternion2;
        [InputControl(name = "quaternion3", layout = "Quaternion")]
        public Quaternion quaternion3;
        [InputControl(name = "quaternion4", layout = "Quaternion")]
        public Quaternion quaternion4;
        [InputControl(name = "quaternion5", layout = "Quaternion")]
        public Quaternion quaternion5;
    }
}