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
    public class VRManipulationControlDelegate : IControlManipulationDelegate
    {
        IControlManipulationDelegate manipulationDelegate;

        public VRManipulationControlDelegate(IControlManipulationDelegate manipulationDelegate)
        {
            this.manipulationDelegate = manipulationDelegate;
        }

        public DofGroupDto Dof { get => manipulationDelegate.Dof; set => manipulationDelegate.Dof = value; }

        public void Associate(
            UMI3DController controller,
            AbstractControlEntity control,
            ulong environmentId,
            AbstractInteractionDto interaction,
            ulong toolId,
            ulong hoveredObjectId
        )
        {
            manipulationDelegate.Associate(controller, control, environmentId, interaction, toolId, hoveredObjectId);
        }

        public bool CanPerform(System.Object value)
        {
            throw new System.NotImplementedException();
        }

        public void Dissociate(AbstractControlEntity control)
        {
            manipulationDelegate.Dissociate(control);
        }

        public DofGroupOptionDto FindBest(DofGroupOptionDto[] options)
        {
            return options[0];
        }

        public AbstractControlEntity GetControl(UMI3DController controller, ManipulationDto interaction)
        {
            return manipulationDelegate.GetControl(controller, interaction);
        }
    }
}