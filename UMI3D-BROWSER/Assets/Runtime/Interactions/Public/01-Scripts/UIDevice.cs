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
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace umi3d.browserRuntime.interactions
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [InputControlLayout(displayName = "UIDevice")]
    public class UIDevice : InputDevice
    {
        #region Current and All Devices

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
            _allUIDevices.Add(this);
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            _allUIDevices.Remove(this);
        }

        #endregion

        #region Initialization

        static UIDevice()
        {
            InputSystem.RegisterLayout<UIDevice>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeInPlayer() { }

        static UIDevice InstantiateNewUIDevice()
        {
            return InputSystem.AddDevice<UIDevice>($"UIDevice{_allUIDevices.Count}");
        }

        #endregion

        public ButtonControl button1 { get; private set; }
        public ButtonControl button2 { get; private set; }
        public ButtonControl button3 { get; private set; }
        public ButtonControl button4 { get; private set; }
        public ButtonControl button5 { get; private set; }

        public AxisControl axis1 { get; private set; }
        public AxisControl axis2 { get; private set; }
        public AxisControl axis3 { get; private set; }
        public AxisControl axis4 { get; private set; }
        public AxisControl axis5 { get; private set; }

        public IntegerControl integer1 { get; private set; }
        public IntegerControl integer2 { get; private set; }
        public IntegerControl integer3 { get; private set; }
        public IntegerControl integer4 { get; private set; }
        public IntegerControl integer5 { get; private set; }

        public DoubleControl double1 { get; private set; }
        public DoubleControl double2 { get; private set; }
        public DoubleControl double3 { get; private set; }
        public DoubleControl double4 { get; private set; }
        public DoubleControl double5 { get; private set; }

        public Vector2Control vectorTwo1 { get; private set; }
        public Vector2Control vectorTwo2 { get; private set; }
        public Vector2Control vectorTwo3 { get; private set; }
        public Vector2Control vectorTwo4 { get; private set; }
        public Vector2Control vectorTwo5 { get; private set; }

        public Vector3Control vectorThree1 { get; private set; }
        public Vector3Control vectorThree2 { get; private set; }
        public Vector3Control vectorThree3 { get; private set; }
        public Vector3Control vectorThree4 { get; private set; }
        public Vector3Control vectorThree5 { get; private set; }

        public QuaternionControl quaternion1 { get; private set; }
        public QuaternionControl quaternion2 { get; private set; }
        public QuaternionControl quaternion3 { get; private set; }
        public QuaternionControl quaternion4 { get; private set; }
        public QuaternionControl quaternion5 { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            button1 = GetChildControl<ButtonControl>("button1");
            button2 = GetChildControl<ButtonControl>("button2");
            button3 = GetChildControl<ButtonControl>("button3");
            button4 = GetChildControl<ButtonControl>("button4");
            button5 = GetChildControl<ButtonControl>("button5");

            axis1 = GetChildControl<AxisControl>("axis1");
            axis2 = GetChildControl<AxisControl>("axis2");
            axis3 = GetChildControl<AxisControl>("axis3");
            axis4 = GetChildControl<AxisControl>("axis4");
            axis5 = GetChildControl<AxisControl>("axis5");

            integer1 = GetChildControl<IntegerControl>("integer1");
            integer2 = GetChildControl<IntegerControl>("integer2");
            integer3 = GetChildControl<IntegerControl>("integer3");
            integer4 = GetChildControl<IntegerControl>("integer4");
            integer5 = GetChildControl<IntegerControl>("integer5");

            double1 = GetChildControl<DoubleControl>("double1");
            double2 = GetChildControl<DoubleControl>("double2");
            double3 = GetChildControl<DoubleControl>("double3");
            double4 = GetChildControl<DoubleControl>("double4");
            double5 = GetChildControl<DoubleControl>("double5");

            vectorTwo1 = GetChildControl<Vector2Control>("vectorTwo1");
            vectorTwo2 = GetChildControl<Vector2Control>("vectorTwo2");
            vectorTwo3 = GetChildControl<Vector2Control>("vectorTwo3");
            vectorTwo4 = GetChildControl<Vector2Control>("vectorTwo4");
            vectorTwo5 = GetChildControl<Vector2Control>("vectorTwo5");

            vectorThree1 = GetChildControl<Vector3Control>("vectorThree1");
            vectorThree2 = GetChildControl<Vector3Control>("vectorThree2");
            vectorThree3 = GetChildControl<Vector3Control>("vectorThree3");
            vectorThree4 = GetChildControl<Vector3Control>("vectorThree4");
            vectorThree5 = GetChildControl<Vector3Control>("vectorThree5");

            quaternion1 = GetChildControl<QuaternionControl>("quaternion1");
            quaternion2 = GetChildControl<QuaternionControl>("quaternion2");
            quaternion3 = GetChildControl<QuaternionControl>("quaternion3");
            quaternion4 = GetChildControl<QuaternionControl>("quaternion4");
            quaternion5 = GetChildControl<QuaternionControl>("quaternion5");

            axis1 = GetChildControl<AxisControl>("axis1");
            axis2 = GetChildControl<AxisControl>("axis2");
            axis3 = GetChildControl<AxisControl>("axis3");
            axis4 = GetChildControl<AxisControl>("axis4");
            axis5 = GetChildControl<AxisControl>("axis5");
        }
    }
}