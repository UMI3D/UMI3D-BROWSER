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
using UnityEngine;

public class ImageModelTests
{
    public class SetSpriteTests
    {
        [Test]
        public void GivenValidSprite_WhenSettingSprite_ThenSprite()
        {
            var model = new ImageModel();
            var sprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), Vector2.zero);
            model.SetSprite(sprite);

            Assert.AreEqual(sprite, model.Sprite);
        }

        [Test]
        public void GivenNull_WhenSettingSprite_ThenNull()
        {
            var model = new ImageModel();
            model.SetSprite(null);

            Assert.IsNull(model.Sprite);
        }
    }

    public class SetColorTests
    {
        [Test]
        public void GivenColor_WhenSettingColor_ThenColor()
        {
            var model = new ImageModel();
            var color = Color.blue;
            model.SetColor(color);

            Assert.AreEqual(color, model.Color);
        }
    }
}