using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.userCapture.description;
using System;


namespace umi3d.common.lbe.description
{
    public class UserGuardianDto : AbstractBrowserRequestDto
    {
         public List<ARAnchorDto> anchorAR { get; set; }
       
    }
}

