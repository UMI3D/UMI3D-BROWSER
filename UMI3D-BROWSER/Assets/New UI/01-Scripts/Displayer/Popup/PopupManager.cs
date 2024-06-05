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
using TMPro;
using umi3dBrowsers.linker;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;

namespace umi3dBrowsers.displayer
{
    public class PopupManager : MonoBehaviour
    {
        [Header("Pop up list")]
        [SerializeField] PopupDisplayer error;
        [SerializeField] PopupDisplayer warn;
        [SerializeField] PopupDisplayer info;
        [SerializeField] PopupDisplayer reportBug;

        public event Action OnPopUpOpen;
        public event Action OnPopUpClose;

        public PopupType ActivePopup;

        private List<PopupDisplayer> activePopUps = new();

        [Header("Linkers")]
        [SerializeField] private ConnectionServiceLinker connectionServiceLinker;

        public enum PopupType
        {
            Error,
            Warning,
            Info,
            ReportBug,
        }

        private void Awake()
        {
            error.OnDisabled += () => OnPopUpClose?.Invoke();
            warn.OnDisabled += () => OnPopUpClose?.Invoke();
            info.OnDisabled += () => OnPopUpClose?.Invoke();
            //reportBug.OnDisabled += () => OnPopUpClose?.Invoke();

            connectionServiceLinker.OnTryToConnect += (url) => {
                SetArguments(PopupManager.PopupType.Info, new Dictionary<string, object>() { { "url", url } });
                ShowPopup(PopupManager.PopupType.Info, "popup_connection_server", "popup_trying_connect");
            };
            connectionServiceLinker.OnConnectionFailure += (message) => {
                SetArguments(PopupManager.PopupType.Error, new Dictionary<string, object>() { { "error", message } });
                ShowPopup(PopupManager.PopupType.Error, "popup_fail_connect", "error_msg",
                    ("popup_close", () => { ClosePopUp(); }
                ));
            };
        }

        /// <param name="type"></param>
        /// <param name="title">Key for localization</param>
        /// <param name="description">Key for localization</param>
        /// <param name="buttons">Key for localization and action onClick (spawns on button per entry)</param>
        public void ShowPopup(PopupType type, string title, string description, params (string, Action)[] buttons)
        {
            OnPopUpOpen?.Invoke();
            ActivePopup = type;
            switch (type)
            {
                case PopupType.Error:
                    ActivatePopup(error, title, description, buttons);
                    break;
                case PopupType.Warning:
                    ActivatePopup(warn, title, description, buttons);
                    break;
                case PopupType.Info:
                    ActivatePopup(info, title, description, buttons);
                    break;
                default:
                    Debug.LogWarning("Unknown popup type: " + type);
                    break;
            }
        }

        /// <remarks> Must be called before ShowPopup </remarks>
        public void SetArguments(PopupType type, Dictionary<string, object> arguments)
        {
            ActivePopup = type;
            switch (type)
            {
                case PopupType.Error:
                    SetArguments(error, arguments);
                    break;
                case PopupType.Warning:
                    SetArguments(warn, arguments);
                    break;
                case PopupType.Info:
                    SetArguments(info, arguments);
                    break;
                default:
                    Debug.LogWarning("Unknown popup type: " + type);
                    break;
            }
        }

        public void ClosePopUp()
        {
            OnPopUpClose?.Invoke();
            HideAllPopup();
        }

        private void SetArguments(PopupDisplayer popup, Dictionary<string, object> arguments)
        {
            popup.SetArguments(arguments);
        }

        private void ActivatePopup(PopupDisplayer popup, string title, string description, params (string, Action)[] buttons)
        {
            HideAllPopup();

            popup.Title = title;
            popup.Description = description;
            popup.SetButtons(buttons);

            popup.gameObject.SetActive(true);

            activePopUps.Add(popup);
        }

        private void HideAllPopup()
        {
            foreach(var p in activePopUps)
                p.gameObject.SetActive(false);

            activePopUps.Clear();


            //GameObject[] allPopups = { error.gameObject, warn.gameObject, info.gameObject };

            //foreach (GameObject p in allPopups)
            //    p.SetActive(false);
        }
    }

}
