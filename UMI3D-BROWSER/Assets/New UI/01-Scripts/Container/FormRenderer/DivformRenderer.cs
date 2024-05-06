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
using System.Threading.Tasks;
using umi3d.cdk;
using umi3d.common.interaction.form;
using umi3d.common.interaction.form.ugui;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.container.formrenderer
{
    public class DivformRenderer : MonoBehaviour
    {
        private GameObject _contentRoot;
        private FormAnswerDto _answer;
        public event Action<FormAnswerDto> OnFormAnswer;
        List<GameObject> allContainers = new();

        [Header("UI Containers")]
        [SerializeField] private GameObject vignetteContainerPrefab;
        [SerializeField] private GameObject groupContainerPrefab;

        [Space]
        [SerializeField] private TabManager tabManager;

        [Header("UI Displayers")]
        [SerializeField] private GameObject labelDisplayerPrefab;
        [SerializeField] private GameObject buttonDisplayerPrefab;
        [SerializeField] private GameObject imageDisplayerPrefab;

        FormDto _form;

        public void Init(GameObject contentRoot)
        {
            this._contentRoot = contentRoot;
        }

        /// <summary>
        /// Makes sure that there is no trace of anything left in the form
        /// </summary>
        /// <param name="id"></param>
        internal void CleanContent(ulong id)
        {
            IniFormAnswer(id);
            tabManager.Clear();

            float delay = 0;
            for (int i = 1; i < allContainers.Count; i++)
            {
                if (allContainers[i] == null) continue;
#if UNITY_EDITOR
                DestroyImmediate(allContainers[i]);
#else
                Destroy(allContainers[i], delay);
                delay += 0.01f;
#endif
            }
            allContainers = new();
        }

        /// <summary>
        /// Reads and instantiate the connection form dto 
        /// </summary>
        /// <param name="connectionFormDto"></param>
        internal void Handle(ConnectionFormDto connectionFormDto)
        {
            allContainers.Add(_contentRoot);
            EvaluateDiv(connectionFormDto, 0);
        }

        /// <summary>
        /// Entry point to parse the connection form dto
        /// Evaluate each div, one after another
        /// </summary>
        /// <param name="divParent"></param>
        /// <param name="parentId"></param>
        private void EvaluateDiv(DivDto divParent, int parentId)
        {
            InstantiateDiv(divParent, parentId);
        }

        private void InstantiateDiv(DivDto divParent, int parentId)
        {
            switch (divParent)
            {
                case BaseInputDto inputDto:
                    HandleInputDto(inputDto, parentId); break;
                case FormDto inputDto:
                    HandleFormDto(inputDto); break;
                case PageDto pageDto:
                    HandlePageDto(pageDto); break;
                case LabelDto labelDto:
                    HandleLabelDto(labelDto, parentId); break;
                case ImageDto imageDto:
                    HandleImageDto(imageDto, parentId); break;
            }
        }

        private void HandleInputDto(BaseInputDto inputDto, int parentId)
        {
            switch (inputDto)
            {
                case GroupDto groupDto:
                    HandleGroupDto(groupDto, parentId); break;
                case ButtonDto buttonDto:
                    HandleButtonDto(buttonDto, parentId); break;
            }
        }

        private void HandleStyle(List<StyleDto> styles, GameObject go, IDisplayer displayer)
        {
            if (styles != null && go != null)
            {
                for (int i = 0; i < styles.Count; i++)
                {
                    ApplyStyle(go, styles[i], displayer);
                }
            }
        }

        private void HandleGroupDto(GroupDto groupDto, int parentId)
        {
            GameObject group = Instantiate(groupContainerPrefab, allContainers[parentId].transform);
            allContainers.Add(group);
            int currentId = allContainers.Count - 1;
            foreach (var div in groupDto.FirstChildren)
            {
                EvaluateDiv(div, currentId);
            }

            HandleStyle(groupDto.styles, group, null);
        }

        private void HandleButtonDto(ButtonDto buttonDto, int parentId)
        {

        }
        private void HandleFormDto(FormDto formDto)
        {
            _form = formDto;
            foreach(var div in formDto.FirstChildren)
            {
                EvaluateDiv(div, 0);
            }

            HandleStyle(formDto.styles, null, null);
        }
        private void HandlePageDto(PageDto pageDto)
        {
            allContainers.Add(tabManager.AddNewTab(pageDto.name));
            int currentId = allContainers.Count - 1;
            foreach (var div in pageDto.FirstChildren)
            {
                EvaluateDiv(div, currentId);
            }

            HandleStyle(pageDto.styles, null, null);
        }
        private void HandleLabelDto(LabelDto labelDto, int parentId)
        {
            GameObject labelGo = null;
            IDisplayer displayer = null;

            labelGo = Instantiate(labelDisplayerPrefab, allContainers[parentId].transform);
            displayer = labelGo.GetComponent<IDisplayer>();

            displayer.SetTitle(labelDto.text);

            HandleStyle(labelDto?.styles, labelGo, displayer);
        }
        /// <summary>
        /// Image and loads the resource
        /// </summary>
        /// <param name="imageDto"></param>
        /// <param name="parentId"></param>
        private async Task HandleImageDto(ImageDto imageDto, int parentId)
        {
            GameObject imageGO = null;
            IDisplayer displayer = null;
            if (imageDto.FirstChildren.Count == 0) // Simple image
            {
                imageGO = Instantiate(imageDisplayerPrefab, allContainers[parentId].transform);
                displayer = imageGO.GetComponent<IDisplayer>();
                if (imageDto.resource != null) 
                    try
                    {
                        object spriteTask = await UMI3DResourcesManager.Instance._LoadFile(0,
                            imageDto.resource.variants[0],
                            new ImageDtoLoader()
                        );

                        Texture2D texture = spriteTask as Texture2D;
                        displayer.SetResource(
                            Sprite.Create(texture,
                                new Rect(0, 0, texture.Size().x, texture.Size().y),
                                new Vector2())
                        );
                    }
                    catch(Exception ex)
                    {
                        Debug.LogException(new Exception("Make sure you are in play mode to load resource in the form," +
                            " or that every networking UMI3D behaviours are ready"), this);
                    }
            }
            else // Vignette
            {
                imageGO = Instantiate(vignetteContainerPrefab, allContainers[parentId].transform);
            }

            HandleStyle(imageDto.styles, imageGO, displayer);
        }

        private void IniFormAnswer(ulong id)
        {
            _answer = new FormAnswerDto()
            {
                formId = id,

                inputs = new()
            };
        }

        private void ApplyStyle(GameObject go, StyleDto styleDto, IDisplayer displayer)
        {
            if (styleDto.variants != null)
            {
                UGUIStyleVariantDto styleVariant = null;
                for (int i = 0; i < styleDto.variants.Count;  i++)
                {
                    if (styleDto.variants[i] is UGUIStyleVariantDto style)
                    {
                        styleVariant = style;
                        break;
                    }
                }

                if (styleVariant != null)
                {
                    for (int i = 0; i < styleVariant.StyleVariantItems.Count; i++)
                    {
                        switch (styleVariant.StyleVariantItems[i]) 
                        { 
                            case PositionStyleDto positionStyleVariant :
                                {
                                    RectTransform rect = go.GetComponent<RectTransform>();
                                    Vector2 rectPos = new Vector2(positionStyleVariant.posX, positionStyleVariant.posY);
                                    rect.anchoredPosition = rectPos;
                                    break;
                                }
                            case SizeStyleDto sizeStyleVariant :
                                {
                                    RectTransform rect = go.GetComponent<RectTransform>();
                                    Vector2 size = new Vector2(sizeStyleVariant.width, sizeStyleVariant.height);
                                    rect.sizeDelta = size;  
                                    break;
                                }
                            case AnchorStyleDto anchorStyleVariant :
                                {
                                    RectTransform rect = go.GetComponent<RectTransform>();
                                    Vector2 anchorMax = new Vector2(anchorStyleVariant.maxX, anchorStyleVariant.maxY);
                                    Vector2 anchorMin = new Vector2(anchorStyleVariant.minX, anchorStyleVariant.minY);
                                    Vector2 pivot = new Vector2(anchorStyleVariant.pivotX, anchorStyleVariant.pivotY);

                                    rect.anchorMax = anchorMax;
                                    rect.anchorMin = anchorMin;
                                    rect.pivot = pivot;
                                    break;
                                }
                            case ColorStyleDto colorStyleVariant :
                                {
                                    if (displayer == null)
                                    {
                                        Debug.LogWarning("Should not try to set the color of a container");
                                    }

                                    Color _color = new Color()
                                    {
                                        a = colorStyleVariant.color.A,
                                        b = colorStyleVariant.color.B,
                                        g = colorStyleVariant.color.G,
                                        r = colorStyleVariant.color.R,
                                    };

                                    displayer.SetColor(_color);

                                    break;
                                }
                            case TextStyleDto textStyleVariant :
                                {
                                    displayer.SetResource(textStyleVariant);
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
