using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.userCapture;
using System;


namespace umi3d.common.lbe
{
    public class UserGuardianDto : AbstractBrowserRequestDto
    {
        /// <summary>
        /// Position anchor.
        /// </summary>
        public Vector3Dto position { get; set; }

        /// <summary>
        /// Rotation anchor.
        /// </summary>
        public Vector4Dto rotation { get; set; }

        /// <summary>
        /// Id anchor.
        /// </summary>
        public ulong trackableId { get; set; }
    }
}

