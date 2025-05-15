using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
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
                NbLine = nbLine,
                privateParameter = true
            };

            _model.SetDto(dto);

            Assert.AreEqual(_model.model.label, name);
            Assert.AreEqual(_model.model.value, value);
            Assert.AreEqual(_model.model.nbrLine, nbLine);
            Assert.AreEqual(TMP_InputField.ContentType.Password, _model.model.ContentType);
        }
    }

    public class ReleaseDtoTests
    {
        [Test]
        public void GivenDto_WhenRelease_ThenDtoNull()
        {
            var model = new InputFieldParameterModel(new InputFieldModel());
            model.SetDto(new StringParameterDto());
            Assert.NotNull(model.dto);

            model.ReleaseDto();

            Assert.Null(model.dto);
        }
    }
}
