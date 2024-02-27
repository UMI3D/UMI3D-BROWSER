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

        public Control_SO control_SO;
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
            model.Init(control_SO);
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