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
    [CreateAssetMenu(fileName = "UMI3D Controller Delegate for [ControllerName]", menuName = "UMI3D/Interactions/Controller/Controller Delegate")]
    public class VRControllerDelegate : AbstractControllerDelegate
    {
        public override bool IsAvailableFor(AbstractTool tool)
        {
            throw new NotImplementedException();
        }

        public override bool IsCompatibleWith(AbstractTool tool)
        {
            return tool.interactionsLoaded.TrueForAll(
                inter =>
                {
                    if (inter is ManipulationDto manipulation)
                    {
                        return manipulation.dofSeparationOptions.Exists(group =>
                            {
                                return !group.separations.Exists(
                                    dof =>
                                    {
                                        return (dof.dofs == DofGroupEnum.X_RX) 
                                        || (dof.dofs == DofGroupEnum.Y_RY) 
                                        || (dof.dofs == DofGroupEnum.Z_RZ);
                                    }
                                );
                            }
                        );
                    }
                    else
                    {
                        return true;
                    }
                }
            );
        }
    }
}