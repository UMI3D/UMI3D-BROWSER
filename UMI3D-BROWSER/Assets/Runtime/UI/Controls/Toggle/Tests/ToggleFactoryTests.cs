using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui.toggle;
using UnityEngine;
using UnityEngine.TestTools;

public class ToggleFactoryTests
{
    public class GetOrCreateToggleTests
    {
        private GameObject _factoryGameObject;
        private ToggleFactory _toggleFactory;
        private ToggleModelContainer _togglePrefab;

        [SetUp]
        public void SetUp()
        {
            // Given: An ToggleFactory instance
            _factoryGameObject = new GameObject();
            _toggleFactory = _factoryGameObject.AddComponent<ToggleFactory>();

            _togglePrefab = new GameObject().AddComponent<ToggleModelContainer>();

            // Assign test prefabs to the factory
            _toggleFactory.GetType().GetField("_togglePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_toggleFactory, _togglePrefab);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_factoryGameObject);
            Object.DestroyImmediate(_togglePrefab.gameObject);
        }

        [Test]
        public void GivenNoAvailableToggles_WhenCreatingToggle_ThenNewToggleIsCreated()
        {
            // Given: No available toggles in the queue
            var parent = new GameObject().transform;

            // When: Creating a new toggle
            var toggle = _toggleFactory.GetOrCreateToggle(parent);

            // Then: A new toggle is created and active
            Assert.IsNotNull(toggle);
            Assert.IsTrue(toggle.activeSelf);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void GivenValidParameters_WhenCreatingToggle_ThenToggleHasCorrectLabelAndValue()
        {
            // Given: Valid parameters for label and value
            var parent = new GameObject().transform;
            string label = "TestLabel";
            bool value = true;

            // When: Creating a new toggle with parameters
            var toggle = _toggleFactory.GetOrCreateToggle(parent, label, value);

            // Then: The toggle has the correct label and value
            var toggleModel = toggle.GetComponent<ToggleModelContainer>().model;
            Assert.AreEqual(label, toggleModel.label);
            Assert.AreEqual(value, toggleModel.value);

            Object.DestroyImmediate(parent.gameObject);
        }
    }

    public class ReturnTests
    {
        private GameObject _factoryGameObject;
        private ToggleFactory _toggleFactory;
        private ToggleModelContainer _togglePrefab;

        [SetUp]
        public void SetUp()
        {
            // Given: An ToggleFactory instance
            _factoryGameObject = new GameObject();
            _toggleFactory = _factoryGameObject.AddComponent<ToggleFactory>();

            _togglePrefab = new GameObject().AddComponent<ToggleModelContainer>();

            // Assign test prefabs to the factory
            _toggleFactory.GetType().GetField("_togglePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_toggleFactory, _togglePrefab);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_factoryGameObject);
            Object.DestroyImmediate(_togglePrefab.gameObject);
        }

        [UnityTest]
        public IEnumerator GivenToggle_WhenReturningToPool_ThenToggleIsInactiveAndInPool()
        {
            // Given: An toggle created by the factory
            Transform parent = new GameObject().transform;
            GameObject Toggle = _toggleFactory.GetOrCreateToggle(parent);
            yield return null;

            // When: Returning the toggle to the pool
            _toggleFactory.Return(Toggle);
            yield return null;

            // Then: The toggle is inactive and in the pool
            Assert.IsFalse(Toggle.activeSelf);
            Assert.AreEqual(1, _toggleFactory.AvailableToggleCount);
        }

        [UnityTest]
        public IEnumerator GivenToggleInPool_WhenCreatingToggle_ThenToggleIsReusedFromPool()
        {
            // Given: An toggle created and returned to the pool
            Transform parent = new GameObject().transform;
            GameObject Toggle = _toggleFactory.GetOrCreateToggle(parent);
            yield return null;
            _toggleFactory.Return(Toggle);
            yield return null;

            // When: Creating another toggle
            GameObject reusedToggle = _toggleFactory.GetOrCreateToggle(parent);
            yield return null;

            // Then: The toggle is reused from the pool
            Assert.AreEqual(Toggle, reusedToggle);
            Assert.IsTrue(reusedToggle.activeSelf);
            Assert.AreEqual(0, _toggleFactory.AvailableToggleCount);
        }
    }
}
