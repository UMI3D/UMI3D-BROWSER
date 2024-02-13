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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3dBrowsers.displayer;
using umi3dBrowsers.utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.container
{
    public class VignetteContainer : MonoBehaviour
    {
        [Header("Vignette")]
        [SerializeField] private UIColliderScallerHandler scaller;
        [Space]
        [SerializeField] private GameObject vignettePrefab;
        [SerializeField] private GameObject emptyVignettePrefab;
        [SerializeField] private List<VignetteDisplayer> vignetteDisplayers;

        [Header("Sizes")]
        [SerializeField] private Vector2 largeVignetteSize;
        [SerializeField] private Vector2 largeVignetteSpace;
        [Space]
        [SerializeField] private Vector2 smallVignetteSize;
        [SerializeField] private Vector2 smallVignetteSpace;

        [Header("Layout")]
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private int largeVignetteRowAmount;
        [SerializeField] private int smallVignetteRowAmount;

        [Header("Scrolling")]
        [SerializeField] private Scrollbar scrollbar;

        public enum VignetteMode
        {
            None = 0,
            Large = 1,
            Small = 2
        }

        [SerializeField] private VignetteMode vignetteMode;

        private void Awake()
        {
            vignetteDisplayers = GetComponentsInChildren<VignetteDisplayer>().ToList();
            scrollbar.value = 0;
        }

        [ContextMenu("Toggle vignette mode")]
        public void ToggleVignette()
        {
            if (vignetteMode == VignetteMode.None) return;

            if (vignetteMode == VignetteMode.Large) 
                ChangeVignetteMode(VignetteMode.Small);
            else if (vignetteMode == VignetteMode.Small) 
                ChangeVignetteMode(VignetteMode.Large);
        }

        public void ChangeVignetteMode(VignetteMode mode)
        {
            vignetteMode = mode;
            switch (mode)
            {
                case VignetteMode.None:
                    break;
                case VignetteMode.Large:
                    SetGridLayout(largeVignetteSize, largeVignetteRowAmount, largeVignetteSpace);
                    break;
                case VignetteMode.Small:
                    SetGridLayout(smallVignetteSize, smallVignetteRowAmount, smallVignetteSpace);
                    break;
            }
        }

        private void SetGridLayout(Vector2 vignetteSize, int vignetteRowAmount, Vector2 vignetteSpace)
        {
            gridLayout.cellSize = vignetteSize;
            gridLayout.constraintCount = vignetteRowAmount;
            gridLayout.spacing = vignetteSpace;

            scrollbar.value = 0;
            if (gameObject.activeSelf && isActiveAndEnabled)
                StartCoroutine(ScaleColliders());
        }

        private void OnValidate()
        {
            switch (vignetteMode)
            {
                case VignetteMode.None:
                    break;
                case VignetteMode.Large:
                    SetGridLayout(largeVignetteSize, largeVignetteRowAmount, largeVignetteSpace);
                    break;
                case VignetteMode.Small:
                    SetGridLayout(smallVignetteSize, smallVignetteRowAmount, smallVignetteSpace);
                    break;
            }
        }

        IEnumerator ScaleColliders()
        {
            yield return new WaitForSeconds(0.35f);
            scaller.ScaleColliders();
        }
    }
}

