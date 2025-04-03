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
using umi3d.browserRuntime.thumbnails;
using UnityEngine;

public class ThumbnailModelTests
{
    public class SetNameTests
    {
        [Test]
        public void GivenString_WhenSettingName_ThenName()
        {
            var name = "TestName";
            var model = new ThumbnailModel();
            model.SetName(name);

            Assert.AreEqual(name, model.Name);
        }

        [Test]
        public void GivenNull_WhenSettingName_ThenEmptyName()
        {
            var model = new ThumbnailModel();
            model.SetName(null);

            Assert.AreEqual(string.Empty, model.Name);
        }
    }

    public class UpdateNameTests
    {
        [Test]
        public void GivenString_WhenUpdatingName_ThenName()
        {
            var name = "TestName";
            var model = new ThumbnailModel();
            model.UpdateName(name);

            Assert.AreEqual(name, model.Name);
        }

        [Test]
        public void GivenNull_WhenSettingName_ThenEmptyName()
        {
            var model = new ThumbnailModel();
            model.UpdateName(null);

            Assert.AreEqual(string.Empty, model.Name);
        }
    }

    public class SetImageTests
    {
        [Test]
        public void GivenSprite_WhenSettingImage_ThenImage()
        {
            var image = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(.5f, .5f));
            var model = new ThumbnailModel();
            model.SetImage(image);

            Assert.AreEqual(image, model.Image);
        }

        [Test]
        public void GivenNull_WhenSettingImage_ThenNull()
        {
            var model = new ThumbnailModel();
            model.SetImage(null);

            Assert.AreEqual(null, model.Image);
        }
    }

    public class SetColorsTests
    {
        [Test]
        public void GivenColors_WhenSettingColors_ThenColors()
        {
            var normalColor = Color.black;
            var hoverColor = Color.red;
            var model = new ThumbnailModel();
            model.SetColors(normalColor, hoverColor);

            Assert.AreEqual(normalColor, model.NormalColor);
            Assert.AreEqual(hoverColor, model.HoverColor);
        }

        [Test]
        public void GivenNull_WhenSettingColors_ThenWhite()
        {
            var normalColor = Color.black;
            var hoverColor = Color.red;
            var model = new ThumbnailModel();
            model.SetColors(normalColor, hoverColor);
            model.SetColors(null, null);

            Assert.AreEqual(Color.white, model.NormalColor);
            Assert.AreEqual(Color.white, model.HoverColor);
        }
    }

    public class UpdateHoverTests
    {
        [Test]
        public void GivenTrue_WhenSettingHover_ThenTrue()
        {
            var model = new ThumbnailModel();
            model.UpdateHover(true);

            Assert.IsTrue(model.Hover);
        }
    }

    public class SetCallbackTests
    {
        [Test]
        public void GivenAction_WhenSettingCallback_ThenCallback()
        {
            var model = new ThumbnailModel();
            model.SetCallback(() => { });

            Assert.IsNotNull(model._callback);
        }

        [Test]
        public void GivenCallback_WhenClicking_ThenCallbackCalled()
        {
            var callbackCalled = false;
            var model = new ThumbnailModel();
            model.SetCallback(() => callbackCalled = true);

            model.Click();

            Assert.IsTrue(callbackCalled);
        }
    }
}