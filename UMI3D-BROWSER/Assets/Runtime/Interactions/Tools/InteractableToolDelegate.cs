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
using System.Linq;
using umi3d.cdk;
using umi3d.cdk.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.interaction
{
    [CreateAssetMenu(fileName = "UMI3D InteractableTool Delegate", menuName = "UMI3D/Interactions/Tool Delegate/InteractableTool Delegate")]
    public class InteractableToolDelegate : AbstractToolDelegate<Interactable>
    {
        public override void CreateInteractionsMenuFor(Interactable tool)
        {
            throw new NotImplementedException();
        }

        public override bool Exists(ulong environmentId, ulong id)
        {
            
            return UMI3DEnvironmentLoader.Instance.TryGetEntity(
                environmentId,
                id,
                out Interactable tool
            );
        }

        public override Interactable GetTool(ulong environmentId, ulong id)
        {
            return UMI3DEnvironmentLoader.Instance.GetEntityObject<Interactable>(
                environmentId,
                id
            );
        }

        public override bool RequiresMenu(Interactable tool)
        {
            throw new NotImplementedException();
        }
    }
}