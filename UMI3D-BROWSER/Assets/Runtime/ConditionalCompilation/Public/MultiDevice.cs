/*
Copyright 2019 - 2024 Inetum

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
using System.Collections.Generic;
using System.Linq;

namespace umi3d.browserRuntime.conditionalCompilation
{
    public enum MultiDevice 
    {
        /// <summary>
        /// For Personal Computer:
        /// <list type="bullet">
        /// <item>Computer (Windows or Mac)</item>
        /// <item>Tablet (Android or iPad)</item>
        /// <item>Smartphone (Android or iPhone)</item>
        /// <item>Video game console</item>
        /// </list>
        /// </summary>
        PC,
        /// <summary>
        /// For Virtual reality, Augmented reality and Mixed reality
        /// </summary>
        XR
    }

    public static class MultiDeviceExtensions
    {
        /// <summary>
        /// Symbols are prefixed by this variable.
        /// </summary>
        const string variablePrefix = "UMI3D";

        /// <summary>
        /// Get the full symbol corresponding to <paramref name="device"/>.<br/>
        /// <br/>
        /// <example>
        /// Given a device when getting symbol then return the symbol.
        /// <code>
        /// MultiDevice.PC.GetSymbol(); // return UMI3D_PC.
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static string GetSymbol(this MultiDevice device)
        {
            return $"{variablePrefix}{GetSymbolEndPoint(device)}";
        }

        /// <summary>
        /// Get the end point corresponding to <paramref name="device"/>.<br/>
        /// <br/>
        /// <example>
        /// Given a device when getting symbol end-point then return symbol end-point.
        /// <code>
        /// MultiDevice.PC.GetSymbolEndPoint(); // return _PC.
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static string GetSymbolEndPoint(this MultiDevice device)
        {
            return $"_{device}";
        }

        /// <summary>
        /// Try to get a device from a symbol.<br/>
        /// If <paramref name="symbol"/> correspond to a device then return true and device. Else return false.<br/>
        /// <br/>
        /// <example>
        /// Given a symbol that correspond to a device when trying to get device then return true and device.
        /// <code>
        /// "UMI3D_PC".TryGetDeviceFromSymbol(out MultiDevice device); // return true and device = MultiDevice.PC.
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static bool TryGetDeviceFromSymbol(
            this string symbol, 
            out MultiDevice device
        )
        {
            if (string.IsNullOrEmpty(symbol))
            {
                device = default;
                return false;
            }
            if (symbol.Length <= variablePrefix.Length + 1)
            {
                device = default;
                return false;
            }
            string tmp = symbol.Substring(variablePrefix.Length + 1);
            return Enum.TryParse(tmp, out device);
        }

        /// <summary>
        /// Get all the symbols.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAllSymbols()
        {
            return Enum.GetValues(typeof(MultiDevice))
                .OfType<MultiDevice>()
                .Select(device => device.GetSymbol());
        }
    }
}