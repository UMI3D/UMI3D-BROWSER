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
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class TMPInputFieldSelection : BaseInputFieldSelection
    {
        public override int startPosition
        {
            get
            {
                return Mathf.Min(
                    inputField.selectionAnchorPosition,
                    inputField.selectionFocusPosition
                );
            }
            set
            {
                if (inputField.selectionAnchorPosition <= inputField.selectionFocusPosition)
                {
                    inputField.selectionAnchorPosition = value;
                }
                else
                {
                    inputField.selectionFocusPosition = value;
                }
            }
        }

        public override int endPosition
        {
            get
            {
                return Mathf.Max(
                    inputField.selectionAnchorPosition,
                    inputField.selectionFocusPosition
                );
            }
            set
            {
                if (inputField.selectionAnchorPosition <= inputField.selectionFocusPosition)
                {
                    inputField.selectionFocusPosition = value;
                }
                else
                {
                    inputField.selectionAnchorPosition = value;
                }
            }
        }

        public override int stringPosition
        {
            get
            {
                return inputField.stringPosition;
            }
            set
            {
                inputField.stringPosition = value;
            }
        }

        public TMPInputFieldSelection(MonoBehaviour context) : base(context)
        {
            inputField = context.GetComponentInChildren<TMP_InputField>();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            inputField.interactable = true;
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Focus()
        {
            bool onFocusSelectAll = inputField.onFocusSelectAll;
            inputField.onFocusSelectAll = false;
            inputField.Select();
            new Task(async () =>
            {
                await Task.Yield();
                inputField.onFocusSelectAll = onFocusSelectAll;
            }).Start(TaskScheduler.FromCurrentSynchronizationContext());
        }

        public override void UpdateSelection()
        {
        }

        public override void UpdateCaret()
        {
        }
    }
}