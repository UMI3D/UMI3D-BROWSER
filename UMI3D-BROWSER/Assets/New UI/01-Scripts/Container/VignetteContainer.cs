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
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3dBrowsers.displayer;
using umi3dBrowsers.services.connection;
using umi3dBrowsers.utils;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
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
        [SerializeField] private bool isFavorite;
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

        public event Action OnReset;

        private void Awake()
        {
            ResetVignettes();
        }

        private void Start()
        {           
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

        private List<VignetteDisplayer> CreateVignettes(VirtualWorlds pVirtualWorlds)
        {
            var lstVignettes = new List<VignetteDisplayer>();

            var lstWorldDatas = isFavorite ? pVirtualWorlds.FavoriteWorlds : pVirtualWorlds.worlds;
            var lstWorldDatasOrdered = lstWorldDatas.OrderBy(w => new DateTime(w.dateLastConnection)).Reverse().ToList();
            foreach (var worldData in lstWorldDatasOrdered)
                lstVignettes.Add(CreateVignette(pVirtualWorlds, worldData));

            return lstVignettes;
        }

        private VignetteDisplayer CreateVignette(VirtualWorlds pVirtualWorlds, VirtualWorldData pWorldData)
        {
            var vignette = Instantiate(vignettePrefab, gridLayout.transform).GetComponent<VignetteDisplayer>();
            vignette.SetupDisplay(pWorldData.worldName);
            vignette.SetupFavoriteButton(() => { pVirtualWorlds.ToggleWorldFavorite(pWorldData); OnReset?.Invoke(); });
            vignette.SetupRemoveButton(() => { pVirtualWorlds.RemoveWorld(pWorldData); OnReset?.Invoke(); });

            return vignette;
        }

        public void ResetVignettes()
        {
            for (var i = gridLayout.transform.childCount - 1; i >= 0; i--)
                Destroy(gridLayout.transform.GetChild(i).gameObject);

            vignetteDisplayers = PlayerPrefsManager.HasVirtualWorldsStored()
                    ? CreateVignettes(PlayerPrefsManager.GetVirtualWorlds())
                    : new();
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

#if UNITY_EDITOR
        [SerializeField] private VirtualWorlds virtualWorlds;

        [Button("CreateVirtualWorldsDataTest")]
        public void ReplaceVirtualWorldsDataTest()
        {
            PlayerPrefsManager.SaveVirtualWorld(virtualWorlds);
        }
#endif
    }
}

