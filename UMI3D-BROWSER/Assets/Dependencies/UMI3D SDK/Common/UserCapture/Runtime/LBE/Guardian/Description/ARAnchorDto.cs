using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.common.lbe.description
{
    [Serializable]
    public class ARAnchorDto : UMI3DDto
    {
        public Vector3Dto position { get; set; }

        public Vector4Dto rotation { get; set; }

        public string trackableId { get; set; }
    }
}
