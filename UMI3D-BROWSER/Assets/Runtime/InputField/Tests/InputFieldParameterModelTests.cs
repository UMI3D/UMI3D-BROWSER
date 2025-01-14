using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using umi3d.browserRuntime.ui.inputField;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.TestTools;

public class InputFieldParameterModelTests
{
    public class SetDtoTests
    {
        InputFieldParameterModel _model;

        [SetUp]
        public void SetUp()
        {
            _model = new InputFieldParameterModel(new InputFieldModel());
        }

        [Test]
        public void GivenStringParameterDto_WhenSetupDto_ThenLabelAndValueAndNbrLines()
        {
            string name = "Test Dto";
            string value = "Test Value";
            int nbLine = 1;
            StringParameterDto dto = new StringParameterDto() {
                name = name,
                value = value,
                NbLine = nbLine
            };

            _model.SetDto(dto);

            Assert.AreEqual(dto.name, name);
            Assert.AreEqual(dto.value, value);
            Assert.AreEqual(dto.NbLine, nbLine);
        }
    }
}
