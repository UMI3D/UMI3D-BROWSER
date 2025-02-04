/*
Copyright 2019 - 2025 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using umi3d.browserRuntime.conditionalCompilation;
using UnityEngine;
using UnityEngine.TestTools;

public class MultiDeviceTests
{
    public class GetSymbolEndPointTest
    {
        [Test]
        public void GivenPCDevice_WhenGettingSymbolEndPoint_ThenReturnPCSymbolEndPoint()
        {
            // Given
            MultiDevice device = MultiDevice.PC;

            // When
            string result = device.GetSymbolEndPoint();

            // Then
            Assert.AreEqual("_PC", result);
        }

        [Test]
        public void GivenXRDevice_WhenGettingSymbolEndPoint_ThenReturnXRSymbolEndPoint()
        {
            // Given
            MultiDevice device = MultiDevice.XR;

            // When
            string result = device.GetSymbolEndPoint();

            // Then
            Assert.AreEqual("_XR", result);
        }
    }

    public class GetSymbolTest
    {
        [Test]
        public void GivenPCDevice_WhenGettingSymbol_ThenReturnPCSymbol()
        {
            // Given
            MultiDevice device = MultiDevice.PC;

            // When
            string result = device.GetSymbol();

            // Then
            Assert.AreEqual("UMI3D_PC", result);
        }

        [Test]
        public void GivenXRDevice_WhenGettingSymbol_ThenReturnXRSymbol()
        {
            // Given
            MultiDevice device = MultiDevice.XR;

            // When
            string result = device.GetSymbol();

            // Then
            Assert.AreEqual("UMI3D_XR", result);
        }
    }

    public class TryGetDeviceFromSymbolTest
    {
        [Test]
        public void GivenPCSymbol_WhenTryingToGetDevice_ThenReturnTrueAndPCDevice()
        {
            // Given
            string symbol = "UMI3D_PC";

            // When
            bool result = symbol.TryGetDeviceFromSymbol(out MultiDevice device);

            // Then
            Assert.IsTrue(result);
            Assert.AreEqual(MultiDevice.PC, device);
        }

        [Test]
        public void GivenXRSymbol_WhenTryingToGetDevice_ThenReturnTrueAndXRDevice()
        {
            // Given
            string symbol = "UMI3D_XR";

            // When
            bool result = symbol.TryGetDeviceFromSymbol(out MultiDevice device);

            // Then
            Assert.IsTrue(result);
            Assert.AreEqual(MultiDevice.XR, device);
        }

        [Test]
        public void GivenInvalidSymbol_WhenTryingToGetDevice_ThenReturnFalseAndDefaultDevice()
        {
            // Given
            string symbol = "UMI3D_INVALID";

            // When
            bool result = symbol.TryGetDeviceFromSymbol(out MultiDevice device);

            // Then
            Assert.IsFalse(result);
            Assert.AreEqual(default(MultiDevice), device);

            // ---- New test ----

            // Given
            symbol = "inv";

            // When
            result = symbol.TryGetDeviceFromSymbol(out device);

            // Then
            Assert.IsFalse(result);
            Assert.AreEqual(default(MultiDevice), device);
        }

        [Test]
        public void GivenEmptySymbol_WhenTryingToGetDevice_ThenReturnFalseAndDefaultDevice()
        {
            // Given
            string symbol = "";

            // When
            bool result = symbol.TryGetDeviceFromSymbol(out MultiDevice device);

            // Then
            Assert.IsFalse(result);
            Assert.AreEqual(default(MultiDevice), device);
        }

        [Test]
        public void GivenNullSymbol_WhenTryingToGetDevice_ThenReturnFalseAndDefaultDevice()
        {
            // Given
            string symbol = null;

            // When
            bool result = symbol.TryGetDeviceFromSymbol(out MultiDevice device);

            // Then
            Assert.IsFalse(result);
            Assert.AreEqual(default(MultiDevice), device);
        }
    }

    public class GetAllSymbolsTest
    {
        [Test]
        public void WhenGettingAllSymbols_ThenReturnAllDeviceSymbols()
        {
            // Given
            var expectedSymbols = new List<string> { "UMI3D_PC", "UMI3D_XR" };

            // When
            var result = MultiDeviceExtensions.GetAllSymbols().ToList();

            // Then
            CollectionAssert.AreEquivalent(expectedSymbols, result);
        }

        [Test]
        public void WhenGettingAllSymbols_ThenReturnNonEmptyList()
        {
            // When
            var result = MultiDeviceExtensions.GetAllSymbols().ToList();

            // Then
            Assert.IsNotEmpty(result);
        }

        [Test]
        public void WhenGettingAllSymbols_ThenReturnCorrectCount()
        {
            // Given
            int expectedCount = Enum.GetValues(typeof(MultiDevice)).Length;

            // When
            var result = MultiDeviceExtensions.GetAllSymbols().ToList();

            // Then
            Assert.AreEqual(expectedCount, result.Count);
        }
    }
}