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
using umi3d.browserRuntime.text;
using UnityEngine;

public class TextFactoryTests
{
    public class GetOrCreateTextTests
    {
        TextFactory _factory;
        Transform _container;

        [SetUp]
        public void SetUp()
        {
            _factory = new GameObject().AddComponent<TextFactory>();
            _container = new GameObject().transform;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_factory.gameObject);
            GameObject.DestroyImmediate(_container.gameObject);
        }

        [Test]
        public void GivenValidArguments_WhenCreatingText_ThenTextCreated()
        {
            var text = "TestText";
            GameObject TextGameObject = _factory.GetOrCreateText(_container, text);

            Assert.IsNotNull(TextGameObject);
            Assert.AreEqual(_container, TextGameObject.transform.parent);
        }

        [Test]
        public void GivenValidArguments_WhenCreatingText_ThenTextCreatedAndConfigured()
        {
            var text = "TestText";
            var settings = new TextFactory.Settings() {
                Transform = new() {
                    Position = Vector3.one,
                    Size = Vector3.one / 2
                },
                Anchor = new() {
                    AnchorMin = Vector2.zero,
                    AnchorMax = Vector2.one,
                    Pivot = Vector2.zero,
                },
                TextStyle = new() {
                    FontSize = 26,
                    Color = Color.red,
                    FontStyles = FontStyles.Bold | FontStyles.Italic,
                    TextAlignementOptions = TextAlignmentOptions.Justified,
                }
            };

            GameObject textGameObject = _factory.GetOrCreateText(_container, text, settings);

            Assert.AreEqual(settings.Transform.Position, ((RectTransform)textGameObject.transform).position);
            Assert.AreEqual(settings.Transform.Size, ((RectTransform)textGameObject.transform).localScale);
            Assert.AreEqual(settings.Anchor.AnchorMin, ((RectTransform)textGameObject.transform).anchorMin);
            Assert.AreEqual(settings.Anchor.AnchorMax, ((RectTransform)textGameObject.transform).anchorMax);
            Assert.AreEqual(settings.Anchor.Pivot, ((RectTransform)textGameObject.transform).pivot);

            var textObject = textGameObject.GetComponentInChildren<TMP_Text>();
            Assert.AreEqual(text, textObject.text);
            Assert.AreEqual(settings.TextStyle.FontSize, textObject.fontSize);
            Assert.AreEqual(settings.TextStyle.Color, textObject.color);
            Assert.AreEqual(settings.TextStyle.FontStyles, textObject.fontStyle);
            Assert.AreEqual(settings.TextStyle.TextAlignementOptions, textObject.alignment);

        }

        [Test]
        public void GivenValidArgumentsAndTextInPool_WhenCreatingText_ThenTextIsReUsedndActive()
        {
            var modelContainer = new GameObject().AddComponent<TextModelContainer>();
            modelContainer.gameObject.SetActive(false);
            _factory._pool.Enqueue(modelContainer);

            var textGameObject = _factory.GetOrCreateText(_container, "");

            Assert.AreEqual(_factory._pool.Count, 0);
            Assert.IsTrue(textGameObject.activeInHierarchy);
        }
    }

    public class ReturnTextTests
    {
        TextFactory _factory;
        Transform _container;

        [SetUp]
        public void SetUp()
        {
            _factory = new GameObject().AddComponent<TextFactory>();
            _container = new GameObject().transform;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_factory.gameObject);
            GameObject.DestroyImmediate(_container.gameObject);
        }

        [Test]
        public void GivenText_WhenReturning_ThenAddedToPool()
        {
            var modelContainer = new GameObject().AddComponent<TextModelContainer>();
            _factory.ReturnText(modelContainer.gameObject);

            Assert.AreEqual(1, _factory._pool.Count);
        }

        [Test]
        public void GivenText_WhenReturning_ThenDisabled()
        {
            var modelContainer = new GameObject().AddComponent<TextModelContainer>();
            _factory.ReturnText(modelContainer.gameObject);

            Assert.IsFalse(modelContainer.gameObject.activeInHierarchy);
            Assert.AreEqual(_factory.transform, modelContainer.transform.parent);
        }

        [Test]
        public void GivenEmptyGameObject_WhenReturning_ThenNotAddedToPool()
        {
            var gameObject = new GameObject();
            gameObject.transform.SetParent(_container, false);

            _factory.ReturnText(gameObject);

            Assert.AreEqual(0, _factory._pool.Count);
        }

        [Test]
        public void GivenNull_WhenReturning_ThenNotAddedToPool()
        {
            _factory.ReturnText(null);

            Assert.AreEqual(0, _factory._pool.Count);
        }
    }
}