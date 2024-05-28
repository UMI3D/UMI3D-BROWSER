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
using Unity.VisualScripting;
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
        [SerializeField] private SimpleButton buttonLeft;
        [SerializeField] private Image leftImage;
        [SerializeField] private SimpleButton buttonRight;
        [SerializeField] private Image rightImage;
        [SerializeField] private float scrollButtonSpeed = 1.0f;

        [Header("Navigation")]
        [Header("Activated Navigation Color")]
        [SerializeField] private ColorBlock enabledNavigationColor;
        [Header("Deactivated Navigation Color")]
        [SerializeField] private ColorBlock disableNavigationColor;

        public enum VignetteScale
        {
            None = 0,
            Large = 2,
            Small = 8
        }

        [SerializeField] private VignetteScale vignetteMode;
        [SerializeField] private VignetteContainerEvent vignetteContainerEvent;

        private void Awake()
        {
            vignetteContainerEvent.OnVignetteReset += ResetVignettes;
            vignetteContainerEvent.OnVignetteChangeMode += ChangeVignetteMode;
        }

        private void Start()
        {           
            scrollbar.value = 0;

            buttonLeft.OnClick.AddListener(() => {
                if (vignetteDisplayers.Count > (int)vignetteMode)
                    scrollbar.value -= scrollButtonSpeed / vignetteDisplayers.Count;
            });
            buttonRight.OnClick.AddListener(() => {
                if(vignetteDisplayers.Count > (int)vignetteMode)
                    scrollbar.value += scrollButtonSpeed / vignetteDisplayers.Count;
            });

            vignetteContainerEvent.OnVignetteReset?.Invoke();
        }

        private void OnDestroy()
        {
            vignetteContainerEvent.OnVignetteReset -= ResetVignettes;
            vignetteContainerEvent.OnVignetteChangeMode -= ChangeVignetteMode;
        }

        [ContextMenu("Toggle vignette mode")]
        public void ToggleVignette()
        {
            if (vignetteMode == VignetteScale.None) return;

            if (vignetteMode == VignetteScale.Large)
                vignetteContainerEvent.OnVignetteChangeMode?.Invoke(VignetteScale.Small);
            else if (vignetteMode == VignetteScale.Small)
                vignetteContainerEvent.OnVignetteChangeMode?.Invoke(VignetteScale.Large);
        }

        public void ChangeVignetteMode(VignetteScale mode)
        {
            vignetteMode = mode;
            switch (mode)
            {
                case VignetteScale.None:
                    break;
                case VignetteScale.Large:
                    SetGridLayout(largeVignetteSize, largeVignetteRowAmount, largeVignetteSpace);
                    UpdateNavigation();
                    break;
                case VignetteScale.Small:
                    SetGridLayout(smallVignetteSize, smallVignetteRowAmount, smallVignetteSpace);
                    UpdateNavigation();
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
            var vignette = Instantiate(vignetteMode == VignetteScale.Small ? smallVignettePrefab : vignettePrefab, gridLayout.transform).GetComponent<VignetteDisplayer>();
            vignette.SetupDisplay(pWorldData.worldName);
            vignette.SetupFavoriteButton(() => { 
                pVirtualWorlds.ToggleWorldFavorite(pWorldData);
                vignetteContainerEvent.OnVignetteReset?.Invoke(); 
            }, pWorldData.isFavorite);
            vignette.SetupRemoveButton(() => {
                popupManager.SetArguments(PopupManager.PopupType.Warning, new() { { "worldName", pWorldData.worldName } });
                popupManager.ShowPopup(PopupManager.PopupType.Warning, "empty", "popup_deleteWorld_description",
                    ("popup_cancel", () => popupManager.ClosePopUp()),
                    ("popup_yes", () => {
                        pVirtualWorlds.RemoveWorld(pWorldData);
                        vignetteContainerEvent.OnVignetteReset?.Invoke();
                        popupManager.ClosePopUp();
                    })
                );
            });
            vignette.SetupRenameButton(newName => { 
                pWorldData.worldName = newName; 
                pVirtualWorlds.UpdateWorld(pWorldData);
                vignetteContainerEvent.OnVignetteReset?.Invoke(); 
            });
            vignette.OnClick += () => {
                connectionProcessorService.TryConnectToMediaServer(pWorldData.worldUrl);
            };

            return vignette;
        }

        public void ResetVignettes() => ResetVignettes(true);

        public void ResetVignettes(bool runtime)
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
            UpdateNavigation();
        }

        private void FillWithEmptyVignettes()
        {
            var nbrVignetteTotal = vignetteMode == VignetteScale.Small ? 8 : 2;
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

        IEnumerator ScaleColliders()
        {
            yield return new WaitForSeconds(0.35f);
            scaller.ScaleColliders();
        }

        private void UpdateNavigation()
        {
            if ((int)vignetteMode < vignetteDisplayers.Count) // cyan
            {
                buttonLeft.interactable = true;
                buttonRight.interactable = true;
                rightImage.color = enabledNavigationColor.normalColor;
                leftImage.color = enabledNavigationColor.normalColor;
            }
            else // grey
            {
                buttonLeft.interactable = false;
                buttonRight.interactable = false;
                rightImage.color = disableNavigationColor.normalColor;
                leftImage.color = disableNavigationColor.normalColor;
            }
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

