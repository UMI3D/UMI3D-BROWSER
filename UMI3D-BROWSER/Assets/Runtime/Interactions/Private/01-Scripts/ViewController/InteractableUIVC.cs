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

using inetum.unityUtils.ui.canvas;
using System.Collections.ObjectModel;
using System.Linq;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.interactions
{
    internal class InteractableUIVC : MonoBehaviour, IView, IInteractableUIDataDelegate
    {
        // The scale of the entire interactable UI.
        const float scale = 0.0005f;

        Transform scaleResetTransform;

        [HideInInspector] public new Renderer renderer;
        [HideInInspector] public new Collider collider;
        public Interactable interactable { get; private set; }
        public InteractableUIModel model { get; private set; }

        [SerializeField] int resetFrameRate = 120;
        int _resetFrameRateCount = 0;

        [Space]
        [SerializeField, Tooltip("The offset between the renderer and the UI.")] float offsetWithRenderer;
        [SerializeField, Tooltip("The distance at which the feedback is not visible.")] float farDistanceOffset;

        [Header("Views")]
        [SerializeField] NameView nameView;
        InputCollectionVC inputCollectionVC;
        FeedbackView feedbackView;

        Camera _camera;

        IView view => this;

        #region IInteractableUIDataDelegate

        public float distanceCameraRenderer => Vector3.Distance(_camera.transform.position, renderer.transform.position);
        public Vector3 directionRendererCamera => (_camera.transform.position - renderer.bounds.center).normalized;
        public float interactionDistance => interactable?.InteractionDistance ?? 0;

        public string interactableName => interactable?.name ?? "";
        public ReadOnlyCollection<AbstractInteractionDto> interactions => interactable?.interactions.Select(i => i.Result).ToList().AsReadOnly();
        public ReadOnlyCollection<EventDto> events => interactable?.interactions.Select(i => i.Result).ToList().FindAll(i => i is EventDto).Select(i => i as EventDto).ToList().AsReadOnly();

        #endregion

        #region Life cycle

        void Awake()
        {
            enabled = false;

            Debug.Assert(view.Set(ref inputCollectionVC, 1));
            Debug.Assert(view.Set(ref feedbackView, 2));

            model = new();
            model.offsetWithRenderer = offsetWithRenderer;
            model.farDistanceOffset = farDistanceOffset;
            model.dataDelegate = this;
        }

        void Start()
        {
            _camera = Camera.main;

            nameView.SetModel(model);
            inputCollectionVC.SetModel(model);
            feedbackView.SetModel(model);
        }

        void OnEnable()
        {
            _resetFrameRateCount = 0;
            ResetScale();
        }

        void Update()
        {
            if (_resetFrameRateCount < resetFrameRate) 
            { 
                _resetFrameRateCount++;
            }
            else
            {
                _resetFrameRateCount = 0;
                ResetScale();
            }

            SetInFrontOfInteractable(renderer);
            LookAtTheCamera();

            model.UpdateDistanceState();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (model != null)
            {
                model.offsetWithRenderer = offsetWithRenderer;
                model.farDistanceOffset = farDistanceOffset;
            }
        }
#endif

#endregion

        #region Position & Scale

        void ResetScale()
        {
            if (scaleResetTransform == null)
            {
                scaleResetTransform = new GameObject("Scale Reset").GetComponent<Transform>();
            }
            if (transform.parent == null || transform.parent != scaleResetTransform)
            {
                scaleResetTransform.SetParent(transform.parent, false);
                transform.SetParent(scaleResetTransform);
                transform.localScale = Vector3.one * scale;
            }

            Transform parent = scaleResetTransform.parent;
            if (parent == null)
            {
                scaleResetTransform.localScale = Vector3.one * scale;
                return;
            }

            scaleResetTransform.localScale = model.GetLocalScale(
                1, 
                parent.lossyScale
            );
        }

        void SetInFrontOfInteractable(Renderer renderer)
        {
            if (_camera == null) { return; }

            transform.position = renderer.bounds.center + model.GetFrontPositionOffset(renderer.bounds.size);
        }

        void LookAtTheCamera()
        {
            transform.LookAt(_camera.transform);
            transform.forward = _camera.transform.forward;
        }

        #endregion

        public void SetInteractable(Interactable interactable)
        {
            model.delegates.ForEach(@delegate =>
            {
                @delegate.OnChangeOfInteractableName(this.interactable.name, interactable.name);
                return inetum.unityUtils.observation.Flow.Continue;
            });
            this.interactable = interactable;
        }

        public void OnInteractableBecameVisible()
        {
            model.OnBecameVisible();

            enabled = true;
        }

        public void OnInteractableBecameInvisible()
        {
            enabled = false;

            model.OnBecameInvisible();
        }

        public void OnInteractableBecameHovered()
        {
            model.OnBecameHovered();
        }

        public void OnInteractableBecameNotHovered()
        {
            model.OnBecameNotHovered();
        }
    }
}