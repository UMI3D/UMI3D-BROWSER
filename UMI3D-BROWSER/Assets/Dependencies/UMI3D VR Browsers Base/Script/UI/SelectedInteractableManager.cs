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
using umi3d.browserRuntime.ui.keyboard;
using umi3d.cdk;
using umi3d.cdk.interaction;
using umi3d.cdk.notification;
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
        public event Action triggerHandler;

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

        Canvas canvas;

        Request playerRequest;

        [HideInInspector]
        public Interactable interactable;
        [HideInInspector]
        public List<AbstractInteractionDto> interactions;
        [HideInInspector]
        public List<EventDto> events;
        [HideInInspector]
        public List<AbstractParameterDto> parameters;

        Coroutine hideCoroutine;

        ulong toolID;
        StringParameterDto stringParameter;
        KeyboardStringInteractionLinker keyboardLinker;
        ControllerType controllerType;
        Request leftTrackerRequest;
        Request rightTrackerRequest;

        private void Awake()
        {
            keyboardLinker = GetComponent<KeyboardStringInteractionLinker>();

            canvas = GetComponentInChildren<Canvas>();

            playerRequest = RequestHub.Default
                .SubscribeAsClient<UMI3DClientRequestKeys.PlayerRequest>(this);

            leftTrackerRequest = RequestHub.Default
                .SubscribeAsClient<UMI3DClientRequestKeys.LeftTrackerRequest>(this);

            rightTrackerRequest = RequestHub.Default
                .SubscribeAsClient<UMI3DClientRequestKeys.RightTrackerRequest>(this);
        }

        private void Start()
        {
            Hide();

            simLinker.SimReady(this);
        }

        void OnEnable()
        {
            keyboardLinker.enterOrSubmit += KeyboardLinker_enterOrSubmit;
        }

        void OnDisable()
        {
            keyboardLinker.enterOrSubmit -= KeyboardLinker_enterOrSubmit;
        }

        void KeyboardLinker_enterOrSubmit(string text)
        {
            uint boneType = 0;
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            if (TryGetTrackerRequest(out Request request))
            {
                request.TryGetInfoT(UMI3DClientRequestKeys.TrackerRequest.BoneType, out boneType);
                request.TryGetInfoT(UMI3DClientRequestKeys.TrackerRequest.Position, out position);
                request.TryGetInfoT(UMI3DClientRequestKeys.TrackerRequest.Rotation, out rotation);
            }

            UnityEngine.Debug.Log($"keyboard enter with text: [{text}]");
            UMI3DClientServer.SendRequest(new ParameterSettingRequestDto()
            {
                boneType = boneType,
                bonePosition = position.Dto(),
                boneRotation = rotation.Dto(),
                toolId = toolID,
                parameter = new StringParameterDto()
                {
                    id = stringParameter.id,
                    description = stringParameter.description,
                    name = stringParameter.name,
                    icon2D = stringParameter.icon2D,
                    icon3D = stringParameter.icon3D,
                    value = text
                },
                id = stringParameter.id,
            }, true);
        }

        bool TryGetTrackerRequest(out Request request)
        {
            switch (controllerType)
            {
                case ControllerType.LeftHandController:
                    request = leftTrackerRequest;
                    return true;

                case ControllerType.RightHandController:
                    request = rightTrackerRequest;
                    return true;

                default:
                    UnityEngine.Debug.LogError($"Unhandled case.");
                    request = null;
                    return false;
            }
        }

        void Update()
        {
            if (interactable == null && gameObject.activeInHierarchy)
            {
                Hide();
            }

            if (!playerRequest.TryGetInfoT(UMI3DClientRequestKeys.PlayerRequest.Camera, out Camera camera))
            {
                return;
            }

            if (isActiveAndEnabled)
            {
                var distance = Vector3.Distance(
                    transform.position,
                    camera.transform.position
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

            if (interactions.Count == 1)
            {
                if (interactions[0] is not StringParameterDto stringParameter)
                {
                    return;
                }

                toolID = interactable.id;
                this.stringParameter = stringParameter;
                keyboardLinker.TextFieldSelected(stringParameter.value);
                this.controllerType = controllerType;
            }
            else if (interactions.Count > 0)
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
                CoroutineManager.Instance.DetachCoroutine(hideCoroutine);
                hideCoroutine = null;
            }

            canvas.gameObject.SetActive(true);

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
                    UnityEngine.Debug.LogError($"[Selected Interactable] Unhandled case 1 : {interactions[0].GetType()}");
                }
            }
            else
            {
                label.text = interactable.name;
                PlayerMenuManager.Instance.CtrlToolMenu.RememberParameters();
            }

            if (string.IsNullOrEmpty(label.text))
            {
                background.enabled = false;
            }

            transform.position = position;

            if (!playerRequest.TryGetInfoT(UMI3DClientRequestKeys.PlayerRequest.Camera, out Camera camera))
            {
                return;
            }
            var offset = transform.position - camera.transform.position;
            transform.LookAt(transform.position + offset);
        }

        /// <summary>
        /// Displays the interactable information.<br/> 
        /// Use an interactable container and a look at point and compute the adequate position.
        /// </summary>
        /// <param name="container"></param>
        public void Display(InteractableContainer container)
        {
            if (!playerRequest.TryGetInfoT(UMI3DClientRequestKeys.PlayerRequest.Transform, out Transform playerTransform))
            {
                return;
            }

            // Convex meshCollider :
            if (container.TryGetComponent(out MeshCollider meshCollider) && meshCollider.convex)
            {
                Display(
                    container.Interactable,
                    meshCollider.ClosestPoint(playerTransform.position)
                );
                return;
            }

            Ray ray = new Ray(playerTransform.position, container.transform.position - playerTransform.position);
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
            canvas.gameObject.SetActive(false);
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

