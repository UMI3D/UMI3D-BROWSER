using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.common.lbe.description
{
    public class LBEGroupSyncRequestDTO  : AbstractOperationDto
    {
        public uint LBEGroupId { get; set; }

        public List<ulong> UserAR { get; set; }

        public List<ulong> UserVR { get; set; }

        public List<ARAnchorDto> ARAnchors { get; set; }

        public LBEGroupSyncRequestDTO ()
        {
            UserAR = new List<ulong>();
            UserVR = new List<ulong>();
            ARAnchors = new List<ARAnchorDto>();
        }
    }
}
