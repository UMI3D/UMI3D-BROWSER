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
using umi3d.common.interaction.form;
using umi3d.common.interaction.form.ugui;
using umi3dBrowsers.linker;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.container.formrenderer
{
    public class DivformRenderer : MonoBehaviour
    {
        private FormContainer _contentRoot;

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

        [SerializeField] private ConnectionServiceLinker connectionServiceLinker;

        FormDto _form;

        List<Action> formBinding = new();
        private FormAnswerDto _answer;
        public event Action<FormAnswerDto> OnFormAnswer;

        public void Init(GameObject contentRoot)
        {
            FormContainer container = new FormContainer();
            container.container = contentRoot;
            this._contentRoot = container;

            OnFormAnswer += connectionServiceLinker.SendDivFormAnswer;
        }

        /// <summary>
        /// Makes sure that there is no trace of anything left in the form
        /// </summary>
        /// <param name="id"></param>
        internal void CleanContent(ulong id)
        {
            InitFormAnswer(id);
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
            tabManager.InitSelectedButtonById();
        }

        /// <summary>
        /// Entry point to parse the connection form dto
        /// Evaluate each div, one after another
        /// </summary>
        /// <param name="divParent"></param>
        /// <param name="parentId"></param>
        private void InstantiateDiv(DivDto divParent, FormContainer parentContainer)
        {
            var inputAnswer = new InputAnswerDto() {
                inputId = divParent.id
            };

            switch (divParent)
            {
                case BaseInputDto inputDto:
                    HandleInputDto(inputDto, parentContainer, inputAnswer); break;
                case FormDto inputDto:
                    HandleFormDto(inputDto, parentContainer, inputAnswer); break;
                case PageDto pageDto:
                    HandlePageDto(pageDto, parentContainer, inputAnswer); break;
                case LabelDto labelDto:
                    HandleLabelDto(labelDto, parentContainer, inputAnswer); break;
                case ImageDto imageDto:
                    HandleImageDto(imageDto, parentContainer, inputAnswer); break;
            }
        }

        private void HandleInputDto(BaseInputDto inputDto, FormContainer parentContainer, InputAnswerDto inputAnswerDto)
        {
            switch (inputDto)
            {
                case GroupDto groupDto:
                    HandleGroupDto(groupDto, parentContainer, inputAnswerDto); break;
                case ButtonDto buttonDto:
                    HandleButtonDto(buttonDto, parentContainer, inputAnswerDto); break;
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

        private void HandleGroupDto(GroupDto groupDto, FormContainer parentContainer, InputAnswerDto inputAnswerDto)
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

        private void HandleButtonDto(ButtonDto buttonDto, FormContainer parentContainer, InputAnswerDto inputAnswerDto)
        {
            GameObject buttonGo = null;
            IDisplayer displayer = null;

            buttonGo = Instantiate(labelDisplayerPrefab, parentContainer.container.transform);
            parentContainer.contents.Add(buttonGo);

            displayer = buttonGo.GetComponent<IDisplayer>();

            displayer.SetTitle(buttonDto.Text);

            var button = buttonGo.GetComponent<Button>();
            switch (buttonDto.buttonType)
            {
                case ButtonType.Submit:
                    _answer.submitId = inputAnswerDto.inputId;
                    button.onClick.AddListener(() => { ValidateForm(); });
                    break;
                case ButtonType.Cancel:
                    button.onClick.AddListener(() => { _answer.isCancelation = true; ValidateForm(); });
                    break;
                case ButtonType.Back:
                    button.onClick.AddListener(() => { _answer.isBack = true; ValidateForm(); });
                    break;
            }

            HandleStyle(buttonDto?.styles, buttonGo, displayer);
        }

        private void HandleFormDto(FormDto formDto, FormContainer parentContainer, InputAnswerDto inputAnswerDto)
        {
            _form = formDto;
            foreach(var div in formDto.FirstChildren)
            {
                InstantiateDiv(div, parentContainer);
            }

            HandleStyle(formDto.styles, null, null);
        }
        private void HandlePageDto(PageDto pageDto, FormContainer parentContainer, InputAnswerDto inputAnswerDto)
        {
            FormContainer container = parentContainer.GetNextFormContainer(tabManager.AddNewTab(pageDto.name));
            allContainers.Add(container);

            formBinding.Add(() => {
                if (container.container.activeInHierarchy)
                    _answer.pageId = inputAnswerDto.inputId;
            });

            int currentId = allContainers.Count - 1;
            foreach (var div in pageDto.FirstChildren)
            {
                InstantiateDiv(div, container);
            }

            HandleStyle(pageDto.styles, null, null);
        }
        private void HandleLabelDto(LabelDto labelDto, FormContainer parentContainer, InputAnswerDto inputAnswerDto)
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
        private async Task HandleImageDto(ImageDto imageDto, FormContainer parentContainer, InputAnswerDto inputAnswerDto)
        {
            GameObject imageGO = null;
            IDisplayer displayer = null;

            if (imageDto.FirstChildren.Count == 0) // Simple image
            {
                imageGO = Instantiate(imageDisplayerPrefab, parentContainer.container.transform);
                parentContainer.contents.Add(imageGO);

                displayer = imageGO.GetComponent<IDisplayer>();
                displayer.SetResource(await imageDto.GetSprite());
            }
            else // Vignette
            {
                VignetteContainer vignetteContainer = parentContainer.container.GetComponent<VignetteContainer>();
                if (vignetteContainer == null)
                {
                    FormContainer newParent = parentContainer.ReplaceContainerWithPrefab(vignetteContainerPrefab);
                    vignetteContainer = newParent.container.GetComponent<VignetteContainer>();
                    //vignetteContainer.Clear();
                }

                //await vignetteContainer.CreateVignetteFromForm(imageDto);
            }

            HandleStyle(imageDto.styles, imageGO, displayer);
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

        [ContextMenu("Validate form ")]
        public void ValidateForm()
        {
            formBinding.ForEach(action => action?.Invoke());
            OnFormAnswer?.Invoke(_answer);
        }

        private void InitFormAnswer(ulong id)
        {
            _answer = new FormAnswerDto() {
                formId = id,

                inputs = new()
            };
        }
    }
}
