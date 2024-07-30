using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.common.lbe
{
    public class LBEGroupDto : MonoBehaviour
    {
        public uint GroupId { get; set; }

        public List<ulong> Members { get; set; } = new List<ulong>();

        public UserGuardianDto userGuardianDto;

        public LBEGroupDto()
        {
            Members = new List<ulong>();
        }
    }
}
