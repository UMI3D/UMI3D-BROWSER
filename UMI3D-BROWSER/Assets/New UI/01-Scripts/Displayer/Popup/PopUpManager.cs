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

using inetum.unityUtils;
using System;
using System.Collections.Generic;
using umi3d.browserRuntime.notificationKeys;
using umi3dBrowsers.displayer;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    public static class PopUpManager 
    {
        public static Transform root { get; private set; }
        public static GameObject infoPopUp { get; private set; }
        public static GameObject warningPopUp { get; private set; }
        public static GameObject errorPopUp { get; private set; }

        static PopUpManager()
        {
            NotificationHub.Default.Subscribe(
                typeof(PopUpManager).FullName,
                DialogueBoxNotificationKey.NewDialogueBox,
                null,
                DialogueBoxNewDialogueBox
            );

            NotificationHub.Default.Subscribe(
                typeof(PopUpManager).FullName,
                DialogueBoxNotificationKey.CloseAllPopUp,
                null,
                CloseAll
            );
        }

        public static void Init(GameObject infoPopUp, GameObject warningPopUp, GameObject errorPopUp)
        {
            GameObject Instantiate(GameObject popUpPrefab)
            {
                GameObject popUp = GameObject.Instantiate(popUpPrefab, root);
                popUp.SetActive(false);
                return popUp;
            }

            PopUpManager.infoPopUp = Instantiate(infoPopUp);
            PopUpManager.warningPopUp = Instantiate(warningPopUp);
            PopUpManager.errorPopUp = Instantiate(errorPopUp);
        }

        public static void SetRoot(Transform root)
        {
            // TODO: Test for case when pupUp is displayer while changing the root.
            PopUpManager.root = root;
            infoPopUp?.transform.SetParent(root);
            warningPopUp?.transform.SetParent(root);
            errorPopUp?.transform.SetParent(root);
        }

        static void DialogueBoxNewDialogueBox(Notification notification)
        {
            UnityEngine.Debug.Log($"PopUpManager new dialogue box");

            GameObject popUp;
            PopupDisplayer displayer;
            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.Type, out DialogueBoxNotificationKey.NewDialogueBoxInfo.PopUpType type))
            {
                switch (type)
                {
                    case DialogueBoxNotificationKey.NewDialogueBoxInfo.PopUpType.Info:
                        popUp = infoPopUp;
                        break;
                    case DialogueBoxNotificationKey.NewDialogueBoxInfo.PopUpType.Warning:
                        popUp = warningPopUp;
                        break;
                    case DialogueBoxNotificationKey.NewDialogueBoxInfo.PopUpType.Error:
                        popUp = errorPopUp;
                        break;
                    default:
                        UnityEngine.Debug.LogError($"[PopUpManager] No PopUp type found: user info");
                        popUp = infoPopUp;
                        break;
                }
                displayer = popUp.GetComponent<PopupDisplayer>();
            }
            else
            {
                popUp = infoPopUp;
                displayer = popUp.GetComponent<PopupDisplayer>();
            }
            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.Arguments, out Dictionary<string, object> arguments))
            {
                displayer.SetArguments(arguments);
            }
            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.Title, out string title))
            {
                displayer.Title = title;
            }
            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.Message, out string message))
            {
                displayer.Description = message;
            }
            if (notification.TryGetInfoT(DialogueBoxNotificationKey.NewDialogueBoxInfo.Buttons, out (string, Action)[] buttons))
            {
                displayer.SetButtons(buttons);
            }

            NotificationHub.Default.Notify(
                typeof(PopUpManager).FullName,
                DialogueBoxNotificationKey.PopUpOpen
            );

            popUp.SetActive(true);
        }

        static void CloseAll()
        {
            NotificationHub.Default.Notify(
                typeof(PopUpManager).FullName,
                DialogueBoxNotificationKey.PopUpClose
            );

            infoPopUp.SetActive(false);
            warningPopUp.SetActive(false);
            errorPopUp.SetActive(false);
        }
    }
}