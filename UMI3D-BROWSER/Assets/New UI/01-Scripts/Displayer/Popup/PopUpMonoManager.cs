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
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.notificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    public class PopUpMonoManager : MonoBehaviour
    {
        public GameObject infoPopUp;
        public GameObject warningPopUp;
        public GameObject errorPopUp;


        void Awake()
        {
            PopUpManager.Init(infoPopUp, warningPopUp, errorPopUp);
        }

        [ContextMenu("Test Info Pop Up")]
        void TestInfoPopUp()
        {
            Action callback = null;
            NotificationHub.Default.Notify(
                    this,
                    DialogueBoxNotificationKey.NewDialogueBox,
                    new()
                    {
                        { DialogueBoxNotificationKey.NewDialogueBoxInfo.Type, DialogueBoxNotificationKey.NewDialogueBoxInfo.PopUpType.Info },
                        { DialogueBoxNotificationKey.NewDialogueBoxInfo.Title, "Test" },
                        { DialogueBoxNotificationKey.NewDialogueBoxInfo.Message, "This is a test" },
                        { DialogueBoxNotificationKey.NewDialogueBoxInfo.Buttons, new[]
                            {
                                ("popup_cancel", callback),
                                ("popup_yes", callback)
                            }
                        },
                    }
                );
        }
    }
}
       

