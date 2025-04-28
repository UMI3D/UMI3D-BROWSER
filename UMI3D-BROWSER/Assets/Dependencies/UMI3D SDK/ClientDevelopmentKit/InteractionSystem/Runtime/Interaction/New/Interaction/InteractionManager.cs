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
using umi3d.common.interaction;

namespace umi3d.cdk.interaction
{
    public class InteractionManager 
    {
        #region Initialize

        static Lazy<InteractionManager> _default = new(() => new());
        public static InteractionManager @default => _default.Value;

        InteractionManager()
        {

        }

        #endregion

        List<Interaction> _interactions = new();
        public ReadOnlyCollection<Interaction> interactions => _interactions.AsReadOnly();

        public bool TryToInstantiateInteraction(out Interaction interaction, ulong environmentId, AbstractInteractionDto dto)
        {
            interaction = _interactions.Find(interaction => interaction.environmentId == environmentId && interaction.dto.id == dto.id);
            if (interaction != null)
            {
                UnityEngine.Debug.LogWarning($"[InteractionManager] Warning: Cannot instantiate interaction for '{environmentId}' and dto's id '{dto.id}' because this interaction already exist.");
                return false;
            }

            interaction = new(environmentId, dto);
            _interactions.Add(interaction);
            UMI3DEnvironmentLoader.Instance.RegisterEntity(environmentId, dto.id, dto, interaction).NotifyLoaded();
            return true;
        }
        public bool TryToRemoveInteraction(Interaction interaction)
        {
            if (!_interactions.Contains(interaction))
            {
                return false;
            }
 
            UMI3DEnvironmentLoader.Instance.DeleteEntityInstance(interaction.environmentId, interaction.dto.id);
            _interactions.Remove(interaction);
            return true;
        }
        public bool TryToRemoveInteraction(ulong environmentId, ulong interactionId)
        {
            Interaction interaction = _interactions.Find(interaction => interaction.environmentId == environmentId && interaction.dto.id == interactionId);
            if (interaction == null)
            {
                return false;
            }

            UMI3DEnvironmentLoader.Instance.DeleteEntityInstance(environmentId, interactionId);
            _interactions.Remove(interaction);
            return true;
        }
        public bool TryToFetchInteraction(out Interaction interaction, ulong environmentId, ulong dtoId)
        {
            interaction = _interactions.Find(interaction => interaction.environmentId == environmentId && interaction.dto.id == dtoId);
            if (interaction == null)
            {
                UnityEngine.Debug.LogWarning($"[ToolManager] Warning: Cannot find tool for '{environmentId}' and dto's id '{dtoId}'.");
                return false;
            }

            return true;
        }
    }
}