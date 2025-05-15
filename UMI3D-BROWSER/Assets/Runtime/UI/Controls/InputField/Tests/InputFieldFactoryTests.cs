using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using umi3d.browserRuntime.ui.inputField;
using UnityEngine;
using UnityEngine.TestTools;

public class InputFieldFactoryTests
{
    public class GetOrCreateInputFieldTests
    {
        private GameObject _factoryGameObject;
        private InputFieldFactory _inputFieldFactory;
        private InputFieldModelContainer _singleLinePrefab;
        private InputFieldModelContainer _multiLinePrefab;

        [SetUp]
        public void SetUp()
        {
            // Given: An InputFieldFactory instance
            _factoryGameObject = new GameObject();
            _inputFieldFactory = _factoryGameObject.AddComponent<InputFieldFactory>();


            _singleLinePrefab = new GameObject().AddComponent<InputFieldModelContainer>();
            _multiLinePrefab = new GameObject().AddComponent<InputFieldModelContainer>();

            // Assign test prefabs to the factory
            _inputFieldFactory.GetType().GetField("_singleLinePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_inputFieldFactory, _singleLinePrefab);
            _inputFieldFactory.GetType().GetField("_multiLinePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_inputFieldFactory, _multiLinePrefab);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_factoryGameObject);
            Object.DestroyImmediate(_singleLinePrefab.gameObject);
            Object.DestroyImmediate(_multiLinePrefab.gameObject);
        }

        [UnityTest]
        public IEnumerator GivenValidParameters_WhenCreatingSingleLineInputField_ThenInputFieldIsCreated()
        {
            // Given: Valid parameters for a single line input field
            Transform parent = new GameObject().transform;
            bool isMultiline = false;
            string label = "Test Label";
            string value = "Test Value";
            string placeholder = "Test Placeholder";
            int nbLine = 1;
            var contentType = TMP_InputField.ContentType.Password;

            // When: Creating the input field
            GameObject inputField = _inputFieldFactory.GetOrCreateInputField(parent, isMultiline, label, value, placeholder, nbLine, contentType);
            yield return null;

            // Then: The input field is created and set up correctly
            Assert.IsNotNull(inputField);
            var inputFieldModelContainer = inputField.GetComponent<InputFieldModelContainer>();
            Assert.IsNotNull(inputFieldModelContainer);
            Assert.AreEqual(label, inputFieldModelContainer.model.label);
            Assert.AreEqual(value, inputFieldModelContainer.model.value);
            Assert.AreEqual(placeholder, inputFieldModelContainer.model.placeholder);
            Assert.AreEqual(1, inputFieldModelContainer.model.nbrLine);
            Assert.AreEqual(contentType, inputFieldModelContainer.model.ContentType);
        }

        [UnityTest]
        public IEnumerator GivenMultipleLine_WhenCreatingSingleLineInputField_ThenInputFieldIsCreatedWithOneLine()
        {
            // Given: Valid parameters for a single line input field
            Transform parent = new GameObject().transform;
            bool isMultiline = false;
            int nbLine = 3;

            // When: Creating the input field
            GameObject inputField = _inputFieldFactory.GetOrCreateInputField(parent, isMultiline, nbLine: nbLine);
            yield return null;

            // Then: The input field is created and set up correctly
            Assert.IsNotNull(inputField);
            var inputFieldModelContainer = inputField.GetComponent<InputFieldModelContainer>();
            Assert.IsNotNull(inputFieldModelContainer);
            Assert.AreEqual(1, inputFieldModelContainer.model.nbrLine);
        }

        [UnityTest]
        public IEnumerator GivenNegativeLine_WhenCreatingSingleLineInputField_ThenInputFieldIsCreatedWithOneLine()
        {
            // Given: Valid parameters for a single line input field
            Transform parent = new GameObject().transform;
            bool isMultiline = false;
            int nbLine = -3;

            // When: Creating the input field
            GameObject inputField = _inputFieldFactory.GetOrCreateInputField(parent, isMultiline, nbLine: nbLine);
            yield return null;

            // Then: The input field is created and set up correctly
            Assert.IsNotNull(inputField);
            var inputFieldModelContainer = inputField.GetComponent<InputFieldModelContainer>();
            Assert.IsNotNull(inputFieldModelContainer);
            Assert.AreEqual(1, inputFieldModelContainer.model.nbrLine);
        }

        [UnityTest]
        public IEnumerator GivenValidParameters_WhenCreatingMultiLineInputField_ThenInputFieldIsCreated()
        {
            // Given: Valid parameters for a multi line input field
            Transform parent = new GameObject().transform;
            bool isMultiline = true;
            string label = "Test Label";
            string value = "Test Value";
            string placeholder = "Test Placeholder";
            int nbLine = 3;

            // When: Creating the input field
            GameObject inputField = _inputFieldFactory.GetOrCreateInputField(parent, isMultiline, label, value, placeholder, nbLine);
            yield return null;

            // Then: The input field is created and set up correctly
            Assert.IsNotNull(inputField);
            var inputFieldModelContainer = inputField.GetComponent<InputFieldModelContainer>();
            Assert.IsNotNull(inputFieldModelContainer);
            Assert.AreEqual(label, inputFieldModelContainer.model.label);
            Assert.AreEqual(value, inputFieldModelContainer.model.value);
            Assert.AreEqual(placeholder, inputFieldModelContainer.model.placeholder);
            Assert.AreEqual(nbLine, inputFieldModelContainer.model.nbrLine);
        }

        [UnityTest]
        public IEnumerator GivenNegativeLine_WhenCreatingMultiLineInputField_ThenInputFieldIsCreatedWithOneLine()
        {
            // Given: Valid parameters for a multi line input field
            Transform parent = new GameObject().transform;
            bool isMultiline = true;
            int nbLine = -3;

            // When: Creating the input field
            GameObject inputField = _inputFieldFactory.GetOrCreateInputField(parent, isMultiline, nbLine: nbLine);
            yield return null;

            // Then: The input field is created and set up correctly
            Assert.IsNotNull(inputField);
            var inputFieldModelContainer = inputField.GetComponent<InputFieldModelContainer>();
            Assert.IsNotNull(inputFieldModelContainer);
            Assert.AreEqual(1, inputFieldModelContainer.model.nbrLine);
        }
    }

    public class ReturnTests
    {
        private GameObject _factoryGameObject;
        private InputFieldFactory _inputFieldFactory;
        private InputFieldModelContainer _singleLinePrefab;
        private InputFieldModelContainer _multiLinePrefab;

        [SetUp]
        public void SetUp()
        {
            // Given: An InputFieldFactory instance
            _factoryGameObject = new GameObject();
            _inputFieldFactory = _factoryGameObject.AddComponent<InputFieldFactory>();


            _singleLinePrefab = new GameObject().AddComponent<InputFieldModelContainer>();
            _multiLinePrefab = new GameObject().AddComponent<InputFieldModelContainer>();

            // Assign test prefabs to the factory
            _inputFieldFactory.GetType().GetField("_singleLinePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_inputFieldFactory, _singleLinePrefab);
            _inputFieldFactory.GetType().GetField("_multiLinePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_inputFieldFactory, _multiLinePrefab);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_factoryGameObject);
            Object.DestroyImmediate(_singleLinePrefab.gameObject);
            Object.DestroyImmediate(_multiLinePrefab.gameObject);
        }

        [UnityTest]
        public IEnumerator GivenInputField_WhenReturningToPool_ThenInputFieldIsInactiveAndInPool()
        {
            // Given: An input field created by the factory
            Transform parent = new GameObject().transform;
            bool isMultiline = false;
            GameObject inputField = _inputFieldFactory.GetOrCreateInputField(parent, isMultiline);
            yield return null;

            // When: Returning the input field to the pool
            _inputFieldFactory.Return(inputField);
            yield return null;

            // Then: The input field is inactive and in the pool
            Assert.IsFalse(inputField.activeSelf);
            Assert.AreEqual(1, _inputFieldFactory.AvailableInputFieldSingleCount);
        }

        [UnityTest]
        public IEnumerator GivenInputFieldInPool_WhenCreatingInputField_ThenInputFieldIsReusedFromPool()
        {
            // Given: An input field created and returned to the pool
            Transform parent = new GameObject().transform;
            bool isMultiline = false;
            GameObject inputField = _inputFieldFactory.GetOrCreateInputField(parent, isMultiline);
            yield return null;
            _inputFieldFactory.Return(inputField);
            yield return null;

            // When: Creating another input field
            GameObject reusedInputField = _inputFieldFactory.GetOrCreateInputField(parent, isMultiline);
            yield return null;

            // Then: The input field is reused from the pool
            Assert.AreEqual(inputField, reusedInputField);
            Assert.IsTrue(reusedInputField.activeSelf);
            Assert.AreEqual(0, _inputFieldFactory.AvailableInputFieldSingleCount);
        }
    }
}