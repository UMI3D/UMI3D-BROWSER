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
using System.Collections;
using System.Collections.Generic;
using umi3d.common.interaction;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

namespace form_generator
{
    /// <summary>
    /// This is a service for the FormGeneratorTool responsible of making up some connection forms dto
    /// </summary>
    public class OldConnectionFormMaker : MonoBehaviour
    {
        private VisualElement root;
        private ConnectionFormDto connectionForm;
        public ConnectionFormDto ConnectionForm => connectionForm;

        public OldConnectionFormMaker(VisualElement root)
        {
            this.root = root;
            connectionForm = new ConnectionFormDto();
        }

        public void AddAnFormParam(OldFormParameter formParam)
        {
            Label label = new Label();
            label.text = Enum.GetName(typeof(OldFormParameter), formParam);
            root.Add(label);

            HandleFormParam(formParam);
        }

        private void HandleFormParam(OldFormParameter formParam)
        {
            switch (formParam)
            {
                case OldFormParameter.None:
                    break;
                case OldFormParameter.Boolean:
                    MakeBoolParam();
                    break;
                case OldFormParameter.Color:
                    break;
                case OldFormParameter.DeviceId:
                    break;
                case OldFormParameter.EnumString:
                    MakeEnumString();
                    break;
                case OldFormParameter.Float:
                    break;
                case OldFormParameter.FloatRange:
                    break;
                case OldFormParameter.Integer:
                    break;
                case OldFormParameter.IntegerRange:
                    break;
                case OldFormParameter.LocalInfo:
                    break;
                case OldFormParameter.String:
                    MakeStringParam();
                    break;
                case OldFormParameter.UploadFile:
                    break;
                case OldFormParameter.Vector2:
                    break;
                case OldFormParameter.Vector3:
                    break;
                case OldFormParameter.Vector4:
                    break;
            }
        }

        private void MakeBoolParam()
        {
            BooleanParameterDto boolParam = new BooleanParameterDto();
            boolParam.name = "Test Bool";
            boolParam.value = false;
            connectionForm.fields.Add(boolParam);
        }

        private void MakeEnumString()
        {
            EnumParameterDto<string> enumParam = new EnumParameterDto<string>();
            enumParam.name = "Test Enum";
            enumParam.possibleValues = new List<string>()
            {
                "0","1","2","3"
            };
            enumParam.value = "0";
            connectionForm.fields.Add(enumParam);
        }

        private void MakeStringParam()
        {
            StringParameterDto stringParam = new StringParameterDto();
            stringParam.value = "TestValue";
            stringParam.description = "A test Value to test";
            stringParam.name = "Test Text";
            connectionForm.fields.Add(stringParam);
        }

        internal void Reset()
        {
            root.Clear();
            connectionForm.fields.Clear();
        }
    }
}

