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
using UnityEngine;
using UnityEngine.UI;

public class ButtonFactoryTests
{
    public class GetOrCreateButtonTests
    {
        ButtonFactory _factory;
        Transform _container;

        [SetUp]
        public void SetUp()
        {
            _factory = new GameObject().AddComponent<ButtonFactory>();
            _container = new GameObject().transform;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_factory.gameObject);
            GameObject.DestroyImmediate(_container.gameObject);
        }

        [Test]
        public void GivenValidArguments_WhenCreatingButton_ThenButtonCreated()
        {
            var label = "TestLabel";
            var callbackCalled = false;
            GameObject buttonGameObject = _factory.GetOrCreateButton(_container, label, () => callbackCalled = true);
            
            Assert.IsNotNull(buttonGameObject);
            Assert.AreEqual(_container, buttonGameObject.transform.parent);
        }

        [Test]
        public void GivenValidArguments_WhenCreatingButton_ThenButtonCreatedAndConfigured()
        {
            var label = "TestLabel";
            var callbackCalled = false;
            GameObject buttonGameObject = _factory.GetOrCreateButton(_container, label, () => callbackCalled = true);

            var text = buttonGameObject.GetComponentInChildren<TMP_Text>();
            Assert.AreEqual(label, text.text);

            buttonGameObject.GetComponentInChildren<Button>().onClick?.Invoke();
            Assert.IsTrue(callbackCalled);
        }

        [Test]
        public void GivenValidArgumentsAndButtonInPool_WhenCreatingButton_ThenButtonIsReUsedndActive()
        {
            var modelContainer = new GameObject().AddComponent<ButtonModelContainer>();
            modelContainer.gameObject.SetActive(false);
            _factory._pool.Enqueue(modelContainer);

            var buttonGameObject = _factory.GetOrCreateButton(_container, "", () => { });

            Assert.AreEqual(_factory._pool.Count, 0);
            Assert.IsTrue(buttonGameObject.activeInHierarchy);
        }
    }
}