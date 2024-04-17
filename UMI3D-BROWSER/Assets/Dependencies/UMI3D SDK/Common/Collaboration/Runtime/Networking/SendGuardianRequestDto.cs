using System.Collections;
using System.Collections.Generic;
using umi3d.common;
using umi3d.common.lbe;
using UnityEngine;

namespace umi3d.common.lbe
{
    public class SendGuardianRequestDto : AbstractOperationDto
    {
        public UserGuardianDto guardianData { get; set; }
    }
}
