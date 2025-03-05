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
    internal class InteractableUIVC : MonoBehaviour
    {
        // The scale of the entire interactable UI.
        const float scale = 0.0005f;

        [HideInInspector] public new Renderer renderer;
        [HideInInspector] public new Interactable interactable;

        [SerializeField] int resetFrameRate = 120;
        int _resetFrameRateCount = 0;
        [SerializeField] float offsetWithRenderer;

        [Header("Views")]
        [SerializeField] NameView nameView;
        [SerializeField] FeedbackView feedbackView;

        Camera _camera;

        #region Life cycle

        void Awake()
        {
            enabled = false;
            DisplayFeedback(false);
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

            if (interactable.InteractionDistance > Vector3.Distance(_camera.transform.position, renderer.transform.position))
            {
                DisplayName(true);
            }
            else
            {
                DisplayName(false);
            }
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

            float _scaleX = scale * parent.localScale.x / parent.lossyScale.x;
            float _scaleY = scale * parent.localScale.y / parent.lossyScale.y;
            float _scaleZ = scale * parent.localScale.z / parent.lossyScale.z;

            transform.localScale = new(_scaleX, _scaleY, _scaleZ);
        }

        void SetInFrontOfInteractable(Renderer renderer)
        {
            if (_camera == null) { return; }

            Vector3 size = renderer.bounds.size;
            float length = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
            Vector3 direction = (_camera.transform.position - renderer.transform.position).normalized;
            transform.position = renderer.transform.position + direction * (length / 2 + offsetWithRenderer);
        }

        void LookAtTheCamera()
        {
            transform.LookAt(_camera.transform);
            transform.forward = _camera.transform.forward;
        }

        #endregion

        public void DisplayFeedback(bool display)
        {
            feedbackView.gameObject.SetActive(display);
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