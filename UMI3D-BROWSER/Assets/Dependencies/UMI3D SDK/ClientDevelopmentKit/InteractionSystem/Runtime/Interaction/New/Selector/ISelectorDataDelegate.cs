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
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public interface ISelectorDataDelegate 
    {
        bool TryToAssociateInteractionAndInput(List<(AbstractInteractionDto interaction, Input input)> associations, ReadOnlyDictionary<AbstractInteractionDto, ReadOnlyCollection<Input>> inputsByInteractions);

        /// <summary>
        /// Try to find all the available inputs on this controller from the collection of controllers of this selector that match this interaction.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="selector"></param>
        /// <param name="controllers"></param>
        /// <returns></returns>
        bool TryGetInputsForEventDto(out ReadOnlyCollection<Input> inputs, Selector selector, ReadOnlyCollection<Controller> controllers);

        //bool TryGetInputForDrawingInteractionDto(out ButtonControl control);
        bool TryGetInputForBooleanParameterDto(out Input input, out Controller controller, Selector selector, ReadOnlyCollection<Controller> controllers);

        //bool TryGetControlForStringParameterDto(out ButtonControl control);

        //bool TryGetInputForFloatParameterDto(out DoubleControl control);
        //bool TryGetInputForIntegerParameterDto(out IntegerControl control);

        //bool TryGetInputForColorParameterDto(out ButtonControl control);

        //bool TryGetInputForVector2ParameterDto(out Vector2Control control);
        //bool TryGetInputForVector3ParameterDto(out Vector3Control control);
        //bool TryGetInputForVector4ParameterDto(out ButtonControl control);

        //bool TryGetInputForEnumParameterDto(out ButtonControl control);
        //bool TryGetInputForFloatRangeParameterDto(out ButtonControl control);
        //bool TryGetInputForIntegerRangeParameterDto(out ButtonControl control);

        //bool TryGetInputForUploadFileParameterDto(out ButtonControl control);
        //bool TryGetInputForLocalInfoRequestParameterDto(out ButtonControl control);
    }
}