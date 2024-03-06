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

using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.Windows;

namespace umi3d.cdk.interaction
{
    [Serializable]
    public sealed class UMI3DToolManager 
    {
        public Tool_SO tool_SO;
        public AbstractToolSelectionDelegate toolSelectionDelegate;
        public AbstractToolUpdateDelegate toolUpdateDelegate;

        public void Init()
        {
        }

        /// <summary>
        /// The currently projected tools.
        /// </summary>
        public IEnumerable<AbstractTool> ProjectedTools
        {
            get
            {
                return tool_SO.projectedTools;
            }
        }

        /// <summary>
        /// Check if a tool with the given id exists.
        /// </summary>
        public bool Exists<Tool>(
            ulong environmentId,
            ulong id
        ) where Tool: AbstractTool
        {
            return UMI3DEnvironmentLoader.Instance.TryGetEntity(environmentId, id, out Tool tool);
        }

        /// <summary>
        /// Get the tool with the given id (if any).
        /// </summary>
        public Tool GetTool<Tool>(
            ulong environmentId,
            ulong id
        ) where Tool: AbstractTool
        {
            UMI3DEnvironmentLoader.Instance.TryGetEntity(environmentId, id, out Tool tool);
            return tool;
        }

        /// <summary>
        /// Return the tools matching a given condition.
        /// </summary>
        public IEnumerable<AbstractTool> GetTools(Predicate<AbstractTool> condition)
        {
            return UMI3DEnvironmentLoader
                .AllEntities()
                .Where(e => e?.Object is AbstractTool)
                .Select(e => e?.Object as AbstractTool)
                .ToList()
                .FindAll(condition);
        }

        /// <summary>
        /// Return all known tools.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AbstractTool> GetTools()
        {
            return GetTools(t => true);
        }

        /// <summary>
        /// Whether this tool is projected.
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public bool IsProjected(AbstractTool tool)
        {
            return tool_SO.projectedTools.Contains(tool);
        }

        /// <summary>
        /// Project <paramref name="tool"/> on this controller.
        /// </summary>
        /// <param name="tool"></param>
        public void ProjectTool(AbstractTool tool)
        {
            if (IsProjected(tool))
            {
                return;
            }

            tool_SO.projectedTools.Add(tool);
        }

        /// <summary>
        /// Release tool from this controller.
        /// </summary>
        /// <param name="tool"></param>
        public void ReleaseTool(AbstractTool tool)
        {
            tool_SO.projectedTools.Remove(tool);
            tool.OnUpdated.RemoveAllListeners();
            tool.OnAdded.RemoveAllListeners();
            tool.OnRemoved.RemoveAllListeners();
        }

        /// <summary>
        /// Whether this <paramref name="input"/> is associated to <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsAssociated(AbstractTool tool, AbstractControlEntity control)
        {
            if (!tool_SO.controlsByTool.ContainsKey(tool.id))
            {
                return false;
            }

            return tool_SO.controlsByTool[tool.id].Contains(control);
        }

        /// <summary>
        /// Associates <paramref name="controls"/> to this <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="controls"></param>
        public void AssociateControls(AbstractTool tool, params AbstractControlEntity[] controls)
        {
            if (!tool_SO.controlsByTool.ContainsKey(tool.id))
            {
                tool_SO.controlsByTool.Add(tool.id, controls);
            }
            else
            {
                tool_SO.controlsByTool[tool.id] = tool_SO.controlsByTool[tool.id]
                    .Concat(controls)
                    .ToArray();
            }
        }

        /// <summary>
        /// Dissociates <paramref name="control"/> from <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="control"></param>
        public void DissociateControl(AbstractTool tool, AbstractControlEntity control)
        {
            if (!tool_SO.controlsByTool.ContainsKey(tool.id))
            {
                return;
            }

            var tmp = new List<AbstractControlEntity>(tool_SO.controlsByTool[tool.id]);
            tmp.Remove(control);
            tool_SO.controlsByTool[tool.id] = tmp.ToArray();
        }

        /// <summary>
        /// Dissociates all inputs from <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        public void DissociateAllControls(AbstractTool tool)
        {
            if (!tool_SO.controlsByTool.ContainsKey(tool.id))
            {
                return;
            }

            tool_SO.controlsByTool.Remove(tool.id);
        }
    }
}