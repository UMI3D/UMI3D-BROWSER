using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.userCapture;
using System;
using umi3d.common.lbe.description;


namespace umi3d.common.lbe
{
    public class UserGuardianDto : AbstractBrowserRequestDto
    {
        public List<ARAnchorDto> ARAnchors { get; set; }

        public uint ARiD { get; set; }
    }
}
