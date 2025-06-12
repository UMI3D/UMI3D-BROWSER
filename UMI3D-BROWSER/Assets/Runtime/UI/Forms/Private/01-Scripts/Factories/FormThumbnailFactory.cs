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

using System;
using System.Linq;
using System.Threading.Tasks;
using umi3d.browserRuntime.thumbnails;
using umi3d.browserRuntime.ui.formMenu;
using umi3d.common.interaction.form;
using UnityEngine;

namespace umi3d.browserRuntime.forms
{
    [ExecuteAlways]
    internal class FormThumbnailFactory : MonoBehaviour
    {
        [SerializeField] private ThumbnailListModelContainer _thumbnailListModelContainerPrefab;

        public async Task<GameObject> CreateThumbnail(ImageDto imageDto, FormFactory.Container container, Action<string> sendAnswer)
        {
            ThumbnailListModelContainer thumbnailListModelContainer = container.Transform.GetComponent<ThumbnailListModelContainer>();
            if (!thumbnailListModelContainer)
                thumbnailListModelContainer = FormFactory.ReplaceContainerWithPrefab(container, _thumbnailListModelContainerPrefab);

            var style = imageDto.GetStyle();

            string labelText = null;
            string headerText = null;
            Color? indicatorColor = null;

            foreach (var child in imageDto.FirstChildren)
            {
                if (child is LabelDto labelDto)
                {
                    if (labelDto.tag == null)
                        labelText = labelDto.text;
                    else
                    {
                        headerText = labelDto.text;
                        indicatorColor = labelDto.GetStyle().Color;
                    }
                }
            }

            thumbnailListModelContainer.Model.AddThumbnail(labelText,
                await imageDto.GetSprite(),
                () => sendAnswer(imageDto.guid),
                style.Color,
                style.HoverColor);

            var formThumbnailModelContainer = thumbnailListModelContainer.Model.ThumbnailContainers.Last().GetComponent<FormThumbnailModelContainer>();
            formThumbnailModelContainer.Model.SetHeader(headerText, indicatorColor);
            formThumbnailModelContainer.Model.SetIsLoading(style.IsLoading);

            return formThumbnailModelContainer.gameObject;
        }
    }
}