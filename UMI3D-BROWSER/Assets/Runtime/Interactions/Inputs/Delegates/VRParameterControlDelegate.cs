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
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.browserRuntime.interaction
{
    [CreateAssetMenu(fileName = "UMI3D VR Parameter Control Delegate", menuName = "UMI3D/Interactions/Control/Delegates/VR Parameter")]
    public class VRParameterControlDelegate : AbstractControlDelegate<AbstractParameterDto>
    {
        public override AbstractControlEntity GetControl(AbstractController controller, AbstractParameterDto interaction)
        {
            throw new NotImplementedException();
        }

        protected override bool CanPerform(InputActionPhase phase)
        {
            throw new NotImplementedException();
        }
    }
}