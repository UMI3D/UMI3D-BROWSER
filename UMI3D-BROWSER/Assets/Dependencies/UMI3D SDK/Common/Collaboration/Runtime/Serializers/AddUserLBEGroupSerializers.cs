using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.lbe.description;
using System;

namespace umi3d.common.lbe
{
    public class AddUserLBEGroupSerializers : UMI3DSerializerModule
    {

        public bool? IsCountable<T>()
        {
            return typeof(T) == typeof(AddUserGroupOperationsDto) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {
            if (typeof(T) == typeof(AddUserGroupOperationsDto))
            {
                ulong userId = UMI3DSerializer.Read<ulong>(container);
                bool isUserAR = UMI3DSerializer.Read<bool>(container);
                readable = UMI3DSerializer.TryRead(container, out uint Key);

                if (readable)
                {
                    var addUserLBEGroup = new AddUserGroupOperationsDto()
                    {
                        UserId = userId,
                        IsUserAR = isUserAR,
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
            if (value is AddUserGroupOperationsDto c)
            {
                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.SetNewUserLBE)
                    + UMI3DSerializer.Write(c.UserId)
                    + UMI3DSerializer.Write(c.IsUserAR);
                return true;
            }

            bytable = null;
            return false;
        }
    }
}

