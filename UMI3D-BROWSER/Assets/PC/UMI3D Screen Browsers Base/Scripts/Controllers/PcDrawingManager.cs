/*
Copyright 2019 - 2023 Inetum

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
using System.Collections.Generic;
using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.cdk;
using umi3d.common.interaction;
using umi3dVRBrowsersBase.interactions.selection.cursor;
using umi3dVRBrowsersBase.ui;
using UnityEngine;

namespace umi3d.desktopBrowser.Controller
{
    public class PcDrawingManager : DrawingManager
    {
        public float distance = 1.5f;
        public float objectDistance = 3f;
        public float offset = 0.01f;

        public override Vector3? GetDrawingWorldPoint(DrawingInteractionDto drawing, List<UMI3DNodeInstance> nodes)
        {
            var screenPos = Input.mousePosition;
            if (nodes != null && nodes.Count > 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(screenPos);
                foreach (var hit in Physics.RaycastAll(ray, objectDistance))
                    if (nodes.Contains(hit.collider.GetComponentInParent<NodeContainer>().instance))
                        return hit.point + hit.normal * offset;
            }

            if (drawing.CanDrawInSpace)
            {
                screenPos.z = distance;
                return Camera.main.ScreenToWorldPoint(screenPos);
            }

            return null;
        }

        protected override void StartDrawingMode(DrawingInteractionDto drawing)
        {
            base.StartDrawingMode(drawing);
            BaseCursor.SetMovement(this,BaseCursor.CursorMovement.Drawing);
        }

        protected override void StopDrawingMode(DrawingInteractionDto drawing)
        {
            base.StopDrawingMode(drawing);
            BaseCursor.UnSetMovement(this);
        }
    }
}