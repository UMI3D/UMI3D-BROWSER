using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.common;
using umi3dBrowsers.displayer;
using umi3dVRBrowsersBase.connection;
using umi3dVRBrowsersBase.ui.playerMenu;
using umi3dVRBrowsersBase.ui.watchMenu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.WSA;

namespace umi3dBrowsers.mv.connection
{
    public class ConnectionProcessor : MonoBehaviour
    {
        public event Action<MediaDto> OnMediaServerConnectionSucess;
        public event Action<string> OnConnectionFailure;
        
        [SerializeField,
            Tooltip("In seconds, after this time, if no connection was established, display an error message.")]
        private float maxConnectionTime = 5;

        /// <summary>
        /// Object which handles the connection to a master server.
        /// </summary>
        private LaucherOnMasterServer _launcher = new LaucherOnMasterServer();
        AdvancedConnectionPanel.Data _mediaConnectionData = new AdvancedConnectionPanel.Data();
        private string _environmentUrl = "";
        private string _mediaDataServerUrl = "";
        private VirtualWorldData _virtualWorldConnectionData = new VirtualWorldData();


        public async void TryConnectToMediaServer(string url)
        {
            url.Trim();
            if (string.IsNullOrEmpty(url))
            {
                OnConnectionFailure?.Invoke("No url sent");
                return;
            }

            _virtualWorldConnectionData.worldUrl = url;

            var media = await GetMediaServer(url);
            if (media != null)
            {
                OnMediaServerConnectionSucess?.Invoke(media);
            }
            else
            {
                OnConnectionFailure?.Invoke("Wrong url");
                return;
            }
        }


        public async Task<MediaDto> GetMediaServer(string url)
        {
            var curentUrl = FormatUrl(_virtualWorldConnectionData.worldUrl) + UMI3DNetworkingKeys.media;
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

        protected static string FormatUrl(string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                return "http://" + url;
            return url;
        }
    }
}

