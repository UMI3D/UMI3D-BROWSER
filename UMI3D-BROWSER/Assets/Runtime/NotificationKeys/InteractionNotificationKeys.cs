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

namespace umi3d.browserRuntime.notificationKeys
{
    public class InteractionNotificationKeys 
    {
        /// <summary>
        /// Event raised when an input corresponding to a parameter has been found.
        /// </summary>
        public class ParameterInputFound
        {
            /// <summary>
            /// The parameter dto.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="umi3d.common.interaction.AbstractParameterDto"/>
            /// </remarks>
            public const string parameterDto = "parameterDto";
            /// <summary>
            /// The input.
            /// </summary>
            /// <remarks>
            /// Value is <see cref="umi3d.cdk.interaction.AbstractUMI3DInput"/>
            /// </remarks>
            public const string parameterInput = "parameterInput";
        }
    }
}