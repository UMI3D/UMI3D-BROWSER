using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.common.interaction;
using umi3dBrowsers.services.connection;
using UnityEngine;
using static umi3dBrowsers.MainContainer;

namespace umi3dBrowsers.linker
{
    [CreateAssetMenu(menuName = "Linker/ConnectionService")]
    public class ConnectionServiceLinker : ScriptableObject
    {
        public event Action<string> OnTryToConnect;
        public void TriesToConnect(string url) { OnTryToConnect?.Invoke(url); }

        public event Action<string> OnConnectionFailure;
        public void ConnectionFailure(string url) { OnConnectionFailure?.Invoke(url); }

        public event Action<VirtualWorldData> OnMediaServerPingSuccess;
        public void MediaServerPingSuccess(VirtualWorldData data) {  OnMediaServerPingSuccess?.Invoke(data);}

        public event Action<ConnectionFormDto> OnParamFormDtoReceived;
        public void ParamFormDtoReceived(ConnectionFormDto data) {  OnParamFormDtoReceived?.Invoke(data);}

        public event Action<List<string>> OnAsksToLoadLibrairies;
        public void AsksToLoadLibrairies(List<string> ids) { OnAsksToLoadLibrairies?.Invoke(ids);}

        public event Action OnConnectionSuccess;
        public void ConnectionSuccess() { OnConnectionSuccess?.Invoke(); }

        public event Action OnAnswerFailed;
        public void AnswerFailed() { OnAnswerFailed?.Invoke(); }

        public event Action<bool> OnSendAnswerToLibrariesDownloadAsk;
        public void SendAnswerToLibrariesDownloadAsk(bool v) { OnSendAnswerToLibrariesDownloadAsk?.Invoke(v);}
    }
}
