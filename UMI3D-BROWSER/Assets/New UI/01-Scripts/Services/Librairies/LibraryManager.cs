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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using umi3d.cdk;
using umi3dBrowsers.displayer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace umi3dBrowsers.services.librairies
{
    /// <summary>
    /// Manages the content of the library manager menu.
    /// </summary>
    public class LibraryManager : MonoBehaviour
    {
        public struct Library
        {
            public string Name;
            public string Path;
            public UMI3DResourcesManager.Library RessourceLib;
            public long Size;

            public Library(string name, string path, UMI3DResourcesManager.Library ressourceLib, long size)
            {
                Name = name;
                Path = path;
                RessourceLib = ressourceLib;
                Size = size;
            }
        }

        public class WorldLibs
        {
            public string Name;
            public List<Library> Libraries;
            public long TotalSize;
            public long UniqueSize;

            public WorldLibs(string name)
            {
                Name = name;
                Libraries = new ();
                TotalSize = 0;
                UniqueSize = 0;
            }

            public long CommonSize => TotalSize - UniqueSize;
        }

        [Header("Libs")]
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject worldStoragePrefab;
        [SerializeField] private Toggle selectAll;
        [SerializeField] private Button deleteButton;

        [Header("Total Info")]
        [SerializeField] private LocalizeStringEvent placeTakenText;
        [SerializeField] private LocalizeStringEvent numberWorldText;

        [Header("Services")]
        [SerializeField] private PopupManager popupManager;

        private List<WorldStorageDisplayer> worldStorageDisplayers;

        /// <summary>
        /// Updates the content of the library list.
        /// </summary>
        public void UpdateContent()
        {
            var (worldsLibs, totalSize) = GetWorldsLibs();

            foreach (Transform child in content.transform)
                Destroy(child.gameObject);

            worldStorageDisplayers = new List<WorldStorageDisplayer>();
            foreach (var world in worldsLibs)
            {
                var worldStorageDisplayer = Instantiate(worldStoragePrefab, content.transform).GetComponent<WorldStorageDisplayer>();
                worldStorageDisplayer.SetWorld(world, totalSize);
                worldStorageDisplayers.Add(worldStorageDisplayer);
            }

            selectAll.onValueChanged.AddListener((isOn) => {
                foreach (var worldStorageDisplayer in worldStorageDisplayers)
                    worldStorageDisplayer.Select(isOn);
            });

            deleteButton.onClick.AddListener(() => {
                var libs = new List<UMI3DResourcesManager.Library>();
                var names = new List<string>();
                foreach (var worldStorageDisplayer in worldStorageDisplayers)
                {
                    if (!worldStorageDisplayer.isSelected)
                        continue;
                    names.Add(worldStorageDisplayer.WorldLibs.Name);
                    foreach (var lib in worldStorageDisplayer.WorldLibs.Libraries)
                    {
                        libs.Add(lib.RessourceLib);
                    }
                }
                if (libs.Count == 0)
                    return;
                popupManager.SetArguments(PopupManager.PopupType.Warning, new() { { "libs", names } });
                popupManager.ShowPopup(PopupManager.PopupType.Warning, "empty", "popup_deleteLib_description",
                    ("popup_cancel", () => popupManager.ClosePopUp()),
                    ("popup_yes", () => {
                        foreach (var lib in libs)
                            UMI3DResourcesManager.RemoveLibrary(lib);
                        UpdateContent();
                        popupManager.ClosePopUp();
                    })
                );
            });

            SetupTotalInfo(worldsLibs, totalSize);

            selectAll.isOn = false;
        }

        private void SetupTotalInfo(List<WorldLibs> worldsLibs, long totalSize)
        {
            var totalInfoArguments = new Dictionary<string, object>() {
                { "worldCount", worldsLibs.Count } ,
                { "placeTaken", totalSize }
            };
            placeTakenText.StringReference.Arguments = new object[] { totalInfoArguments };
            placeTakenText.StringReference.RefreshString();
            numberWorldText.StringReference.Arguments = new object[] { totalInfoArguments };
            numberWorldText.StringReference.RefreshString();
        }

        private (List<WorldLibs>, long) GetWorldsLibs()
        {
            var worldsLibs = new List<WorldLibs>();
            long totalSize = 0;
            foreach (UMI3DResourcesManager.DataFile lib in UMI3DResourcesManager.Libraries)
            {
                if (lib.applications == null) // TODO : Unknown Worlds
                    continue;

                var libSize = DirectorySize(new DirectoryInfo(lib.path));
                totalSize += libSize;
                foreach (string app in lib.applications)
                {
                    if (!worldsLibs.Any(worldLibs => worldLibs.Name == app))
                        worldsLibs.Add(new WorldLibs(app));

                    var worldLib = worldsLibs.Find(worldLibs => worldLibs.Name == app);
                    worldLib.Libraries.Add(new Library(lib.key, lib.path, lib.library, libSize));
                    worldLib.TotalSize += libSize;
                    if (lib.applications.Count == 1)
                        worldLib.UniqueSize += libSize;
                }
            }

            return (worldsLibs, totalSize);
        }

        private static long DirectorySize(DirectoryInfo directoryInfo)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = directoryInfo.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = directoryInfo.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirectorySize(di);
            }
            return size;
        }
    }
}