/*
Copyright 2019 - 2023 Inetum

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
using TMPro;
using umi3d.common.interaction;
using umi3dBrowsers.container;
using umi3dBrowsers.services.connection;
using umi3dVRBrowsersBase.connection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace umi3d
{
    public class MainContainer : MonoBehaviour
    {
        [Header("Navigation-navbar")]
        [SerializeField] private Button parameterButton;
        [SerializeField] private Button storageButton;
        [SerializeField] private Button hintButton;
        [SerializeField] private Button bugButton;
        [Space]
        [SerializeField] private ColorBlock navBarButtonsColors = new ColorBlock();
        [Space]
        [SerializeField] private UnityEvent parametersClicked;
        [SerializeField] private UnityEvent storageClicked;
        [SerializeField] private UnityEvent hintClicked;
        [SerializeField] private UnityEvent bugClicked;
        [Space]
        [SerializeField] private GameObject parametersContent;
        [SerializeField] private GameObject storageContent;
        [SerializeField] private GameObject mainContent;
        [SerializeField] private GameObject standUpContent;
        [SerializeField] private GameObject flagContent;
        [SerializeField] private GameObject dynamicServerContent;
        [Space]
        [SerializeField] private GameObject Top;
        // todo : call something like an hint manager hints
        // todo : call something like a pop up manager to open a bug popup

        public enum ContentState { mainContent, storageContent, parametersContent, flagContent, standUpContent, dynamicServerContent };
        [SerializeField, Tooltip("start content")] private ContentState contentState;
#if UNITY_EDITOR
        public ContentState _ContentState { get { return contentState; } set { contentState = value; } }
        public void ToolAccessProcessForm(ConnectionFormDto connectionFormDto) { ProcessForm(connectionFormDto); }
#endif


        [Header("Navigation")]
        [SerializeField] private SimpleButton backButton;
        [SerializeField] private SimpleButton standUpButton;
        [SerializeField] private SimpleButton flagButton;
        [Space]
        [SerializeField] private UnityEvent backButtonClicked;
        [SerializeField] private UnityEvent standUpButtonClicked;
        [SerializeField] private UnityEvent flagButtonClicked;
        [Space]
        [SerializeField] private GameObject backgroundShadder;

        [Header("Title")]
        [SerializeField] private TextMeshProUGUI prefixText;
        [SerializeField] private TextMeshProUGUI suffixText;

        [Header("Version")]
        [SerializeField] private TextMeshProUGUI versionText;

        [Header("Connection")]
        [SerializeField] private URLDisplayer urlDisplayer;
        [SerializeField] private DynamicServerContainer dynamicServerContainer;

        [Header("Services")]
        [SerializeField] private ConnectionProcessor connectionProcessorService;

        private void Awake()
        {
            navBarButtonsColors.colorMultiplier = 1.0f;
        }

        private void Start()
        {
            BindNavigationButtons();
            BindURL();
            BindFormContainer();
            BindServices();
            HandleContentState(contentState);
        }

        public void SetVersion(string version)
        {
            versionText.text = version;
        }

        public void SetTitle(string prefix, string suffix)
        {
            float suffitLength = suffix.Length;
            float prefixLength = prefix.Length;
            Rect suffitRectTransform = suffixText.rectTransform.rect;
            Rect prefixRectTransform = prefixText.rectTransform.rect;

            prefixText.text = prefix;
            suffixText.text = suffix;
            suffitRectTransform.width = suffitLength * 12.5f;
            prefixRectTransform.width = prefixLength * 10f;
            suffixText.rectTransform.sizeDelta = new Vector2(suffitRectTransform.width, suffitRectTransform.height);
            prefixText.rectTransform.sizeDelta = new Vector2(prefixRectTransform.width, prefixRectTransform.height);
        }

        private void BindNavigationButtons()
        {
            parameterButton.colors = navBarButtonsColors;
            storageButton.colors = navBarButtonsColors;
            hintButton.colors = navBarButtonsColors;
            bugButton.colors = navBarButtonsColors;

            parameterButton?.onClick.AddListener(() =>
            {
                parametersClicked?.Invoke();
                HandleContentState(ContentState.parametersContent);
            });
            storageButton?.onClick.AddListener(() => {
                storageClicked?.Invoke();
                HandleContentState(ContentState.storageContent);
            });
            hintButton?.onClick.AddListener(() => {
                hintClicked?.Invoke();

            });
            bugButton?.onClick.AddListener(() => {
                bugClicked?.Invoke();

            });
            backButton?.OnClick.AddListener(() =>
            {
                backButtonClicked?.Invoke();
                if (contentState == ContentState.parametersContent || contentState == ContentState.storageContent)
                    HandleContentState(ContentState.mainContent);
            });
            flagButton?.OnClick.AddListener(() =>
            {
                flagButtonClicked?.Invoke();
                HandleContentState(ContentState.standUpContent);
            });
            standUpButton?.OnClick.AddListener(() =>
            {
                standUpButtonClicked?.Invoke();
                SetUpSkeleton setUp = PlayerDependenciesAccessor.Instance.SetUpSkeleton;
                StartCoroutine(setUp.SetUpAvatar());
                HandleContentState(ContentState.mainContent);
            });
        }

        private void BindURL()
        {
            urlDisplayer.OnSubmit.AddListener((url) =>
            {
                connectionProcessorService.TryConnectToMediaServer(url);
            });
        }
        private void BindFormContainer()
        {
            dynamicServerContainer.OnFormAnwser += (formAnswer) => connectionProcessorService.SendFormAnswer(formAnswer);
        }

        private void BindServices()
        {
            connectionProcessorService.OnConnectionFailure += (message) => { Debug.LogError("Failled to conenct"); };
            connectionProcessorService.OnMediaServerPingSuccess += (virtualWorldData) => 
            {
                SetTitle("Connected to", virtualWorldData.worldName);
            };
            connectionProcessorService.OnFormReceived += (connectionFormDto) =>
            {
                HandleContentState(ContentState.dynamicServerContent);
                ProcessForm(connectionFormDto);
            };
            connectionProcessorService.OnAsksToLoadLibrairies += (ids) => connectionProcessorService.SendAnswerToLibrariesDownloadAsk(true);
            connectionProcessorService.OnConnectionSuccess += () => backgroundShadder.SetActive(false);
        }

        private void ProcessForm(ConnectionFormDto connectionFormDto)
        {
            dynamicServerContainer.ProcessConnectionFormDto(connectionFormDto);
        }

        private void HandleContentState(ContentState state)
        {
            contentState = state;
            switch (state)
            {
                case ContentState.mainContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    mainContent.SetActive(true);
                    SetTitle("Connect to an", "Intraverse Portal");
                    break;
                case ContentState.storageContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    storageContent.SetActive(true);
                    backButton?.gameObject.SetActive(true);
                    break;
                case ContentState.parametersContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    parametersContent.SetActive(true);
                    backButton?.gameObject.SetActive(true);
                    SetTitle("", "Settings");
                    break;
                case ContentState.flagContent:
                    CloseAllPanels();
                    flagContent.SetActive(true);
                    SetTitle("Choose your", "language");
                    break;
                case ContentState.standUpContent:
                    CloseAllPanels();
                    standUpContent.SetActive(true);
                    break;
                case ContentState.dynamicServerContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    dynamicServerContent.SetActive(true);
                    break;
            }
        }

        private void CloseAllPanels()
        {
            Top.SetActive(false);
            parametersContent.SetActive(false);
            storageContent.SetActive(false);
            mainContent.SetActive(false);
            backButton?.gameObject.SetActive(false);
            flagContent.SetActive(false);
            standUpContent.SetActive(false);
        }

        //public void ConnectToUmi3DEnvironement(string url, string port)
        //{

        //    StartCoroutine(WaitReady(new AdvancedConnectionPanel.Data() { ip = url, port = port }));
        //}

        //private IEnumerator WaitReady(AdvancedConnectionPanel.Data data)
        //{
        //    while (!Connecting.Exists && !UMI3DEnvironmentLoader.Exists)
        //        yield return new WaitForEndOfFrame();

        //    Connecting.Instance.Connect(data);

        //    while (!UMI3DEnvironmentLoader.Exists)
        //        yield return new WaitForEndOfFrame();

        //    UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => Debug.Log("environment loaded"));
        //}
    }
}