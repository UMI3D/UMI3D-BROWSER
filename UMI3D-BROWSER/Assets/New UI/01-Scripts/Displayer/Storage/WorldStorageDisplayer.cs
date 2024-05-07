/*
Copyright 2019 - 2023 Inetum

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
using System.Collections.Generic;
using TMPro;
using umi3dBrowsers.services.librairies;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace umi3dBrowsers.displayer
{
    public class WorldStorageDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text worldNameText;
        [Space]
        [SerializeField] private LocalizeStringEvent placeTakenText;
        [SerializeField] private LocalizeStringEvent placeWorldText;
        [SerializeField] private LocalizeStringEvent placeCommonText;
        [Space]
        [SerializeField] private ProgressBar uniqueProgressBar;
        [SerializeField] private ProgressBar commonProgressBar;

        public void SetWorld(LibraryManager.WorldLibs worldLibs, long totalSize)
        {
            worldNameText.text = worldLibs.Name;

            placeTakenText.StringReference.Arguments = new object[] { 
                new Dictionary<string, long>() {
                    { "worldSize", worldLibs.TotalSize },
                    { "totalSize", totalSize }
                }
            };
            placeWorldText.StringReference.Arguments = new object[] {
                new Dictionary<string, long>() {
                    { "placeTaken", worldLibs.UniqueSize },
                }
            };
            placeCommonText.StringReference.Arguments = new object[] {
                new Dictionary<string, long>() {
                    { "placeTaken", worldLibs.CommonSize },
                }
            };

            uniqueProgressBar.SetProgressBarMaxValue(1);
            float uniqueProgress = ((float)worldLibs.UniqueSize) / totalSize;
            uniqueProgressBar.SetProgressBarValue(uniqueProgress);
            commonProgressBar.SetProgressBarMaxValue(1);
            float commonProgress = ((float)worldLibs.TotalSize) / totalSize;
            commonProgressBar.SetProgressBarValue(commonProgress);
        }
    }
}
