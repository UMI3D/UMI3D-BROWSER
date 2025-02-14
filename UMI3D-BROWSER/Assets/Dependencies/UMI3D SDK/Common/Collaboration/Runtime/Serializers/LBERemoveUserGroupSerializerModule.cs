using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.lbe.description;
using System;

namespace umi3d.common.lbe
{
    public class LBERemoveUserGroupSerializerModule : UMI3DSerializerModule
    {
        public bool? IsCountable<T>()
        {
            return typeof(T) == typeof(LBERemoveUserGroupOperationDto) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {
            if (typeof(T) == typeof(LBERemoveUserGroupOperationDto))
            {
                ulong userId = UMI3DSerializer.Read<ulong>(container);
                readable = UMI3DSerializer.TryRead(container, out uint Key);

                if (readable)
                {
                    var dellUserLBEGroup = new LBERemoveUserGroupOperationDto()
                    {
                        userId = userId,
                    };
                    readable = true;
                    result = (T)Convert.ChangeType(dellUserLBEGroup, typeof(T));

                    return true;
                }
            }
            result = default(T);
            readable = false;
            return false;
        }

        public bool Write<T>(T value, out Bytable bytable, params object[] parameters)
        {
            if (value is LBERemoveUserGroupOperationDto dto)
            {
                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.MDMRemoveUserOperation)
                    + UMI3DSerializer.Write(dto.userId);
                return true;
            }

            bytable = null;
            return false;
        }
    }
}
