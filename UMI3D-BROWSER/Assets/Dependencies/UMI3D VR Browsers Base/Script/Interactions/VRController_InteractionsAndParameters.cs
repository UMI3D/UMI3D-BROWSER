/*
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
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using umi3dVRBrowsersBase.interactions.input;
using umi3dVRBrowsersBase.ui.playerMenu;
using UnityEngine;

namespace umi3dVRBrowsersBase.interactions
{
    public partial class VRController
    {
        ///// <summary>
        ///// Clear all menus and the projected tools.
        ///// </summary>
        //public override void Clear()
        //{
        //    ClearInputs(ref booleanInputs, input =>
        //    {
        //        if (!input.IsAvailable()) input.Dissociate();
        //    });
        //    ClearInputs(ref manipulationInputs, input =>
        //    {
        //        if (!input.IsAvailable()) input.Dissociate();
        //    });

        //}

        ///// <summary>
        ///// <inheritdoc/>
        ///// </summary>
        ///// <param name="link"></param>
        ///// <param name="unused"></param>
        ///// <returns></returns>
        //public override AbstractUMI3DInput FindInput(LinkDto link, bool unused = true)
        //{
        //    throw new System.NotImplementedException();
        //}

        ///// <summary>
        ///// <inheritdoc/>
        ///// </summary>
        ///// <param name="evt"></param>
        ///// <param name="unused"></param>
        ///// <param name="tryToFindInputForHoldableEvent"></param>
        ///// <returns></returns>
        //public override AbstractUMI3DInput FindInput(EventDto evt, bool unused = true, bool tryToFindInputForHoldableEvent = false)
        //{
        //    AbstractVRInput res = null;

        //    if (HoldInput != null && tryToFindInputForHoldableEvent && HoldInput.IsAvailable())
        //        res = HoldInput;

        //    if (res == null)
        //    {
        //        foreach (BooleanInput input in booleanInputs)
        //        {
        //            if (input.IsAvailable() || !unused)
        //            {
        //                res = input;
        //                break;
        //            }
        //        }
        //    }

        //    if (res == null) res = FindVRInput(menuInputs, i => i.IsAvailable() || !unused, this.gameObject);

        //    PlayerMenuManager.Instance.CtrlToolMenu.AddBinding(res);

        //    return res;
        //}



        ///// <summary>
        ///// Find the best free input for a given manipulation dof.
        ///// </summary>
        ///// <param name="manip">Manipulation to associate input to</param>
        ///// <param name="dof">Degree of freedom to associate input to</param>
        ///// <returns></returns>
        //public override AbstractUMI3DInput FindInput(ManipulationDto manip, DofGroupDto dof, bool unused = true)
        //{
        //    AbstractVRInput result = null;

        //    foreach (ManipulationInput input in manipulationInputs)
        //    {
        //        if (input.IsCompatibleWith(manip))
        //        {
        //            if (input.IsAvailable() || !unused)
        //            {
        //                result = input;
        //                break;
        //            }
        //        }
        //    }

        //    if (result == null)
        //    {
        //        //if no input was found
        //        result = this.gameObject.AddComponent<MenuInput>();
        //        result.Init(this);
        //        menuInputs.Add(result as MenuInput);
        //    }

        //    PlayerMenuManager.Instance.CtrlToolMenu.AddBinding(result);

        //    return result;
        //}

        //#endregion
    }
}
