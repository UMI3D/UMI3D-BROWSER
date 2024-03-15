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
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using umi3d.cdk.menu.interaction;
using umi3d.common.interaction;
using umi3dVRBrowsersBase.connection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private GameObject contentRoot;

        [SerializeField] private ParamformRenderer formParamRenderer;
        [SerializeField] private DivformRenderer formDivRenderer;

        public event Action<FormAnswerDto> OnFormAnwser;
        public event Action<umi3d.common.interaction.form.FormAnswerDto> OnDivformAnswer;

        private void Start()
        {
            formParamRenderer.OnFormAnswer += (formDto) => OnFormAnwser?.Invoke(formDto);

            formDivRenderer.OnFormAnswer += (formDto) => OnDivformAnswer?.Invoke(formDto);
        }

        public void HandleParamForm(ConnectionFormDto connectionFormDto)
        {
            formParamRenderer.Init(contentRoot);
            formParamRenderer.CleanContent(connectionFormDto.id);
            formParamRenderer.Handle(connectionFormDto);
        }

        public void HandleDivForm(umi3d.common.interaction.form.ConnectionFormDto connectionFormDto)
        {
            formDivRenderer.Init(contentRoot);
            formDivRenderer.CleanContent(connectionFormDto.id);
            formDivRenderer.Handle(connectionFormDto);
        }
    }
}

