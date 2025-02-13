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
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    public struct Version 
    {
        /// <summary>
        /// Additional version.
        /// </summary>
        public readonly string additionalVersion;
        /// <summary>
        /// Major version.
        /// </summary>
        public readonly int majorVersion;
        /// <summary>
        /// Minor version.
        /// </summary>
        public readonly int minorVersion;
        /// <summary>
        /// Build count version.
        /// </summary>
        public readonly int buildCountVersion;
        /// <summary>
        /// Date of the version.
        /// </summary>
        public readonly string date;

        public override string ToString()
        {
            return GetFormattedVersion(date);
        }

        public string ToStringFromNow()
        {
            return GetFormattedVersion(DateTime.Now.ToString("yyMMdd"));
        }

        public string GetFormattedVersion(string date, string separator = ".")
        {
            string result = $"";

            if (!string.IsNullOrEmpty(additionalVersion))
            {
                result += $"{additionalVersion}_";
            }

            result += $"{majorVersion}{separator}{minorVersion}{separator}{buildCountVersion}{separator}{date}";

            return result;
        }

        /// <summary>
        /// Bundle version for Android.
        /// </summary>
        public int BundleVersion
        {
            get
            {
                return majorVersion * 10_000 + minorVersion * 100 + buildCountVersion;
            }
        }
    }
}