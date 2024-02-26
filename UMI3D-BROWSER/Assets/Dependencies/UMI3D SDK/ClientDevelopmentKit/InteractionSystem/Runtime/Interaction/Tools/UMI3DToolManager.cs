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
        public AbstractToolDelegate<Interactable> interactableToolDelegate;
        public AbstractToolDelegate<GlobalTool> globalToolDelegate;
        public AbstractToolDelegate<Toolbox> toolboxDelegate;

        public void Init()
        {
            interactableToolDelegate.tool_SO = tool_SO;
            globalToolDelegate.tool_SO = tool_SO;
            toolboxDelegate.tool_SO = tool_SO;
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
        )
            where Tool: AbstractTool
        {
            return default(Tool) switch
            {
                Interactable => interactableToolDelegate.Exists(environmentId, id),
                Toolbox => toolboxDelegate.Exists(environmentId, id),
                GlobalTool => globalToolDelegate.Exists(environmentId, id),
                _ => throw new NoToolFoundException()
            };
        }

        /// <summary>
        /// Get the tool with the given id (if any).
        /// </summary>
        public AbstractTool GetTool<Tool>(
            ulong environmentId,
            ulong id
        )
        {
            return default(Tool) switch
            {
                Interactable => interactableToolDelegate.GetTool(environmentId, id),
                Toolbox => toolboxDelegate.GetTool(environmentId, id),
                GlobalTool => globalToolDelegate.GetTool(environmentId, id),
                _ => throw new NoToolFoundException()
            };
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

        public bool IsProjected(AbstractTool tool)
        {
            switch (tool)
            {
                case Interactable interactable:
                    return interactableToolDelegate.IsProjected(interactable);
                case Toolbox toolbox:
                    return toolboxDelegate.IsProjected(toolbox);
                case GlobalTool globalTool:
                    return globalToolDelegate.IsProjected(globalTool);
                default:
                    throw new NoToolFoundException();
            }
        }

        /// <summary>
        /// Whether or not <paramref name="tool"/> requires the generation of a menu to be projected.
        /// </summary>
        /// <param name="tool"> The tool to be projected.</param>
        /// <returns></returns>
        public bool RequiresMenu(AbstractTool tool)
        {
            switch (tool)
            {
                case Interactable interactable:
                    return interactableToolDelegate.RequiresMenu(interactable);
                case Toolbox toolbox:
                    return toolboxDelegate.RequiresMenu(toolbox);
                case GlobalTool globalTool:
                    return globalToolDelegate.RequiresMenu(globalTool);
                default:
                    throw new NoToolFoundException();
            }
        }

        /// <summary>
        /// Create a menu to access each interactions of a tool separately.
        /// </summary>
        /// <param name="interactions"></param>
        public void CreateInteractionsMenuFor(AbstractTool tool)
        {
            switch (tool)
            {
                case Interactable interactable:
                    interactableToolDelegate.CreateInteractionsMenuFor(interactable);
                    break;
                case Toolbox toolbox:
                    toolboxDelegate.CreateInteractionsMenuFor(toolbox);
                    break;
                case GlobalTool globalTool:
                    globalToolDelegate.CreateInteractionsMenuFor(globalTool);
                    break;
                default:
                    throw new NoToolFoundException();
            }
        }

        /// <summary>
        /// Project <paramref name="tool"/> on this controller.
        /// </summary>
        /// <param name="tool"></param>
        public void ProjectTool(AbstractTool tool)
        {
            switch (tool)
            {
                case Interactable interactable:
                    interactableToolDelegate.ProjectTool(interactable);
                    break;
                case Toolbox toolbox:
                    toolboxDelegate.ProjectTool(toolbox);
                    break;
                case GlobalTool globalTool:
                    globalToolDelegate.ProjectTool(globalTool);
                    break;
                default:
                    throw new NoToolFoundException();
            }
        }

        /// <summary>
        /// Release tool from this controller.
        /// </summary>
        /// <param name="tool"></param>
        public void ReleaseTool(AbstractTool tool)
        {
            switch (tool)
            {
                case Interactable interactable:
                    interactableToolDelegate.ReleaseTool(interactable);
                    break;
                case Toolbox toolbox:
                    toolboxDelegate.ReleaseTool(toolbox);
                    break;
                case GlobalTool globalTool:
                    globalToolDelegate.ReleaseTool(globalTool);
                    break;
                default:
                    throw new NoToolFoundException();
            }
        }

        /// <summary>
        /// Whether this <paramref name="input"/> is associated to <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsAssociated(AbstractTool tool, AbstractUMI3DInput input)
        {
            switch (tool)
            {
                case Interactable interactable:
                    return interactableToolDelegate.IsAssociated(interactable, input);
                case Toolbox toolbox:
                    return toolboxDelegate.IsAssociated(toolbox, input);
                case GlobalTool globalTool:
                    return globalToolDelegate.IsAssociated(globalTool, input);
                default:
                    throw new NoToolFoundException();
            }
        }

        /// <summary>
        /// Associates <paramref name="inputs"/> to this <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="inputs"></param>
        public void AssociateInputs(AbstractTool tool, params AbstractUMI3DInput[] inputs)
        {
            switch (tool)
            {
                case Interactable interactable:
                    interactableToolDelegate.AssociateInputs(interactable, inputs);
                    break;
                case Toolbox toolbox:
                    toolboxDelegate.AssociateInputs(toolbox, inputs);
                    break;
                case GlobalTool globalTool:
                    globalToolDelegate.AssociateInputs(globalTool, inputs);
                    break;
                default:
                    throw new NoToolFoundException();
            }
        }

        /// <summary>
        /// Dissociates <paramref name="input"/> from <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="input"></param>
        public void DissociateInput(AbstractTool tool, AbstractUMI3DInput input)
        {
            switch (tool)
            {
                case Interactable interactable:
                    interactableToolDelegate.DissociateInput(interactable, input);
                    break;
                case Toolbox toolbox:
                    toolboxDelegate.DissociateInput(toolbox, input);
                    break;
                case GlobalTool globalTool:
                    globalToolDelegate.DissociateInput(globalTool, input);
                    break;
                default:
                    throw new NoToolFoundException();
            }
        }

        /// <summary>
        /// Dissociates all inputs from <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool"></param>
        public void DissociateAllInputs(AbstractTool tool)
        {
            switch (tool)
            {
                case Interactable interactable:
                    interactableToolDelegate.DissociateAllInputs(interactable);
                    break;
                case Toolbox toolbox:
                    toolboxDelegate.DissociateAllInputs(toolbox);
                    break;
                case GlobalTool globalTool:
                    globalToolDelegate.DissociateAllInputs(globalTool);
                    break;
                default:
                    throw new NoToolFoundException();
            }
        }
    }
}