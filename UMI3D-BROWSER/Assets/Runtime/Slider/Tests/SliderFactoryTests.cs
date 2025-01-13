using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui.slider;
using UnityEngine;
using UnityEngine.TestTools;

public class SliderFactoryTests
{
    public class GetOrCreateSlider
    {
        private SliderFactory _sliderFactory;
        private Transform _parentTransform;

        [SetUp]
        public void SetUp()
        {
            // Given: Initial state of the class
            var gameObject = new GameObject();
            _sliderFactory = gameObject.AddComponent<SliderFactory>();
            _parentTransform = new GameObject().transform;

            // Assuming _intPrefab and _floatPrefab are set up in the inspector or via code
            _sliderFactory.GetType().GetField("_intPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_sliderFactory, new GameObject().AddComponent<SliderModelContainer>());
            _sliderFactory.GetType().GetField("_floatPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_sliderFactory, new GameObject().AddComponent<SliderModelContainer>());
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            Object.DestroyImmediate(_sliderFactory.gameObject);
            Object.DestroyImmediate(_parentTransform.gameObject);
        }

        [Test]
        public void GivenValidParameters_WhenCreatingIntegerSlider_ThenIntegerSliderIsCreated()
        {
            // Given: Initial state of the class is already set up in SetUp method

            // When: Creating an integer slider
            var slider = _sliderFactory.GetOrCreateSlider(_parentTransform, "TestLabel", 5, 0, 10, true);

            // Then: Verify the slider is created and has the correct properties
            Assert.IsNotNull(slider);
            var sliderModel = slider.GetComponent<SliderModelContainer>().model;
            Assert.AreEqual("TestLabel", sliderModel.label);
            Assert.AreEqual(0, sliderModel.minValue);
            Assert.AreEqual(10, sliderModel.maxValue);
            Assert.AreEqual(5, sliderModel.value);
            Assert.IsTrue(sliderModel.isInteger);
        }

        [Test]
        public void GivenValidParameters_WhenCreatingFloatSlider_ThenFloatSliderIsCreated()
        {
            // Given: Initial state of the class is already set up in SetUp method

            // When: Creating a float slider
            var slider = _sliderFactory.GetOrCreateSlider(_parentTransform, "TestLabel", 5.5f, 0, 10, false);

            // Then: Verify the slider is created and has the correct properties
            Assert.IsNotNull(slider);
            var sliderModel = slider.GetComponent<SliderModelContainer>().model;
            Assert.AreEqual("TestLabel", sliderModel.label);
            Assert.AreEqual(0, sliderModel.minValue);
            Assert.AreEqual(10, sliderModel.maxValue);
            Assert.AreEqual(5.5f, sliderModel.value);
            Assert.IsFalse(sliderModel.isInteger);
        }

        [Test]
        public void GivenNoAvailableIntegerSliders_WhenCreatingIntegerSlider_ThenNewIntegerSliderIsCreated()
        {
            // Given: No available integer sliders in the queue

            // When: Creating an integer slider
            var slider = _sliderFactory.GetOrCreateSlider(_parentTransform, "TestLabel", 5, 0, 10, true);

            // Then: Verify a new integer slider is created
            Assert.IsNotNull(slider);
            var sliderModel = slider.GetComponent<SliderModelContainer>().model;
            Assert.AreEqual("TestLabel", sliderModel.label);
            Assert.AreEqual(0, sliderModel.minValue);
            Assert.AreEqual(10, sliderModel.maxValue);
            Assert.AreEqual(5, sliderModel.value);
            Assert.IsTrue(sliderModel.isInteger);
        }

        [Test]
        public void GivenNoAvailableFloatSliders_WhenCreatingFloatSlider_ThenNewFloatSliderIsCreated()
        {
            // Given: No available float sliders in the queue

            // When: Creating a float slider
            var slider = _sliderFactory.GetOrCreateSlider(_parentTransform, "TestLabel", 5.5f, 0, 10, false);

            // Then: Verify a new float slider is created
            Assert.IsNotNull(slider);
            var sliderModel = slider.GetComponent<SliderModelContainer>().model;
            Assert.AreEqual("TestLabel", sliderModel.label);
            Assert.AreEqual(0, sliderModel.minValue);
            Assert.AreEqual(10, sliderModel.maxValue);
            Assert.AreEqual(5.5f, sliderModel.value);
            Assert.IsFalse(sliderModel.isInteger);
        }
    }

    public class Return
    {
        private SliderFactory _sliderFactory;
        private Transform _parentTransform;

        [SetUp]
        public void SetUp()
        {
            // Given: Initial state of the class
            var gameObject = new GameObject();
            _sliderFactory = gameObject.AddComponent<SliderFactory>();
            _parentTransform = new GameObject().transform;

            // Assuming _intPrefab and _floatPrefab are set up in the inspector or via code
            _sliderFactory.GetType().GetField("_intPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_sliderFactory, new GameObject().AddComponent<SliderModelContainer>());
            _sliderFactory.GetType().GetField("_floatPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_sliderFactory, new GameObject().AddComponent<SliderModelContainer>());
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            Object.DestroyImmediate(_sliderFactory.gameObject);
            Object.DestroyImmediate(_parentTransform.gameObject);
        }

        [Test]
        public void GivenValidParameters_WhenCreatingIntegerSlider_ThenIntegerSliderIsCreated()
        {
            // Given: Initial state of the class is already set up in SetUp method

            // When: Creating an integer slider
            var slider = _sliderFactory.GetOrCreateSlider(_parentTransform, "TestLabel", 5, 0, 10, true);

            // Then: Verify the slider is created and has the correct properties
            Assert.IsNotNull(slider);
            var sliderModel = slider.GetComponent<SliderModelContainer>().model;
            Assert.AreEqual("TestLabel", sliderModel.label);
            Assert.AreEqual(0, sliderModel.minValue);
            Assert.AreEqual(10, sliderModel.maxValue);
            Assert.AreEqual(5, sliderModel.value);
            Assert.IsTrue(sliderModel.isInteger);
        }

        [Test]
        public void GivenValidParameters_WhenCreatingFloatSlider_ThenFloatSliderIsCreated()
        {
            // Given: Initial state of the class is already set up in SetUp method

            // When: Creating a float slider
            var slider = _sliderFactory.GetOrCreateSlider(_parentTransform, "TestLabel", 5.5f, 0, 10, false);

            // Then: Verify the slider is created and has the correct properties
            Assert.IsNotNull(slider);
            var sliderModel = slider.GetComponent<SliderModelContainer>().model;
            Assert.AreEqual("TestLabel", sliderModel.label);
            Assert.AreEqual(0, sliderModel.minValue);
            Assert.AreEqual(10, sliderModel.maxValue);
            Assert.AreEqual(5.5f, sliderModel.value);
            Assert.IsFalse(sliderModel.isInteger);
        }

        [Test]
        public void GivenNoAvailableIntegerSliders_WhenCreatingIntegerSlider_ThenNewIntegerSliderIsCreated()
        {
            // Given: No available integer sliders in the queue

            // When: Creating an integer slider
            var slider = _sliderFactory.GetOrCreateSlider(_parentTransform, "TestLabel", 5, 0, 10, true);

            // Then: Verify a new integer slider is created
            Assert.IsNotNull(slider);
            var sliderModel = slider.GetComponent<SliderModelContainer>().model;
            Assert.AreEqual("TestLabel", sliderModel.label);
            Assert.AreEqual(0, sliderModel.minValue);
            Assert.AreEqual(10, sliderModel.maxValue);
            Assert.AreEqual(5, sliderModel.value);
            Assert.IsTrue(sliderModel.isInteger);
        }

        [Test]
        public void GivenNoAvailableFloatSliders_WhenCreatingFloatSlider_ThenNewFloatSliderIsCreated()
        {
            // Given: No available float sliders in the queue

            // When: Creating a float slider
            var slider = _sliderFactory.GetOrCreateSlider(_parentTransform, "TestLabel", 5.5f, 0, 10, false);

            // Then: Verify a new float slider is created
            Assert.IsNotNull(slider);
            var sliderModel = slider.GetComponent<SliderModelContainer>().model;
            Assert.AreEqual("TestLabel", sliderModel.label);
            Assert.AreEqual(0, sliderModel.minValue);
            Assert.AreEqual(10, sliderModel.maxValue);
            Assert.AreEqual(5.5f, sliderModel.value);
            Assert.IsFalse(sliderModel.isInteger);
        }

        [Test]
        public void GivenIntegerSlider_WhenReturningSlider_ThenSliderIsAddedToIntegerQueue()
        {
            // Given: An integer slider
            var slider = _sliderFactory.GetOrCreateSlider(_parentTransform, "TestLabel", 5, 0, 10, true);

            // When: Returning the slider
            _sliderFactory.Return(slider);

            // Then: Verify the slider is added to the integer queue and deactivated
            Assert.AreEqual(1, _sliderFactory.AvailableIntSlider);
            Assert.IsFalse(slider.activeSelf);
        }

        [Test]
        public void GivenFloatSlider_WhenReturningSlider_ThenSliderIsAddedToFloatQueue()
        {
            // Given: A float slider
            var slider = _sliderFactory.GetOrCreateSlider(_parentTransform, "TestLabel", 5.5f, 0, 10, false);

            // When: Returning the slider
            _sliderFactory.Return(slider);

            // Then: Verify the slider is added to the float queue and deactivated
            Assert.AreEqual(1, _sliderFactory.AvailableFloatSlider);
            Assert.IsFalse(slider.activeSelf);
        }
    }
}
