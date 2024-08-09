using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.common.lbe
{
    public class AddUserLBEGroupDTO : AbstractOperationDto
    {
        public ulong UserId { get; set; }
        public bool IsUserAR { get; set; }
    }
}
