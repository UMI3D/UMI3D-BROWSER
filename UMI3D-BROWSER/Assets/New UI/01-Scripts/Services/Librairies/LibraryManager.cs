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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using umi3d.cdk;
using umi3dBrowsers.displayer;
using UnityEngine;
using UnityEngine.Localization.Components;

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
            public long Size;

            public Library(string name, string path, long size)
            {
                Name = name;
                Path = path;
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

        [Header("Total Info")]
        [SerializeField] private LocalizeStringEvent placeTakenText;
        [SerializeField] private LocalizeStringEvent numberWorldText;

        [Header("Services")]
        [SerializeField] private PopupManager popupManager;

        /// <summary>
        /// Updates the content of the library list.
        /// </summary>
        public void UpdateContent()
        {
            var (worldsLibs, totalSize) = GetWorldsLibs();

            foreach (Transform child in content.transform)
                Destroy(child);

            foreach (var world in worldsLibs)
                Instantiate(worldStoragePrefab, content.transform).GetComponent<WorldStorageDisplayer>().SetWorld(world, totalSize);

            SetupTotalInfo(worldsLibs, totalSize);
        }

        private void SetupTotalInfo(List<WorldLibs> worldsLibs, long totalSize)
        {
            var totalInfoArguments = new Dictionary<string, object>() {
                { "worldCount", worldsLibs.Count } ,
                { "placeTaken", totalSize }
            };
            placeTakenText.StringReference.Arguments = new object[] { totalInfoArguments };
            numberWorldText.StringReference.Arguments = new object[] { totalInfoArguments };
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
                    worldLib.Libraries.Add(new Library(lib.key, lib.path, libSize));
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