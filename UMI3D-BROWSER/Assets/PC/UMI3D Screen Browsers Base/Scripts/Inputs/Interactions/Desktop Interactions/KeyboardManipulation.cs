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
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.common.interaction;
using UnityEngine;
using inetum.unityUtils;

namespace umi3d.baseBrowser.inputs.interactions
{
    public class KeyboardManipulation : BaseKeyInteraction
    {
        public static List<KeyboardManipulation> S_Manipulations = new List<KeyboardManipulation>();

        /// <summary>
        /// Root of the avatar that will be ignore by raycast.
        /// </summary>
        public Transform AvatarParent;
        /// <summary>
        /// Head of the avatar.
        /// </summary>
        public Transform Head;
        public Transform Cursor;

        public float distCursor = 1;
        public float MaxDistCursorDelta = 0.3f;
        public float MinimumCursorDistance = 0.5f;
        public float ScrollToDistSpeed = 20f;

        protected bool constrainDistanceChange = false;
        protected Transform lastObject;
        protected List<Transform> ignore;

        protected override void Start()
        {
            base.Start();
            onInputDown.AddListener(() =>
            {
                lastObject = null;
                constrainDistanceChange = true;
            });
            onInputUp.AddListener(() =>
            {
                constrainDistanceChange = false;
            });

        }
    }


    public class DrawingManager : Singleton<DrawingManager>
    {
        public DrawingInteractionDto current;

        static public bool IsAvailableFor(DrawingInteractionDto drawing)
        {
            return Instance?.current == null || Instance?.current == drawing;
        }

        static public bool SwitchDrawing(DrawingInteractionDto drawing)
        {
            return Instance?.InternalSwitchDrawing(drawing) ?? false;
        }

        protected bool InternalSwitchDrawing(DrawingInteractionDto drawing)
        {
            if (this.current == null)
            {
                this.current = drawing;
                StartDrawingMode(drawing);
                return true;
            }

            if (this.current != drawing)
                throw new System.Exception("Drawing is already under going");

            StopDrawingMode(drawing);
            this.current = null;
            return false;
        }

        protected virtual void StartDrawingMode(DrawingInteractionDto drawing)
        {


        }
        protected virtual void StopDrawingMode(DrawingInteractionDto drawing)
        {

        }

        public virtual void StartDrawing(DrawingInteractionDto drawing)
        {
            if (drawing != current)
                return;


            UnityEngine.Debug.Log("Drawing !!!!!!!!!!!!");
        }

        public virtual void StopDrawing(DrawingInteractionDto drawing)
        {
            if (drawing != current)
                return;

            UnityEngine.Debug.Log("What a draw");
        }

    }
}
