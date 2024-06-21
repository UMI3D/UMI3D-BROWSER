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
using umi3d.common.interaction;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace umi3dBrowsers.services.connection
{
    [CreateAssetMenu(fileName = "Identifier", menuName = "UMI3D/Identifier")]
    public class Identifier : ClientIdentifierApi
    {
        public Action<List<string>, Action<bool>> OnLibrairiesAvailible;
        public Action<ConnectionFormDto, Action<FormAnswerDto>> OnParamFormAvailible;
        public Action<umi3d.common.interaction.form.ConnectionFormDto, Action<umi3d.common.interaction.form.FormAnswerDto>> OnDivFormAvailable;

        public Action OnAnswerFailed;

        private ConnectionFormDto _connectionFormDto;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="callback"></param>
        public override async Task<FormAnswerDto> GetParameterDtos(ConnectionFormDto parameter)
        {
            if (AreConnectionFormEqual(parameter, _connectionFormDto))
                OnAnswerFailed?.Invoke();
            _connectionFormDto = parameter;

            bool isWaiting = true;
            FormAnswerDto form = null;

            Action<FormAnswerDto> callback = (formAnswer) => { form = formAnswer; isWaiting = false; };

            OnParamFormAvailible.Invoke(parameter, callback);

            while (isWaiting)
                await Task.Yield();
            return form;
        }

        public override async Task<umi3d.common.interaction.form.FormAnswerDto> GetParameterDtos(umi3d.common.interaction.form.ConnectionFormDto parameter)
        {
            bool isWaiting = true;
            umi3d.common.interaction.form.FormAnswerDto form = null;

            Action<umi3d.common.interaction.form.FormAnswerDto> callback = (formAnswer) => { form = formAnswer; isWaiting = false; };

            OnDivFormAvailable.Invoke(parameter, callback);

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

        public void Reset()
        {
            _connectionFormDto = null;
        }

        private static bool AreConnectionFormEqual(ConnectionFormDto form1, ConnectionFormDto form2)
        {
            if (form1 == null || form2 == null)
                return false;
            if (form1.fields.Count != form2.fields.Count)
                return false;
            for (var i = 0; i < form1.fields.Count; i++)
            {
                var field = form1.fields[i];
                var oldField = form2?.fields[i];
                if (field.name != oldField.name)
                    return false;
            }

            return true;
        }
    }
}