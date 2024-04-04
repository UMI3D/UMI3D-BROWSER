using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using umi3d.common.lbe.description;
using System;

namespace umi3d.common.lbe.guardian
{
    public class UserARAnchorSerializers : UMI3DSerializerModule
    {
        public bool? IsCountable<T>()
        {
            Debug.Log("Remi : anchor Countable");
            return typeof(T) == typeof(ARAnchorDto) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {
            Debug.Log("Remi : anchor Read 1");

            if (typeof(T) == typeof(ARAnchorDto))
            {
                if (UMI3DSerializer.TryRead(container, out ulong trackableId)
                    && UMI3DSerializer.TryRead(container, out Vector3Dto position)
                    && UMI3DSerializer.TryRead(container, out Vector4Dto rotation))
                {
                    var arAnchorDto = new ARAnchorDto
                    {
                        trackableId = trackableId,
                        position = position,
                        rotation = rotation,
                    };

                    readable = true;
                    result = (T)Convert.ChangeType(arAnchorDto, typeof(T));
                    return true;
                }
            }

            result = default(T);
            readable = false;
            return false;
        }

        public bool Write<T>(T value, out Bytable bytable, params object[] parameters)
        {
            Debug.Log("Remi : anchor Write 1");

            if (value is ARAnchorDto c)
            {
                Debug.Log("Remi : anchor Write 2");

                // Ecriture de la liste anchorAR
                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.ARAnchorRequest)
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
