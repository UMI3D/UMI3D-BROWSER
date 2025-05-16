///*
//Copyright 2019 - 2022 Inetum

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

//using inetum.unityUtils.observation;
//using System.Collections.Generic;
//using umi3d.baseBrowser.inputs.interactions;
//using umi3d.cdk;
//using umi3d.common.interaction;
//using UnityEngine;

//namespace umi3dVRBrowsersBase.interactions
//{
//    public partial class VRController  
//    {
//        protected virtual void Awake()
//        {
//            if (!VRDrawingManager.Exists)
//                new VRDrawingManager();

//            UnityEngine.Physics.queriesHitBackfaces = true;
//        }
//    }


//    public class VRDrawingManager : DrawingManager
//    {
//        List<(VRController,RayCursor)> vRControllers = new();

//        public void Declare(VRController controller)
//        {
//        }

//        public float distance = 1.5f;
//        public float objectDistance = 50f;
//        public float offset = 0.01f;
//        public float handOffset = 0f;

//        public override (Vector3,ulong)? GetDrawingWorldPoint(DrawingInteractionDto drawing, List<UMI3DNodeInstance> nodes/*, AbstractUMI3DInput input*/)
//        {
//            var cursor = vRControllers
//                .FirstOrDefault(c =>
//                        c.Item1.HoldInput == input
//                        || c.Item1.booleanInputs
//                                .Any(b => b == input)
//                ).Item2;

//            if (nodes != null && nodes.Count > 0)
//            {
//                var zone = new RaySelectionZone<NodeContainer>(cursor.transform.position, cursor.transform.up);
//                foreach (var nodeAndRay in zone.GetObjectsOnRayWithRayCastHits())
//                    if (nodeAndRay.Value.distance <= distance && nodes.Contains(nodeAndRay.Key.instance))
//                        return (nodeAndRay.Value.point + nodeAndRay.Value.normal * offset, nodeAndRay.Key.instance.Id);
//            }

//            if (drawing.canDrawInSpace)
//                return (cursor.transform.position + cursor.transform.up * handOffset, 0);

//            return null;
//        }
//    }
//}