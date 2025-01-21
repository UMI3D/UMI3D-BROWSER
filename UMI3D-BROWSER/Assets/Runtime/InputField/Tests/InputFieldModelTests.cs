using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui.inputField;
using UnityEngine;
using UnityEngine.TestTools;

public class InputFieldModelTests
{
    class SetLabelTests
    {
        InputFieldModel _model;

        [SetUp]
        public void SetUp()
        {
            _model = new InputFieldModel();
        }

        [TearDown]
        public void TearDown()
        {
            _model = null;
        }

        [Test]
        public void GivenLabelNullAndNotVisible_WhenSetLabel_ThenLabelVisible()
        {
            Assert.IsNull(_model.label);

            string label = "Test Label";
            _model.SetLabel(label);

            Assert.AreEqual(_model.label, label);
            Assert.IsTrue(_model.isLabelVisible);
        }

        [Test]
        public void GivenLabelNullAndNotVisible_WhenSetLabelEmpty_ThenLabelEmptyAndNotVisible()
        {
            Assert.IsNull(_model.label);

            string label = "";
            _model.SetLabel(label);

            Assert.AreEqual(_model.label, label);
            Assert.IsFalse(_model.isLabelVisible);
        }

        [Test]
        public void GivenLabelNullAndNotVisible_WhenSetLabelNull_ThenLabelNullAndNotVisible()
        {
            Assert.IsNull(_model.label);

            string label = null;
            _model.SetLabel(label);

            Assert.AreEqual(_model.label, label);
            Assert.IsFalse(_model.isLabelVisible);
        }
    }

    class SetValueTests
    {
        InputFieldModel _model;

        [SetUp]
        public void SetUp()
        {
            _model = new InputFieldModel();
        }

        [TearDown]
        public void TearDown()
        {
            _model = null;
        }

        [Test]
        public void GivenValueNull_WhenSetValue_ThenValue()
        {
            Assert.IsNull(_model.value);

            string value = "Test Value";
            _model.SetValue(value);

            Assert.AreEqual(_model.value, value);
        }
    }

    class UpdateValueTests
    {
        InputFieldModel _model;

        [SetUp]
        public void SetUp()
        {
            _model = new InputFieldModel();
        }

        [TearDown]
        public void TearDown()
        {
            _model = null;
        }

        [Test]
        public void GivenValueNull_WhenSetValue_ThenValue()
        {
            Assert.IsNull(_model.value);

            string value = "Test Value";
            _model.UpdateValue(value);

            Assert.AreEqual(_model.value, value);
        }
    }

    class SetPlaceholderTests
    {
        InputFieldModel _model;

        [SetUp]
        public void SetUp()
        {
            _model = new InputFieldModel();
        }

        [TearDown]
        public void TearDown()
        {
            _model = null;
        }

        [Test]
        public void GivenPlaceholderNull_WhenSetPlaceholder_ThenPlaceholder()
        {
            Assert.IsNull(_model.placeholder);

            string placeholder = "Test Placeholder";
            _model.SetPlaceholder(placeholder);

            Assert.AreEqual(_model.placeholder, placeholder);
        }
    }

    class SetNbrLinesTests
    {
        InputFieldModel _model;

        [SetUp]
        public void SetUp()
        {
            _model = new InputFieldModel();
        }

        [TearDown]
        public void TearDown()
        {
            _model = null;
        }

        [Test]
        public void GivenNbrLineOne_WhenSetNbrLinesTwo_ThenNbrLineTwo()
        {
            Assert.AreEqual(_model.nbrLine, 1);

            int nbrLine = 2;
            _model.SetNbrLines(nbrLine);

            Assert.AreEqual(_model.nbrLine, nbrLine);
        }
    }
}
