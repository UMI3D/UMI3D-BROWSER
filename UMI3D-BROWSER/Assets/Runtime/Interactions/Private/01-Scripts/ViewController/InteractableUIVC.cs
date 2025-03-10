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

using umi3d.cdk.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.interactions
{
    internal class InteractableUIVC : MonoBehaviour, IInteractableUIDataDelegate
    {
        // The scale of the entire interactable UI.
        const float scale = 0.0005f;

        [HideInInspector] public new Renderer renderer;
        [HideInInspector] public Interactable interactable;
        [HideInInspector] public InteractableUIModel model;

        [SerializeField] int resetFrameRate = 120;
        int _resetFrameRateCount = 0;

        [Space]
        [SerializeField, Tooltip("The offset between the renderer and the UI.")] float offsetWithRenderer;
        [SerializeField, Tooltip("The distance at which the feedback is not visible.")] float farDistanceOffset;

        [Header("Views")]
        [SerializeField] NameView nameView;
        [SerializeField] FeedbackView feedbackView;

        Camera _camera;

        #region IInteractableUIDataDelegate

        public float distanceCameraRenderer => Vector3.Distance(_camera.transform.position, renderer.transform.position);
        public Vector3 directionRendererCamera => (_camera.transform.position - renderer.transform.position).normalized;
        public float interactionDistance => interactable?.InteractionDistance ?? 0;

        #endregion

        #region Life cycle

        void Awake()
        {
            enabled = false;

            model = new();
            model.offsetWithRenderer = offsetWithRenderer;
            model.farDistanceOffset = farDistanceOffset;
            model.dataDelegate = this;

            feedbackView.SetModel(model);

            DisplayName(false);
        }

        void Start()
        {
            _camera = Camera.main;
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

        #endregion

        #region Position & Scale

        void ResetScale()
        {
            Transform parent = transform.parent;
            if (parent == null)
            {
                transform.localScale = Vector3.one * scale;
                return;
            }

            transform.localScale = model.GetLocalScale(
                scale, 
                parent.localScale, 
                parent.lossyScale
            );
        }

        void SetInFrontOfInteractable(Renderer renderer)
        {
            if (_camera == null) { return; }

            Vector3 direction = (_camera.transform.position - renderer.transform.position).normalized;

            transform.position = renderer.transform.position + model.GetFrontPositionOffset(renderer.bounds.size);
        }

        void LookAtTheCamera()
        {
            transform.LookAt(_camera.transform);
            transform.forward = _camera.transform.forward;
        }

        #endregion

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

        public void DisplayName(bool display)
        {
            nameView.gameObject.SetActive(display);
        }

        public void SetInteractableName(string name)
        {
            nameView.SetText(name);
            // TODO: Set name in interaction view.
        }
    }
}