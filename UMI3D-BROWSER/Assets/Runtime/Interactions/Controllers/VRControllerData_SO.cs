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
using umi3d.cdk.interaction;
using umi3d.cdk.userCapture.tracking;
using UnityEngine;

namespace umi3d.browserRuntime.interaction
{
    [CreateAssetMenu(fileName = "UMI3D Controller Data For [ControllerName]", menuName = "UMI3D/Interactions/Controller/VR Controller Data")]
    public class VRControllerData_SO : AbstractControllerData_SO
    {
        public enum ControllerType
        {
            LeftHand,
            RightHand,
        }

        public ControllerType type;
        /// <summary>
        /// Associated bone.
        /// </summary>
        public Tracker bone;

        public override uint BoneType
        {
            get
            {
                return bone.BoneType;
            }
        }

        public override Transform BoneTransform
        {
            get
            {
                return bone.transform;
            }
        }

        public override Transform ManipulationTransform
        {
            get
            {
                return bone.transform;
            }
        }
    }
}