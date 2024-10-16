﻿/*
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


using inetum.unityUtils;
using System;
using System.Collections;

using umi3d.common;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.cdk
{
    /// <summary>
    /// Loader for <see cref="UICanvasDto"/>.
    /// </summary>
    public class UMI3DUICanvasNodeLoader
    {
        /// <summary>
        /// Load a Canvas Node.
        /// </summary>
        /// <param name="dto">dto.</param>
        /// <param name="node">gameObject on which the Canvas Node will be loaded.</param>
        /// <param name="finished">Finish callback.</param>
        /// <param name="failed">error callback.</param>
        public void ReadUMI3DExtension(UICanvasDto dto, GameObject node)
        {
            CanvasScaler canvasScaler = node.GetOrAddComponent<CanvasScaler>();
            canvasScaler.dynamicPixelsPerUnit = dto.dynamicPixelsPerUnit;
            canvasScaler.referencePixelsPerUnit = dto.referencePixelsPerUnit;
            Canvas canvas = node.GetOrAddComponent<Canvas>();
            canvas.overrideSorting = dto.orderInLayer != 0; // having a sorting order different from 0 require to activate overriding
            canvas.sortingOrder = dto.orderInLayer;
            
            // overrideSorting property cannot be modified if object is disabled, need to update at each activation.
            ActivationEventListener canvasListener = node.GetOrAddComponent<ActivationEventListener>();
            canvasListener.OnEnabled += () => CoroutineManager.Instance.AttachCoroutine(WaitAndSetOrder(dto, node));
        }

        /// <summary>
        /// Update a property.
        /// </summary>
        /// <param name="entity">entity to be updated.</param>
        /// <param name="property">property containing the new value.</param>
        /// <returns></returns>
        public bool SetUMI3DPorperty(UICanvasDto dto, UMI3DNodeInstance node, SetEntityPropertyDto property)
        {
            switch (property.property)
            {
                case UMI3DPropertyKeys.DynamicPixelsPerUnit:
                    {
                        CanvasScaler canvasScaler = node.GameObject.GetOrAddComponent<CanvasScaler>();
                        canvasScaler.dynamicPixelsPerUnit = dto.dynamicPixelsPerUnit = (float)(Double)property.value;
                    }
                    break;
                case UMI3DPropertyKeys.ReferencePixelsPerUnit:
                    {
                        CanvasScaler canvasScaler = node.GameObject.GetOrAddComponent<CanvasScaler>();
                        canvasScaler.referencePixelsPerUnit = dto.referencePixelsPerUnit = (float)(Double)property.value;
                    }
                    break;
                case UMI3DPropertyKeys.OrderInLayer:
                    {
                        Canvas canvas = node.GameObject.GetOrAddComponent<Canvas>();
                        canvas.sortingOrder = dto.orderInLayer = (int)(Int64)property.value;
                        canvas.overrideSorting = dto.orderInLayer != 0;
                        break;
                    }

                default:
                    return false;
            }
            return true;
        }

        private IEnumerator WaitAndSetOrder(UICanvasDto dto, GameObject node) // unity lock those properties on the frame during which the object is activated
        {
            yield return null;
            Canvas canvas = node.GetOrAddComponent<Canvas>();
            canvas.overrideSorting = dto.orderInLayer != 0; // having a sorting order different from 0 require to activate overriding
            canvas.sortingOrder = dto.orderInLayer;
        }

        public bool SetUMI3DPorperty(UICanvasDto dto, UMI3DNodeInstance node, uint operationId, uint propertyKey, ByteContainer container)
        {
            switch (propertyKey)
            {
                case UMI3DPropertyKeys.DynamicPixelsPerUnit:
                    {
                        CanvasScaler canvasScaler = node.GameObject.GetOrAddComponent<CanvasScaler>();
                        canvasScaler.dynamicPixelsPerUnit = dto.dynamicPixelsPerUnit = UMI3DSerializer.Read<float>(container);
                    }
                    break;
                case UMI3DPropertyKeys.ReferencePixelsPerUnit:
                    {
                        CanvasScaler canvasScaler = node.GameObject.GetOrAddComponent<CanvasScaler>();
                        canvasScaler.referencePixelsPerUnit = dto.referencePixelsPerUnit = UMI3DSerializer.Read<float>(container);
                    }
                    break;
                case UMI3DPropertyKeys.OrderInLayer:
                    {
                        Canvas canvas = node.GameObject.GetOrAddComponent<Canvas>();
                        canvas.sortingOrder = dto.orderInLayer = UMI3DSerializer.Read<int>(container);
                        canvas.overrideSorting = dto.orderInLayer != 0;
                        break;
                    }
                default:
                    return false;
            }
            return true;
        }
    }
}