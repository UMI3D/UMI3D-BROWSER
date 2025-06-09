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


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace umi3d
{
    public class PlayerRayColorUIOverride : MonoBehaviour
    {
        [SerializeField] private XRInteractorLineVisual interactorLineVisual;

        private Gradient _baseColor;
        private Gradient _interactableColor;


        private void Awake()
        {
            _baseColor = interactorLineVisual.invalidColorGradient;
            _interactableColor = interactorLineVisual.validColorGradient;
        }

        public void OnHoverEntered(UIHoverEventArgs args)
        {
            if (args.uiObject.GetComponent<Selectable>() != null || args.uiObject.GetComponentInParent<Selectable>() != null)
                interactorLineVisual.validColorGradient = _interactableColor ;
            else
                interactorLineVisual.validColorGradient = _baseColor;
        }

        public void OnHoverExit(UIHoverEventArgs args)
        {
            interactorLineVisual.validColorGradient = _interactableColor;
        }
    }
}