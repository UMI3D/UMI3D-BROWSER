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
using umi3d.browserRuntime.image;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ImageFactoryTests
{
    public class GetOrCreateImageTests
    {
        ImageFactory _factory;
        Transform _container;

        [SetUp]
        public void SetUp()
        {
            _factory = new GameObject().AddComponent<ImageFactory>();
            _container = new GameObject().transform;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_factory.gameObject);
            GameObject.DestroyImmediate(_container.gameObject);
        }

        [Test]
        public void Given_WhenCreatingImage_ThenImageCreated()
        {
            GameObject ImageGameObject = _factory.GetOrCreateImage(_container, null, new ImageFactory.Settings());

            Assert.IsNotNull(ImageGameObject);
            Assert.AreEqual(_container, ImageGameObject.transform.parent);
        }

        [Test]
        public void GivenValidArguments_WhenCreatingImage_ThenImageCreatedAndConfigured()
        {
            var sprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), Vector2.zero);
            var settings = new ImageFactory.Settings() {
                Color = Color.blue,
                Transform = new() {
                    Position = Vector2.one,
                    Size = Vector2.one / 2,
                },
                Anchor = new() {
                    AnchorMin = Vector2.zero,
                    AnchorMax = Vector2.one,
                    Pivot = Vector2.zero
                }
            };
            GameObject ImageGameObject = _factory.GetOrCreateImage(_container, sprite, settings);

            Assert.AreEqual(settings.Transform.Position, ((RectTransform)ImageGameObject.transform).position);
            Assert.AreEqual(settings.Transform.Size, ((RectTransform)ImageGameObject.transform).localScale);
            Assert.AreEqual(settings.Anchor.AnchorMin, ((RectTransform)ImageGameObject.transform).anchorMin);
            Assert.AreEqual(settings.Anchor.AnchorMax, ((RectTransform)ImageGameObject.transform).anchorMax);
            Assert.AreEqual(settings.Anchor.Pivot, ((RectTransform)ImageGameObject.transform).pivot);


            var ImageObject = ImageGameObject.GetComponentInChildren<Image>();
            Assert.AreEqual(sprite, ImageObject.sprite);
            Assert.AreEqual(settings.Color, ImageObject.color);
        }

        [Test]
        public void GivenValidArgumentsAndImageInPool_WhenCreatingImage_ThenImageIsReUsedndActive()
        {
            var modelContainer = new GameObject().AddComponent<ImageModelContainer>();
            modelContainer.gameObject.SetActive(false);
            _factory._pool.Enqueue(modelContainer);

            var ImageGameObject = _factory.GetOrCreateImage(_container, null, new ImageFactory.Settings());

            Assert.AreEqual(_factory._pool.Count, 0);
            Assert.IsTrue(ImageGameObject.activeInHierarchy);
        }
    }

    public class ReturnImageTests
    {
        ImageFactory _factory;
        Transform _container;

        [SetUp]
        public void SetUp()
        {
            _factory = new GameObject().AddComponent<ImageFactory>();
            _container = new GameObject().transform;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_factory.gameObject);
            GameObject.DestroyImmediate(_container.gameObject);
        }

        [Test]
        public void GivenImage_WhenReturning_ThenAddedToPool()
        {
            var modelContainer = new GameObject().AddComponent<ImageModelContainer>();
            _factory.ReturnImage(modelContainer.gameObject);

            Assert.AreEqual(1, _factory._pool.Count);
        }

        [Test]
        public void GivenImage_WhenReturning_ThenDisabled()
        {
            var modelContainer = new GameObject().AddComponent<ImageModelContainer>();
            _factory.ReturnImage(modelContainer.gameObject);

            Assert.IsFalse(modelContainer.gameObject.activeInHierarchy);
            Assert.AreEqual(_factory.transform, modelContainer.transform.parent);
        }

        [Test]
        public void GivenEmptyGameObject_WhenReturning_ThenNotAddedToPool()
        {
            var gameObject = new GameObject();
            gameObject.transform.SetParent(_container, false);

            _factory.ReturnImage(gameObject);

            Assert.AreEqual(0, _factory._pool.Count);
        }

        [Test]
        public void GivenNull_WhenReturning_ThenNotAddedToPool()
        {
            _factory.ReturnImage(null);

            Assert.AreEqual(0, _factory._pool.Count);
        }
    }
}