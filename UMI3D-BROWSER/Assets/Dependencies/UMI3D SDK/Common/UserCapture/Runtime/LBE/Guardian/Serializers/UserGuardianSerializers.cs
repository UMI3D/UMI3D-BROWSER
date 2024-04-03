
using System;
using UnityEngine;
using System.Collections.Generic;
using umi3d.common.lbe;


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

                if(UMI3DSerializer.TryRead(container, out ulong trackableId)
                    && UMI3DSerializer.TryRead(container, out Vector3Dto position)
                    && UMI3DSerializer.TryRead(container, out Vector4Dto rotation)
                    )
                {
                    Debug.Log("Remi : Read 2");

                    var userguardian = new UserGuardianDto
                    {
                        trackableId = trackableId,
                        position = position,
                        rotation = rotation,
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

                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.GuardianRequest)
                    + UMI3DSerializer.Write(c.trackableId)
                    + UMI3DSerializer.Write(c.position)
                    + UMI3DSerializer.Write(c.rotation);
                return true;
            }
            bytable = null; 
            return false; 
        }
    }
}

