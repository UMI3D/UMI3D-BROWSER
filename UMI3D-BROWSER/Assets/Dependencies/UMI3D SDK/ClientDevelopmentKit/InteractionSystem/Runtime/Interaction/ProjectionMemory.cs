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
using System.Collections.Generic;
using umi3d.common.interaction;
using umi3d.debug;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    /// <summary>
    /// Saves and manages the links between projected tools and their associated inputs. 
    /// This projection is based on a tree constituted of <see cref="ProjectionTreeNode"/>.
    /// </summary>
    public class ProjectionMemory : MonoBehaviour
    {
        [SerializeField]
        UMI3DLogger logger = new();
        public ProjectionTree_SO projectionTree_SO;
        public ProjectionTreeManipulationNodeDelegate ptManipulationNodeDelegate;
        public ProjectionTreeEventNodeDelegate ptEventNodeDelegate;
        public ProjectionTreeFormNodeDelegate ptFormNodeDelegate;
        public ProjectionTreeLinkNodeDelegate ptLinkNodeDelegate;
        public ProjectionTreeParameterNodeDelegate ptParameterNodeDelegate;

        /// <summary>
        /// Projection memory.
        /// </summary>
        protected ProjectionTreeNode memoryRoot;

        protected string treeId = "";
        /// <summary>
        /// Id of the projection tree.
        /// </summary>
        public string TreeId
        {
            get
            {
                if (string.IsNullOrEmpty(treeId))
                {
                    treeId = (this.gameObject.GetInstanceID() + Random.Range(0, 1000)).ToString();
                }
                return treeId;
            }
        }

        protected virtual void Awake()
        {
            logger.MainContext = this;
            logger.MainTag = nameof(ProjectionMemory);
            memoryRoot = new ProjectionTreeNode(TreeId) { id = 0 };
        }

        private void Start()
        {
            logger.Assert(ptManipulationNodeDelegate != null, $"{nameof(ptManipulationNodeDelegate)} is null");
            logger.Assert(ptEventNodeDelegate != null, $"{nameof(ptEventNodeDelegate)} is null");
            logger.Assert(ptFormNodeDelegate != null, $"{nameof(ptFormNodeDelegate)} is null");
            logger.Assert(ptLinkNodeDelegate != null, $"{nameof(ptLinkNodeDelegate)} is null");
            logger.Assert(ptParameterNodeDelegate != null, $"{nameof(ptParameterNodeDelegate)} is null");

            ptManipulationNodeDelegate.Init(projectionTree_SO, TreeId);
            ptEventNodeDelegate.Init(projectionTree_SO, TreeId);
            ptFormNodeDelegate.Init(projectionTree_SO, TreeId);
            ptLinkNodeDelegate.Init(projectionTree_SO, TreeId);
            ptParameterNodeDelegate.Init(projectionTree_SO, TreeId);
        }

        /// <summary>
        /// Get Inputs of a controller for a list of interactions.
        /// </summary>
        /// <param name="controller">The controller on which the input should be</param>
        /// <param name="interactions">the array of interaction for which an input is seeked</param>
        /// <param name="unused"></param>
        /// <returns></returns>
        public AbstractUMI3DInput[] GetInputs(
            AbstractController controller,
            AbstractInteractionDto[] interactions,
            bool unused = true
        )
        {
            ProjectionTreeNode currentMemoryTreeState = memoryRoot;

            System.Func<ProjectionTreeNode> deepProjectionCreation;
            System.Predicate<ProjectionTreeNode> adequation;
            System.Action<ProjectionTreeNode> chooseProjection;

            List<AbstractUMI3DInput> selectedInputs = new();

            for (int depth = 0; depth < interactions.Length; depth++)
            {
                AbstractInteractionDto interaction = interactions[depth];

                switch (interaction)
                {
                    case ManipulationDto manipulationDto:

                        DofGroupOptionDto[] options = manipulationDto.dofSeparationOptions.ToArray();
                        DofGroupOptionDto bestDofGroupOption = controller.FindBest(options);

                        foreach (DofGroupDto sep in bestDofGroupOption.separations)
                        {
                            ptManipulationNodeDelegate.sep = sep;
                            ptManipulationNodeDelegate.PrepareForNodeFactory(
                                manipulationDto,
                                () =>
                                {
                                    return controller.FindInput(manipulationDto, sep, unused);
                                },
                                out adequation,
                                out deepProjectionCreation,
                                out chooseProjection,
                                selectedInputs
                            );

                            currentMemoryTreeState = Project(
                                currentMemoryTreeState,
                                adequation,
                                deepProjectionCreation,
                                chooseProjection
                            );
                        }
                        break;

                    case EventDto eventDto:

                        ptEventNodeDelegate.PrepareForNodeFactory(
                            eventDto, 
                            () =>
                            {
                                return controller.FindInput(eventDto, unused);
                            }, 
                            out adequation, 
                            out deepProjectionCreation, 
                            out chooseProjection, 
                            selectedInputs
                        );

                        currentMemoryTreeState = Project(
                            currentMemoryTreeState,
                            adequation,
                            deepProjectionCreation,
                            chooseProjection
                        );
                        break;

                    case FormDto formDto:

                        ptFormNodeDelegate.PrepareForNodeFactory(
                            formDto, 
                            () =>
                            {
                                return controller.FindInput(formDto, unused);
                            },
                            out adequation, 
                            out deepProjectionCreation, 
                            out chooseProjection, 
                            selectedInputs
                        );

                        currentMemoryTreeState = Project(
                            currentMemoryTreeState,
                            adequation,
                            deepProjectionCreation,
                            chooseProjection
                        );

                        break;

                    case LinkDto linkDto:

                        ptLinkNodeDelegate.PrepareForNodeFactory(linkDto, 
                            () =>
                            {
                                return controller.FindInput(linkDto, unused);
                            },
                            out adequation,
                            out deepProjectionCreation,
                            out chooseProjection,
                            selectedInputs
                        );

                        currentMemoryTreeState = Project(
                            currentMemoryTreeState, 
                            adequation, 
                            deepProjectionCreation, 
                            chooseProjection
                        );
                        break;

                    case AbstractParameterDto parameterDto:

                        ptParameterNodeDelegate.PrepareForNodeFactory(
                            parameterDto,
                            () =>
                            {
                                return controller.FindInput(parameterDto, unused);
                            },
                            out adequation,
                            out deepProjectionCreation,
                            out chooseProjection,
                            selectedInputs
                        );

                        currentMemoryTreeState = Project(
                            currentMemoryTreeState, 
                            adequation, 
                            deepProjectionCreation, 
                            chooseProjection
                        );
                        break;

                    default:
                        throw new System.Exception("Unknown interaction type, can't project !");
                }
            }

            return selectedInputs.ToArray();
        }

        /// <summary>
        /// Project a dto on a controller and return associated input.
        /// </summary>
        /// <param name="controller">Controller to project on</param>
        /// <param name="evt">Event dto to project</param>
        /// <param name="unusedInputsOnly">Project on unused inputs only</param>
        public AbstractUMI3DInput PartialProject<Dto>(
            AbstractController controller,
            ulong environmentId,
            Dto dto,
            ulong toolId,
            ulong hoveredObjectId,
            bool unusedInputsOnly = false,
            DofGroupDto dof = null
        )
            where Dto : AbstractInteractionDto
        {
            System.Predicate<ProjectionTreeNode> adequation;
            System.Func<ProjectionTreeNode> deepProjectionCreation;
            System.Action<ProjectionTreeNode> chooseProjection;

            switch (dto)
            {
                case ManipulationDto manipulationDto:
                    ptManipulationNodeDelegate.sep = dof;
                    ptManipulationNodeDelegate.PartialProject(
                        manipulationDto,
                        environmentId,
                        toolId,
                        hoveredObjectId,
                        unusedInputsOnly,
                        () =>
                        {
                            return controller.FindInput(manipulationDto, dof, unusedInputsOnly);
                        },
                        out adequation,
                        out deepProjectionCreation,
                        out chooseProjection
                    );
                    break;
                case EventDto eventDto:
                    ptEventNodeDelegate.PartialProject(
                        eventDto,
                        environmentId,
                        toolId,
                        hoveredObjectId,
                        unusedInputsOnly,
                        () =>
                        {
                            return controller.FindInput(eventDto, true);
                        },
                        out adequation,
                        out deepProjectionCreation,
                        out chooseProjection
                    );
                    break;
                case FormDto formDto:
                    ptFormNodeDelegate.PartialProject(
                        formDto,
                        environmentId,
                        toolId,
                        hoveredObjectId,
                        unusedInputsOnly,
                        () =>
                        {
                            return controller.FindInput(formDto, true);
                        },
                        out adequation,
                        out deepProjectionCreation,
                        out chooseProjection
                    );
                    break;
                case LinkDto linkDto:
                    ptLinkNodeDelegate.PartialProject(
                        linkDto,
                        environmentId,
                        toolId,
                        hoveredObjectId,
                        unusedInputsOnly,
                        () =>
                        {
                            return controller.FindInput(linkDto, true);
                        },
                        out adequation,
                        out deepProjectionCreation,
                        out chooseProjection
                    );
                    break;
                case AbstractParameterDto parameterDto:
                    ptParameterNodeDelegate.PartialProject(
                        parameterDto,
                        environmentId,
                        toolId,
                        hoveredObjectId,
                        unusedInputsOnly,
                        () =>
                        {
                            return controller.FindInput(parameterDto, true);
                        },
                        out adequation,
                        out deepProjectionCreation,
                        out chooseProjection
                    );
                    break;
                default:
                    throw new System.Exception("Unknown interaction type : " + dto);
            }

            return Project(memoryRoot, adequation, deepProjectionCreation, chooseProjection, unusedInputsOnly).projectedInput;
        }

        /// <summary>
        /// Project on a given controller a set of interactions and return associated inputs.
        /// </summary>
        /// <param name="controller">Controller to project interactions on</param>
        /// <param name="interactions">Interactions to project</param>
        public AbstractUMI3DInput[] Project(
            AbstractController controller, 
            ulong environmentId, 
            AbstractInteractionDto[] interactions, 
            ulong toolId, 
            ulong hoveredObjectId
        )
        {
            bool foundHoldableEvent = false;
            if (InteractionMapper.Instance.shouldProjectHoldableEventOnSpecificInput)
            {
                var temp = new List<AbstractInteractionDto>();

                foreach (AbstractInteractionDto dto in interactions)
                {
                    if (dto is EventDto eventDto && eventDto.hold && !foundHoldableEvent)
                    {
                        temp.Insert(0, dto);
                        foundHoldableEvent = true;
                    }
                    else
                    {
                        temp.Add(dto);
                    }
                }

                if (foundHoldableEvent)
                {
                    interactions = temp.ToArray();
                }
            }

            ProjectionTreeNode currentMemoryTreeState = memoryRoot;

            System.Func<ProjectionTreeNode> deepProjectionCreation;
            System.Predicate<ProjectionTreeNode> adequation;
            System.Action<ProjectionTreeNode> chooseProjection;

            var selectedInputs = new List<AbstractUMI3DInput>();

            for (int depth = 0; depth < interactions.Length; depth++)
            {
                AbstractInteractionDto interaction = interactions[depth];

                switch (interaction)
                {
                    case ManipulationDto manipulationDto:

                        DofGroupOptionDto[] options = (interaction as ManipulationDto).dofSeparationOptions.ToArray();
                        DofGroupOptionDto bestDofGroupOption = controller.FindBest(options);

                        foreach (DofGroupDto sep in bestDofGroupOption.separations)
                        {
                            ptManipulationNodeDelegate.sep = sep;
                            ptManipulationNodeDelegate.PrepareForNodeFactory(
                                manipulationDto,
                                environmentId,
                                toolId,
                                hoveredObjectId,
                                () =>
                                {
                                    return controller.FindInput(manipulationDto, sep, true);
                                },
                                out adequation,
                                out deepProjectionCreation,
                                out chooseProjection,
                                selectedInputs
                            );

                            currentMemoryTreeState = Project(
                                currentMemoryTreeState, 
                                adequation, 
                                deepProjectionCreation, 
                                chooseProjection
                            );

                        }
                        break;

                    case EventDto eventDto:

                        ptEventNodeDelegate.PrepareForNodeFactory(
                            eventDto,
                            environmentId,
                            toolId,
                            hoveredObjectId,
                            () =>
                            {
                                return controller.FindInput(eventDto, true, depth == 0 && foundHoldableEvent);
                            },
                            out adequation,
                            out deepProjectionCreation,
                            out chooseProjection,
                            selectedInputs
                        );

                        currentMemoryTreeState = Project(
                            currentMemoryTreeState, 
                            adequation, 
                            deepProjectionCreation, 
                            chooseProjection
                        );
                        break;

                    case FormDto formDto:

                        ptFormNodeDelegate.PrepareForNodeFactory(
                            formDto,
                            environmentId,
                            toolId,
                            hoveredObjectId,
                            () =>
                            {
                                return controller.FindInput(formDto, true);
                            },
                            out adequation,
                            out deepProjectionCreation,
                            out chooseProjection,
                            selectedInputs
                        );

                        currentMemoryTreeState = Project(
                            currentMemoryTreeState,
                            adequation,
                            deepProjectionCreation,
                            chooseProjection
                        );
                        break;

                    case LinkDto linkDto:

                        ptLinkNodeDelegate.PrepareForNodeFactory(
                            linkDto,
                            environmentId,
                            toolId,
                            hoveredObjectId,
                            () =>
                            {
                                return controller.FindInput(linkDto, true);
                            },
                            out adequation,
                            out deepProjectionCreation,
                            out chooseProjection,
                            selectedInputs
                        );

                        currentMemoryTreeState = Project(
                            currentMemoryTreeState,
                            adequation,
                            deepProjectionCreation,
                            chooseProjection
                        );
                        break;

                    case AbstractParameterDto parameterDto:

                        ptParameterNodeDelegate.PrepareForNodeFactory(
                            parameterDto,
                            environmentId,
                            toolId,
                            hoveredObjectId,
                            () =>
                            {
                                return controller.FindInput(parameterDto, true);
                            },
                            out adequation,
                            out deepProjectionCreation,
                            out chooseProjection,
                            selectedInputs
                        );

                        currentMemoryTreeState = Project(
                            currentMemoryTreeState,
                            adequation,
                            deepProjectionCreation,
                            chooseProjection
                        );
                        break;

                    default:
                        throw new System.Exception("Unknown interaction type : " + interaction);
                }
            }

            return selectedInputs.ToArray();
        }

        /// <summary>
        /// Navigates through tree and project an interaction. Updates the tree if necessary.
        /// </summary>
        /// <param name="nodeAdequationTest">Decides if the given projection node is adequate for the interaction to project</param>
        /// <param name="deepProjectionCreation">Create a new deep projection node, should throw an <see cref="NoInputFoundException"/> if no input is available</param>
        /// <param name="chooseProjection">Project the interaction to the given node's input</param>
        /// <param name="currentTreeNode">Current node in tree projection</param>
        /// <param name="unusedInputsOnly">Project on unused inputs only</param>
        /// <exception cref="NoInputFoundException"></exception>
        private ProjectionTreeNode Project(ProjectionTreeNode currentTreeNode,
            System.Predicate<ProjectionTreeNode> nodeAdequationTest,
            System.Func<ProjectionTreeNode> deepProjectionCreation,
            System.Action<ProjectionTreeNode> chooseProjection,
            bool unusedInputsOnly = true,
            bool updateMemory = true)
        {
            if (!unusedInputsOnly)
            {
                try
                {
                    ProjectionTreeNode p = Project(currentTreeNode, nodeAdequationTest, deepProjectionCreation, chooseProjection, true, updateMemory);
                    return p;
                }
                catch (NoInputFoundException) { }
            }

            ProjectionTreeNode deepProjection = currentTreeNode.children.Find(nodeAdequationTest);
            if (deepProjection != null)
            {
                if (unusedInputsOnly && !deepProjection.projectedInput.IsAvailable())
                {
                    ProjectionTreeNode alternativeProjection = deepProjectionCreation();
                    chooseProjection(alternativeProjection);
                    return alternativeProjection;
                }
                else
                {
                    chooseProjection(deepProjection);
                    return deepProjection;
                }
            }
            else
            {
                ProjectionTreeNode rootProjection = memoryRoot.children.Find(nodeAdequationTest);
                if ((rootProjection == null) || (unusedInputsOnly && !rootProjection.projectedInput.IsAvailable()))
                {
                    deepProjection = deepProjectionCreation();
                    chooseProjection(deepProjection);
                    if (updateMemory)
                        currentTreeNode.AddChild(deepProjection);
                    return deepProjection;
                }
                else
                {
                    chooseProjection(rootProjection);
                    if (updateMemory)
                        currentTreeNode.AddChild(rootProjection);
                    return rootProjection;
                }
            }
        }

        /// <summary>
        /// Save current state of the memory.
        /// </summary>
        /// <param name="path">path to the file</param>
        public void SaveToFile(string path)
        {
            memoryRoot.SaveToFile(path);
        }

        /// <summary>
        /// Load a state of memory.
        /// </summary>
        /// <param name="path">path to the file</param>
        public void LoadFromFile(string path)
        {
            memoryRoot.LoadFromFile(path);
        }

        /// <summary>
        /// Exception thrown when not associated input has been found for an interaction.
        /// </summary>
        public class NoInputFoundException : System.Exception
        {
            public NoInputFoundException() { }
            public NoInputFoundException(string message) : base(message) { }
            public NoInputFoundException(string message, System.Exception inner) : base(message, inner) { }
        }
    }





    /// <summary>
    /// Projection tree node associated to an <see cref="EventDto"/>.
    /// </summary>
    [System.Serializable]
    public class EventNode : ProjectionTreeNode
    {
        /// <summary>
        /// Associated Event DTO
        /// </summary>
        [SerializeField, Tooltip("Associated Event DTO")]
        public EventDto evt;

        public EventNode(string treeId) : base(treeId) { }
    }

    /// <summary>
    /// Projection tree node associated to a <see cref="ManipulationDto"/>.
    /// </summary>
    [System.Serializable]
    public class ManipulationNode : ProjectionTreeNode
    {
        /// <summary>
        /// Associated Manipulation DTO
        /// </summary>
        [SerializeField, Tooltip("Associated Manipulation DTO")]
        public ManipulationDto manipulation;

        /// <summary>
        /// Associated Degree of Freedom Group DTO
        /// </summary>
        [SerializeField, Tooltip("Associated Degree of Freedom Group DTO")]
        public DofGroupDto manipulationDofGroupDto;

        public ManipulationNode(string treeId) : base(treeId) { }
    }

    /// <summary>
    /// Projection tree node associated to a <see cref="FormDto"/>.
    /// </summary>
    [System.Serializable]
    public class FormNode : ProjectionTreeNode
    {
        /// <summary>
        /// Associated Form DTO
        /// </summary>
        [SerializeField, Tooltip("Associated Form DTO")]
        public FormDto form;

        public FormNode(string treeId) : base(treeId) { }
    }

    /// <summary>
    /// Projection tree node associated to a <see cref="LinkDto"/>.
    /// </summary>
    [System.Serializable]
    public class LinkNode : ProjectionTreeNode
    {
        /// <summary>
        /// Associated Link DTO
        /// </summary>
        [SerializeField, Tooltip("Associated Link DTO")]
        public LinkDto link;

        public LinkNode(string treeId) : base(treeId) { }
    }

    /// <summary>
    /// Projection tree node associated to an <see cref="AbstractParameterDto"/>.
    /// </summary>
    [System.Serializable]
    public class ParameterNode : ProjectionTreeNode
    {
        /// <summary>
        /// Associated Parameter DTO
        /// </summary>
        [SerializeField, Tooltip("Associated Parameter DTO")]
        public AbstractParameterDto parameter;

        public ParameterNode(string treeId) : base(treeId) { }
    }
}