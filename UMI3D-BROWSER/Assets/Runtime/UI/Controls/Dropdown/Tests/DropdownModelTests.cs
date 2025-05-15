using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui.dropdown;
using UnityEngine;
using UnityEngine.TestTools;

public class DropdownModelTests
{
    public class SetLabelTests
    {
        DropdownModel dropdownModel;

        [SetUp]
        public void Setup()
        {
            dropdownModel = new DropdownModel();
        }

        [Test]
        public void GivenEmptyLabel_WhenSettingLabel_ThenLabelIsNotVisible()
        {
            // When
            dropdownModel.SetLabel("");

            // Then
            Assert.IsFalse(dropdownModel.isLabelVisible);
            Assert.AreEqual("", dropdownModel.label);
        }

        [Test]
        public void GivenNonEmptyLabel_WhenSettingLabel_ThenLabelIsVisible()
        {
            // When
            dropdownModel.SetLabel("Test Label");

            // Then
            Assert.IsTrue(dropdownModel.isLabelVisible);
            Assert.AreEqual("Test Label", dropdownModel.label);
        }

        [Test]
        public void GivenInitialState_WhenCreatingDropdownModel_ThenLabelIsNotVisible()
        {
            // Then
            Assert.IsFalse(dropdownModel.isLabelVisible);
            Assert.IsNull(dropdownModel.label);
            Assert.IsNull(dropdownModel.value);
            Assert.IsNotNull(dropdownModel.options);
            Assert.IsEmpty(dropdownModel.options);
        }
    }

    public class SetValueTests
    {
        [Test]
        public void GivenDropdownModel_WhenSetValue_ThenValueIsUpdated()
        {
            // Given
            var dropdownModel = new DropdownModel();
            var newValue = "New Value";

            // When
            dropdownModel.SetValue(newValue);

            // Then
            Assert.AreEqual(newValue, dropdownModel.value);
        }
    }

    public class UpdateValueTests
    {
        [Test]
        public void GivenDropdownModel_WhenUpdateValue_ThenValueIsUpdated()
        {
            // Given
            var dropdownModel = new DropdownModel();
            var newValue = "New Value";

            // When
            dropdownModel.UpdateValue(newValue);

            // Then
            Assert.AreEqual(newValue, dropdownModel.value);
        }
    }

    public class SetOptionsTests
    {
        [Test]
        public void GivenDropdownModel_WhenSetOptions_ThenOptionsIsUpdated()
        {
            // Given
            var dropdownModel = new DropdownModel();
            var newValue = new List<string>() { "New Value" , "New Value 1" };

            // When
            dropdownModel.SetOptions(newValue);

            // Then
            Assert.AreEqual(newValue, dropdownModel.options);
        }
    }
}
