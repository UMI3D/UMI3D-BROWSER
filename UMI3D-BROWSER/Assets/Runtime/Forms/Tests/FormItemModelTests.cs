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
using umi3d.browserRuntime.forms;
using UnityEngine;

public class FormItemModelTests
{
    public class Test
    {

        public class SetPositionTests
        {
            [Test]
            public void GivenPosition_WhenSettingPosition_ThenPosition()
            {
                var position = new Vector3(200, 200, 0);
                FormItemModel model = new FormItemModel();
                model.SetPosition(position);

                Assert.AreEqual(position, model.Position);
            }

            [Test]
            public void GivenNull_WhenSettingPosition_ThenNothingChange()
            {
                var position = new Vector3(200, 200, 0);
                FormItemModel model = new FormItemModel();
                model.SetPosition(position);
                model.SetPosition(null);

                Assert.AreEqual(position, model.Position);
            }
        }

        public class SetSizeTests
        {
            [Test]
            public void GivenSize_WhenSettingSize_ThenSize()
            {
                var size = new Vector2(2, 2);
                FormItemModel model = new FormItemModel();
                model.SetSize(size);

                Assert.AreEqual(size, model.Size);
            }

            [Test]
            public void GivenNull_WhenSettingSize_ThenNothingChange()
            {
                var size = new Vector2(2, 2);
                FormItemModel model = new FormItemModel();
                model.SetSize(size);
                model.SetSize(null);

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
                FormItemModel model = new FormItemModel();
                model.SetAnchor(anchorMin, anchorMax, pivot);

                Assert.AreEqual(anchorMin, model.AnchorMin);
                Assert.AreEqual(anchorMax, model.AnchorMax);
                Assert.AreEqual(pivot, model.Pivot);
            }

            [Test]
            public void GivenNull_WhenSettingAnchor_ThenNothingChange()
            {
                var anchorMin = new Vector2(0, 0);
                var anchorMax = new Vector2(1, 1);
                var pivot = new Vector2(0, 0);
                FormItemModel model = new FormItemModel();
                model.SetAnchor(anchorMin, anchorMax, pivot);
                model.SetAnchor(null, null, null);

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
                FormItemModel model = new FormItemModel();
                model.SetTextStyle(textSize, textColor, textStyles, textAlignements);

                Assert.AreEqual(textSize, model.TextFontSize);
                Assert.AreEqual(textColor, model.TextColor);
                Assert.AreEqual(textStyles, model.TextStyles);
                Assert.AreEqual(textAlignements, model.TextAlignmentOptions);
            }

            [Test]
            public void GivenNull_WhenSettingTextStyle_ThenNothingChange()
            {
                var textSize = 26;
                var textColor = Color.red;
                var textStyles = FontStyles.Bold | FontStyles.Italic;
                var textAlignements = TextAlignmentOptions.Justified;
                FormItemModel model = new FormItemModel();
                model.SetTextStyle(textSize, textColor, textStyles, textAlignements);
                model.SetTextStyle(null, null, null, null);

                Assert.AreEqual(textSize, model.TextFontSize);
                Assert.AreEqual(textColor, model.TextColor);
                Assert.AreEqual(textStyles, model.TextStyles);
                Assert.AreEqual(textAlignements, model.TextAlignmentOptions);
            }
        }
    }
}