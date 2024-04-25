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

        public PopupType ActivePopup;

        public enum PopupType
        {
            Error,
            Warning,
            Info,
            ReportBug,
        }

        /// <param name="type"></param>
        /// <param name="title">Key for localization</param>
        /// <param name="description">Key for localization</param>
        /// <param name="buttons">Key for localization and action onClick</param>
        public void ShowPopup(PopupType type, string title, string description, params (string, Action)[] buttons)
        {
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
        }

        private void HideAllPopup()
        {
            GameObject[] allPopups = { error.gameObject, warn.gameObject, info.gameObject };

            foreach (GameObject p in allPopups)
                p.SetActive(false);
        }
    }

}
