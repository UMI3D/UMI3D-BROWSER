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
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace umi3d.cdk.interaction
{
    public interface IControllerDataDelegate 
    { 
        /// <summary>
        /// Try to find all inputs on this controller that are compatibles with this interactionDto.<br/>
        /// <br/>
        /// The input does not need to be available.
        /// </summary>
        /// <param name="inputs">The list of compatible inputs. Should be filled with compatible inputs at the end of the method.</param>
        /// <param name="controller"></param>
        /// <returns></returns>
        bool TryGetInputsForEventDto(List<Input> inputs, Controller controller);
        //bool TryGetInputForDrawingInteractionDto(out ButtonControl control, Controller controller);
        bool TryGetInputForBooleanParameterDto(out Input input, Controller controller);

        //bool TryGetInputForStringParameterDto(out ButtonControl control, Controller controller);

        //bool TryGetInputForFloatParameterDto(out DoubleControl control, Controller controller);
        //bool TryGetInputForIntegerParameterDto(out IntegerControl control, Controller controller);

        //bool TryGetInputForColorParameterDto(out ButtonControl control, Controller controller);

        //bool TryGetInputForVector2ParameterDto(out Vector2Control control, Controller controller);
        //bool TryGetInputForVector3ParameterDto(out Vector3Control control, Controller controller);
        //bool TryGetInputForVector4ParameterDto(out ButtonControl control, Controller controller);

        //bool TryGetInputForEnumParameterDto(out ButtonControl control, Controller controller);
        //bool TryGetInputForFloatRangeParameterDto(out ButtonControl control, Controller controller);
        //bool TryGetInputForIntegerRangeParameterDto(out ButtonControl control, Controller controller);

        //bool TryGetInputForUploadFileParameterDto(out ButtonControl control, Controller controller);
        //bool TryGetInputForLocalInfoRequestParameterDto(out ButtonControl control, Controller controller);
    }
}