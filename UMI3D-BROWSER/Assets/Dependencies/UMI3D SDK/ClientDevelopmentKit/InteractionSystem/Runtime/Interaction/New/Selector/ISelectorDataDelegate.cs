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
        /// <summary>
        /// How many tools can be projected at the same time on the controllers of this selector.
        /// </summary>
        int toolCountLimitation { get; }

        void AssociateInteractionAndInput(List<(AbstractInteractionDto interaction, Input input)> associations, ReadOnlyDictionary<AbstractInteractionDto, ReadOnlyCollection<Input>> inputsByInteractions);

        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs">The list of compatible inputs. Should be filled with compatible inputs at the end of the method.</param>
        /// <returns></returns>
        bool TryGetInputsForEventDto(out ReadOnlyCollection<Input> inputs);

        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs">The list of compatible inputs. Should be filled with compatible inputs at the end of the method.</param>
        /// <returns></returns>
        bool TryGetInputsForDrawingInteractionDto(out ReadOnlyCollection<Input> inputs);

        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForBooleanParameterDto(out ReadOnlyCollection<Input> inputs);

        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForStringParameterDto(out ReadOnlyCollection<Input> inputs);

        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForFloatParameterDto(out ReadOnlyCollection<Input> inputs);
        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForIntegerParameterDto(out ReadOnlyCollection<Input> inputs);

        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForColorParameterDto(out ReadOnlyCollection<Input> inputs);

        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForVector2ParameterDto(out ReadOnlyCollection<Input> inputs);
        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForVector3ParameterDto(out ReadOnlyCollection<Input> inputs);
        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForVector4ParameterDto(out ReadOnlyCollection<Input> inputs);

        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForEnumParameterDto(out ReadOnlyCollection<Input> inputs);
        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForFloatRangeParameterDto(out ReadOnlyCollection<Input> inputs);
        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForIntegerRangeParameterDto(out ReadOnlyCollection<Input> inputs);

        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForUploadFileParameterDto(out ReadOnlyCollection<Input> inputs);
        /// <summary>
        /// Try to find all inputs compatible with this interactions.<br/>
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        bool TryGetInputsForLocalInfoRequestParameterDto(out ReadOnlyCollection<Input> inputs);
    }
}