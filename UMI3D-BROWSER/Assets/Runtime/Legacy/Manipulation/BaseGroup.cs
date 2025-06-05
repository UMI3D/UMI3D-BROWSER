///*
//Copyright 2019 - 2022 Inetum

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//*/
//using inetum.unityUtils;
//using System.Collections.Generic;
//using UnityEngine;

//namespace umi3d.baseBrowser.inputs.interactions
//{
//    public abstract class BaseGroup<InteractionType, E> : BaseInteraction<InteractionType> where InteractionType : common.interaction.AbstractInteractionDto
//    {
//        protected static List<BaseGroup<InteractionType, E>> s_instances = new();
//        protected static Dictionary<BaseGroup<InteractionType, E>, List<E>> s_elementByGroup = new();
//        protected static int s_currentIndex;

//        /// <summary>
//        /// Current manipulation group.
//        /// </summary>
//        public static BaseGroup<InteractionType, E> CurrentGroup
//            => s_instances.Count > 0 ? s_instances[s_currentIndex] : null;

//        /// <summary>
//        /// Current list of manipulations associated with the <see cref="CurrentGroup"/>
//        /// </summary>
//        public static List<E> CurrentManipulations
//            => CurrentGroup == null
//                || s_elementByGroup == null
//                || !s_elementByGroup.ContainsKey(CurrentGroup)
//            ? null
//            : s_elementByGroup[CurrentGroup];

//        #region Static methods and properties

//        #region Incrementation and decrementation

//        /// <summary>
//        /// Deactivate current group and activate next one.
//        /// </summary>
//        public static void NextGroup() => SwicthGroup(s_currentIndex + 1);

//        /// <summary>
//        /// Deactivate current group and activate previous one.
//        /// </summary>
//        public static void PreviousGroup() => SwicthGroup(s_currentIndex - 1);

//        protected static void SwicthGroup(int i)
//        {
//            if (s_instances.Count == 0) return;

//            if (s_currentIndex < s_instances.Count && s_currentIndex >= 0) (s_instances[s_currentIndex] as BaseManipulationGroup).Deactivate();

//            if (s_instances.Count == 0)
//            {
//                s_currentIndex = -1;
//                return;
//            }

//            if (i < 0) s_currentIndex = s_instances.Count - 1;
//            else if (i >= s_instances.Count) s_currentIndex = 0;
//            else s_currentIndex = i;

//            (s_instances[s_currentIndex] as BaseManipulationGroup).Activate();
//        }

//        #endregion

//        #endregion

//        #region Activation, Deactivation, Select

//        /// <summary>
//        /// Whether or not this group is active.
//        /// </summary>
//        public bool IsActive { get => m_isActive; protected set => m_isActive = value; }
//        [SerializeField]
//        [ReadOnly]
//        private bool m_isActive;

//        protected virtual void Activate()
//        {
//            IsActive = true;
//        }
//        protected virtual void Deactivate()
//        {
//            IsActive = false;
//        }

//        protected void Select() => SwicthGroup(s_instances.FindIndex(a => a == this));

//        #endregion

//    }
//}
