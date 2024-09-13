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
            return typeof(T) == typeof(SendLBEGroupRequestDTO) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {
            if (typeof(T) == typeof(SendLBEGroupRequestDTO))
            {
                readable = UMI3DSerializer.TryRead(container, out uint Key);

                uint groupId = UMI3DSerializer.Read<uint>(container);
                List<ulong> userAR = UMI3DSerializer.ReadList<ulong>(container);
                List<ulong> userVR = UMI3DSerializer.ReadList<ulong>(container);
                List<ARAnchorDto> anchorAR = UMI3DSerializer.ReadList<ARAnchorDto>(container);


                if (readable && Key == UMI3DOperationKeys.SetLBEGroupRequest)
                {
                    var lBEGroup = new SendLBEGroupRequestDTO()
                    {
                        GroupId = groupId,
                        UserAR = userAR,
                        UserVR = userVR,
                        anchorAR = anchorAR
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
            if (value is SendLBEGroupRequestDTO c)
            {
                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.SetLBEGroupRequest)
                    + UMI3DSerializer.Write(c.GroupId)
                    + UMI3DSerializer.WriteCollection(c.UserAR)
                    + UMI3DSerializer.WriteCollection(c.UserVR)
                    + UMI3DSerializer.WriteCollection(c.anchorAR);
                return true;
            }

            bytable = null;
            return false;
        }
    }
}
