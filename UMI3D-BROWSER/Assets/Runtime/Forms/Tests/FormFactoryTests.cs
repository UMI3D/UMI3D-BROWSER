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
using NUnit.Framework;
using TMPro;
using umi3d.browserRuntime.button;
using umi3d.browserRuntime.forms;
using umi3d.browserRuntime.image;
using umi3d.browserRuntime.text;
using umi3d.browserRuntime.thumbnails;
using umi3d.browserRuntime.ui.inputField;
using umi3d.browserRuntime.ui.slider;
using umi3d.common.interaction.form;
using UnityEngine;
using UnityEngine.UI;

public class FormFactoryTests
{
    public class CreateFormTests
    {
        FormFactory _formFactory;

        [SetUp]
        public void Setup()
        {
            _formFactory = new GameObject("AddDivTest_FormFactory").AddComponent<FormFactory>();
            _formFactory._content = new GameObject().transform;
            _formFactory._content.SetParent(_formFactory.transform);
            _formFactory._inputFieldFactory = GameObject.Instantiate(_formFactory._inputFieldFactory);
            _formFactory._inputFieldFactory.transform.SetParent(_formFactory.transform, false);
            _formFactory._sliderFactory = GameObject.Instantiate(_formFactory._sliderFactory);
            _formFactory._sliderFactory.transform.SetParent(_formFactory.transform, false);
            _formFactory._buttonFactory = GameObject.Instantiate(_formFactory._buttonFactory);
            _formFactory._buttonFactory.transform.SetParent(_formFactory.transform, false);
            _formFactory._imageFactory = GameObject.Instantiate(_formFactory._imageFactory);
            _formFactory._imageFactory.transform.SetParent(_formFactory.transform, false);
            _formFactory._textFactory = GameObject.Instantiate(_formFactory._textFactory);
            _formFactory._textFactory.transform.SetParent(_formFactory.transform, false);
            _formFactory._tabManager = GameObject.Instantiate(_formFactory._tabManager);
            _formFactory._tabManager.transform.SetParent(_formFactory.transform, false);
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_formFactory.gameObject);
        }

        [Test]
        public void GivenFormDto_WhenAddingDiv_ThenCreatingChildren()
        {
            var formDto = new FormDto() {
                FirstChildren = new System.Collections.Generic.List<DivDto>() { 
                    new GroupDto(),
                    new InputDto<string>() 
                }
            };
            _formFactory.CreateForm(formDto);


            Assert.AreEqual(2, _formFactory._content.childCount);
        }
    }

    public class AddDivTests
    {
        FormFactory _formFactory;
        Transform _baseContainer;

        [SetUp]
        public void Setup()
        {
            _formFactory = new GameObject("AddDivTest_FormFactory").AddComponent<FormFactory>();
            _formFactory._inputFieldFactory = GameObject.Instantiate(_formFactory._inputFieldFactory);
            _formFactory._inputFieldFactory.transform.SetParent(_formFactory.transform, false);
            _formFactory._sliderFactory = GameObject.Instantiate(_formFactory._sliderFactory);
            _formFactory._sliderFactory.transform.SetParent(_formFactory.transform, false);
            _formFactory._buttonFactory = GameObject.Instantiate(_formFactory._buttonFactory);
            _formFactory._buttonFactory.transform.SetParent(_formFactory.transform, false);
            _formFactory._imageFactory = GameObject.Instantiate(_formFactory._imageFactory);
            _formFactory._imageFactory.transform.SetParent(_formFactory.transform, false);
            _formFactory._textFactory = GameObject.Instantiate(_formFactory._textFactory);
            _formFactory._textFactory.transform.SetParent(_formFactory.transform, false);
            _formFactory._tabManager = GameObject.Instantiate(_formFactory._tabManager);
            _formFactory._tabManager.transform.SetParent(_formFactory.transform, false);
            _baseContainer = new GameObject("AddDivTest_BaseContainer").transform;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_formFactory.gameObject);
            if (_baseContainer)
                GameObject.DestroyImmediate(_baseContainer.gameObject);
        }

        #region Input
        [Test]
        public void GivenGroupDto_WhenAddingDiv_ThenGroupCreated()
        {
            GroupDto groupDto = new GroupDto();
            _formFactory.AddDiv(groupDto, new(_baseContainer));

            Assert.AreEqual(1, _baseContainer.childCount);
        }

        [Test]
        public void GivenGroupDtoWithChildren_WhenAddingDiv_ThenGroupAndChildrenCreated()
        {
            GroupDto groupDto = new GroupDto() {
                FirstChildren = new() { new GroupDto(), new GroupDto() }
            };
            _formFactory.AddDiv(groupDto, new(_baseContainer));

            Assert.AreEqual(1, _baseContainer.childCount);
            Assert.AreEqual(2, _baseContainer.GetChild(0).childCount);
        }

        [Test]
        public void GivenInputDtoString_WhenAddingDiv_ThenInputFieldCreatedAndConfigured()
        {
            InputDto<string> inputDto = new InputDto<string>() {
                label = "TestName",
                Value = "TestValue",
                PlaceHolder = "TestPlaceHolder",
                TextType = TextType.Text,
            };
            _formFactory.AddDiv(inputDto, new(_baseContainer));

            var model = _baseContainer.GetComponentInChildren<InputFieldModelContainer>().model;
            var inputField = _baseContainer.GetComponentInChildren<TMP_InputField>();
            Assert.IsNotNull(model);
            Assert.IsNotNull(inputField);
            Assert.AreEqual(model.label, inputDto.label);
            Assert.AreEqual(model.value, inputDto.Value);
            Assert.AreEqual(model.placeholder, inputDto.PlaceHolder);
            Assert.AreEqual(TMP_InputField.ContentType.Standard, inputField.contentType);
        }

        [Test]
        public void GivenInputDtoStringPassword_WhenAddingDiv_ThenInputFieldCreatedAndConfigured()
        {
            var inputDto = new InputDto<string>() {
                label = "TestName",
                Value = "TestValue",
                PlaceHolder = "TestPlaceHolder",
                TextType = TextType.Password,
            };
            _formFactory.AddDiv(inputDto, new(_baseContainer));

            var model = _baseContainer.GetComponentInChildren<InputFieldModelContainer>().model;
            var inputField = _baseContainer.GetComponentInChildren<TMP_InputField>();
            Assert.IsNotNull(model);
            Assert.IsNotNull(inputField);
            Assert.AreEqual(model.label, inputDto.label);
            Assert.AreEqual(model.value, inputDto.Value);
            Assert.AreEqual(model.placeholder, inputDto.PlaceHolder);
            Assert.AreEqual(TMP_InputField.ContentType.Password, inputField.contentType);
        }

        [Test]
        public void GivenInputDtoInt_WhenAddingDiv_ThenInputFieldCreatedAndConfigured()
        {
            var inputDto = new InputDto<int>() {
                label = "TestName",
                Value = 0,
                PlaceHolder = 000000,
                TextType = TextType.Number,
            };
            _formFactory.AddDiv(inputDto, new(_baseContainer));

            var model = _baseContainer.GetComponentInChildren<InputFieldModelContainer>().model;
            var inputField = _baseContainer.GetComponentInChildren<TMP_InputField>();
            Assert.IsNotNull(model);
            Assert.IsNotNull(inputField);
            Assert.AreEqual(model.label, inputDto.label);
            Assert.AreEqual(int.Parse(model.value), inputDto.Value);
            Assert.AreEqual(int.Parse(model.placeholder), inputDto.PlaceHolder);
            Assert.AreEqual(TMP_InputField.ContentType.IntegerNumber, inputField.contentType);
        }

        [Test]
        public void GivenRangeDtoInt_WhenAddingDiv_ThenSliderCreatedAndConfigured()
        {
            var rangeDto = new RangeDto<int>() {
                label = "TestLabel",
                Min = 1,
                Max = 10,
                Value = 5
            };
            _formFactory.AddDiv(rangeDto, new(_baseContainer));

            var model = _baseContainer.GetComponentInChildren<SliderModelContainer>().model;
            var slider = _baseContainer.GetComponentInChildren<Slider>();
            Assert.IsNotNull(model);
            Assert.IsNotNull(slider);
            Assert.AreEqual(model.label, rangeDto.label);
            Assert.AreEqual(model.minValue, rangeDto.Min);
            Assert.AreEqual(model.maxValue, rangeDto.Max);
            Assert.AreEqual(model.value, rangeDto.Value);
            Assert.AreEqual(true, slider.wholeNumbers);
        }

        [Test]
        public void GivenRangeDtoFloat_WhenAddingDiv_ThenSliderCreatedAndConfigured()
        {
            var rangeDto = new RangeDto<float>() {
                label = "TestLabel",
                Min = 0.0f,
                Max = 1.0f,
                Value = 0.5f
            };
            _formFactory.AddDiv(rangeDto, new(_baseContainer));

            var model = _baseContainer.GetComponentInChildren<SliderModelContainer>().model;
            var slider = _baseContainer.GetComponentInChildren<Slider>();
            Assert.IsNotNull(model);
            Assert.IsNotNull(slider);
            Assert.AreEqual(model.label, rangeDto.label);
            Assert.AreEqual(model.minValue, rangeDto.Min);
            Assert.AreEqual(model.maxValue, rangeDto.Max);
            Assert.AreEqual(model.value, rangeDto.Value);
            Assert.AreEqual(false, slider.wholeNumbers);
        }

        [Test]
        public void GivenButtonDto_WhenAddingDiv_ThenButtonCreated()
        {
            ButtonDto buttonDto = new ButtonDto() {
                Text = "TestLabel"
            };
            _formFactory.AddDiv(buttonDto, new(_baseContainer));

            var model = _baseContainer.GetComponentInChildren<ButtonModelContainer>().Model;
            Assert.IsNotNull(model);
            Assert.AreEqual("TestLabel", model.Label);
        }
        #endregion

        #region Base
        [Test]
        public void GivenPageDto_WhenAddingDiv_ThenPageCreated()
        {
            PageDto pageDto = new PageDto() {
                name = "TestPage",
            };
            _formFactory.AddDiv(pageDto, new(_baseContainer));
        }

        [Test]
        public void GivenPageDtoWithChildren_WhenAddingDiv_ThenPageCreatedWithChildren()
        {
            PageDto pageDto = new PageDto() {
                name = "TestPage",
                FirstChildren = new() {
                    new LabelDto() {
                        text = "TestText"
                    }
                }
            };
            _formFactory.AddDiv(pageDto, new(_baseContainer));
            var modelContainer = _formFactory._tabManager.GetComponentInChildren<TextModelContainer>();
            Assert.IsNotNull(modelContainer);
            Assert.AreEqual(((LabelDto)pageDto.FirstChildren[0]).text, modelContainer.Model.Text);
        }

        [Test]
        public void GivenLabelDto_WhenAddingDiv_ThenTextCreated()
        {
            LabelDto labelDto = new LabelDto() {
                text = "TestText"
            };
            _formFactory.AddDiv(labelDto, new(_baseContainer));

            var model = _baseContainer.GetComponentInChildren<TextModelContainer>().Model;
            Assert.IsNotNull(model);
            Assert.AreEqual(labelDto.text, model.Text);
        }

        [Test]
        public void GivenImageDto_WhenAddingDiv_ThenImageCreated()
        {
            ImageDto imageDto = new ImageDto();
            _formFactory.AddDiv(imageDto, new(_baseContainer));

            var model = _baseContainer.GetComponentInChildren<ImageModelContainer>().Model;
            Assert.IsNotNull(model);
        }

        [Test]
        public void GivenImageDtoWithChildren_WhenAddingDiv_ThenThumbnailCreated()
        {
            ImageDto imageDto = new ImageDto() {
                FirstChildren = new() {
                    new LabelDto() {
                        text = "TestText"
                    }
                }
            };
            var container = new FormFactory.Container(_baseContainer);
            _formFactory.AddDiv(imageDto, container);

            var modelContainer = container.Transform.GetComponent<ThumbnailListModelContainer>();
            Assert.IsNotNull(modelContainer);
            Assert.AreEqual(1, modelContainer.Model.Thumbnails.Count);
        }
        #endregion

        #region Form

        [Test]
        public void GivenNullDto_WhenAddingDiv_ThenDoesNotCrash()
        {
            _formFactory.AddDiv(null, new(_baseContainer));
        }

        [Test]
        public void GivenNullParent_WhenAddingDiv_ThenDoesNotCrash()
        {
            _formFactory.AddDiv(new GroupDto(), null);
        }
        #endregion
    }

    public class ReplaceContainerWithPrefabTests
    {
        [Test]
        public void GivenNewPrefab_WhenReplacingContainer_ThenNewObjectCreated()
        {
            var transform1 = new GameObject().transform;
            var object2 = new GameObject().AddComponent<Image>();
            Assert.IsNotNull(object2);

            var container = new FormFactory.Container(transform1);

            var objectCreated = FormFactory.ReplaceContainerWithPrefab(container, object2);

            Assert.IsNotNull(container.Transform);

            if (transform1)
                GameObject.DestroyImmediate(transform1.gameObject);
            if (object2)
                GameObject.DestroyImmediate(object2.gameObject);
            if (objectCreated)
                GameObject.DestroyImmediate(objectCreated.gameObject);
        }
    }

    public class ClearTests
    {
        FormFactory _formFactory;

        [SetUp]
        public void Setup()
        {
            _formFactory = new GameObject("AddDivTest_FormFactory").AddComponent<FormFactory>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_formFactory.gameObject);
        }

        [Test]
        public void GivenForm_WhenClearing_ThenFormCleared()
        {
            var formDto = new FormDto() {
                FirstChildren = new System.Collections.Generic.List<DivDto>() { new GroupDto(), new GroupDto() }
            };
            _formFactory.CreateForm(formDto);

            _formFactory.Clear();

            Assert.AreEqual(0, _formFactory._content.childCount);
        }
    }
}