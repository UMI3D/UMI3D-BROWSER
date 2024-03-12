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

using inetum.unityUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.UIElements;

namespace form_generator
{
    public class OldFormListElementsOptions
    {
        const int itemHeight = 16;

        private VisualElement root;
        private ListView listView;

        public event Action<OldFormParameter> OnFormElementSelected;

        public OldFormListElementsOptions(VisualElement root)
        {
            this.root = root;
            Init();
        }

        private void Init()
        {
            Enum.GetNames(typeof(OldFormParameter)).ForEach(name =>
            {
                Button button = new Button();
                button.text = name;
                button.clicked += () => OnFormElementSelected?.Invoke(Enum.Parse<OldFormParameter>(button.text));
                root.Add(button);
            });
        }
    }

    public enum OldFormParameter
    {
        None, 
        Boolean,
        Color, 
        DeviceId, 
        EnumString,
        Float, 
        FloatRange, 
        Integer, 
        IntegerRange,
        LocalInfo, 
        String, 
        UploadFile, 
        Vector2, 
        Vector3, 
        Vector4
    }
}

