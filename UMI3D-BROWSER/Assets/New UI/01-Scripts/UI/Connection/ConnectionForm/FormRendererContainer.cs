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
using umi3d.common.interaction;
using umi3dBrowsers.linker;
using umi3dBrowsers.linker.ui;
using UnityEngine;

namespace umi3dBrowsers.container.formrenderer
{
    /// <summary>
    /// This class handles the dispaying of the ConnectionForms
    /// </summary>
    /// You should used this monobehaviour on its own panel because its going to instantiate many prefabs. 
    /// If you wish you can add multiple Synamic server Container in your scene, just make sure they are not on the 
    /// same gameobject.
    /// The class uses the OnFormAnswer event to send back the form, you should subscribe to this event.
    public class FormRendererContainer : MonoBehaviour
    {
        [Header("params")]
        [SerializeField] private GameObject paramRoot;

        [SerializeField] private WaitRenderer waitRenderer;

        [Header("Linkers")]
        [SerializeField] private ConnectionServiceLinker connectionServiceLinker;
        [SerializeField] private MenuNavigationLinker menuNavigationLinker;

        private void Awake()
        {
            connectionServiceLinker.OnWaitReceived += HandleWait;
        }

        public void HandleWait(WaitConnectionDto connectionFormDto)
        {
            menuNavigationLinker.SetCancelButtonActive(true);
            waitRenderer.Init(paramRoot);
            waitRenderer.CleanContent();
            waitRenderer.Handle(connectionFormDto);
        }
    }
}

