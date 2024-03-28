
using System;
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
            if(typeof(T) == typeof(UserGuardianDto))
            {
                readable = container.length > 0;

                if(readable)
                {
                    List<ARAnchorDto> anchor = UMI3DSerializer.ReadList<ARAnchorDto>(container);

                    var userguardian = new UserGuardianDto
                    {
                        anchorAR = anchor,
                    };
                    readable = true;
                    result = (T)Convert.ChangeType(userguardian, typeof(T));
                    return true;
                }
               
            }

            readable = false;
            result = default(T);
            return false;
        }

        public bool Write<T>(T value, out Bytable bytable, params object[] parameters)
        {
            if(value is UserGuardianDto c)
            {
                bytable = UMI3DSerializer.WriteCollection(c.anchorAR);
                return true;
            }
            bytable = null; 
            return false; 
        }
    }
}

