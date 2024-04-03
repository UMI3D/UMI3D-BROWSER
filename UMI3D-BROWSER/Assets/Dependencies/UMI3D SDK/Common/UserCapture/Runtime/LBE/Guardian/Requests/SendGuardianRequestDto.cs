using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common;
using umi3d.common.lbe;


namespace umi3d.common.lbe.request
{
    public class SendGuardianRequestDto : AbstractBrowserRequestDto
    {
        /// <summary>
        /// Donn�es du guardian � envoyer.
        /// </summary>
        public UserGuardianDto guardianDto;

        public SendGuardianRequestDto(UserGuardianDto guardianDto)
        {
            this.guardianDto = guardianDto;
        }
    }
}

