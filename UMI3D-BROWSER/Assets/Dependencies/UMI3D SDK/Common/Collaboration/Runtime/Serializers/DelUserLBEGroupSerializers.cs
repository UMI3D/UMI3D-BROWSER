using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.lbe.description;
using System;

namespace umi3d.common.lbe
{
    public class DelUserLBEGroupSerializers : UMI3DSerializerModule
    {
        public bool? IsCountable<T>()
        {
            return typeof(T) == typeof(DelUserLBEGroupDto) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {
            if (typeof(T) == typeof(DelUserLBEGroupDto))
            {
                Debug.Log("REMY : Reading DelUserGroupOperationsDto 0");

                ulong userId = UMI3DSerializer.Read<ulong>(container);
                readable = UMI3DSerializer.TryRead(container, out uint Key);
                Debug.Log("REMY : Reading DelUserGroupOperationsDto 1");


                if (readable)
                {
                    Debug.Log("REMY : Reading DelUserGroupOperationsDto 2");

                    var dellUserLBEGroup = new DelUserLBEGroupDto()
                    {
                        UserId = userId,
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
            if (value is DelUserLBEGroupDto c)
            {
                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.DeleteUserLBE)
                    + UMI3DSerializer.Write(c.UserId);
                return true;
            }

            bytable = null;
            return false;
        }
    }

    public class DelUserLBEGroupSerializersID : UMI3DSerializerModule
    {
        public bool? IsCountable<T>()
        {
            return typeof(T) == typeof(DelUserGroupOperationsDto) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {
            if (typeof(T) == typeof(DelUserGroupOperationsDto))
            {
                Debug.Log("REMY : ID Reading DelUserGroupOperationsDto 0");

                readable = UMI3DSerializer.TryRead(container, out uint Key);
                readable &= UMI3DSerializer.TryRead(container, out DelUserLBEGroupDto delUserLBEGroupDto);

                Debug.Log("REMY : ID Reading DelUserGroupOperationsDto 1 -> " + Key + " + " +readable);


                if (readable /*&& Key == UMI3DOperationKeys.DeleteUserLBE*/)
                {
                    Debug.Log("REMY : ID Reading DelUserGroupOperationsDto 2");

                    var delUserGroupOperationsDto = new DelUserGroupOperationsDto
                    {
                        delUserLBEGroupDto = delUserLBEGroupDto
                    };

                    readable = true;
                    result = (T)Convert.ChangeType(delUserGroupOperationsDto, typeof(T));
                    return true;
                }
            }

            result = default(T);
            readable = false;
            return false;
        }

        public bool Write<T>(T value, out Bytable bytable, params object[] parameters)
        {
            if (value is DelUserGroupOperationsDto s)
            {
                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.DeleteUserLBE)
                    + UMI3DSerializer.Write(s.delUserLBEGroupDto);
                return true;
            }

            bytable = null;
            return false;
        }
    }
}
