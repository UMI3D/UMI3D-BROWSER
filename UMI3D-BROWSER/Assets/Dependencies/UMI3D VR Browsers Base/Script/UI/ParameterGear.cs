/*
Copyright 2019 - 2022 Inetum

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
using System.Linq;
using TMPro;
using umi3d.cdk.interaction;
using umi3dVRBrowsersBase.interactions;
using umi3dVRBrowsersBase.ui.playerMenu;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;

namespace umi3dVRBrowsersBase.ui
{
    /// <summary>
    /// 3D Object to show users its associated UMI3D Entity has UMI3D Parameters, by clicking on this gear, opens <see cref="playerMenu.PlayerMenuManager"/> to edit them.
    /// </summary>
    public class ParameterGear : AbstractClientInteractableElement, ITriggerableElement
    {
        #region Fields

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public UnityEvent OnTriggered { get; private set; } = new UnityEvent();

        /// <summary>
        /// <see cref="Interactable"/> which contains the parameters.
        /// </summary>
        private Interactable currentAssociatedInteractable;
        public Interactable CurrentAssociatedInteractable
        {
            get => currentAssociatedInteractable;
            set
            {
                currentAssociatedInteractable = value;

                if (value == null)
                {
                    container = null;
                }
                else
                {
                    container = InteractableContainer.containers.Find(c => c.Interactable == currentAssociatedInteractable);
                    Debug.Assert(container != null, "No container found for an Interactable, should not happen.");
                }
            }
        }

        /// <summary>
        /// Container associated to <see cref="CurrentAssociatedInteractable"/>.
        /// </summary>
        private InteractableContainer container;

        public TMP_Text label;

        [SerializeField]
        [Tooltip("player needed for distance.")]
        private Transform player;


        public float delayBeforeHiding = 1f;
        [Tooltip("Used for gear scale.")]
        public float minDistance = 2f;
        [Tooltip("Used for gear scale.")]
        public float maxDistance = 6f;
        [Tooltip("Used for gear scale.")]
        public float minScale = 0.15f;
        [Tooltip("Used for gear scale.")]
        public float maxScale = .5f;

        #endregion

        #region Methods

        private void Start()
        {
            PlayerMenuManager.Instance.onMenuClose.AddListener(Hide);

            Hide();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="controller"></param>
        public void Trigger(ControllerType controllerType)
        {
            OnTriggered?.Invoke();

            PlayerMenuManager.Instance.OpenParameterMenu(controllerType, menuAsync: true);
        }

        /// <summary>
        /// Displays the parameter gear for an interactable.
        /// </summary>
        /// <param name="interactable"></param>
        /// <param name="position">World position of the gear</param>
        /// <param name="normal">World normal of the gear</param>
        public void Display(Interactable interactable, Vector3 position, Vector3 normal)
        {
            gameObject.SetActive(true);

            CurrentAssociatedInteractable = interactable;

            transform.position = position;
            transform.rotation = Quaternion.LookRotation(-normal, Vector3.up);

            label.text = interactable.name;
            UnityEngine.Debug.Log($"[PG] name = {interactable.name}, {interactable.dto.name}, {interactable.description}");
        }

        /// <summary>
        /// Displays the parameter gear for an interactable. Use an interactable container and a look at point and compute the adequate position.
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
        /// Hides the parameter gear.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            CurrentAssociatedInteractable = null;
        }


        public void HideWithDelay()
        {
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
            }

            CoroutineManager.Instance.AttachCoroutine(HideCoroutine());
        }

        public override void Select(VRController controller)
        {
            isSelected = true;
        }

        public override void Deselect(VRController controller)
        {
            isSelected = false;
        }

        private void Update()
        {
            if (container == null && gameObject.activeInHierarchy)
            {
                Hide();
            }

            if (isActiveAndEnabled)
            {
                var distance = Vector3.Distance(transform.position, player.position);
                var scale = Mathf.Lerp(minScale, maxScale, Mathf.InverseLerp(minDistance, maxDistance, distance));
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        #endregion
    }
}

