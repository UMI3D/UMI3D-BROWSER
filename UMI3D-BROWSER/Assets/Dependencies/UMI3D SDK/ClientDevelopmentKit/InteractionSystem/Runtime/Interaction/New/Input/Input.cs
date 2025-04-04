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

using System;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public sealed class Input 
    {
        internal Input()
        {

        }

        public bool isAvailable { get; private set; }

        public Controller controller { get; private set; }
        internal bool Associate(Controller controller)
        {
            if (this.controller != null) { return false; }

            this.controller = controller;
            return true;
        }
        internal void DissociateFromController()
        {
            this.controller = null;
        }

        internal void Associate(Tool tool, Selector selector)
        {
            if (!isAvailable) { return; }

            throw new System.NotImplementedException();
        }
        internal void DissociateFromTool()
        {
            throw new System.NotImplementedException();
        }
    }
}