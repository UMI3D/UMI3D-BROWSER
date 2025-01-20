/*
Copyright 2019 - 2022 Inetum

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

using inetum.unityUtils.lifeCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk.menu;
using umi3dBrowsers.linker;
using umi3dVRBrowsersBase.connection;
using umi3dVRBrowsersBase.interactions;
using umi3dVRBrowsersBase.settings;
using umi3dVRBrowsersBase.ui;
using umi3dVRBrowsersBase.ui.playerMenu;
using UnityEngine;

namespace umi3dBrowsers.player
{
    /// <summary>
    /// This class manages the display and items of all UMI3DWatches.
    /// </summary>
    public class WatchMenu : MonoBehaviour
    {
        #region Static Fields and Methods

        /// <summary>
        /// Stores all instances.
        /// </summary>
        public static HashSet<WatchMenu> instances = new HashSet<WatchMenu>();

        /// <summary>
        /// If exists returns the first <see cref="WatchMenu"/> associated to <paramref name="controller"/>.
        /// Warning : this method convert an Hashset to an array so do not used it each frame.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static WatchMenu FindInstanceAssociatedToController(ControllerType controller)
        {
            return Array.Find(instances.ToArray(), w => w.associatedController == controller);
        }

        #endregion

        #region Fields

        [SerializeField]
        [Tooltip("Controller associated to this menu")]
        private ControllerType associatedController;

        [Header("Wristband buttons")]
        [SerializeField]
        [Tooltip("Button to display all menus pinned by users")]
        OnOffButton EmoteBtn;

        [Header("Settings menu")]
        [SerializeField]
        [Tooltip("Root of the settings menu")]
        GameObject settingsMenuRoot;
        [SerializeField]
        [Tooltip("Button to enable/disable microphone")]
        OnOffButton MicBtn;
        [SerializeField]
        [Tooltip("Button to enable/disable sound")]
        OnOffButton SoundBtn;

        [Header("Linkers")]
        [SerializeField] private ConnectionToImmersiveLinker linker;
        [SerializeField] private DialogBox dialogBox;

        /// <summary>
        /// Transform of the player camera.
        /// </summary>
        private Transform playerCamera;

        /// <summary>
        /// Angle to considerer this wath in the field of view of the player
        /// </summary>
        private float detectionConeAngle = 20f;

        /// <summary>
        /// Height of the detection cone to consider this watch inside the player's field of view.
        /// </summary>
        private float detectionConeDistance = 1f;

        /// <summary>
        /// Is the menu open ?
        /// </summary>
        public bool IsOpen { get; protected set; } = false;

        public Transform notificationContainer;

        #endregion


        #region Monobehavior's callback

        private void Awake()
        {
            instances.Add(this);
            settingsMenuRoot.SetActive(false);
        }

        private void Start()
        {

            EnvironmentSettings.Instance.micSetting.OnValueChanged.AddListener(MicBtn.Toggle);
            EnvironmentSettings.Instance.audioSetting.OnValueChanged.AddListener(SoundBtn.Toggle);
            SetMicStatus(false);

            EmoteMenu.EmoteButtonStatusChanged += value =>
            {
                EmoteBtn.Toggle(value);
                IsOpen = value;
            };

            playerCamera = PlayerMenuManager.Instance.PlayerCameraTransform;
        }

        private void OnDestroy()
        {
            instances.Remove(this);
        }


        #endregion

        public void ToggleDisplayEmote()
        {
            if (IsOpen)
            {
                EmoteMenu.Instance.Hide();
            }
            else
            {
                EmoteMenu.Instance.Display();
            }
        }

        #region Setting Menu

        private bool isSettingsMenuOpened = false;
        /// <summary>
        /// Toggles the display of the menu to change environment settings.
        /// </summary>
        public void ToggleSettingsMenuDisplay()
        {
            if (isSettingsMenuOpened)
            {
                isSettingsMenuOpened = false;
                settingsMenuRoot.SetActive(false);
            }
            else
            {
                isSettingsMenuOpened = true;
                settingsMenuRoot.SetActive(true);
            }
        }

        /// <summary>
        /// Asks to change the microphone status.
        /// </summary>
        /// <param name="val"></param>
        public void SetMicStatus(bool val)
        {
            EnvironmentSettings.Instance.micSetting.SetValue(val);
        }

        /// <summary>
        /// Asks the sound activation/deactivation.
        /// </summary>
        /// <param name="val"></param>
        public void SetSoundStatus(bool val)
        {
            EnvironmentSettings.Instance.audioSetting.SetValue(val);
        }

        /// <summary>
        /// Leaves the application or the environement.
        /// </summary>
        [ContextMenu("Leave")]
        public void Leave()
        {
            System.Action<bool> leaveCallback = (b) =>
            {
                if (b)
                {
                    if (EnvironmentSettings.Instance.IsEnvironmentLoaded)
                    {
                        //Connecting.Instance.Leave();
                        linker.Leave();
                    }
                    else
                        Quitting.instance.Quit(this);
                }
            };

            string title = EnvironmentSettings.Instance.IsEnvironmentLoaded ? "Go back to main menu" : "Close application";
            dialogBox.Display(title, "Are you sure you want to leave ?", "Yes", leaveCallback);
        }

        #endregion

        /// <summary>
        /// Is this object considered in the player's field of view ? Based on a detection cone defined by <see cref="detectionConeAngle"/> and <see cref="detectionConeDistance"/>.
        /// </summary>
        public bool IsObjectInPlayerFieldOfView()
        {
            Vector3 coneDirection = playerCamera.forward;
            Vector3 cameraToWatch = this.transform.position - playerCamera.position;

            if (cameraToWatch.magnitude > detectionConeDistance)
                return false;

            if (Vector3.Angle(coneDirection, cameraToWatch) < detectionConeAngle)
                return true;
            else
                return false;
        }
    }
}
