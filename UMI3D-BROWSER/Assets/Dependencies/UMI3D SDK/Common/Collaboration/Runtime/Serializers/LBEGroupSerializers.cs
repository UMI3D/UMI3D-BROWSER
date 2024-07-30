using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.lbe.description;
using System;

namespace umi3d.common.lbe
{
    public class LBEGroupSerializers : UMI3DSerializerModule
    {

        public bool? IsCountable<T>()
        {
            return typeof(T) == typeof(LBEGroupDto) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {
            if (typeof(T) == typeof(LBEGroupDto))
            {
                uint groupId = UMI3DSerializer.Read<uint>(container);
                List<ulong> members = UMI3DSerializer.ReadList<ulong>(container);
                readable = UMI3DSerializer.TryRead(container, out uint Key);
                UserGuardianDto userGuardianDto = UMI3DSerializer.Read<UserGuardianDto>(container);

                if (readable)
                {
                    var lBEGroup = new LBEGroupDto()
                    {
                        GroupId = groupId,
                        Members = members,
                        userGuardianDto = userGuardianDto
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
            if (value is LBEGroupDto c)
            {
                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.SetLBEGroupRequest)
                    + UMI3DSerializer.Write(c.GroupId)
                    + UMI3DSerializer.WriteCollection(c.Members)
                    + UMI3DSerializer.Write(c.userGuardianDto);
                return true;
            }

            bytable = null;
            return false;
        }
    }

    public class LBEGroupSerializersID : UMI3DSerializerModule
    {
        public bool? IsCountable<T>()
        {
            return typeof(T) == typeof(SendLBEGroupRequestDTO) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {
            if (typeof(T) == typeof(SendLBEGroupRequestDTO))
            {
                readable = UMI3DSerializer.TryRead(container, out uint Key);
                readable &= UMI3DSerializer.TryRead(container, out LBEGroupDto lBEGroupDto);

                if (readable && Key == UMI3DOperationKeys.SetLBEGroupRequest)
                {
                    var sendLBEGroupRequest = new SendLBEGroupRequestDTO
                    {
                        lBEGroupData = lBEGroupDto
                    };

                    readable = true;
                    result = (T)Convert.ChangeType(sendLBEGroupRequest, typeof(T));
                    return true;
                }
            }

            result = default(T);
            readable = false;
            return false;
        }

        public bool Write<T>(T value, out Bytable bytable, params object[] parameters)
        {
            if (value is SendLBEGroupRequestDTO s)
            {
                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.SetLBEGroupRequest)
                    + UMI3DSerializer.Write(s.lBEGroupData);
                return true;
            }

            bytable = null;
            return false;
        }
    }
}
