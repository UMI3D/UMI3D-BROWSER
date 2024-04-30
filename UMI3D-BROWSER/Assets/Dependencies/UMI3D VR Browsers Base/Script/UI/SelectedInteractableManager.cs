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

using inetum.unityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using umi3d.browserRuntime.player;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using umi3dVRBrowsersBase.interactions;
using umi3dVRBrowsersBase.ui.playerMenu;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace umi3dVRBrowsersBase.ui
{
    /// <summary>
    /// Manage the information to of the selected interactable.
    /// </summary>
    public class SelectedInteractableManager : AbstractClientInteractableElement, ITriggerableElement
    {
        public TMP_Text label;
        public Image background;
        public new BoxCollider collider;

        public float delayBeforeHiding = 1f;
        [Tooltip("Used for gear scale.")]
        public float minDistance = 2f;
        [Tooltip("Used for gear scale.")]
        public float maxDistance = 6f;
        [Tooltip("Used for gear scale.")]
        public float minScale = 0.15f;
        [Tooltip("Used for gear scale.")]
        public float maxScale = .5f;

        public event Action triggerHandler;

        [HideInInspector]
        public Interactable interactable;
        [HideInInspector]
        public List<AbstractInteractionDto> interactions;
        [HideInInspector]
        public List<EventDto> events;
        [HideInInspector]
        public List<AbstractParameterDto> parameters;
        [HideInInspector]
        public Transform playerTransform;

        Coroutine hideCoroutine;

        private void Start()
        {
            Global.Get(out UMI3DVRPlayer player);
            playerTransform = player.transform;

            Hide();
        }

        private void Update()
        {
            if (interactable == null && gameObject.activeInHierarchy)
            {
                Hide();
            }

            if (isActiveAndEnabled)
            {
                var distance = Vector3.Distance(transform.position, playerTransform.position);
                var scale = Mathf.Lerp(
                    minScale,
                    maxScale,
                    Mathf.InverseLerp(minDistance, maxDistance, distance)
                );
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        public void Trigger(ControllerType controllerType)
        {
            triggerHandler?.Invoke();
            
            if (interactions.Count > 0)
            {
                PlayerMenuManager.Instance.OpenParameterMenu(controllerType, menuAsync: true);
            }
        }

        /// <summary>
        /// Displays the interactable information.
        /// </summary>
        /// <param name="interactable"></param>
        /// <param name="position">World position of the gear</param>
        /// <param name="normal">World normal of the gear</param>
        public void Display(Interactable interactable, Vector3 position, Vector3 normal)
        {
            Rest();
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
                hideCoroutine = null;
            }

            gameObject.SetActive(true);

            this.interactable = interactable;
            interactions = interactable
                .interactions
                .Select(i => i.Result)
                .ToList();
            events = interactions
                .FindAll(i => i is EventDto)
                .Select(i => i as EventDto)
                .ToList();
            parameters = interactions
                .FindAll(i => i is AbstractParameterDto)
                .Select(i => i as AbstractParameterDto)
                .ToList();

            background.enabled = true;
            if (interactions.Count == 0)
            {
                collider.enabled = false;
                label.text = "";
                background.enabled = false;
            }
            else if (interactions.Count == 1)
            {
                if (interactions[0] is EventDto || interactions[0] is ManipulationDto)
                {
                    
                    string _label = interactions[0].name;
                    if (string.IsNullOrEmpty(_label) || _label == "new tool")
                    {
                        _label = interactable.name;
                    }
                    if (string.IsNullOrEmpty(_label) || _label == "new tool")
                    {
                        _label = "To Trigger";
                    }
                    label.text = _label;
                }
                else if (interactions[0] is StringParameterDto)
                {
                    collider.enabled = true;
                    label.text = $"Edit text";
                    PlayerMenuManager.Instance.CtrlToolMenu.RememberParameters();
                }
                else
                {
                    label.text = interactions[0].name;
                    UnityEngine.Debug.LogError($"[Selected Interactable] Unhandled case");
                }
            }
            else
            {
                label.text = interactable.name;
                PlayerMenuManager.Instance.CtrlToolMenu.RememberParameters();
                UnityEngine.Debug.LogError($"[Selected Interactable] Unhandled case");
            }

            transform.position = position;
            transform.rotation = Quaternion.LookRotation(-normal, Vector3.up);
        }

        /// <summary>
        /// Displays the interactable information.<br/> 
        /// Use an interactable container and a look at point and compute the adequate position.
        /// </summary>
        /// <param name="interactableContainer"></param>
        /// <param name="lookAtPoint">World position of the point the object is looked at.</param>
        public void Display(InteractableContainer interactableContainer, Vector3 lookAtPoint)
        {
            Vector3 rootPosition;
            Vector3 normal;
            if (interactableContainer.TryGetComponent(out MeshCollider collider) && collider.convex)
            {
                rootPosition = collider.ClosestPoint(lookAtPoint);
                normal = -(rootPosition - lookAtPoint).normalized;
            }
            else
            {
                Ray ray = new Ray(lookAtPoint, interactableContainer.transform.position - lookAtPoint);
                (RaycastHit[] hits, int hitCount) hitsInfo = umi3d.common.Physics.RaycastAll(ray);

                if (hitsInfo.hitCount == 0) // happens if the center of the object is outside of the mesh
                {
                    rootPosition = interactableContainer.transform.position;
                    normal = -(rootPosition - lookAtPoint).normalized;
                }
                else
                {
                    //TODO : remove try catch later for a better test
                    try
                    {
                        Collider icCollider = interactableContainer.GetComponentInChildren<Collider>();
                        RaycastHit[] hits = hitsInfo.hits.SubArray(0, hitsInfo.hitCount);
                        float closestDist = hits.Where(x => x.collider == icCollider).Min(x => x.distance);
                        RaycastHit closest = Array.Find(hits, x => x.distance == closestDist);
                        rootPosition = closest.point;
                        normal = closest.normal;
                    }
                    catch
                    {
                        rootPosition = interactableContainer.transform.position;
                        normal = -(rootPosition - lookAtPoint).normalized;
                    }
                }
            }

            Display(interactableContainer.Interactable, rootPosition, normal);
        }

        /// <summary>
        /// Hides the interactable information.
        /// </summary>
        public void Hide()
        {
            Rest();
            gameObject.SetActive(false);
        }

        public void HideWithDelay()
        {
            if (hideCoroutine != null)
            {
                return;
            }

            float time = 0f;
            IEnumerator HideCoroutine()
            {
                while (isSelected)
                {
                    yield return null;
                }

                while (time < delayBeforeHiding)
                {
                    time += Time.deltaTime;
                    yield return null;
                }

                if (!isSelected)
                {
                    Hide();
                }

                hideCoroutine = null;
            }

            hideCoroutine = CoroutineManager.Instance.AttachCoroutine(HideCoroutine());
        }

        public override void Select(VRController controller)
        {
            isSelected = true;
        }

        public override void Deselect(VRController controller)
        {
            isSelected = false;
        }

        public void Rest()
        {
            interactable = null;
            interactions = null;
            events = null;
            parameters = null;
            collider.enabled = false;
        }
    }
}

