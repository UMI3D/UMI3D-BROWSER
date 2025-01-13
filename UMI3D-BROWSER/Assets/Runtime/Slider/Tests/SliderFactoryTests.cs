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
        private SliderModelContainer _prefab;

        [SetUp]
        public void SetUp()
        {
            // Given: Initial state of the class
            var gameObject = new GameObject();
            _sliderFactory = gameObject.AddComponent<SliderFactory>();
            _prefab = new GameObject().AddComponent<SliderModelContainer>();
            _sliderFactory.GetType().GetField("_prefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_sliderFactory, _prefab);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            Object.DestroyImmediate(_sliderFactory.gameObject);
            Object.DestroyImmediate(_prefab.gameObject);
        }

        [Test]
        public void GivenNoAvailableSliders_WhenCreatingSlider_ThenNewSliderIsCreated()
        {
            // Given: No available sliders in the queue
            Transform parent = new GameObject().transform;

            // When: Creating a new slider
            var slider = _sliderFactory.GetOrCreateSlider(parent);

            // Then: A new slider is created
            Assert.IsNotNull(slider);
            Assert.IsTrue(slider.activeSelf);
        }

        [Test]
        public void GivenAvailableSlider_WhenCreatingSlider_ThenSliderIsReused()
        {
            // Given: An available slider in the queue
            Transform parent = new GameObject().transform;
            var availableSlider = GameObject.Instantiate(_prefab);
            _sliderFactory.GetType().GetField("_lstSlidersAvailable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_sliderFactory, new Queue<SliderModelContainer>(new[] { availableSlider }));

            // When: Creating a new slider
            var slider = _sliderFactory.GetOrCreateSlider(parent);

            // Then: The available slider is reused
            Assert.AreEqual(availableSlider.gameObject, slider);
            Assert.IsTrue(slider.activeSelf);
        }

        [Test]
        public void GivenValidParameters_WhenCreatingSlider_ThenSliderIsConfiguredCorrectly()
        {
            // Given: Valid parameters for the slider
            Transform parent = new GameObject().transform;
            string label = "Test Label";
            float value = 5;
            float minValue = 0;
            float maxValue = 10;
            bool isInteger = true;

            // When: Creating a new slider
            var slider = _sliderFactory.GetOrCreateSlider(parent, label, value, minValue, maxValue, isInteger);
            var sliderModel = slider.GetComponent<SliderModelContainer>().model;

            // Then: The slider is configured correctly
            Assert.AreEqual(label, sliderModel.label);
            Assert.AreEqual(value, sliderModel.value);
            Assert.AreEqual(minValue, sliderModel.minValue);
            Assert.AreEqual(maxValue, sliderModel.maxValue);
            Assert.AreEqual(isInteger, sliderModel.isInteger);
        }
    }

    public class Return
    {
        private SliderFactory _sliderFactory;
        private SliderModelContainer _prefab;

        [SetUp]
        public void SetUp()
        {
            // Given: Initial state of the class
            var gameObject = new GameObject();
            _sliderFactory = gameObject.AddComponent<SliderFactory>();
            _prefab = new GameObject().AddComponent<SliderModelContainer>();
            _sliderFactory.GetType().GetField("_prefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_sliderFactory, _prefab);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            Object.DestroyImmediate(_sliderFactory.gameObject);
            Object.DestroyImmediate(_prefab.gameObject);
        }

        [Test]
        public void GivenSliderGameObject_WhenReturningSlider_ThenSliderIsAddedToQueue()
        {
            // Given: A slider GameObject
            var sliderGameObject = GameObject.Instantiate(_prefab).gameObject;

            // When: Returning the slider
            _sliderFactory.Return(sliderGameObject);

            // Then: The slider is added to the queue
            var queue = (Queue<SliderModelContainer>)_sliderFactory.GetType().GetField("_lstSlidersAvailable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_sliderFactory);
            Assert.AreEqual(1, queue.Count);
            Assert.AreEqual(sliderGameObject, queue.Peek().gameObject);
        }

        [Test]
        public void GivenSliderGameObject_WhenReturningSlider_ThenSliderIsDeactivated()
        {
            // Given: A slider GameObject
            var sliderGameObject = GameObject.Instantiate(_prefab).gameObject;

            // When: Returning the slider
            _sliderFactory.Return(sliderGameObject);

            // Then: The slider is deactivated
            Assert.IsFalse(sliderGameObject.activeSelf);
        }

        [Test]
        public void GivenSliderGameObject_WhenReturningSlider_ThenSliderParentIsSetToFactory()
        {
            // Given: A slider GameObject
            var sliderGameObject = GameObject.Instantiate(_prefab).gameObject;

            // When: Returning the slider
            _sliderFactory.Return(sliderGameObject);

            // Then: The slider's parent is set to the factory
            Assert.AreEqual(_sliderFactory.transform, sliderGameObject.transform.parent);
        }

        [Test]
        public void GivenInvalidSliderGameObject_WhenReturningSlider_ThenSliderIsNotAddedToQueue()
        {
            // Given: An invalid slider GameObject (without SliderModelContainer component)
            var invalidSliderGameObject = new GameObject();

            // When: Returning the slider
            _sliderFactory.Return(invalidSliderGameObject);

            // Then: The slider is not added to the queue
            var queue = (Queue<SliderModelContainer>)_sliderFactory.GetType().GetField("_lstSlidersAvailable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_sliderFactory);
            Assert.AreEqual(0, queue.Count);
        }
    }
}
