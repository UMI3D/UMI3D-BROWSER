using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui.slider;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.TestTools;

public class SliderFloatParameterModelTests
{
    public class SetDto
    {
        private SliderFloatParameterModel sliderFloatParameterModel;
        private SliderModel mockSliderModel;
        private FloatRangeParameterDto mockDto;

        [SetUp]
        public void SetUp()
        {
            // Initialize mock objects
            mockSliderModel = new SliderModel();
            mockDto = new FloatRangeParameterDto {
                name = "Test Slider",
                min = 0,
                max = 10,
                value = 5.5f
            };

            // Initialize the class under test
            sliderFloatParameterModel = new SliderFloatParameterModel(mockSliderModel);
        }

        [Test]
        public void GivenValidDto_WhenSettingDto_ThenDtoIsSetCorrectly()
        {
            // Given
            // (Initial state is already set in SetUp)

            // When
            sliderFloatParameterModel.SetDto(mockDto);

            // Then
            Assert.AreEqual(mockDto, sliderFloatParameterModel.dto);
        }

        [Test]
        public void GivenValidDto_WhenSettingDto_ThenModelIsUpdatedCorrectly()
        {
            // Given
            // (Initial state is already set in SetUp)

            // When
            sliderFloatParameterModel.SetDto(mockDto);

            // Then
            Assert.AreEqual(mockDto.name, mockSliderModel.label);
            Assert.AreEqual(mockDto.min, mockSliderModel.minValue);
            Assert.AreEqual(mockDto.max, mockSliderModel.maxValue);
            Assert.AreEqual(mockDto.value, mockSliderModel.value);
            Assert.IsFalse(mockSliderModel.isInteger);
        }
    }
}
