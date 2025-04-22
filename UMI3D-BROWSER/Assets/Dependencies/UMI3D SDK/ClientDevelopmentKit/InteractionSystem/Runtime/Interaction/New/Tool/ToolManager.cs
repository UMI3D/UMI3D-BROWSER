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
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public class ToolManager 
    {
        #region Initialize

        static Lazy<ToolManager> _default = new(() => new());
        public static ToolManager @default => _default.Value;

        ToolManager()
        {

        }

        #endregion

        List<Tool> _tools = new();
        ReadOnlyCollection<Tool> tools => _tools.AsReadOnly();
        public bool TryToInstantiateTool(out Tool tool, ulong environmentId, AbstractToolDto dto)
        {
            tool = _tools.Find(tool => tool.environmentId == environmentId && tool.dto.id == dto.id);
            if (tool != null)
            {
                UnityEngine.Debug.LogWarning($"[ToolManager] Warning: Cannot instantiate tool for '{environmentId}' and dto's id '{dto.id}' because this tool already exist.");
                return false;
            }

            tool = new(environmentId, dto);
            _tools.Add(tool);
            UMI3DEnvironmentLoader.Instance.RegisterEntity(environmentId, dto.id, dto, tool).NotifyLoaded();
            return true;
        }
        public bool TryToRemoveTool(Tool tool)
        {
            if (!_tools.Contains(tool))
            {
                return false;
            }

            UMI3DEnvironmentLoader.Instance.DeleteEntityInstance(tool.environmentId, tool.dto.id);
            _tools.Remove(tool);
            return true;
        }
        public bool TryToRemoveTool(ulong environmentId, ulong toolId)
        {
            Tool tool = _tools.Find(tool => tool.environmentId == environmentId && tool.dto.id == toolId);
            if (tool == null)
            {
                return false;
            }

            UMI3DEnvironmentLoader.Instance.DeleteEntityInstance(environmentId, toolId);
            _tools.Remove(tool);
            return true;
        }
        public bool TryToFetchTool(out Tool tool, ulong environmentId, ulong dtoId)
        {
            tool = _tools.Find(tool => tool.environmentId == environmentId && tool.dto.id == dtoId);
            if (tool == null)
            {
                UnityEngine.Debug.LogWarning($"[ToolManager] Warning: Cannot find tool for '{environmentId}' and dto's id '{dtoId}'.");
                return false;
            }

            return true;
        }
    }
}