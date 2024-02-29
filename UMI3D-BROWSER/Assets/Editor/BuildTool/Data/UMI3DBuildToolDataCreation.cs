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
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    public static class UMI3DBuildToolDataCreation 
    {
        public static string path;
        public static string ExcludedPath
        {
            get
            {
                return path + "EXCLUDED";
            }
        }
        public static List<string> filesPath;

        public static void GetPath()
        {
            // Get the relative path of this file from the Assets folder.
            string fileName = $"{nameof(UMI3DBuildToolDataCreation)}";
            var assets = AssetDatabase.FindAssets($"t:Script {fileName}");
            path = AssetDatabase.GUIDToAssetPath(assets[0]);

            // Remove script name with extension.
            path = path.Substring(0, path.Length - fileName.Length - 3);
        }

        public static void CreateExcludedFolderIfNecessary()
        {
            // Add EXCLUDED folder.
            if (!Directory.Exists(ExcludedPath))
            {
                Directory.CreateDirectory(ExcludedPath);
            }
        }

        public static void GetFiles()
        {
            filesPath = Directory.GetFiles(
                ExcludedPath,
                "*.asset",
                SearchOption.TopDirectoryOnly
            ).ToList();
        }

        public static SO GetSO<SO>(string name)
            where SO: ScriptableObject
        {
            bool Contains(string fileName)
            {
                var file = filesPath.Find(file =>
                {
                    return file.Contains(fileName);
                });

                return !string.IsNullOrEmpty(file);
            }

            SO so = null;
            if (!Contains(name))
            {
                UnityEngine.Debug.Log($"message");
                so = ScriptableObject.CreateInstance<SO>();
                AssetDatabase.CreateAsset(so, ExcludedPath + $"/UMI3D Build Tool {name}.asset");
            }
            else
            {
                so = AssetDatabase.LoadAssetAtPath<SO>(ExcludedPath + $"/UMI3D Build Tool {name}.asset");
            }

            return so;
        }

        public static void SaveAndRefresh()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}