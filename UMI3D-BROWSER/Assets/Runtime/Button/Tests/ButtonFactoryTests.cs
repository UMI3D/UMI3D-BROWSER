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
using System;
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
        public void Given_WhenCreatingButton_ThenButtonCreated()
        {
            GameObject buttonGameObject = _factory.GetOrCreateButton(_container, "", null, new ButtonFactory.Settings());

            Assert.IsNotNull(buttonGameObject);
            Assert.AreEqual(_container, buttonGameObject.transform.parent);
        }

        [Test]
        public void GivenValidArguments_WhenCreatingButton_ThenButtonCreatedAndConfigured()
        {
            var callbackCalled = false;
            var label = "TestLabel";
            Action callback = () => callbackCalled = true;
            var settings = new ButtonFactory.Settings() {
                Image = new() {
                    Sprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(.5f, .5f)),
                    ColorBlock = new ColorBlock() { normalColor = Color.blue, highlightedColor = Color.red },
                },
                Transform = new() {
                    Position = Vector3.one,
                    Size = Vector3.one / 2
                },
                Anchor = new () {
                    AnchorMin = Vector2.zero,
                    AnchorMax = Vector2.one,
                    Pivot = Vector2.zero,
                },
                TextStyle = new () {
                    FontSize = 26,
                    Color = Color.red,
                    FontStyles = FontStyles.Bold | FontStyles.Italic,
                    TextAlignementOptions = TextAlignmentOptions.Justified,
                } 
            };

            GameObject buttonGameObject = _factory.GetOrCreateButton(_container, label, callback, settings);

            Assert.AreEqual(settings.Transform.Position, ((RectTransform)buttonGameObject.transform).position);
            Assert.AreEqual(settings.Transform.Size, ((RectTransform)buttonGameObject.transform).localScale);
            Assert.AreEqual(settings.Anchor.AnchorMin, ((RectTransform)buttonGameObject.transform).anchorMin);
            Assert.AreEqual(settings.Anchor.AnchorMax, ((RectTransform)buttonGameObject.transform).anchorMax);
            Assert.AreEqual(settings.Anchor.Pivot, ((RectTransform)buttonGameObject.transform).pivot);

            var text = buttonGameObject.GetComponentInChildren<TMP_Text>();
            Assert.AreEqual(label, text.text);
            Assert.AreEqual(settings.TextStyle.FontSize, text.fontSize);
            Assert.AreEqual(settings.TextStyle.Color, text.color);
            Assert.AreEqual(settings.TextStyle.FontStyles, text.fontStyle);
            Assert.AreEqual(settings.TextStyle.TextAlignementOptions, text.alignment);

            var button = buttonGameObject.GetComponent<Button>();
            Assert.AreEqual(settings.Image.Sprite, button.image.sprite);
            Assert.AreEqual(settings.Image.ColorBlock, button.colors);
            button.onClick?.Invoke();
            Assert.IsTrue(callbackCalled);
        }

        [Test]
        public void GivenValidArgumentsAndButtonInPool_WhenCreatingButton_ThenButtonIsReUsedndActive()
        {
            var modelContainer = new GameObject().AddComponent<ButtonModelContainer>();
            modelContainer.gameObject.SetActive(false);
            _factory._pool.Enqueue(modelContainer);

            var buttonGameObject = _factory.GetOrCreateButton(_container, "", null, new ButtonFactory.Settings());

            Assert.AreEqual(_factory._pool.Count, 0);
            Assert.IsTrue(buttonGameObject.activeInHierarchy);
        }
    }

    public class ReturnButtonTests
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
        public void GivenButton_WhenReturning_ThenAddedToPool()
        {
            var modelContainer = new GameObject().AddComponent<ButtonModelContainer>();
            _factory.ReturnButton(modelContainer.gameObject);

            Assert.AreEqual(1, _factory._pool.Count);
        }

        [Test]
        public void GivenButton_WhenReturning_ThenDisabled()
        {
            var modelContainer = new GameObject().AddComponent<ButtonModelContainer>();
            _factory.ReturnButton(modelContainer.gameObject);

            Assert.IsFalse(modelContainer.gameObject.activeInHierarchy);
            Assert.AreEqual(_factory.transform, modelContainer.transform.parent);
            Assert.IsNull(modelContainer.Model._callback);
        }

        [Test]
        public void GivenEmptyGameObject_WhenReturning_ThenNotAddedToPool()
        {
            var gameObject = new GameObject();
            gameObject.transform.SetParent(_container, false);

            _factory.ReturnButton(gameObject);

            Assert.AreEqual(0, _factory._pool.Count);
        }

        [Test]
        public void GivenNull_WhenReturning_ThenNotAddedToPool()
        {
            _factory.ReturnButton(null);

            Assert.AreEqual(0, _factory._pool.Count);
        }
    }
}