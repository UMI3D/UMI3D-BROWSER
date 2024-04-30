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
using System.Security.Policy;
using umi3dBrowsers.displayer;
using umi3dBrowsers.services.connection;
using umi3dBrowsers.utils;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.container
{
    public class VignetteContainer : MonoBehaviour
    {
        [SerializeField] private ConnectionProcessor connectionProcessorService;
        [SerializeField] private PopupManager popupManager;
        [Header("Vignette")]
        [SerializeField] private UIColliderScallerHandler scaller;
        [Space]
        [SerializeField] private GameObject vignettePrefab;
        [SerializeField] private GameObject emptyVignettePrefab;
        [SerializeField] private GameObject smallVignettePrefab;
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
        public event Action<VignetteMode> OnChangeMode;

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
                OnChangeMode?.Invoke(VignetteMode.Small);
            else if (vignetteMode == VignetteMode.Small)
                OnChangeMode?.Invoke(VignetteMode.Large);
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
            var vignette = Instantiate(vignetteMode == VignetteMode.Small ? smallVignettePrefab : vignettePrefab, gridLayout.transform).GetComponent<VignetteDisplayer>();
            vignette.SetupDisplay(pWorldData.worldName);
            vignette.SetupFavoriteButton(() => { 
                pVirtualWorlds.ToggleWorldFavorite(pWorldData); 
                OnReset?.Invoke(); 
            });
            vignette.SetupRemoveButton(() => {
                popupManager.SetArguments(PopupManager.PopupType.Warning, new() { { "worldName", pWorldData.worldName } });
                popupManager.ShowPopup(PopupManager.PopupType.Warning, "empty", "popup_deleteWorld_description",
                    ("popup_cancel", () => popupManager.ClosePopUp()),
                    ("popup_yes", () => {
                        pVirtualWorlds.RemoveWorld(pWorldData);
                        OnReset?.Invoke();
                        popupManager.ClosePopUp();
                    })
                );
            });
            vignette.SetupRenameButton(newName => { 
                pWorldData.worldName = newName; 
                pVirtualWorlds.UpdateWorld(pWorldData);
                OnReset?.Invoke(); 
            });
            vignette.OnClick += () => {
                connectionProcessorService.TryConnectToMediaServer(pWorldData.worldUrl);
            };

            return vignette;
        }

        public void ResetVignettes(bool runtime = true)
        {
            for (var i = gridLayout.transform.childCount - 1; i >= 0; i--)
            {
                if (runtime)
                    Destroy(gridLayout.transform.GetChild(i).gameObject);
                else
                    DestroyImmediate(gridLayout.transform.GetChild(i).gameObject);
            }

                vignetteDisplayers = PlayerPrefsManager.HasVirtualWorldsStored()
                    ? CreateVignettes(PlayerPrefsManager.GetVirtualWorlds())
                    : new();

            FillWithEmptyVignettes();
        }

        private void FillWithEmptyVignettes()
        {
            var nbrVignetteTotal = vignetteMode == VignetteMode.Small ? 8 : 2;
            for (var i = vignetteDisplayers.Count - nbrVignetteTotal; i < 0; i++)
                Instantiate(emptyVignettePrefab, gridLayout.transform);
        }

        private void SetGridLayout(Vector2 vignetteSize, int vignetteRowAmount, Vector2 vignetteSpace, bool runtime = true)
        {
            gridLayout.cellSize = vignetteSize;
            gridLayout.constraintCount = vignetteRowAmount;
            gridLayout.spacing = vignetteSpace;

            scrollbar.value = 0;
            if (gameObject.activeSelf && isActiveAndEnabled)
                StartCoroutine(ScaleColliders());

            ResetVignettes(runtime);
        }

        private void OnValidate()
        {
            //switch (vignetteMode)
            //{
            //    case VignetteMode.None:
            //        break;
            //    case VignetteMode.Large:
            //        SetGridLayout(largeVignetteSize, largeVignetteRowAmount, largeVignetteSpace, false);
            //        break;
            //    case VignetteMode.Small:
            //        SetGridLayout(smallVignetteSize, smallVignetteRowAmount, smallVignetteSpace, false);
            //        break;
            //}
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

