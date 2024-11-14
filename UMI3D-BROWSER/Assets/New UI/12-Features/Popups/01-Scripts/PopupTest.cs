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

using System.Security.Policy;
using umi3d.browserRuntime.notificationKeys;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace umi3d.browserRuntime.ui.popup
{
    public class PopupTest : MonoBehaviour
    {
        PopupNotifier notifier;

        const string INETUM_TABLE = "UMI3D_inetum";

        void Awake()
        {
            notifier = new(this);
        }

        [ContextMenu("Test connection")]
        void TestConnection()
        {
            notifier
            .enqueue
            .SetArguments(("url", "test/url.com"))
            .SetTitle(INETUM_TABLE, "popup_connection_server")
            .SetDescription(INETUM_TABLE, "popup_trying_connect")
            .Notify();
        }

        [ContextMenu("Test close App")]
        void TestCloseApp()
        {
            notifier
            .enqueue
            .SetTitle(INETUM_TABLE, "Quit")
            .SetButtons((INETUM_TABLE, "Quit"), (INETUM_TABLE, "Cancel"))
            .Notify();
        }

        [ContextMenu("Test leave")]
        void TestLeave()
        {
            notifier
              .enqueue
              .SetType(PopupType.Information)
              .SetTitle(INETUM_TABLE, "popup_leave")
              .SetButtons((INETUM_TABLE, "popup_yes"), (INETUM_TABLE, "popup_no"))
              .Notify();
        }

        [ContextMenu("Test Error")]
        void TestError()
        {
            notifier
              .enqueue
              .SetType(PopupType.Error)
              .SetArguments(("error", "Error message, a long message.\nThat contain line break."))
              .SetTitle(INETUM_TABLE, "popup_fail_connect")
              .SetDescription(INETUM_TABLE, "error_msg")
              .SetButtons((INETUM_TABLE, "popup_close"))
              .Notify();
        }
    }
}