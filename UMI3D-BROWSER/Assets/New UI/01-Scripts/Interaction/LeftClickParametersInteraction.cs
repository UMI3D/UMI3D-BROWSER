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
using umi3d.baseBrowser.Controller;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.common.interaction;
using UnityEngine;

public class LeftClickParametersInteraction : MonoBehaviour
{
    public event Action<List<AbstractParameterDto>> OnClicked;

    private List<AbstractParameterDto> _parameters;

    private void Awake()
    {
        _parameters = new List<AbstractParameterDto>();
    }

    private void OnEnable()
    {
        BaseController.Instance.OnAddParameter += AddParameter;
        BaseController.Instance.OnRelease += Release;

        KeyboardShortcut.AddDownListener(ShortcutEnum.DisplayHideContextualMenu, OnClick);
    }

    private void OnDisable()
    {
        BaseController.Instance.OnAddParameter -= AddParameter;
        BaseController.Instance.OnRelease -= Release;

        KeyboardShortcut.RemoveDownListener(ShortcutEnum.DisplayHideContextualMenu, OnClick);
    }

    private void AddParameter(AbstractParameterDto dto)
    {
        _parameters.Add(dto);
    }

    private void Release()
    {
        _parameters.Clear();
    }

    private void OnClick()
    {
        OnClicked?.Invoke(_parameters);
    }
}
