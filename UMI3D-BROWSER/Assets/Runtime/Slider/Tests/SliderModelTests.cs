using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui.slider;
using UnityEngine;
using UnityEngine.TestTools;

public class SliderModelTests
{
    public class SetLabel
    {
        private SliderModel sliderModel;

        [SetUp]
        public void SetUp()
        {
            sliderModel = new SliderModel();
        }

        [Test]
        public void GivenInitialState_WhenSettingLabelToNonEmptyString_ThenLabelIsVisible()
        {
            // Given
            Assert.IsFalse(sliderModel.isLabelVisible);
            Assert.IsNull(sliderModel.label);

            // When
            sliderModel.SetLabel("Test Label");

            // Then
            Assert.IsTrue(sliderModel.isLabelVisible);
            Assert.AreEqual("Test Label", sliderModel.label);
        }

        [Test]
        public void GivenInitialState_WhenSettingLabelToEmptyString_ThenLabelIsNotVisible()
        {
            // Given
            Assert.IsFalse(sliderModel.isLabelVisible);
            Assert.IsNull(sliderModel.label);

            // When
            sliderModel.SetLabel(string.Empty);

            // Then
            Assert.IsFalse(sliderModel.isLabelVisible);
            Assert.AreEqual(string.Empty, sliderModel.label);
        }

        [Test]
        public void GivenLabelIsSet_WhenSettingLabelToNull_ThenLabelIsNotVisible()
        {
            // Given
            sliderModel.SetLabel("Initial Label");
            Assert.IsTrue(sliderModel.isLabelVisible);
            Assert.AreEqual("Initial Label", sliderModel.label);

            // When
            sliderModel.SetLabel(null);

            // Then
            Assert.IsFalse(sliderModel.isLabelVisible);
            Assert.IsNull(sliderModel.label);
        }
    }

    public class SetValue
    {
        private SliderModel sliderModel;

        [SetUp]
        public void SetUp()
        {
            sliderModel = new SliderModel();
        }

        [Test]
        public void GivenInitialState_WhenSettingValueWithinRange_ThenValueIsUpdated()
        {
            // Given
            sliderModel.SetMinValue(0f);
            sliderModel.SetMaxValue(10f);
            Assert.AreEqual(0f, sliderModel.value);

            // When
            sliderModel.SetValue(5.5f);

            // Then
            Assert.AreEqual(5.5f, sliderModel.value);
        }

        [Test]
        public void GivenInitialState_WhenSettingValueAboveMaxValue_ThenValueIsClampedToMaxValue()
        {
            // Given
            sliderModel.SetMinValue(0f);
            sliderModel.SetMaxValue(10f);
            Assert.AreEqual(0f, sliderModel.value);

            // When
            sliderModel.SetValue(15f);

            // Then
            Assert.AreEqual(10f, sliderModel.value);
        }

        [Test]
        public void GivenInitialState_WhenSettingValueBelowMinValue_ThenValueIsClampedToMinValue()
        {
            // Given
            sliderModel.SetMinValue(0f);
            sliderModel.SetMaxValue(10f);
            Assert.AreEqual(0f, sliderModel.value);

            // When
            sliderModel.SetValue(-5f);

            // Then
            Assert.AreEqual(0f, sliderModel.value);
        }
    }

    public class UpdateValue
    {
        private SliderModel sliderModel;

        [SetUp]
        public void SetUp()
        {
            sliderModel = new SliderModel();
        }

        [Test]
        public void GivenInitialState_WhenUpdatingValueWithinRange_ThenValueIsUpdated()
        {
            // Given
            sliderModel.SetMinValue(0f);
            sliderModel.SetMaxValue(10f);
            Assert.AreEqual(0f, sliderModel.value);

            // When
            sliderModel.UpdateValue(5.5f);

            // Then
            Assert.AreEqual(5.5f, sliderModel.value);
        }

        [Test]
        public void GivenInitialState_WhenUpdatingValueAboveMaxValue_ThenValueIsClampedToMaxValue()
        {
            // Given
            sliderModel.SetMinValue(0f);
            sliderModel.SetMaxValue(10f);
            Assert.AreEqual(0f, sliderModel.value);

            // When
            sliderModel.UpdateValue(15f);

            // Then
            Assert.AreEqual(10f, sliderModel.value);
        }

        [Test]
        public void GivenInitialState_WhenUpdatingValueBelowMinValue_ThenValueIsClampedToMinValue()
        {
            // Given
            sliderModel.SetMinValue(0f);
            sliderModel.SetMaxValue(10f);
            Assert.AreEqual(0f, sliderModel.value);

            // When
            sliderModel.UpdateValue(-5f);

            // Then
            Assert.AreEqual(0f, sliderModel.value);
        }
    }

    public class SetMaxValue
    {
        private SliderModel sliderModel;

        [SetUp]
        public void SetUp()
        {
            sliderModel = new SliderModel();
        }

        [Test]
        public void GivenInitialState_WhenSettingMaxValue_ThenMaxValueIsUpdated()
        {
            // Given
            sliderModel.SetMinValue(0f);
            sliderModel.SetMaxValue(10f);
            Assert.AreEqual(10f, sliderModel.maxValue);

            // When
            sliderModel.SetMaxValue(20f);

            // Then
            Assert.AreEqual(20f, sliderModel.maxValue);
        }

        [Test]
        public void GivenInitialState_WhenSettingMaxValueBelowCurrentValue_ThenValueIsClampedToMaxValue()
        {
            // Given
            sliderModel.SetMinValue(0f);
            sliderModel.SetMaxValue(10f);
            sliderModel.UpdateValue(15f);
            Assert.AreEqual(10f, sliderModel.value);

            // When
            sliderModel.SetMaxValue(5f);

            // Then
            Assert.AreEqual(5f, sliderModel.maxValue);
            Assert.AreEqual(5f, sliderModel.value);
        }

        [Test]
        public void GivenInitialState_WhenSettingMaxValueAboveCurrentValue_ThenValueRemainsUnchanged()
        {
            // Given
            sliderModel.SetMinValue(0f);
            sliderModel.SetMaxValue(10f);
            sliderModel.UpdateValue(5f);
            Assert.AreEqual(5f, sliderModel.value);

            // When
            sliderModel.SetMaxValue(15f);

            // Then
            Assert.AreEqual(15f, sliderModel.maxValue);
            Assert.AreEqual(5f, sliderModel.value);
        }
    }

    public class SetMinValue
    {
        private SliderModel sliderModel;

        [SetUp]
        public void SetUp()
        {
            sliderModel = new SliderModel();
        }

        [Test]
        public void GivenInitialState_WhenSettingMinValue_ThenMinValueIsUpdated()
        {
            // Given
            sliderModel.SetMinValue(0f);
            sliderModel.SetMaxValue(10f);
            Assert.AreEqual(0f, sliderModel.minValue);

            // When
            sliderModel.SetMinValue(-5f);

            // Then
            Assert.AreEqual(-5f, sliderModel.minValue);
        }

        [Test]
        public void GivenInitialState_WhenSettingMinValueAboveCurrentValue_ThenValueIsClampedToMinValue()
        {
            // Given
            sliderModel.SetMinValue(0f);
            sliderModel.SetMaxValue(10f);
            sliderModel.UpdateValue(-5f);
            Assert.AreEqual(0f, sliderModel.value);

            // When
            sliderModel.SetMinValue(5f);

            // Then
            Assert.AreEqual(5f, sliderModel.minValue);
            Assert.AreEqual(5f, sliderModel.value);
        }

        [Test]
        public void GivenInitialState_WhenSettingMinValueBelowCurrentValue_ThenValueRemainsUnchanged()
        {
            // Given
            sliderModel.SetMinValue(0f);
            sliderModel.SetMaxValue(10f);
            sliderModel.UpdateValue(5f);
            Assert.AreEqual(5f, sliderModel.value);

            // When
            sliderModel.SetMinValue(-5f);

            // Then
            Assert.AreEqual(-5f, sliderModel.minValue);
            Assert.AreEqual(5f, sliderModel.value);
        }
    }

    public class SetIsInteger
    {
        private SliderModel sliderModel;

        [SetUp]
        public void SetUp()
        {
            sliderModel = new SliderModel();
        }

        [Test]
        public void GivenInitialState_WhenSettingIsIntegerToTrue_ThenIsIntegerIsUpdated()
        {
            // Given
            Assert.IsFalse(sliderModel.isInteger);

            // When
            sliderModel.SetIsInteger(true);

            // Then
            Assert.IsTrue(sliderModel.isInteger);
        }

        [Test]
        public void GivenInitialState_WhenSettingIsIntegerToFalse_ThenIsIntegerRemainsFalse()
        {
            // Given
            Assert.IsFalse(sliderModel.isInteger);

            // When
            sliderModel.SetIsInteger(false);

            // Then
            Assert.IsFalse(sliderModel.isInteger);
        }

        [Test]
        public void GivenIsIntegerIsTrue_WhenSettingIsIntegerToFalse_ThenIsIntegerIsUpdated()
        {
            // Given
            sliderModel.SetIsInteger(true);
            Assert.IsTrue(sliderModel.isInteger);

            // When
            sliderModel.SetIsInteger(false);

            // Then
            Assert.IsFalse(sliderModel.isInteger);
        }
    }
}
