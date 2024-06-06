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
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.common;
using umi3d.common.interaction;
using umi3dBrowsers.linker;
using umi3dVRBrowsersBase.connection;
using umi3dVRBrowsersBase.ui.watchMenu;
using UnityEngine;

namespace umi3dBrowsers.services.connection
{
    public class ConnectionProcessor : MonoBehaviour
    {
        [SerializeField,
            Tooltip("In seconds, after this time, if no connection was established, display an error message.")]
        private float maxConnectionTime = 5;
        [SerializeField] private Identifier identifier;

        /// <summary>
        /// Object which handles the connection to a master server.
        /// </summary>
        private LaucherOnMasterServer _launcher = new LaucherOnMasterServer();
        AdvancedConnectionPanel.Data _mediaConnectionData = new AdvancedConnectionPanel.Data();
        private string _environmentUrl = "";
        private string _mediaDataServerUrl = "";
        private Action<FormAnswerDto> _formParamAnswerCallBack;
        private Action<bool> _shouldDownloadLibrariesCallBack;

        [Header("Linkers")]
        [SerializeField] private ConnectionServiceLinker connectionServiceLinker;

        private void Start()
        {
            identifier.OnParamFormAvailible += HandleParameters;
            identifier.OnLibrairiesAvailible += HandleLibrairies;
            UMI3DCollaborationEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => connectionServiceLinker.ConnectionSuccess());

            connectionServiceLinker.OnTryToConnect += TryConnectToMediaServer;
        }

        public async void TryConnectToMediaServer(string url)
        {
            UMI3DCollaborationClientServer.Instance.Clear();
            VirtualWorldData virtualWorldData = new VirtualWorldData();

            url.Trim();
            if (string.IsNullOrEmpty(url))
            {
                connectionServiceLinker.ConnectionFailure("enter_url");
                return;
            }

            var media = await GetMediaServer(url);
            if (media != null)
            {
                virtualWorldData.worldUrl = url;
                virtualWorldData.worldName = media.name;
                virtualWorldData.isFavorite = false;
                virtualWorldData.dateFirstConnection = DateTime.UtcNow.ToFileTime();
                virtualWorldData.dateLastConnection = DateTime.UtcNow.ToFileTime();
                
                connectionServiceLinker.MediaServerPingSuccess(virtualWorldData);
                services.connection.PlayerPrefsManager.GetVirtualWorlds().AddWorld(virtualWorldData);

                UMI3DCollaborationClientServer.Instance.Identifier = identifier;
                UMI3DCollaborationClientServer.Connect(media, (message) =>
                {
                    connectionServiceLinker.ConnectionFailure(message);
                });
            }
            else
            {
                connectionServiceLinker.ConnectionFailure("url_mismatch");
                return;
            }
        }

        public void SendAnswerToLibrariesDownloadAsk(bool canDownload)
        {
            _shouldDownloadLibrariesCallBack?.Invoke(canDownload);
        }

        private async Task<MediaDto> GetMediaServer(string url)
        {
            var curentUrl = FormatUrl(url) + UMI3DNetworkingKeys.media;
            _mediaDataServerUrl = curentUrl;
            try
            {
                return await UMI3DCollaborationClientServer.GetMedia(_mediaDataServerUrl, (e) => _mediaDataServerUrl == curentUrl && e.count < 3);
            }
            catch
            {
                return null;
            }
        }

        private void HandleParameters(ConnectionFormDto dto, Action<FormAnswerDto> action)
        {
            _formParamAnswerCallBack = action;
            connectionServiceLinker.ParamFormDtoReceived(dto);
        }

        private void HandleLibrairies(List<string> ids, Action<bool> action)
        {
            _shouldDownloadLibrariesCallBack = action;

            if (ids.Count == 0)
            {
                _shouldDownloadLibrariesCallBack.Invoke(true);
            }
            else
                connectionServiceLinker.AsksToLoadLibrairies(ids);
        }

        protected static string FormatUrl(string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                return "http://" + url;
            return url;
        }

        public void SendFormAnswer(FormAnswerDto formAnswer)
        {
            _formParamAnswerCallBack?.Invoke(formAnswer);
        }

        /// <summary>
        /// Leaves current UMI3D Environment.
        /// </summary>
        public void Disconnect()
        {
            var mainThreadDispatcher = MainThreadDispatcher.UnityMainThreadDispatcher.Instance() as CustomMainThreadDispatcher;
            if (mainThreadDispatcher != null)
            {
                mainThreadDispatcher.StopAllCoroutines();
                mainThreadDispatcher.ClearQueue();
            }
            else
            {
                Debug.LogError("MainTheadDispatcher should not be null");
            }

            UMI3DEnvironmentLoader.Clear();
            UMI3DResourcesManager.Instance.ClearCache();
            UMI3DCollaborationClientServer.Logout();
            umi3dVRBrowsersBase.DontDestroyOnLoad.DestroyAllInstances();

            WatchMenu.UnPinAllMenus();
            identifier.Reset();
        }
    }
}

