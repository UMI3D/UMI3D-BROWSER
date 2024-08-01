using inetum.unityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using umi3d.browserRuntime.notificationKeys;
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
        public void TriesToConnect(string url) 
        {
            Action callback = null;
            NotificationHub.Default.Notify(
                this,
                DialogueBoxNotificationKey.NewDialogueBox,
                new()
                {
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Type, DialogueBoxNotificationKey.NewDialogueBoxInfo.PopUpType.Info },
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Arguments, new Dictionary<string, object>() { { "url", url } } },
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Title, "popup_connection_server" },
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Message, "popup_trying_connect" },
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Buttons, new[]
                        {
                            ("popup_close", callback)
                        }
                    },
                }
            );
            OnTryToConnect?.Invoke(url); 
        }
        public void TriesToConnect(TMP_Text url) { TriesToConnect(url.text.Substring(0, url.text.Length - 1)); }

        public event Action<string> OnConnectionFailure;
        public void ConnectionFailure(string url) 
        {
            Action callback = null;
            NotificationHub.Default.Notify(
                this,
                DialogueBoxNotificationKey.NewDialogueBox,
                new()
                {
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Type, DialogueBoxNotificationKey.NewDialogueBoxInfo.PopUpType.Error },
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Arguments, new Dictionary<string, object>() { { "error", url } } },
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Title, "popup_fail_connect" },
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Message, "error_msg" },
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Buttons, new[]
                        {
                            ("popup_close", callback)
                        }
                    },
                }
            );
            OnConnectionFailure?.Invoke(url); 
        }

        public event Action<VirtualWorldData> OnMediaServerPingSuccess;
        public void MediaServerPingSuccess(VirtualWorldData data) {  OnMediaServerPingSuccess?.Invoke(data);}

        public event Action<ConnectionFormDto> OnParamFormDtoReceived;
        public void ParamFormDtoReceived(ConnectionFormDto data) {  OnParamFormDtoReceived?.Invoke(data); }

        public event Action<umi3d.common.interaction.form.ConnectionFormDto> OnDivFormDtoReceived;
        public void DivFormDtoReceived(umi3d.common.interaction.form.ConnectionFormDto data) { OnDivFormDtoReceived?.Invoke(data); }

        public event Action<List<string>, Action<bool>> OnAsksToLoadLibrairies;
        public void AsksToLoadLibrairies(List<string> ids, Action<bool> action) { OnAsksToLoadLibrairies?.Invoke(ids, action);}

        public event Action OnConnectionSuccess;
        public void ConnectionSuccess() { OnConnectionSuccess?.Invoke(); }

        public event Action OnAnswerFailed;
        public void AnswerFailed()
        {
            Action callback = null;
            NotificationHub.Default.Notify(
                this,
                DialogueBoxNotificationKey.NewDialogueBox,
                new()
                {
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Type, DialogueBoxNotificationKey.NewDialogueBoxInfo.PopUpType.Error },
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Title, "popup_answer_failed_title" },
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Message, "popup_answer_failed_description" },
                    { DialogueBoxNotificationKey.NewDialogueBoxInfo.Buttons, new[]
                        {
                            ("popup_close", callback)
                        }
                    },
                }
            );
            OnAnswerFailed?.Invoke(); 
        }

        public event Action<bool> OnSendAnswerToLibrariesDownloadAsk;
        public void SendAnswerToLibrariesDownloadAsk(bool v) { OnSendAnswerToLibrariesDownloadAsk?.Invoke(v); }

        public event Action<FormAnswerDto> OnSendFormAnwser;
        public void SendFormAnswer(FormAnswerDto formAnswer) { OnSendFormAnwser?.Invoke(formAnswer); }

        public event Action<umi3d.common.interaction.form.FormAnswerDto> OnSendDivFormAnwser;
        public void SendDivFormAnswer(umi3d.common.interaction.form.FormAnswerDto formAnswer) { OnSendDivFormAnwser?.Invoke(formAnswer); }
    }
}
