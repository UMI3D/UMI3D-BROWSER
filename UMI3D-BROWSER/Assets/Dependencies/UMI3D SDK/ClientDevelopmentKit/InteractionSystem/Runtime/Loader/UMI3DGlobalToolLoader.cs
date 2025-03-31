/*
Copyright 2019 - 2021 Inetum

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
using System.ComponentModel;
using System.Threading.Tasks;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine.Events;

namespace umi3d.cdk.interaction
{
    /// <summary>
    /// Helper class that manages the loading of <see cref="GlobalTool"/> entities.
    /// </summary>
    public class UMI3DGlobalToolLoader : UMI3DAbstractToolLoader
    {
        private const DebugScope scope = DebugScope.CDK | DebugScope.Interaction | DebugScope.Loading;

        #region CRUD events
        private static readonly GlobalToolEvent onGlobalToolCreation = new GlobalToolEvent();
        private static readonly GlobalToolEvent onGlobalToolUpdate = new GlobalToolEvent();
        private static readonly GlobalToolEvent onGlobalToolDelete = new GlobalToolEvent();

        public static void SubscribeToGlobalToolCreation(UnityAction<ulong,GlobalTool> callback)
        {
            onGlobalToolCreation.AddListener(callback);
        }
        public static void UnsubscribeToGlobalToolCreation(UnityAction<ulong,GlobalTool> callback)
        {
            onGlobalToolCreation.RemoveListener(callback);
        }
        public static void SubscribeToGlobalToolUpdate(UnityAction<ulong,GlobalTool> callback)
        {
            onGlobalToolUpdate.AddListener(callback);
        }
        public static void UnsubscribeToGlobalToolUpdate(UnityAction<ulong, GlobalTool> callback)
        {
            onGlobalToolUpdate.RemoveListener(callback);
        }
        public static void SubscribeToGlobalToolDelete(UnityAction<ulong, GlobalTool> callback)
        {
            onGlobalToolDelete.AddListener(callback);
        }
        public static void UnsubscribeToGlobalToolDelete(UnityAction<ulong, GlobalTool> callback)
        {
            onGlobalToolDelete.RemoveListener(callback);
        }

        #endregion

        public override bool CanReadUMI3DExtension(ReadUMI3DExtensionData data)
        {
            return data.dto is GlobalToolDto;
        }

        public override Task ReadUMI3DExtension(ReadUMI3DExtensionData value)
        {
            return ReadUMI3DExtension(value.environmentId, value.dto as GlobalToolDto);
        }

        /// <summary>
        /// Reads the value of an <see cref="GlobalToolDto"/> and updates it.
        /// <br/> Part of the bytes networking workflow.
        /// </summary>
        /// <param name="dto">Tool dto</param>
        private static async Task ReadUMI3DExtension(ulong environmentId, GlobalToolDto dto)
        {
            if (GlobalTool.GetGlobalTools().Exists(t => t.id == dto.id))
                return;

            onGlobalToolCreation?.Invoke(environmentId,new GlobalTool(environmentId, dto));
        }

        public override async Task<bool> SetUMI3DProperty(SetUMI3DPropertyData data)
        {
            var dto = data.entity?.dto as GlobalToolDto;

            if (dto == null)
                return false;

            if (await base.SetUMI3DProperty(data))
            {
                onGlobalToolUpdate.Invoke(data.environmentId, GlobalTool.GetGlobalTool(dto.id));
                return true;
            }

            return false;
        }

        public override async Task<bool> SetUMI3DProperty(SetUMI3DPropertyContainerData data)
        {
            var dto = data.entity?.dto as GlobalToolDto;
            if (dto == null)
                return false;
            if (await base.SetUMI3DProperty(data))
                return true;

            return false;
        }



        /// <summary>
        /// Remove a <see cref="GlobalTool"/>.
        /// </summary>
        /// <param name="tool">Tool to remove dto</param>
        public static void RemoveTool(ulong environmentId,GlobalToolDto tool)
        {
            var t = GlobalTool.GetGlobalTool(tool.id);
            t.Delete();
            onGlobalToolDelete?.Invoke(environmentId, t);
        }

        /// <summary>
        /// Reads the value of an unknown <see cref="object"/> based on a received <see cref="ByteContainer"/> and updates it.
        /// <br/> Part of the bytes networking workflow.
        /// </summary>
        /// <param name="value">Boxing object to retrieve read value.</param>
        /// <param name="propertyKey">Property to update key in <see cref="UMI3DPropertyKeys"/></param>
        /// <param name="container">Received byte container</param>
        /// <returns>True if property setting was successful</returns>
        public override async Task<bool> ReadUMI3DProperty(ReadUMI3DPropertyData data)
        {
            if (await base.ReadUMI3DProperty(data)) return true;
            switch (data.propertyKey)
            {
                case UMI3DPropertyKeys.ToolboxTools:
                    data.result = UMI3DSerializer.ReadList<GlobalToolDto>(data.container);
                    return true;
                default:
                    return false;
            }
        }
    }
}