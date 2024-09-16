
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
            return typeof(T) == typeof(UserGuardianDto) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {
            if (typeof(T) == typeof(UserGuardianDto))
            {
                List<ARAnchorDto> ARAnchors = UMI3DSerializer.ReadList<ARAnchorDto>(container);
                float offsetGuardian = UMI3DSerializer.Read<float>(container);
                uint ARid = UMI3DSerializer.Read<uint>(container);

                if (ARAnchors != null)
                {
                    var userguardian = new UserGuardianDto
                    {
                        ARAnchors = ARAnchors,
                        ARiD = ARid
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
            if (value is UserGuardianDto c)
            {
                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.GuardianBrowserRequest)
                    + UMI3DSerializer.WriteCollection(c.ARAnchors)
                    + UMI3DSerializer.Write(c.ARiD);
                return true;
            }

            bytable = null;
            return false;
        }
    }
}

