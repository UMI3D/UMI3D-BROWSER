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
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public class ControlModel 
    {
        debug.UMI3DLogger logger = new(mainTag: nameof(ControlModel));

        public Controls_SO controls_SO;

        public void Init(Controls_SO controls_SO)
        {
            this.controls_SO = controls_SO;
        }

        public PhysicalButtonControlData GetPhysicalButton()
        {
            return controls_SO
                .physicalButtonControls
                .Find(
                control =>
                    {
                        return !control.isUsed;
                    }
                );
        }

        public UIButtonControlData GetUIButton()
        {
            var uiButton = controls_SO
                .uIButtonControls
                .Find(
                    control =>
                    {
                        return !control.isUsed;
                    }
                );
            if (uiButton == null)
            {
                uiButton = controls_SO.uIButtonControlPrefabs[0];
            }
            return uiButton;
        }
    }
}