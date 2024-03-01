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
using System.Threading.Tasks;
using umi3d.cdk.collaboration;
using umi3d.common.collaboration;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3dBrowsers.services.connection
{
    [CreateAssetMenu(fileName = "Identifier", menuName = "UMI3D/Identifier")]
    public class Identifier : ClientIdentifierApi
    {
        public Action<List<string>, Action<bool>> OnLibrairiesAvailible;
        public Action<ConnectionFormDto, Action<FormAnswerDto>> OnParametersAvailible;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="callback"></param>
        public override async Task<FormAnswerDto> GetParameterDtos(ConnectionFormDto parameter)
        {
            bool isWaiting = true;
            FormAnswerDto form = null;

            Action<FormAnswerDto> callback = (formAnswer) => { form = formAnswer; isWaiting = false; };

            OnParametersAvailible.Invoke(parameter, callback);

            while (isWaiting)
                await Task.Yield();
            return form;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="callback"></param>
        public override async Task<bool> ShouldDownloadLibraries(List<string> LibrariesId)
        {
            bool b = true;
            bool form = false;
            Action<bool> callback = (f) => { form = f; b = false; };

            OnLibrairiesAvailible.Invoke(LibrariesId, callback);
            while (b)
                await Task.Yield();
            return form;
        }
    }
}