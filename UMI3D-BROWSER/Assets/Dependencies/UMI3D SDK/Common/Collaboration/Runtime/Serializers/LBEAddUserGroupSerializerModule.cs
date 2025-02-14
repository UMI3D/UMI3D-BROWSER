using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.lbe.description;
using System;

namespace umi3d.common.lbe
{
    public class LBEAddUserGroupSerializerModule : UMI3DSerializerModule
    {
        public bool? IsCountable<T>()
        {
            return typeof(T) == typeof(LBEAddUserGroupOperationDto) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {
            if (typeof(T) == typeof(LBEAddUserGroupOperationDto))
            {
                ulong userId = UMI3DSerializer.Read<ulong>(container);
                bool isImmersive = UMI3DSerializer.Read<bool>(container);
                readable = UMI3DSerializer.TryRead(container, out uint Key);

                if (readable)
                {
                    LBEAddUserGroupOperationDto addUserLBEGroup = new LBEAddUserGroupOperationDto()
                    {
                        userId = userId,
                        isImmersive = isImmersive,
                    };
                    readable = true;
                    result = (T)Convert.ChangeType(addUserLBEGroup, typeof(T));
                    return true;
                }
            }
            result = default(T);
            readable = false;
            return false;
        }

        public bool Write<T>(T value, out Bytable bytable, params object[] parameters)
        {
            if (value is LBEAddUserGroupOperationDto dto)
            {
                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.MDMAddUserOperation)
                    + UMI3DSerializer.Write(dto.userId)
                    + UMI3DSerializer.Write(dto.isImmersive);
                return true;
            }

            bytable = null;
            return false;
        }
    }
}