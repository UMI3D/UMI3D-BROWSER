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
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.common.interaction;
using UnityEngine;
using inetum.unityUtils;
using umi3d.cdk.interaction;
using System.Linq;
using umi3d.common;
using static umi3d.common.volume.GeometryTools;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using GLTFast.Schema;

namespace umi3d.baseBrowser.inputs.interactions
{

    public interface IDrawerData
    {
        ulong environmentId { get; }
        GameObject gameObject { get; }
        AbstractUMI3DInput Input { get; }
        Transform BoneTransform { get; }
        uint BoneType { get; }
        ulong ToolId { get; }
        ulong HoveredObjectId { get; }
        ulong LastSurfaceId { get; set; }
        ulong? LineId { get; set; }

        ulong DrawingID { get; set; }

        List<UMI3DNodeInstance> Meshes { get; set; }

        List<Vector3> Positions { get; set; }

        float LastUpdateTime { get; set; }
        float TimeSynchronization { get; set; }
        float MinDistance { get; set; }
    }

    public class DrawingManager : Singleton<DrawingManager>
    {
        public DrawingInteractionDto current;
        public ulong currentEnvironmentId;

        private static ulong lastDrawingId = 1;

        static public bool IsAvailableFor(DrawingInteractionDto drawing)
        {
            return Instance?.current == null || Instance?.current == drawing;
        }

        static public bool SwitchDrawing(ulong environmentId, DrawingInteractionDto drawing, bool forceOff = false)
        {
            return Instance?.InternalSwitchDrawing(environmentId, drawing, forceOff) ?? false;
        }

        protected bool InternalSwitchDrawing(ulong environmentId, DrawingInteractionDto drawing, bool forceOff = false)
        {
            if (this.current == null)
            {
                if(forceOff)
                    return false;

                this.current = drawing;
                this.currentEnvironmentId = environmentId;
                StartDrawingMode(drawing);
                return true;
            }

            if (this.current != drawing)
                throw new System.Exception("Drawing is already under going");

            StopDrawingMode(drawing);
            this.current = null;
            return false;
        }


        static public ulong GetDrawingID()
        {
            return lastDrawingId++;
        }

        protected virtual void StartDrawingMode(DrawingInteractionDto drawing)
        {}

        protected virtual void StopDrawingMode(DrawingInteractionDto drawing)
        {}

        public virtual void StartDrawing(DrawingInteractionDto drawing, IDrawerData drawer)
        {
            drawer.DrawingID = GetDrawingID();
        }

        public virtual void StopDrawing(DrawingInteractionDto drawing, IDrawerData drawer)
        {
            drawer.DrawingID = 0;
        }

        public virtual (Vector3, ulong)? GetDrawingWorldPoint(DrawingInteractionDto drawing, List<UMI3DNodeInstance> nodes, AbstractUMI3DInput input)
        {
            return null;
        }

        public virtual async Task Init(IDrawerData drawer, DrawingInteractionDto drawing)
        {
            await CreateLine(drawer, drawing);

            drawer.Meshes.Clear();

            if (drawing.MeshIds != null)
                foreach (var meshId in drawing.MeshIds)
                {
                    var meshEntity = await UMI3DEnvironmentLoader.Instance.WaitUntilEntityLoaded(drawer.environmentId, meshId, new());
                    if (meshEntity is UMI3DNodeInstance node)
                        drawer.Meshes.Add(node);
                }
        }

        public virtual async Task CreateLine(IDrawerData drawer, DrawingInteractionDto drawing)
        {
            drawer.Positions.Clear();
            drawer.LineId = null;

            if (drawing.LineId != 0)
            {
                var lineEntity = await UMI3DEnvironmentLoader.Instance.WaitUntilEntityLoaded(drawer.environmentId, drawing.LineId, new());
                if ((lineEntity?.dto as GlTFNodeDto)?.extensions?.umi3d is UMI3DLineDto lineDto && lineEntity is UMI3DNodeInstance node)
                {
                    var template = UMI3DLineRendererLoader.GetOrCreateLine(node.GameObject, lineDto.clientLineId);
                    var c = UMI3DLineRendererLoader.CopyLine(drawer.gameObject, template);
                    drawer.LineId = c.Item2;
                    c.Item1.positionCount = 0;
                }
            }
        }

        public virtual async void DrawUpdate(DrawingInteractionDto drawing, ParticleSystem particleSystem, IDrawerData drawer)
        {
            (Vector3,ulong)? positionTMP = DrawingManager.Instance.GetDrawingWorldPoint(drawing, drawer.Meshes, drawer.Input);
            if (!positionTMP.HasValue)
            {
                //Cut line here ?
                if (particleSystem?.isPlaying ?? false)
                    particleSystem.Stop();
                return;
            }

            Vector3 position = positionTMP.Value.Item1;
            ulong surface = positionTMP.Value.Item2;

            if (drawer.Positions.Count > 0 && Vector3.Distance(position, drawer.Positions.Last()) < drawer.MinDistance)
            {
                if (particleSystem?.isPlaying ?? false)
                    particleSystem.Stop();
                return;
            }

            if (drawer.Positions.Count > 0)
                particleSystem?.transform.SetPositionAndRotation(position, Quaternion.LookRotation(drawer.Positions.Last() - position));
            else
                particleSystem?.transform.SetPositionAndRotation(position, Quaternion.LookRotation(Vector3.up));

            if (particleSystem?.isStopped ?? false)
                particleSystem.Play();

            particleSystem?.Emit(1);

            drawer.Positions.Add(position);

            if (drawer.LineId.HasValue)
            {
                var line = UMI3DLineRendererLoader.GetLine(drawer.LineId.Value);
                if (line != null)
                {
                    line.positionCount = drawer.Positions.Count;
                    //line.useWorldSpace = true;
                    if(line.useWorldSpace)
                        line.SetPositions(drawer.Positions.ToArray());
                    else
                        line.SetPositions(drawer.Positions.Select(p => line.transform.InverseTransformPoint(p)).ToArray());
                }
            }

            if(drawer.LastSurfaceId == 0 && drawer.Positions.Count == 0)
                drawer.LastSurfaceId = surface;
            
            if (Time.time >= drawer.TimeSynchronization + drawer.LastUpdateTime || drawer.Positions.Count > 100)
            {
                //todo add delay
                var drawingDto = new common.interaction.DrawingDto
                {
                    drawingEnd = false,
                    clientDrawingId = drawer.DrawingID,
                    clientLineId = drawer.LineId.HasValue ? drawer.LineId.Value : 0,
                    positions = drawer.Positions.Select(p => p.Dto()).ToList(),

                    surfaceId = drawer.LastSurfaceId,

                    boneType = drawer.BoneType,
                    id = drawing.id,
                    toolId = drawer.ToolId,
                    hoveredObjectId = drawer.HoveredObjectId,
                    bonePosition = (Vector3Dto)drawer.BoneTransform.position.Dto(),
                    boneRotation = (Vector4Dto)drawer.BoneTransform.rotation.Dto()
                };
                cdk.UMI3DClientServer.SendRequest(drawingDto, true);
                drawer.LastUpdateTime = Time.time;

                if(drawer.Positions.Count > 100 || drawer.LastSurfaceId != surface)
                {
                    await CreateLine(drawer, drawing);
                    drawer.Positions.Add(position);
                    drawer.LastSurfaceId = surface;
                }

            }
        }

    }
}
