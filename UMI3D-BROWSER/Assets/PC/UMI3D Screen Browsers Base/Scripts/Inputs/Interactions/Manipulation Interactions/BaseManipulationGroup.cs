/*
Copyright 2019 - 2022 Inetum

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
using umi3d.baseBrowser.Cursor;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.baseBrowser.inputs.interactions
{

    /// <summary>
    /// Group of manipulations.
    /// </summary>
    public abstract class BaseManipulationGroup : BaseGroup<ManipulationDto, BaseManipulation>
    {
        /// <summary>
        /// <see cref="BaseManipulation.strength"/>
        /// </summary>
        public float strength;
        /// <summary>
        /// Reference to the <see cref="Cursor.FrameIndicator"/>
        /// </summary>
        public Cursor.FrameIndicator frameIndicator;
        /// <summary>
        /// TODO: not define
        /// </summary>
        public Transform manipulationCursor;

        public static List<DofGroupEnum> DofGroups = new List<DofGroupEnum>
        {
            DofGroupEnum.X,
            DofGroupEnum.Y,
            DofGroupEnum.Z,

            DofGroupEnum.XY,
            DofGroupEnum.XZ,
            DofGroupEnum.YZ,

            DofGroupEnum.RX,
            DofGroupEnum.RY,
            DofGroupEnum.RZ
        };

        protected override void Activate()
        {
            base.Activate();
            BaseManipulation.SelectFirst();
        }
        protected override void Deactivate()
        {
            base.Deactivate();
            foreach (var input in s_elementByGroup[this]) input.Deactivate();
        }

        #region Associate

        public Func<DofGroupEnum, float, FrameIndicator, Transform, BaseManipulation> InstanciateManipulation;

        public override void Associate(ulong environmentId, AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
            => throw new System.NotImplementedException();

        public override void Associate(ulong environmentId, ManipulationDto manipulation, DofGroupEnum dofs, ulong toolId, ulong hoveredObjectId)
        {
            UnityEngine.Debug.Log("Associate");

            if (!IsAvailableFor(manipulation)) throw new System.Exception($"This input is not available for {manipulation}");

            if (!IsCompatibleWith(manipulation)) throw new System.Exception("Trying to associate an uncompatible interaction !");

            AddGroup();

            this.hoveredObjectId = hoveredObjectId;
            this.toolId = toolId;
            associatedInteraction = manipulation;
            this.environmentId = environmentId;

            BaseManipulation input = InstanciateManipulation(dofs, strength, frameIndicator, manipulationCursor);
            input.Menu = Menu;
            input.bone = bone;
            input.Associate(environmentId, manipulation, dofs, toolId, hoveredObjectId);
            input.Deactivate();
            AddManipulation(input);

            if (s_instances.Count == 1)
            {
                s_currentIndex = 0;
                Activate();
            }
            else Deactivate();
        }

        protected void AddGroup()
        {
            if (s_instances.Contains(this)) return;

            s_instances.Add(this);
            s_elementByGroup.Add(this, new List<BaseManipulation>());
        }

        protected void AddManipulation(BaseManipulation input)
        {
            if (s_elementByGroup[this].Contains(input)) return;

            s_elementByGroup[this].Add(input);
        }

        #endregion

        #region Dissociate

        public override void Dissociate()
        {
            var manipulations = s_elementByGroup[this];
            for (int i = manipulations.Count - 1; i >= 0; i--)
            {
                var input = manipulations[i];
                input.Dissociate();
                RemoveManipulation(input);
                Destroy(input);
            }

            RemoveGroup();
            associatedInteraction = null;
        }

        protected void RemoveGroup()
        {
            if (!s_instances.Contains(this)) return;

            s_instances.Remove(this);
            s_elementByGroup.Remove(this);
        }

        protected void RemoveManipulation(BaseManipulation input)
        {
            if (!s_elementByGroup[this].Contains(input)) return;

            s_elementByGroup[this].Remove(input);
        }

        #endregion

        public bool IsAvailableFor(ManipulationDto manipulation)
            => manipulation == associatedInteraction || IsAvailable();

        public override bool IsCompatibleWith(AbstractInteractionDto interaction)
        {
            if (!(interaction is ManipulationDto manipulationDto)) return false;

            return manipulationDto.dofSeparationOptions.Exists
            (
                (sep) =>
                {
                    foreach (DofGroupDto dof in sep.separations)
                    {
                        if (!DofGroups.Contains(dof.dofs)) return false;
                    }
                    return true;
                }
            );
        }

        public override void UpdateHoveredObjectId(ulong hoveredObjectId)
        {
            base.UpdateHoveredObjectId(hoveredObjectId);
            foreach (var input in s_elementByGroup[this]) input.UpdateHoveredObjectId(hoveredObjectId);
        }
    }
}
