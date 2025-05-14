using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui;
using UnityEngine;
using UnityEngine.TestTools;

public class ToggleModelTests
{
    public class SetLabelTests
    {
        private ToggleModel _toggleModel;

        [SetUp]
        public void SetUp()
        {
            _toggleModel = new ToggleModel();
        }

        [Test]
        public void GivenInitialState_WhenSettingLabelToNonEmptyString_ThenLabelIsVisible()
        {
            // Given
            Assert.IsFalse(_toggleModel.isLabelVisible);
            Assert.IsNull(_toggleModel.label);

            // When
            _toggleModel.SetLabel("Test Label");

            // Then
            Assert.IsTrue(_toggleModel.isLabelVisible);
            Assert.AreEqual("Test Label", _toggleModel.label);
        }

        [Test]
        public void GivenInitialState_WhenSettingLabelToEmptyString_ThenLabelIsNotVisible()
        {
            // Given
            Assert.IsFalse(_toggleModel.isLabelVisible);
            Assert.IsNull(_toggleModel.label);

            // When
            _toggleModel.SetLabel("");

            // Then
            Assert.IsFalse(_toggleModel.isLabelVisible);
            Assert.AreEqual("", _toggleModel.label);
        }

        [Test]
        public void GivenLabelIsVisible_WhenSettingLabelToNull_ThenLabelIsNotVisible()
        {
            // Given
            _toggleModel.SetLabel("Initial Label");
            Assert.IsTrue(_toggleModel.isLabelVisible);
            Assert.AreEqual("Initial Label", _toggleModel.label);

            // When
            _toggleModel.SetLabel(null);

            // Then
            Assert.IsFalse(_toggleModel.isLabelVisible);
            Assert.IsNull(_toggleModel.label);
        }
    }

    public class SetValueTests
    {
        private ToggleModel _toggleModel;

        [SetUp]
        public void SetUp()
        {
            _toggleModel = new ToggleModel();
        }

        [Test]
        public void GivenInitialState_WhenSettingValueToTrue_ThenValueIsTrue()
        {
            // Given
            Assert.IsFalse(_toggleModel.value);

            // When
            _toggleModel.SetValue(true);

            // Then
            Assert.IsTrue(_toggleModel.value);
        }

        [Test]
        public void GivenInitialState_WhenSettingValueToFalse_ThenValueIsFalse()
        {
            // Given
            Assert.IsFalse(_toggleModel.value);

            // When
            _toggleModel.SetValue(false);

            // Then
            Assert.IsFalse(_toggleModel.value);
        }
    }
}
