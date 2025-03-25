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
using umi3d.browserRuntime.button;

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