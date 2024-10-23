using UnityEngine;
using static umi3d.cdk.collaboration.UMI3DWorldControllerClient;


/// <summary>
/// Set umi3d.cdk.collaboration.UMI3DWorldControllerClient.formCompatibleVersions regarding the type of device.
/// </summary>
public class SetClientFormatCompatibleVersion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UMI3D_PC
        formCompatibleVersions = new() { Versions.version1, Versions.version2, Versions.version2_1, Versions.windows};
#else
        formCompatibleVersions = new() { Versions.version1, Versions.version2, Versions.version2_1, Versions.vr };
#endif
    }
}
