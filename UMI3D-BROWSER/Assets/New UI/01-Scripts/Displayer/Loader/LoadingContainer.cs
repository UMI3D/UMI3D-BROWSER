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
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using umi3dBrowsers.data.ui;
using umi3dBrowsers.linker;
using umi3dBrowsers.linker.ui;
using umi3dBrowsers.services.environment;
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

        private umi3d.cdk.Progress _currentProgress = null;
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
            UMI3DEnvironmentClient.EnvironementJoinned.AddListener(() =>
            {
                OnLoadingFinished?.Invoke();
                loadingTipDisplayer.StopDisplayTips();
                m_linker.StopDisplayEnvironmentHandler();
                _loadingInProgress = false;
            });
            UMI3DCollaborationClientServer.onProgress.AddListener(NewProgressReceived);
        }

        void NewProgressReceived(umi3d.cdk.Progress progress)
        {
            if (_currentProgress != null)
            {
                _currentProgress.OnCompleteUpdated -= OnCompleteUpdated;
                _currentProgress.OnFailedUpdated -= OnFailedUpdated;
                _currentProgress.OnStatusUpdated -= OnStatusUpdated;
            }
            _currentProgress = progress;

            void OnCompleteUpdated(float i) { OnProgressChange(_currentProgress.progressPercent / 100f); }
            void OnFailedUpdated(float i) { }
            void OnStatusUpdated(string i) {
                loadingBarDisplayer.SetLoadingState(_currentProgress.currentState); 
                loadingBarDisplayer.SetProgressBarValue(_currentProgress.progressPercent / 100f);
            }

            _currentProgress.OnCompleteUpdated += OnCompleteUpdated;
            _currentProgress.OnFailedUpdated += OnFailedUpdated;
            _currentProgress.OnStatusUpdated += OnStatusUpdated;

            OnProgressChange(_currentProgress.progressPercent / 100f);
            loadingBarDisplayer.SetLoadingState(_currentProgress.currentState);
        }

        private void OnProgressChange(float val)
        {
            if (val >= 0 && val < 1)
            {
                if (_loadingInProgress == false) // Notify that a loading is in progress
                {
                    OnLoadingInProgress?.Invoke();
                    m_linker.DisplayEnvironmentHandler();
                    loadingTipDisplayer.DisplayTips();
                    _loadingInProgress = true;
                }
            }
            else
            {
                if (_loadingInProgress) // Notify that the loading has finished
                {
                    OnLoadingFinished?.Invoke();
                    loadingTipDisplayer.StopDisplayTips();
                    m_linker.StopDisplayEnvironmentHandler();
                    _loadingInProgress = false;
                }
            }
        }
    }
}

