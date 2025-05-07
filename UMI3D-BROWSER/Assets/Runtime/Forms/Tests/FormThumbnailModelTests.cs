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
using UnityEngine;
using umi3d.browserRuntime.forms;

public class FormThumbnailModelTests
{
    public class SetHeaderTests
    {
        [Test]
        public void GivenStringAndColor_WhenSettingColor_ThenHeaderValuesSet()
        {
            var headerText = "HeaderText";
            var indicatorColor = Color.green;

            var model = new FormThumbnailModel();
            model.SetHeader(headerText, indicatorColor);

            Assert.AreEqual(headerText, model.HeaderText);
            Assert.AreEqual(indicatorColor, model.IndicatorColor);
        }
    }
    public class SetLoadingTests
    {
        [Test]
        public void GivenStringAndColor_WhenSettingColor_ThenHeaderValuesSet()
        {
            var model = new FormThumbnailModel();
            model.SetIsLoading(true);

            Assert.IsTrue(model.IsLoading);
        }
    }
}