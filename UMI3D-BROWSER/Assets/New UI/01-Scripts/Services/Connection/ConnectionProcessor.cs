using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.common;
using umi3d.common.interaction;
using umi3dVRBrowsersBase.connection;
using UnityEngine;

namespace umi3dBrowsers.services.connection
{
    public class ConnectionProcessor : MonoBehaviour
    {
        public event Action<VirtualWorldData> OnMediaServerPingSuccess;
        public event Action<string> OnConnectionFailure;
        public event Action<ConnectionFormDto> OnFormReceived;
        public event Action<List<string>> OnAsksToLoadLibrairies;
        public event Action OnConnectionSuccess;

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
        private Action<FormAnswerDto> _formAnswerCallBack;
        private Action<bool> _shouldDownloadLibrariesCallBack;

        private void Start()
        {
            identifier.OnParametersAvailible += HandleParameters;
            identifier.OnLibrairiesAvailible += HandleLibrairies;
            UMI3DCollaborationEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => OnConnectionSuccess.Invoke());
        }

        public async void TryConnectToMediaServer(string url)
        {
            VirtualWorldData virtualWorldData = new VirtualWorldData();

            url.Trim();
            if (string.IsNullOrEmpty(url))
            {
                OnConnectionFailure?.Invoke("Please enter an URL");
                return;
            }

            var media = await GetMediaServer(url);
            if (media != null)
            {
                virtualWorldData.worldUrl = url;
                virtualWorldData.worldName = media.name;
                virtualWorldData.isFavorite = false;
                virtualWorldData.dateFirstConnection = DateTime.UtcNow.ToString();
                virtualWorldData.dateLastConnection = DateTime.UtcNow.ToString();

                OnMediaServerPingSuccess?.Invoke(virtualWorldData);

                UMI3DCollaborationClientServer.Instance.Identifier = identifier;
                UMI3DCollaborationClientServer.Connect(media, (message) =>
                {
                    OnConnectionFailure?.Invoke(message);
                });
            }
            else
            {
                OnConnectionFailure?.Invoke("URL mismatch");
                return;
            }
        }

        public void SendFormAnswer(FormAnswerDto formAnswer)
        {
            _formAnswerCallBack?.Invoke(formAnswer);
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
            _formAnswerCallBack = action;
            OnFormReceived?.Invoke(dto);
        }

        private void HandleLibrairies(List<string> ids, Action<bool> action)
        {
            _shouldDownloadLibrariesCallBack = action;

            if (ids.Count == 0)
            {
                _shouldDownloadLibrariesCallBack.Invoke(true);
            }
            else
                OnAsksToLoadLibrairies?.Invoke(ids);
        }

        protected static string FormatUrl(string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                return "http://" + url;
            return url;
        }
    }
}

