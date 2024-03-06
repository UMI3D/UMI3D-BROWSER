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
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    [Serializable]
    public sealed class UMI3DControlManager 
    {
        [SerializeField]
        debug.UMI3DLogger logger = new();

        public Controls_SO controls_SO;
        public AbstractManipulationControlDelegate manipulationDelegate;
        public AbstractEventControlDelegate eventDelegate;
        public AbstractControlDelegate<FormDto> formDelegate;
        public AbstractControlDelegate<LinkDto> linkDelegate;
        public AbstractControlDelegate<AbstractParameterDto> parameterDelegate;

        [HideInInspector] public ControlModel model = new();
        [HideInInspector] public UMI3DController controller;

        public void Init(MonoBehaviour context, UMI3DController controller)
        {
            logger.MainContext = context;
            logger.MainTag = nameof(UMI3DControlManager);
            model.Init(controls_SO);
        }

        /// <summary>
        /// Return a control compatible with <paramref name="interaction"/>.<br/> 
        /// Return null if no control is available.
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="unused"></param>
        /// <param name="tryToFindInputForHoldableEvent"></param>
        /// <param name="dof"></param>
        /// <returns></returns>
        public AbstractControlEntity GetControl<Interaction>(
            Interaction interaction,
            bool tryToFindInputForHoldableEvent = false,
            DofGroupDto dof = null
        )
            where Interaction: AbstractInteractionDto
        {
            switch (interaction)
            {
                case ManipulationDto manipulation:
                    manipulationDelegate.dof = dof;
                    return manipulationDelegate.GetControl(
                        controller,
                        manipulation
                    );
                case EventDto button:
                    eventDelegate.tryToFindInputForHoldableEvent = tryToFindInputForHoldableEvent;
                    return eventDelegate.GetControl(
                        controller,
                        button
                    );
                case FormDto form:
                    return formDelegate.GetControl(
                        controller,
                        form
                    );
                case LinkDto link:
                    return linkDelegate.GetControl(
                        controller,
                        link
                    );
                case AbstractParameterDto parameter:
                    return parameterDelegate.GetControl(
                        controller,
                        parameter
                    );
                default:
                    throw new System.Exception("Unknown interaction type, can't project !");
            }
        }

        public void Associate(
            AbstractControlEntity control,
            ulong environmentId,
            ulong toolId,
            AbstractInteractionDto interaction,
            ulong hoveredObjectId,
            DofGroupDto dof = null
        )
        {
            switch (interaction)
            {
                case ManipulationDto manipulation:
                    manipulationDelegate.dof = dof;
                    manipulationDelegate.Associate(
                        controller,
                        control,
                        environmentId,
                        interaction,
                        toolId,
                        hoveredObjectId
                    );
                    break;
                case EventDto button:
                    eventDelegate.Associate(
                        controller,
                        control,
                        environmentId,
                        interaction,
                        toolId,
                        hoveredObjectId
                    );
                    break;
                case FormDto form:
                    formDelegate.Associate(
                        controller,
                        control,
                        environmentId,
                        interaction,
                        toolId,
                        hoveredObjectId
                    );
                    break;
                case LinkDto link:
                    linkDelegate.Associate(
                        controller,
                        control,
                        environmentId,
                        interaction,
                        toolId,
                        hoveredObjectId
                    );
                    break;
                case AbstractParameterDto parameter:
                    parameterDelegate.Associate(
                        controller,
                        control,
                        environmentId,
                        interaction,
                        toolId,
                        hoveredObjectId
                    );
                    break;
                default:
                    throw new System.Exception("Unknown interaction type, can't project !");
            }
        }
    }
}