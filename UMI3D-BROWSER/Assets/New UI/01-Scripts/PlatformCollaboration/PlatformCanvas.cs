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

using UnityEngine;

namespace umi3dBrowsers.platform
{
    public class PlatformCanvas : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        [Header("WorldSpace Spec")]
        [SerializeField] private Vector3 worldSpaceAnchoredPosition;
        [SerializeField] private Vector2 worldSpaceSizeDelta;
        [SerializeField] private Vector3 worldSpaceLocalScale;

        private void Awake()
        {
            ApplyPlatformSpecific();
        }

        [ContextMenu("Apply")]
        private void ApplyPlatformSpecific()
        {
#if UMI3D_PC
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
#elif UMI3D_XR
            canvas.renderMode = RenderMode.WorldSpace;
            var rectTransform = canvas.transform as RectTransform;
            rectTransform.localScale = worldSpaceLocalScale;
            rectTransform.sizeDelta = worldSpaceSizeDelta;
            rectTransform.anchoredPosition3D = worldSpaceAnchoredPosition;
#endif
        }
    }
}
