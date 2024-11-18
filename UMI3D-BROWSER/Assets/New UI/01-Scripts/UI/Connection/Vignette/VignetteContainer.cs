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
using System.Threading.Tasks;
using umi3d.browserRuntime.notificationKeys;
using umi3d.browserRuntime.ui.connection.vignette;
using umi3d.browserRuntime.ui.popup;
using umi3d.common.interaction.form;
using umi3dBrowsers.displayer;
using umi3dBrowsers.linker;
using umi3dBrowsers.services.connection;
using umi3dBrowsers.utils;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.container
{
    public class VignetteContainer : MonoBehaviour
    {
        [SerializeField] private ConnectionServiceLinker connectionServiceLinker;

        [Header("Vignette")]
        [SerializeField] private UIColliderScallerHandler scaler;
        [Space]
        [SerializeField] private GameObject emptyVignettePrefab;
        [SerializeField] private List<VignetteDisplayer> vignetteDisplayers;
        private List<GameObject> m_emptyVignettes = new();
        private List<VignetteBuffer> m_vignetteBuffers = new();
        [Space]
        [SerializeField] private List<VignetteContainerData> m_vignetteContainerDatas = new();
        [Space]
        [SerializeField] private E_VignetteScale vignetteMode;
        [SerializeField] private E_VignetteScale primaryVignetteMode;
        [SerializeField] private E_VignetteScale secondaryVignetteMode;
        [SerializeField] private VignetteContainerEvent vignetteContainerEvent;

        [Header("Layout")]
        [SerializeField] private bool isFavorite;
        [SerializeField] private GridLayoutGroup gridLayout;

        [Header("Scrolling")]
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private Button buttonLeft;
        [SerializeField] private Image leftImage;
        [SerializeField] private Button buttonRight;
        [SerializeField] private Image rightImage;
        [SerializeField] private float scrollButtonSpeed = 1.0f;

        private bool m_shouldResetAndFetchVignetteFromDB = true;

        const string LOCALIZATION_TABLE = "UMI3D_inetum";
        private PopupNotifier popupNotifier;

        public bool ShouldResetAndFetchVignetteFromDB { get => m_shouldResetAndFetchVignetteFromDB; set => m_shouldResetAndFetchVignetteFromDB = value; }

        private void Awake()
        {
            popupNotifier = new(this);

            vignetteContainerEvent.OnVignetteReset += ResetVignettes;
            vignetteContainerEvent.OnVignetteChangeMode += ChangeVignetteMode;

            buttonLeft.onClick.AddListener(() => {
                if (vignetteDisplayers.Count > (int)vignetteMode)
                    scrollbar.value -= scrollButtonSpeed / (vignetteDisplayers.Count - (int)vignetteMode);
            });
            buttonRight.onClick.AddListener(() => {
                if (vignetteDisplayers.Count > (int)vignetteMode)
                    scrollbar.value += scrollButtonSpeed / (vignetteDisplayers.Count - (int)vignetteMode);
            });
        }

        private void OnEnable()
        {
            new Task(async () => {
                await Task.Yield();
                scrollbar.value = 0;
            }).Start(TaskScheduler.FromCurrentSynchronizationContext());

            vignetteContainerEvent.OnVignetteReset?.Invoke();
        }

        private void Start()
        {
            ChangeVignetteMode(PlayerPrefs.GetInt(VignettePlayerPrefKeys.IsPrimaryVignetteMode, 1) == 1 ? primaryVignetteMode : secondaryVignetteMode);
        }

        private void OnDestroy()
        {
            vignetteContainerEvent.OnVignetteReset -= ResetVignettes;
            vignetteContainerEvent.OnVignetteChangeMode -= ChangeVignetteMode;
        }

        [ContextMenu("Toggle vignette mode")]
        public void ToggleVignette()
        {
            if (vignetteMode == E_VignetteScale.None) return;

            if (vignetteMode == primaryVignetteMode)
                vignetteContainerEvent.OnVignetteChangeMode?.Invoke(secondaryVignetteMode);
            else if (vignetteMode == secondaryVignetteMode)
                vignetteContainerEvent.OnVignetteChangeMode?.Invoke(primaryVignetteMode);
        }

        public void ChangePrimaryVignetteMode(E_VignetteScale vignetteScale)
        {
            primaryVignetteMode = vignetteScale;
            ChangeVignetteMode(PlayerPrefs.GetInt(VignettePlayerPrefKeys.IsPrimaryVignetteMode, 1) == 1 ? primaryVignetteMode : secondaryVignetteMode);
        }

        public void ChangeSecondaryVignetteMode(E_VignetteScale vignetteScale)
        {
            secondaryVignetteMode = vignetteScale;
            ChangeVignetteMode(PlayerPrefs.GetInt(VignettePlayerPrefKeys.IsPrimaryVignetteMode, 1) == 1 ? primaryVignetteMode : secondaryVignetteMode);
        }

        public void ChangeVignetteMode(E_VignetteScale mode)
        {
            vignetteMode = mode;
            SetGridLayout(VignetteContainerData.FindVignetteContainerDataByVignetteScale(vignetteMode, m_vignetteContainerDatas));
            UpdateNavigation();
            FillWithEmptyVignettes();

            PlayerPrefs.SetInt(VignettePlayerPrefKeys.IsPrimaryVignetteMode, vignetteMode == primaryVignetteMode ? 1 : 0);
        }

        public void Clear()
        {
            for (var i  = 0; i < vignetteDisplayers.Count; i++)
            {
                if (vignetteDisplayers[i] == null) continue;
                Destroy(vignetteDisplayers[i].gameObject);
            }
            vignetteDisplayers.Clear();

            foreach (var buffer in m_vignetteBuffers)
            {
                if (buffer.VignetteGo != null)
                    Destroy(buffer.VignetteGo);
            }
            m_vignetteBuffers.Clear();

            UpdateNavigation();
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
            VignetteContainerData data = VignetteContainerData.FindVignetteContainerDataByVignetteScale(vignetteMode, m_vignetteContainerDatas);
            GameObject vignetteGO = Instantiate(data.VignettePrefab);
            vignetteGO.transform.SetParent(gridLayout.transform, false);
            var vignette = vignetteGO.GetComponent<VignetteDisplayer>();
            vignette.SetupDisplay(pWorldData.worldName);
            vignette.SetupFavoriteButton(() => { 
                pVirtualWorlds.ToggleWorldFavorite(pWorldData);
                vignetteContainerEvent.OnVignetteReset?.Invoke(); 
            }, pWorldData.isFavorite);
            vignette.SetupRemoveButton(() => {
                popupNotifier
                    .enqueue
                    .SetType(PopupType.Warning)
                    .SetArguments(("worldName", pWorldData.worldName))
                    .SetDescription(LOCALIZATION_TABLE, "popup_deleteWorld_description")
                    .SetButtons((LOCALIZATION_TABLE, "popup_cancel"), (LOCALIZATION_TABLE, "popup_yes"))
                    .SetButtonsAction(index =>
                    {
                        if (index == 1)
                        {
                            pVirtualWorlds.RemoveWorld(pWorldData);
                            vignetteContainerEvent.OnVignetteReset?.Invoke();
                        }
                    })
                    .Notify();
            });
            vignette.SetupRenameButton(newName => { 
                pWorldData.worldName = newName; 
                pVirtualWorlds.UpdateWorld(pWorldData);
            });
            vignette.OnClick += () => {
                connectionServiceLinker.TriesToConnect(pWorldData.worldUrl);
                pWorldData.dateLastConnection = DateTime.UtcNow.ToFileTime();
                pVirtualWorlds.UpdateWorld(pWorldData);
            };

            return vignette;
        }

        public async Task<VignetteBuffer> CreateVignette(ImageDto pImageDto, VignetteBuffer pBuffer = null, Action onClick = null)
        {
            VignetteContainerData data = VignetteContainerData.FindVignetteContainerDataByVignetteScale(vignetteMode, m_vignetteContainerDatas);
            var vignette = Instantiate(data.VignettePrefab, gridLayout.transform).GetComponent<VignetteDisplayer>();
            vignette.SetFavoryActive(false);
            vignette.SetDeleteActive(false);

            if (pImageDto.FirstChildren.Count > 0) // should be a label at least
                foreach(var child in pImageDto.FirstChildren)
                    if (child is LabelDto label)
                        vignette.SetupDisplay(label.text);

            Sprite sprite = await pImageDto.GetSprite();
            vignette.SetSprite(sprite);

            vignetteDisplayers.Add(vignette);

            if (pBuffer == null)
            {
                pBuffer = new VignetteBuffer();
                m_vignetteBuffers.Add(pBuffer);
            }

            pBuffer.SetImageDto(pImageDto);
            pBuffer.OnVignetteClicked += onClick;
            pBuffer.SetVignetteDisplayer(vignette);
            FillWithEmptyVignettes();
            return pBuffer;
        }

        public void ResetVignettes() => ResetVignettes(true);

        public void ResetVignettes(bool runtime)
        {
            if (!m_shouldResetAndFetchVignetteFromDB)
            {
                ResetVignetteForForms();
                return;
            }

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

        private async void ResetVignetteForForms()
        {
            foreach(var buffer in m_vignetteBuffers)
            {
                Destroy(buffer.VignetteGo);
            }

            vignetteDisplayers.Clear();

            foreach (var buffer in m_vignetteBuffers)
            {
                await CreateVignette(buffer.ImageDto, buffer);
            }
            UpdateNavigation();
        }

        public void FillWithEmptyVignettes()
        {
            foreach (var emptyVignette in m_emptyVignettes)
                Destroy(emptyVignette.gameObject);

            m_emptyVignettes = new();

            var nbrVignetteTotal = 0;
            if (vignetteMode == E_VignetteScale.Small) nbrVignetteTotal = 8;
            else if (vignetteMode == E_VignetteScale.Mid) nbrVignetteTotal = 3;
            else if (vignetteMode == E_VignetteScale.Large) nbrVignetteTotal = 2;

            for (var i = vignetteDisplayers.Count - nbrVignetteTotal; i < 0; i++)
                m_emptyVignettes.Add(Instantiate(emptyVignettePrefab, gridLayout.transform));
        }
 
        private void SetGridLayout(VignetteContainerData data, bool runtime = true)
        {
            if (data == null) return;

            gridLayout.cellSize = data.VignetteSize;
            gridLayout.constraintCount = data.VignetteRowAmount;
            gridLayout.spacing = data.VignetteSpace;

            scrollbar.value = 0;
            if (gameObject.activeSelf && isActiveAndEnabled)
                StartCoroutine(ScaleColliders());

            ResetVignettes(runtime);
        }

        IEnumerator ScaleColliders()
        {
            yield return new WaitForSeconds(0.35f);
            scaler.ScaleColliders();
        }

        public void UpdateNavigation()
        {
            if ((int)vignetteMode < vignetteDisplayers.Count)
            {
                buttonLeft.gameObject.SetActive(true);
                buttonRight.gameObject.SetActive(true);
            }
            else
            {
                buttonLeft.gameObject.SetActive(false);
                buttonRight.gameObject.SetActive(false);
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

    /// <summary>
    /// A class use to handle vignettes when they are created useing a orm system
    /// The matter is to reparent the OnClick event to the rigth answer DTO.
    /// </summary>
    public class VignetteBuffer
    {
        private VignetteDisplayer m_vignetteDisplayer;
        private ImageDto m_imageDto;
        public ImageDto ImageDto => m_imageDto;
        public GameObject VignetteGo => m_vignetteDisplayer.gameObject;


        public event Action OnVignetteClicked;

        public void SetImageDto(ImageDto dto)
        {
            m_imageDto = dto;
        }

        public void SetVignetteDisplayer(VignetteDisplayer vignetteDisplayer)
        {
            if (m_vignetteDisplayer != null) m_vignetteDisplayer.OnClick -= OnVignetteClicked;
            m_vignetteDisplayer = vignetteDisplayer;
            m_vignetteDisplayer.OnClick += OnVignetteClicked;
        }
    }
}

