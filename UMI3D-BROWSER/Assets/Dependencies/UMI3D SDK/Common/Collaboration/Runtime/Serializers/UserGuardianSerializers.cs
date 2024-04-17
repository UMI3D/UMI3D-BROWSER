
using System;
using UnityEngine;
using System.Collections.Generic;
using umi3d.common.lbe.description;


namespace umi3d.common.lbe.guardian
{
    public class UserGuardianSerializers : UMI3DSerializerModule
    {

        public bool? IsCountable<T>()
        {
            Debug.Log("Remi : Countable");
            return typeof(T) == typeof(UserGuardianDto) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {
            Debug.Log("Remi : Read 1");

            if (typeof(T) == typeof(UserGuardianDto))
            {
                List<ARAnchorDto> anchorARList = UMI3DSerializer.ReadList<ARAnchorDto>(container);

                if (anchorARList != null && anchorARList.Count > 0)
                {
                    var userguardian = new UserGuardianDto
                    {
                        anchorAR = anchorARList
                    };

                    readable = true;
                    result = (T)Convert.ChangeType(userguardian, typeof(T));
                    return true;
                }
            }

            result = default(T);
            readable = false;
            return false;
        }

        public bool Write<T>(T value, out Bytable bytable, params object[] parameters)
        {
            Debug.Log("Remi : Write 1");

            if (value is UserGuardianDto c)
            {
                Debug.Log("Remi : Write 2");

                // Ecriture de la liste anchorAR
                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.GuardianBrowserRequest)
                    + UMI3DSerializer.WriteCollection(c.anchorAR);
                return true;
            }
            bytable = null;
            return false;
        }
    }
}

