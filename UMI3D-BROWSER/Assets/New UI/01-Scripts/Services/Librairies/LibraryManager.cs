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

using Pico.Platform;
using System.Collections.Generic;
using umi3d.cdk;
using umi3dBrowsers.data.ui;
using umi3dBrowsers.displayer;
using umi3dBrowsers.linker.ui;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.services.librairies
{
    /// <summary>
    /// Manages the content of the library manager menu.
    /// </summary>
    public class LibraryManager : MonoBehaviour
    {
        [SerializeField] private PopupLinker popupLinker;
        [SerializeField] private PopupData deleteLibPopup;
        [SerializeField] private SimpleButton buttonUp;
        [SerializeField] private SimpleButton buttonDown;

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

            var libs = new Dictionary<string, List<UMI3DResourcesManager.DataFile>>();

            foreach (UMI3DResourcesManager.DataFile lib in UMI3DResourcesManager.Libraries)
            {
                if (lib.applications != null)
                    foreach (string app in lib.applications)
                    {
                        if (!libs.ContainsKey(app)) libs[app] = new List<UMI3DResourcesManager.DataFile>();
                        libs[app].Add(lib);
                    }
            }

            foreach (KeyValuePair<string, List<UMI3DResourcesManager.DataFile>> app in libs)
            {
                foreach (UMI3DResourcesManager.DataFile lib in app.Value)
                {
                    // 1. Diplay lib name
                    LibraryManagerEntry entry = Instantiate(libraryItemPrefab, container.transform).GetComponent<LibraryManagerEntry>();
                    if (entry == null)
                        throw new System.ArgumentException("libraryItemPrefab must have a LibraryManagerEntry script");

                    entry.gameObject.name = "LibraryItem_" + lib.key;
                    entry.LibLabel.text = lib.key;

                    //2. Display environments which use this lib
                    //Could be done with lib.applications if needed;

                    //3. Display lib size
                    //Could be done with lib.path if needed

                    //4.Bind the button to unistall this lib
                    entry.DeleteButton.onClick.AddListener(() =>
                    {
                        popupLinker.SetArguments(deleteLibPopup, new() { { "libName", lib.key } });
                        popupLinker.Show(deleteLibPopup, "empty", "popup_deleteLib_description",
                            ("popup_cancel", () => popupLinker.CloseAll()),
                            ("popup_yes", () => {
                                lib.applications.Remove(app.Key);
                                UMI3DResourcesManager.RemoveLibrary(lib.library);
                                UpdateContent();
                                popupLinker.CloseAll();
                            }
                        )
                        );
                    });

                    currentEntries.Add(entry);
                }
            }

            indexOfCurrentTopEntryDisplayed = 0;
            UpdateDisplay();
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