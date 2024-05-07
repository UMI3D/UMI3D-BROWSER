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
        private FormContainer _contentRoot;
        private FormAnswerDto _answer;
        public event Action<FormAnswerDto> OnFormAnswer;

        List<FormContainer> allContainers = new();

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
            FormContainer container = new FormContainer();
            container.container = contentRoot;
            this._contentRoot = container;
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
                DestroyImmediate(allContainers[i].container);
#else
                Destroy(allContainers[i].container, delay);
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
            InstantiateDiv(connectionFormDto, _contentRoot);
        }

        /// <summary>
        /// Entry point to parse the connection form dto
        /// Evaluate each div, one after another
        /// </summary>
        /// <param name="divParent"></param>
        /// <param name="parentId"></param>
        private void InstantiateDiv(DivDto divParent, FormContainer parentContainer)
        {
            switch (divParent)
            {
                case BaseInputDto inputDto:
                    HandleInputDto(inputDto, parentContainer); break;
                case FormDto inputDto:
                    HandleFormDto(inputDto, parentContainer); break;
                case PageDto pageDto:
                    HandlePageDto(pageDto, parentContainer); break;
                case LabelDto labelDto:
                    HandleLabelDto(labelDto, parentContainer); break;
                case ImageDto imageDto:
                    HandleImageDto(imageDto, parentContainer); break;
            }
        }

        private void HandleInputDto(BaseInputDto inputDto, FormContainer parentContainer)
        {
            switch (inputDto)
            {
                case GroupDto groupDto:
                    HandleGroupDto(groupDto, parentContainer); break;
                case ButtonDto buttonDto:
                    HandleButtonDto(buttonDto, parentContainer); break;
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

        private void HandleGroupDto(GroupDto groupDto, FormContainer parentContainer)
        {
            GameObject group = Instantiate(groupContainerPrefab, parentContainer.container.transform);
            FormContainer container = parentContainer.GetNextFormContainer(group);
            allContainers.Add(container);

            foreach (var div in groupDto.FirstChildren)
            {
                InstantiateDiv(div, container);
            }

            HandleStyle(groupDto.styles, group, null);
        }

        private void HandleButtonDto(ButtonDto buttonDto, FormContainer parentContainer)
        {

        }
        private void HandleFormDto(FormDto formDto, FormContainer parentContainer)
        {
            _form = formDto;
            foreach(var div in formDto.FirstChildren)
            {
                InstantiateDiv(div, parentContainer);
            }

            HandleStyle(formDto.styles, null, null);
        }
        private void HandlePageDto(PageDto pageDto, FormContainer parentContainer)
        {
            FormContainer container = parentContainer.GetNextFormContainer(tabManager.AddNewTab(pageDto.name));
            allContainers.Add(container);

            int currentId = allContainers.Count - 1;
            foreach (var div in pageDto.FirstChildren)
            {
                InstantiateDiv(div, container);
            }

            HandleStyle(pageDto.styles, null, null);
        }
        private void HandleLabelDto(LabelDto labelDto, FormContainer parentContainer)
        {
            GameObject labelGo = null;
            IDisplayer displayer = null;

            labelGo = Instantiate(labelDisplayerPrefab, parentContainer.container.transform);
            parentContainer.contents.Add(labelGo);

            displayer = labelGo.GetComponent<IDisplayer>();

            displayer.SetTitle(labelDto.text);

            HandleStyle(labelDto?.styles, labelGo, displayer);
        }
        /// <summary>
        /// Image and loads the resource
        /// </summary>
        /// <param name="imageDto"></param>
        /// <param name="parentId"></param>
        private async Task HandleImageDto(ImageDto imageDto, FormContainer parentContainer)
        {
            GameObject imageGO = null;
            IDisplayer displayer = null;

            if (imageDto.FirstChildren.Count == 0) // Simple image
            {
                imageGO = Instantiate(imageDisplayerPrefab, parentContainer.container.transform);
                parentContainer.contents.Add(imageGO);

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
                VignetteContainer vignetteContainer = parentContainer.container.GetComponent<VignetteContainer>();
                if (vignetteContainer == null)
                {
                    FormContainer newParent = parentContainer.ReplaceContainerWithPrefab(vignetteContainerPrefab);
                    vignetteContainer = newParent.container.GetComponent<VignetteContainer>();
                }

                //Add vignette
                //imageGO = vignetteContainer
                //parentContainer.contents.Add(imageGO);

                // set the reste of the vignette
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