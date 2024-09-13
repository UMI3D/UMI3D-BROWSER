using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.common.lbe.description
{
    public class SendLBEGroupRequestDTO : AbstractOperationDto
    {
        public uint GroupId { get; set; }

        public List<ulong> UserAR { get; set; }

        public List<ulong> UserVR { get; set; }

        public List<ARAnchorDto> anchorAR { get; set; }

        public SendLBEGroupRequestDTO()
        {
            UserAR = new List<ulong>();
            UserVR = new List<ulong>();
            anchorAR = new List<ARAnchorDto>();
        }
    }
}
