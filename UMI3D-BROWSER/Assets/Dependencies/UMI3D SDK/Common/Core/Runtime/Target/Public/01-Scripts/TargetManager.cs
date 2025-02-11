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
using System.Collections.Generic;
using System.Linq;

namespace umi3d.common.core.target
{
    public class TargetManager : ITargetDataDelegate
    {
        public static TargetManager @default => _default.Value;
        static readonly Lazy<TargetManager> _default = new(() => new TargetManager());
        TargetManager() {}

        public ITargetDataDelegate dataDelegate;

        public OperatingSystem GetCurrentOperatingSystem()
        {
            return dataDelegate.GetCurrentOperatingSystem();
        }

        public IReadOnlyList<Controller> GetAuthorizedControllers()
        {
            return dataDelegate.GetAuthorizedControllers();
        }

        public IReadOnlyList<Plugin> GetActivePlugins()
        {
            return dataDelegate.GetActivePlugins();
        }

        public IReadOnlyList<Feature> GetActiveFeatures()
        {
            return dataDelegate.GetActiveFeatures();
        }

        public static bool isWindows => @default.GetCurrentOperatingSystem().Equals(OperatingSystem.windows);
        public static bool isAndroid => @default.GetCurrentOperatingSystem().Equals(OperatingSystem.android);

        public static bool isPC => @default.GetAuthorizedControllers().Contains(Controller.keyboardAndMouse);
        public static bool isVR
        {
            get
            {
                IReadOnlyList<Controller> controllers = @default.GetAuthorizedControllers();
                if (controllers.Contains(Controller.vrController)) { return true; }
                else if (controllers.Contains(Controller.hand)) { return true; }
                else { return false; }
            }
        }
        public static bool isMobile => @default.GetAuthorizedControllers().Contains(Controller.screen);
    }
}