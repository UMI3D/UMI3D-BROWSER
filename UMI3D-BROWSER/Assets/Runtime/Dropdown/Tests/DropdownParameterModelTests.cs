using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui.dropdown;
using umi3d.common.interaction;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.TestTools;

public class DropdownParameterModelTests
{
    public class SetDtoTests
    {
        private DropdownParameterModel dropdownParameterModel;
        private EnumParameterDto<string> testDto;
        private DropdownModel testModel;

        [SetUp]
        public void SetUp()
        {
            // Initialize the test objects
            testModel = new DropdownModel();
            dropdownParameterModel = new DropdownParameterModel(testModel);
            testDto = new EnumParameterDto<string> {
                name = "TestName",
                value = "TestValue",
                possibleValues = new List<string> { "Option1", "Option2", "Option3" }
            };
        }

        [Test]
        public void GivenValidDto_WhenSettingDto_ThenModelIsUpdated()
        {
            // Given
            var initialDto = new EnumParameterDto<string> {
                name = "InitialName",
                value = "InitialValue",
                possibleValues = new List<string> { "InitialOption1", "InitialOption2" }
            };
            dropdownParameterModel.SetDto(initialDto);

            // When
            dropdownParameterModel.SetDto(testDto);

            // Then
            Assert.AreEqual(testDto.name, dropdownParameterModel.model.label);
            Assert.AreEqual(testDto.value, dropdownParameterModel.model.value);
            CollectionAssert.AreEqual(testDto.possibleValues, dropdownParameterModel.model.options);
        }
    }

    /// <summary>
    /// This method releases the DTO by setting it to null.<br/>
    /// <br/>
    /// <example>
    /// Given a model with a non-null DTO, when calling ReleaseDto, then the DTO should be null.
    /// <code>
    /// model.ReleaseDto();
    /// </code>
    /// </example>
    /// </summary>
    public class ReleaseDtoTests
    {
        [Test]
        public void GivenDto_WhenRelease_ThenDtoNull()
        {
            var model = new DropdownParameterModel(new DropdownModel());
            model.SetDto(new EnumParameterDto<string>());
            Assert.NotNull(model.dto);

            model.ReleaseDto();

            Assert.Null(model.dto);
        }
    }
}
