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
using umi3d.cdk.collaboration;
using umi3d.common;
using umi3dBrowsers.data.ui;
using umi3dBrowsers.linker;
using umi3dBrowsers.linker.ui;
using UnityEngine;

namespace umi3dBrowsers.displayer
{
    public class LoadingContainer : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private LoadingBarDisplayer loadingBarDisplayer;
        [SerializeField] private LoadingTipDisplayer loadingTipDisplayer;

        public event Action OnLoadingInProgress;
        public event Action OnLoadingFinished;

        private Progress _currentProgress = null;
        private bool _loadingInProgress = false;

        [SerializeField] private ConnectionToImmersiveLinker m_linker;
        [SerializeField] private MenuNavigationLinker m_menuNavigationLinker;
        [SerializeField] private PanelData m_loadingPanel;

        private void Awake()
        {
            m_linker.OnLeave += () => _loadingInProgress = false;
            OnLoadingInProgress += () => {
                m_menuNavigationLinker.ShowPanel(m_loadingPanel);
                m_menuNavigationLinker.ReplacePlayerAndShowPanel();
            };
            UMI3DEnvironmentClient.EnvironementLoaded.AddListener(() =>
            {
                OnLoadingFinished?.Invoke();
                loadingTipDisplayer.StopDisplayTips();
                m_linker.StopDisplayEnvironmentHandler();
                _loadingInProgress = false;
            });
            UMI3DCollaborationClientServer.onProgress.AddListener(NewProgressReceived);
        }

        void NewProgressReceived(Progress progress)
        {
            if (_currentProgress != null)
            {
                _currentProgress.OnFailedUpdated -= OnFailedUpdated;
                _currentProgress.OnStatusUpdated -= OnStatusUpdated;
            }
            _currentProgress = progress;

            void OnFailedUpdated(float i) { }
            void OnStatusUpdated(string i) {
                loadingBarDisplayer.SetLoadingState(_currentProgress.currentState); 
                loadingBarDisplayer.SetProgressBarValue(_currentProgress.progressPercent / 100f);
            }

            _currentProgress.OnFailedUpdated += OnFailedUpdated;
            _currentProgress.OnStatusUpdated += OnStatusUpdated;

            loadingBarDisplayer.SetLoadingState(_currentProgress.currentState);

            OnLoadingInProgress?.Invoke();
            m_linker.DisplayEnvironmentHandler();
            loadingTipDisplayer.DisplayTips();
            _loadingInProgress = true;
        }
    }
}

