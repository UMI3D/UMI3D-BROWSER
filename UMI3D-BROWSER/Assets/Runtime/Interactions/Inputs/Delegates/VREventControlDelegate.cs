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

using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine.InputSystem;

namespace umi3d.browserRuntime.interaction
{
    public class VREventControlDelegate : IControlEventDelegate
    {
        IControlEventDelegate eventDelegate;

        public VREventControlDelegate(IControlEventDelegate eventDelegate)
        {
            this.eventDelegate = eventDelegate;
        }

        public bool TryToFindInputForHoldableEvent
        {
            get
            {
                return eventDelegate.TryToFindInputForHoldableEvent;
            }
            set
            {
                eventDelegate.TryToFindInputForHoldableEvent = value;
            }
        }

        public void Associate(
            UMI3DController controller,
            AbstractControlEntity control,
            ulong environmentId,
            AbstractInteractionDto interaction,
            ulong toolId,
            ulong hoveredObjectId
        )
        {
            eventDelegate.Associate(controller, control, environmentId, interaction, toolId, hoveredObjectId);
        }

        public bool CanPerform(InputActionPhase phase)
        {
            throw new System.NotImplementedException();
        }

        public void Dissociate(AbstractControlEntity control)
        {
            eventDelegate.Dissociate(control);
        }

        public AbstractControlEntity GetControl(UMI3DController controller, EventDto interaction)
        {
            return eventDelegate.GetControl(controller, interaction);
        }
    }
}