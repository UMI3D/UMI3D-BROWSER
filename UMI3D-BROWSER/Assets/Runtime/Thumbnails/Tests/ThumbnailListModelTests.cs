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
using System.Linq;
using NUnit.Framework;
using umi3d.browserRuntime.thumbnails;
using UnityEngine;

public class ThumbnailListModelTests
{
    public class AddThumbnailTest
    {
        ThumbnailListModelContainer modelContainer;

        [SetUp]
        public void SetUp()
        {
            modelContainer = new GameObject().AddComponent<ThumbnailListModelContainer>();
            modelContainer.gameObject.AddComponent<ThumbnailFactory>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(modelContainer.gameObject);
        }

        [Test]
        public void Given_WhenAddingThumbnail_ThenCreatedAndAddedToList()
        {
            var model = modelContainer.Model;
            model.AddThumbnail();

            Assert.AreEqual(1, model.Thumbnails.Count);
            Assert.AreEqual(1, model.ThumbnailContainers.Count);
        }
        [Test]
        public void Given2Thumbnail_WhenAddingThumbnail_ThenThumbnailTempsCreated()
        {
            var model = modelContainer.Model;

            model.AddThumbnail();
            Assert.AreEqual(Mathf.Max(model.Mode.NbrColumn * model.Mode.NbrRow - model.Thumbnails.Count, 0), model._thumbnailContainersTemp.Count);
            model.AddThumbnail();
            Assert.AreEqual(Mathf.Max(model.Mode.NbrColumn * model.Mode.NbrRow - model.Thumbnails.Count, 0), model._thumbnailContainersTemp.Count);
        }

        [Test]
        public void GivenValidArgument_WhenAddingThumbnail_ThenCreatedAndAddedToList()
        {
            var name = "TestName";
            var image = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(.5f, .5f));
            var callbackCalled = false;
            Action callback = () => callbackCalled = true;
            var settings = new ThumbnailFactory.Settings() {
                NormalColor = Color.red,
                HoverColor = Color.green,
            };

            var model = modelContainer.Model;
            model.AddThumbnail(name, image, callback, settings);

            var thumbnailModel = model.Thumbnails.FirstOrDefault();
            Assert.IsNotNull(thumbnailModel);

            Assert.AreEqual(name, thumbnailModel.Name);
            Assert.AreEqual(image, thumbnailModel.Image);
            Assert.AreEqual(settings.NormalColor, thumbnailModel.NormalColor);
            Assert.AreEqual(settings.HoverColor, thumbnailModel.HoverColor);
            thumbnailModel.Click();
            Assert.IsTrue(callbackCalled);
        }
    }

    public class ClearThumbnailsTest
    {
        ThumbnailListModelContainer modelContainer;

        [SetUp]
        public void SetUp()
        {
            modelContainer = new GameObject().AddComponent<ThumbnailListModelContainer>();
            modelContainer.gameObject.AddComponent<ThumbnailFactory>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(modelContainer.gameObject);
        }

        [Test]
        public void GivenThumbnails_WhenClearingThumbnails_ThenEmpty()
        {
            var model = modelContainer.Model;
            model.AddThumbnail();
            model.AddThumbnail();

            model.ClearThumbnails();
            Assert.AreEqual(0, model.Thumbnails.Count);
            Assert.AreEqual(0, model.ThumbnailContainers.Count);
        }
    }

    public class ToggleDisplayModeTest
    {
        ThumbnailListModelContainer modelContainer;

        [SetUp]
        public void SetUp()
        {
            modelContainer = new GameObject().AddComponent<ThumbnailListModelContainer>();
            modelContainer.gameObject.AddComponent<ThumbnailFactory>();
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(modelContainer.gameObject);
        }

        [Test]
        public void GivenThumbnails_WhenClearingThumbnails_ThenEmpty()
        {
            var model = modelContainer.Model;
            model.AddThumbnail();
            model.AddThumbnail();

            model.ChangeModeTo(new ThumbnailMode() {
                NbrRow = 2,
                NbrColumn = 4,
                Spacing = 11
            });

            Assert.AreEqual(2, model.Thumbnails.Count);
            Assert.AreEqual(2, model.ThumbnailContainers.Count);
            Assert.AreEqual(Mathf.Max(model.Mode.NbrColumn * model.Mode.NbrRow - model.Thumbnails.Count, 0), model._thumbnailContainersTemp.Count);
        }
    }
}