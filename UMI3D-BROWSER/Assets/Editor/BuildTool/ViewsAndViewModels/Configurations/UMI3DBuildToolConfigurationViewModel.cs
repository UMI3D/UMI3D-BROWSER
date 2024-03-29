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
using UnityEditor;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DBuildToolConfigurationViewModel 
    {
        public UMI3DBuildToolTarget_SO buildToolTarget_SO;
        public UMI3DBuildToolKeystore_SO buildToolKeystore_SO;

        public UMI3DBuildToolConfigurationViewModel(
            UMI3DBuildToolTarget_SO buildToolTarget_SO,
            UMI3DBuildToolKeystore_SO buildToolKeystore_SO
        )
        {
            this.buildToolTarget_SO = buildToolTarget_SO;
            this.buildToolKeystore_SO = buildToolKeystore_SO;
        }

        public void UpdateBuildFolder(string path)
        {
            buildToolTarget_SO.buildFolder = path;

            for (int i = 0; i < buildToolTarget_SO.targets.Count; i++)
            {
                var target = buildToolTarget_SO.targets[i];
                target.BuildFolder = path;
                buildToolTarget_SO.targets[i] = target;
            }

            SaveBuildToolTarget();
        }

        public void BrowseBuildFolder(Action<string> updateView)
        {
            string directory = string.IsNullOrEmpty(buildToolTarget_SO.buildFolder)
                ? Application.dataPath
                : buildToolTarget_SO.buildFolder;

            string path = EditorUtility.OpenFolderPanel(
                    title: "Build Folder",
                    directory,
                    defaultName: ""
                );

            UpdateBuildFolder(path);
            updateView?.Invoke(path);
        }

        public void UpdateInstaller(string path)
        {
            buildToolTarget_SO.installer = path;

            SaveBuildToolTarget();
        }

        public void BrowseInstaller(Action<string> updateView)
        {
            string directory = string.IsNullOrEmpty(buildToolTarget_SO.installer)
                ? Application.dataPath
                : buildToolTarget_SO.installer;

            string path = EditorUtility.OpenFilePanel(
                    title: "Installer Path",
                    directory,
                    extension: "iss"
                );
            UpdateInstaller(path);
            updateView?.Invoke(path);
        }

        public void UpdateLicense(string path)
        {
            buildToolTarget_SO.license = path;

            SaveBuildToolTarget();
        }

        public void BrowseLicense(Action<string> updateView)
        {
            string directory = string.IsNullOrEmpty(buildToolTarget_SO.license) 
                ? Application.dataPath
                : buildToolTarget_SO.license;

            string path = EditorUtility.OpenFilePanel(
                    title: "License Path",
                    directory,
                    extension: "txt"
                );

            UpdateLicense(path);
            updateView?.Invoke(path);
        }

        public void SaveBuildToolTarget()
        {
            EditorUtility.SetDirty(buildToolTarget_SO);
        }

        public void UpdateKeystorePath(string path)
        {
            buildToolKeystore_SO.path = path;

            SaveBuildToolKeystore();
        }

        public void BrowseKeystorePath(Action<string> updateView)
        {
            string directory = string.IsNullOrEmpty(buildToolKeystore_SO.path)
                ? Application.dataPath
                : buildToolKeystore_SO.path;

            string path = EditorUtility.OpenFilePanel(
                    title: "Keystore Path",
                    directory,
                    extension: "keystore"
                );

            UpdateKeystorePath(path);
            updateView?.Invoke(path);
        }

        public void UpdateKeystorePassword(string password)
        {
            buildToolKeystore_SO.password = password;

            SaveBuildToolKeystore();
        }

        public void SaveBuildToolKeystore()
        {
            EditorUtility.SetDirty(buildToolKeystore_SO);
        }
    }
}