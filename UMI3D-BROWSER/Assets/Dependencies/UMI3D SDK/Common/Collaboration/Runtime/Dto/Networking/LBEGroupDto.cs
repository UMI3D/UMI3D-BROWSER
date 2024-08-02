using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace umi3d.common.lbe
{
    public class LBEGroupDto : UMI3DDto
    {
        public uint GroupId { get; set; }

        public List<ulong> UserAR { get; set; }

        public List<ulong> UserVR { get; set; }

        public UserGuardianDto userGuardianDto;

        public LBEGroupDto()
        {
            UserAR = new List<ulong>();
            UserVR = new List<ulong>();
        }
    }
}
