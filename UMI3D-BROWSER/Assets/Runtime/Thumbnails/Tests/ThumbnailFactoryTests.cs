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
using System;
using NUnit.Framework;
using TMPro;
using umi3d.browserRuntime.thumbnails;
using UnityEngine;
using UnityEngine.UI;

public class ThumbnailFactoryTests
{
    public class GetOrCreateThumbnailTests
    {
        ThumbnailFactory _factory;
        Transform _container;

        [SetUp]
        public void SetUp()
        {
            _factory = new GameObject().AddComponent<ThumbnailFactory>();
            _container = new GameObject().transform;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_factory.gameObject);
            GameObject.DestroyImmediate(_container.gameObject);
        }

        [Test]
        public void Given_WhenCreatingThumbnail_ThenThumbnailCreated()
        {
            GameObject ThumbnailGameObject = _factory.GetOrCreateThumbnail(_container);

            Assert.IsNotNull(ThumbnailGameObject);
            Assert.AreEqual(_container, ThumbnailGameObject.transform.parent);
        }

        [Test]
        public void GivenValidArguments_WhenCreatingThumbnail_ThenThumbnailCreatedAndConfigured()
        {
            var callbackCalled = false;
            var name = "TestName";
            var image = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(.5f, .5f));
            Action callback = () => callbackCalled = true;
            var settings = new ThumbnailFactory.Settings() {
                NormalColor = Color.red,
                HoverColor = Color.green,
                Position = Vector3.one,
                Size = Vector3.one / 2,
                AnchorMin = Vector2.zero,
                AnchorMax = Vector2.one,
                Pivot = Vector2.zero,
            };

            GameObject thumbnailGameObject = _factory.GetOrCreateThumbnail(_container, name, image, callback, settings);

            Assert.AreEqual(settings.Position, ((RectTransform)thumbnailGameObject.transform).position);
            Assert.AreEqual(settings.Size, ((RectTransform)thumbnailGameObject.transform).sizeDelta);
            Assert.AreEqual(settings.AnchorMin, ((RectTransform)thumbnailGameObject.transform).anchorMin);
            Assert.AreEqual(settings.AnchorMax, ((RectTransform)thumbnailGameObject.transform).anchorMax);
            Assert.AreEqual(settings.Pivot, ((RectTransform)thumbnailGameObject.transform).pivot);

            var text = thumbnailGameObject.GetComponentInChildren<TMP_Text>();
            Assert.AreEqual(name, text.text);

            var imageObject = thumbnailGameObject.GetComponentInChildren<ThumbnailImageView>().GetComponent<Image>();
            Assert.AreEqual(image, imageObject.sprite);
            Assert.AreEqual(settings.NormalColor, imageObject.color);

            var button = thumbnailGameObject.GetComponent<Button>();
            button.onClick?.Invoke();
            Assert.IsTrue(callbackCalled);
        }

        [Test]
        public void GivenValidArgumentsAndThumbnailInPool_WhenCreatingThumbnail_ThenThumbnailIsReUsedAndActive()
        {
            var modelContainer = new GameObject().AddComponent<ThumbnailModelContainer>();
            modelContainer.gameObject.SetActive(false);
            _factory._pool.Enqueue(modelContainer);

            var ThumbnailGameObject = _factory.GetOrCreateThumbnail(_container);

            Assert.AreEqual(_factory._pool.Count, 0);
            Assert.IsTrue(ThumbnailGameObject.activeInHierarchy);
        }
    }

    public class ReturnThumbnailTests
    {
        ThumbnailFactory _factory;
        Transform _container;

        [SetUp]
        public void SetUp()
        {
            _factory = new GameObject().AddComponent<ThumbnailFactory>();
            _container = new GameObject().transform;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_factory.gameObject);
            GameObject.DestroyImmediate(_container.gameObject);
        }

        [Test]
        public void GivenThumbnail_WhenReturning_ThenAddedToPool()
        {
            var modelContainer = new GameObject().AddComponent<ThumbnailModelContainer>();
            _factory.ReturnThumbnail(modelContainer.gameObject);

            Assert.AreEqual(1, _factory._pool.Count);
        }

        [Test]
        public void GivenThumbnail_WhenReturning_ThenDisabled()
        {
            var modelContainer = new GameObject().AddComponent<ThumbnailModelContainer>();
            _factory.ReturnThumbnail(modelContainer.gameObject);

            Assert.IsFalse(modelContainer.gameObject.activeInHierarchy);
            Assert.AreEqual(_factory.transform, modelContainer.transform.parent);
            Assert.IsNull(modelContainer.Model._callback);
        }

        [Test]
        public void GivenEmptyGameObject_WhenReturning_ThenNotAddedToPool()
        {
            var gameObject = new GameObject();
            gameObject.transform.SetParent(_container, false);

            _factory.ReturnThumbnail(gameObject);

            Assert.AreEqual(0, _factory._pool.Count);
        }

        [Test]
        public void GivenNull_WhenReturning_ThenNotAddedToPool()
        {
            _factory.ReturnThumbnail(null);

            Assert.AreEqual(0, _factory._pool.Count);
        }
    }
}