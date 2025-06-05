///*
//Copyright 2019 - 2023 Inetum

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//*/
//using System.Collections.Generic;

//namespace umi3d.baseBrowser.inputs.interactions
//{
//    [System.Serializable]
//    public class DrawModeDrawInteraction : BaseKeyInteraction
//    {
//        public static List<DrawModeDrawInteraction> S_Draws = new List<DrawModeDrawInteraction>();

//        public BaseDrawGroup drawGroup;

//        protected override void CreateMenuItem()
//        {
//            menuItem = new ButtonMenuItem
//            {
//                Name = associatedInteraction.name,
//                IsHoldable = associatedInteraction.hold
//            };
//        }

//        protected override void PressedDown()
//        {
//            onInputDown.Invoke();

//        }

//        protected override void PressedUp()
//        {
//            onInputUp.Invoke();

//        }
//    }
//}
