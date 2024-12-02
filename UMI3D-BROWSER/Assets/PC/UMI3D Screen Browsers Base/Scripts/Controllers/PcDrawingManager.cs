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
using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.desktopBrowser.Controller
{
    public class PcDrawingManager : DrawingManager
    {
        public float distance = 1.5f;

        public override Vector3 GetDrawingWorldPoint(DrawingInteractionDto drawing)
        {
            var screenPos = Input.mousePosition;
            screenPos.z = distance;
            return Camera.main.ScreenToWorldPoint(screenPos);
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