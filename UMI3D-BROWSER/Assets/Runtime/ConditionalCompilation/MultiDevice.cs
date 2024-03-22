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
        const string variablePrefix = "UMI3D_";
        public static string GetSymbol(this MultiDevice device)
        {
            return $"{variablePrefix}{device}";
        }

        public static bool TryGetDevice(
            this string variable, 
            out MultiDevice device
        )
        {
            var tmp = variable.Substring(variablePrefix.Length);
            return Enum.TryParse(tmp, out device);
        }

        public static List<string> GetAllSymbols()
        {
            List<string> variables = new List<string>();
            foreach (MultiDevice device in Enum.GetValues(typeof(MultiDevice)))
            {
                variables.Add(GetSymbol(device));
            }

            return variables;
        }

        public static bool UpdateSymbols(
            this MultiDevice device,
            string[] symboles,
            List<string> allSymbols,
            ref List<string> newSymbols
        )
        {
            var deviceSymbol = device.GetSymbol();
            newSymbols.Clear();
            // -1: no device symbole found.
            // 0: no change needed, the symbol is already present.
            // 1: The symbol has been added.
            int result = -1;
            foreach (var symbole in symboles)
            {
                if (!allSymbols.Contains(symbole))
                {
                    newSymbols.Add(symbole);
                }
                else if (symbole == deviceSymbol)
                {
                    newSymbols.Add(symbole);
                    result = 0;
                }
            }
            if (result == -1)
            {
                newSymbols.Add(deviceSymbol);
                result = 1;
            }

            return result == 1;
        }
    }
}