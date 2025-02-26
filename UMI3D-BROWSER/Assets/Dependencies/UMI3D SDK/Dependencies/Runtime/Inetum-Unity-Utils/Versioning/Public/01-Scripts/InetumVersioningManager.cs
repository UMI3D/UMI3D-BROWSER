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

using inetum.unityUtils.conditionalCompilation;
using System;
using System.Diagnostics;

namespace inetum.unityUtils.versioning
{
    public class InetumVersioningManager : IVersioningDataDelegate
    {
        const string INETUM = "INETUM_";

        #region Initialization

        public static InetumVersioningManager @default => _default.Value;
        static readonly Lazy<InetumVersioningManager> _default = new(() => new InetumVersioningManager());

        InetumVersioningManager() { }

        #endregion

        #region IVersioningDataDelegate

        public IVersioningDataDelegate dataDelegate;

        public Version GetVersion()
        {
            return dataDelegate.GetVersion();
        }

        public ReleaseCycle GetReleaseCycle()
        {
            return dataDelegate.GetReleaseCycle();
        }

        public void UpdateVersioning()
        {
            ScriptingSymbolHelper.@default.UpdateSymbols(
                allCases, 
                dataDelegate.currentVersionForScriptingSymbol,
                dataDelegate.currentReleaseCycleForScriptingSymbol
            );
        }

        #endregion

        string[] allCases = { 
            INETUM + ALPHA, INETUM + BETA, INETUM + PROD,
            INETUM + Version1_0, INETUM + Version1_1,
        };

        #region Version

        /// <summary>
        /// Current version.
        /// </summary>
        const string Version1_0 = "1_0";
        /// <summary>
        /// Next version.
        /// </summary>
        const string Version1_1 = "1_1";

        [Conditional(INETUM + Version1_0)]
        public static void Version_1_0(Action action)
        {
            action?.Invoke();
        }

        [Conditional(INETUM + Version1_0), Conditional(INETUM + Version1_1)]
        public static void Version_1_0_OrAbove(Action action)
        {
            action?.Invoke();
        }

        [Conditional(INETUM + Version1_1)]
        public static void Version_1_1(Action action)
        {
            action?.Invoke();
        }

        #endregion

        #region Release cycle

        const string ALPHA = "ALPHA";
        const string BETA = "BETA";
        const string PROD = "PROD";

        [Conditional(INETUM + ALPHA)]
        public static void Alpha(Action action)
        {
            action?.Invoke();
        }

        [Conditional(INETUM + BETA)]
        public static void Beta(Action action)
        {
            action?.Invoke();
        }

        [Conditional(INETUM + PROD)]
        public static void Prod(Action action)
        {
            action?.Invoke();
        }

        #endregion
    }
}