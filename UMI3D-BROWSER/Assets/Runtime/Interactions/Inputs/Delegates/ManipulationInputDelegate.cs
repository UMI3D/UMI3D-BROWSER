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

namespace umi3d.browserRuntime.interaction
{
    [CreateAssetMenu(fileName = "UMI3D Manipulation Input Delegate", menuName = "UMI3D/Interactions/Input Delegates/Manipulation Input Delegate")]
    public class ManipulationInputDelegate : AbstractManipulationInputDelegate
    {
        public override void Associate(
            AbstractControlData control,
            ulong environmentId,
            AbstractInteractionDto interaction,
            ulong toolId,
            ulong hoveredObjectId
        )
        {
            if (interaction is not ManipulationDto manipInteraction)
            {
                throw new Exception($"[UMI3D] Control: Interaction is not an {nameof(ManipulationDto)}.");
            }

            foreach (DofGroupOptionDto group in manipInteraction.dofSeparationOptions)
            {
                foreach (DofGroupDto sep in group.separations)
                {
                    //if (IsCompatibleWith(sep.dofs))
                    //{
                    //    Associate(
                    //        environmentId,
                    //        toolId,
                    //        hoveredObjectId,
                    //        manipInteraction,
                    //        sep.dofs
                    //    );
                    //    return;
                    //}
                }
            }
        }

        public void Associate(
            ulong environmentId,
            ulong toolId,
            ulong hoveredObjectId,
            ManipulationDto interaction,
            DofGroupEnum dof
        )
        {

        }

        public override DofGroupOptionDto FindBest(DofGroupOptionDto[] options)
        {
            throw new NotImplementedException();
        }

        public override Guid? GetControlId(ManipulationDto interaction, bool unused = true)
        {
            throw new NotImplementedException();
        }
    }
}