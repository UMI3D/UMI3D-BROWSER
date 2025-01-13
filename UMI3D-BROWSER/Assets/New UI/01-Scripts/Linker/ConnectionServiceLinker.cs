using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using umi3d.common.interaction;
using umi3dBrowsers.services.connection;
using Unity.VisualScripting;
using UnityEngine;
using static umi3dBrowsers.MainContainer;

namespace umi3dBrowsers.linker
{
    [CreateAssetMenu(menuName = "Linker/ConnectionService")]
    public class ConnectionServiceLinker : ScriptableObject
    {
        public event Action<string> OnTryToConnect;
        public void TriesToConnect(string url) { OnTryToConnect?.Invoke(url); }
        public void TriesToConnect(TMP_Text url) { OnTryToConnect?.Invoke(url.text.Substring(0, url.text.Length - 1)); }

        public event Action<string> OnConnectionFailure;
        public void ConnectionFailure(string url) { OnConnectionFailure?.Invoke(url); }

        public event Action<VirtualWorldData> OnMediaServerPingSuccess;
        public void MediaServerPingSuccess(VirtualWorldData data) {  OnMediaServerPingSuccess?.Invoke(data);}

        public event Action<ConnectionFormDto> OnParamFormDtoReceived;
        public void ParamFormDtoReceived(ConnectionFormDto data) {  OnParamFormDtoReceived?.Invoke(data); }

        public event Action<umi3d.common.interaction.form.ConnectionFormDto> OnDivFormDtoReceived;
        public void DivFormDtoReceived(umi3d.common.interaction.form.ConnectionFormDto data) { OnDivFormDtoReceived?.Invoke(data); }

        public event Action<WaitConnectionDto> OnWaitReceived;
        public void WaitReceived(WaitConnectionDto data) { OnWaitReceived?.Invoke(data); }


        public event Action<List<string>, Action<bool>> OnAsksToLoadLibrairies;
        public void AsksToLoadLibrairies(List<string> ids, Action<bool> action) { OnAsksToLoadLibrairies?.Invoke(ids, action);}

        public event Action OnConnectionSuccess;
        public void ConnectionSuccess() { OnConnectionSuccess?.Invoke(); }

        public event Action OnAnswerFailed;
        public void AnswerFailed() { OnAnswerFailed?.Invoke(); }

        public event Action<bool> OnSendAnswerToLibrariesDownloadAsk;
        public void SendAnswerToLibrariesDownloadAsk(bool v) { OnSendAnswerToLibrariesDownloadAsk?.Invoke(v); }

        public event Action<FormAnswerDto> OnSendFormAnswer;
        public void SendFormAnswer(FormAnswerDto formAnswer) { OnSendFormAnswer?.Invoke(formAnswer); }

        public event Action<umi3d.common.interaction.form.FormAnswerDto> OnSendDivFormAnswer;
        public void SendDivFormAnswer(umi3d.common.interaction.form.FormAnswerDto formAnswer) { OnSendDivFormAnswer?.Invoke(formAnswer); }

        public event Action OnSendWaitAnswer;
        public void SendWaitAnswer() { OnSendWaitAnswer?.Invoke(); }
    }
}
