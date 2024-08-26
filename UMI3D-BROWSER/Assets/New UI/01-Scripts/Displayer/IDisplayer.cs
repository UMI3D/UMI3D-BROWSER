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

using System.Collections.Generic;
using UnityEngine;

namespace umi3dBrowsers.displayer
{
    /// <summary>
    /// An interface to connect any displayer to the dynamic server container
    /// </summary>
    /// If you want to make a new displayer you need to implement this interface. 
    /// This is intended to be used with the UMI3D form system
    public interface IDisplayer
    {
        public object GetValue(bool trim);
        public void SetTitle(string title);
        public void SetPlaceHolder(List<string> placeHolder);
        public void SetColor(Color color);
        public void SetResource(object resource);
    }
}
