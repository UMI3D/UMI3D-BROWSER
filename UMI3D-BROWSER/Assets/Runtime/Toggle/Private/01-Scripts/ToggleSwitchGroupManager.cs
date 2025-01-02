/*
Copyright 2019 - 2024 Inetum

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
using UnityEngine;

namespace umi3d.browserRuntime.ui.toggle
{
    [AddComponentMenu("UMI3D_UI/Toggle Switch Group", 30)]
    public class ToggleSwitchGroupManager : MonoBehaviour
    {
        [Header("Start Value")]
        [SerializeField] private ToggleSwitch _initialToggleSwitch;

        [Header("Toggle Options")]
        [SerializeField] private bool _allCanBeToggleOff;

        private List<ToggleSwitch> _toggleSwitchies = new();

        private void Awake()
        {
            ToggleSwitch[] toggleSwitches = GetComponentsInChildren<ToggleSwitch>();
            foreach (ToggleSwitch toggleSwitch in toggleSwitches)
                RegisterToggleButtonToGroup(toggleSwitch);
        }

        private void RegisterToggleButtonToGroup(ToggleSwitch toggleSwitch)
        {
            if (_toggleSwitchies.Contains(toggleSwitch)) 
                return;

            toggleSwitch.onToggleOff += () => ToggleGroup(toggleSwitch);
            toggleSwitch.onToggleOn += () => ToggleGroup(toggleSwitch);
            _toggleSwitchies.Add(toggleSwitch);
        }

        private void Start()
        {
            foreach (var button in _toggleSwitchies) 
            {
                if (!button.CurrentValue) 
                    return;
            }

            if (_allCanBeToggleOff) 
                return;

            if (_initialToggleSwitch != null)
                _initialToggleSwitch.SetValue(true);
            else
                _toggleSwitchies[0].SetValue(true);

        }

        internal void ToggleGroup(ToggleSwitch toggleSwitch)
        {
            if (_toggleSwitchies.Count <= 1) 
                return;

            if (_allCanBeToggleOff && toggleSwitch.CurrentValue)
            {
                foreach(var button in _toggleSwitchies)
                {
                    if(button == null || button == toggleSwitch) 
                        continue;

                    button.SetValue(false);
                }
            }
            else
            {
                foreach(var button in _toggleSwitchies)
                {
                    if (button == null) 
                        continue;

                    button.SetValue(button == toggleSwitch);
                }
            }
        }
    }
}