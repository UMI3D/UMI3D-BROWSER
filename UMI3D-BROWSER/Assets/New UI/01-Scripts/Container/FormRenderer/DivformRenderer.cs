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
using umi3d.common;
using umi3d.common.interaction.form;
using umi3d.common.interaction.form.ugui;
using umi3dBrowsers.displayer;
using umi3dBrowsers.linker;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.container.formrenderer
{
    public class DivformRenderer : MonoBehaviour
    {
        private FormContainer _contentRoot;

        private List<FormContainer> allContainers = new();

        [Header("UI Containers")]
        [SerializeField] private GameObject vignetteContainerPrefab;
        [SerializeField] private GameObject groupContainerPrefab;
        [Space]
        [SerializeField] private TabManager tabManager;
        [SerializeField] private SimpleButton validationButton;

        [Header("UI Displayers")]
        [SerializeField] private GameObject labelDisplayerPrefab;
        [SerializeField] private GameObject buttonDisplayerPrefab;
        [SerializeField] private GameObject imageDisplayerPrefab;
        [SerializeField] private GameObject inputFieldDisplayerPrefab;
        [SerializeField] private GameObject rangeDisplayerPrefab;

        [SerializeField] private ConnectionServiceLinker connectionServiceLinker;
        [SerializeField] private List<GameObject> objectsToNotCleanup;
        [SerializeField] private ConnectionToImmersiveLinker connectionToImmersiveLinker;

        private FormDto _form;

        private List<Action> formBinding = new();
        private FormAnswerDto _answer;
        public event Action<FormAnswerDto> OnFormAnswer;
        private List<VignetteContainer> m_vignetteContainers = new();

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
        internal void CleanContent(string id)
        {
            InitFormAnswer(id);
            tabManager.Clear();

            foreach (var container in allContainers)
                RemoveContainer(container);

            allContainers = new();
            m_vignetteContainers = new();
        }

        private void RemoveContainer(FormContainer container)
        {
            if (container == null) 
                return;

            if (!objectsToNotCleanup.Contains(container?.container))
                DestroyImmediate(container?.container);

            foreach (var childContainer in container.childrenFormContainers)
                RemoveContainer(childContainer);
        }

        [ContextMenu("Validate form ")]
        public void ValidateForm(string submitId)
        {
            _answer.submitId = submitId;
            formBinding.ForEach(action => action?.Invoke());
            OnFormAnswer?.Invoke(_answer);
        }

        private void InitFormAnswer(string id)
        {
            _answer = new FormAnswerDto() {
                formId = id,

                inputs = new()
            };
        }

        /// <summary>
        /// Reads and instantiate the connection form dto 
        /// </summary>
        /// <param name="connectionFormDto"></param>
        internal void Handle(ConnectionFormDto connectionFormDto)
        {
            validationButton.gameObject.SetActive(false);
            validationButton.OnClick.RemoveAllListeners();

            allContainers.Add(_contentRoot);
            InstantiateDiv(connectionFormDto, _contentRoot);
            tabManager.InitSelectedButtonById();

            foreach(var  container in m_vignetteContainers)
            {
                if (container == null) continue;
                container.FillWithEmptyVignettes();
            }
        }

        private void HandleFormDto(FormDto formDto, FormContainer parentContainer, InputAnswerDto inputAnswerDto)
        {
            _form = formDto;
            foreach (var div in formDto.FirstChildren)
                InstantiateDiv(div, parentContainer);
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
                inputId = divParent.guid
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
                    HandleDivDto(labelDto, labelDisplayerPrefab, parentContainer, (itemDto, container, displayer) => {
                        displayer.SetTitle(labelDto.text);
                    }); break;
                case ImageDto imageDto:
                    HandleImageDto(imageDto, parentContainer, inputAnswer); break;
            }
        }

        private void HandleInputDto(BaseInputDto inputDto, FormContainer parentContainer, InputAnswerDto inputAnswerDto)
        {
            switch (inputDto)
            {
                case GroupDto groupDto:
                    HandleDivDto(groupDto, groupContainerPrefab, parentContainer, (itemDto, container, displayer) => {
                        allContainers.Add(container);

                        foreach (var div in groupDto.FirstChildren)
                            InstantiateDiv(div, container);
                    }); break;
                case ButtonDto buttonDto:
                    HandleDivDto(buttonDto, buttonDisplayerPrefab, parentContainer, async (itemDto, container, displayer) => {
                        displayer.SetTitle(buttonDto.Text);
                        displayer.SetResource(await buttonDto.GetSprite());

                        var button = container.container.GetComponent<Button>();
                        switch (buttonDto.buttonType)
                        {
                            case ButtonType.Submit:
                                button.onClick.AddListener(() => { ValidateForm(inputAnswerDto.inputId); });
                                break;
                            case ButtonType.Cancel:
                                button.onClick.AddListener(() => { connectionToImmersiveLinker.Leave(); });
                                break;
                            case ButtonType.Back:
                                button.onClick.AddListener(() => { _answer.isBack = true; ValidateForm(inputAnswerDto.inputId); });
                                break;
                        }
                    }); break;
                case InputDto<string> inputStringDto:
                    HandleDivDto(inputStringDto, inputFieldDisplayerPrefab, parentContainer, (itemDto, container, displayer) => {
                        HandleInputStringDto(itemDto, container, displayer as InputFieldDispayer, inputAnswerDto);
                    }); break;
                case InputDto<int> inputStringDto:
                    HandleDivDto(inputStringDto, inputFieldDisplayerPrefab, parentContainer, (itemDto, container, displayer) => {
                        HandleInputStringDto(itemDto, container, displayer as InputFieldDispayer, inputAnswerDto);
                    }); break;
                case InputDto<float> inputStringDto:
                    HandleDivDto(inputStringDto, inputFieldDisplayerPrefab, parentContainer, (itemDto, container, displayer) => {
                        HandleInputStringDto(itemDto, container, displayer as InputFieldDispayer, inputAnswerDto);
                    }); break;
                case RangeDto<int> rangeDto: // Can't merge range int and float because T can't be cast to Slider.value
                    HandleDivDto(rangeDto, inputFieldDisplayerPrefab, parentContainer, (itemDto, container, displayer) => {
                        var rangeDisplayer = displayer as SliderDisplayer;
                        rangeDisplayer.SetTitle(rangeDto.label);
                        rangeDisplayer.Slider.wholeNumbers = true;
                        rangeDisplayer.Slider.value = rangeDto.Value;
                        rangeDisplayer.Slider.minValue = rangeDto.Min;
                        rangeDisplayer.Slider.maxValue = rangeDto.Max;

                        _answer.inputs.Add(inputAnswerDto);
                        formBinding.Add(() => {
                            inputAnswerDto.value = displayer.GetValue(true);
                        });
                    }); break;
                case RangeDto<float> rangeDto:
                    HandleDivDto(rangeDto, inputFieldDisplayerPrefab, parentContainer, (itemDto, container, displayer) => {
                        var rangeDisplayer = displayer as SliderDisplayer;
                        rangeDisplayer.SetTitle(rangeDto.label);
                        rangeDisplayer.Slider.wholeNumbers = false;
                        rangeDisplayer.Slider.value = rangeDto.Value;
                        rangeDisplayer.Slider.minValue = rangeDto.Min;
                        rangeDisplayer.Slider.maxValue = rangeDto.Max;

                        _answer.inputs.Add(inputAnswerDto);
                        formBinding.Add(() => {
                            inputAnswerDto.value = displayer.GetValue(true);
                        });
                    });
                    break;
            }
        }

        private void HandleDivDto<T>(T itemDto, GameObject prefab, FormContainer parentContainer, Action<T, FormContainer, IDisplayer> handleItem) where T : DivDto
        {
            GameObject gameObject = Instantiate(prefab, parentContainer.container.transform);
            FormContainer container = parentContainer.GetNextFormContainer(gameObject, itemDto.styles);
            var displayer = gameObject.GetComponent<IDisplayer>();

            handleItem?.Invoke(itemDto, container, displayer);

            HandleStyle(gameObject, displayer, itemDto.styles);
        }

        private void HandleInputStringDto<T>(InputDto<T> itemDto, FormContainer container, InputFieldDispayer displayer, InputAnswerDto inputAnswerDto)
        {
            displayer.SetTitle(itemDto.Name);
            displayer.SetPlaceHolder(new List<string> { itemDto.PlaceHolder.ToString() });
            displayer.SetType(itemDto.TextType);

            _answer.inputs.Add(inputAnswerDto);
            formBinding.Add(() => {
                inputAnswerDto.value = displayer.GetValue(true);
            });
        }

        private void HandlePageDto(PageDto pageDto, FormContainer parentContainer, InputAnswerDto inputAnswerDto)
        {
            FormContainer container = parentContainer.GetNextFormContainer(tabManager.AddNewTab(pageDto.name, out var tab), pageDto.styles);
            allContainers.Add(container);

            foreach (var div in pageDto.FirstChildren)
                InstantiateDiv(div, container);

            formBinding.Add(() => {
                if (container != null && container.container != null && container.container.activeInHierarchy)
                    _answer.pageId = inputAnswerDto.inputId;
            });

            HandleStyle(tab.gameObject, null, pageDto.styles);
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

            if (imageDto.FirstChildren == null || imageDto.FirstChildren.Count == 0) // Simple image
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
                    vignetteContainer.ShouldResetAndFetchVignetteFromDB = false;
                    vignetteContainer.Clear();
                    vignetteContainer.ChangePrimaryVignetteMode(E_VignetteScale.Mid);

                    HandleStyle(newParent.container, newParent.container.GetComponent<IDisplayer>(), newParent.Styles);
                    m_vignetteContainers.Add(vignetteContainer);
                }

                VignetteBuffer buffer =  await vignetteContainer.CreateVignette(imageDto);
                vignetteContainer.UpdateNavigation();

                buffer.OnVignetteClicked += () => {
                    ValidateForm(imageDto.guid);
                };
            }
            
            HandleStyle(imageGO, displayer, imageDto.styles);
        }

        private void HandleStyle(GameObject gameObject, IDisplayer displayer, List<StyleDto> styleDtos)
        {
            if (gameObject == null || styleDtos == null)
                return;
            foreach (StyleDto styleDto in styleDtos)
            {
                if (styleDto.variants == null)
                    continue;

                var variantDto = styleDto.variants.Find(variantDto => variantDto is UGUIStyleVariantDto) as UGUIStyleVariantDto;
                if (variantDto == null)
                    continue;

                foreach (var styleItemDto in variantDto.StyleVariantItems)
                    ApplyStyle(gameObject, displayer, styleItemDto);
            }
        }

        private void ApplyStyle(GameObject go, IDisplayer displayer, UGUIStyleItemDto styleItemDto)
        {
            switch (styleItemDto) 
            { 
                case PositionStyleDto positionStyleVariant :
                {
                    go.GetComponent<RectTransform>().anchoredPosition = new Vector2(positionStyleVariant.posX, positionStyleVariant.posY);
                    break;
                }
                case SizeStyleDto sizeStyleVariant :
                {
                    go.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeStyleVariant.width, sizeStyleVariant.height);  
                    break;
                }
                case AnchorStyleDto anchorStyleVariant :
                {
                    RectTransform rect = go.GetComponent<RectTransform>();
                    rect.anchorMax = new Vector2(anchorStyleVariant.maxX, anchorStyleVariant.maxY);
                    rect.anchorMin = new Vector2(anchorStyleVariant.minX, anchorStyleVariant.minY);
                    rect.pivot = new Vector2(anchorStyleVariant.pivotX, anchorStyleVariant.pivotY);
                    break;
                }
                case ColorStyleDto colorStyleVariant :
                {
                    displayer?.SetColor(new Color() {
                        a = colorStyleVariant.color.A,
                        b = colorStyleVariant.color.B,
                        g = colorStyleVariant.color.G,
                        r = colorStyleVariant.color.R
                    });
                    break;
                }
                case TextStyleDto textStyleVariant :
                {
                    displayer?.SetResource(textStyleVariant);
                    break;
                }
            }
        }
    }
}
