using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui.toggle;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.TestTools;

public class ToggleParameterModelTests
{
    public class SetDtoTests
    {
        ToggleParameterModel _model;

        [SetUp]
        public void SetUp()
        {
            _model = new ToggleParameterModel(new ToggleModel());
        }

        [Test]
        public void GivenStringParameterDto_WhenSetupDto_ThenLabelAndValueAndNbrLines()
        {
            string name = "Test Dto";
            bool value = true;
            BooleanParameterDto dto = new BooleanParameterDto() {
                name = name,
                value = value
            };

            _model.SetDto(dto);

            Assert.AreEqual(dto.name, name);
            Assert.AreEqual(dto.value, value);
        }
    }
}
