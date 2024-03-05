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
using UnityEngine;
using UnityEngine.Windows;

namespace umi3d.cdk.interaction
{
    [Serializable]
    public class PhysicalManipulationControlEntity : AbstractControlEntity, HasManipulationControlData
    {
        public ManipulationControlData manipulationData = new();
        [Tooltip("The input that activates the manipulation")]
        /// <summary>
        /// The input that activates the manipulation.
        /// </summary>
        public NewInputType activationInput;
        [Tooltip("The input that tracks the manipulation")]
        /// <summary>
        /// The input that track the manipulation.
        /// </summary>
        public NewInputType trackingInput;

        public PhysicalManipulationControlEntity()
        {
            activationInput.actionPerformed = controlData.ActionPerformed;
            controlData.enableHandler += Enable;
            controlData.disableHandler += Disable;
        }

        public ManipulationControlData ManipulationControlData
        {
            get
            {
                return manipulationData;
            }
            set
            {
                manipulationData = value;
            }
        }

        public void Disable()
        {
            try
            {
                activationInput
                    .inputActionProperty
                    .action
                    .performed -= activationInput.ActionPerformed;
                trackingInput
                   .inputActionProperty
                   .action
                   .performed -= trackingInput.ActionPerformed;
            }
            catch (NullReferenceException)
            {
                UnityEngine.Debug.LogError($"[UMI3D] Control: new input type action is null");
            }
        }

        public void Enable()
        {
            try
            {
                activationInput
                    .inputActionProperty
                    .action
                    .performed += activationInput.ActionPerformed;
                trackingInput
                   .inputActionProperty
                   .action
                   .performed += trackingInput.ActionPerformed;
            }
            catch (NullReferenceException)
            {
                UnityEngine.Debug.LogError($"[UMI3D] Control: new input type action is null");
            }
        }
    }
}