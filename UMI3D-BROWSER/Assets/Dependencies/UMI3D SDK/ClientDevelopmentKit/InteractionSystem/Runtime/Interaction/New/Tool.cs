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
using System.Linq;
using System.Threading.Tasks;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public class Tool 
    {
        /// <summary>
        /// Environment Id.
        /// </summary>
        public readonly ulong environmentId;

        public AbstractToolDto toolDto { get; private set; }

        internal Tool(ulong environmentId, AbstractToolDto toolDto)
        {
            this.environmentId = environmentId;
            this.toolDto = toolDto;

            GetInteractions();
        }

        public bool TryCast<ToolDto>(out ToolDto dto) where ToolDto : AbstractToolDto
        {
            if (toolDto == null || toolDto is not ToolDto)
            {
                dto = null;
                return false;
            }

            dto = toolDto as ToolDto;
            return true;
        }

        List<AbstractInteractionDto> _interactions = new();
        public ReadOnlyCollection<AbstractInteractionDto> interactions;
        async void GetInteractions()
        {
            IEnumerable<Task<UMI3DEntityInstance>> entities = toolDto.interactions
                .Select(id => UMI3DEnvironmentLoader.WaitForAnEntityToBeLoaded(environmentId, id, null));

            foreach (Task<UMI3DEntityInstance> entity in entities)
            {
                AbstractInteractionDto interaction = (await entity).dto as AbstractInteractionDto;
                _interactions.Add(interaction);
            }
        }
    }
}