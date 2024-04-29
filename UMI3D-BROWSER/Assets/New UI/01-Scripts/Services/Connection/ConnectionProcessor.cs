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
        public event Action<string> OnTryToConnect;
        public event Action<string> OnConnectionFailure;
        public event Action<ConnectionFormDto> OnParamFormReceived;
        public event Action<umi3d.common.interaction.form.ConnectionFormDto> OnDivFormReceived;
        public event Action<List<string>> OnAsksToLoadLibrairies;
        public event Action OnConnectionSuccess;
        public event Action OnAnswerFailed {
            add { identifier.OnAnswerFailed += value; } 
            remove { identifier.OnAnswerFailed -= value; }
        }

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
        private Action<umi3d.common.interaction.form.FormAnswerDto> _formDivAnswerCallBack;
        private Action<bool> _shouldDownloadLibrariesCallBack;

        private void Start()
        {
            identifier.OnParamFormAvailible += HandleParameters;
            identifier.OnDivFormAvailible += HandleDivs;
            identifier.OnLibrairiesAvailible += HandleLibrairies;
            UMI3DCollaborationEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => OnConnectionSuccess.Invoke());
        }

        public async void TryConnectToMediaServer(string url)
        {
            VirtualWorldData virtualWorldData = new VirtualWorldData();
            OnTryToConnect?.Invoke(url);

            url.Trim();
            if (string.IsNullOrEmpty(url))
            {
                OnConnectionFailure?.Invoke("enter_url");
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
                
                OnMediaServerPingSuccess?.Invoke(virtualWorldData);

                UMI3DCollaborationClientServer.Instance.Identifier = identifier;
                UMI3DCollaborationClientServer.Connect(media, (message) =>
                {
                    OnConnectionFailure?.Invoke(message);
                });
            }
            else
            {
                OnConnectionFailure?.Invoke("url_mismatch");
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
            OnParamFormReceived?.Invoke(dto);
        }

        private void HandleDivs(umi3d.common.interaction.form.ConnectionFormDto dto, Action<umi3d.common.interaction.form.FormAnswerDto> action)
        {
            _formDivAnswerCallBack = action;
            OnDivFormReceived?.Invoke(dto);

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

        public void SendFormAnswer(FormAnswerDto formAnswer)
        {
            _formParamAnswerCallBack?.Invoke(formAnswer);
        }

        public void SendFormAnswer(umi3d.common.interaction.form.FormAnswerDto formAnswer)
        {
            Debug.Log("Todo :: SendAnswer to server======================================");
            //_formAnswerCallBack?.Invoke(formAnswer);
        }
    }
}

