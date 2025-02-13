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

using System.Collections.Generic;
using UnityEngine.UIElements;

namespace umi3d.browserEditor.BuildTool
{
    public class ViewContainer : ScrollView
    {
        Dictionary<View, VisualElement> views = new();

        public ViewContainer() 
        {
            style.marginLeft = 15;
            style.marginRight = 15;
        }

        public void AddView(View view, VisualElement visualElement)
        {
            views[view] = visualElement;
            visualElement.style.display = DisplayStyle.None;
            Add(visualElement);
        }

        public void Display(View view)
        {
            foreach (KeyValuePair<View, VisualElement> item in views)
            {
                item.Value.style.display = item.Key.Equals(view) 
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }
        }
    }
}