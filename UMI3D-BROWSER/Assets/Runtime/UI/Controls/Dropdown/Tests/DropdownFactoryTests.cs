using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using umi3d.browserRuntime.ui.dropdown;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class DropdownFactoryTests
{
    public class GetOrCreateDropdownTests
    {
        private DropdownFactory dropdownFactory;
        private DropdownModelContainer dropdownPrefab;
        private Transform parentTransform;

        [SetUp]
        public void SetUp()
        {
            // Initialize the test objects
            var gameObject = new GameObject();
            dropdownFactory = gameObject.AddComponent<DropdownFactory>();
            dropdownPrefab = new GameObject().AddComponent<DropdownModelContainer>();

            // Use reflection to set the private field _dropdownPrefab
            var dropdownPrefabField = typeof(DropdownFactory).GetField("_dropdownPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
            dropdownPrefabField.SetValue(dropdownFactory, dropdownPrefab);

            parentTransform = new GameObject().transform;
        }

        [Test]
        public void GivenNoAvailableDropdowns_WhenGetOrCreateDropdown_ThenNewDropdownIsCreated()
        {
            // Given
            Assert.AreEqual(0, dropdownFactory.AvailableDropdownCount);

            // When
            var dropdown = dropdownFactory.GetOrCreateDropdown(parentTransform);

            // Then
            Assert.IsNotNull(dropdown);
            Assert.AreEqual(0, dropdownFactory.AvailableDropdownCount);
        }

        [Test]
        public void GivenAvailableDropdowns_WhenGetOrCreateDropdown_ThenExistingDropdownIsUsed()
        {
            // Given
            var dropdown = dropdownFactory.GetOrCreateDropdown(parentTransform);

            // Use reflection to access the private field _lstDropdownsAvaible
            var lstDropdownsAvaibleField = typeof(DropdownFactory).GetField("_lstDropdownsAvaible", BindingFlags.NonPublic | BindingFlags.Instance);
            var lstDropdownsAvaible = (Queue<DropdownModelContainer>)lstDropdownsAvaibleField.GetValue(dropdownFactory);
            lstDropdownsAvaible.Enqueue(dropdown.GetComponent<DropdownModelContainer>());

            Assert.AreEqual(1, dropdownFactory.AvailableDropdownCount);

            // When
            var newDropdown = dropdownFactory.GetOrCreateDropdown(parentTransform);

            // Then
            Assert.IsNotNull(newDropdown);
            Assert.AreEqual(0, dropdownFactory.AvailableDropdownCount);
        }

        [Test]
        public void GivenParameters_WhenGetOrCreateDropdown_ThenDropdownIsConfigured()
        {
            // Given
            string label = "TestLabel";
            List<string> options = new List<string> { "Option1", "Option2" };
            string value = "Option1";

            // When
            var dropdown = dropdownFactory.GetOrCreateDropdown(parentTransform, label, options, value);
            var dropdownModel = dropdown.GetComponent<DropdownModelContainer>().model;

            // Then
            Assert.AreEqual(label, dropdownModel.label);
            CollectionAssert.AreEqual(options, dropdownModel.options);
            Assert.AreEqual(value, dropdownModel.value);
        }
    }

    public class ReturnTests
    {
        private GameObject _factoryGameObject;
        private DropdownFactory _dropdownFactory;
        private DropdownModelContainer _dropdownPrefab;

        [SetUp]
        public void SetUp()
        {
            // Given: An DropdownFactory instance
            _factoryGameObject = new GameObject();
            _dropdownFactory = _factoryGameObject.AddComponent<DropdownFactory>();

            _dropdownPrefab = new GameObject().AddComponent<DropdownModelContainer>();

            // Assign test prefabs to the factory
            _dropdownFactory.GetType().GetField("_dropdownPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_dropdownFactory, _dropdownPrefab);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_factoryGameObject);
            Object.DestroyImmediate(_dropdownPrefab.gameObject);
        }

        [UnityTest]
        public IEnumerator GivenDropdown_WhenReturningToPool_ThenDropdownIsInactiveAndInPool()
        {
            // Given: An dropdown created by the factory
            Transform parent = new GameObject().transform;
            GameObject Dropdown = _dropdownFactory.GetOrCreateDropdown(parent);
            yield return null;

            // When: Returning the dropdown to the pool
            _dropdownFactory.Return(Dropdown);
            yield return null;

            // Then: The dropdown is inactive and in the pool
            Assert.IsFalse(Dropdown.activeSelf);
            Assert.AreEqual(1, _dropdownFactory.AvailableDropdownCount);
        }

        [UnityTest]
        public IEnumerator GivenDropdownInPool_WhenCreatingDropdown_ThenDropdownIsReusedFromPool()
        {
            // Given: An dropdown created and returned to the pool
            Transform parent = new GameObject().transform;
            GameObject Dropdown = _dropdownFactory.GetOrCreateDropdown(parent);
            yield return null;
            _dropdownFactory.Return(Dropdown);
            yield return null;

            // When: Creating another dropdown
            GameObject reusedDropdown = _dropdownFactory.GetOrCreateDropdown(parent);
            yield return null;

            // Then: The dropdown is reused from the pool
            Assert.AreEqual(Dropdown, reusedDropdown);
            Assert.IsTrue(reusedDropdown.activeSelf);
            Assert.AreEqual(0, _dropdownFactory.AvailableDropdownCount);
        }

    }
}