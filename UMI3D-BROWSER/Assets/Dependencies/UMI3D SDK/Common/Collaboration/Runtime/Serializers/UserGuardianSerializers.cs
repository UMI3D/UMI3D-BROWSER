
using System;
using UnityEngine;
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
            if (typeof(T) == typeof(UserGuardianDto))
            {
                // Afficher le type de l'objet T
                Debug.Log("Remi : Reading UserGuardianDto...");

                //readable = UMI3DSerializer.TryRead(container, out List<ARAnchorDto> anchorARList);

                List<ARAnchorDto> anchorARList = UMI3DSerializer.ReadList<ARAnchorDto>(container);
                float offsetGuardian = UMI3DSerializer.Read<float>(container);
                uint aR = UMI3DSerializer.Read<uint>(container);

                Debug.Log("Number of anchor DTOs read: " + anchorARList.Count);             

                if (anchorARList != null)
                {
                    Debug.Log("Remi : Guardian anchor");

                    var userguardian = new UserGuardianDto
                    {
                        anchorAR = anchorARList,
                        OffsetGuardian = offsetGuardian,
                        AR = aR
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

            if (value is UserGuardianDto c)
            {
                Debug.Log("Remi : anchor guardian  Write 2");
                Debug.Log("Remi : Offset Write -> " + c.OffsetGuardian);

                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.GuardianBrowserRequest)
                    + UMI3DSerializer.WriteCollection(c.anchorAR)
                    + UMI3DSerializer.Write(c.OffsetGuardian)
                    + UMI3DSerializer.Write(c.AR);
                return true;
            }

            bytable = null;
            return false;
        }
    }

    public class UserGuardianSerializersID : UMI3DSerializerModule
    {
        public bool? IsCountable<T>()
        {
            return typeof(T) == typeof(SendGuardianRequestDto) ? true : null;
        }

        public bool Read<T>(ByteContainer container, out bool readable, out T result)
        {

            if (typeof(T) == typeof(SendGuardianRequestDto))
            {
                Debug.Log("Remi ID : Reading SendGuardianRequestDto");

                readable = UMI3DSerializer.TryRead(container, out uint Key);

                readable &= UMI3DSerializer.TryRead(container, out UserGuardianDto userGuardianDto);

                if (readable && Key == UMI3DOperationKeys.GuardianBrowserRequest)
                {
                    var sendGuardianRequest = new SendGuardianRequestDto
                    {
                        guardianData = userGuardianDto
                    };

                    readable = true;
                    result = (T)Convert.ChangeType(sendGuardianRequest, typeof(T));
                    return true;
                }
            }

            result = default(T);
            readable = false;
            return false;
        }

        public bool Write<T>(T value, out Bytable bytable, params object[] parameters)
        {

            if (value is SendGuardianRequestDto s)
            {
                Debug.Log("<color=blue> Remi : Writing SendGuardianRequestDto </color>");
                Debug.Log("Remi : Offset Write 2 -> "+s.guardianData.OffsetGuardian);
                Debug.Log("Remi : AR Write  -> " + s.guardianData.AR);


                bytable = UMI3DSerializer.Write(UMI3DOperationKeys.GuardianBrowserRequest)
                    + UMI3DSerializer.Write(s.guardianData);
                return true;
            }

            bytable = null;
            return false;
        }
    }
}

