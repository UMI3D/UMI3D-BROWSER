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
    public sealed class UMI3DInputManager 
    {
        [SerializeField]
        debug.UMI3DLogger logger = new();

        public Controls_SO controls_SO;
        public AbstractManipulationInputDelegate manipulationDelegate;
        public AbstractEventInputDelegate eventInputDelegate;
        public AbstractInputDelegate<FormDto> formInputDelegate;
        public AbstractInputDelegate<LinkDto> linkInputDelegate;
        public AbstractInputDelegate<AbstractParameterDto> parameterInputDelegate;

        public ControlModel model = new();

        public void Init(MonoBehaviour context)
        {
            logger.MainContext = context;
            logger.MainTag = nameof(UMI3DInputManager);
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
        public AbstractControlData GetControl<Interaction>(
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
                        manipulation
                    );
                case EventDto button:
                    eventInputDelegate.tryToFindInputForHoldableEvent = tryToFindInputForHoldableEvent;
                    return eventInputDelegate.GetControl(
                        button
                    );
                case FormDto form:
                    return formInputDelegate.GetControl(
                        form
                    );
                case LinkDto link:
                    return linkInputDelegate.GetControl(
                        link
                    );
                case AbstractParameterDto parameter:
                    return parameterInputDelegate.GetControl(
                        parameter
                    );
                default:
                    throw new System.Exception("Unknown interaction type, can't project !");
            }
        }

        public void Associate(
            AbstractControlData control,
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
                        control,
                        environmentId,
                        interaction,
                        toolId,
                        hoveredObjectId
                    );
                    break;
                case EventDto button:
                    eventInputDelegate.Associate(
                        control,
                        environmentId,
                        interaction,
                        toolId,
                        hoveredObjectId
                    );
                    break;
                case FormDto form:
                    formInputDelegate.Associate(
                        control,
                        environmentId,
                        interaction,
                        toolId,
                        hoveredObjectId
                    );
                    break;
                case LinkDto link:
                    linkInputDelegate.Associate(
                        control,
                        environmentId,
                        interaction,
                        toolId,
                        hoveredObjectId
                    );
                    break;
                case AbstractParameterDto parameter:
                    parameterInputDelegate.Associate(
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

        public AbstractUMI3DInput FindInput<T>(
            List<T> inputs,
            Predicate<T> predicate
        ) where T : AbstractUMI3DInput, new()
        {
            T input = inputs.Find(predicate);
            if (input == null) AddInput(inputs, out input);
            return input;
        }

        void AddInput<T>(
            List<T> inputs,
            out T input
        ) where T : AbstractUMI3DInput, new()
        {
            input = new T();

            //input.Init(this);
            //input.Menu = ObjectMenu.menu;
            inputs.Add(input);
        }
    }
}