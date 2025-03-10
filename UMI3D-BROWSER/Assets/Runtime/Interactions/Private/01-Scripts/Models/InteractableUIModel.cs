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

using inetum.unityUtils.observation;
using System;
using UnityEngine;

namespace umi3d.browserRuntime.interactions
{
    internal class InteractableUIModel 
    {
        public float farDistanceOffset = 0f;
        public float offsetWithRenderer = .1f;

        public IInteractableUIDataDelegate dataDelegate;
        public Delegates<IInteractableUIDelegate> delegates = new();

        public InteractableVisibilityState visibilityState { get; private set; }
        public InteractableHoveringState hoveringState { get; private set; }
        public InteractableDistanceState distanceState { get; private set; }

        float _farDistance => (dataDelegate?.interactionDistance ?? 0) + farDistanceOffset;

        public void OnBecameVisible()
        {
            InteractableVisibilityState oldState = visibilityState;
            InteractableVisibilityState newState = InteractableVisibilityState.Visible;
            visibilityState = newState;

            if (oldState != newState)
            {
                delegates.ForEach(@delegate =>
                {
                    @delegate.OnChangeOfVisibilityState(oldState, newState);
                    return Flow.Continue;
                });
            }
        }

        public void OnBecameInvisible()
        {
            InteractableVisibilityState oldState = visibilityState;
            InteractableVisibilityState newState = InteractableVisibilityState.NotVisible;
            visibilityState = newState;

            if (oldState != newState)
            {
                delegates.ForEach(@delegate =>
                {
                    @delegate.OnChangeOfVisibilityState(oldState, newState);
                    return Flow.Continue;
                });
            }
        }

        public void OnBecameHovered()
        {
            InteractableHoveringState oldState = hoveringState;
            InteractableHoveringState newState = InteractableHoveringState.Hover;
            hoveringState = newState;

            if (oldState != newState)
            {
                delegates.ForEach(@delegate =>
                {
                    @delegate.OnChangeOfHoveringState(oldState, newState);
                    return Flow.Continue;
                });
            }
        }

        public void OnBecameNotHovered()
        {
            InteractableHoveringState oldState = hoveringState;
            InteractableHoveringState newState = InteractableHoveringState.NotHover;
            hoveringState = newState;

            if (oldState != newState)
            {
                delegates.ForEach(@delegate =>
                {
                    @delegate.OnChangeOfHoveringState(oldState, newState);
                    return Flow.Continue;
                });
            }
        }

        public void UpdateDistanceState()
        {
            void Update(InteractableDistanceState newState)
            {
                InteractableDistanceState oldState = distanceState;
                distanceState = newState;

                if (oldState != newState)
                {
                    delegates.ForEach(@delegate =>
                    {
                        @delegate.OnChangeOfDistanceState(oldState, newState);
                        return Flow.Continue;
                    });
                }
            }

            if (IsCloseDistance()) { Update(InteractableDistanceState.Close); }
            else if (IsFarDistance()) { Update(InteractableDistanceState.Far); }
            else { Update(InteractableDistanceState.Middle); }
        }

        /// <summary>
        /// The distance at which:<br/> 
        /// - the feedback is visible as a round shape when the user is not hovering the interactable and as a circle when it is.<br/>
        /// - the label and the interaction keys are visible when the user is hovering the interactable.
        /// </summary>
        /// <returns></returns>
        bool IsCloseDistance()
        {
            if (dataDelegate?.interactionDistance < 0) return true;

            return dataDelegate?.interactionDistance > dataDelegate?.distanceCameraRenderer;
        }

        /// <summary>
        /// The distance at which the feedback is not visible.
        /// </summary>
        /// <returns></returns>
        bool IsFarDistance()
        {
            if (dataDelegate?.interactionDistance <= 0) { return false; }

            return dataDelegate?.distanceCameraRenderer > _farDistance;
        }

        public Vector3 GetLocalScale(float WorldScale, Vector3 parentLocalScale, Vector3 parentLossyScale)
        {
            float _scaleX = WorldScale * parentLocalScale.x / parentLossyScale.x;
            float _scaleY = WorldScale * parentLocalScale.y / parentLossyScale.y;
            float _scaleZ = WorldScale * parentLocalScale.z / parentLossyScale.z;

            return new(_scaleX, _scaleY, _scaleZ);
        }

        public Vector3 GetFrontPositionOffset(Vector3 rendererSize)
        {
            float length = Mathf.Max(rendererSize.x, Mathf.Max(rendererSize.y, rendererSize.z));
            Vector3 direction = dataDelegate?.directionRendererCamera ?? Vector3.zero;

            return direction * (length / 2 + offsetWithRenderer);
        }
    }

    internal interface IInteractableUIDataDelegate
    {
        float distanceCameraRenderer { get; }
        /// <summary>
        /// The normalized direction from the renderer to the camera.
        /// </summary>
        Vector3 directionRendererCamera { get; }
        float interactionDistance { get; }

    }

    internal interface IInteractableUIDelegate
    {
        void OnChangeOfVisibilityState(InteractableVisibilityState oldState, InteractableVisibilityState newState) { }
        void OnChangeOfHoveringState(InteractableHoveringState oldState, InteractableHoveringState newState) { }
        void OnChangeOfDistanceState(InteractableDistanceState oldState, InteractableDistanceState newState) { }
    }
}