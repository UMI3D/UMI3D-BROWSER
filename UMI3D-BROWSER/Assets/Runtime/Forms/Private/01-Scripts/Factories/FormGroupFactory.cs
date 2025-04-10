/*
Copyright 2019 - 2025 Inetum

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

using umi3d.common.interaction.form;
using UnityEngine;

namespace umi3d.browserRuntime.forms
{
    [ExecuteAlways]
    public class FormGroupFactory : MonoBehaviour
    {
        [SerializeField] private GameObject _groupPrefab;

        public GameObject CreateGroup(GroupDto groupDto, Transform parent)
        {
            var style = groupDto.GetStyle();

            var groupeGameObject = Instantiate(_groupPrefab);
            groupeGameObject.transform.SetParent(parent, false);

            var formItemModelContainer = groupeGameObject.GetComponent<FormItemModelContainer>();

            formItemModelContainer.Model.SetPosition(style.Position);
            formItemModelContainer.Model.SetSize(style.Size);
            formItemModelContainer.Model.SetAnchor(style.AnchorMin, style.AnchorMax, style.Pivot);
            formItemModelContainer.Model.SetTextStyle(style.FontSize, style.FontColor, style.FontStyles, style.FontAlignmentOptions);

            return groupeGameObject;
        }
    }
}