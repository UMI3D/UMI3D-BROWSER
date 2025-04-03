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

        [SetUp]
        public void SetUp()
        {
            _factory = new GameObject().AddComponent<ThumbnailFactory>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_factory.gameObject);
        }

        [Test]
        public void Given_WhenCreatingThumbnail_ThenThumbnailCreated()
        {
            var thumbnailModelContainer = _factory.GetOrCreateThumbnail();

            Assert.IsNotNull(thumbnailModelContainer);
            Assert.AreEqual(_factory._content, thumbnailModelContainer.transform.parent);
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
            };

            var thumbnailModelContainer = _factory.GetOrCreateThumbnail( name, image, callback, settings);

            var text = thumbnailModelContainer.GetComponentInChildren<TMP_Text>();
            Assert.AreEqual(name, text.text);

            var imageObject = thumbnailModelContainer.GetComponentInChildren<ThumbnailImageView>().GetComponent<Image>();
            Assert.AreEqual(image, imageObject.sprite);
            Assert.AreEqual(settings.NormalColor, imageObject.color);

            var button = thumbnailModelContainer.GetComponent<Button>();
            button.onClick?.Invoke();
            Assert.IsTrue(callbackCalled);
        }

        [Test]
        public void GivenValidArgumentsAndThumbnailInPool_WhenCreatingThumbnail_ThenThumbnailIsReUsedAndActive()
        {
            var modelContainer = new GameObject().AddComponent<ThumbnailModelContainer>();
            modelContainer.gameObject.SetActive(false);
            _factory._pool.Enqueue(modelContainer);

            var thumbnailModelContainer = _factory.GetOrCreateThumbnail();

            Assert.AreEqual(0, _factory._pool.Count);
            Assert.IsTrue(thumbnailModelContainer.gameObject.activeInHierarchy);
        }
    }

    public class ReturnThumbnailTests
    {
        ThumbnailFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new GameObject().AddComponent<ThumbnailFactory>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(_factory.gameObject);
        }

        [Test]
        public void GivenThumbnail_WhenReturning_ThenAddedToPool()
        {
            var modelContainer = new GameObject().AddComponent<ThumbnailModelContainer>();
            _factory.ReturnThumbnail(modelContainer);

            Assert.AreEqual(1, _factory._pool.Count);
        }

        [Test]
        public void GivenThumbnail_WhenReturning_ThenDisabled()
        {
            var modelContainer = new GameObject().AddComponent<ThumbnailModelContainer>();
            _factory.ReturnThumbnail(modelContainer);

            Assert.IsFalse(modelContainer.gameObject.activeInHierarchy);
            Assert.AreEqual(_factory.transform, modelContainer.transform.parent);
            Assert.IsNull(modelContainer.Model._callback);
        }

        [Test]
        public void GivenNull_WhenReturning_ThenNotAddedToPool()
        {
            _factory.ReturnThumbnail(null);

            Assert.AreEqual(0, _factory._pool.Count);
        }
    }
}