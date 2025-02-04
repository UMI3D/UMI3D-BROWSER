using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui.slider;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.TestTools;

public class SliderIntParameterModelTests
{
    public class SetDto
    {
        private SliderIntParameterModel sliderIntParameterModel;
        private SliderModel mockSliderModel;
        private IntegerRangeParameterDto mockDto;

        [SetUp]
        public void SetUp()
        {
            // Initialize mock objects
            mockSliderModel = new SliderModel();
            mockDto = new IntegerRangeParameterDto {
                name = "Test Slider",
                min = 0,
                max = 100,
                value = 50
            };

            // Initialize the class under test
            sliderIntParameterModel = new SliderIntParameterModel(mockSliderModel);
        }

        [Test]
        public void GivenValidDto_WhenSettingDto_ThenDtoIsSetCorrectly()
        {
            // Given
            // (Initial state is already set in SetUp)

            // When
            sliderIntParameterModel.SetDto(mockDto);

            // Then
            Assert.AreEqual(mockDto, sliderIntParameterModel.dto);
        }

        [Test]
        public void GivenValidDto_WhenSettingDto_ThenModelIsUpdatedCorrectly()
        {
            // Given
            // (Initial state is already set in SetUp)

            // When
            sliderIntParameterModel.SetDto(mockDto);

            // Then
            Assert.AreEqual(mockDto.name, mockSliderModel.label);
            Assert.AreEqual(mockDto.min, mockSliderModel.minValue);
            Assert.AreEqual(mockDto.max, mockSliderModel.maxValue);
            Assert.AreEqual(mockDto.value, mockSliderModel.value);
            Assert.IsTrue(mockSliderModel.isInteger);
        }
    }

    public class ReleaseDtoTests
    {
        [Test]
        public void GivenDto_WhenRelease_ThenDtoNull()
        {
            var model = new SliderIntParameterModel(new SliderModel());
            model.SetDto(new IntegerRangeParameterDto());
            Assert.NotNull(model.dto);

            model.ReleaseDto();

            Assert.Null(model.dto);
        }
    }
}
