using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.common.lbe
{
    public class SendLBEGroupRequestDTO : AbstractOperationDto
    {
        public LBEGroupDto lBEGroupData { get; set; }
    }
}
