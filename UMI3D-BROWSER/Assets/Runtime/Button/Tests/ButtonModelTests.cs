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

            Assert.AreEqual(string.Empty, model.Label);
        }
    }

    public class SetCallbackTests
    {
        [Test]
        public void GivenCallback_WhenClicking_CallbackCalled()
        {
            var callbackCalled = false;
            ButtonModel model = new ButtonModel();
            model.SetCallback(() => callbackCalled = true);

            Assert.IsNotNull(model._callback);
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
    }

    public class SetPositionTests
    {
        [Test]
        public void GivenPosition_WhenSettingPosition_ThenPosition()
        {
            var position = new Vector3(200, 200, 0);
            ButtonModel model = new ButtonModel();
            model.SetPosition(position);

            Assert.AreEqual(position, model.Position);
        }
    }

    public class SetSizeTests
    {
        [Test]
        public void GivenSize_WhenSettingSize_ThenSize()
        {
            var size = new Vector3(2, 2, 2);
            ButtonModel model = new ButtonModel();
            model.SetSize(size);

            Assert.AreEqual(size, model.Size);
        }
    }

    public class SetAnchorTests
    {
        [Test]
        public void GivenAnchor_WhenSettingAnchor_ThenAnchor()
        {
            var anchorMin = new Vector2(0, 0);
            var anchorMax = new Vector2(1, 1);
            var pivot = new Vector2(0, 0);
            ButtonModel model = new ButtonModel();
            model.SetAnchor(anchorMin, anchorMax, pivot);

            Assert.AreEqual(anchorMin, model.AnchorMin);
            Assert.AreEqual(anchorMax, model.AnchorMax);
            Assert.AreEqual(pivot, model.Pivot);
        }
    }

    public class SetTextStyleTests
    {
        [Test]
        public void GivenTextStyle_WhenSettingTextStyle_ThenTextStyle()
        {
            var textSize = 26;
            var textColor = Color.red;
            var textStyles = FontStyles.Bold | FontStyles.Italic;
            var textAlignements = TextAlignmentOptions.Justified;
            ButtonModel model = new ButtonModel();
            model.SetTextStyle(textSize, textColor, textStyles, textAlignements);

            Assert.AreEqual(textSize, model.TextFontSize);
            Assert.AreEqual(textColor, model.TextColor);
            Assert.AreEqual(textStyles, model.TextStyles);
            Assert.AreEqual(textAlignements, model.TextAlignementOptions);
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