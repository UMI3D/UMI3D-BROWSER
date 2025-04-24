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

namespace umi3d.cdk.interaction
{
    public class Projection 
    {
        public Selector selector {  get; internal set; }
        public Controller controller { get; internal set; }
        public Tool tool { get; internal set; }
        public Interaction interaction { get; internal set; }
        public Input input { get; internal set; }

        internal Projection(Selector selector, Controller controller, Tool tool, Interaction interaction, Input input)
        {
            this.selector = selector;
            this.controller = controller;
            this.tool = tool;
            this.interaction = interaction;
            this.input = input;
        }

        public string debugDescription
        {
            get
            {
                string result = "---- Projection ----\n";
                result += $"{selector?.id ?? "No selector"}, {controller?.id ?? "No controller"}, {tool?.dto?.name ?? "No tool"}, {interaction?.dto?.name ?? "No interaction"}, {input?.control?.name ?? "No input"}\n";

                return result;
            }
        }
    }
}