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
using System.Collections.Generic;
using System.Linq;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    /// <summary>
    /// A controller is a set of inputs.<br/>
    /// <br/>
    /// 
    /// <example>
    /// For example: The following device can be seen as a controller.
    /// <list type="bullet">
    /// <item>A VR controller.</item>
    /// <item>A keyboard and a mouse.</item>
    /// </list>
    /// </example>
    /// 
    /// You have as many controller as you have of selector.<br/>
    /// <example>
    /// For example: A VR headset has 2 controllers with laser to select. A computer as 1 mouse to select (mouse and trackpad can be seen as the same input).
    /// </example>
    /// </summary>
    [System.Serializable]
    public abstract class AbstractController : MonoBehaviour
    {
        public UMI3DInputManager inputManager;
        public UMI3DToolManager toolManager;
        public ProjectionManager projectionManager;

        #region properties

        /// <summary>
        /// Controller's inputs.
        /// </summary>
        public abstract List<AbstractUMI3DInput> inputs { get; }

        /// <summary>
        /// Inputs associated to a given tool (keys are tools' ids).
        /// </summary>
        protected Dictionary<ulong, AbstractUMI3DInput[]> associatedInputs = new Dictionary<ulong, AbstractUMI3DInput[]>();

        #endregion

        #region interface

        /// <summary>
        /// Clear all menus and the projected tools
        /// </summary>
        public abstract void Clear();

        #endregion

        /// <summary>
        /// Project a tool on this controller.
        /// </summary>
        /// <param name="tool"> The ToolDto to be projected.</param>
        /// <see cref="Release(AbstractTool)"/>
        public virtual void Project(AbstractTool tool, bool releasable, InteractionMappingReason reason, ulong hoveredObjectId)
        {
            if (!toolManager.toolDelegate.IsCompatibleWith(tool))
                throw new System.Exception("Trying to project an uncompatible tool !");

            if (toolManager.toolDelegate.Tool != null)
                throw new System.Exception("A tool is already projected !");

            if (toolManager.toolDelegate.RequiresMenu(tool))
            {
                toolManager.toolDelegate.CreateInteractionsMenuFor(tool);
            }
            else
            {
                AbstractInteractionDto[] interactions = tool.interactionsLoaded.ToArray();
                AbstractUMI3DInput[] inputs = projectionManager.Project(this, tool.environmentId, interactions, tool.id, hoveredObjectId);
                associatedInputs.Add(tool.id, inputs);
            }

            toolManager.toolDelegate.Tool = tool;
        }


        /// <summary>
        /// Project a tool on this controller.
        /// </summary>
        /// <param name="tool"> The ToolDto to be projected.</param>
        /// <see cref="Release(AbstractTool)"/>
        public virtual void Update(AbstractTool tool, bool releasable, InteractionMappingReason reason)
        {
            if (toolManager.toolDelegate.Tool != tool)
                throw new System.Exception("Try to update wrong tool");

            Release(tool, new ToolNeedToBeUpdated());
            Project(tool, releasable, reason, toolManager.toolDelegate.CurrentHoverTool.id);
        }


        /// <summary>
        /// Release a projected tool from this controller.
        /// </summary>
        /// <param name="tool">Tool to release</param>
        /// <see cref="Project(AbstractTool)"/>
        public virtual void Release(AbstractTool tool, InteractionMappingReason reason)
        {
            if (toolManager.toolDelegate.Tool == null)
                throw new System.Exception("no tool is currently projected on this controller");
            if (toolManager.toolDelegate.Tool.id != tool.id)
                throw new System.Exception("This tool is not currently projected on this controller");

            if (associatedInputs.TryGetValue(tool.id, out AbstractUMI3DInput[] inputs))
            {
                foreach (AbstractUMI3DInput input in inputs)
                {
                    if (input.CurrentInteraction() != null)
                        input.Dissociate();
                }
                associatedInputs.Remove(tool.id);
            }
            toolManager.toolDelegate.Tool = null;
        }

        /// <summary>
        /// Change a tool on this controller to add a new interaction
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="releasable"></param>
        /// <param name="abstractInteractionDto"></param>
        /// <param name="reason"></param>
        public virtual void AddUpdate(AbstractTool tool, bool releasable, AbstractInteractionDto abstractInteractionDto, InteractionMappingReason reason)
        {
            if (toolManager.toolDelegate.Tool != tool)
                throw new System.Exception("Try to update wrong tool");

            if (toolManager.toolDelegate.RequiresMenu(tool))
            {
                toolManager.toolDelegate.CreateInteractionsMenuFor(tool);
            }
            else
            {
                var interaction = new AbstractInteractionDto[] { abstractInteractionDto };
                AbstractUMI3DInput[] inputs = projectionManager.Project(this, tool.environmentId, interaction, tool.id, toolManager.toolDelegate.CurrentHoverTool.id);
                if (associatedInputs.ContainsKey(tool.id))
                {
                    associatedInputs[tool.id] = associatedInputs[tool.id].Concat(inputs).ToArray();
                }
                else
                {
                    associatedInputs.Add(tool.id, inputs);
                }
            }
            toolManager.toolDelegate.Tool = tool;
        }
    }
}