using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.lbe.description;
using System;

namespace umi3d.common.lbe
{
    public class LBEGroupDataSerializers : UMI3DSerializerModule
    {
        public bool? IsCountable<T>()
        {
            return typeof(T) == typeof(LBEGroupSyncRequestDto) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {
            if (typeof(T) == typeof(LBEGroupSyncRequestDto))
            {
                readable = UMI3DSerializer.TryRead(container, out uint Key);

                uint lbeGroupId = UMI3DSerializer.Read<uint>(container);
                ulong adminUserId = UMI3DSerializer.Read<ulong>(container);

                if (readable && Key == UMI3DOperationKeys.SetLBEGroupRequest)
                {
                    var lBEGroup = new LBEGroupSyncRequestDto()
                    {
                        LBEGroupId = lbeGroupId,
                        AdminUserId = adminUserId
                    };
                    readable = true;
                    result = (T)Convert.ChangeType(lBEGroup, typeof(T));
                    return true;
                }
            }

            result = default(T);
            readable = false;
            return false;
        }

        public bool Write<T>(T value, out Bytable bytable, params object[] parameters)
        {
            if (value is LBEGroupSyncRequestDto dto)
            {
                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.SetLBEGroupRequest)
                    + UMI3DSerializer.Write(dto.LBEGroupId)
                    + UMI3DSerializer.Write(dto.AdminUserId);
                return true;
            }

            bytable = null;
            return false;
        }
    }
}
