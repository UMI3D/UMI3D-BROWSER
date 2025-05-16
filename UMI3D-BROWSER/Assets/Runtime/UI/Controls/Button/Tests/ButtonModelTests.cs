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
using umi3d.browserRuntime.ui;
using UnityEngine;
using UnityEngine.UI;

public class ButtonModelTests
{
    public class SetLabelTests
    {
        [Test]
        public void GivenLabel_WhenSetLabel_ThenLabelUpdated()
        {
            var label = "TestLabel";
            ButtonModel model = new ButtonModel();

            model.SetLabel(label);

            Assert.AreEqual(label, model.Label);
        }

        [Test]
        public void GivenLabelNull_WhenSetLabel_ThenLabelEmpty()
        {
            ButtonModel model = new ButtonModel();

            model.SetLabel(null);

            Assert.Null(model.Label);
        }
    }

    public class SetImageTests
    {
        [Test]
        public void GivenImage_WhenSettingImage_ThenImage()
        {
            var sprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(.5f, .5f));
            var colors = new ColorBlock() { normalColor = Color.blue };
            ButtonModel model = new ButtonModel();
            model.SetImage(colors, sprite);

            Assert.AreEqual(sprite, model.Sprite);
            Assert.AreEqual(colors, model.ColorBlock);
        }

        [Test]
        public void GivenNull_WhenSettingImage_ThenDefault()
        {
            ButtonModel model = new ButtonModel();
            model.SetImage(null, null);

            Assert.IsNull(model.Sprite);
            Assert.AreEqual(ColorBlock.defaultColorBlock, model.ColorBlock);
        }
    }

    public class ClickTests
    {
        [Test]
        public void GivenCallback_WhenClicking_CallbackCalled()
        {
            var callbackCalled = false;
            ButtonModel model = new ButtonModel();
            model.SetCallback(() => callbackCalled = true);

            model.Click();

            Assert.IsTrue(callbackCalled);
        }
    }
}