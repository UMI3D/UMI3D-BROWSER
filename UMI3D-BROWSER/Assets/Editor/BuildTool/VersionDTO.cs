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

namespace umi3d.browserEditor.BuildTool
{
    [Serializable]
    public struct VersionDTO 
    {
        /// <summary>
        /// Additional version.
        /// </summary>
        public string additionalVersion;
        /// <summary>
        /// Major version.
        /// </summary>
        public int majorVersion;
        /// <summary>
        /// Minor version.
        /// </summary>
        public int minorVersion;
        /// <summary>
        /// Build count version.
        /// </summary>
        public int buildCountVersion;
        /// <summary>
        /// Date of the version.
        /// </summary>
        public string date;

        public string VersionFromNow
        {
            get
            {
                return GetFormattedVersion(DateTime.Now.ToString("yyMMdd"));
            }
        }

        public string Version
        {
            get
            {
                return GetFormattedVersion(date);
            }
        }

        string GetFormattedVersion(string date)
        {
            string result = $"";

            if (!string.IsNullOrEmpty(additionalVersion))
            {
                result += $"{additionalVersion}_";
            }

            result += $"{majorVersion}.{minorVersion}.{buildCountVersion}_{date}";

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