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

using inetum.unityUtils.saveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public abstract class AbstractToolDelegate<Tool> : SerializableScriptableObject
        where Tool : AbstractTool
    {
        [HideInInspector]
        public Tool_SO tool_SO;

        /// <summary>
        /// Check if a tool with the given id exists.
        /// </summary>
        public abstract bool Exists(ulong environmentId, ulong id);

        /// <summary>
        /// Get the tool with the given id (if any).
        /// </summary>
        public abstract Tool GetTool(ulong environmentId, ulong id);

        /// <summary>
        /// Whether this tool is projected.
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public virtual bool IsProjected(AbstractTool tool)
        {
            return tool_SO.projectedTools.Contains(tool);
        }

        /// <summary>
        /// Whether or not <paramref name="tool"/> requires the generation of a menu to be projected.
        /// </summary>
        /// <param name="tool"> The tool to be projected.</param>
        /// <returns></returns>
        public abstract bool RequiresMenu(Tool tool);

        /// <summary>
        /// Create a menu to access each interactions of a tool separately.
        /// </summary>
        /// <param name="interactions"></param>
        public abstract void CreateInteractionsMenuFor(Tool tool);

        /// <summary>
        /// Project <paramref name="tool"/> on this controller.
        /// </summary>
        /// <param name="tool"></param>
        public virtual void ProjectTool(Tool tool)
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
        public virtual void ReleaseTool(Tool tool)
        {
            tool_SO.projectedTools.Remove(tool);
        }

        /// <summary>
        /// Whether this <paramref name="input"/> is associated to <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual bool IsAssociated(Tool tool, AbstractUMI3DInput input)
        {
            if (!tool_SO.inputsByTool.ContainsKey(tool.id))
            {
                return false;
            }

            return tool_SO.inputsByTool[tool.id].Contains(input);
        }

        /// <summary>
        /// Associates <paramref name="inputs"/> to this <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="inputs"></param>
        public virtual void AssociateInputs(Tool tool, params AbstractUMI3DInput[] inputs)
        {
            if (!tool_SO.inputsByTool.ContainsKey(tool.id))
            {
                tool_SO.inputsByTool.Add(tool.id, inputs);
            }
            else
            {
                tool_SO.inputsByTool[tool.id] = tool_SO.inputsByTool[tool.id]
                    .Concat(inputs)
                    .ToArray();
            }
        }

        /// <summary>
        /// Dissociates <paramref name="input"/> from <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="input"></param>
        public virtual void DissociateInput(Tool tool, AbstractUMI3DInput input)
        {
            if (!tool_SO.inputsByTool.ContainsKey(tool.id))
            {
                return;
            }

            var tmp = new List<AbstractUMI3DInput>(tool_SO.inputsByTool[tool.id]);
            tmp.Remove(input);
            tool_SO.inputsByTool[tool.id] = tmp.ToArray();
        }

        /// <summary>
        /// Dissociates all inputs from <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        public virtual void DissociateAllInputs(Tool tool)
        {
            if (!tool_SO.inputsByTool.ContainsKey(tool.id))
            {
                return;
            }

            tool_SO.inputsByTool.Remove(tool.id);
        }
    }
}