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

using inetum.unityUtils;
using System.Collections.Generic;
using TMPro;
using umi3d.browserRuntime.libraries;
using umi3d.browserRuntime.notificationKeys;
using umi3d.browserRuntime.ui.popup;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.services.librairies
{
    /// <summary>
    /// Manages the content of the library manager menu.
    /// </summary>
    public class LibraryManager : MonoBehaviour
    {
        [SerializeField] private Button buttonUp;
        [SerializeField] private Button buttonDown;
        [SerializeField] private SimpleButton buttonDeleteAll;
        [SerializeField] private TMP_Text textNoLib;

        /// <summary>
        /// Prefab used to represent a library in the menu.
        /// 
        /// Must have a LibraryManagerEntry script.
        /// </summary>
        public GameObject libraryItemPrefab;

        /// <summary>
        /// Library list.
        /// </summary>
        public VerticalLayoutGroup container;

        [SerializeField]
        [Tooltip("Maximum number of libraries visible at the same time")]
        private int nbLibraryDisplayedAtSameTime = 5;

        /// <summary>
        /// List of all current <see cref="LibraryManagerEntry"/>
        /// </summary>
        private List<LibraryManagerEntry> currentEntries = new List<LibraryManagerEntry>();

        /// <summary>
        /// Index of the first <see cref="LibraryManagerEntry"/>
        /// </summary>
        private int indexOfCurrentTopEntryDisplayed = 0;

        private Notifier popupCloseAll;

        const string POPUP_TABLE = "BrowserPopups";
        PopupNotifier popupNotifier;

        private void Awake()
        {
            popupNotifier = new(this);
            buttonDeleteAll.OnClick.AddListener(DeleteAllLibClick);
        }

        private void DeleteAllLibClick()
        {
            popupNotifier
                .enqueue
                .SetType(PopupType.Warning)
                .SetArguments(("count", currentEntries.Count))
                .SetTitle(POPUP_TABLE, "warningDeleteAllLibs")
                .SetDescription(POPUP_TABLE, "warningDeleteAllLibs_message")
                .SetButtons((POPUP_TABLE, "warningDeleteLib_buttonCancel"), (POPUP_TABLE, "warningDeleteLib_buttonDelete"))
                .SetButtonsAction(index =>
                {
                    if (index == 1)
                    {
                        foreach (var entry in currentEntries)
                        {
                            entry.Delete();
                        }
                        NotificationHub.Default.Notify(this, LibraryNotificationKeys.LibraryDeleted);
                        UpdateContent();
                    }
                })
                .Notify();
        }

        private void OnEnable()
        {
            UpdateContent();
        }

        /// <summary>
        /// Updates the content of the library list.
        /// </summary>
        public void UpdateContent()
        {
            foreach (LibraryManagerEntry entry in currentEntries)
                Destroy(entry.gameObject);
            currentEntries.Clear();

            foreach (UMI3DResourcesManager.DataFile lib in UMI3DResourcesManager.Libraries)
            {
                // 1. Display lib name
                LibraryManagerEntry entry = Instantiate(libraryItemPrefab, container.transform).GetComponent<LibraryManagerEntry>();
                if (entry == null)
                    throw new System.ArgumentException("libraryItemPrefab must have a LibraryManagerEntry script");

                entry.gameObject.name = "LibraryItem_" + lib.key;
                entry.LibLabel.text = lib.key + " " + lib.version;

                //2. Display environments which use this lib
                //Could be done with lib.applications if needed;

                //3. Display lib size
                //Could be done with lib.path if needed

                //4.Bind the button to uninstall this lib
                entry.DeleteLib += () =>
                {
                    UMI3DResourcesManager.RemoveLibrary(lib.library);
                };
                entry.DeleteButton.onClick.AddListener(() =>
                {
                    popupNotifier
                        .enqueue
                        .SetType(PopupType.Warning)
                        .SetArguments(("lib", lib.key))
                        .SetTitle(POPUP_TABLE, "warningDeleteLib")
                        .SetDescription(POPUP_TABLE, "warningDeleteLib_message")
                        .SetButtons((POPUP_TABLE, "warningDeleteLib_buttonCancel"), (POPUP_TABLE, "warningDeleteLib_buttonDelete"))
                        .SetButtonsAction(index =>
                        {
                            if (index == 1)
                            {
                                entry.Delete();
                                NotificationHub.Default.Notify(this, LibraryNotificationKeys.LibraryDeleted);
                                UpdateContent();
                            }
                        })
                        .Notify();
                });

                currentEntries.Add(entry);
            }

            indexOfCurrentTopEntryDisplayed = 0;
            UpdateDisplay();

            buttonDeleteAll.gameObject.SetActive(UMI3DCollaborationClientServer.Environement == null && currentEntries.Count > 0);
            textNoLib.gameObject.SetActive(currentEntries.Count == 0);
        }

        /// <summary>
        /// Navigates up in the library list.
        /// </summary>
        public void NavigateUp()
        {
            if (indexOfCurrentTopEntryDisplayed > 0)
            {
                indexOfCurrentTopEntryDisplayed--;
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Navigates down in the library list.
        /// </summary>
        public void NavigateDown()
        {
            if ((indexOfCurrentTopEntryDisplayed + nbLibraryDisplayedAtSameTime < currentEntries.Count - 1))
            {
                indexOfCurrentTopEntryDisplayed++;
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Displays the libraries which have an index between [indexOfCurrentTopEntryDisplayed; indexOfCurrentTopEntryDisplayed + nbLibraryDisplayedAtSameTime].
        /// </summary>
        private void UpdateDisplay()
        {
            for (int i = 0; i < currentEntries.Count; i++)
            {
                bool display = (i >= indexOfCurrentTopEntryDisplayed) && (i <= indexOfCurrentTopEntryDisplayed + nbLibraryDisplayedAtSameTime);
                currentEntries[i].gameObject.SetActive(display);
            }
            buttonUp.gameObject.SetActive(currentEntries.Count > nbLibraryDisplayedAtSameTime);
            buttonDown.gameObject.SetActive(currentEntries.Count > nbLibraryDisplayedAtSameTime);
        }
    }
}