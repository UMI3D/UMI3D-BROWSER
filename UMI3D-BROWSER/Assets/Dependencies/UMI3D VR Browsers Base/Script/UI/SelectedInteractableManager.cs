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
using inetum.unityUtils.async;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using umi3d.browserRuntime.player;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using umi3dBrowsers.linker;
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
        [SerializeField] private SIM_Linker simLinker;

        public TMP_Text label;
        public Image background;
        public new BoxCollider collider;

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
        public Task<Transform> playerTransform;
        [HideInInspector]
        public Task<Transform> cameraTransform;

        Coroutine hideCoroutine;

        private void Start()
        {
            var player = Global.GetAsync<UMI3DVRPlayer>();
            playerTransform = player.ContinueWith(task =>
            {
                return task.Result.transform;
            });
            cameraTransform = player.ContinueWith(task =>
            {
                return task.Result.mainCamera.transform;
            });

            Hide();

            simLinker.SimReady(this);
        }

        private void Update()
        {
            if (interactable == null && gameObject.activeInHierarchy)
            {
                Hide();
            }

            if (!cameraTransform.TryGet(out Transform ct))
            {
                return;
            }
            if (isActiveAndEnabled)
            {
                var distance = Vector3.Distance(
                    transform.position,
                    ct.position
                );
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
        public void Display(Interactable interactable, Vector3 position)
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
                    if (_label == "new tool")
                    {
                        _label = "";
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

            if (string.IsNullOrEmpty(label.text))
            {
                background.enabled = false;
            }

            transform.position = position;
            cameraTransform.IfCompleted(ct =>
            {
                var offset = transform.position - ct.position;
                transform.LookAt(transform.position + offset);
            });
        }

        /// <summary>
        /// Displays the interactable information.<br/> 
        /// Use an interactable container and a look at point and compute the adequate position.
        /// </summary>
        /// <param name="container"></param>
        public void Display(InteractableContainer container)
        {
            if (!playerTransform.TryGet(out Transform pt))
            {
                return;
            }

            // Convex meshCollider :
            if (container.TryGetComponent(out MeshCollider meshCollider) && meshCollider.convex)
            {
                Display(
                    container.Interactable,
                    meshCollider.ClosestPoint(pt.position)
                );
                return;
            }

            Ray ray = new Ray(pt.position, container.transform.position - pt.position);
            (RaycastHit[] hits, int hitCount) = umi3d.common.Physics.RaycastAll(ray);

            // if the center of the object is outside of the mesh :
            if (hitCount == 0)
            {
                Display(
                    container.Interactable,
                    container.transform.position
                );
                return;
            }

            bool CollideWith(Collider collider, out IEnumerable<RaycastHit> collidedHits)
            {
                hits = hits.SubArray(0, hitCount);
                collidedHits = hits.Where(x => x.collider == collider);
                return collidedHits.Count() != 0;
            };
            if (container.TryGetComponent(out Collider collider) 
                && CollideWith(collider, out IEnumerable<RaycastHit> collidedHits))
            {
                float closestDist = collidedHits.Min(x => x.distance);
                RaycastHit closest = Array.Find(hits, x => x.distance == closestDist);
                Display(container.Interactable, closest.point);
                return;
            }

            Display(container.Interactable, container.transform.position);
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

            IEnumerator HideCoroutine()
            {
                while (isSelected)
                {
                    yield return null;
                }

                // Wait one frame so that text field "show keyboard" action works.
                yield return null;

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

