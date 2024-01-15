﻿/*
Copyright 2019 - 2022 Inetum

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

using umi3d.cdk.menu;
using umi3d.cdk.menu.interaction;
using umi3dVRBrowsersBase.interactions.input;

namespace umi3dVRBrowsersBase.ui.displayers.watchMenu
{
    /// <summary>
    /// Tool displayer for <see cref="ui.watchMenu.WatchMenu"/>.
    /// </summary>
    public class ToolWatchDisplayer : AbstractWatchDisplayer
    {
        #region Methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override int IsSuitableFor(AbstractMenuItem menu)
        {
            return (menu is EventMenuItem) ? 2 : (menu is MenuItem) ? 1 : 0;
        }

        #endregion
    }
}

